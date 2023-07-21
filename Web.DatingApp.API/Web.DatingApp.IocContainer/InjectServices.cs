using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Web.DatingApp.API.Web.DatingApp.Database;
using Web.DatingApp.API.Web.DatingApp.Implenentations;
using Web.DatingApp.API.Web.DatingApp.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Web.DatingApp.API.Web.DatingApp.Interfaces.Repositories;
using Web.DatingApp.API.Web.DatingApp.Implenentations.Implementations;
using Web.DatingApp.API.Web.DatingApp.Helpers;
using Azure.Storage.Blobs;

namespace Web.DatingApp.API.Web.DatingApp.IocContainer
{
    public static class InjectServices
    {
        public static void InjectDependencies(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddCors();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                     {
                       new OpenApiSecurityScheme
                       {
                         Reference = new OpenApiReference
                         {
                           Type = ReferenceType.SecurityScheme,
                           Id = "Bearer"
                         }
                        },
                        new string[] { }
                      }
                    });
            });
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IContainerService, ContainerService>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            builder.Services.AddDbContext<DatingAppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatingAppConnectionString"));
            });
            builder.Services.AddSingleton(s => new BlobServiceClient(builder.Configuration.GetConnectionString("StorageConnectionString")));
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CLoudinarySettings"));
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtTokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            builder.Services.AddScoped<LogUserActivity>();
        }
    }
}
