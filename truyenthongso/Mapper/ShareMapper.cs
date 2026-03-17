using AutoMapper;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Mapper
{
    public class ShareMapper : Profile
    {
        public ShareMapper()
        {
            CreateMap<ShareDTO, Shere>();
            CreateMap<Shere, ShareDTO>();
        }
    }
}
