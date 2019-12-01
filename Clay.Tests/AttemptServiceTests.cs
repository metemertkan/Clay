using System;
using System.Linq;
using Clay.Constants;
using Clay.Data;
using Clay.Data.Pagination;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;
using Clay.Services.Implementation;
using Moq;
using Xunit;

namespace Clay.Tests
{
    public class AttemptServiceTests
    {
        [Fact]
        public void Get_User_Attempts_Should_Return_User_Attemps()
        {
            //Arrange
            var pagedModel = new PagedModel();

            var mockAttemptRepository = new Mock<IAttemptRepository>();

            mockAttemptRepository.Setup(m => m.Attempts).Returns(new Attempt[] {
                new Attempt {Id = 1,Action = Actions.LOCK,IsSuccessful = true,LockId = Guid.NewGuid(),UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 2,Action = Actions.LOCK,IsSuccessful = true,LockId = Guid.NewGuid(),UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 3,Action = Actions.UNLOCK,IsSuccessful = false,LockId = Guid.NewGuid(),UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 4,Action = Actions.UNLOCK,IsSuccessful = false,LockId = Guid.NewGuid(),UserId = "321",Time = DateTime.Now}
            }.AsQueryable<Attempt>());

            //Act
            var target = new AttemptService(mockAttemptRepository.Object);
            var pagedResult = target.GetUserAttempts("123",pagedModel);


            //Assert
            Assert.Equal(3, pagedResult.Results.Count);
            Assert.Equal(1, pagedResult.Results.FirstOrDefault().Id);
        }

        [Fact]
        public void Get_Lock_Attempts_Should_Return_Lock_Attemps()
        {
            //Arrange
            var pagedModel = new PagedModel();
            var lockId = Guid.NewGuid();
            var mockAttemptRepository = new Mock<IAttemptRepository>();

            mockAttemptRepository.Setup(m => m.Attempts).Returns(new Attempt[] {
                new Attempt {Id = 1,Action = Actions.LOCK,IsSuccessful = true,LockId = lockId,UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 2,Action = Actions.LOCK,IsSuccessful = true,LockId = lockId,UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 3,Action = Actions.UNLOCK,IsSuccessful = false,LockId = Guid.NewGuid(),UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 4,Action = Actions.UNLOCK,IsSuccessful = false,LockId = Guid.NewGuid(),UserId = "321",Time = DateTime.Now}
            }.AsQueryable<Attempt>());

            //Act
            var target = new AttemptService(mockAttemptRepository.Object);
            var pagedResult = target.GetLockAttempts(lockId, pagedModel);


            //Assert
            Assert.Equal(2, pagedResult.Results.Count);
            Assert.Equal(1, pagedResult.Results.FirstOrDefault().Id);
        }

        [Fact]
        public void Get_All_Attemps_Returns_All_Attemps()
        {
            //Arrange
            var pagedModel = new PagedModel();
            var lockId = Guid.NewGuid();
            var mockAttemptRepository = new Mock<IAttemptRepository>();

            mockAttemptRepository.Setup(m => m.Attempts).Returns(new Attempt[] {
                new Attempt {Id = 1,Action = Actions.LOCK,IsSuccessful = true,LockId = lockId,UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 2,Action = Actions.LOCK,IsSuccessful = true,LockId = lockId,UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 3,Action = Actions.UNLOCK,IsSuccessful = false,LockId = Guid.NewGuid(),UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 4,Action = Actions.UNLOCK,IsSuccessful = false,LockId = Guid.NewGuid(),UserId = "321",Time = DateTime.Now}
            }.AsQueryable<Attempt>());

            //Act
            var target = new AttemptService(mockAttemptRepository.Object);
            var pagedResult = target.GetAttempts(pagedModel);


            //Assert
            Assert.Equal(4, pagedResult.Results.Count);
            Assert.Equal(1, pagedResult.Results.FirstOrDefault().Id);
        }

        [Fact]
        public void Create_Attempt_Should_Create_Attempt()
        {
            //Arrange
            var lockId = Guid.NewGuid();
            var testAttempt= new Attempt
            {
                Id = 5,
                Action = Actions.UNLOCK,
                LockId = lockId,
                UserId = "123",
                IsSuccessful = true,
                Time = DateTime.Now
            };
            var mockAttemptRepository = new Mock<IAttemptRepository>();

            mockAttemptRepository.Setup(m => m.Attempts).Returns(new Attempt[] {
                new Attempt {Id = 1,Action = Actions.LOCK,IsSuccessful = true,LockId = lockId,UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 2,Action = Actions.LOCK,IsSuccessful = true,LockId = lockId,UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 3,Action = Actions.UNLOCK,IsSuccessful = false,LockId = Guid.NewGuid(),UserId = "123",Time = DateTime.Now},
                new Attempt {Id = 4,Action = Actions.UNLOCK,IsSuccessful = false,LockId = Guid.NewGuid(),UserId = "321",Time = DateTime.Now}
            }.AsQueryable<Attempt>());

            //Act
            var target = new AttemptService(mockAttemptRepository.Object);
            

            try
            {
                target.CreateAttempt(testAttempt);
                Assert.True(true);
            }
            catch
            {
                Assert.True(false);
            }
        }
    }
}