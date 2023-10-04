using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Data.Common;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HolyShiftDb>(builder.Configuration.GetSection("HolyShiftDb"));

builder.Services.AddDbContext<HolyShiftDbContext>(options =>
{
    var connection = new DbConnectionStringBuilder
    {
        ["Server"] = builder.Configuration["HolyShiftDb:Server"],
        ["Database"] = builder.Configuration["HolyShiftDb:Database"],
        ["User"] = builder.Configuration["HolyShiftDb:User"],
        ["Password"] = builder.Configuration["HolyShiftDb:Password"]
    };

    options.UseMySql(
        connection.ConnectionString,
        ServerVersion.AutoDetect(connection.ConnectionString));
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseHealthChecks("/health");

app.Run();

