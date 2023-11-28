using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService
{
    public class AfsDatabaseService : IAfsDatabaseService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly FormRepository _formRepository;
        private readonly SubmissionLogRepository _submissionLogRepository;

        public AfsDatabaseService(IMemoryCache memoryCache, FormRepository formRepository, SubmissionLogRepository submissionLogRepository)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _formRepository = formRepository;
            _submissionLogRepository = submissionLogRepository;
        }

        public async Task<IList<SubmissionLog>?> GetAllSubmissionLogs()
        {
            // If data not in the cache -> retrieve data from database -> save to submissionLogCachedData
            if(!_memoryCache.TryGetValue(CacheKey.SubmissionLogKey, out IList<SubmissionLog>? submissionLogCachedData))
            {
                // Request data from Repository
                submissionLogCachedData = await _submissionLogRepository.GetSubmissionLogs();

                // Setup Cache option
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    // Cache will be fixed remove from the Cache after 15 minutes
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4),

                    // If no access to the data with 3 minutes -> remove from the cache -> need to retrieve data again
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };

                // Cache new data into memory
                _memoryCache.Set(CacheKey.SubmissionLogKey, submissionLogCachedData, cacheEntryOptions);
            }

            return submissionLogCachedData;
        }
    }
}
