using Food_Ordering.Extensions;
using FoodOrdering.Infrastructure.Configuration;
using FoodOrdering.Infrastructure.Options;
using FoodOrdering.Infrastructure.Services.SignalR.SignalR_Hub;
using FoodOrdering.Presentation.Configuration;
using FoodOrdering.Presentation.Extensions;
using FoodOrdering.Presentation.Middleware;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Serilog;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);

builder.Host.UseSerilog((context, services, configuration) => 
   configuration
   .ReadFrom.Configuration(context.Configuration)
   .ReadFrom.Services(services)
   .Enrich.FromLogContext()
   .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
   );

// Add extensions
builder.Services.AddExtensions();
builder.Services.AddConnection();
builder.Services.AddHangfireServer();
builder.Services.AddDistributedMemoryCache();
// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<CloudinaryOptions>(
    builder.Configuration.GetSection("CloudinaryOptions"));

builder.Services.Configure<PayOsOptions>(
    builder.Configuration.GetSection("PayOsOptions"));

builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection("EmailOptions"));

builder.Services.Configure<GoogleAuthOptions>(
    builder.Configuration.GetSection("GoogleAuthOptions"));

builder.Services.AddCors(o =>
{
    o.AddPolicy("Cors",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});



builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle((opts) =>
    {
        var googleOptions = builder.Configuration
          .GetSection("GoogleAuthOptions")
          .Get<GoogleAuthOptions>();

        opts.ClientId = googleOptions.ClientId;
        opts.ClientSecret = googleOptions.ClientSecret;
        opts.CallbackPath = "/signin-google";
        opts.SaveTokens = true;

        opts.Scope.Add("profile");
        opts.Scope.Add("email");

        opts.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        opts.ClaimActions.MapJsonKey("picture", "picture");
    });

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.ApplyMigrationsAsync();
await app.SeedAsync();
app.UseHttpsRedirection();

app.UseCors("Cors");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapHub<NotificationHub>("/hubs/notification");

app.UseHangfireDashboard("/dashboard");

app.UseRecurringJobs();

app.MapControllers();

app.Run();
