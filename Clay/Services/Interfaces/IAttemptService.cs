using System;
using System.Collections.Generic;
using Clay.Data;
using Clay.Data.Pagination;
using Clay.Models.Domain;

namespace Clay.Services.Interfaces
{
    public interface IAttemptService
    {
        PagedResult<Attempt> GetUserAttempts(string userId, PagedModel pagedModel);
        PagedResult<Attempt> GetLockAttempts(Guid lockId, PagedModel pagedModel);
        PagedResult<Attempt> GetAttempts(PagedModel pagedModel);
        void CreateAttempt(Attempt attempt);

    }
}