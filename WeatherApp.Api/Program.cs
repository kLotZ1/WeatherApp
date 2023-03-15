using System.IdentityModel.Tokens.Jwt;
using ImageGalary.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WeatherApp.Api;
using WeatherApp.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.RegisterSerilog();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Audience = "weatherapi";
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters = new()
        {
            ValidTypes = new[] { "at+jwt" },
            RoleClaimType = "role",
            NameClaimType = "given_name"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanGetWeather", AuthPolicies.CanGetWeather());
    options.AddPolicy("ClientAppCanWrite", policyBuilder =>
    {
        policyBuilder.RequireClaim("scope", "weatherapi.write");
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandler>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();