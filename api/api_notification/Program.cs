using api_notification.BackgroundServices;
using api_notification.Models;
using api_notification.Services;
using infrastructures;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var assembly = System.Reflection.Assembly.GetExecutingAssembly();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerService(builder.Configuration);
builder.Services.AddNatsService(builder.Configuration, assembly);
builder.Services.AddHostedService<ComsumerValidateTicket>();

builder.Services.AddCors();

builder.Services.Configure<SmtpOptions>(options => builder.Configuration.Bind("SmtpOptions", options));

builder.Services.AddTransient<SmtpService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();