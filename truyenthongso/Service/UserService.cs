using AutoMapper;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
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
        private readonly VerificationTaskWorker _verificationTaskWorker;
        public UserService(DBContext context, IMapper mapper, IOptionsMonitor<Jwt> jwt, IOptions<Cloud> cloud,
            SendEmais emails, IHttpContextAccessor httpContextAccessor, IUserNameService userNameService, VerificationTaskWorker verificationTaskWorker)
        {
            _context = context;
            _mapper = mapper;
            _jwt = jwt.CurrentValue;
            _cloud = cloud.Value;
            _emails = emails;
            _httpContextAccessor = httpContextAccessor;
            _userNameService = userNameService;
            _verificationTaskWorker = verificationTaskWorker;

        }
        public async Task<PayLoad<UserDTO>> Add(UserDTO userDTO)
        {
            try
            {
                if(userDTO.Age <= 0) return PayLoad<UserDTO>.CreatedFail("Age 0 ???");
                var checkData = _context.users.FirstOrDefault(x => x.UserName == userDTO.UserName && !x.deleted && x.Email == userDTO.Email);
                if(checkData != null)
                    return await Task.FromResult(PayLoad<UserDTO>.CreatedFail(Status.DATATONTAI));

                var checkRole = _context.roles.FirstOrDefault(x => x.Url.ToLower() == Status.USER.ToLower());
                if (checkRole == null)
                    return await Task.FromResult(PayLoad<UserDTO>.CreatedFail(Status.DATANULL));

                var dataMap = _mapper.Map<User>(userDTO);
                dataMap.Password = EncryptionHelper.CreatePasswordHash(userDTO.Password, _jwt.Key);
                dataMap.role = checkRole;
                dataMap.role_id = checkRole.id;

                _context.users.Add(dataMap);

                _context.SaveChanges();

                return await Task.FromResult(PayLoad<UserDTO>.Successfully(userDTO));
                //var dataNew = _context.users.OrderByDescending(x => x.id).FirstOrDefault();

                //var descriptionEmail = new SendEmail
                //{
                //    title = "Mã xác nhận tài khoản",
                //    message = "Thông tin xác nhận",
                //    name = dataNew.UserName,
                //    active = RanDomCode.geneAction(4) + dataNew.id.ToString()
                //};

                //var tokenOTP = new Token
                //{
                //    user = dataNew,
                //    userid = dataNew.id,
                //    token = descriptionEmail.active
                //};

                //_context.tokens.Add(tokenOTP);

                //if (await _context.SaveChangesAsync() > 0)
                //{
                //    var tempalte = Status.TEMPLATEVIEW;

                //    var tempalateEmail = await _emails.RenderViewToStringAsync(tempalte, descriptionEmail);
                //    await _emails.SendEmai(dataNew.Email, descriptionEmail.title, tempalateEmail);

                //    // Khởi động Background Task để xử lý
                //    _ = Task.Run(async () =>
                //    {
                //        //var work = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<VerificationTaskWorker>();
                //        //await work.RunOnceAsync(); // Chuyền dữ liệu vào hàm "VerificationTaskWorker" này
                //        await _verificationTaskWorker.RunOnceAsync(dataNew.id); // Chuyền dữ liệu vào hàm "VerificationTaskWorker" này
                //    });
                //    return await Task.FromResult(PayLoad<UserDTO>.Successfully(userDTO));
                //}

                //return PayLoad<UserDTO>.CreatedFail(Status.DATANULL);
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
                    x.UserName,
                    x.Email,
                    x.Image,
                    x.Address,
                    x.role.Url,
                    x.Action,
                    x.cretoredat
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

        public async Task<PayLoad<string>> DeleteAccountNoAction(int id = 0)
        {
            try
            {
                if(id > 0)
                {
                    var checkUser = _context.users.FirstOrDefault(x => x.id == id && !x.deleted && !x.Action);
                    var checkToken = _context.tokens.Where(x => x.userid == id && !x.deleted).ToList();

                    if(checkUser != null)
                        _context.users.Remove(checkUser);
                    if (checkToken.Count > 0)
                        _context.tokens.RemoveRange(checkToken);
                }
                else
                {
                    var time = DateTimeOffset.UtcNow.AddMinutes(-1).AddSeconds(-18);
                    var data = _context.users.Where(x => x.cretoredat >= time && !x.deleted && !x.Action).ToList();
                    var dataToken = _context.tokens.Where(x => x.cretoredat >= time).ToList();
                    if (data.Count > 0)
                    {
                        _context.users.RemoveRange(data);
                    }

                    if (dataToken.Count > 0)
                    {
                        _context.tokens.RemoveRange(dataToken);

                    }
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
                var checkEmail = _context.users.OrderByDescending(x => x.id).FirstOrDefault(x => x.Email == email && !x.deleted && !x.Action);
                if(checkEmail == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                var checkToken = _context.tokens.Where(x => x.userid == checkEmail.id).ToList();
                //if (checkToken.Count >= 1) return PayLoad<string>.CreatedFail("Đợi 1 phút !!!");
                if(checkToken.Count > 0)
                {
                    _context.tokens.RemoveRange(checkToken);
                    _context.SaveChanges();
                    return PayLoad<string>.CreatedFail("Đợi 1 phút !!!");
                }

                var otpCode = RanDomCode.geneAction(4) + checkEmail.id.ToString();
                otpCode = otpCode.Length > 6
                    ? otpCode.Substring(otpCode.Length - 6, 6) // Nếu chuỗi hiện tại lớn 6 ký tự thì chỉ lấy 6 ký tự từ dưới lên, dùng (otpCode.Length - 6, 6)
                    : otpCode.PadRight(6, 's'); // Nếu chuỗi nhỏ hơn 6 ký tự thì sẽ thêm 1 ký tự vào lên phải của chuỗi, ở đây đang thêm ký tự "s"

                //var myString = string.Empty;
                //myString = myString = (myString ?? "")
                //    .PadLeft(6, 'X') // Nếu chuỗi nhỏ hơn 6 ký tự thì sẽ thêm 1 ký tự vào lên trái của chuỗi, ở đây đang thêm ký tự "X"
                //    .Substring(0, 6); // Nếu chuỗi hiện tại lớn 6 ký tự thì chỉ lấy 6 ký tự từ trên xuống, dùng (0, 6)

                var descriptionEmail = new SendEmail
                {
                    title = "Mã xác nhận tài khoản",
                    message = "Thông tin xác nhận",
                    name = checkEmail.UserName,
                    active = otpCode
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
                    await _verificationTaskWorker.RunOnceAsync(checkEmail.id); // Chuyền dữ liệu vào hàm "VerificationTaskWorker" này
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
                var checkEmail = _context.users.OrderByDescending(x => x.id).FirstOrDefault(x => x.Email == data.email && !x.deleted && !x.Action);
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
                    x.Action,
                    x.Image
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
                    token = TokenLogin.GenerateToken(claims, _jwt),
                    image = checkData.Image
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