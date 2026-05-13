using Headlight.Data;
using Headlight.Models.API.Stats;
using Microsoft.Extensions.Caching.Memory;

namespace Headlight.Services
{
    public interface IStatsService
    {
        Stats GetStats();
    }
    public class StatsService : IStatsService
    {
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _context;

        public StatsService(IMemoryCache cache, AppDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public Stats GetStats()
        {
            return new();
        }
    }
}
