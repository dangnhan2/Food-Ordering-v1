using Food_Ordering.Extensions;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Infrastructure.Configuration;
using FoodOrdering.Presentation.Middleware;
using Hangfire;
using Npgsql;
using Serilog;
using Serilog.Core;

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
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHangfireDashboard("/dashboard");

RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
    "DeleteExpiredCarts_3hours",
    j => j.DeleteExpiredCarts_3hours(),
    Cron.Hourly);

RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
    "DeleteExpiredRefreshTokens_3months",
    j => j.DeleteExpiredRefreshTokens_3months(),
    Cron.Daily());

RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
    "PublicVouchers_24hours",
    j => j.PublicVouchers_24hours(),
    Cron.Daily(),
    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
    "RetrieveVouchers_24hours",
    j => j.RetrieveVouchers_24hours(),
    Cron.Daily(),
    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
    "ResetVoucherRedemptions_24hours",
    j => j.ResetVoucherRedemptions_24hours(),
    Cron.Daily(),
    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
    "DeleteNotifications_1month",
    j => j.DeleteNotifications_1month(),
    Cron.Monthly(),
    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

app.MapControllers();

app.Run();
