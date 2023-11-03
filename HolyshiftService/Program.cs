using HolyShift;
using HolyShift.Auth;
using HolyShift.Config;
using HolyShift.Database;
using HolyShift.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HolyShiftDbConfig>(builder.Configuration.GetSection("HolyShiftDbConfig"));
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("Auth"));

builder.Services.AddDbContext<HolyShiftDbContext>();
builder.Services.AddTransient<JwtService>();
builder.Services.AddTransient<PasswordHashService>();
builder.Services.AddTransient<IUserDao, UserDao>();
builder.Services.AddTransient<AuthManager>();
builder.Services.AddTransient<IAuthHandler, AuthHandler>();

builder.AddAuth();

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecks("/health");

app.MapPost("api/SignIn", AuthenticationRoutes.SignIn);
app.MapAuthEndpoints();

app.Run();
