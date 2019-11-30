using System;
using System.Collections.Generic;
using Clay.Models.Domain;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clay.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ILockService _lockService;
        private readonly IUserLockService _userLockService;
        private readonly IAttemptService _attemptService;

        public AdminController(ILockService lockService, IUserLockService userLockService, IAttemptService attemptService)
        {
            _lockService = lockService;
            _userLockService = userLockService;
            _attemptService = attemptService;
        }

        [HttpGet]
        public List<Lock> GetLocks()
        {
            return _lockService.GetAll();
        }

        [HttpPost]
        public IActionResult SaveLock(Lock lockModel)
        {
            _lockService.SaveLock(lockModel);
            return Ok();
        }

        [HttpPost]
        public IActionResult AssignUserToLock(string userId, Guid lockId)
        {
            _userLockService.SaveUserLock(userId, lockId);
            return Ok();
        }

        [HttpPost]
        public IActionResult UnAssignUserFromLock(string userId, Guid lockId)
        {
            _userLockService.RemoveUserLock(userId, lockId);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetAllAttempts()
        {
            return Ok(_attemptService.GetAttempts());
        }
    }
}