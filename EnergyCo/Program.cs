using EnergyCo.Data;
using EnergyCo.Services;
using Microsoft.EntityFrameworkCore;
using System;
using EnergyCo.Data.Interfaces;
using EnergyCo.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=../EnergyCo.Data/energyco.db")); // You would replace with SQL server or other in prod

// Register services
builder.Services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
app.UseRouting();
app.MapControllers();
app.Run();
