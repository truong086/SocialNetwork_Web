using Microsoft.EntityFrameworkCore;
using System.Drawing;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class NationService : INationService
    {
        private readonly DBContext _context;
        private readonly IUserNameService _userNameService;
        private readonly Utils _util;
        public NationService(IUserNameService userNameService, DBContext context, Utils util)
        {
            _userNameService = userNameService;
            _context = context;
            _util = util;
        }
        public async Task<PayLoad<CityDTO>> Add(CityDTO city)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var userId = _util.CheckUser(n);
                    var checkName = _context.nations.FirstOrDefault(x => x.name == city.name && !x.deleted);
                    if (checkName != null || userId == null) return PayLoad<CityDTO>.CreatedFail("User Not Found And Data Exis");

                    _context.nations.Add(new Nation
                    {
                        name = city.name,
                        creator = userId.UserName + " đã tạo bản ghi vào lúc " + DateTime.Now
                    });

                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<CityDTO>.Successfully(city));
                }


                return PayLoad<CityDTO>.NotFound();
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
                    var userId = _util.CheckUser(n);
                    var checkId = _context.nations.FirstOrDefault(x => x.id == id && !x.deleted);
                    if (userId == null || checkId == null) return PayLoad<string>.NotFound();

                    checkId.deleted = true;
                    checkId.creator = checkId.creator + ", " + userId.UserName + " đã xóa bản ghi vào lúc " + DateTime.Now;

                    _context.nations.Update(checkId);
                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));

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
                var data = _context.nations.AsNoTracking().Where(x => !x.deleted).Select(x => new
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
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindByCity(int id)
        {
            try
            {
                var checkId = _context.citys.AsNoTracking().Where(x => x.nation_id == id && !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator
                }).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOneId(int id)
        {
            try
            {
                var checkId = _context.nations.AsNoTracking().Where(x => x.id == id && !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.creator
                }).ToList();

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
                    var userId = _util.CheckUser(n);
                    var checkName = _context.nations.FirstOrDefault(x => x.id != id && x.name == city.name && !x.deleted);
                    var checkId = _context.nations.FirstOrDefault(x => x.id == id && !x.deleted);

                    if (checkId == null || checkName != null || checkId == null) return PayLoad<CityDTO>.NotFound();

                    checkId.name = city.name;   
                    checkId.creator = checkId.creator + ", " + userId.UserName + " đã sửa bản ghi vào lúc " + DateTime.Now;

                    _context.nations.Update(checkId);
                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<CityDTO>.Successfully(city));

                }

                return PayLoad<CityDTO>.NotFound();
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<CityDTO>.CreatedFail(ex.Message));
            }
        }
    }
}
