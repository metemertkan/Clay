using System;
using System.Collections.Generic;
using Clay.Models.Domain;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clay.Controllers
{
    //[Authorize(Roles = "User,Administrator")]
    public class UserController : Controller
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly ILockService _lockService;
        private readonly IAttemptService _attemptService;
        private readonly IUserLockService _userLockService;
        public UserController(UserManager<AppIdentityUser> userManager, ILockService lockService, IAttemptService attemptService, IUserLockService userLockService)
        {
            _userManager = userManager;
            _lockService = lockService;
            _attemptService = attemptService;
            _userLockService = userLockService;
        }

        [HttpGet]
        public IActionResult GetMyLocks()
        {
            var loggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;
            return Json(_lockService.GetByUserId(loggedInUser.Id));
        }

        [HttpGet]
        public IActionResult GetMyHistory()
        {
            var loggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;
            return Json(_attemptService.GetUserAttempts(loggedInUser.Id));
        }

        [HttpGet]
        public IActionResult GetLockHistory(Guid lockId)
        {
            if (!CanUserAccess(lockId))
                return Unauthorized();
            return Json(_attemptService.GetLockAttempts(lockId));
        }

        [HttpPost]
        public IActionResult Lock(Guid lockId)
        {
            if (!CanUserAccess(lockId))
                return Unauthorized();
            _lockService.Lock(lockId);
            return Ok();
        }

        [HttpPost]
        public IActionResult UnLock(Guid lockId)
        {
            if (!CanUserAccess(lockId))
                return Unauthorized();
            _lockService.UnLock(lockId);
            return Ok();
        }

        private bool CanUserAccess(Guid lockId)
        {
            var loggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;
            return _userLockService.CanUserAccess(loggedInUser.Id, lockId);
        }
    }
}