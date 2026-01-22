using Buildflow.Api.Middlewares;
using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Library.Repository;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Library.UOW;
using Buildflow.Service.Service;
using Buildflow.Service.Service.Employee;
using Buildflow.Service.Service.Inventory;
using Buildflow.Service.Service.Master;
using Buildflow.Service.Service.Material;
using Buildflow.Service.Service.MaterialStockAlert;
using Buildflow.Service.Service.Milestone;
using Buildflow.Service.Service.Notification;
using Buildflow.Service.Service.Project;
using Buildflow.Service.Service.Report;
using Buildflow.Service.Service.Ticket;
using Buildflow.Service.Service.Vendor;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch(
    "Npgsql.EnableLegacyTimestampBehavior",
    true
);

// ----------------------
// Environment Log
// ----------------------
Console.WriteLine("Current Environment: " + builder.Environment.EnvironmentName);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();

// ----------------------
// CORS FIXED
// ----------------------
var corsOrigins = builder.Configuration["Cors:HostName"]
    .Split(",", StringSplitOptions.RemoveEmptyEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ----------------------
// JWT Setup
// ----------------------
var jwt = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]))
    };
});

// ----------------------
// DATABASE
// ----------------------
builder.Services.AddDbContext<BuildflowAppContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// FORM UPLOAD LIMIT
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 100_000_000;
});

// ----------------------
// DEPENDENCY INJECTION
// ----------------------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRegisterRepository, RegisterRepository>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();

builder.Services.AddScoped<IDailyStockRepository, DailyStockRepository>();
builder.Services.AddScoped<DailyStockService>();
builder.Services.AddHostedService<DailyStockBackgroundService>();
builder.Services.AddScoped<IMilestoneMasterRepository, MilestoneMasterRepository>();

builder.Services.AddScoped<MaterialService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<RegisterService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<VendorService>();
builder.Services.AddScoped<MaterialStockAlertService>();
builder.Services.AddScoped<MilestoneMasterService>();
builder.Services.AddScoped<IMaterialStockAlertRepository, MaterialStockAlertRepository>();


builder.Services.AddHttpContextAccessor();



builder.Services.AddControllers(options =>
{
    // secure all controllers by default
    options.Filters.Add(new AuthorizeFilter(
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build()));
})
.AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    opt.JsonSerializerOptions.WriteIndented = true;
});

// Allow LoginController without JWT
builder.Services.PostConfigure<MvcOptions>(options =>
{
    var authFilter = options.Filters.OfType<AuthorizeFilter>().FirstOrDefault();
    if (authFilter != null)
        options.Filters.Remove(authFilter);
});

// ----------------------
// Swagger
// ----------------------
builder.Services.AddSwaggerGen(opt =>
{
    opt.EnableAnnotations();
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "BuildFlowAPI", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Enter JWT Token",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// ----------------------
// Build App
// ----------------------
var app = builder.Build();

app.UseStaticFiles();
app.UseHttpsRedirection();

// CORS MUST COME BEFORE AUTH
app.UseCors("AllowFrontend");

app.ExceptionMiddleware();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
