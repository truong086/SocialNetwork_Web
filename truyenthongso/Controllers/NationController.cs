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
    public class NationController : ControllerBase
    {
        private readonly INationService _nationService;
        public NationController(INationService nationService)
        {
            _nationService = nationService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _nationService.FindAll(name, page, pageSize);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            return await _nationService.FindOneId(id);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindByCity))]
        public async Task<PayLoad<object>> FindByCity(int id)
        {
            return await _nationService.FindByCity(id);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<CityDTO>> Update(int id, CityDTO data)
        {
            return await _nationService.Update(id, data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _nationService.Delete(id);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<CityDTO>> Add (CityDTO city)
        {
            return await _nationService.Add(city);
        }
    }
}
