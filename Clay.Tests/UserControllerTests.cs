using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Clay.Constants;
using Clay.Controllers;
using Clay.Data.Pagination;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.Models.InputModels.Admin;
using Clay.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Clay.Tests
{
    public class UserControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IUserLockManager> _userLockMock;
        private PagedModel _defaultPagedModel;
        private PagedResult<Attempt> _attemptList;
        private PagedResult<UserLock> _userLockList;
        private Mock<UserManager<AppIdentityUser>> _userMgr;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userLockMock = new Mock<IUserLockManager>();
            _defaultPagedModel = new PagedModel();

            var userStoreMock = Mock.Of<IUserStore<AppIdentityUser>>();
            _userMgr = new Mock<UserManager<AppIdentityUser>>(userStoreMock, null, null, null, null, null, null, null, null);
            var user = new AppIdentityUser() { Id = "f00", UserName = "f00", Email = "f00@example.com" };
            var tcs = new TaskCompletionSource<AppIdentityUser>();
            tcs.SetResult(user);
            _userMgr.Setup(x => x.FindByNameAsync("f00")).Returns(tcs.Task);

            _attemptList = new PagedResult<Attempt>
            {
                CurrentPage = 1,
                PageCount = 1,
                PageSize = 5,
                RowCount = 5,
                Results = new List<Attempt>
                {
                    new Attempt
                    {
                        Id = 1, UserId = "123", LockId = new Guid(), Action = Actions.LOCK, IsSuccessful = true,
                        Time = DateTime.Now
                    },
                    new Attempt
                    {
                        Id = 2, UserId = "1234", LockId = new Guid(), Action = Actions.UNLOCK, IsSuccessful = true,
                        Time = DateTime.Now
                    },
                    new Attempt
                    {
                        Id = 3, UserId = "1235", LockId = new Guid(), Action = Actions.UNLOCK, IsSuccessful = true,
                        Time = DateTime.Now
                    },
                    new Attempt
                    {
                        Id = 4, UserId = "1236", LockId = new Guid(), Action = Actions.LOCK, IsSuccessful = true,
                        Time = DateTime.Now
                    },
                    new Attempt
                    {
                        Id = 5, UserId = "1232", LockId = new Guid(), Action = Actions.LOCK, IsSuccessful = true,
                        Time = DateTime.Now
                    }
                }
            };

            _userLockList = new PagedResult<UserLock>
            {
                CurrentPage = 1,
                PageCount = 1,
                PageSize = 5,
                RowCount = 5,
                Results = new List<UserLock>
                {
                    new UserLock
                    {
                        LockId = new Guid(),
                        Lock = new Lock {Id = new Guid(), IsLocked = false, Name = "lock1", Place = "place1"},
                        UserId = "f00"
                    },
                    new UserLock
                    {
                        LockId = new Guid(),
                        Lock = new Lock {Id = new Guid(), IsLocked = false, Name = "lock2", Place = "place2"},
                        UserId = "f00"
                    },
                    new UserLock
                    {
                        LockId = new Guid(),
                        Lock = new Lock {Id = new Guid(), IsLocked = true, Name = "lock3", Place = "place3"},
                        UserId = "f00"
                    },
                    new UserLock
                    {
                        LockId = new Guid(),
                        Lock = new Lock {Id = new Guid(), IsLocked = false, Name = "lock4", Place = "place4"},
                        UserId = "f00"
                    },
                    new UserLock
                    {
                        LockId = new Guid(),
                        Lock = new Lock {Id = new Guid(), IsLocked = true, Name = "lock5", Place = "place5"},
                        UserId = "f00"
                    }
                }
            };
        }

        private void SetupUser(ClayControllerBase controller, string username)
        {
            var mockContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockContext.SetupGet(hc => hc.User.Claims).Returns(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name,"f00")
            });
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
        }

        [Test]
        public void GetMyLocks_Should_Return_User_Locks()
        {
            //Arrange

            _unitOfWorkMock.Setup(u =>
                    u.UserLockRepository.SearchBy(_defaultPagedModel, It.IsAny<Expression<Func<UserLock, bool>>>(), It.IsAny<Expression<Func<UserLock, object>>>()))
                .Returns(
                    async () =>
                    {
                        await Task.Yield();
                        return _userLockList;
                    });

            var target = new UserController(_unitOfWorkMock.Object, _userLockMock.Object);
            SetupUser(target, "f00");
            //Act

            var result = target.GetMyLocks(_defaultPagedModel).Result as OkObjectResult;
            if (result == null)
                Assert.IsTrue(false);
            var resultValue = result.Value as PagedResult<Lock>;
            //Assert
            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Results);
            Assert.AreEqual(5, resultValue.Results.Count);
            var firstLock = resultValue.Results.FirstOrDefault(l => l.Name == "lock1");
            Assert.NotNull(firstLock);
            Assert.AreEqual("place1", firstLock.Place);
        }

        [Test]
        public void GetMyHistory_Should_Return_History()
        {
            //Arrange
            _unitOfWorkMock.Setup(u =>
                    u.AttemptRepository.SearchBy(_defaultPagedModel, It.IsAny<Expression<Func<Attempt, bool>>>()))
                .Returns(
                    async () =>
                    {
                        await Task.Yield();
                        return _attemptList;
                    });
            var target = new UserController(_unitOfWorkMock.Object,_userLockMock.Object);
            SetupUser(target,"f00");
            //Act
            var result = target.GetMyHistory(_defaultPagedModel).Result as OkObjectResult;
            if (result == null)
                Assert.IsTrue(false);
            //Assert
            var resultValue = result.Value as PagedResult<Attempt>;
            //Assert
            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Results);
            Assert.AreEqual(5, resultValue.Results.Count);
            var firstAttempt = resultValue.Results.FirstOrDefault(a=>a.UserId.Equals("123"));
            Assert.NotNull(firstAttempt);
            Assert.AreEqual(Actions.LOCK, firstAttempt.Action);
        }

        [Test]
        public void GetLockHistory_Should_Return_History()
        {
            //Arrange
            _userLockMock.Setup(ul => ul.CanAccess(It.IsAny<string>(), It.IsAny<Guid>())).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });

            _unitOfWorkMock.Setup(u =>
                    u.AttemptRepository.SearchBy(_defaultPagedModel, It.IsAny<Expression<Func<Attempt, bool>>>()))
                .Returns(
                    async () =>
                    {
                        await Task.Yield();
                        return _attemptList;
                    });
            var target = new UserController(_unitOfWorkMock.Object, _userLockMock.Object);
            SetupUser(target, "f00");
            //Act
            //Assert
            var getLockHistoryModel = new GetLockHistoryModel {LockId = new Guid(), PagedModel = _defaultPagedModel};
            var result = target.GetLockHistory(getLockHistoryModel).Result as OkObjectResult;
            if (result == null)
                Assert.IsTrue(false);
            //Assert
            var resultValue = result.Value as PagedResult<Attempt>;
            //Assert
            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Results);
            Assert.AreEqual(5, resultValue.Results.Count);
            var firstAttempt = resultValue.Results.FirstOrDefault(a => a.UserId.Equals("123"));
            Assert.NotNull(firstAttempt);
            Assert.AreEqual(Actions.LOCK, firstAttempt.Action);
        }

        [Test]
        public void Lock_Should_Call_LockRepository_Lock_Ones()
        {
            //Arrange
            _userLockMock.Setup(ul => ul.CanAccess(It.IsAny<string>(), It.IsAny<Guid>())).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });

            var target = new UserController( _unitOfWorkMock.Object, _userLockMock.Object);
            SetupUser(target, "f00");

            _unitOfWorkMock.Setup(u => u.LockRepository.Lock(It.IsAny<Lock>())).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });

            _unitOfWorkMock.Setup(u => u.AttemptRepository.Add(It.IsAny<Attempt>()));
            _unitOfWorkMock.Setup(x => x.Save()).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });
            //Act
            var lockModel=new LockActionModel{LockId = new Guid()};
            var result = target.Lock(lockModel).Result as OkResult;
            
            //Assert
            if (result == null)
                Assert.IsTrue(false);

            _unitOfWorkMock.Verify(u=>u.LockRepository.Lock(It.IsAny<Lock>()),Times.Once);
            _unitOfWorkMock.Verify(u=>u.AttemptRepository.Add(It.IsAny<Attempt>()),Times.Once);
            _unitOfWorkMock.Verify(u=>u.Save(),Times.Once);
        }

        [Test]
        public void UnLock_Should_Call_LockRepository_UnLock_Ones()
        {
            //Arrange
            _userLockMock.Setup(ul => ul.CanAccess(It.IsAny<string>(), It.IsAny<Guid>())).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });
            
            var target = new UserController(_unitOfWorkMock.Object, _userLockMock.Object);
            SetupUser(target, "f00");

            _unitOfWorkMock.Setup(u => u.LockRepository.Unlock(It.IsAny<Lock>())).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });

            _unitOfWorkMock.Setup(u => u.AttemptRepository.Add(It.IsAny<Attempt>()));
            _unitOfWorkMock.Setup(x => x.Save()).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });
            //Act
            var lockModel = new LockActionModel { LockId = new Guid() };
            var result = target.UnLock(lockModel).Result as OkResult;

            //Assert
            if (result == null)
                Assert.IsTrue(false);

            _unitOfWorkMock.Verify(u => u.LockRepository.Unlock(It.IsAny<Lock>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.AttemptRepository.Add(It.IsAny<Attempt>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }
    }
}
