using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using truyenthongso.Service;
using truyenthongso.ViewModel;

namespace truyenthongso.ChatHub
{
    [Authorize]
    public class CommentHub : Hub
    {
        private readonly IUserNameService _user;
        public CommentHub(IUserNameService user)
        {
            _user = user;
        }
        public async Task JoinPost(string postId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var test = _user.name();
            //await Groups.AddToGroupAsync(Context.ConnectionId, postId);
            await Groups.AddToGroupAsync(Context.ConnectionId, userId == null || userId == "" ? "0" : userId);
        }

        public async Task JoinPostData(string postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, postId);

        }

        public async Task LeavePost(string postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId);
        }
    }
}
