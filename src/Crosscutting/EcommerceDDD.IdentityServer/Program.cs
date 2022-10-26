using MediatR;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.IdentityServer.Database;
using EcommerceDDD.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EcommerceDDD.Core.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ---- Services
var tokenIssuerSettings = builder.Configuration.GetSection("TokenIssuerSettings");
builder.Services.Configure<TokenIssuerSettings>(tokenIssuerSettings); 
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies()); 
builder.Services.AddScoped<IdentityApplicationDbContext>();
builder.Services.AddScoped<ITokenRequester, TokenRequester>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

// ---- AspNet.Core.Identity settings
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");
var migrationsAssembly = typeof(Program)
    .GetTypeInfo().Assembly.GetName().Name;

builder.Services.AddDbContext<IdentityApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

builder.Services.AddIdentityServer(opt =>
    opt.IssuerUri = tokenIssuerSettings.GetValue<string>("Authority"))
    .AddDeveloperSigningCredential() // without a certificate, for dev only
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
            b.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
        options.EnableTokenCleanup = true;
    })
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
            b.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    .AddAspNetIdentity<ApplicationUser>();

// ---- Cors
builder.Services.AddCors(o =>
    o.AddPolicy("CorsPolicy", builder => {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200");
    }
));

// ---- App
var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseIdentityServer();
app.UseAuthorization();
app.MapControllers();

app.MigrateDatabase().Run();