using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface IFriendShipService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1,int pageSize = 20);    
        Task<PayLoad<object>> FindOneId(int id);    
        Task<PayLoad<object>> TestRedis();    
        Task<PayLoad<FriendShipDTO>> AddFriend(FriendShipDTO data);    
        Task<PayLoad<object>> Suggestafriend(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<List<SuggestionDto>>> GetSuggestions();
    }
}
