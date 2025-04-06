using Microsoft.EntityFrameworkCore;
using WindTurbineApi.API.Configurations;
using WindTurbineApi.Application.Interfaces;
using WindTurbineApi.Application.Mappers;
using WindTurbineApi.Application.Services;
using WindTurbineApi.Domain.Repositories;
using WindTurbineApi.Domain.Services;
using WindTurbineApi.Infrastructure.Persistence;
using WindTurbineApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration(); // Extension created in API Configurations

builder.Logging.ClearProviders().AddConsole().AddDebug();

// Database Context
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "Infrastructure","Persistence", "WindTurbine.db");
var dbDirectory = Path.GetDirectoryName(dbPath);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));


// Repositories and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ISensorRecordRepository, SensorRecordRepository>();

builder.Services.AddScoped<ISensorFactory, SensorFactory>();

// Services
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<ISensorRecordService, SensorRecordService>();

// AutoMapper
builder.Services.AddAutoMapper(
    typeof(SensorMappingProfile),
    typeof(SensorRecordMappingProfile));

// Logging
builder.Logging.AddConsole();

var app = builder.Build();

app.UseSwaggerConfiguration(); // Extension created in API Configurations
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
