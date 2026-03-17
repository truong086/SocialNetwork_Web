using AutoMapper;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Pkcs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using truyenthongso.Clouds;
using truyenthongso.Common;
using truyenthongso.EmailConfig;
using truyenthongso.FunctionAuto;
using truyenthongso.Models;
using truyenthongso.ViewModel;

namespace truyenthongso.Service
{
    public class UserService : IUserService
    {
        private IMapper _mapper;
        private readonly DBContext _context;
        private Jwt _jwt;
        private readonly Cloud _cloud;
        private readonly SendEmais _emails;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserNameService _userNameService;
        public UserService(DBContext context, IMapper mapper, IOptionsMonitor<Jwt> jwt, IOptions<Cloud> cloud,
            SendEmais emails, IHttpContextAccessor httpContextAccessor, IUserNameService userNameService)
        {
            _context = context;
            _mapper = mapper;
            _jwt = jwt.CurrentValue;
            _cloud = cloud.Value;
            _emails = emails;
            _httpContextAccessor = httpContextAccessor;
            _userNameService = userNameService;
        }
        public async Task<PayLoad<UserDTO>> Add(UserDTO userDTO)
        {
            try
            {
                var checkData = _context.users.FirstOrDefault(x => x.UserName == userDTO.UserName && !x.deleted && x.Email == userDTO.Email);
                if(checkData != null)
                    return await Task.FromResult(PayLoad<UserDTO>.CreatedFail(Status.DATATONTAI));

                var checkRole = _context.roles.FirstOrDefault(x => x.Url.ToLower() == Status.ADMIN.ToLower());
                if (checkRole == null)
                    return await Task.FromResult(PayLoad<UserDTO>.CreatedFail(Status.DATANULL));

                var dataMap = _mapper.Map<User>(userDTO);
                dataMap.Password = EncryptionHelper.CreatePasswordHash(userDTO.Password, _jwt.Key);
                dataMap.role = checkRole;
                dataMap.role_id = checkRole.id;

                _context.users.Add(dataMap);
               

                var descriptionEmail = new SendEmail
                {
                    title = "Mã xác nhận tài khoản",
                    message = "Thông tin xác nhận",
                    name = dataMap.UserName,
                    active = RanDomCode.geneAction(6) + dataMap.id.ToString()
                };

                var tokenOTP = new Token
                {
                    user = dataMap,
                    userid = dataMap.id,
                    token = descriptionEmail.active
                };

                _context.tokens.Add(tokenOTP);
                _context.SaveChanges();

                var tempalte = Status.TEMPLATEVIEW;

                var tempalateEmail = await _emails.RenderViewToStringAsync(tempalte, descriptionEmail);
                await _emails.SendEmai(dataMap.Email, descriptionEmail.title, tempalateEmail);

                // Khởi động Background Task để xử lý
                _ = Task.Run(async () =>
                {
                    var work = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<VerificationTaskWorker>();
                    await work.RunOnceAsync(); // Chuyền dữ liệu vào hàm "VerificationTaskWorker" này
                });
                return await Task.FromResult(PayLoad<UserDTO>.Successfully(userDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<UserDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try
            {
                var checkData = _context.users.FirstOrDefault(x => x.id == id);
                if (checkData == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkData.deleted = true;

                _context.users.Update(checkData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
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
                var data = _context.users.Where(x => !x.deleted).Select(x => new
                {
                    x.id,
                    x.FullName,
                    x.Email,
                    x.Image,
                    x.Address,
                    x.role.Url
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

        public async Task<PayLoad<object>> FindOne(int id)
        {
            try
            {
                var checkId = _context.users.Select(x => new
                {
                    x.id,
                    x.FullName,
                    x.Email,
                    x.Image,
                    x.Address,
                    x.role.Url
                }).FirstOrDefault();

                if(checkId == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
                return await Task.FromResult(PayLoad<object>.Successfully(checkId));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<UserDTO>> Update(int id, UserDTO userDTO)
        {
            try
            {
                var checkId = _context.users.FirstOrDefault(x => x.id == id);
                if(checkId == null)
                    return await Task.FromResult(PayLoad<UserDTO>.CreatedFail(Status.DATANULL));

                checkId.UserName = userDTO.UserName;
                checkId.Email = userDTO.Email;
                checkId.FullName = userDTO.FullName;
                checkId.Age = userDTO.Age;
                checkId.Commune = userDTO.Commune;
                checkId.District = userDTO.District;
                checkId.City = userDTO.City;
                checkId.Address = userDTO.Address;

                _context.users.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<UserDTO>.Successfully(userDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<UserDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> DeleteAccountNoAction()
        {
            try
            {
                var data = _context.users.Where(x => x.cretoredat > DateTimeOffset.UtcNow.AddMinutes(1) && !x.deleted && !x.Action).ToList();
                var dataToken = _context.tokens.Where(x => x.cretoredat > DateTimeOffset.UtcNow.AddMinutes(1)).ToList();
                if(data.Count > 0)
                {
                    _context.users.RemoveRange(data);
                }

                if(dataToken.Count > 0)
                {
                    _context.tokens.RemoveRange(dataToken);
                    
                }

                _context.SaveChanges();
                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> GenToken(string email)
        {
            try
            {
                var checkEmail = _context.users.FirstOrDefault(x => x.Email == email && !x.deleted && !x.Action);
                if(checkEmail == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                var checkToken = _context.tokens.Where(x => x.userid == checkEmail.id).ToList();
                if(checkToken.Count > 0)
                {
                    _context.tokens.RemoveRange(checkToken);
                }

                var descriptionEmail = new SendEmail
                {
                    title = "Mã xác nhận tài khoản",
                    message = "Thông tin xác nhận",
                    name = checkEmail.UserName,
                    active = RanDomCode.geneAction(6) + checkEmail.id.ToString()
                };


                var tokenNew = new Token
                {
                    user = checkEmail,
                    userid = checkEmail.id,
                    token = descriptionEmail.active
                };

                _context.tokens.Add(tokenNew);
                _context.SaveChanges();

                var tempalte = Status.TEMPLATEVIEW;

                var tempalateEmail = await _emails.RenderViewToStringAsync(tempalte, descriptionEmail);
                await _emails.SendEmai(checkEmail.Email, descriptionEmail.title, tempalateEmail);

                // Khởi động Background Task để xử lý
                _ = Task.Run(async () =>
                {
                    var work = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<VerificationTaskWorker>();
                    await work.RunOnceAsync(); // Chuyền dữ liệu vào hàm "VerificationTaskWorker" này
                });

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Action(ActionUser data)
        {
            try
            {
                var checkEmail = _context.users.FirstOrDefault(x => x.Email == data.email && !x.deleted && !x.Action);
                if(checkEmail == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                var checkToken = _context.tokens.FirstOrDefault(x => x.userid == checkEmail.id && x.token == data.token);
                if(checkToken == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                if(data.images != null)
                {
                    uploadCloud.CloudInaryIFromAccount(data.images, checkEmail.Email + "_1s" + checkEmail.id.ToString(), _cloud);
                    checkEmail.Image = uploadCloud.Link;
                    checkEmail.PublicId = uploadCloud.publicId;
                }
                checkEmail.Action = true;

                _context.users.Update(checkEmail);

                var deleteDataToken = _context.tokens.Where(x => x.userid == checkEmail.id).ToList();
                _context.tokens.RemoveRange(deleteDataToken);

                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> Login(LoginDTO userDTO)
        {
            try
            {
                var hashPassword = EncryptionHelper.CreatePasswordHash(userDTO.password, _jwt.Key);
                var checkData = _context.users.Select(x => new
                {
                    x.id,
                    x.UserName,
                    x.Password,
                    x.role.Url,
                    x.deleted,
                    x.Action
                }).FirstOrDefault(x => x.UserName == userDTO.username && x.Password == hashPassword && !x.deleted && x.Action);
                if(checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(Status.IDAUTHENTICATION, checkData.id.ToString())
                };

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    id = checkData.id,
                    username = checkData.UserName,
                    role = checkData.Url,
                    Token = TokenLogin.GenerateToken(claims, _jwt)
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Logout()
        {
             _userNameService.Logout();
            return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
        }
    }
}