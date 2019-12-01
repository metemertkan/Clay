using System;
using System.Collections.Generic;
using System.Linq;
using Clay.Data;
using Clay.Data.Pagination;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;
using Clay.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clay.Services.Implementation
{
    public class LockService : ILockService
    {
        private readonly ILockRepository _lockRepository;
        private readonly IUserLockRepository _userLockRepository;
        public LockService(ILockRepository lockRepository, IUserLockRepository userLockRepository)
        {
            _lockRepository = lockRepository;
            _userLockRepository = userLockRepository;
        }

        public PagedResult<Lock> GetAll(PagedModel pagedModel)
        {
            return _lockRepository.Locks.GetPaged(pagedModel);
        }

        public Lock GetById(Guid id)
        {
            return _lockRepository.Locks.FirstOrDefault(l => l.Id == id);
        }
        public Lock GetByName(string name)
        {
            return _lockRepository.Locks.FirstOrDefault(l => string.Equals(l.Name, name));
        }

        public PagedResult<Lock> GetByUserId(string userId, PagedModel pagedModel)
        {
            var userLocks = _userLockRepository.UserLocks.Where(ul => ul.UserId.Equals(userId)).Include(ul => ul.Lock).GetPaged(pagedModel);

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

            return list;
        }

        public void SaveLock(Lock lockModel)
        {
            _lockRepository.SaveLock(lockModel);
        }

        public void Lock(Guid id)
        {
            Action(id, true);
        }

        public void UnLock(Guid id)
        {
            Action(id, false);
        }

        private void Action(Guid id, bool @lock)
        {

            var foundLock = _lockRepository.Locks.FirstOrDefault(l => l.Id.Equals(id));
            if (foundLock == null)
                throw new Exception("Lock not found!");

            if (foundLock.IsLocked == @lock)
                throw new Exception("Lock is already in that state");

            foundLock.IsLocked = @lock;
            _lockRepository.SaveLock(foundLock);
        }
    }
}