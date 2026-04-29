using MimeKit.Encodings;
using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface ICityService
    {
        Task<PayLoad<object>> FindAll (string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<CityDTO>> Add (CityDTO cityDTO);
        Task<PayLoad<CityDTO>> Update (int id, CityDTO cityDTO);
        Task<PayLoad<string>> Delete (int id);
        Task<PayLoad<object>> FindOneId(int id);
        Task<PayLoad<object>> FindByDistris(int id);
        Task<PayLoad<object>> FindByCommunes(int id);
    }
}
