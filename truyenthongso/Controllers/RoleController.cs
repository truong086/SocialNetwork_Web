using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using truyenthongso.Common;
using truyenthongso.Service;
using truyenthongso.ViewModel;

namespace truyenthongso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 10)
        {
            return await _roleService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOne))]
        public async Task<PayLoad<object>> FindOne(int id)
        {
            return await _roleService.FindOne(id);
        }

        [HttpGet]
        [Route(nameof(FindAllGroupRole))]
        public async Task<PayLoad<object>> FindAllGroupRole()
        {
            return await _roleService.FindAllGroupRole();
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<RoleDTO>> Add(RoleDTO data)
        {
            return await _roleService.Add(data);
        }

        [HttpPost]
        [Route(nameof(AddGroupRole))]
        public async Task<PayLoad<GroupRoleDTO>> AddGroupRole(GroupRoleDTO data)
        {
            return await _roleService.AddGroupRole(data);
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<RoleDTO>> Update(int id, RoleDTO data)
        {
            return await _roleService.Update(id, data);
        }

        [HttpPut]
        [Route(nameof(UpdateGroupRole))]
        public async Task<PayLoad<GroupRoleDTO>> UpdateGroupRole(int id, GroupRoleDTO data)
        {
            return await _roleService.UpdateGroupRole(id, data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        public async Task<PayLoad<string>> Delete(int id)
        {
            return await _roleService.Delete(id);
        }
    }
}
