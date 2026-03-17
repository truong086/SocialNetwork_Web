using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface IRoleService
    {
        Task<PayLoad<RoleDTO>> Add(RoleDTO roleDTO);
        Task<PayLoad<GroupRoleDTO>> AddGroupRole(GroupRoleDTO data);
        Task<PayLoad<GroupRoleDTO>> UpdateGroupRole(int id, GroupRoleDTO data);
        Task<PayLoad<RoleDTO>> Update(int id, RoleDTO roleDTO);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 5);
        Task<PayLoad<object>> FindAllGroupRole();
        Task<PayLoad<object>> FindOne(int id);
        Task<PayLoad<string>> Delete(int id);
    }
}
