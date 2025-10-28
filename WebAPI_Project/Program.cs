
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using WebAPI.Domain.Mappings;
using WebAPI.Domain.Service;
using WebAPI_Project.Middlewares;

namespace WebAPI_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add Serilog NuGet Package
            // Configure Serilog for logging all data including debug level
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .Enrich.FromLogContext()
            //    .Enrich.WithEnvironmentUserName()
            //    .Enrich.WithThreadId()
            //    .WriteTo.Console()
            //    .WriteTo.File(
            //        "logs/api-.txt",
            //        rollingInterval: RollingInterval.Day,
            //        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            //    .CreateLogger();

            // Configure Serilog for logging information level and above
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()  // Change from Debug to Information
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)  // Only warnings and above from Microsoft
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithThreadId()
            .WriteTo.Console()
            .WriteTo.File(
            "logs/api-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

            builder.Host.UseSerilog();
            #endregion

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


            #region Disable Automatic Model State Validation
            // if you want to disable automatic model state validation, but be aware that you'll need to handle validation manually in your controllers.
            //by using ModelState.IsValid in every controller actions
            //builder.Services.AddControllers().ConfigureApiBehaviorOptions(
            //    options =>
            //        options.SuppressModelStateInvalidFilter = true);
            #endregion

            #region Global Exception Handling Filter
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<Filters.ExceptionHandleFilter>();
            });
            #endregion

            #region Register DbContext and Services in the DI Container
            builder.Services.AddDbContext<UniversityContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("UniversityDB"));
            });

            //REGISTER ApplicationUser and IdentityRole with the DI Container
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<UniversityContext>();

            //Dependency Injection for Service Layer
            builder.Services.AddScoped<StudentService>();
            builder.Services.AddScoped<DepartmentService>();
            #endregion

            #region Configure CORS to allow requests from any origin
            builder.Services.AddCors(options =>
            {
                // Policy 1: Allow everything (for development)
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });

                // Policy 2: Restrict to specific origin (for production)
                options.AddPolicy("ProductionPolicy", builder =>
                {
                    builder.WithOrigins("https://myapp.com", "https://www.myapp.com")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });

                // Policy 3: Read-only access
                options.AddPolicy("ReadOnly", builder =>
                {
                    builder.AllowAnyOrigin()
                           .WithMethods("GET")
                           .AllowAnyHeader();
                });
            });
            #endregion

            #region Add AutoMapper service
            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
            #endregion

            #region override default authentication middleware to validate token not cookies
            builder.Services.AddAuthentication(options =>
            {
                //Check JWT token header
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                //Change the default redirect url when [Authorize] attribute activated (instead of /Account/Login => /api/Account/Login)
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                //And this option is for all and any other service that don't have a DefaultScheme
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => //verified key
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;  //If you have a Https certification  

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:IssuerIP"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:AudienceIP"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"] ?? ""))
                };
            });
            #endregion

            // Add Swagger generation service
            //builder.Services.AddSwaggerGen();

            #region Swagger Setting to enable adding token
            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 8 Web API",
                    Description = " ITI Projrcy"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                    });
            });
            #endregion


            var app = builder.Build();

            // Add custom middleware
            app.UseMiddleware<SerilogLoggingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");

            app.UseAuthorization();


            app.MapControllers();

            app.MapGet("/hello", () => "Hello from WebAPI_Project!");

            app.Run();
        }
    }
}
