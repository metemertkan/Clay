using System;
using System.Collections.Generic;
using Clay.Models.Domain;

namespace Clay.Services.Interfaces
{
    public interface ILockService
    {
        List<Lock> GetAll();
        Lock GetById(Guid id);
        Lock GetByName(string name);
        List<Lock> GetByUserId(string userId);
        void SaveLock(Lock lockModel);
        void Lock(Guid id);
        void UnLock(Guid id);
    }
}