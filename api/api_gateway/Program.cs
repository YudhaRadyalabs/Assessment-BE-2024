using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using infrastructures;
using api_gateway.infrastructures;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Configuration.AddOcelotWithSwaggerSupport(options =>
{
    options.Folder = "Configs";
});

//builder.Services.AddAuthService(builder.Configuration);
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddSwaggerService(builder.Configuration);

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerForOcelotUI(options =>
    {
        options.PathToSwaggerGenerator = "/swagger/docs";
        options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
    }, optionUI =>
    {
        optionUI.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        optionUI.OAuthUsePkce();
    }).UseOcelot().Wait();
}

app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
