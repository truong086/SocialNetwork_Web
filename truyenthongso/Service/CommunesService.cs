using Microsoft.EntityFrameworkCore;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class CommunesService : ICommunesService
    {
        private readonly DBContext _context;
        private readonly IUserNameService _userNameService;
        private readonly Utils _utils;
        public CommunesService(DBContext context, IUserNameService userNameService, Utils utils)
        {
            _context = context;
            _userNameService = userNameService;
            _utils = utils;
        }
        public async Task<PayLoad<CityDTO>> AddCommunes(CityDTO cityDTO)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var user = _utils.CheckUser(n);
                    var checkName = _context.communes.AsNoTracking().FirstOrDefault(x => x.name == cityDTO.name && !x.deleted);
                    var checkIdDistrics = _context.districts.FirstOrDefault(x => x.id == cityDTO.id && !x.deleted);
                    var checkIdCity = _context.citys.FirstOrDefault(x => x.id == cityDTO.id_city && !x.deleted);

                    if (user == null || checkName != null || checkIdDistrics == null || checkIdCity == null) 
                        return await Task.FromResult(PayLoad<CityDTO>.CreatedFail(Status.DATANULL));

                    _context.communes.Add(new Communes
                    {
                        name = cityDTO.name,
                        citys = checkIdCity,
                        city_id = checkIdCity.id,
                        districts = checkIdDistrics,
                        district_id = checkIdDistrics.id,
                        creator = user.UserName + " đã tạo bản ghi vào lúc " + DateTime.Now
                    });

                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<CityDTO>.Successfully(cityDTO));
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
                    var checkUser = _utils.CheckUser(n);
                    var checkId = _context.communes.FirstOrDefault(c => c.id == id && !c.deleted);

                    if (checkUser == null || checkId == null)
                        return PayLoad<string>.NotFound();

                    checkId.deleted = true;
                    checkId.creator = checkId.creator + ", " + checkUser.UserName + " đã xóa bản ghi này vào lúc " + DateTime.Now;

                    _context.communes.Update(checkId);
                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
                }

                return PayLoad<string>.NotFound();
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));   
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.communes.AsNoTracking().Where(x => !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator
                }).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name)).ToList();

                var pageList = new PageList<object>(data, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalPages,
                    pageList.totalCounts
                }));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<CityDTO>> Update(int id, CityDTO cityDTO)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var checkId = _context.communes.FirstOrDefault(x => x.id == id && !x.deleted);
                    var checkName = _context.communes.FirstOrDefault(x => x.id != id && x.name == cityDTO.name && !x.deleted);
                    var checkUser = _utils.CheckUser(n);

                    if (checkId == null || checkName != null || checkUser == null)
                        return PayLoad<CityDTO>.NotFound();

                    if(cityDTO.id != null)
                    {
                        var checkDistrics = _context.districts.FirstOrDefault(x => x.id == cityDTO.id && !x.deleted);
                        if(checkDistrics == null) return PayLoad<CityDTO>.NotFound();

                        checkId.districts = checkDistrics;
                        checkId.district_id = checkDistrics.id;
                    }

                    if (cityDTO.id_city != null) {
                        var checkCity = _context.citys.FirstOrDefault(x => x.id == cityDTO.id_city && !x.deleted);
                        if(checkCity == null) return PayLoad<CityDTO>.NotFound();

                        checkId.city_id = checkCity.id;
                        checkId.citys = checkCity;
                    }

                    checkId.name = cityDTO.name;
                    checkId.creator = checkId.creator + ", " + checkUser.UserName + " đã chỉnh sửa bản ghi vào lúc " + DateTime.Now;

                    _context.communes.Update(checkId);
                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<CityDTO>.Successfully(cityDTO));
                }

                return PayLoad<CityDTO>.NotFound("User !!!");
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<CityDTO>.CreatedFail($"{ex.Message}"));
            }
        }
    }
}
