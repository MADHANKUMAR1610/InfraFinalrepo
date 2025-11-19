
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
using Buildflow.Service.Service.MaterialStatus;
using Buildflow.Service.Service.MaterialStockAlert;
using Buildflow.Service.Service.Notification;
using Buildflow.Service.Service.Project;
using Buildflow.Service.Service.Report;
using Buildflow.Service.Service.Ticket;
using Buildflow.Service.Service.Vendor;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("Current Environment: " + builder.Environment.EnvironmentName);
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

//builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
var frontendUrl = builder.Configuration["Frontend:Url"];


var supportSystemSpecificOrigins = "_wiseSpecificOrigins";
var corsHostName = builder.Configuration.GetSection("Cors").GetSection("HostName").Value;
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: supportSystemSpecificOrigins, policyBuilder =>
    {
        policyBuilder
            .WithOrigins(
                frontendUrl,"http://localhost:2126"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
builder.Services.AddDbContext<BuildflowAppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100_000_000; // 100 MB
});


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
builder.Services.AddScoped<IMaterialStatusRepository, MaterialStatusRepository>();

builder.Services.AddScoped<IDailyStockRepository, DailyStockRepository>();
builder.Services.AddScoped<DailyStockService>();
builder.Services.AddScoped<DailyStockBackgroundService>();

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
builder.Services.AddScoped<MaterialStatusService>();
builder.Services.AddScoped<MaterialStockAlertService>();


builder.Services.AddScoped<IMaterialStockAlertRepository, MaterialStockAlertRepository>();




builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
});



//builder.Services.AddSwaggerGen(c =>
//{
//    c.EnableAnnotations();
//});

SetSwaggerAction(builder);


var app = builder.Build();

app.UseStaticFiles();


app.UseHttpsRedirection();
app.UseCors(supportSystemSpecificOrigins);
app.ExceptionMiddleware();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.Run();


void SetSwaggerAction(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddSwaggerGen(opt =>
    {
        opt.EnableAnnotations();
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "BuildFlowAPI", Version = "v1" });
        //opt.OperationFilter<FileUploadOperationFilter>();

        SetupSecurityAction(opt);
        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });

    void SetupSecurityAction(SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Kindly Provide Valid Token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
    }
}