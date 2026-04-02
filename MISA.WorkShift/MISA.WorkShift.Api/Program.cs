using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MISA.WorkShift.Core.Entities;
using MISA.WorkShift.Core.Interfaces.Repositories;
using MISA.WorkShift.Core.Interfaces.Services;
using MISA.WorkShift.Core.Services;
using MISA.WorkShift.Infrastructure.Data;
using MISA.WorkShift.Infrastructure.Repositories;
using MISA.WorkShift.Api.Services;
using MISA.WorkShift.Api.Hubs;
using System.Text;
using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add JWT auth to Swagger UI so you can test authorized endpoints
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });

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
            new string[] { }
        }
    });
});

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:5173") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Bắt buộc cho SignalR
    });
});

// Cấu hình DI
var connectionString = builder.Configuration.GetConnectionString("MISAFRESHER2025"); // Đảm bảo khớp với appsettings.json

// Đăng ký DbContext với đúng cấu hình Options
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Repository/Service
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<MISA.WorkShift.Core.Interfaces.Services.IInvoiceXmlService, MISA.WorkShift.Core.Services.InvoiceXmlService>();
// Đăng ký thêm cho Registration
builder.Services.AddScoped<MISA.WorkShift.Core.Interfaces.Repositories.IInvoiceRegistrationRepository, MISA.WorkShift.Infrastructure.Repositories.InvoiceRegistrationRepository>();
builder.Services.AddScoped<IInvoiceRegistrationService, InvoiceRegistrationService>();
builder.Services.AddScoped<IBaseRepository<InvoiceDetail>, BaseRepository<InvoiceDetail>>();
builder.Services.AddScoped<IBaseRepository<RegDigitalSignature>, BaseRepository<RegDigitalSignature>>();

// Đăng ký cho Company và User
builder.Services.AddScoped<IBaseRepository<Company>, BaseRepository<Company>>();
builder.Services.AddScoped<IBaseService<Company>, BaseService<Company>>();
builder.Services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
builder.Services.AddScoped<IBaseService<User>, BaseService<User>>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>(); // Thêm dòng này
builder.Services.AddScoped<IAuthService, AuthService>();
// Notification service (SignalR) - implementation lives in API project
builder.Services.AddScoped<INotificationService, MISA.WorkShift.Api.Services.NotificationService>();
// Template repositories and file service
builder.Services.AddScoped<MISA.WorkShift.Core.Interfaces.Repositories.IInvoiceTemplateRepository, MISA.WorkShift.Infrastructure.Repositories.InvoiceTemplateRepository>();
builder.Services.AddScoped<MISA.WorkShift.Core.Interfaces.Repositories.IInvoiceTemplateXsltRepository, MISA.WorkShift.Infrastructure.Repositories.InvoiceTemplateXsltRepository>();
builder.Services.AddScoped<MISA.WorkShift.Api.Services.TemplateFileService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger UI (enable in all environments for debugging)
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseCors("AllowVueApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

// Ensure Chromium is downloaded once at application startup to avoid first-request delay
try
{
    // Blocking download during startup (use default behavior)
    new BrowserFetcher().DownloadAsync().GetAwaiter().GetResult();
}
catch
{
    // swallow any errors during download - runtime requests may still attempt to download
}

app.Run();
