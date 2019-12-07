using Clay.Data;
using Clay.Models.Domain;

namespace Clay.Repositories.Implementations
{
    public class EFAttemptRepository : BaseRepository<Attempt>
    {
        public EFAttemptRepository(WebDbContext context) : base (context)
        { }
    }
}