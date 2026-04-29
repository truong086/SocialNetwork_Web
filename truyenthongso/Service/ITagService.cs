using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface ITagService
    {
        Task<PayLoad<TagDTO>> Add (TagDTO tagDTO);
        Task<PayLoad<TagDTO>> Update (int id, TagDTO tagDTO);
        Task<PayLoad<string>> Delete (int id);
        Task<PayLoad<object>> FindAll (string? name, int page = 1, int pageSize = 20);
    }
}
