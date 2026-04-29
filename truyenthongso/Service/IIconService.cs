using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface IIconService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 10);
        Task<PayLoad<IconDTO>> Add (IconDTO iconDTO);
        Task<PayLoad<IconDTO>> Update (int id, IconDTO iconDTO);
        Task<PayLoad<string>> Delete (int id);
        Task<PayLoad<object>> FindIOne(int id);
    }
}
