using System.IdentityModel.Tokens.Jwt;
using ImageGalary.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using WeatherApp;

var builder = WebApplication.CreateBuilder(args);

// Remove unused algorithms for JWT
JwtSecurityTokenHandler.DefaultOutboundAlgorithmMap.Clear();

// Enable logging of Personally Identifiable Information (PII)
IdentityModelEventSource.ShowPII = true;

// Register Serilog for logging
builder.Host.RegisterSerilog();

// Get WeatherClient Oauth Configuration
var weatherClientIp = builder.Configuration["WeatherClientIp"];
var weatherAuthority = builder.Configuration["WeatherAuthority"];
var weatherClientId = builder.Configuration["WeatherClientId"];
var weatherClientSecret = builder.Configuration["WeatherClientSecret"];
var weatherResponseType = builder.Configuration["WeatherResponseType"];
var weatherCallbackPath = builder.Configuration["WeatherCallbackPath"];
var weatherSignedOutCallbackPath = builder.Configuration["WeatherSignedOutCallbackPath"];
var weatherNameClaimType = builder.Configuration["WeatherNameClaimType"];
var weatherRoleClaimType = builder.Configuration["WeatherRoleClaimType"];

// Register services
builder.Services.AddControllersWithViews();
builder.Services.AddAccessTokenManagement();
builder.Services.AddHttpClient("WeatherApiClient", client =>
{
    client.BaseAddress = new Uri(weatherClientIp);
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
}).AddUserAccessTokenHandler();

builder.Services.AddHttpClient("IDPClient", client =>
{
    client.BaseAddress = new Uri(weatherAuthority);
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
{
    opt.AccessDeniedPath = "/Authentication/ActionDenied";
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opt =>
{
    opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.Authority = weatherAuthority;
    opt.ClientId = weatherClientId;
    opt.ClientSecret = weatherClientSecret;
    opt.ResponseType = weatherResponseType;
    opt.Scope.Add("openid");
    opt.Scope.Add("profile");
    opt.CallbackPath = new PathString(weatherCallbackPath);
    opt.SignedOutCallbackPath = new PathString(weatherSignedOutCallbackPath);
    opt.SaveTokens = true;
    opt.GetClaimsFromUserInfoEndpoint = true;

    opt.Scope.Add("roles");
    opt.Scope.Add("weatherapi.read");
    opt.Scope.Add("weatherapi.write");
    opt.Scope.Add("country");
    opt.Scope.Add("offline_access");

    opt.ClaimActions.Remove("aud");
    opt.ClaimActions.DeleteClaim("sid");
    opt.ClaimActions.DeleteClaim("idp");

    opt.ClaimActions.MapJsonKey("role", "role");
    opt.ClaimActions.MapUniqueJsonKey("country", "country");

    opt.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = weatherNameClaimType,
        RoleClaimType = weatherRoleClaimType
    };
});


// Configure authorization policies
builder.Services.AddAuthorization(options => { options.AddPolicy("CanGetWeather", AuthPolicies.CanGetWeather()); });

// Build the app
var app = builder.Build();

// Use error handling and HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Use middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configure routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the app
app.Run();