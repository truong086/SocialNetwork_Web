using truyenthongso.Common;

namespace truyenthongso.Service
{
    public interface IRedisService
    {
        Task<PayLoad<object>> DeleteCacheByUser();
        Task<PayLoad<object>> DeleteCacheAllData(string key);
        Task<PayLoad<object>> DeleteCacheAll();
        Task<PayLoad<object>> AddAllCacheLike();

    }
}
