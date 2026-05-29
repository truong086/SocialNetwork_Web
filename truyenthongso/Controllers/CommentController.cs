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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _comment;
        public CommentController(ICommentService comment)
        {
            _comment = comment;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int post_id, int page = 1, int pageSize = 10)
        {
            return await _comment.FindAll(name, post_id, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(AddComment))]
        public async Task<PayLoad<CommentDTo>> AddComment([FromForm]CommentDTo data)
        {
            return await _comment.AddComment(data);
        }
    }
}
