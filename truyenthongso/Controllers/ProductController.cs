using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truyenthongso.Common;
using truyenthongso.Service;
using truyenthongso.ViewModel;

namespace truyenthongso.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route(nameof(FindAllAI))]
        public async Task<PayLoad<object>> FindAllAI(string? name, int category, int page = 1, int pageSize = 10)
        {
            return await _productService.FindAllAI(name, category, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 10)
        {
            return await _productService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllLike))]
        public async Task<PayLoad<object>> FindAllLike(int posst_id, int page = 1, int pageSize = 10)
        {
            return await _productService.FindAllLike(posst_id, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<PostResponse>> Add([FromForm]ProductDTO data)
        {
            return await _productService.Add(data);
        }

        [HttpPost]
        [Route(nameof(AddLike))]
        public async Task<PayLoad<LikeDTO>> AddLike(LikeDTO data)
        {
            return await _productService.AddLike(data);
        }
    }
}
