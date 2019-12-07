using System;
using System.Threading.Tasks;
using Clay.Constants;
using Clay.Data.Pagination;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.Models.InputModels.Admin;
using Clay.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clay.Controllers
{
    [Authorize(Roles = "User, Administrator")]
    public class UserController : ClayControllerBase
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserLockManager _userLockManager
            ;

        public UserController(UserManager<AppIdentityUser> userManager, IUnitOfWork unitOfWork, IUserLockManager userLockManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userLockManager = userLockManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyLocks(PagedModel pagedModel)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (pagedModel == null)
                pagedModel = new PagedModel();

            var userLocks = await _unitOfWork.UserLockRepository.SearchBy(pagedModel, ul => ul.UserId.Equals(loggedInUser.Id), ul => ul.Lock);

            var list = new PagedResult<Lock>();
            foreach (var userLock in userLocks.Results)
            {
                list.Results.Add(new Lock
                {
                    Id = userLock.LockId,
                    Name = userLock.Lock.Name,
                    IsLocked = userLock.Lock.IsLocked,
                    Place = userLock.Lock.Place
                });
            }

            list.CurrentPage = userLocks.CurrentPage;
            list.PageCount = userLocks.PageCount;
            list.PageSize = userLocks.PageSize;
            list.RowCount = userLocks.RowCount;

            return Ok(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyHistory(PagedModel pagedModel)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (pagedModel == null)
                pagedModel = new PagedModel();

            var results = await _unitOfWork.AttemptRepository.SearchBy(pagedModel, a => a.UserId.Equals(loggedInUser.Id));
            return Ok(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetLockHistory(GetLockHistoryModel model)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (!await CanUserAccess(loggedInUser, model.LockId))
                return Unauthorized();

            if (model.PagedModel == null)
                model.PagedModel = new PagedModel();

            return Ok(await _unitOfWork.AttemptRepository.SearchBy(model.PagedModel, a => a.LockId.Equals(model.LockId)));
        }

        [HttpPost]
        public async Task<IActionResult> Lock(LockActionModel model)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (loggedInUser == null)
                return Unauthorized();


            if (!await CanUserAccess(loggedInUser, model.LockId))
                return Unauthorized();

            var attempt = new Attempt
            {
                Action = Actions.LOCK,
                IsSuccessful = true,
                LockId = model.LockId,
                Time = DateTime.Now,
                UserId = loggedInUser.Id
            };

            var lockActionResult = await _unitOfWork.LockRepository.Lock(new Lock { Id = model.LockId });

            if (!lockActionResult)
            {
                attempt.IsSuccessful = false;
                await _unitOfWork.AttemptRepository.Add(attempt);
                await _unitOfWork.Save();
                return BadRequest(Task.FromResult(false));
            }

            await _unitOfWork.AttemptRepository.Add(attempt);
            await _unitOfWork.Save();
            return Ok(Task.FromResult(true));
        }

        [HttpPost]
        public async Task<IActionResult> UnLock(LockActionModel model)
        {
            var loggedInUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (loggedInUser == null)
                return Unauthorized();

            if (!await CanUserAccess(loggedInUser, model.LockId))
                return Unauthorized();

            var attempt = new Attempt
            {
                Action = Actions.UNLOCK,
                IsSuccessful = true,
                LockId = model.LockId,
                Time = DateTime.Now,
                UserId = loggedInUser.Id
            };

            var lockActionResult = await _unitOfWork.LockRepository.Unlock(new Lock { Id = model.LockId });

            if (!lockActionResult)
            {
                attempt.IsSuccessful = false;
                await _unitOfWork.AttemptRepository.Add(attempt);
                await _unitOfWork.Save();
                return BadRequest(Task.FromResult(false));
            }

            await _unitOfWork.AttemptRepository.Add(attempt);
            await _unitOfWork.Save();
            return Ok(Task.FromResult(true));
        }

        private async Task<bool> CanUserAccess(AppIdentityUser loggedInUser, Guid lockId)
        {
            return await _userLockManager.CanAccess(loggedInUser.Id, lockId);
        }
    }
}