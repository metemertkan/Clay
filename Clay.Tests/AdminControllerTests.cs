using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Clay.Constants;
using Clay.Controllers;
using Clay.Data;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;

namespace Clay.Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public void Admin_Can_Get_All_Locks()
        {
            //Arrange
            var mockLockService = new Mock<ILockService>();
            var mockAttemptService = new Mock<IAttemptService>();
            var mockUserLockManager = new Mock<IUserLockManager>();
            var mockLogger = NullLogger<AdminController>.Instance;

            var pagedModel = new PagedModel();

            mockLockService.Setup(l => l.GetAll(pagedModel)).Returns(new PagedResult<Lock>
            {
                CurrentPage = 1,
                PageCount = 1,
                PageSize = 10,
                RowCount = 3,
                Results = new List<Lock>
                {
                    new Lock { Id = Guid.NewGuid(),Name = "lock1",IsLocked = false,Place = "entrance"},
                    new Lock { Id = Guid.NewGuid(),Name = "lock2",IsLocked = false,Place = "main"},
                    new Lock { Id = Guid.NewGuid(),Name = "lock3",IsLocked = false,Place = "hall"}
                }
            });

            //Act
            var target = new AdminController(mockLockService.Object, mockAttemptService.Object, mockLogger, mockUserLockManager.Object);

            var actionResult = target.GetLocks(pagedModel);
            var okResult = actionResult as OkObjectResult;
            var pagedResult = okResult.Value as PagedResult<Lock>;

            //Assert
            Assert.Equal(3, pagedResult.RowCount);
            Assert.Equal(pagedResult.Results.FirstOrDefault().Name, "lock1");
        }

        [Fact]
        public void Admin_Can_Get_Attempts()
        {
            var mockLockService = new Mock<ILockService>();
            var mockAttemptService = new Mock<IAttemptService>();
            var mockUserLockManager = new Mock<IUserLockManager>();
            var mockLogger = NullLogger<AdminController>.Instance;

            var pagedModel = new PagedModel();

            mockAttemptService.Setup(l => l.GetAttempts(pagedModel)).Returns(new PagedResult<Attempt>
            {
                CurrentPage = 1,
                PageCount = 1,
                PageSize = 10,
                RowCount = 3,
                Results = new List<Attempt>
                {
                    new Attempt
                    {
                        Id = 1, Action = Actions.LOCK, IsSuccessful = true, LockId = Guid.Empty, Time = DateTime.Now,
                        UserId = ""
                    },
                    new Attempt
                    {
                        Id = 2, Action = Actions.UNLOCK, IsSuccessful = true, LockId = Guid.Empty, Time = DateTime.Now,
                        UserId = ""
                    },
                    new Attempt
                    {
                        Id = 3, Action = Actions.LOCK, IsSuccessful = true, LockId = Guid.Empty, Time = DateTime.Now,
                        UserId = ""
                    },
                }
            });

            //Act
            var target = new AdminController(mockLockService.Object, mockAttemptService.Object, mockLogger, mockUserLockManager.Object);

            var actionResult = target.GetAllAttempts(pagedModel);
            var okResult = actionResult as OkObjectResult;
            var pagedResult = okResult.Value as PagedResult<Attempt>;

            //Assert
            Assert.Equal(3, pagedResult.RowCount);
            Assert.Equal(pagedResult.Results.FirstOrDefault().Action, Actions.LOCK);
            Assert.True(pagedResult.Results.FirstOrDefault().IsSuccessful);
        }
    }
}