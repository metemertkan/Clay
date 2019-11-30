using System;
using System.Collections.Generic;
using System.Linq;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;

namespace Clay.Services
{
    public class LockService
    {
        private readonly ILockRepository _lockRepository;

        public LockService(ILockRepository lockRepository)
        {
            _lockRepository = lockRepository;
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