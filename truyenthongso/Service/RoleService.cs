using AutoMapper;
using Microsoft.EntityFrameworkCore;
using truyenthongso.Common;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class RoleService : IRoleService
    {
        private readonly DBContext _context;
        private IMapper _mapper;
        public RoleService(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PayLoad<RoleDTO>> Add(RoleDTO roleDTO)
        {
            try
            {
                var checkData = _context.roles.FirstOrDefault(x => x.Url == roleDTO.Url);
                if(checkData != null)
                    return await Task.FromResult(PayLoad<RoleDTO>.CreatedFail(Status.DATATONTAI));

                _context.roles.Add(_mapper.Map<Role>(roleDTO));
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<RoleDTO>.Successfully(roleDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<RoleDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<GroupRoleDTO>> AddGroupRole(GroupRoleDTO data)
        {
            try
            {
                var checIdRole = _context.roles.FirstOrDefault(x => x.id == data.roleid);
                if(checIdRole == null)
                    return await Task.FromResult(PayLoad<GroupRoleDTO>.CreatedFail(Status.DATANULL));

                if(data.groupids != null && data.groupids.Count() > 0)
                {
                    var list = new List<Group_Role>();
                    foreach(var item in data.groupids)
                    {
                        var checkData = _context.groups.FirstOrDefault(x => x.id == item);
                        if(checkData != null)
                        {
                            list.Add(new Group_Role
                            {
                                group_id = checkData.id,
                                group = checkData,
                                role = checIdRole,
                                role_id = checIdRole.id
                            });
                        }
                    }

                    _context.groupRoles.AddRange(list);
                    _context.SaveChanges();
                }

                return await Task.FromResult(PayLoad<GroupRoleDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<GroupRoleDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkId = _context.roles.FirstOrDefault(x => x.id == id);
                if(checkId == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                _context.roles.Remove(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 5)
        {
            try
            {
                var data = _context.roles.AsNoTracking().Select(x => new
                {
                    x.id,
                    x.Url,
                    x.Description,
                    role_group = x.Group_Roles.Select(x1 => new
                    {
                        id_role = x1.role.Url,
                        id_group = x1.group.Name
                    }).ToList()
                }).ToList();

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

        public async Task<PayLoad<object>> FindAllGroupRole()
        {
            try 
            {
                var data = _context.groupRoles.ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOne(int id)
        {
            try
            {
                var checkId = _context.roles.FirstOrDefault(x => x.id == id);
                if(checkId == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<RoleDTO>> Update(int id, RoleDTO roleDTO)
        {
            try
            {
                var checkId = _context.roles.FirstOrDefault(x => x.id == id);
                if(checkId == null)
                    return await Task.FromResult(PayLoad<RoleDTO>.CreatedFail(Status.DATANULL));

                checkId.Url = roleDTO.Url;
                checkId.Description = roleDTO.Description != null && roleDTO.Description != "" ? roleDTO.Description : checkId.Description;

                _context.roles.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<RoleDTO>.Successfully(roleDTO));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<RoleDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<GroupRoleDTO>> UpdateGroupRole(int id, GroupRoleDTO data)
        {
            try
            {
                var checkRole = _context.roles.FirstOrDefault(x => x.id == id);
                var checkGroupRole = _context.groupRoles.FirstOrDefault(x => x.role_id == id);
                if (checkRole != null && checkGroupRole != null)
                {
                    await AddGroupRole(data);
                    return await Task.FromResult(PayLoad<GroupRoleDTO>.Successfully(data));
                }

                return await Task.FromResult(PayLoad<GroupRoleDTO>.CreatedFail(Status.DATANULL));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<GroupRoleDTO>.CreatedFail(ex.Message));
            }
        }
    }
}