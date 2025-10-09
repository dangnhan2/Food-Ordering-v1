using Food_Ordering.Extensions;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Infrastructure.Configuration;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

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
app.UseHangfireDashboard("/dashboard");

RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
    "DeleteExpiredCarts_3hours",
    j => j.DeleteExpiredCarts_3hours(),
    Cron.Hourly);

RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
    "DeleteExpiredRefreshTokens_3months",
    j => j.DeleteExpiredRefreshTokens_3months(),
    Cron.Daily());

app.MapControllers();

app.Run();
