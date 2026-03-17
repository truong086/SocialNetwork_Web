using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface ICategoryService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOne(int id);
        Task<PayLoad<CategoryDTO>> Add(CategoryDTO data);
        Task<PayLoad<CategoryDTO>> Update(int id, CategoryDTO data);
        Task<PayLoad<string>> Delete(int id);
    }
}
