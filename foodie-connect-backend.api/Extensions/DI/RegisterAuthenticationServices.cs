using System.Text;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Heads;
using foodie_connect_backend.Modules.Sessions;
using foodie_connect_backend.Modules.Users;
using foodie_connect_backend.Modules.Verification;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol;

namespace foodie_connect_backend.Extensions.DI;

public static class RegisterAuthenticationServices
{
    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        
        services.AddScoped<HeadsService>();
        services.AddScoped<UsersService>();
        services.AddScoped<SessionsService>();
        services.AddScoped<VerificationService>();
        
        services.AddAuthentication()
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = configuration["ENVIRONMENT"] == "production" ? CookieSecurePolicy.Always : CookieSecurePolicy.None;
                options.Cookie.SameSite = configuration["ENVIRONMENT"] == "production" ? SameSiteMode.None : SameSiteMode.Lax;;
                options.Cookie.Name = "access_token";
                options.Cookie.IsEssential = true;
                options.Cookie.Domain = ".foodie.town";
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(AuthError.NotAuthenticated().ToJson()));
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(AuthError.NotAuthorized().ToJson()));
                        return Task.CompletedTask;
                    }
                };
            });
        
        services.AddIdentityCore<User>(options => { options.User.RequireUniqueEmail = true; })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();
    }
}