using System;
using System.Collections.Generic;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Clay.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ILockService _lockService;
        private readonly IUserLockService _userLockService;
        private readonly IAttemptService _attemptService;
        private readonly IUserLockManager _userLockManager;
        private readonly ILogger<AdminController> _log;

        public AdminController(ILockService lockService, IUserLockService userLockService, IAttemptService attemptService, ILogger<AdminController> log)
        {
            _lockService = lockService;
            _userLockService = userLockService;
            _attemptService = attemptService;
            _log = log;
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
            try
            {
                _userLockManager.Assign(userId, lockId);
            }
            catch (Exception e)
            {
                _log.LogError($"Something went wrong: {e.Message}");
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult UnAssignUserFromLock(string userId, Guid lockId)
        {
            try
            {
                _userLockManager.UnAssign(userId,lockId);
            }
            catch (Exception e)
            {
                _log.LogError($"Something went wrong: {e.Message}");
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult GetAllAttempts()
        {
            return Ok(_attemptService.GetAttempts());
        }
    }
}