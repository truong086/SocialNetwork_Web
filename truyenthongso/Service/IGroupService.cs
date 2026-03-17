using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface IGroupService
    {
        Task<PayLoad<GroupDTO>> Add(GroupDTO data);
        Task<PayLoad<GroupDTO>> Update(int id, GroupDTO data);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOne(int id);
    }
}
