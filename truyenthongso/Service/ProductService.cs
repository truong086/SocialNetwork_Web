using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Linq;
using truyenthongso.Clouds;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.PythonAI;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class ProductService : IProductService
    {
        private readonly DBContext _context;
        private readonly IUserNameService _userNameService;
        private IMapper _mapper;
        private readonly Cloud _cloud;
        private readonly IAIService _aIService;
        private readonly IDatabase _cache;
        private readonly RedisService _redis;
        public ProductService(DBContext context, IUserNameService userNameService,
            IMapper mapper, IOptions<Cloud> cloud, IAIService aIService,
            IConnectionMultiplexer redis, RedisService redisFunc)
        {
            _context = context;
            _userNameService = userNameService;
            _mapper = mapper;
            _cloud = cloud.Value;
            _aIService = aIService;
            _cache  = redis.GetDatabase();
            _redis = redisFunc;
        }
        public async Task<PayLoad<PostResponse>> Add(ProductDTO productDTO)
        {
            try
            {
                var user = _userNameService.name();
                //if(user != null) return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.DATANULL));
                var checkUser = _context.users.FirstOrDefault(x => x.id == int.Parse(user) && !x.deleted);
                var checkCategory = _context.categories.FirstOrDefault(x => x.id == productDTO.Categoryid && !x.deleted);
                if (checkUser == null || checkCategory == null)
                    return await Task.FromResult(PayLoad<PostResponse>.CreatedFail(Status.DATANULL));


                var mapData = _mapper.Map<Post>(productDTO);
                
                mapData.User_id = checkUser.id;
                mapData.Category_id = checkCategory.id;
                mapData.user = checkUser;
                mapData.category = checkCategory;

                _context.posts.Add(mapData);
                _context.SaveChanges();

                var dataUserTagPost = FormatTagUser.FomatData(mapData.Description == null || mapData.Description == "" ? "" : mapData.Description);

                var dataNew = _context.posts.OrderByDescending(x => x.id);
                if (dataNew == null || dataNew.Count() <= 0 || !dataNew.Any())
                    return await Task.FromResult(PayLoad<PostResponse>.CreatedFail(Status.DATANULL));

                var dataNew1 = dataNew.FirstOrDefault();
                if (productDTO.tagId != null && productDTO.tagId.Count > 0)
                {
                    var checkIdTag = _context.tags.Where(x => productDTO.tagId.Contains(x.id) && !x.deleted).ToList();

                    
                    var listTagAdd = checkIdTag.Select(x => new Tag_Post
                    {
                        post = dataNew1,
                        Post_id = dataNew1.id,
                        Tag_id = x.id,
                        tag = x
                    }).ToList();

                    _context.tagPosts.AddRange(listTagAdd);
                    
                }
                if (productDTO.images != null && productDTO.images.Count > 0)
                {
                    var listImage = new List<Post_Image>();
                    foreach (var image in productDTO.images)
                    {
                        uploadCloud.CloudInaryIFromAccount(image, checkUser.FullName + "" + dataNew1.id, _cloud);
                        var newDataImage = new Post_Image()
                        {
                            post = dataNew1,
                            Post_id = dataNew1.id,
                            PublicId = uploadCloud.publicId,
                            Url = uploadCloud.Link
                        };
                        listImage.Add(newDataImage);
                    }

                    _context.postImages.AddRange(listImage);
                    //_context.SaveChanges();

                }

                if(dataUserTagPost != null && dataUserTagPost.Count > 0)
                {
                    // Truy vấn lấy ra tất cả User có id trong "dataUserTagPost"
                    var userTagged = _context.users.Where(x => dataUserTagPost.Contains(x.id)).ToList();

                    // Tạo danh sách PostUserTag từ User đã truy vấn ở trên( userTagged )
                    var listUserData = userTagged.Select(x => new PostUserTag
                    {
                        post_id = dataNew1.id,
                        user_id = x.id
                    }).ToList();

                    if (listUserData.Any())
                    {
                        _context.postUserTags.AddRange(listUserData);
                        //_context.SaveChanges();
                    }
                }

                if (productDTO.tag_friendId != null && productDTO.tag_friendId.userId.Count > 0) 
                {
                    var userFriendTag = _context.users.Where(x => productDTO.tag_friendId.userId.Contains(x.id) && !x.deleted).ToList();

                    var dataTagFriend = userFriendTag.Select(x => new Tag_Friend
                    {
                        post = dataNew1,
                        post_id = dataNew1.id,
                        user = checkUser,
                        user_id = checkUser.id,
                        friend = x,
                        friend_id = x.id
                    }).ToList();

                    if (dataTagFriend.Any())
                    {
                        _context.tag_Friend.AddRange(dataTagFriend);
                    }
                }

                if(await _context.SaveChangesAsync() > 0)
                {
                    var dataNewDTO = dataNew.Select(x => new PostResponse
                    {
                        Id = x.id,
                        Title = x.Title,
                        Description = x.Description,
                        Image = x.Post_Images.Select(i => i.Url),
                        Like = x.Likes.Count(l => !l.deleted),
                        Tym = x.Likes.Count(l => l.status == 2),
                        Share = x.Sheres.Count(),
                        Comment = x.Comments.Count() + x.Comments.SelectMany(c => c.CommentDescriptions).Count(),
                        User = x.user.FullName,
                        Tag = x.TagPosts.Select(xt => new tagPostData
                        {
                            Id = xt.Tag_id.Value,
                            name = xt.tag == null ? "" : xt.tag.Name
                        }).ToList(),
                        UserId = x.user.id,
                        category_id = x.Category_id.Value,
                        category_name = x.category.Name,
                        UserImage = x.user.Image,
                        url_icon = x.Likes.Where(l => l.User_id == Convert.ToInt32(user) && !l.deleted).Select(l => l.url_icon).FirstOrDefault(),
                        //isLike = x.Likes.FirstOrDefault(xl => xl.Post_id == x.id && xl.User_id == userId) != null ? true : false,
                        //isLike = x.Likes.Any(xl => xl.Post_id == x.id && xl.User_id == n && !xl.deleted),
                        isLike = false,
                        Date = x.cretoredat,
                        Tag_Friend = x.tag_Friends.Where(t => t.user_id == x.User_id && t.post_id == x.id).Select(t => new tagPostData
                        {
                            Id = t.friend_id.Value,
                            name = t.friend.UserName
                        }).ToList(),
                        Status = x.Status
                    }).FirstOrDefault();

                    return await Task.FromResult(PayLoad<PostResponse>.Successfully(dataNewDTO));
                }

                return await Task.FromResult(PayLoad<PostResponse>.CreatedFail(Status.DATANULL));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<PostResponse>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<LikeDTO>> AddLike(LikeDTO likeDTO)
        {
            try
            {
                var user = _userNameService.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(user) && !x.deleted && x.Action);
                var checkPost = _context.posts.FirstOrDefault(x => x.id == likeDTO.Post_id && !x.deleted);
                var checkIcon = _context.icons.FirstOrDefault(x => x.id == likeDTO.id_icon && x.url == likeDTO.url_icon && !x.deleted);
                if (checkAccount == null || checkPost == null || checkIcon == null)
                    return await Task.FromResult(PayLoad<LikeDTO>.CreatedFail(Status.DATANULL));

                var checKLike = _context.likes.FirstOrDefault(x => x.User_id == int.Parse(user) && x.Post_id == likeDTO.Post_id && !x.deleted);
                if (checKLike == null)
                {
                    _context.likes.Add(new Like
                    {
                        post = checkPost,
                        Post_id = checkPost.id,
                        user = checkAccount,
                        User_id = checkAccount.id,
                        status = 1,
                        icon = checkIcon,
                        url_icon = likeDTO.url_icon
                    });

                    _context.articles_Viewedss.Add(new Articles_Viewed
                    {
                        post = checkPost,
                        post_id = checkPost.id,
                        user = checkAccount,
                        user_id = checkAccount.id
                    });
                    await _cache.SetAddAsync(ConvertCacheKey.GetCacheKey(int.Parse(user)), likeDTO.Post_id);
                }
                else
                {
                    if (likeDTO.status == 1)
                    {
                        checKLike.deleted = true;
                        await _cache.SetRemoveAsync(ConvertCacheKey.GetCacheKey(int.Parse(user)), likeDTO.Post_id);
                    }
                    else if (likeDTO.status == 2)
                    {
                        checKLike.icon = checkIcon;
                        checKLike.url_icon = checkIcon.url;
                    }
                    
                    
                }

                await _context.SaveChangesAsync();

                var total_Like = _context.likes.Where(x => x.Post_id == likeDTO.Post_id && !x.deleted).Count();
                likeDTO.totalLike = total_Like;

                return await Task.FromResult(PayLoad<LikeDTO>.Successfully(likeDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<LikeDTO>.CreatedFail(ex.Message));
            }
        }

        public Task<PayLoad<ShareDTO>> AddShere(ShareDTO shareDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.posts.FirstOrDefault(x => x.id == id && !x.deleted);
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;

                _context.posts.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        // Lấy ra id đã là bạn bè của User đăng nhập
        private List<int> FriendIdDatas(int userid)
        {
            return _context.friendships.Where(x => (x.UserId1 == userid || x.UserId2 == userid) && !x.deleted)
                .Select(x => x.UserId1 == userid ? x.UserId2 : x.UserId1)
                //.Where(x => x.HasValue) // Cách 1: Lấy dữ liệu của "UserId1" và "UserId2" với điều kiện là giá trị phải khác null
                .Where(x => x != null) // Cách 2: Lấy dữ liệu của "UserId1" và "UserId2" với điều kiện là giá trị phải khác null
                .Select(x => x.Value)
                .ToList();
        }
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                int userId = int.Parse(_userNameService.name());

                var dateCheckOld = DateTimeOffset.Now.AddDays(-7);
                var dateCheckNew = DateTimeOffset.Now.AddDays(7);
                var checkUser = _context.users
                    .FirstOrDefault(x => x.id == userId && !x.deleted);
                if (checkUser == null) return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var key = ConvertCacheKey.GetCacheKey(userId);

                var listPostId = _cache.SetMembers(key)
                    .Select(x => (int)x)
                    .ToHashSet();

                var checkBehavioral = _context.behavioral_Analysess.Where(x => x.user_id == checkUser.id 
                && x.total >= 0.1m 
                && x.cretoredat >= dateCheckOld 
                && x.cretoredat <= dateCheckNew)
                    .ToList();

                var ViewePosts = _context.articles_Viewedss.Where(x => x.user_id == checkUser.id && !x.deleted).Select(x  => x.post_id).ToList();

                var behaviorCategories = checkBehavioral.Select(x => x.category_id).ToList();

                var random = new Random();

                int takeCount = random.Next(2, 6); // Random từ 2 -> 5, nghĩa là min là 2, lớn nhất là 6 nhưng sẽ không lấy đến số lớn nhất, nên chỉ lấy đến số 5
                var dataAll = checkBehavioral == null || checkBehavioral.Count <= 0 ? _context.posts.AsNoTracking().OrderByDescending(x => x.cretoredat).Select(x => new PostResponse
                {
                    Id = x.id,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Post_Images.Select(i => i.Url),
                    //Like = x.Likes.Count(l => l.status == 1 && !l.deleted),
                    Like = x.Likes.Where(l => !l.deleted).Count(),
                    Tym = x.Likes.Count(l => l.status == 2 && !l.deleted),
                    Share = x.Sheres.Count(),
                    Comment = x.Comments.Count() + x.Comments.SelectMany(c => c.CommentDescriptions).Count(),
                    User = x.user.FullName,
                    Tag = x.TagPosts.Where(xt => xt.Post_id == x.id).Select(xt => new tagPostData
                    {
                        Id = xt.Tag_id.Value,
                        name = xt.tag == null ? "" : xt.tag.Name
                    }).ToList(),
                    UserId = x.user.id,
                    category_id = x.Category_id.Value,
                    category_name = x.category.Name,
                    UserImage = x.user.Image,
                    url_icon = x.Likes.Where(l => l.User_id == userId && !l.deleted).Select(l => l.url_icon).FirstOrDefault(),
                    //isLike = x.Likes.FirstOrDefault(xl => xl.Post_id == x.id && xl.User_id == userId) != null ? true : false,
                    //isLike = x.Likes.Any(xl => xl.Post_id == x.id && xl.User_id == userId && !xl.deleted),
                    isLike = listPostId.Contains(x.id),
                    Date = x.cretoredat,
                    deleted = x.deleted,
                    Tag_Friend = x.tag_Friends.Where(t => t.user_id == x.User_id && t.post_id == x.id).Select(t => new tagPostData
                    {
                        Id = t.friend_id.Value,
                        name = t.friend.UserName
                    }).ToList(),
                    comment_Data = x.Comments.Select(c => new commentData
                    {
                        id = c.id,
                        id_post = c.Post_id.Value,
                        image_user = c.user.Image,
                        text = c.Description,
                        user_name = c.user.UserName,
                        total_CommentDescript = c.CommentDescriptions.Where(cd => cd.Comment_id == c.id && !cd.deleted).Count(),
                        url = c.Url
                    }).OrderByDescending(c => c.id).ThenBy(c => Guid.NewGuid()).Take(ranDomData()).ToList(),
                    //iconCounts = x.Likes.AsQueryable().Where(l => l.Post_id == x.id && !l.deleted).GroupBy(l => l.url_icon).Select(l => new iconCount
                    //{
                    //    // .AsQueryable() dùng cho Data lớn
                    //    icon = l.Key,
                    //    count = l.Count()
                    //    //count = l.Select(g => g.User_id).Distinct().Count(),
                    //}).ToList(),
                    iconCounts = x.Likes.AsQueryable().Where(l => !l.deleted).GroupBy(l => l.url_icon).Select(l => new iconCount
                    {
                        icon = l.Key,
                        count = l.Count()
                    }).OrderByDescending(l => l.count).ToList(),
                    Status = x.Status
                }).Where(x => !x.deleted && x.isLike == false && x.UserId != userId 
                && ((FriendIdDatas(userId).Contains(x.UserId) && x.Status == 3) || x.Status == 1)).ToList().OrderBy(x => Guid.NewGuid()).ToList()
                :
                _context.posts.AsNoTracking().OrderByDescending(x => x.cretoredat).Select(x => new PostResponse
                {
                    Id = x.id,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Post_Images.Select(i => i.Url),
                    //Like = x.Likes.Count(l => !x.deleted),
                    Like = x.Likes.Where(l => !l.deleted).Count(),
                    Tym = x.Likes.Count(l => l.status == 2 && !x.deleted),
                    Share = x.Sheres.Count(),
                    Comment = x.Comments.Count() + x.Comments.SelectMany(c => c.CommentDescriptions).Count(),
                    User = x.user.FullName,
                    Tag = x.TagPosts.Where(xt => xt.Post_id == x.id).Select(xt => new tagPostData
                    {
                        Id = xt.Tag_id.Value,
                        name = xt.tag == null ? "" : xt.tag.Name
                    }).ToList(),
                    UserId = x.user.id,
                    category_id = x.Category_id ?? 0,
                    category_name = x.category.Name,
                    UserImage = x.user.Image,
                    url_icon = x.Likes.Where(l => l.User_id == userId && !l.deleted).Select(l => l.url_icon).FirstOrDefault(),
                    //isLike = x.Likes.FirstOrDefault(xl => xl.Post_id == x.id && xl.User_id == userId) != null ? true : false,
                    //isLike = x.Likes.Any(xl => xl.Post_id == x.id && xl.User_id == userId && !xl.deleted),
                    isLike = listPostId.Contains(x.id),
                    Date = x.cretoredat,
                    deleted = x.deleted,
                    Tag_Friend = x.tag_Friends.Where(t => t.user_id == x.User_id && x.User_id != userId && t.post_id == x.id).Select(t => new tagPostData
                    {
                        Id = t.friend_id.Value,
                        name = t.friend.UserName
                    }).ToList(),
                    comment_Data = x.Comments.Select(c => new commentData
                    {
                        id = c.id,
                        id_post = c.Post_id.Value,
                        image_user = c.user.Image,
                        text = c.Description,
                        user_name = c.user.UserName,
                        total_CommentDescript = c.CommentDescriptions.Where(cd => cd.Comment_id == c.id && !cd.deleted).Count(),
                        url = c.Url
                    }).OrderByDescending(c => c.id).ThenBy(c => Guid.NewGuid()).Take(ranDomData()).ToList(),
                    iconCounts = x.Likes.AsQueryable().Where(l => !l.deleted).GroupBy(l => l.url_icon).Select(l => new iconCount
                    {
                        icon = l.Key,
                        count = l.Count()
                    }).OrderByDescending(l => l.count).ToList(),
                    Status = x.Status
                }).Where(x => !x.deleted && x.UserId != userId && x.isLike == false && !ViewePosts.Contains(x.Id) && behaviorCategories.Contains(x.category_id) 
                        && ((FriendIdDatas(userId).Contains(x.UserId) && x.Status == 3) || x.Status == 1))
                .ToList().OrderBy(x => Guid.NewGuid()).ToList();

                if(dataAll.Count < 5)
                {
                    var dataNew = _context.posts.AsNoTracking()
                        .Select(x => new PostResponse
                        {
                            Id = x.id,
                            Title = x.Title,
                            Description = x.Description,
                            Image = x.Post_Images.Select(i => i.Url),
                            //Like = x.Likes.Count(l => !x.deleted),
                            Like = x.Likes.Where(l => !l.deleted).Count(),
                            Tym = x.Likes.Count(l => l.status == 2 && !x.deleted),
                            Share = x.Sheres.Count(),
                            Comment = x.Comments.Count() + x.Comments.SelectMany(c => c.CommentDescriptions).Count(),
                            User = x.user.FullName,
                            Tag = x.TagPosts.Where(xt => xt.Post_id == x.id).Select(xt => new tagPostData
                            {
                                Id = xt.Tag_id.Value,
                                name = xt.tag == null ? "" : xt.tag.Name
                            }).ToList(),
                            UserId = x.user.id,
                            category_id = x.Category_id ?? 0,
                            category_name = x.category.Name,
                            UserImage = x.user.Image,
                            url_icon = x.Likes.Where(l => l.User_id == userId && !l.deleted).Select(l => l.url_icon).FirstOrDefault(),
                            //isLike = x.Likes.FirstOrDefault(xl => xl.Post_id == x.id && xl.User_id == userId) != null ? true : false,
                            //isLike = x.Likes.Any(xl => xl.Post_id == x.id && xl.User_id == userId && !xl.deleted),
                            isLike = listPostId.Contains(x.id),
                            Date = x.cretoredat,
                            deleted = x.deleted,
                            Tag_Friend = x.tag_Friends.Where(t => t.user_id == x.User_id && x.User_id != userId && t.post_id == x.id).Select(t => new tagPostData
                            {
                                Id = t.friend_id.Value,
                                name = t.friend.UserName
                            }).ToList(),
                            comment_Data = x.Comments.Select(c => new commentData
                            {
                                id = c.id,
                                id_post = c.Post_id.Value,
                                image_user = c.user.Image,
                                text = c.Description,
                                user_name = c.user.UserName,
                                total_CommentDescript = c.CommentDescriptions.Where(cd => cd.Comment_id == c.id && !cd.deleted).Count(),
                                url = c.Url
                            }).OrderByDescending(c => c.id).ThenBy(c => Guid.NewGuid()).Take(ranDomData()).ToList(),
                            iconCounts = x.Likes.AsQueryable().Where(l => !l.deleted).GroupBy(l => l.url_icon).Select(l => new iconCount
                            {
                                icon = l.Key,
                                count = l.Count()
                            }).OrderByDescending(l => l.count).ToList(),
                            Status = x.Status
                        })
                        .Where(x => !x.deleted && !behaviorCategories.Contains(x.category_id) && x.isLike == false && !ViewePosts.Contains(x.Id) 
                               && FriendIdDatas(userId).Contains(x.UserId) && ((FriendIdDatas(userId).Contains(x.UserId) && x.Status == 3) || x.Status == 1))
                        .OrderByDescending(x => x.Id).Take(5 - dataAll.Count()).ToList().OrderBy(x => Guid.NewGuid()).ToList();

                    dataAll.AddRange(dataNew);
                }
                var pageList = new PageList<object>(dataAll, page - 1, pageSize);

                pageList.Take(5).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalPages,
                    pageList.totalCounts
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllLike(int post_id, int page = 1, int pageSize = 20)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var friends = await _redis.GetSetAsync($"user:{n}:friends");

                    /* Chuyển sang HashSet mục đích để tăng tốc tìm kiếm (contains) vì nếu dùng "List.Contains" thì tốc độ là "0(n)",
                     * còn nếu chuyển sang HashSet thì tốc độ sẽ là "0(1)" nên là rất nhanh
                     */
                    var friendIdFriends = friends.Select(int.Parse).ToHashSet();
                    var checkId = _context.likes.Where(x => x.Post_id == post_id && !x.deleted).Select(x => new
                    {
                        x.Post_id,
                        x.User_id,
                        x.user.Image,
                        x.user.FullName,
                        x.status,
                        x.url_icon,
                        check_friend = friendIdFriends.Contains(x.User_id.Value) ? true : false
                    }).ToList();

                    return await Task.FromResult(PayLoad<object>.Successfully(checkId));
                }

                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllShare(int post_id, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkId = _context.sheres.Where(x => x.Post_id == post_id).Select(x => new
                {
                    x.id,
                    x.Post_id,
                    x.User_id,
                    x.user.FullName,
                    x.user.Image,
                    UserFirstImage = x.post.user.Image,
                    UserFirstUserName = x.post.user.FullName,
                    x.post.Title,
                    x.post.Description,
                    imagePost = x.post.Post_Images.Select(x => x.Url).ToList()
                }).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOne(int id)
        {
            try
            {
                var checkId = _context.posts.FirstOrDefault(x => x.id == id && !x.deleted);
                if(checkId == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<ProductDTO>> Update(int id, ProductDTO productDTO)
        {
            try
            {
                var user = _userNameService.name();
                var checkId = _context.posts.FirstOrDefault(x => x.id == id && !x.deleted);
                var checkUser = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(user) && !x.deleted);

                if (checkId == null && checkUser == null)
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.DATANULL));

                var mapData = MapperData.GanData(checkId, productDTO);
                if(productDTO.Categoryid != 0)
                {
                    var checkCategory = _context.categories.FirstOrDefault(x => x.id == productDTO.Categoryid && !x.deleted);
                    if(checkCategory != null)
                    {
                        mapData.category = checkCategory;
                        mapData.Category_id = checkCategory.id;
                    }
                    
                }

                mapData.creator = checkUser.FullName + " update " + DateTime.UtcNow;

                _context.posts.Update(mapData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ProductDTO>.Successfully(productDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(ex.Message));
            }
        }

        //public async Task<PayLoad<object>> FindAllAI(string? name, int category, int page = 1, int pageSize = 20)
        //{
        //    try
        //    {

        //        var user = _userNameService.name();

        //        var data = category > 0 ? await _aIService.GetAi(new AIInput
        //        {
        //            UserId = int.Parse(user),
        //            CategoryId = category,
        //            TimeOnPage = 1,
        //            ScrolledToBottom = false,
        //            Liked = false,
        //            TimeOfDay = 1,
        //            Device = "Mobile"
        //        }) : 0;

        //        var dataMap = DataPosst(category, data, int.Parse(user));

        //        if (!string.IsNullOrEmpty(name))
        //            dataMap = dataMap.searchData(name, x => x.Title, x => x.Description);

        //        var pageList = new PageList<object>(dataMap, page - 1, pageSize);

        //        return await Task.FromResult(PayLoad<object>.Successfully(new
        //        {
        //            data = pageList,
        //            page,
        //            pageList.pageSize,
        //            pageList.totalCounts,
        //            pageList.totalPages
        //        }));
        //    }
        //    catch(Exception ex)
        //    {
        //        return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
        //    }
        //}
        public async Task<PayLoad<object>> FindAllAI(string? name, int category, int page = 1, int pageSize = 20)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var checkUser = _context.users.FirstOrDefault(x => x.id == n && !x.deleted);
                    if (checkUser == null) return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                    var keyCache = ConvertCacheKey.GetCacheKey(n);
                    // Lấy toàn bộ set 1 lần
                    var likePostDataId = _cache.SetMembers(keyCache)
                        .Select(x => (int)x)
                        .ToHashSet();

                    var dataAi = _context.behavioral_Analysess.Where(x => x.user_id == checkUser.id && !x.deleted)
                        .GroupBy(x => x.category_id)
                        .Select(x => new
                        {
                            id_category = x.Key, // "x.key" là id của "category_id" vì ở trên đã GroupBy theo trường dữ liệu "category_id" nên key ở đây sẽ là "category_id"
                            //id_category = x.First().category_id,
                            //id = x.Select(x => x.id),
                            maxTotal = x.Max(g => g.total)
                        }).OrderByDescending(x => x.maxTotal)
                        .Take(5)
                        .ToList();

                    var checkPostUserView = _context.articles_Viewedss.Where(x => x.user_id == n && !x.deleted).Select(x => x.post_id).ToList();

                    var categoryIds = dataAi.Select(x => x.id_category ?? 0).ToList();

                    if(dataAi.Count() < 5)
                    {
                        var dataCategoryNew = _context.categories.Where(x => !x.deleted && !categoryIds.Contains(x.id))
                            .Select(x => x.id)
                            .Take(5 - dataAi.Count()).ToList();

                        categoryIds.AddRange(dataCategoryNew);
                    }

                    var random = new Random();

                    int takeCount = random.Next(2, 6);

                    /*
            Status: 1 => Public
                    2 => Private
                    3 => Friend
         */
                    var dataPost = _context.posts.AsNoTracking().Where(x => dataAi.Count <= 5 ? !x.deleted && x.User_id != n 
                    && ((FriendIdDatas(n).Contains(x.User_id.Value) && x.Status == 3) || x.Status == 1) : 
                    (categoryIds.Contains(x.Category_id ?? 0) && !checkPostUserView.Contains(x.id) && x.User_id != n && !x.deleted 
                    && ((FriendIdDatas(n).Contains(x.User_id.Value) && x.Status == 3) || x.Status == 1))).Select(x => new PostResponse
                    {
                        Id = x.id,
                        Title = x.Title,
                        Description = x.Description,
                        Image = x.Post_Images.Select(i => i.Url),
                        Like = x.Likes.Where(l => !l.deleted).Count(),
                        //Like = x.Likes.Count(l => !l.deleted),
                        Tym = x.Likes.Count(l => l.status == 2 && !l.deleted),
                        Share = x.Sheres.Count(),
                        Comment = x.Comments.Count() + x.Comments.SelectMany(c => c.CommentDescriptions).Count(),
                        User = x.user.FullName,
                        Tag = x.TagPosts.Select(xt => new tagPostData
                        {
                            Id = xt.Tag_id.Value,
                            name = xt.tag == null ? "" : xt.tag.Name
                        }).ToList(),
                        UserId = x.user.id,
                        category_id = x.Category_id.Value,
                        category_name = x.category.Name,
                        UserImage = x.user.Image,
                        url_icon = x.Likes.Where(l => l.User_id == n && !l.deleted).Select(l => l.url_icon).FirstOrDefault(),
                        //isLike = x.Likes.FirstOrDefault(xl => xl.Post_id == x.id && xl.User_id == userId) != null ? true : false,
                        //isLike = x.Likes.Any(xl => xl.Post_id == x.id && xl.User_id == n && !xl.deleted),
                        isLike = likePostDataId.Contains(x.id),
                        Date = x.cretoredat,
                        Tag_Friend = x.tag_Friends.Where(t => t.user_id == x.User_id && t.post_id == x.id).Select(t => new tagPostData
                        {
                            Id = t.friend_id.Value,
                            name = t.friend.UserName
                        }).ToList(),
                        comment_Data = x.Comments.Select(c => new commentData
                        {
                            id = c.id,
                            id_post = c.Post_id.Value,
                            image_user = c.user.Image,
                            text = c.Description,
                            user_name = c.user.UserName,
                            total_CommentDescript = c.CommentDescriptions.Where(cd => cd.Comment_id == c.id && !cd.deleted).Count(),
                            url = c.Url
                        }).OrderByDescending(c => c.id).ThenBy(c => Guid.NewGuid()).Take(ranDomData()).ToList(),
                        iconCounts = x.Likes.AsQueryable().Where(l => l.Post_id == x.id && !l.deleted).GroupBy(l => l.url_icon).Select(l => new iconCount
                        {
                            // .AsQueryable() dùng cho Data lớn
                            icon = l.Key,
                            count = l.Count()
                            //count = l.Select(g => g.User_id).Distinct().Count(),
                        }).OrderByDescending(l => l.count).ToList(),
                        Status = x.Status
                    }).Select(x => new PostResponse
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        Image = x.Image,
                        Like = x.Like,
                        Tym = x.Tym,
                        Share = x.Share,
                        Comment = x.Comment,
                        User = x.User,
                        Tag = x.Tag,
                        UserId = x.UserId,
                        category_id = x.category_id,
                        category_name = x.category_name,
                        UserImage = x.UserImage,
                        url_icon = x.url_icon,
                        //isLike = x.Likes.FirstOrDefault(xl => xl.Post_id == x.id && xl.User_id == userId) != null ? true : false,
                        isLike = x.isLike,
                        Date = x.Date,
                        Tag_Friend = x.Tag_Friend,
                        Status = x.Status,
                        comment_Data = x.comment_Data,
                        iconCounts = x.iconCounts
                    }).ToList().OrderBy(x => Guid.NewGuid()).ToList(); // Lấy dữ liệu kiểu random

                    if (!string.IsNullOrEmpty(name))
                        dataPost = dataPost.Where(x => x.Title.Contains(name) || x.Description.Contains(name)).ToList();


                    var pageList = new PageList<object>(dataPost, page - 1, pageSize);

                    return PayLoad<object>.Successfully(new
                    {
                        data = pageList,
                        page,
                        pageList.pageSize,
                        pageList.totalPages,
                        pageList.totalCounts
                    });
                    //var categoryData = _context.categories.Where(x => categoryIds.Contains(x.id) && !x.deleted).ToDictionary(x => x.id);

                    //var userData = _context.users.Where(x => x.id == checkUser.id && !x.deleted).Select(x => x.UserName).FirstOrDefault();

                    //var allData = dataAi.Select(x => new
                    //{
                    //    x.id_category,
                    //    x.maxTotal,
                    //    category_name = categoryData.ContainsKey(x.id_category.Value) ? categoryData[x.id_category.Value].Name : "No Data",
                    //    userName = userData
                    //}).ToList();
                }

                return PayLoad<object>.CreatedFail(Status.DATANULL);
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private int ranDomData()
        {
            var random = new Random();

            int takeCount = random.Next(2, 6);
            return takeCount;
        }
        private List<PostResponse> DataPosst(int category, float number, int userId)
        {
            if (category == 0)
            {
                var  dataAll = _context.posts.Where(x => !x.deleted).OrderByDescending(x => x.cretoredat).Select(x => new PostResponse
                {
                    Id = x.id,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Post_Images.Select(i => i.Url),
                    Like = x.Likes.Count(l => l.status == 1),
                    Tym = x.Likes.Count(l => l.status == 2),
                    Share = x.Sheres.Count(),
                    Comment = x.Comments.Count() + x.Comments.SelectMany(c => c.CommentDescriptions).Count(),
                    User = x.user.FullName,
                    Tag = x.TagPosts.Where(xt => xt.Post_id == x.id).Select(xt => new tagPostData
                    {
                        Id = xt.Tag_id.Value,
                        name = xt.tag == null ? "" : xt.tag.Name
                    }).ToList(),
                    UserId = x.user.id,
                    category_id = x.Category_id.Value,
                    category_name = x.category.Name,
                    UserImage = x.user.Image,
                    //isLike = x.Likes.FirstOrDefault(xl => xl.Post_id == x.id && xl.User_id == userId) != null ? true : false,
                    isLike = x.Likes.Any(xl => xl.Post_id == x.id && xl.User_id == userId),
                    Date = x.cretoredat
                }).ToList();

                return dataAll;
            }
            var data = number > 0.85 ? _context.posts.Where(x => x.Category_id == category && !x.deleted).Select(x => new PostResponse
                                                {
                                                    Id = x.id,
                                                    Title = x.Title,
                                                    Description = x.Description,
                                                    Image = x.Post_Images.Select(i => i.Url),
                                                    Like = x.Likes.Count(l => l.status == 1),
                                                    Tym = x.Likes.Count(l => l.status == 2),
                                                    Share = x.Sheres.Count(),
                                                    Comment = x.Comments.Count() + x.Comments.SelectMany(c => c.CommentDescriptions).Count(),
                                                    User = x.user.FullName,
                                                    Tag = x.TagPosts.Where(xt => xt.Post_id == x.id).Select(xt => new tagPostData
                                                    {
                                                        Id = xt.Tag_id.Value,
                                                        name = xt.tag == null ? "" : xt.tag.Name
                                                    }).ToList(),
                                                    UserId = x.user.id,
                                                    category_id = x.Category_id.Value,
                                                    category_name = x.category.Name,
                                                    UserImage = x.user.Image,
                                                    //isLike = x.Likes.FirstOrDefault(xl => xl.Post_id == x.id && xl.User_id == userId) != null ? true : false,
                                                    isLike = x.Likes.Any(xl => xl.Post_id == x.id && xl.User_id == userId),
                                                    Date = x.cretoredat
                                                }).ToList()
                                     : _context.posts.Where(x => x.Category_id != category && !x.deleted).Select(x => new PostResponse
                                     {
                                         Id = x.id,
                                         Title = x.Title,
                                         Description = x.Description,
                                         Image = x.Post_Images.Select(i => i.Url),
                                         Like = x.Likes.Count(l => l.status == 1),
                                         Tym = x.Likes.Count(l => l.status == 2),
                                         Share = x.Sheres.Count(),
                                         Comment = x.Comments.Count() + x.Comments.SelectMany(c => c.CommentDescriptions).Count(),
                                         Tag = x.TagPosts.Where(xt => xt.Post_id == x.id).Select(xt => new tagPostData
                                         {
                                             Id = xt.Tag_id.Value,
                                             name = xt.tag == null ? "" : xt.tag.Name
                                         }).ToList(),
                                         User = x.user.FullName,
                                         category_id = x.Category_id.Value,
                                         category_name = x.category.Name,
                                         UserId = x.user.id,
                                         UserImage = x.user.Image,
                                         //isLike = x.Likes.FirstOrDefault(xl => xl.Post_id == x.id && xl.User_id == userId) != null ? true : false,
                                         isLike = x.Likes.Any(xl => xl.Post_id == x.id && x.User_id == userId),
                                         Date = x.cretoredat
                                     }).ToList();

            return data;
        }

        public async Task<PayLoad<object>> FindAllComment(int post_id, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkComment = _context.comments.Where(x => x.Post_id == post_id).Select(x => new
                {
                    x.id,
                    x.Url,
                    x.Description,
                    x.User_id,
                    x.user.FullName,
                    x.user.Image,
                    x.cretoredat,
                    rep1 = x.CommentDescriptions.Where(x1 => x1.Comment_id == x.id).Select(x1 => new
                    {
                        x1.id,
                        x1.User_id,
                        x1.user.FullName,
                        x1.user.Image,
                        x1.descript,
                        x1.Url,
                        x1.cretoredat,
                        rep2 = x1.commentDescriptions2.Where(x2 => x2.CommentDescription_id == x1.id).Select(x2 => new
                        {
                            x2.id,
                            x2.User_id,
                            x2.user.FullName,
                            x2.user.Image,
                            x2.descript,
                            x2.Url,
                            x2.cretoredat
                        }).ToList()
                    }).ToList()
                }).ToList();

                var pageList = new PageList<object>(checkComment, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllByUser(int userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkUser = _context.users.FirstOrDefault(x => x.id == userId && !x.deleted);
                if (checkUser == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                var checkPost = _context.posts.Where(x => x.User_id == checkUser.id && !x.deleted).Select(x => new
                {
                    Id = x.id,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Post_Images.Select(i => i.Url),
                    Like = x.Likes.Count(l => l.status == 1),
                    Tym = x.Likes.Count(l => l.status == 2),
                    Share = x.Sheres.Count(),
                    Comment = x.Comments.Count() + x.Comments.Sum(c => c.CommentDescriptions.Count),
                    User = x.user.FullName,
                    UserId = x.user.id,
                    UserImage = x.user.Image,
                    Date = x.cretoredat,
                    Post = x,
                    Type = "Post",
                    UserPostShareId = (int?)0,
                    UserPostShareFullName = x.Sheres.FirstOrDefault(x1 => x1.Post_id == x.id).user.FullName,
                    UserPostShareDate = x.Sheres.FirstOrDefault(x1 => x1.Post_id == x.id).cretoredat,

                    UserPostShareChuDate = x.Sheres.FirstOrDefault(x1 => x1.Post_id == x.id).post.cretoredat,
                    UserPostShareChuImage = x.Sheres.FirstOrDefault(x1 => x1.Post_id == x.id).post.user.Image,
                    UserPostShareChuFullName = x.Sheres.FirstOrDefault(x1 => x1.Post_id == x.id).post.user.FullName,
                    UserPostShareDateChuId = x.Sheres.FirstOrDefault(x1 => x1.Post_id == x.id).post.user.id,
                    PostShareDateChuTitle = x.Sheres.FirstOrDefault(x1 => x1.Post_id == x.id).post.Title,
                    PostShareDateChuDescription = x.Sheres.FirstOrDefault(x1 => x1.Post_id == x.id).post.Description,
                    PostShareDateChuImage = x.Sheres.FirstOrDefault(x1 => x1.Post_id == x.id).post.Post_Images.Select(x => x.Url).ToList(),
                    ActionDate = x.cretoredat

                    /*
                        Nếu những trường dữ liệu ở bảng PostShare mà ở bảng Post này không có thì có thể để như sau
                         Ví dụ: PostShareDateChuImage = null (Có thể để như này)
                                UserPostShareDateChuId = null
                        
                     */
                });

                var sharePost = from ps in _context.sheres
                                join p in _context.posts on ps.Post_id equals p.id
                                where ps.User_id == userId && !p.deleted && !ps.deleted
                                select new
                                {
                                    Id = p.id,
                                    Title = string.Empty,
                                    Description = p.Description,
                                    Image = p.Post_Images.Select(i => i.Url),
                                    Like = p.Likes.Count(l => l.status == 1),
                                    Tym = p.Likes.Count(l => l.status == 2),
                                    Share = p.Sheres.Count(),
                                    Comment = p.Comments.Count() + p.Comments.Sum(c => c.CommentDescriptions.Count),
                                    User = p.user.FullName,
                                    UserId = p.user.id,
                                    UserImage = p.user.Image,
                                    Date = p.cretoredat,
                                    Post = p,
                                    Type = "Post",
                                    UserPostShareId = ps.User_id,
                                    UserPostShareFullName = ps.user.FullName,
                                    UserPostShareDate = ps.cretoredat,

                                    UserPostShareChuDate = ps.post.cretoredat,
                                    UserPostShareChuImage = ps.post.user.Image,
                                    UserPostShareChuFullName = ps.post.user.FullName,
                                    UserPostShareDateChuId = ps.post.user.id,
                                    PostShareDateChuTitle = ps.post.Title,
                                    PostShareDateChuDescription = ps.post.Description,
                                    PostShareDateChuImage = ps.post.Post_Images.Select(x => x.Url).ToList(),
                                    ActionDate = ps.cretoredat

                                    /*
                                        Nếu những trường dữ liệu ở bảng Post này mà có mà ở bảng PostShare này không có thì có thể để như sau
                                         Ví dụ: Comment = null (Có thể để như này),
                                                Title = null

                                     */
                                };

                var postLastData = checkPost.Union(sharePost).OrderByDescending(x => x.Post.cretoredat).ToList(); // Gộp dữ liệu 2 bảng vào với nhau

                var pageList = new PageList<object>(postLastData, page - 1, pageSize);
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        
    }
}
