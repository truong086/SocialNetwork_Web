using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class UserNameService : IUserNameService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserNameService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.SignOutAsync();
        }

        public string name()
        {
            string value = string.Empty;
            if (_httpContextAccessor != null)
            {
                value = _httpContextAccessor.HttpContext.User.FindFirstValue(Status.IDAUTHENTICATION);
            }

            return value;
        }

    }
}
