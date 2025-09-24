using Food_Ordering.Extensions;
using Food_Ordering.Services.BackgroundJob;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add extensions
builder.Services.AddExtensions();
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
app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<IBackgoundJobService>(
    "CheckOrderExpired",
    service => service.CheckOrderExpired(),
    Cron.Minutely()
);

app.MapControllers();

app.Run();
