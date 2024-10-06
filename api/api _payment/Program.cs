using api_event.BackgroundServices;
using api_event.EntityFrameworks.Contexts;
using infrastructures;
using persistences;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var assembly = System.Reflection.Assembly.GetExecutingAssembly();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAppService(assembly);
builder.Services.AddAuthService(builder.Configuration);
builder.Services.AddPostgresqlService<AppDbContext>(connectionString);
builder.Services.AddSwaggerService(builder.Configuration);
builder.Services.AddNatsService(builder.Configuration, assembly);
builder.Services.AddHostedService<ConsumerTicket>();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
