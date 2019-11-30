using System.Linq;
using Clay.Data;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;

namespace Clay.Repositories.Implementations
{
    public class EFAttemptRepository : IAttemptRepository
    {
        private WebDbContext _context;

        public EFAttemptRepository(WebDbContext context)
        {
            _context = context;
        }

        public void CreateAttempt(Attempt attempt)
        {
            _context.Attempts.Add(attempt);
            _context.SaveChanges();
        }

        public IQueryable<Attempt> Attempts => _context.Attempts;
    }
}