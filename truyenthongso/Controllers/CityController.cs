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
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _cityService.FindAll(name, page, pageSize);    
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            return await _cityService.FindOneId(id);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindByDistris))]
        public async Task<PayLoad<object>> FindByDistris(int id)
        {
            return await _cityService.FindByDistris(id);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(FindByCommunes))]
        public async Task<PayLoad<object>> FindByCommunes(int id)
        {
            return await _cityService.FindByCommunes(id);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<CityDTO>> Add (CityDTO cityDTO)
        {
            return await _cityService.Add(cityDTO);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<CityDTO>> Update (int id, CityDTO cityDTO)
        {
            return await _cityService.Update(id, cityDTO);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _cityService.Delete(id);
        }
    }
}
