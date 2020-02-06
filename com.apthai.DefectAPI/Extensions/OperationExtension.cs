using com.apthai.DefectAPI.Model.DefectAPI;
using com.apthai.DefectAPI.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.DefectAPI.Extensions
{
    

    public static class OperationExtension
    {
        //public static List<vwUser> GetCacheUserAsync(this IMemoryCache _cache, IUnitOfWork _unitOfWork)
        //{

        //    // QIS
        //    List<vwUser> cacheUsersEntry;

        //    // Look for cache key.
        //    if (!_cache.TryGetValue(CacheKeys.UserProfilesEntry, out cacheUsersEntry))
        //    {

        //        // Key not in cache, so get data.
        //        cacheUsersEntry = _unitOfWork.UserRepository.GetAllUser().Result;

        //        // Set cache options.
        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //            // Keep in cache for this time, reset time if accessed.
        //            .SetSlidingExpiration(TimeSpan.FromDays(1));

        //        // Save data in cache.
        //        _cache.Set(CacheKeys.UserProfilesEntry, cacheUsersEntry, cacheEntryOptions);

        //    }
        //    cacheUsersEntry = cacheUsersEntry ?? new List<vwUser>();
        //    return cacheUsersEntry;

        //}
    }

}

