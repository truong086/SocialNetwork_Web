using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truyenthongso.Common;
using truyenthongso.Service;

namespace truyenthongso.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IRedisService _redisService;
        public CacheController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpPost]
        [Route(nameof(AddAllCacheLike))]
        public async Task<PayLoad<object>> AddAllCacheLike()
        {
            return await _redisService.AddAllCacheLike();
        }

        [HttpPost]
        [Route(nameof(DeleteCacheByUser))]
        public async Task<PayLoad<object>> DeleteCacheByUser()
        {
            return await _redisService.DeleteCacheByUser();
        }

        [HttpPost]
        [Route(nameof(DeleteCacheAllData))]
        public async Task<PayLoad<object>> DeleteCacheAllData(string key)
        {
            return await _redisService.DeleteCacheAllData(key);
        }

        [HttpPost]
        [Route(nameof(DeleteCacheAll))]
        public async Task<PayLoad<object>> DeleteCacheAll()
        {
            return await _redisService.DeleteCacheAll();
        }
    }
}
