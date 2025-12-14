//in this file I have the startups and configs of my asp.net core web api

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using sevrer.Api.Models;         
using sevrer.Api;
using sevrer.Api.ML;

//  initializes the application builder with configuration, logging, and service registration capabilities.
var builder = WebApplication.CreateBuilder(args);

//DB config
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//JWT config
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException(
        "JWT signing key is missing. Set it using User Secrets:\n" +
        "dotnet user-secrets set \"Jwt:Key\" \"your-64-hex-char-key\""
    );
}

builder.Services.AddAuthentication(options =>
{   
    //used to choose with auth scheme used to auth incoming request
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //used in case auth fails
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(jwtKey!);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//CORS
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

//password hashing
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

//controllers
builder.Services.AddControllers();

builder.Services.AddScoped<WastePredictionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("VueFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

app.Run();