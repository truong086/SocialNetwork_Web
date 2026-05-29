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
    public class FriendShipController : ControllerBase
    {
        private readonly IFriendShipService _service;
        public FriendShipController(IFriendShipService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _service.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            return await _service.FindOneId(id);
        }

        [HttpGet]
        [Route(nameof(Suggestafriend))]
        public async Task<PayLoad<object>> Suggestafriend(string? name, int page = 1, int pageSize = 20)
        {
            return await _service.Suggestafriend(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(GetSuggestions))]
        public async Task<PayLoad<List<SuggestionDto>>> GetSuggestions()
        {
            return await _service.GetSuggestions();
        }

        [HttpGet]
        [Route(nameof(TestRedis))]
        public async Task<PayLoad<object>> TestRedis()
        {
            return await _service.TestRedis();
        }

        [HttpGet]
        [Route(nameof(FindOneIdByUserInfo))]
        public async Task<PayLoad<InfoView>> FindOneIdByUserInfo(int id)
        {
            return await _service.FindOneIdByUserInfo(id);
        }

        [HttpPost]
        [Route(nameof(AddFriend))]
        public async Task<PayLoad<FriendShipDTO>> AddFriend(FriendShipDTO data)
        {
            return await _service.AddFriend(data);
        }
    }
}
