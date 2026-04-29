using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface IDistrictsService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOneId(int id);
        Task<PayLoad<CityDTO>> Add (CityDTO city);
        Task<PayLoad<CityDTO>> Update(int id, CityDTO city);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindOneByCommunes(int id);
    }
}
