using IRecharge.Core.Application.Interface;
using IRecharge.Core.Application.UserServices;
using IRecharge.Infrastructure;
using IRecharge.Infrastructure.Data;
using IRecharge.UserManagementAPI.Extension;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

LogSetting.SetupLogging();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDBContext>(options =>
              options.UseSqlServer(config.GetConnectionString("DbConfig")));

builder.Services.AddSingleton<ServiceBusHelper>();
builder.Services.AddScoped<IUserService, UserService>();


// ############################## CORS POLICY ##################################### //
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_AllowAll", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var app = builder.Build();
app.UseCors("_AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseAuthorization();

app.MapControllers();

app.Run();
