using CloudinaryDotNet;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using Quartz;
using StackExchange.Redis;
using System;
using System.Runtime.InteropServices;
using System.Text;
using truyenthongso.ChatHub;
using truyenthongso.Clouds;
using truyenthongso.Common;
using truyenthongso.EmailConfig;
using truyenthongso.FunctionAuto;
using truyenthongso.Models;
using truyenthongso.PythonAI;
using truyenthongso.QuartzService;
using truyenthongso.Service;
using truyenthongso.ViewModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/commentHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});
#endregion

#region CORS
var corsBuilder = new CorsPolicyBuilder();
corsBuilder.AllowAnyHeader();
corsBuilder.AllowAnyMethod();
corsBuilder.AllowAnyOrigin();
//corsBuilder.WithOrigins("http://34.80.69.96:8080"); // Đây là Url bên frontEnd
corsBuilder.WithOrigins("http://localhost:8080"); // Đây là Url bên frontEnd
//corsBuilder.WithOrigins("https://5dc9-34-80-69-96.ngrok-free.app", "http://34.80.69.96:8080"); // Đây là Url bên frontEnd
corsBuilder.AllowCredentials();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", corsBuilder.Build());
});

#endregion

// Cấu hình Redis cho Hangfire
builder.Services.AddHangfire(config =>
    config.UseRedisStorage("localhost:6379", new RedisStorageOptions
    {
        Prefix = "hangfire:"
    }));

builder.Services.AddHangfireServer();

var connection = builder.Configuration.GetConnectionString("MyDB");
builder.Services.AddDbContext<DBContext>(option =>
{
    option.UseSqlServer(connection); // "ThuongMaiDienTu" đây là tên của project, vì tách riêng model khỏi project sang 1 lớp khác nên phải để câu lệnh này "b => b.MigrationsAssembly("ThuongMaiDienTu")"
});
//builder.Services.AddDbContext<DBContext>(options =>
//    options.UseOracle(builder.Configuration.GetConnectionString("MyDB")));
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CoreJwtExample",
        Version = "v1",
        Description = "Hello Anh Em",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
        {
            Name = "Thanh Toán Online",
            Url = new Uri("https://localhost:44316/")
        }
    });



    // Phần xác thực người dùng(JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer asddvsvs123'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    //var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var path = Path.Combine(AppContext.BaseDirectory, xmlFileName);
    //c.IncludeXmlComments(path);
});
// Đọc cấu hình Cloudinary từ appsettings.json
var cloudinaryAccount = new CloudinaryDotNet.Account(
    builder.Configuration["Cloud:Cloudinary_Name"],
    builder.Configuration["Cloud:Api_Key"],
    builder.Configuration["Cloud:Serec_Key"]
);
var cloudinary = new Cloudinary(cloudinaryAccount);
// Đăng ký Cloudinary làm một dịch vụ Singleton
builder.Services.AddSingleton(cloudinary);
builder.Services.Configure<Cloud>(builder.Configuration.GetSection("Cloud"));
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var jobKey = new JobKey("TuDongMoiTuan");
    q.AddJob<TuDongMoiTuan>(Otp => Otp.WithIdentity(jobKey));
    q.AddTrigger(otps => otps.ForJob(jobKey).WithIdentity("WeeklyTrigger")
    .StartNow()
    .WithCronSchedule("0 0 0/1 * * ?"));
    //.WithCronSchedule("0 0/1 * * * ?")); // "0/1" là chạy mỗi phút, để "1" là chỉ chạy 1 phút lần đầu
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));
builder.Services.AddSwaggerGen();
// Đăng ký HostedService cho Quartz.NET
//builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddAuthentication(); // Sử dụng phân quyền
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserNameService, UserNameService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAIGentsService, AIGentsService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<INationService, NationService>();
builder.Services.AddScoped<IDistrictsService, DistrictsService>();
builder.Services.AddScoped<ICommunesService, CommunesService>();
builder.Services.AddScoped<IArticles_ViewedService, Articles_ViewedService>();
builder.Services.AddScoped<ITraningModelService, TraningModelService>();
builder.Services.AddScoped<IIconService, IconService>();
builder.Services.AddScoped<IRedisService, CacheFuncsService>();
builder.Services.AddScoped<IFriendShipService, FriendShipService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<RedisService>();
builder.Services.Configure<RedisConfig>(
    builder.Configuration.GetSection("Redis"));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("Redis");

    var options = ConfigurationOptions.Parse(connectionString);
    options.AllowAdmin = true;
    return ConnectionMultiplexer.Connect(options);
    //return ConnectionMultiplexer.Connect(connectionString);
});

//builder.Services.AddScoped<TuDongMoiTuan>();
builder.Services.AddScoped<SendEmais>();
builder.Services.AddScoped<KiemTraBase64>();
builder.Services.AddScoped<Utils>();
builder.Services.AddScoped<VerificationTaskWorker>();
builder.Services.AddControllers();
builder.Services.AddAuthentication(); // nếu có
builder.Services.AddAuthorization();
builder.Services.AddSignalR(); // Real-Time

// Đăng ký IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddMvc();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddResponseCompression();
var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRouting();
// Dashboard để theo dõi job (tùy chọn)
app.UseHangfireDashboard("/hangfire");

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy();
app.MapControllers();

app.MapHub<CommentHub>("/commentHub");
app.Run();
