using System.Runtime.InteropServices;

namespace BCHousing.AfsWebAppMvc.Servives.CacheManagementService
{
    public static class CacheKey
    {
        public static string GetFormCacheKey(string key)
        {
            return $"FormKey::{key}";
        }

        public static string GetSubmissionLogCacheKey([Optional] string key)
        {
            return string.IsNullOrEmpty(key) ? "SubmissionLogKey" : $"SubmissionLogKey::{key}";
        }
    }
}
