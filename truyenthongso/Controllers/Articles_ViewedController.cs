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
    public class Articles_ViewedController : ControllerBase
    {
        private readonly IArticles_ViewedService _service;
        public Articles_ViewedController(IArticles_ViewedService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<object>> Add (Articles_ViewedDTO data)
        {
            return await _service.Add(data);
        }
    }
}
