using truyenthongso.Common;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public interface ICommunesService
    {
        Task<PayLoad<CityDTO>> AddCommunes(CityDTO cityDTO);
        Task<PayLoad<CityDTO>> Update(int id, CityDTO cityDTO);
        Task<PayLoad<string>> Delete(int id);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
    }
}
