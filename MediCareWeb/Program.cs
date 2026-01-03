using MediCare.Helpers;
using MediCare.Helpers.Interface;
using MediCareDto.API;
using MediCareWeb.Helpers.Interface;
using MediCareWeb.Services.Implementations;
using MediCareWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<JwtFromCookieHandler>();

builder.Services.AddHttpClient<Auth>();
builder.Services.AddScoped<IAuth, Auth>();

builder.Services.AddScoped<IJwtHelper, JwtHelper>();
builder.Services.AddScoped<JwtHelper>();

builder.Services.AddScoped<ISessionHelper, SessionHelper>();
builder.Services.AddScoped<ISessionUtilityHelper, SessionUtilityHelper>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Configure Authentication & Authorization
var jwtKey = builder.Configuration["JwtSettings:Key"];
if (string.IsNullOrEmpty(jwtKey) || Encoding.UTF8.GetBytes(jwtKey).Length < 32)
{
    throw new InvalidOperationException("Invalid JWT key. Ensure it is at least 32 characters long.");
}


var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {

                var sessionHelper = context.HttpContext.RequestServices.GetService<ISessionHelper>();
                var userSession = sessionHelper?.GetObject<SessionDto>("UserSession");

                if (userSession != null && !string.IsNullOrEmpty(userSession.UserToken))
                {
                    context.Token = userSession.UserToken;
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity?.Name != null)
                {
                    Console.WriteLine("Token validated for user: " + context.Principal.Identity.Name);
                }
                else
                {
                    Console.WriteLine("Token validated but user identity is null.");
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ISessionHelper, SessionHelper>();
builder.Services.AddScoped<ISessionUtilityHelper, SessionUtilityHelper>();
builder.Services.AddScoped<ISessionHelper, SessionHelper>();
builder.Services.AddScoped<ISessionUtilityHelper, SessionUtilityHelper>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");
app.Run();
