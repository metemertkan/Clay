using System;
using System.Collections.Generic;
using Clay.Constants;
using Clay.Models.Domain;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clay.Controllers
{
    [Authorize(Roles = "User,Administrator")]
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

            if (loggedInUser == null)
                return Unauthorized();

            return Json(_lockService.GetByUserId(loggedInUser.Id));
        }

        [HttpGet]
        public IActionResult GetMyHistory()
        {
            var loggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;

            if (loggedInUser == null)
                return Unauthorized();

            return Json(_attemptService.GetUserAttempts(loggedInUser.Id));
        }

        [HttpGet]
        public IActionResult GetLockHistory(Guid lockId)
        {
            var loggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (!CanUserAccess(loggedInUser, lockId))
                return Unauthorized();
            return Json(_attemptService.GetLockAttempts(lockId));
        }

        [HttpPost]
        public IActionResult Lock(Guid lockId)
        {
            var loggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (!CanUserAccess(loggedInUser, lockId))
                return Unauthorized();

            var attempt = new Attempt
            {
                Action = Actions.LOCK,
                IsSuccessful = true,
                LockId = lockId,
                Time = DateTime.Now,
                UserId = loggedInUser.Id
            };

            try
            {
                _lockService.Lock(lockId);
                _attemptService.CreateAttempt(attempt);
            }
            catch (Exception e)
            {
                attempt.IsSuccessful = false;
                _attemptService.CreateAttempt(attempt);
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult UnLock(Guid lockId)
        {
            var loggedInUser = _userManager.GetUserAsync(HttpContext.User).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (!CanUserAccess(loggedInUser, lockId))
                return Unauthorized();

            var attempt = new Attempt
            {
                Action = Actions.UNLOCK,
                IsSuccessful = true,
                LockId = lockId,
                Time = DateTime.Now,
                UserId = loggedInUser.Id
            };

            try
            {
                _lockService.UnLock(lockId);
                _attemptService.CreateAttempt(attempt);
            }
            catch (Exception e)
            {
                attempt.IsSuccessful = false;
                _attemptService.CreateAttempt(attempt);
                return BadRequest(e.Message);
            }

            return Ok();
        }

        private bool CanUserAccess(AppIdentityUser loggedInUser, Guid lockId)
        {
            return _userLockService.CanUserAccess(loggedInUser.Id, lockId);
        }
    }
}