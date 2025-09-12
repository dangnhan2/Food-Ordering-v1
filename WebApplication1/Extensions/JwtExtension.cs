using DotNetEnv;
using Food_Ordering.Data;
using Food_Ordering.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Food_Ordering.Extensions
{
    public static class JwtExtension
    {
        public static IServiceCollection AddJwtConfig(this IServiceCollection services) {
            Env.Load();

            // Cấu hình mật khẩu
            services.Configure<IdentityOptions>(opts =>
            {
                opts.Password.RequireDigit = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;

                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+ ";
            });

            // Cấu hình Identity
            services.AddIdentity<User, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddUserManager<UserManager<User>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Cấu hình authentication
            services
            .AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(config =>
            {
                config.SaveToken = true;
                config.RequireHttpsMetadata = false;
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,  // Kiểm tra người phát hành Token
                    ValidateAudience = true, // Kiểm tra người nhận Token
                    ValidateIssuerSigningKey = true, // Kiểm tra chữ ký Token
                    ValidIssuer = Env.GetString("ISSUER"), // Chỉ chấp nhận token được phát hành bởi ISSUER
                    ValidAudience = Env.GetString("AUDIENCE"), // Chỉ chấp nhận token phát cho AUDIENCE
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("KEY")))
                };
            });
            return services;
        }
    }
}
