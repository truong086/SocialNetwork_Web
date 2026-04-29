using StackExchange.Redis;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class CacheFuncService : IRedisService
    {
        private readonly IDatabase _redis;
        private readonly IUserNameService _userNameService;
        private readonly DBContext _context;
        public CacheFuncService(IDatabase redis, IUserNameService userNameService, DBContext context)
        {
            _redis = redis;
            _userNameService = userNameService;
            _context = context;
        }

        // Add lại dữ liệu Like vào Redis
        public async Task<PayLoad<object>> AddAllCacheLike()
        {
            try
            {
                var data = _context.likes.Where(x => !x.deleted).Select(x => new
                {
                    x.User_id,
                    x.Post_id
                }).ToList();

                var groudData = data.GroupBy(x => x.User_id);

                var batch = _redis.CreateBatch();

                // Cách 2
                foreach (var item in groudData) {
                    var key = ConvertCacheKey.GetCacheKey(item.Key.Value);

                    foreach(var cache in item)
                    {
                        await batch.SetAddAsync(key, cache.Post_id);
                    }
                }

                batch.Execute();

                // Cách 1
                //foreach(var item in groudData)
                //{
                //    var key = ConvertCacheKey.GetCacheKey(item.Key.Value);

                //    foreach(var cache in item)
                //    {
                //        _redis.SetAdd(key, cache.Post_id);
                //    }

                //    // Cài đặt thời gian hết hạn
                //    _redis.KeyExpire(key, TimeSpan.FromDays(7));
                //}

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    Status.SUCCESS
                }));
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        // Xóa toàn bộ dữ liệu trong Redis
        public async Task<PayLoad<object>> DeleteCacheAll()
        {
            try
            {
                var key = _redis.Multiplexer.GetEndPoints();
                var server = _redis.Multiplexer.GetServer(key.First());

                server.FlushDatabase();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    Status.SUCCESS
                }));
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        // Xóa toàn bộ dữ liệu redis theo key
        public async Task<PayLoad<object>> DeleteCacheAllData(string key)
        {
            try
            {
                var endpoin = _redis.Multiplexer.GetEndPoints();
                var server = _redis.Multiplexer.GetServer(endpoin.First());

                foreach(var item in server.Keys(pattern: key))
                {
                    _redis.KeyDelete(item);
                }

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    Status.SUCCESS
                }));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        // Xóa dữ liệu redis theo User
        public async Task<PayLoad<object>> DeleteCacheByUser()
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var key = ConvertCacheKey.GetCacheKey(n);
                    _redis.KeyDelete(key);

                    return await Task.FromResult(PayLoad<object>.Successfully(new
                    {
                        Status.SUCCESS
                    }));
                }

                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
