using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using OA.Core.Configurations;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.Services.Helpers;
using OA.Domain.Services;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service;
using OA.Service.Helpers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(x => x.SerializerSettings.ContractResolver = new DefaultContractResolver())
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

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
                          .AllowAnyMethod());
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT authentication
void RegisterJWT(IServiceCollection services)
{
    var jwtAppSettingOptions = builder.Configuration.GetSection(nameof(JwtIssuerOptions));

    services.Configure<UploadConfigurations>(builder.Configuration.GetSection(nameof(UploadConfigurations)));

    services.Configure<JwtIssuerOptions>(options =>
    {
        options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
        options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
        options.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-signing-key")), SecurityAlgorithms.HmacSha256);
    });

    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
        ValidateAudience = true,
        ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-signing-key")),
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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "", // Đường dẫn request mặc định
    OnPrepareResponse = ctx =>
    {
        if (ctx.File.PhysicalPath != null && ctx.File.PhysicalPath.Contains("avatars"))
        {
            const int durationInSeconds = 31536000; // 1 năm
            ctx.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age={durationInSeconds}";
            ctx.Context.Response.Headers[HeaderNames.Expires] = DateTime.UtcNow.AddSeconds(durationInSeconds).ToString("R");
        }
        //  Thêm điều kiện cho các file khác nếu cần
        // else 
        // {
        //     ctx.Context.Response.Headers[HeaderNames.CacheControl] = "no-store"; // hoặc "no-cache"
        // }
    }
});


// Xóa dòng `app.UseStaticFiles();`  ở phía trên

app.UseCors("AllowSpecificOrigin"); // Thêm dòng này để sử dụng cấu hình CORS

app.UseAuthentication(); // Ensure authentication middleware is used
app.UseAuthorization();

app.MapControllers();

app.Run();