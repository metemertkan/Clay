using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clay.Constants;
using Clay.Controllers;
using Clay.Data.Pagination;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.Models.InputModels.Admin;
using Clay.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Clay.Tests
{
    public class AdminControllerTests
    {
        private Mock<ILogger<AdminController>> _logger;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IUserLockManager> _userLockMock;
        private PagedModel _defaultPagedModel;
        private PagedResult<Lock> _lockList;
        private PagedResult<Attempt> _attemptList;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<AdminController>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userLockMock = new Mock<IUserLockManager>();
            _defaultPagedModel = new PagedModel();

            _lockList = new PagedResult<Lock>
            {
                CurrentPage = 1,
                PageCount = 1,
                PageSize = 5,
                RowCount = 5,
                Results = new List<Lock>
                {
                    new Lock{Id = new Guid(),Name = "lock1",Place = "place1",IsLocked = false},
                    new Lock{Id = new Guid(),Name = "lock2",Place = "place2",IsLocked = false},
                    new Lock{Id = new Guid(),Name = "lock3",Place = "place3",IsLocked = true},
                    new Lock{Id = new Guid(),Name = "lock4",Place = "place4",IsLocked = true},
                    new Lock{Id = new Guid(),Name = "lock5",Place = "place5",IsLocked = false},
                }
            };

            _attemptList = new PagedResult<Attempt>
            {
                CurrentPage = 1,
                PageCount = 1,
                PageSize = 5,
                RowCount = 5,
                Results = new List<Attempt>
                {
                    new Attempt{Id = 1,UserId = "123",LockId = new Guid(),Action = Actions.LOCK,IsSuccessful = true, Time = DateTime.Now},
                    new Attempt{Id = 2,UserId = "1234",LockId = new Guid(),Action = Actions.UNLOCK,IsSuccessful = true, Time = DateTime.Now},
                    new Attempt{Id = 3,UserId = "1235",LockId = new Guid(),Action = Actions.UNLOCK,IsSuccessful = true, Time = DateTime.Now},
                    new Attempt{Id = 4,UserId = "1236",LockId = new Guid(),Action = Actions.LOCK,IsSuccessful = true, Time = DateTime.Now},
                    new Attempt{Id = 5,UserId = "1232",LockId = new Guid(),Action = Actions.LOCK,IsSuccessful = true, Time = DateTime.Now}
                }
            };
        }

        [Test]
        public void GetLocks_Should_Return_Locks()
        {
            //Arrange
            _unitOfWorkMock.Setup(x => x.LockRepository.GetAll(_defaultPagedModel)).Returns(async () =>
            {
                await Task.Yield();
                return _lockList;
            });
            //Act
            var target = new AdminController(_logger.Object, _unitOfWorkMock.Object, _userLockMock.Object);
            var okResult = target.GetLocks(_defaultPagedModel).Result as OkObjectResult;
            if (okResult == null)
                Assert.IsTrue(false);
            var response = okResult.Value as PagedResult<Lock>;
            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Results);
            Assert.AreEqual(5, response.Results.Count);
            var firstLock = response.Results.FirstOrDefault(l => l.Name.Equals("lock1"));
            Assert.NotNull(firstLock);
            Assert.AreEqual("place1", firstLock.Place);

        }

        [Test]
        public void Valid_SaveLock_Should_Call_Add_When_No_Id_Provided()
        {
            //Arrange
            var newLock = new Lock
            {
                Name = "testLock",
                Place = "testPlace",
                IsLocked = false
            };
            _unitOfWorkMock.Setup(x => x.LockRepository.Add(newLock)).Returns(async () => { return true; });

            //Act
            var target = new AdminController(_logger.Object, _unitOfWorkMock.Object, _userLockMock.Object);

            target.SaveLock(newLock).Wait();

            //Assert
            _unitOfWorkMock.Verify(u => u.LockRepository.Add(newLock), Times.Once);
        }

        [Test]
        public void Valid_SaveLock_Should_Call_Update_When_Id_Provided()
        {
            //Arrange
            var newLock = new Lock
            {
                Id = Guid.NewGuid(),
                Name = "testLock",
                Place = "testPlace",
                IsLocked = false
            };
            _unitOfWorkMock.Setup(x => x.LockRepository.Update(newLock)).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });
            _unitOfWorkMock.Setup(x => x.Save()).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });

            //Act
            var target = new AdminController(_logger.Object, _unitOfWorkMock.Object, _userLockMock.Object);
            target.SaveLock(newLock);

            //Assert
            _unitOfWorkMock.Verify(u => u.LockRepository.Update(newLock), Times.Once);
        }

        [Test]
        public void Valid_AssignUserToLock_Should_Call_Assign()
        {
            //Arrange
            var userLockModel = new UserLockModel
            {
                UserId = "123",
                LockId = new Guid()
            };
            _userLockMock.Setup(x => x.Assign(userLockModel.UserId, userLockModel.LockId));
            _unitOfWorkMock.Setup(x => x.Save()).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });

            //Act
            var target = new AdminController(_logger.Object, _unitOfWorkMock.Object, _userLockMock.Object);
            var result = target.AssignUserToLock(userLockModel).Result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            _userLockMock.Verify(ul => ul.Assign(userLockModel.UserId, userLockModel.LockId), Times.Once);
        }

        [Test]
        public void Valid_UnAssignUserToLock_Should_Call_UnAssign()
        {
            //Arrange
            var userLockModel = new UserLockModel
            {
                UserId = "123",
                LockId = Guid.NewGuid()
            };
            _userLockMock.Setup(x => x.Assign(userLockModel.UserId, userLockModel.LockId));
            _unitOfWorkMock.Setup(x => x.Save()).Returns(async () =>
            {
                await Task.Yield();
                return true;
            });

            //Act
            var target = new AdminController(_logger.Object, _unitOfWorkMock.Object, _userLockMock.Object);
            var result = target.UnAssignUserFromLock(userLockModel).Result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            _userLockMock.Verify(ul => ul.UnAssign(userLockModel.UserId, userLockModel.LockId), Times.Once);
        }

        [Test]
        public void GetAttempts_Should_Return_Attempts()
        {
            //Arrange
            _unitOfWorkMock.Setup(x => x.AttemptRepository.GetAll(_defaultPagedModel)).Returns(async () =>
            {
                await Task.Yield();
                return _attemptList;
            });

            //Act
            var target = new AdminController(_logger.Object, _unitOfWorkMock.Object, _userLockMock.Object);
            var okResult = target.GetAttempts(_defaultPagedModel).Result as OkObjectResult;
            if (okResult == null)
                Assert.IsTrue(false);
            var response = okResult.Value as PagedResult<Attempt>;
            //Assert
            Assert.NotNull(response);
            Assert.AreEqual(5, response.Results.Count);
            Assert.AreEqual(Actions.LOCK, response.Results.FirstOrDefault(l => l.Id.Equals(1)).Action);
        }
    }
}