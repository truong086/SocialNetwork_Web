using AutoMapper;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
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
        public ProductService(DBContext context, IUserNameService userNameService,
            IMapper mapper, IOptions<Cloud> cloud, IAIService aIService,
            IConnectionMultiplexer redis)
        {
            _context = context;
            _userNameService = userNameService;
            _mapper = mapper;
            _cloud = cloud.Value;
            _aIService = aIService;
            _cache  = redis.GetDatabase();
        }
        public async Task<PayLoad<ProductDTO>> Add(ProductDTO productDTO)
        {
            try
            {
                var user = _userNameService.name();
                var checkUser = _context.users.FirstOrDefault(x => x.id == int.Parse(user) && !x.deleted);
                var checkCategory = _context.categories.FirstOrDefault(x => x.id == productDTO.Categoryid && !x.deleted);
                if (checkUser == null || checkCategory == null)
                    return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.DATANULL));


                var mapData = _mapper.Map<Post>(productDTO);
                
                mapData.User_id = checkUser.id;
                mapData.Category_id = checkCategory.id;
                mapData.user = checkUser;
                mapData.category = checkCategory;

                _context.posts.Add(mapData);
                _context.SaveChanges();

                var dataUserTagPost = FormatTagUser.FomatData(mapData.Description == null || mapData.Description == "" ? "" : mapData.Description);

                if(productDTO.images != null && productDTO.images.Count > 0)
                {
                    var postNew = _context.posts.OrderByDescending(x => x.id).FirstOrDefault();
                    var listImage = new List<Post_Image>();
                    foreach (var image in productDTO.images)
                    {
                        uploadCloud.CloudInaryIFromAccount(image, checkUser.FullName + "" + postNew.id, _cloud);
                        var newDataImage = new Post_Image()
                        {
                            post = postNew,
                            Post_id = postNew.id,
                            PublicId = uploadCloud.publicId,
                            Url = uploadCloud.Link
                        };
                        listImage.Add(newDataImage);
                    }

                    _context.postImages.AddRange(listImage);
                    _context.SaveChanges();

                }

                if(dataUserTagPost != null && dataUserTagPost.Count > 0)
                {
                    var dataNew = _context.posts.OrderByDescending(x => x.id).FirstOrDefault();
                    if (dataNew == null)
                        return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(Status.DATANULL));

                    // Truy vấn lấy ra tất cả User có id trong "dataUserTagPost"
                    var userTagged = _context.users.Where(x => dataUserTagPost.Contains(x.id)).ToList();

                    // Tạo danh sách PostUserTag từ User đã truy vấn ở trên( userTagged )
                    var listUserData = userTagged.Select(x => new PostUserTag
                    {
                        post_id = dataNew.id,
                        user_id = x.id
                    }).ToList();

                    if (listUserData.Any())
                    {
                        _context.postUserTags.AddRange(listUserData);
                        _context.SaveChanges();
                    }
                }

                return await Task.FromResult(PayLoad<ProductDTO>.Successfully(productDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ProductDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<LikeDTO>> AddLike(LikeDTO likeDTO)
        {
            try
            {
                var user = _userNameService.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(user) && !x.deleted && x.Action);
                var checkPost = _context.posts.FirstOrDefault(x => x.id == likeDTO.Post_id && !x.deleted);
                if (checkAccount == null || checkPost == null)
                    return await Task.FromResult(PayLoad<LikeDTO>.CreatedFail(Status.DATANULL));

                var checKLike = _context.likes.FirstOrDefault(x => x.User_id == int.Parse(user) && x.Post_id == likeDTO.Post_id && !x.deleted);
                if(checKLike == null)
                {
                    _context.likes.Add(new Like
                    {
                        post = checkPost,
                        Post_id = checkPost.id,
                        user = checkAccount,
                        User_id = checkAccount.id,
                        status = 1
                    });
                    _cache.SetAdd(ConvertCacheKey.GetCacheKey(int.Parse(user)), likeDTO.Post_id);
                }
                else
                {
                    checKLike.deleted = true;
                    _cache.SetRemove(ConvertCacheKey.GetCacheKey(int.Parse(user)), likeDTO.Post_id);
                }

                _context.SaveChanges();

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

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.posts.Where(x => !x.deleted).ToList();
                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.Title.Contains(name) && !x.deleted).ToList();

                var pageList = new PageList<object>(data, page - 1, pageSize);

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

        public async Task<PayLoad<object>> FindAllLike(int post_id, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkId = _context.likes.Where(x => x.Post_id == post_id).Select(x => new
                {
                    x.Post_id,
                    x.User_id,
                    x.user.Image,
                    x.user.FullName,
                    x.status
                }).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
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

        public async Task<PayLoad<object>> FindAllAI(string? name, int category, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _userNameService.name();

                var data = await _aIService.GetAi(new AIInput
                {
                    UserId = int.Parse(user),
                    CategoryId = category,
                    TimeOnPage = 155,
                    ScrolledToBottom = true,
                    Liked = true,
                    TimeOfDay = 20,
                    Device = "Mobile"
                });

                var dataMap = DataPosst(category, data);
                if (!string.IsNullOrEmpty(name))
                    dataMap = dataMap.Where(x => x.Title.Contains(name)).ToList();

                var pageList = new PageList<object>(dataMap, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<PostResponse> DataPosst(int category, float number)
        {
            var data = number > 0.85 ? _context.posts.Where(x => x.Category_id == category && !x.deleted).Select(x => new PostResponse
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
                                         Comment = x.Comments.Count() + x.Comments.Sum(c => c.CommentDescriptions.Count),
                                         User = x.user.FullName,
                                         UserId = x.user.id,
                                         UserImage = x.user.Image,
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
