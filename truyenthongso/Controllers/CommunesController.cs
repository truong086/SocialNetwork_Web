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
    public class CommunesController : ControllerBase
    {
        private readonly ICommunesService _communesService;
        public CommunesController(ICommunesService communesService)
        {
            _communesService = communesService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _communesService.FindAll(name, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<CityDTO>> Add(CityDTO cityDTO)
        {
            return await _communesService.AddCommunes(cityDTO);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<CityDTO>> Update(int id, CityDTO data)
        {
            return await _communesService.Update(id, data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _communesService.Delete(id);
        }
    }
}
