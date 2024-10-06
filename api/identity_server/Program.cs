using System;
using identity_server;
using identity_server.EntityFrameworks.Contexts;
using identity_server.Infrastructures;
using identity_server.Services;
using infrastructures;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using persistences;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var assemmbly = System.Reflection.Assembly.GetExecutingAssembly();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddPostgresqlService<AppDbContext>(connectionString);
builder.Services.AddSwaggerService(builder.Configuration);
builder.Services.AddAppService(assemmbly);
builder.Services.AddAuthService(builder.Configuration);
builder.Services.AddNatsService(builder.Configuration, assemmbly);

#region identity
var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "temp-keys");
builder.Services.AddDataProtection()
    .SetApplicationName("assessment_2024")
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

builder.Services.AddIdentityServer(options =>
{
    options.IssuerUri = builder.Configuration.GetSection("IssuerUri").Get<string>();
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
    options.Authentication.CookieLifetime = TimeSpan.FromDays(1);
    options.Authentication.CookieSlidingExpiration = true;
})
    //.AddInMemoryApiScopes(Config.ApiScopes)
    //.AddInMemoryClients(Config.Clients)
    .AddConfigurationStore(option => option.ConfigureDbContext = builder => builder.UseNpgsql(connectionString, options => options.MigrationsAssembly("identity_server")))
    .AddOperationalStore(option =>
    {
        option.ConfigureDbContext = builder => builder.UseNpgsql(connectionString, options => options.MigrationsAssembly("identity_server"));
        option.EnableTokenCleanup = true;
        option.TokenCleanupInterval = 3600; // seconds
    })
    .AddResourceOwnerValidator<CustomPasswordValidator>()
    .AddProfileService<ProfileService>()
    .AddDeveloperSigningCredential();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.None;
});
#endregion

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.UseHttpsRedirection();

app.UseIdentityServer();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
