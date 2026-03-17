using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truyenthongso.Common;
using truyenthongso.PythonAI;

namespace truyenthongso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiGentsController : ControllerBase
    {
        private readonly IAIGentsService _service;
        public AiGentsController(IAIGentsService service)
        {
            _service = service;
        }
        [HttpPost]
        [Route(nameof(AIGents))]
        public async Task<PayLoad<object>> AIGents(IFormFile file)
        {
            return await _service.AIGents(file);
        }

        [HttpPost]
        [Route(nameof(AIGentsTiengTrung))]
        public async Task<PayLoad<object>> AIGentsTiengTrung(string capdo, string type, string tu)
        {
            return await _service.AIGentsTiengTrung(capdo, type, tu);
        }
    }
}
