using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface IUserService
    {
        Task<PayLoad<object>> Login (LoginDTO userDTO);
        Task<PayLoad<string>> Logout();
        Task<PayLoad<UserDTO>> Add (UserDTO userDTO);
        Task<PayLoad<UserDTO>> Update (int id, UserDTO userDTO);
        Task<PayLoad<string>> Delete (int id);
        Task<PayLoad<string>> DeleteAccountNoAction();
        Task<PayLoad<string>> Action(ActionUser data);
        Task<PayLoad<string>> GenToken (string email);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOne(int id);
    }
}
