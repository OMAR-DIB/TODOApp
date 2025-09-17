
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Extensions.Logging;
using System.Text;
using ToDo.API.ClaimProvider;
using ToDo.API.middelware;
using ToDo.API.Services;
using ToDo.API.Services.EmailServices;
using ToDo.API.Services.TokenServices;
using ToDo.API.Services.UserServices;
using ToDo.Data.DI;
using ToDo.Data.Repositories;
using NLog.Web;

namespace ToDo.API
{
    public class Program
    {
        public static void Main(string[] args) 
        {

            var builder = WebApplication.CreateBuilder(args);

            var logger = LogManager.Setup().LoadConfigurationFromSection(builder.Configuration).GetCurrentClassLogger();
            // Add services to the container.
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            try
            {
                // db config //
                var mssqlSection = builder.Configuration.GetSection("DbConfiguration");
                builder.Services.AddMyApplicationDbContext(mssqlSection);
                ///////////////////////
                builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                builder.Services.AddScoped<IToDosServices, ToDosServices>();
                builder.Services.AddScoped<IUserServices, UserServices>();
                builder.Services.AddControllers();
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();
                builder.Logging.ClearProviders();   // ant
                builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.Host.UseNLog();


                builder.Services.AddTransient<IEmailService, EmailService>();




                // register TokenService
                builder.Services.AddSingleton<ITokenService, TokenService>();

                // HttpContextAccessor + ClaimsProvider (your instructor's pattern)
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<IClaimsProvider, ClaimsProvider>();

                // JWT auth config
                var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
                var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30), // small skew
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                    };
                });

                builder.Services.AddAuthorization();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.UseMiddleware<AuthLoggingMiddleware>();
                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                }

                app.UseHttpsRedirection();

                app.UseAuthentication(); // must come before UseAuthorization
                app.UseAuthorization();


                app.MapControllers();

                app.Run();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while starting the application.");
            }

        }
    }
}
