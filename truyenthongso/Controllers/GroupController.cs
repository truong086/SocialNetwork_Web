using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truyenthongso.Common;
using truyenthongso.Service;
using truyenthongso.ViewModel;

namespace truyenthongso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        [Route(nameof(findAll))]
        public async Task<PayLoad<object>> findAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _groupService.FindAll(name, page, pageSize);   
        }

        [HttpGet]
        [Route(nameof(findOne))]
        public async Task<PayLoad<object>> findOne(int id)
        {
            return await _groupService.FindOne(id);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<GroupDTO>> Add(GroupDTO data)
        {
            return await _groupService.Add(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<GroupDTO>> Update(int id, GroupDTO data)
        {
            return await _groupService.Update(id, data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _groupService.Delete(id);
        }
    }
}
