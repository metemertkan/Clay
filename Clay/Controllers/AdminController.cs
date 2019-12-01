using System;
using System.Collections.Generic;
using Clay.Constants;
using Clay.Data;
using Clay.Data.Pagination;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Clay.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ILockService _lockService;
        private readonly IAttemptService _attemptService;
        private readonly IUserLockManager _userLockManager;
        private readonly ILogger<AdminController> _log;
        private readonly IMemoryCache _cache;

        public AdminController(ILockService lockService, IAttemptService attemptService, ILogger<AdminController> log, IUserLockManager userLockManager, IMemoryCache cache)
        {
            _lockService = lockService;
            _attemptService = attemptService;
            _log = log;
            _userLockManager = userLockManager;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult GetLocks(PagedModel pagedModel)
        {
            if (pagedModel == null)
                pagedModel = new PagedModel();

            return Ok(_lockService.GetAll(pagedModel));
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
                _userLockManager.UnAssign(userId, lockId);
            }
            catch (Exception e)
            {
                _log.LogError($"Something went wrong: {e.Message}");
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult GetAllAttempts(PagedModel pagedModel)
        {
            if (pagedModel == null)
                pagedModel = new PagedModel();

            var cacheKey =
                CacheKeys.ADMINCONTROLLER + CacheKeys.DELIMITER + pagedModel.Page + CacheKeys.DELIMITER + pagedModel.PageSize;

            if (_cache.TryGetValue(cacheKey,
                out PagedResult<Attempt> attempts))
                return Ok(attempts);

            var result = _attemptService.GetAttempts(pagedModel);

            _cache.Set(cacheKey, result);

            return Ok(result);
        }
    }
}