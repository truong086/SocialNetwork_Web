using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truyenthongso.Common;
using truyenthongso.Service;
using truyenthongso.ViewModel;

namespace truyenthongso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route(nameof(findAll))]
        public async Task<PayLoad<object>> findAll (string? name, int page = 1, int pageSize = 20)
        {
            return await _userService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOne))]
        public async Task<PayLoad<object>> FindOne(int id)
        {
            return await _userService.FindOne(id);
        }

        [HttpGet]
        [Route(nameof(FindAllSearchFriend))]
        public async Task<PayLoad<object>> FindAllSearchFriend(string? name)
        {
            return await _userService.FindAllSearchFriend(name);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<UserDTO>> Add(UserDTO userDTO)
        {
            return await _userService.Add(userDTO);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<UserDTO>> Update(int id, UserDTO userDTO)
        {
            return await _userService.Update(id, userDTO);
        }

        [HttpDelete]
        [Route(nameof(Update))]
        public async Task<PayLoad<string>> Delete (int id)
        {
            return await _userService.Delete(id);
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<PayLoad<object>> Login(LoginDTO login)
        {
            return await _userService.Login(login);
        }

        [HttpPost]
        [Route(nameof(Action))]
        public async Task<PayLoad<string>> Action([FromForm] ActionUser data)
        {
            return await _userService.Action(data);
        }

        [HttpPost]
        [Route(nameof(GenToken))]
        public async Task<PayLoad<string>> GenToken(string data)
        {
            return await _userService.GenToken(data);
        }
    }
}
