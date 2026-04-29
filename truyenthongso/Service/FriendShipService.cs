using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class FriendShipService : IFriendShipService
    {
        private readonly DBContext _context;
        private readonly IUserNameService _userNameService;
        private readonly RedisService _redis;
        public FriendShipService(DBContext context, IUserNameService userNameService, RedisService redis)
        {
            _context = context;
            _userNameService = userNameService;
            _redis = redis;

        }
        public async Task<PayLoad<FriendShipDTO>> AddFriend(FriendShipDTO data)
        {
            try
            {
                var checkData1 = _context.users.FirstOrDefault(x => x.id == data.user1 && !x.deleted);
                var checkData2 = _context.users.FirstOrDefault(x => x.id == data.user2 && !x.deleted);
                if (checkData1 == null || checkData2 == null)
                    return await Task.FromResult(PayLoad<FriendShipDTO>.CreatedFail(Status.DATANULL));

                // Kiểm tra đã kết bạn chưa
                var existed = await _context.friendships.AnyAsync(x => (x.UserId1 == data.user1 && x.UserId2 == data.user2)
                            || (x.UserId1 == data.user2 && x.UserId2 == data.user1)
                                && x.status == Status.AddFriend
                            );

                if (existed)
                    return PayLoad<FriendShipDTO>.CreatedFail(Status.DATATONTAI);

                using var transaction = await _context.Database.BeginTransactionAsync();

                _context.friendships.Add(new Friendship
                {
                    status = Status.AddFriend,
                    user1 = checkData1,
                    user2 = checkData2,
                    UserId1 = data.user1,
                    UserId2 = data.user2
                });

                _context.SaveChanges();

                //await _redis.SetAsync($"user:{data.user1}:friends", new[] { data.user2.ToString() });
                //await _redis.SetAsync($"user:{data.user2}:friends", new[] { data.user1.ToString() });

                // Set Redis
                await _redis.SetAsync($"user:{data.user1}:friends", data.user2.ToString());
                await _redis.SetAsync($"user:{data.user2}:friends", data.user1.ToString());

                await transaction.CommitAsync();

                // Background Job, Hangfile
                BackgroundJob.Enqueue(() => RecalculateFriendOfFriends(data.user1));
                BackgroundJob.Enqueue(() => RecalculateFriendOfFriends(data.user2));

                return PayLoad<FriendShipDTO>.Successfully(data);
                //return await Task.FromResult(PayLoad<FriendShipDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<FriendShipDTO>.CreatedFail(ex.Message));
            }
        }

        private async Task RecalculateFriendOfFriends(int userId)
        {
            var friends = await _redis.GetSetAsync($"user:{userId}:friends"); // Lấy ra danh sách những người đã là "bạn"
            var allFoFs = new HashSet<string>();

            foreach(var friendsId in friends)
            {
                var friendOfFriend = await _redis.GetSetAsync($"user:{friendsId}:friends"); // Lấy danh sách bạn bè của friendsId (tức là "bạn của bạn").
                foreach (var foFsId in friendOfFriend)
                {
                    // Điều kiện để chấp nhận một người vào danh sách friendsOfFriends
                    /*
                     * "foFsId != userId.ToString()" Là không gợi ý chính bản thân user (tránh gợi ý bản thân)
                     * "!friends.Contains(foFsId)" Đây là không gợi ý những người đã là bạn trực tiếp (không muốn gợi ý người đã kết bạn rồi)
                     */
                    if (foFsId != userId.ToString() && !friends.Contains(foFsId))
                    {
                        allFoFs.Add(foFsId);
                    }
                }
            }

            await _redis.SetAsync($"user:{userId}:friendsOfFriends", allFoFs.ToArray());
            await _redis.SetStringAsync($"user:{userId}:friendsOfFriends:timestamp", DateTime.UtcNow.ToString("o"));
        }
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.friendships.Select(x => new
                {
                    id = x.id
                }).ToList();
                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public Task<PayLoad<object>> FindOneId(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<object>> Suggestafriend(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userNameService.name();

                var checkAccount = _context.users.FirstOrDefault(x => x.id == int.Parse(user) && !x.deleted);
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                //var data = _context.users.Where(x => x.id == Convert.ToInt32(user)  && x.deleted).Select(x => new
                //{
                //    data1 = x.Friendships1.Where(x1 => x1.UserId1 == x.id && !x.deleted).Select(x2 => new
                //    {
                //        data2 = _context.friendships.Where(x3 => x3.UserId1 == x.id && x3.UserId2 == x2.UserId2).FirstOrDefault()
                //    })
                //}).FirstOrDefault();

                var data = _context.friendships.Where(x => x.UserId1 == Convert.ToInt32(user) && !x.deleted).Select(x => new
                {
                    id = x.id,
                    userId1 = x.UserId1,
                    userId2 = x.UserId2
                }).ToList();

                var checkUserId2 = _context.friendships.Where(x => data.Select(x1 => x1.userId2).Contains(x.UserId1) && !x.deleted).Select(x => new
                {
                    x.UserId2,
                    x.UserId1,
                    data = _context.friendships.Where(x1 => x1.UserId1 == int.Parse(user) && x1.UserId2 == x.UserId2).ToList()
                })
                    .Select(x2 => new
                    {
                        id1 = x2.data != null && x2.data.Count() > 0 ? x2.data.Select(x3 => x3.user2.id) : null,
                        userName1 = x2.data != null && x2.data.Count() > 0 ? x2.data.Select(x3 => x3.user2.FullName) : null ,
                        image1 = x2.data != null && x2.data.Count() > 0 ? x2.data.Select(x3 => x3.user2.Image) : null,
                    }).ToList();



                //checkUserId2 = checkUserId2.Where(x => x.UserId2 == int.Parse(user)).ToList();
                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task saveSuggestionPro(int userId, List<SuggestionDto> list)
        {
            var zsetKey = $"user:{userId}:suggestions";
            var hashKey = $"user:{userId}:suggestions:data";

            // Xóa dữ liệu cũ
            await _redis.DeleteKeyAsync(zsetKey);
            await _redis.DeleteKeyAsync(hashKey);

            // Batch (Tối ưu)
            var zsetEntries = new List<(string member, double score)>();
            var hashEntries = new List<(string field, string value)>();

            foreach(var item in list)
            {
                // ZSET => ranking
                zsetEntries.Add((item.Id.ToString(), item.MutualCount));

                // HASH => data
                hashEntries.Add((
                        item.Id.ToString(),
                        JsonSerializer.Serialize(item)
                    ));

                // Ghi dữ liệu vào Redis (Cực kỳ nhanh)
                await _redis.SortedSetAddManyAsync(zsetKey, zsetEntries);
                await _redis.HashSetManyAsync(hashKey, hashEntries);

            }
        }

        public async Task<PayLoad<List<SuggestionDto>>> GetSuggestions()
        {
            try
            {
                int userId = int.Parse(_userNameService.name());

                var zsetKey = $"user:{userId}:suggestions";
                var hashKey = $"user:{userId}:suggestions:data";

                var ids = await _redis.SortedSetRangeByScoreAsync(
                        zsetKey,
                        order: Order.Descending,
                        take: 20
                    );

                var result = await _redis.HashGetManyAsync<SuggestionDto>(hashKey, ids);

                return await Task.FromResult(PayLoad<List<SuggestionDto>>.Successfully(result));
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<List<SuggestionDto>>.CreatedFail(ex.Message));
            }
        }

        public async Task RecalculateSuggestions (int userId)
        {
            var friends = await _redis.GetSetAsync($"user:${userId}:friends");

            var mutualCount = new Dictionary<int, int>();

            foreach (var friendId in friends)
            {
                var fofList = await _redis.GetSetAsync($"user:{friendId}:friends");
                
                foreach(var fof in fofList)
                {
                    int fofId = int.Parse(fof);

                    if (fofId == userId || friends.Contains(fof))
                        continue;

                    if (!mutualCount.ContainsKey(fofId))
                        mutualCount[fofId] = 0;

                    mutualCount[fofId]++;
                }
            }

            // Lấy user Info
            var user = await _context.users
                .Where(u => mutualCount.Keys.Contains(u.id))
                .Select(u => new SuggestionDto
                {
                    Id = u.id,
                    Name = u.UserName,
                    Avatar = u.Image,
                    MutualCount = mutualCount[u.id]
                }).ToListAsync();


            var sorted = user.OrderByDescending(x => x.MutualCount).ToList();

            await saveSuggestionPro(userId, sorted);
        }
    }
}