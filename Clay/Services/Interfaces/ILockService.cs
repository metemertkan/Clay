using System;
using System.Collections.Generic;
using Clay.Data;
using Clay.Data.Pagination;
using Clay.Models.Domain;

namespace Clay.Services.Interfaces
{
    public interface ILockService
    {
        PagedResult<Lock> GetAll(PagedModel pagedModel);
        Lock GetById(Guid id);
        Lock GetByName(string name);
        PagedResult<Lock> GetByUserId(string userId,PagedModel pagedModel);
        void SaveLock(Lock lockModel);
        void Lock(Guid id);
        void UnLock(Guid id);
    }
}