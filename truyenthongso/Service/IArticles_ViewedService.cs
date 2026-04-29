using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface IArticles_ViewedService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> Add(Articles_ViewedDTO data);
        Task<PayLoad<object>> Update(int id, Articles_ViewedDTO data);
        Task<PayLoad<string>> Delete(int id);
    }
}
