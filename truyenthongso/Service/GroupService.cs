using AutoMapper;
using Microsoft.EntityFrameworkCore;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class GroupService : IGroupService
    {
        private readonly DBContext _context;
        private IMapper _mapper;
        public GroupService(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PayLoad<GroupDTO>> Add(GroupDTO data)
        {
            try
            {
                var checkData = _context.groups.FirstOrDefault(x => x.Name == data.name);
                if(checkData != null)
                    return await Task.FromResult(PayLoad<GroupDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<Group>(data);
                _context.groups.Add(mapData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<GroupDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<GroupDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.groups.FirstOrDefault(x => x.id == id);
                if(checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;
                _context.groups.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.groups.Where(x => !x.deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.Name.Contains(name)).ToList();

                var pageList = new PageList<object>(data, page - 1, pageSize);
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalPages,
                    pageList.totalCounts
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOne(int id)
        {
            try
            {
                var checkData = _context.groups.FirstOrDefault(x => x.id == id);
                if(checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(checkData));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<GroupDTO>> Update(int id, GroupDTO data)
        {
            try
            {
                var checkId = _context.groups.FirstOrDefault(x => x.id == id);
                if(checkId == null)
                    return await Task.FromResult(PayLoad<GroupDTO>.CreatedFail(Status.DATANULL));

                var checkData = _context.groups.FirstOrDefault(x => x.Name == data.name && x.id != checkId.id);
                if(checkData != null)
                    return await Task.FromResult(PayLoad<GroupDTO>.CreatedFail(Status.DATATONTAI));

                checkId.Name = data.name;
                checkId.updateat = DateTimeOffset.UtcNow;

                _context.groups.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<GroupDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<GroupDTO>.CreatedFail(ex.Message));
            }
        }
    }
}
