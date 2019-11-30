using System.Linq;
using Clay.Models.Domain;

namespace Clay.Repositories.Interfaces
{
    public interface IAttemptRepository
    {
        void CreateAttempt(Attempt attempt);
        IQueryable<Attempt> Attempts { get; }
    }
}