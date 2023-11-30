using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.InteropServices;

namespace BCHousing.AfsWebAppMvc.Servives.CacheManagementService
{
    public class CacheManagementService
    {
        private readonly IMemoryCache _memoryCache;
        private static TimeSpan defaultExpirationTime = TimeSpan.FromMinutes(3);        //Fixed time period force data to being removed from Cache
        private static TimeSpan defaultSlidingExpiration = TimeSpan.FromMinutes(1);     //Idle time period force data to being removed from Cache

        public CacheManagementService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////// Sync Method ////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RefreshCache<TResult>(string cacheKey, Func<TResult> GetDataToCache, [Optional] TimeSpan expirationTime, [Optional] TimeSpan slidingExpiration)
        {
            _memoryCache.Set(cacheKey, GetDataToCache(), new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime == TimeSpan.Zero ? defaultExpirationTime : expirationTime,
                SlidingExpiration = slidingExpiration == TimeSpan.Zero ? defaultSlidingExpiration : slidingExpiration
            });
        }

        public TResult? GetCachedData<TResult>(string cacheKey, Func<TResult> GetDataToCache, [Optional] TimeSpan expirationTime, [Optional] TimeSpan slidingExpiration)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out TResult? cachedData))
                RefreshCache(cacheKey, GetDataToCache, expirationTime, slidingExpiration);

            return cachedData;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////// Async Method ////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public async Task RefreshCacheAsync<TResult>(string cacheKey, Func<Task<TResult>> GetDataToCache, [Optional] TimeSpan expirationTime, [Optional] TimeSpan slidingExpiration)
        {
            _memoryCache.Set(cacheKey, await GetDataToCache(), new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime == TimeSpan.Zero ? defaultExpirationTime : expirationTime,
                SlidingExpiration = slidingExpiration == TimeSpan.Zero ? defaultSlidingExpiration : slidingExpiration
            });
        }

        public async Task<TResult>? GetCachedDataAsync<TResult>(string cacheKey, Func<Task<TResult>?> GetDataToCache, [Optional] TimeSpan expirationTime, [Optional] TimeSpan slidingExpiration)
        {
            //If data is not yet cache -> cache new data
            if (!_memoryCache.TryGetValue(cacheKey, out TResult? cachedData))
                await RefreshCacheAsync(cacheKey, GetDataToCache, expirationTime, slidingExpiration);

            return cachedData;
        }

    }
}
