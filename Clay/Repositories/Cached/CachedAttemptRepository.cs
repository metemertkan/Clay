using Clay.Models.Domain;
using Clay.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Clay.Repositories.Cached
{
    public class CachedAttemptRepository : CachedBaseRepositoryDecorator<Attempt>
    {
        public CachedAttemptRepository(IBaseRepository<Attempt> baseRepository, IDistributedCache distributedCache) : base(baseRepository, distributedCache)
        {
        }
    }
}