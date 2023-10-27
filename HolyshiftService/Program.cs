using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HolyShiftDbConfig>(builder.Configuration.GetSection("HolyShiftDbConfig"));
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("Auth"));

builder.Services.AddDbContext<HolyShiftDbContext>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordHashService>();
builder.Services.AddScoped<IUserDao, UserDao>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authService = builder.Services.BuildServiceProvider().GetService<AuthService>();
        options.TokenValidationParameters = authService.GetTokenValidationParameters();
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

app.MapControllers();

app.Run();
