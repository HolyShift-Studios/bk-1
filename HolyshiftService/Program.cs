using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

var configDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Config");

builder.Configuration
    .SetBasePath(configDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<HolyShiftDbConfig>(builder.Configuration.GetSection("HolyShiftDbConfig"));

builder.Services.AddDbContext<HolyShiftDbContext>();

builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("Auth"));

builder.Services.AddSingleton<JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authConfig = builder.Configuration.GetSection("Auth").Get<AuthConfig>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = authConfig.Issuer, 
            ValidAudience = authConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Key)),
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("api/login");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecks("/health");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapControllerRoute(
        name: "registration",
        pattern: "api/register",
        defaults: new { controller = "Registration", action = "Register" }
    );

    endpoints.MapControllerRoute(
        name: "login",
        pattern: "api/login",
        defaults: new { controller = "Login", action = "Login" }
    );
});

app.Run();
