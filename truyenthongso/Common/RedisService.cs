using StackExchange.Redis;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace truyenthongso.Common
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisService(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
        }

        public async Task SetAsync(string key, string[] value)
        {
            await _db.SetAddAsync(key, value.Select(v => (RedisValue)v).ToArray());
        }

        public async Task<string[]> GetSetAsync(string key)
        {
            var member = await _db.SetMembersAsync(key);

            return member.Select(m => m.ToString()).ToArray();
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null) {
            await _db.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        // Các hàm làm việc với set (tập hợp)
        public async Task AddToSetAsync(string key, string value)
        {
            await _db.SetAddAsync(key, value);
        }

        public async Task RemoveFromSetAsync(string key, string value)
        {
            await _db.SetRemoveAsync(key, value);
        }

        public async Task<HashSet<string>> GetSetHashAsync(string key)
        {
            var member = await _db.SetMembersAsync(key);
            return member.Select(x => x.ToString()).ToHashSet();
        }

        // Hàm lưu mảng hoặc object JSON
        public async Task SetAsync<T>(string key, T data, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(data);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _db.StringGetAsync(key); // Dòng này gọi Redis để lấy dữ liệu dạng chuỗi được lưu với key tương ứng, Redis chỉ lưu dữ liệu dạng string (chuỗi) hoặc byte[], nên ta cần gọi StringGetAsync
            if (!json.HasValue) return default; // Kiểm tra xem Redis có trả dữ liệu về không

            // "JsonSerializer.Deserialize<T>(...)" chuyển chuỗi JSON về kiểu dữ liệu T
            return JsonSerializer.Deserialize<T>(json!); // Dấu "!" (null-forgiving operator) nói với compiler rằng biến này chắc chắn không null, để tránh warning.
        } 

        // Hàm xóa key
        public async Task DeleteKeyAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task HashSetAsync(string key, string field, string value)
        {
            await _db.HashSetAsync(key, field, value);
        }

        public async Task SortedSetAddAsync(string key, string value, double score)
        {
            await _db.SortedSetAddAsync(key, value, score);
        }

        public async Task<bool> CacheExpired(string key)
        {
            var timestampStr = await GetStringAsync($"{key}:timestamp");
            if (string.IsNullOrEmpty(timestampStr)) return true;

            if(DateTime.TryParse(timestampStr, out var timestamp))
            {
                return (DateTime.UtcNow - timestamp).TotalMinutes > 30; // Cachr 30 phút
            }

            return true;
        }
    }
}
