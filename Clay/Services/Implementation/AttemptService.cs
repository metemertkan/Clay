using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<Attempt> GetUserAttempts(string userId)
        {
            return _attemptRepository.Attempts.Where(a => a.UserId == userId).ToList();
        }

        public List<Attempt> GetLockAttempts(Guid lockId)
        {
            return _attemptRepository.Attempts.Where(a => a.LockId == lockId).ToList();
        }

        public List<Attempt> GetAttempts()
        {
            return _attemptRepository.Attempts.ToList();
        }

        public void CreateAttempt(Attempt attempt)
        {
            _attemptRepository.CreateAttempt(attempt);
        }
    }
}