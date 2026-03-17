using AutoMapper;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Mapper
{
    public class RoleMapper : Profile
    {
        public RoleMapper()
        {
            CreateMap<Role, RoleDTO>();
            CreateMap<RoleDTO, Role>();
        }
    }
}
