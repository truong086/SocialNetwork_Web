using Microsoft.EntityFrameworkCore;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class TagService : ITagService
    {
        private readonly DBContext _context;
        private readonly IUserNameService _userNameService;
        private readonly Utils _utils;
        public TagService(DBContext context, IUserNameService userNameService, Utils utils)
        {
            _context = context;
            _userNameService = userNameService;
            _utils = utils;
        }
        public async Task<PayLoad<TagDTO>> Add(TagDTO tagDTO)
        {
            try
            {
                int userId = int.Parse(_userNameService.name());

                var checkUser = _context.users.FirstOrDefault(x => x.id == userId && !x.deleted);
                var checkTagName = _context.tags.FirstOrDefault(x => x.Name == tagDTO.name && !x.deleted);

                if (checkUser == null || checkTagName != null) return PayLoad<TagDTO>.CreatedFail("User không tồn tại hoặc Name của tag đã tồn tại");

                _context.tags.Add(new Tag
                {
                    Name = tagDTO.name
                });

                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<TagDTO>.Successfully(tagDTO));
                
            }
            catch (Exception ex) { 
            
                return await Task.FromResult(PayLoad<TagDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out var n))
                {
                    var checkUser = _utils.CheckUser(n);
                    var checkId = _context.tags.FirstOrDefault(_ => _.id == id && !_.deleted);

                    if(checkUser == null || checkId == null) return PayLoad<string>.CreatedFail(Status.DATANULL);

                    checkId.deleted = true;
                    checkId.creator = checkUser.UserName + " đã xóa bản ghi vào lúc " + DateTime.Now;

                    _context.tags.Update(checkId);

                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
                }

                return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));
            }
            catch (Exception ex) {

                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.tags.AsNoTracking().Where(x => !x.deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.Name.Contains(name) && !x.deleted).ToList();

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
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            
            }
        }

        public async Task<PayLoad<TagDTO>> Update(int id, TagDTO tagDTO)
        {
            try
            {
                if(int.TryParse(_userNameService.name(), out int n))
                {
                    var checkId = _context.tags.FirstOrDefault(x => x.id == id && !x.deleted);
                    if (checkId == null) return PayLoad<TagDTO>.CreatedFail(Status.DATANULL);

                    var checkAccount = _utils.CheckUser(n);
                    var checkTag = _context.tags.FirstOrDefault(x => x.id != id && x.Name == tagDTO.name && !x.deleted);
                    if (checkAccount == null || checkTag != null) return PayLoad<TagDTO>.CreatedFail("User chưa tồn tại hoặc Tag đã tồn tại");

                    checkId.Name = tagDTO.name;
                    checkId.creator = checkAccount.UserName + " đã chỉnh sửa bản ghi vào lúc " + DateTime.Now;
                    _context.tags.Update(checkId);
                    await _context.SaveChangesAsync();

                    return await Task.FromResult(PayLoad<TagDTO>.Successfully(tagDTO));
                }
                return await Task.FromResult(PayLoad<TagDTO>.NotFound());
            }
            catch (Exception ex) {
                return await Task.FromResult(PayLoad<TagDTO>.CreatedFail(ex.Message));
            }
        }
    }
}
