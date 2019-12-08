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
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork, IUserLockManager userLockManager)
        {
            _unitOfWork = unitOfWork;
            _userLockManager = userLockManager;
        }

        [HttpGet]
        [PaginationCorrection(ParamName = Parameters.PAGEDMODEL)]
        [ServiceFilter(typeof(ExceptionFilter))]
        public async Task<IActionResult> GetLocks(PagedModel pagedModel)
        {
            var result = await _unitOfWork.LockRepository.GetAll(pagedModel);

            return Ok(result);
        }

        [HttpPost]
        [ValidateViewModel]
        [ServiceFilter(typeof(ExceptionFilter))]
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

            await _unitOfWork.Save();
            return Ok();
        }

        [HttpPost]
        [ValidateViewModel]
        [ServiceFilter(typeof(ExceptionFilter))]
        public async Task<IActionResult> AssignUserToLock(UserLockModel model)
        {
            await _userLockManager.Assign(model.UserId, model.LockId);
            await _unitOfWork.Save();
            return Ok();
        }

        [HttpPost]
        [ValidateViewModel]
        [ServiceFilter(typeof(ExceptionFilter))]
        public async Task<IActionResult> UnAssignUserFromLock(UserLockModel model)
        {
            await _userLockManager.UnAssign(model.UserId, model.LockId);
            await _unitOfWork.Save();
            return Ok();
        }

        [HttpGet]
        [PaginationCorrection(ParamName = Parameters.PAGEDMODEL)]
        [ServiceFilter(typeof(ExceptionFilter))]
        public async Task<IActionResult> GetAttempts(PagedModel pagedModel)
        {
            var result = await _unitOfWork.AttemptRepository.GetAll(pagedModel);
            return Ok(result);
        }
    }
}