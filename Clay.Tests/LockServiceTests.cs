
using System;
using System.Linq;
using Clay.Data;
using Clay.Data.Pagination;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;
using Clay.Services.Implementation;
using Moq;
using Xunit;

namespace Clay.Tests
{
    public class LockServiceTests
    {/*
        [Fact]
        public void Get_All_Should_Return_All_Locks()
        {
            //Arrange
            var pagedModel = new PagedModel();
            var mockLockRepository = new Mock<ILockRepository>();
            var mockUserLockRepository = new Mock<IUserLockRepository>();

            mockLockRepository.Setup(m => m.Locks).Returns(new Lock[] {
                new Lock{Id = Guid.NewGuid(),IsLocked = false,Name ="lock1",Place = "place1"},
                new Lock{Id = Guid.NewGuid(),IsLocked = true,Name ="lock2",Place = "place2"},
                new Lock{Id = Guid.NewGuid(),IsLocked = false,Name ="lock3",Place = "place3"},
                new Lock{Id = Guid.NewGuid(),IsLocked = true,Name ="lock4",Place = "place4"}
            }.AsQueryable<Lock>());

            //Act
            var target = new LockService(mockLockRepository.Object, mockUserLockRepository.Object);
            var pagedResult = target.GetAll(pagedModel);

            //Assert
            Assert.Equal(4, pagedResult.Results.Count);
            Assert.Equal("place1", pagedResult.Results.FirstOrDefault().Place);
        }

        [Fact]
        public void Get_ById_Should_Return_One_Lock()
        {
            //Arrange
            var testLock = new Lock
            {
                Id = Guid.NewGuid(),
                Name = "test",
                IsLocked = true,
                Place = "test"
            };
            var mockLockRepository = new Mock<ILockRepository>();
            var mockUserLockRepository = new Mock<IUserLockRepository>();

            mockLockRepository.Setup(m => m.Locks).Returns(new Lock[] {
                new Lock{Id = Guid.NewGuid(),IsLocked = false,Name ="lock1",Place = "place1"},
                new Lock{Id = Guid.NewGuid(),IsLocked = true,Name ="lock2",Place = "place2"},
                new Lock{Id = Guid.NewGuid(),IsLocked = false,Name ="lock3",Place = "place3"},
                new Lock{Id = Guid.NewGuid(),IsLocked = true,Name ="lock4",Place = "place4"},
                testLock
            }.AsQueryable<Lock>());

            //Act
            var target = new LockService(mockLockRepository.Object, mockUserLockRepository.Object);
            var result = target.GetById(testLock.Id);

            //Assert
            Assert.Equal(testLock.Id, result.Id);
            Assert.Equal(testLock.Name, result.Name);
        }

        [Fact]
        public void Get_ByName_Should_Return_One_Lock()
        {
            //Arrange
            var testLock = new Lock
            {
                Id = Guid.NewGuid(),
                Name = "test",
                IsLocked = true,
                Place = "test"
            };
            var mockLockRepository = new Mock<ILockRepository>();
            var mockUserLockRepository = new Mock<IUserLockRepository>();

            mockLockRepository.Setup(m => m.Locks).Returns(new Lock[] {
                new Lock{Id = Guid.NewGuid(),IsLocked = false,Name ="lock1",Place = "place1"},
                new Lock{Id = Guid.NewGuid(),IsLocked = true,Name ="lock2",Place = "place2"},
                new Lock{Id = Guid.NewGuid(),IsLocked = false,Name ="lock3",Place = "place3"},
                new Lock{Id = Guid.NewGuid(),IsLocked = true,Name ="lock4",Place = "place4"},
                testLock
            }.AsQueryable<Lock>());

            //Act
            var target = new LockService(mockLockRepository.Object, mockUserLockRepository.Object);
            var result = target.GetByName(testLock.Name);

            //Assert
            Assert.Equal(testLock.Id, result.Id);
            Assert.Equal(testLock.Name, result.Name);
        }

        //[Fact]
        //public void Get_ByUserId_Should_Return_Users_Locks()
        //{
        //    //Arrange
        //    var testLock = new Lock
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "test",
        //        IsLocked = true,
        //        Place = "test"
        //    };
        //    var pagedModel= new PagedModel();
        //    var exampleUserId = Guid.NewGuid();
        //    var mockLockRepository = new Mock<ILockRepository>();
        //    var mockUserLockRepository = new Mock<IUserLockRepository>();

        //    mockLockRepository.Setup(m => m.Locks).Returns(new Lock[] {
        //        new Lock{Id = Guid.NewGuid(),IsLocked = false,Name ="lock1",Place = "place1"},
        //        new Lock{Id = Guid.NewGuid(),IsLocked = true,Name ="lock2",Place = "place2"},
        //        new Lock{Id = Guid.NewGuid(),IsLocked = false,Name ="lock3",Place = "place3"},
        //        new Lock{Id = Guid.NewGuid(),IsLocked = true,Name ="lock4",Place = "place4"},
        //        testLock
        //    }.AsQueryable<Lock>());

        //    mockUserLockRepository.Setup(m => m.UserLocks).Returns(new UserLock[]
        //    {
        //        new UserLock
        //        {
        //            LockId = testLock.Id,
        //            UserId = exampleUserId.ToString()
        //        }, 
        //    }.AsQueryable<UserLock>());


        //    //Act
        //    var target = new LockService(mockLockRepository.Object, mockUserLockRepository.Object);
        //    var result = target.GetByUserId(exampleUserId.ToString(),pagedModel);

        //    //Assert
        //    Assert.Equal(1,result.Results.Count);
        //    Assert.Equal("test",result.Results.FirstOrDefault().Name);
        //}*/
    }
}