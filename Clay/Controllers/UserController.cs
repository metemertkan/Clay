using System;
using Clay.Constants;
using Clay.Data.Pagination;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clay.Controllers
{
    [Authorize(Roles = "User, Administrator")]
    public class UserController : ClayControllerBase
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly ILockService _lockService;
        private readonly IAttemptService _attemptService;
        private readonly IUserLockManager _userLockManager;

        public UserController(UserManager<AppIdentityUser> userManager, ILockService lockService, IAttemptService attemptService, IUserLockService userLockService, IUserLockManager userLockManager)
        {
            _userManager = userManager;
            _lockService = lockService;
            _attemptService = attemptService;
            _userLockManager = userLockManager;
        }

        [HttpGet]
        public IActionResult GetMyLocks([FromBody]PagedModel pagedModel)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (pagedModel == null)
                pagedModel = new PagedModel();

            return Ok(_lockService.GetByUserId(loggedInUser.Id, pagedModel));
        }

        [HttpGet]
        public IActionResult GetMyHistory(PagedModel pagedModel)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (pagedModel == null)
                pagedModel = new PagedModel();

            return Ok(_attemptService.GetUserAttempts(loggedInUser.Id, pagedModel));
        }

        [HttpGet]
        public IActionResult GetLockHistory(Guid lockId, PagedModel pagedModel)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (!CanUserAccess(loggedInUser, lockId))
                return Unauthorized();

            if (pagedModel == null)
                pagedModel = new PagedModel();

            return Ok(_attemptService.GetLockAttempts(lockId, pagedModel));
        }

        [HttpPost]
        public IActionResult Lock(Guid lockId)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

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
            }
            catch (Exception e)
            {
                attempt.IsSuccessful = false;
                _attemptService.CreateAttempt(attempt);
                return BadRequest(e.Message);
            }

            _attemptService.CreateAttempt(attempt);

            return Ok();
        }

        [HttpPost]
        public IActionResult UnLock(Guid lockId)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

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
            }
            catch (Exception e)
            {
                attempt.IsSuccessful = false;
                _attemptService.CreateAttempt(attempt);
                return BadRequest(e.Message);
            }
            _attemptService.CreateAttempt(attempt);

            return Ok();
        }

        private bool CanUserAccess(AppIdentityUser loggedInUser, Guid lockId)
        {
            return _userLockManager.CanAccess(loggedInUser.Id, lockId);
        }
    }
}