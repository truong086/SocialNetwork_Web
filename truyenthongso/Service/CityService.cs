using Microsoft.EntityFrameworkCore;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class CityService : ICityService
    {
        private readonly DBContext _context;
        private readonly IUserNameService _userNameService;
        private readonly Utils _utils;
        public CityService(DBContext context, IUserNameService userNameService, Utils utils)
        {
            _context = context;
            _userNameService = userNameService;
            _utils = utils;
        }
        public async Task<PayLoad<CityDTO>> Add(CityDTO cityDTO)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out var n))
                {
                    var userId = _utils.CheckUser(n);
                    var checkName = _context.citys.FirstOrDefault(x => x.name == cityDTO.name && !x.deleted);
                    var checkNation = _context.nations.FirstOrDefault(x => x.id == cityDTO.id && !x.deleted);

                    if (userId == null || checkName != null || checkNation == null) return PayLoad<CityDTO>.CreatedFail("User chưa tồn tại hoặc dữ liệu city đã tồn tại !!!");

                    _context.citys.Add(new Citys
                    {
                        name = cityDTO.name,
                        creator = userId.UserName + " đã tạo bản ghi vào lúc " + DateTime.Now,
                        nation = checkNation,
                        nation_id = checkNation.id
                    });

                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<CityDTO>.Successfully(cityDTO));
                }

                return await Task.FromResult(PayLoad<CityDTO>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<CityDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var userId = _utils.CheckUser(n);
                    var checkId = _context.citys.FirstOrDefault(x => x.id == id && !x.deleted);
                    if (userId == null || checkId == null) return PayLoad<string>.CreatedFail(Status.DATANULL);

                    checkId.deleted = true;
                    checkId.creator = ", " + userId.UserName + " đã xóa bản ghi vào lúc " + DateTime.Now;
                    
                    _context.citys.Update(checkId);
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
                var data = _context.citys.AsNoTracking().Where(x => !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator,
                    Nation =  x.nation.name
                }).ToList();

                if (!string.IsNullOrEmpty(name)) {
                    data = data.Where(x => x.name.Contains(name)).ToList();
                }

                var pageList = new PageList<object>(data, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindByCommunes(int id)
        {
            try
            {
                var checkId = _context.communes.AsNoTracking().Where(x => x.city_id == id && !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator
                }).ToList();

                if (!checkId.Any()) return PayLoad<object>.NotFound();

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindByDistris(int id)
        {
            try
            {
                var checkId = _context.districts.AsNoTracking().Where(x => x.city_id == id && !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator
                }).ToList();
                if (checkId == null || checkId.Count <= 0) return PayLoad<object>.NotFound();

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOneId(int id)
        {
            try
            {
                var checkId = _context.citys.Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator,
                    x.deleted
                }).FirstOrDefault(x => x.id == id && !x.deleted);
                if (checkId == null) return PayLoad<object>.NotFound();

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
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
                    var userId = _utils.CheckUser(n);
                    var checkName = _context.citys.FirstOrDefault(x => x.id != id && x.name == cityDTO.name && !x.deleted);
                    var checkId = _context.citys.FirstOrDefault(x => x.id == id && !x.deleted);
                    

                    if (checkName != null || checkId == null || userId == null) return PayLoad<CityDTO>.CreatedFail(Status.DATANULL);

                    if(cityDTO.id != null || cityDTO.id > 0)
                    {
                        var checkNation = _context.nations.FirstOrDefault(x => x.id == cityDTO.id && !x.deleted);
                        if (checkNation == null) return PayLoad<CityDTO>.NotFound();

                        checkId.nation = checkNation;
                        checkId.nation_id = checkNation.id;
                    }
                    checkId.name = cityDTO.name;
                    checkId.creator = checkId.creator + ", " + userId.UserName + " đã sửa bản ghi vào lúc " + DateTime.Now;
                    
                    _context.citys.Update(checkId);
                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<CityDTO>.Successfully(cityDTO));
                }

                return PayLoad<CityDTO>.CreatedFail(Status.DATANULL);
            }
            catch (Exception ex) {

                return await Task.FromResult(PayLoad<CityDTO>.CreatedFail(ex.Message));
            }
        }
    }
}
