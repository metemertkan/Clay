using System;
using System.Collections.Generic;
using Clay.Models.Domain;

namespace Clay.Services.Interfaces
{
    public interface IAttemptService
    {
        List<Attempt> GetUserAttempts(string userId);
        List<Attempt> GetLockAttempts(Guid lockId);
        List<Attempt> GetAttempts();
        void CreateAttempt(Attempt attempt);

    }
}