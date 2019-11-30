﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Lock> GetAll()
        {
            return _lockRepository.Locks.ToList();
        }

        public Lock GetById(Guid id)
        {
            return _lockRepository.Locks.FirstOrDefault(l => l.Id == id);
        }
        public Lock GetByName(string name)
        {
            return _lockRepository.Locks.FirstOrDefault(l => string.Equals(l.Name, name));
        }

        public List<Lock> GetByUserId(string userId)
        {
            var userLocks = _userLockRepository.UserLocks.Where(ul => ul.UserId.Equals(userId)).Include(ul=>ul.Lock).ToList();

            var list = new List<Lock>();
            foreach (var userLock in userLocks)
            {
                list.Add(new Lock
                {
                    Id = userLock.LockId,
                    Name = userLock.Lock.Name,
                    IsLocked = userLock.Lock.IsLocked,
                    Place = userLock.Lock.Place
                });
            }

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
                return;

            if (foundLock.IsLocked == @lock)
            {
                //return info already same
            }
            else
            {
                foundLock.IsLocked = @lock;
                _lockRepository.SaveLock(foundLock);
            }
        }
    }
}