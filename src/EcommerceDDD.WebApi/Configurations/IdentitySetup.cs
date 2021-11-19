using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using EcommerceDDD.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Infrastructure.Identity.Claims;
using EcommerceDDD.Infrastructure.Identity.Roles;
using EcommerceDDD.Infrastructure.Identity.Users;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EcommerceDDD.WebApi.Configurations;

public static class IdentitySetup
{
    public static void AddIdentitySetup(this IServiceCollection services, 
        IConfiguration configuration)
    {
        if (null == services) 
            throw new ArgumentNullException(nameof(services));

        services.AddIdentity<ApplicationUser, UserRole>()
            .AddUserStore<UserStore<ApplicationUser, UserRole, IdentityContext, Guid>>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddRoles<UserRole>()
            .AddDefaultTokenProviders();

        var appSettingsSection = configuration.GetSection("AppSettings");
        services.Configure<AppSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<AppSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings.Secret);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            // JWT Setup
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = appSettings.ValidAt,
                ValidIssuer = appSettings.Issuer
            };
        });

        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
        });
    }

    public static void AddAuthSetup(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddAuthorization(options =>
        {                
            options.AddPolicy("CanRead", policy => policy.Requirements.Add(new ClaimRequirement("CanRead", "Read")));
            options.AddPolicy("CanSave", policy => policy.Requirements.Add(new ClaimRequirement("CanSave", "Save")));
            options.AddPolicy("CanDelete", policy => policy.Requirements.Add(new ClaimRequirement("CanDelete", "Delete")));
        });
    }
}