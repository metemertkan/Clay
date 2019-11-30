using System;
using System.Collections.Generic;
using System.Linq;
using Clay.Data;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;
using Clay.Services.Interfaces;

namespace Clay.Services.Implementation
{
    public class AttemptService : IAttemptService
    {
        private readonly IAttemptRepository _attemptRepository;

        public AttemptService(IAttemptRepository attemptRepository)
        {
            _attemptRepository = attemptRepository;
        }
        public PagedResult<Attempt> GetUserAttempts(string userId, PagedModel pagedModel)
        {
            return _attemptRepository.Attempts.Where(a => a.UserId == userId).GetPaged(pagedModel);
        }

        public PagedResult<Attempt> GetLockAttempts(Guid lockId, PagedModel pagedModel)
        {
            return _attemptRepository.Attempts.Where(a => a.LockId == lockId).GetPaged(pagedModel);
        }

        public PagedResult<Attempt> GetAttempts(PagedModel pagedModel)
        {
            return _attemptRepository.Attempts.GetPaged(pagedModel);
        }

        public void CreateAttempt(Attempt attempt)
        {
            _attemptRepository.CreateAttempt(attempt);
        }
    }
}