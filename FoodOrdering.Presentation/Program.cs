using Food_Ordering.Extensions;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Infrastructure.Configuration;
using FoodOrdering.Infrastructure.SignalR_Hub;
using FoodOrdering.Presentation.Configuration;
using FoodOrdering.Presentation.Extensions;
using FoodOrdering.Presentation.Middleware;
using Hangfire;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext() // meta data into log (requestId, endpoint, path)
    .Enrich.WithThreadId() // thread id into log
    .Enrich.WithEnvironmentName() // eviroment 
    .WriteTo.Console() // display log to console
    .WriteTo.Seq("http://localhost:5341") // display log to seq
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog((context, services, configuration) => 
   configuration.ReadFrom.Configuration(context.Configuration));

// Add extensions
builder.Services.AddExtensions();
builder.Services.AddConnection();
builder.Services.AddHangfireServer();
builder.Services.AddDistributedMemoryCache();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();   
   
    await app.SeedAsync();
}

app.UseHttpsRedirection();

app.UseCors("Happy Food");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapHub<NotificationHub>("/hubs/notification");

app.UseHangfireServer();
app.UseHangfireDashboard("/dashboard");

app.UseRecurringJobs();

app.MapControllers();

app.Run();
