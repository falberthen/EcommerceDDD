var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddMemoryCache();
services.AddHealthChecks();

// Token settings
var tokenIssuerSettings = builder.Configuration.GetSection("TokenIssuerSettings");
services.Configure<TokenIssuerSettings>(tokenIssuerSettings);

// Services
services.AddScoped<IdentityApplicationDbContext>();
services.AddScoped<ITokenRequester, TokenRequester>();
services.AddScoped<IIdentityManager, IdentityManager>();
services.AddTransient<IProfileService, CustomProfileService>();

// ---- AspNet.Core.Identity settings
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");
var migrationsAssembly = typeof(Program)
    .GetTypeInfo().Assembly.GetName().Name;

// DbContext
services.AddDbContext<IdentityApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Authorization and Identity
services.AddIdentity<ApplicationUser, IdentityRole>()
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
    .AddAspNetIdentity<ApplicationUser>()
    .AddProfileService<CustomProfileService>();

// App
var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.UseSwagger(builder.Configuration);

app.UseRouting();
app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();

app.MigrateDatabase().Run();