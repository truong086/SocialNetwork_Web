using AutoMapper;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Mapper
{
    public class IconMapper : Profile
    {
        public IconMapper()
        {
            CreateMap<IconDTO, Icon>();
            CreateMap<Icon, IconDTO>();
        }
    }
}
