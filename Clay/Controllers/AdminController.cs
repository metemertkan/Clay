using System;
using System.Threading.Tasks;
using Clay.Constants;
using Clay.Data.Pagination;
using Clay.Filters;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.Models.InputModels.Admin;
using Clay.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Clay.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : ClayControllerBase
    {
        private readonly IUserLockManager _userLockManager;
        private readonly ILogger<AdminController> _log;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(ILogger<AdminController> log, IUnitOfWork unitOfWork, IUserLockManager userLockManager)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _userLockManager = userLockManager;
        }

        [HttpGet]
        [PaginationCorrection(ParamName = Parameters.PAGEDMODEL)]
        public async Task<IActionResult> GetLocks(PagedModel pagedModel)
        {
            var result = await _unitOfWork.LockRepository.GetAll(pagedModel);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLock(Lock lockModel)
        {
            if (lockModel.Id == Guid.Empty)
            {
                await _unitOfWork.LockRepository.Add(lockModel);
            }
            else
            {
                await _unitOfWork.LockRepository.Update(lockModel);
            }
            
            return Ok(await _unitOfWork.Save());
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserToLock(UserLockModel model)
        {
            try
            {
                await _userLockManager.Assign(model.UserId, model.LockId);
            }
            catch (Exception e)
            {
                _log.LogError($"Something went wrong: {e.Message}");
                return StatusCode(500);
            }
            return Ok(await _unitOfWork.Save());
        }

        [HttpPost]
        public async Task<IActionResult> UnAssignUserFromLock(UserLockModel model)
        {
            try
            {
                await _userLockManager.UnAssign(model.UserId, model.LockId);
            }
            catch (Exception e)
            {
                _log.LogError($"Something went wrong: {e.Message}");
                return StatusCode(500);
            }
            return Ok(await _unitOfWork.Save());
        }

        [HttpGet]
        [PaginationCorrection(ParamName = Parameters.PAGEDMODEL)]
        public async Task<IActionResult> GetAttempts(PagedModel pagedModel)
        {
            var result = await _unitOfWork.AttemptRepository.GetAll(pagedModel);

            return Ok(result);
        }
    }
}