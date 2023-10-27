using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HolyShiftDbConfig>(builder.Configuration.GetSection("HolyShiftDbConfig"));
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("Auth"));

builder.Services.AddDbContext<HolyShiftDbContext>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordHashService>();
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<AuthManager>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authService = builder.Services.BuildServiceProvider().GetService<AuthService>();
        options.TokenValidationParameters = authService.GetTokenValidationParameters();
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthChecks("/health");

app.MapPost("api/SignIn", async Task<ResponseDto>(HttpContext context, AuthManager authManager) =>
{
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var requestDto = JsonConvert.DeserializeObject<RequestDto>(requestBody);
    ResponseDto response = await authManager.SignIn(requestDto);
    return response;
});

app.MapPost("api/SignUp", async Task<RegisterResponseDto>(HttpContext context, AuthManager authManager) =>
{
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var requestDto = JsonConvert.DeserializeObject<RegistereRequestDto>(requestBody);
    RegisterResponseDto response = await authManager.SignUp(requestDto);
    return response;
});

app.Run();