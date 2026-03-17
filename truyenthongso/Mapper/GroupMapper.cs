using AutoMapper;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Mapper
{
    public class GroupMapper : Profile
    {
        public GroupMapper()
        {
            CreateMap<Group, GroupDTO>();
            CreateMap<GroupDTO, Group>();
        }
    }
}
