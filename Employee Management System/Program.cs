using Employee_Management_System.Hubs;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using OA.Core.Configurations;
using OA.Core.Constants;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.Services.Helpers;
using OA.Domain.Services;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service;
using OA.Service.Helpers;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(x => x.SerializerSettings.ContractResolver = new DefaultContractResolver())
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddSignalR();

// Configure FormOptions to set the max file size
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000") // Thay thế bằng URL frontend của bạn
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Thêm định nghĩa bảo mật JWT
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập 'Bearer' [space] và sau đó là token của bạn.\n\nVí dụ: Bearer abcdef12345"
    });

    // Thêm yêu cầu bảo mật JWT vào tất cả các endpoint
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>() // Không yêu cầu scope
        }
    });
});


// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(CommonConstants.Authorize.CustomAuthorization, policy =>
    {
        policy.RequireAuthenticatedUser(); // Yêu cầu người dùng đã đăng nhập
    });
});

// Configure JWT authentication
void RegisterJWT(IServiceCollection services)
{
    byte[] keyBytes = new byte[32]; // 32 bytes = 256 bits
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(keyBytes);
    }

    string secretKey = Convert.ToBase64String(keyBytes); // Chuyển sang base64 để lưu trữ

    var jwtAppSettingOptions = builder.Configuration.GetSection(nameof(JwtIssuerOptions));

    services.Configure<UploadConfigurations>(builder.Configuration.GetSection(nameof(UploadConfigurations)));

    services.Configure<JwtIssuerOptions>(options =>
    {
        options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
        options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
        options.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256);
    });

    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
        ValidateAudience = true,
        ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        RequireExpirationTime = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(configureOptions =>
    {
        configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
        configureOptions.TokenValidationParameters = tokenValidationParameters;
        configureOptions.SaveToken = true;
    });
}

// Register JWT services
RegisterJWT(builder.Services);

// Add other services
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSingleton<IUserConnectionService, UserConnectionService>();
builder.Services.AddScoped<IAuthMessageSender, AuthMessageSender>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ISysApiService, SysApiService>();
builder.Services.AddScoped<IAspNetUserService, AspNetUserService>();
builder.Services.AddScoped<IAspNetRoleService, AspNetRoleService>();
builder.Services.AddScoped<IJwtFactory, JwtFactory>();
builder.Services.AddScoped<ISysFileService, SysFileService>();
builder.Services.AddScoped<ISysFunctionService, SysFunctionService>();
builder.Services.AddScoped<ISysConfigurationService, SysConfigurationService>();
builder.Services.AddScoped<ISalaryService, SalaryService>();
builder.Services.AddScoped<IBenefitService, BenefitService>();
builder.Services.AddScoped<IInsuranceService, InsuranceService>();
builder.Services.AddScoped<ITimekeepingService, TimekeepingService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ITimeOffService, TimeOffService>();
builder.Services.AddScoped<IEmploymentContractService, EmploymentContractService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<HtmlSanitizer>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<IDisciplineService, DisciplineService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IWorkingRulesService, WorkingRulesService>();
builder.Services.AddScoped<IWorkShiftService, WorkShiftService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IErrorReportService, ErrorReportService>();
builder.Services.AddScoped<IJobHistoryService, JobHistoryService>();
builder.Services.AddScoped<IMassageService, MassageService>();
builder.Services.AddScoped<ITransferHistoryService, TransferHistoryService>();

// Configure Identity
var identityBuilder = builder.Services.AddIdentityCore<AspNetUser>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 6;
});
identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(AspNetRole), identityBuilder.Services);
identityBuilder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
identityBuilder.AddRoleManager<RoleManager<AspNetRole>>();

// Session configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware
app.UseMiddleware<LoggingRequest>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // Cho phép phục vụ file tĩnh
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/avatars")),
    RequestPath = "/avatars"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload")),
    RequestPath = "/Upload"
});

app.UseCors("AllowSpecificOrigin"); // Thêm dòng này để sử dụng cấu hình CORS


app.UseSession();

app.UseAuthentication(); // Ensure authentication middleware is used
app.UseAuthorization();

app.UseMiddleware<CustomAuthorizationMiddleware>();

app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();

app.Run();