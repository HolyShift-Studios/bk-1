using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore; 




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HolyShiftDbContext>(options =>
{
    var holyShiftDbConfig = builder.Configuration.GetSection("HolyShiftDbConfig");
    var connectionString = $"Server={holyShiftDbConfig["Server"]};Database={holyShiftDbConfig["Database"]};User={holyShiftDbConfig["User"]};Password={holyShiftDbConfig["Password"]}";
    Console.WriteLine($"Connection String: {connectionString}");
    
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
});


builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseHealthChecks("/health");

app.Run();
