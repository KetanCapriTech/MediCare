using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Ocelot.Cache.CacheManager;
using System.Text;
using MediCareDto.Auth.Jwt;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;

builder.Configuration
    // appsettings (automatic, but explicit is fine)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)

    .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"configuration.{env}.json", optional: true, reloadOnChange: true);

// -------------------------------
// Bind JWT Settings
// -------------------------------
var jwtTokenConfig = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

if (jwtTokenConfig == null || string.IsNullOrEmpty(jwtTokenConfig.Key))
{
    throw new Exception("JWT configuration not found or invalid.");
}

var key = new SymmetricSecurityKey(
    Encoding.ASCII.GetBytes(jwtTokenConfig.Key)
);


// -------------------------------
// Configure JWT Authentication
// -------------------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = key,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();


// -------------------------------
// Add Ocelot + Polly + CacheManager
// -------------------------------
builder.Services.AddOcelot(builder.Configuration)
    .AddPolly()
    .AddCacheManager(settings => settings.WithDictionaryHandle());


Console.WriteLine("ENVIRONMENT: " + builder.Environment.EnvironmentName);
Console.WriteLine("Ocelot Routes Loaded:");
Console.WriteLine(builder.Configuration.GetSection("Routes").GetChildren().Count());


// -------------------------------
// Build App
// -------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapGet("/", () => "API Gateway is running");

// IMPORTANT: Ocelot must be last
 app.UseOcelot().Wait();

app.Run();
