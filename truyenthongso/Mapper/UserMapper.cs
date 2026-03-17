using AutoMapper;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserDTO, User>();
            CreateMap<User, UserDTO>();
        }
    }
}
