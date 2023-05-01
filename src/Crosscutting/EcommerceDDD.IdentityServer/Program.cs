var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddMemoryCache();
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

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

// Cors
builder.Services.AddCors(o =>
    o.AddPolicy("CorsPolicy", builder => {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200");
    }
));

// App
var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseIdentityServer();
app.UseAuthorization();
app.MapControllers();

app.MigrateDatabase().Run();