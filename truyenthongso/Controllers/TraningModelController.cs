using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truyenthongso.Common;
using truyenthongso.PythonAI;
using truyenthongso.Service;
using truyenthongso.ViewModel;

namespace truyenthongso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraningModelController : ControllerBase
    {
        private readonly ITraningModelService _service;
        public TraningModelController(ITraningModelService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<object>> Add (List<TraningModel> model)
        {
            return await _service.Add(model);
        }

        [HttpPost]
        [Route(nameof(AddAiNew))]
        public async Task<PayLoad<object>> AddAiNew(List<AIInput> model)
        {
            return await _service.AddAiNew(model);
        }
    }
}
