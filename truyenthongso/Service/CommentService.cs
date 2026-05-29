using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using truyenthongso.ChatHub;
using truyenthongso.Clouds;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class CommentService : ICommentService
    {
        private readonly DBContext _context;
        private readonly IUserNameService _userNameService;
        private readonly IHubContext<CommentHub> _hubContext;
        private readonly Cloud _cloud;
        public CommentService(DBContext context, IUserNameService userNameService, IHubContext<CommentHub> hub, IOptions<Cloud> cloud)
        {
            _context = context;
            _userNameService = userNameService;
            _hubContext = hub;
            _cloud = cloud.Value;
        }
        public async Task<PayLoad<CommentDTo>> AddComment(CommentDTo data)
        {
            try
            {
                if (int.TryParse(_userNameService.name(), out int n))
                {
                    var checkUser = _context.users.FirstOrDefault(x => x.id == n && !x.deleted);
                    var checkPost = _context.posts.FirstOrDefault(x => x.id == data.post_id && !x.deleted);
                    if (checkUser == null || checkPost == null) return await Task.FromResult(PayLoad<CommentDTo>.CreatedFail(Status.DATANULL));

                    var commentAdd = new Comment();
                    commentAdd.post = checkPost;
                    commentAdd.Post_id = checkPost.id;
                    commentAdd.user = checkUser;
                    commentAdd.User_id = checkUser.id;
                    commentAdd.Description = data.description;

                    if (data.file != null)
                    {
                        uploadCloud.CloudInaryIFromAccount(data.file, checkUser.UserName + "_comment_social" + checkUser.id.ToString(), _cloud);
                        commentAdd.Url = uploadCloud.Link;
                        commentAdd.PublicId_id = uploadCloud.publicId;
                    }

                    _context.comments.Add(commentAdd);

                    if(await _context.SaveChangesAsync() > 0)
                    {
                        var dataNew = _context.comments.Where(x => !x.deleted).OrderByDescending(x => x.id).FirstOrDefault();

                        await _hubContext.Clients.Group(checkPost.User_id.ToString())
                            .SendAsync("comment_new", new
                            {
                                id = dataNew.id,
                                id_post = checkPost.id,
                                image_user = checkUser.Image,
                                text = data.description,
                                user_name = checkUser.UserName,
                                url = dataNew.Url
                            });

                        await _hubContext.Clients.Group("comment_" + checkPost.id.ToString())
                            .SendAsync("comment_new_post", new
                            {
                                id = dataNew.id,
                                id_post = checkPost.id,
                                image_user = checkUser.Image,
                                text = data.description,
                                user_name = checkUser.UserName,
                                url = dataNew.Url
                            });

                        return await Task.FromResult(PayLoad<CommentDTo>.Successfully(data));
                    }

                    return await Task.FromResult(PayLoad<CommentDTo>.CreatedFail(Status.DBERROR));
                }

                return await Task.FromResult(PayLoad<CommentDTo>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<CommentDTo>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int post_id, int page = 1, int pageSize = 10)
        {
            try
            {
                var data = _context.comments.Where(x => x.Post_id == post_id && !x.deleted).Select(x => new
                {
                    id = x.id,
                    id_post = x.id,
                    image_user = x.user.Image,
                    text = x.Description,
                    user_name = x.user.UserName
                }).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.text.Contains(name) || x.user_name.Contains(name)).ToList();

                var pageList = new PageList<object>(data, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
