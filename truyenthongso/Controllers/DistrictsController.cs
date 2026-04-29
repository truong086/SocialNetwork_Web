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
    public class DistrictsController : ControllerBase
    {
        private readonly IDistrictsService _districtsService;
        public DistrictsController(IDistrictsService districtsService)
        {
            _districtsService = districtsService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _districtsService.FindAll(name, page, pageSize);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            return await _districtsService.FindOneId(id);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindOneByCommunes))]
        public async Task<PayLoad<object>> FindOneByCommunes(int id)
        {
            return await _districtsService.FindOneByCommunes(id);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<CityDTO>> Add(CityDTO data)
        {
            return await _districtsService.Add(data);
        }

        [HttpPut, Route(nameof(Update))]
        public async Task<PayLoad<CityDTO>> Update(int id,CityDTO data)
        {
            return await _districtsService.Update(id, data);    
        }

        [HttpDelete, Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete (int id)
        {
            return await (_districtsService.Delete(id));
        }
    }
}
