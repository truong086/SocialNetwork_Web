using Microsoft.EntityFrameworkCore;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class DistrictsService : IDistrictsService
    {
        private readonly DBContext _context;
        private readonly IUserNameService _userNameService;
        private readonly Utils _utils;
        public DistrictsService(DBContext context, IUserNameService userNameService, Utils utils)
        {
            _context = context;
            _userNameService = userNameService;
            _utils = utils;
        }
        public async Task<PayLoad<CityDTO>> Add(CityDTO city)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var user = _utils.CheckUser(n);
                    var checkName = _context.districts.AsNoTracking().FirstOrDefault(x => x.name == city.name && !x.deleted);
                    var checkCity = _context.citys.FirstOrDefault(x => x.id == city.id && !x.deleted);

                    if (user == null || checkName != null || checkCity == null) return PayLoad<CityDTO>.NotFound();

                    _context.districts.Add(new Districts
                    {
                        name = city.name,
                        citys = checkCity,
                        city_id = checkCity.id,
                        creator = user.UserName + " đã bảo bản ghi này vào lúc " + DateTime.Now
                    });

                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<CityDTO>.Successfully(city));
                }

                return await Task.FromResult(PayLoad<CityDTO>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<CityDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var checkId = _context.districts.AsNoTracking().FirstOrDefault(x => x.id == id && !x.deleted);
                    var user = _utils.CheckUser(n);

                    if(checkId == null || user == null) return PayLoad<string>.NotFound();

                    checkId.deleted = true;
                    checkId.creator = checkId.creator + ", " + user.UserName + " đã xóa bản ghi vào lúc " + DateTime.Now;

                    _context.districts.Update(checkId);
                    await _context.SaveChangesAsync();

                    return PayLoad<string>.Successfully(Status.SUCCESS);
                }

                return PayLoad<string>.NotFound();

            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.districts.AsNoTracking().Where(x => !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator
                }).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name)).ToList();

                var pageList = new PageList<object>(data, page - 1, pageSize);
                return PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                });
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOneByCommunes(int id)
        {
            try
            {
                var checkId = _context.communes.AsNoTracking().Where(x => x.district_id == id && !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator
                }).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
            }
            catch (Exception ex)
            {
                return PayLoad<object>.CreatedFail(ex.Message);
            }
        }

        public async Task<PayLoad<object>> FindOneId(int id)
        {
            try
            {
                var checkId = _context.districts.AsNoTracking().Where(x => x.id == id && !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator
                }).FirstOrDefault();

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<CityDTO>> Update(int id, CityDTO city)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var user = _utils.CheckUser(n);
                    var checkName = _context.districts.AsNoTracking().FirstOrDefault(x => x.id != id && x.name == city.name && !x.deleted);
                    var checkId = _context.districts.AsNoTracking().FirstOrDefault(x => x.id == id && !x.deleted);

                    if (checkName != null || checkId == null || user == null) return PayLoad<CityDTO>.NotFound();

                    if(city.id != null || city.id > 0)
                    {
                        var checkCity = _context.citys.AsNoTracking().FirstOrDefault(x => x.id == city.id && !x.deleted);
                        if(checkCity == null) return PayLoad<CityDTO>.NotFound();

                        checkId.city_id = checkCity.id;
                        checkId.citys = checkCity;
                    }

                    checkId.name = city.name;
                    checkId.creator = checkId.creator + ", " + user.UserName + " đã sửa bản ghi vào lúc " + DateTime.Now;

                    _context.districts.Update(checkId);
                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<CityDTO>.Successfully(city));
                }

                return await Task.FromResult(PayLoad<CityDTO>.NotFound());
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<CityDTO>.CreatedFail(ex.Message));
            }
        }
    }
}
