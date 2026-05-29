using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface IProductService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllAI(string? name, int category, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllShare(int post_id, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllLike(int post_id, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllComment(int post_id, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllByUser(int userId, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOne(int id);
        Task<PayLoad<PostResponse>> Add (ProductDTO productDTO);
        Task<PayLoad<LikeDTO>> AddLike (LikeDTO likeDTO);
        Task<PayLoad<ShareDTO>> AddShere (ShareDTO shareDTO);
        Task<PayLoad<ProductDTO>> Update (int id, ProductDTO productDTO);
        Task<PayLoad<string>> Delete (int id);
    }
}
