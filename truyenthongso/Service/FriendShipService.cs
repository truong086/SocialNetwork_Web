using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;
using truyenthongso.Common;
using truyenthongso.Migrations;
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
                int userId = int.Parse(_userNameService.name());
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

                if(await _context.SaveChangesAsync() > 0)
                {
                    //await _redis.SetAsync($"user:{data.user1}:friends", new[] { data.user2.ToString() });
                    //await _redis.SetAsync($"user:{data.user2}:friends", new[] { data.user1.ToString() });

                    // Set Redis
                    //await _redis.SetAsync($"user:{data.user1}:friends", data.user2.ToString());
                    //await _redis.SetAsync($"user:{data.user2}:friends", data.user1.ToString());

                    await _redis.AddSetAsync($"user:{data.user1}:friends", new List<string> { data.user2.ToString() });
                    await _redis.AddSetAsync($"user:{data.user2}:friends", new List<string> { data.user1.ToString() });

                    await transaction.CommitAsync();

                    // Background Job, Hangfile
                    BackgroundJob.Enqueue(() => RecalculateFriendOfFriends(data.user1));
                    BackgroundJob.Enqueue(() => RecalculateFriendOfFriends(data.user2));

                    BackgroundJob.Enqueue(() => RecalculateSuggestions(data.user1, data.user2));
                    BackgroundJob.Enqueue(() => RecalculateSuggestions(data.user2, data.user1));

                    //await RecalculateFriendOfFriends(data.user1);
                    //await RecalculateFriendOfFriends(data.user2);

                    //await RecalculateSuggestions(data.user1, data.user2);
                    //await RecalculateSuggestions(data.user2, data.user1);


                    return PayLoad<FriendShipDTO>.Successfully(data);
                }

                return PayLoad<FriendShipDTO>.CreatedFail(Status.DATANULL);
                //return await Task.FromResult(PayLoad<FriendShipDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<FriendShipDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task RecalculateFriendOfFriends(int userId)
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
                    if (foFsId != userId.ToString() && !friends.Contains(foFsId)) // Kiểm tra chưa kết bạn "!friends.Contains(foFsId)" và khác với user đang đăng nhập "foFsId != userId.ToString()"
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

        private List<SuggestionDto> GetMutualFriends(
    List<MutualMap> maps,
    int userId,
    List<User> users
)
        {
            var mutualIds = maps
                .Where(x => x.User2 == userId)
                .Select(x => x.User1)
                .Distinct()
                .ToList();

            var checkUser = _context.users.Where(x => mutualIds.Contains(x.id) && !x.deleted).Select(x => new SuggestionDto
            {
                Id = x.id,
                Name = x.UserName,
                Avatar = x.Image
            }).ToList();

            return checkUser;
            //return users
            //    .Where(x => mutualIds.Contains(x.id))
            //    .Select(x => new SuggestionDto
            //    {
            //        Id = x.id,
            //        Name = x.UserName,
            //        Avatar = x.Image
            //    })
            //    .ToList();
        }

        // Cách 2
        public async Task RecalculateSuggestions(int userId, int userIdLogin)
        {
            /* Lấy ra danh sách bạn bè hiện tại, ở đây key redis đang là "user:{userId}:friends" thì dữ liệu sẽ chứa là, ví dụ:
             * => Nghĩa là User có  bạn bè là: 1, 2, 3
             */
            var friends = await _redis.GetSetAsync($"user:{userId}:friends");

            /* Chuyển sang HashSet mục đích để tăng tốc tìm kiếm (contains) vì nếu dùng "List.Contains" thì tốc độ là "0(n)",
             * còn nếu chuyển sang HashSet thì tốc độ sẽ là "0(1)" nên là rất nhanh
             */
            var friendIds = friends.Select(int.Parse).ToHashSet();

            /* Khởi tạo Dictionary đếm số bạn chung, ví dụ: 
             *   Key = userId được gợi ý
             *   Value = số bạn chung
             *   
             *   ==> Ví dụ: "5 => 2" nghĩa là user có id là 5 có 2 bạn chung
             */
            var mutualCount = new Dictionary<int, int>();

            /* "MutualMap" này dùng để lưu ai là bạn chung với ai
             * Ví dụ: user1 = 2
             *        user2 = 5
             *        => Nghĩa là user2 là bạn chung của user5
             */
            var mutualMaps = new List<MutualMap>();


            /* Dùng foreach để lặp qua từng bạn bè, Ví dụ:
             * => friends: 2,3,4
             */
            foreach (var friendIdStr in friends)
            {
                int friendId = int.Parse(friendIdStr);

                /* Lấy ra bạn của bạn (Freiend of Friend), ví dụ: 
                 * user:2:friends => 1,5,6
                 */
                var fofList = await _redis.GetSetAsync(
                    $"user:{friendId}:friends"
                );

                /* lặp qua từng bạn của bạn (friend-of-friend)
                 */
                foreach (var fofStr in fofList)
                {
                    int fofId = int.Parse(fofStr);

                    // bỏ qua chính mình
                    // bỏ qua bạn bè đã kết bạn
                    if (fofId == userId || friendIds.Contains(fofId))
                        continue;

                    /* Khởi tạo mutual count, Ví dụ: 
                     * 5 chưa có 
                     * --- Tạo 5 => 0
                     */
                    if (!mutualCount.ContainsKey(fofId))
                    {
                        mutualCount[fofId] = 0;
                    }

                    /* Tăng số lượng bạn chung, Ví dụ: 
                        5 => 2 Nghĩa là 5 có 2 bạn chung
                     */
                    mutualCount[fofId]++;


                    /* Lưu thông tin bạn chung, Ví dụ:
                     * => User1 = 2 
                     *    User2 = 5
                     *    ===> Nghĩa là User có id là 2 có bạn chung là User có id là 5
                     */
                    mutualMaps.Add(new MutualMap
                    {
                        User1 = friendId,
                        User2 = fofId
                    });
                }
            }

            //if (!mutualCount.Any())
            //    return;

            // query user 1 lần, để lấy dữ liệu từ Database
            var users = await _context.users
                .Where(x =>
                    mutualCount.Keys.Contains(x.id)
                    && !x.deleted
                )
                .ToListAsync();

            /* Build suggestion, Ví dụ kết quả là:
                {
                  "Id": 5,
                  "Name": "John",
                  "MutualCount": 2,
                  "Mutual_friend": [...]
                }
                
             */
            var suggestions = users
                .Select(u => new SuggestionDto
                {
                    Id = u.id,
                    Name = u.UserName,
                    Avatar = u.Image,
                    MutualCount = mutualCount[u.id],
                    Mutual_friend = GetMutualFriends( // "GetMutualFriends()" hàm này dùng để lấy ra thông tin ai là bạn chung của User đó
                        mutualMaps,
                        u.id,
                        users
                    )
                })
                .OrderByDescending(x => x.MutualCount) // Sắp xếp theo số lượng bạn chung
                .ToList();


            // Lưu dữ liệu vào Redis Cache
            await saveSuggestionPro(userId, suggestions);


            // Chạy song song update redis cache mutual
            var tasks = users.Select(u =>
                CheckRedisData(
                    mutualMaps,
                    u.id,
                    userId
                )
            );

            /* Dùng Task.WhenAll() để chạy song song thay vì chạy tuần tự bởi vì chạy song song nhanh hơn rất nhiều
             */
            await Task.WhenAll(tasks);
        }

        // Cách 2
        public async Task<bool> CheckRedisData(
    List<MutualMap> data,
    int id,
    int userId
)
        {
            try
            {
                string key = $"user:{id}:suggestions:data";

                string field = userId.ToString();

                // check tồn tại
                bool exists = await _redis.HashExistsAsync(
                    key,
                    field
                );

                if (!exists)
                    return false;

                // lấy data redis
                var redisData = await _redis
                    .HashGetAsync<SuggestionDto>(
                        key,
                        field
                    );

                if (redisData == null)
                    return false;

                if (redisData.Mutual_friend == null)
                {
                    redisData.Mutual_friend =
                        new List<SuggestionDto>();
                }

                // mutual ids
                var mutualIds = data
                    .Where(x => x.User2 == id)
                    .Select(x => x.User1)
                    .Distinct()
                    .ToList();

                // user data
                var userData = await _context.users
                    .Where(x =>
                        mutualIds.Contains(x.id)
                        && !x.deleted
                    )
                    .Select(x => new SuggestionDto
                    {
                        Id = x.id,
                        Name = x.UserName,
                        Avatar = x.Image
                    })
                    .ToListAsync();

                // hashset để check nhanh
                var existingIds = redisData
                    .Mutual_friend
                    .Select(x => x.Id)
                    .ToHashSet();

                bool changed = false;

                foreach (var item in userData)
                {
                    if (!existingIds.Contains(item.Id))
                    {
                        redisData.Mutual_friend.Add(item);

                        existingIds.Add(item.Id);

                        changed = true;
                    }
                }

                // chỉ update khi có thay đổi
                if (changed)
                {
                    await _redis.UpdateHashAsync(
                        key,
                        field,
                        redisData
                    );
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cách 1
        //public async Task RecalculateSuggestions (int userId, int userIdLogin)
        //{
        //    var friends = await _redis.GetSetAsync($"user:{userId}:friends");

        //    var mutualCount = new Dictionary<int, int>();
        //    var banChung = new List<object>();
        //    foreach (var friendId in friends)
        //    {
        //        var fofList = await _redis.GetSetAsync($"user:{friendId}:friends");

        //        foreach(var fof in fofList)
        //        {
        //            int fofId = int.Parse(fof);

        //            if (fofId == userId || friends.Contains(fof))
        //                continue;

        //            if (!mutualCount.ContainsKey(fofId))
        //                mutualCount[fofId] = 0;

        //            banChung.Add(new
        //            {
        //                user1 = int.Parse(friendId),
        //                user2 = fofId
        //            });

        //            mutualCount[fofId]++;
        //        }
        //    }


        //    // Lấy user Info
        //    //var user = await _context.users
        //    //    .Where(u => mutualCount.Keys.Contains(u.id))
        //    //    .Select(u => new SuggestionDto
        //    //    {
        //    //        Id = u.id,
        //    //        Name = u.UserName,
        //    //        Avatar = u.Image,
        //    //        MutualCount = mutualCount[u.id],
        //    //        Mutual_friend = CheckFriendOfFriend(banChung, u.id)
        //    //    }).ToListAsync();

        //    var userQuery = await _context.users.Where(x => mutualCount.Keys.Contains(x.id) && !x.deleted).ToListAsync();

        //    var user = userQuery.Select(u => new SuggestionDto
        //    {
        //        Id = u.id,
        //        Name = u.UserName,
        //        Avatar = u.Image,
        //        MutualCount = mutualCount[u.id],
        //        Mutual_friend = CheckFriendOfFriend(banChung, u.id)
        //    }).ToList();


        //    var sorted = user.OrderByDescending(x => x.MutualCount).ToList();
        //    var userCheckss = userQuery.Select(async u => new
        //    {
        //        Mutual_friend = await checkRedidData(banChung, u.id, userId)
        //    }).ToList();
        //    await saveSuggestionPro(userId, sorted);
        //}

        // Cách 1
        //public async Task<bool> checkRedidData(List<object>? data, int id, int userId)
        //{
        //    try
        //    {
        //        var checkData = await _redis.HashExistsAsync($"user:{id}:suggestions:data", userId.ToString());
        //        if (checkData)
        //        {
        //            if (data == null || !data.Any())
        //                return await Task.FromResult(false);

        //            var idUser = data.Cast<dynamic>()
        //                .Where(x => x.user2 == id)
        //                .Select(x => x.user1)
        //                .Distinct()
        //                .ToList();

        //            var userData = _context.users
        //                .Where(x => idUser.Contains(x.id) && !x.deleted)
        //                .Select(x => new SuggestionDto
        //                {
        //                    Id = x.id,
        //                    Avatar = x.Image,
        //                    Name = x.UserName
        //                })
        //                .ToList();

        //            var dataCheck = await _redis.HashGetAsync<SuggestionDto>($"user:{id}:suggestions:data", userId.ToString());

        //            if(dataCheck.Mutual_friend == null)
        //            {
        //                dataCheck.Mutual_friend = new List<SuggestionDto>();
        //            }

        //            foreach(var item in userData)
        //            {
        //                bool checkMutualFriend = dataCheck.Mutual_friend.Any(x => x.Id == item.Id);
        //                if (!checkMutualFriend)
        //                {

        //                    dataCheck.Mutual_friend.Add(new SuggestionDto
        //                    {
        //                        Id = item.Id,
        //                        Name = item.Name,
        //                        Avatar = item.Avatar
        //                    });

        //                    var updateJson = JsonSerializer.Serialize(dataCheck);

        //                    await _redis.UpdateHashAsync(
        //                            $"user:{id}:suggestions:data",
        //                            userId.ToString(),
        //                            updateJson
        //                        );
        //                }
        //            }

        //            return await Task.FromResult(true);


        //        }

        //        return await Task.FromResult(false);
        //    }
        //    catch(Exception ex)
        //    {
        //        return await Task.FromResult(false);
        //    }
        //}
        public  List<SuggestionDto> CheckFriendOfFriend(List<object>? data, int id)
        {
            if (data == null || !data.Any())
                return new List<SuggestionDto>();

            var idUser = data.Cast<dynamic>()
                .Where(x => x.user2 == id)
                .Select(x => x.user1)
                .Distinct()
                .ToList();

            var userData = _context.users
                .Where(x => idUser.Contains(x.id) && !x.deleted)
                .Select(x => new SuggestionDto
                {
                    Id = x.id,
                    Avatar = x.Image,
                    Name = x.UserName
                })
                .ToList();

            return userData;
        }

        //public List<SuggestionDto>? CheckFriendOfFriend(List<object>? data, int id)
        //{
        //    // Cách 2
        //    var checkData = data.Cast<dynamic>() // Vì List là 1 object nên phải dùng "Cast" để chuyển sang "dynamic" để duyệt mảng
        //        .GroupBy(x => x.user2).Where(x => x.Key == id).Select(x => new
        //        {
        //            id = x.Key,
        //            user = x.Select(x => (int)x.user1).ToList()
        //        }).ToList();

        //    /*
        //     * Trong trường hợp này phải dùng "SelectMany" vì "user" là 1 List nên "SelectMany" sẽ trả ra kiểu dữ liệu như này "[1,2,3,4,5]", trả ra dữ liệu như này sẽ dễ tìm kiếm hơn
        //     * vì user là 1 list nên nếu dùng "Select" thì sẽ trả ra như này: 
        //     * [
        //     *     [1,2,3],
        //           [4,5]
        //        ]
        //     */
        //    var idUser = checkData.SelectMany(x => x.user).Distinct().ToList(); 

        //    var userData = _context.users.Where(x => idUser.Contains(x.id) && !x.deleted).Select(x => new SuggestionDto
        //    {
        //        Id = x.id,
        //        Avatar = x.Image,
        //        Name = x.UserName
        //    }).ToList();

        //    return userData == null ? new List<SuggestionDto>() : userData;

        //    //Cách 1
        //    /*
        //        var userIds = data.Cast<dynamic>()
        //        .Where(x => x.user2 == id)
        //        .Select(x => (int)x.user1)
        //        .Distinct()
        //        .ToList();

        //    var users = await _context.Users
        //        .Where(x => userIds.Contains(x.Id))
        //        .ToListAsync();
        //     */
        //}

        public async Task<PayLoad<object>> TestRedis()
        {
            try
            {
                var user = 1;
                var friends = await _redis.GetSetAsync($"user:{user}:friends");
                return await Task.FromResult(PayLoad<object>.Successfully(friends));

            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}