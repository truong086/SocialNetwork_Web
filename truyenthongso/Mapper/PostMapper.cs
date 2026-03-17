using AutoMapper;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Mapper
{
    public class PostMapper : Profile
    {
        public PostMapper()
        {
            CreateMap<Post, ProductDTO>();
            CreateMap<ProductDTO, Post>();
        }
    }
}
