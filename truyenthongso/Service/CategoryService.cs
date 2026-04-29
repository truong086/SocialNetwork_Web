using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Xml.Linq;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly DBContext _context;
        private IMapper _mapper;
        public CategoryService(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PayLoad<CategoryDTO>> Add(CategoryDTO data)
        {
            try
            {
                var checkName = _context.categories.FirstOrDefault(x => x.Name == data.name && !x.deleted);
                if (checkName != null)
                    return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(Status.DATANULL));

                _context.categories.Add(_mapper.Map<Category>(data));
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<CategoryDTO>.Successfully(data));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.categories.FirstOrDefault(x => x.id == id && !x.deleted);
                if (checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkId.deleted = true;
                _context.categories.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.categories.AsNoTracking().ToList();
                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.Name.Contains(name)).ToList();

                var mapData = _mapper.Map<List<CategoryDTO>>(data);

                var pageList = new PageList<object>(mapData, page - 1, pageSize);
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
                var data = _context.categories.AsNoTracking().FirstOrDefault(x => x.id == id && !x.deleted);
                var mapData = _mapper.Map<List<CategoryDTO>>(data);

                return await Task.FromResult(PayLoad<object>.Successfully(mapData));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<CategoryDTO>> Update(int id, CategoryDTO data)
        {
            try
            {
                var checkId = _context.categories.FirstOrDefault(x => x.id == id && !x.deleted);
                if(checkId == null)
                    return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(Status.DATANULL));

                var checkName = _context.categories.FirstOrDefault(x => x.Name == data.name && !x.deleted && x.id != checkId.id);
                if (checkName != null)
                    return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(Status.DATATONTAI));
                
                checkId.Name = data.name;
                checkId.updateat = DateTimeOffset.UtcNow;

                _context.categories.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<CategoryDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(ex.Message));
            }
        }
    }
}