using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Clay.Constants;
using Clay.Data.Pagination;
using Clay.Filters;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserLockManager _userLockManager
            ;

        public UserController(IUnitOfWork unitOfWork, IUserLockManager userLockManager)
        {
            _unitOfWork = unitOfWork;
            _userLockManager = userLockManager;
        }

        [HttpGet]
        [PaginationCorrection(ParamName = Parameters.PAGEDMODEL)]
        public async Task<IActionResult> GetMyLocks(PagedModel pagedModel)
        {
            var userLocks = await _unitOfWork.UserLockRepository.SearchBy(pagedModel, ul => ul.UserId.Equals(GeLogedinUserId()), ul => ul.Lock);

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
        [PaginationCorrection(ParamName = Parameters.PAGEDMODEL)]
        public async Task<IActionResult> GetMyHistory(PagedModel pagedModel)
        {
            if (pagedModel == null)
                pagedModel = new PagedModel();

            var results = await _unitOfWork.AttemptRepository.SearchBy(pagedModel, a => a.UserId.Equals(GeLogedinUserId()));
            return Ok(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetLockHistory(GetLockHistoryModel model)
        {
            if (!await CanUserAccess(GeLogedinUserId(), model.LockId))
                return Unauthorized();

            if (model.PagedModel == null)
                model.PagedModel = new PagedModel();

            return Ok(await _unitOfWork.AttemptRepository.SearchBy(model.PagedModel, a => a.LockId.Equals(model.LockId)));
        }

        [HttpPost]
        public async Task<IActionResult> Lock(LockActionModel model)
        {
            var loggedInUserId = GeLogedinUserId();

            if (!await CanUserAccess(loggedInUserId, model.LockId))
                return Unauthorized();

            var attempt = new Attempt
            {
                Action = Actions.LOCK,
                IsSuccessful = true,
                LockId = model.LockId,
                Time = DateTime.Now,
                UserId = loggedInUserId
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
            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> UnLock(LockActionModel model)
        {
            var loggedInUserId = GeLogedinUserId();

            if (!await CanUserAccess(loggedInUserId, model.LockId))
                return Unauthorized();

            var attempt = new Attempt
            {
                Action = Actions.UNLOCK,
                IsSuccessful = true,
                LockId = model.LockId,
                Time = DateTime.Now,
                UserId = loggedInUserId
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
            return Ok(true);
        }

        private async Task<bool> CanUserAccess(string userId, Guid lockId)
        {
            return await _userLockManager.CanAccess(userId, lockId);
        }

        private string GeLogedinUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
        }
    }
}