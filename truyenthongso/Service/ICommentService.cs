using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface ICommentService
    {
        Task<PayLoad<CommentDTo>> AddComment(CommentDTo data);
        Task<PayLoad<object>> FindAll(string? name, int post_id, int page = 1, int pageSize = 10);
    }
}
