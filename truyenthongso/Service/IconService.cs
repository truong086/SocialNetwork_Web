using AutoMapper;
using Microsoft.EntityFrameworkCore;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class IconService : IIconService
    {
        private readonly IUserNameService _userNameService;
        private IMapper _mapper;
        private readonly DBContext _context;
        public IconService(IUserNameService userNameService, IMapper mapper, DBContext context)
        {
            _userNameService = userNameService;
            _mapper = mapper;
            _context = context;
        }
        public async Task<PayLoad<IconDTO>> Add(IconDTO iconDTO)
        {
            try
            {
                var checkUrl = _context.icons.FirstOrDefault(x => x.url == iconDTO.url && !x.deleted);
                if (checkUrl != null) return await Task.FromResult(PayLoad<IconDTO>.CreatedFail(Status.DATANULL));

                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var checkUser = _context.users.FirstOrDefault(x => x.id == n && !x.deleted);
                    if(checkUser == null) return await Task.FromResult(PayLoad<IconDTO>.CreatedFail(Status.DATANULL));

                    var mapData = _mapper.Map<Icon>(iconDTO);
                    mapData.user = checkUser;
                    mapData.user_id = n;
                    mapData.creator = checkUser.UserName + " đã add icon vào lúc " + DateTime.Now;

                    _context.icons.Add(mapData);

                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<IconDTO>.Successfully(iconDTO));
                }

                return await Task.FromResult(PayLoad<IconDTO>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<IconDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.icons.FirstOrDefault(x => x.id == id && !x.deleted);

                if(checkId == null) return PayLoad<string>.CreatedFail(Status.DATANULL);

                checkId.deleted = true;

                _context.icons.Update(checkId);
                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 10)
        {
            try
            {
                var data = _context.icons.AsNoTracking().Where(x => !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.url,
                    x.user.UserName,
                    x.creator
                }).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name) || x.url.Contains(name)).ToList();

                var pageList = new PageList<object>(data, page - 1, pageSize);

                return PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalPages,
                    pageList.totalCounts
                });
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindIOne(int id)
        {
            try
            {
                var checkId = _context.icons.AsNoTracking().Where(x => x.id == id && !x.deleted).Select(x => new
                {
                    x.id,
                    x.name,
                    x.url,
                    x.user.UserName,
                    x.creator
                }).FirstOrDefault();
                if (checkId == null) return PayLoad<object>.CreatedFail(Status.DATANULL);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = checkId
                }));
            }
            catch (Exception ex) {
                return PayLoad<object>.CreatedFail(ex.Message);
            }
        }

        public async Task<PayLoad<IconDTO>> Update(int id, IconDTO iconDTO)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var checkUser = _context.users.FirstOrDefault(x => x.id == n && !x.deleted);
                    var checkId = _context.icons.FirstOrDefault(x => x.id == id && !x.deleted);
                    var checkUrl = _context.icons.FirstOrDefault(x => x.id != id && x.url == iconDTO.url && !x.deleted);
                    if (checkUser == null || checkId == null || checkUrl != null) return await Task.FromResult(PayLoad<IconDTO>.CreatedFail(Status.DATANULL));

                    checkId.url = iconDTO.url;
                    checkId.name = iconDTO.name;
                    checkId.creator = checkId.creator + ", " + checkUser.UserName + " đã cập nhật bản ghi vào lúc " + DateTime.Now;

                    _context.icons.Update(checkId);

                    await _context.SaveChangesAsync();

                    return PayLoad<IconDTO>.Successfully(iconDTO);
                }

                return await Task.FromResult(PayLoad<IconDTO>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(PayLoad<IconDTO>.CreatedFail(ex.Message));
            }
        }
    }
}
