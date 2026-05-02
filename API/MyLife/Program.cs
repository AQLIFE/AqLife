
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyLife.Core.Func;
using MyLife.Core.Provider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.BindLogger().BindConfiguration().BindFilePolicy().BindStoragePolicy().BindGlobalExceptionPolicy();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.InitCheckDatabaseConnection();

app.UseExceptionHandler().UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/quick", new HealthCheckOptions
{
    // 只运行标记为 "db" 的轻量检查
    Predicate = (check) => check.Tags.Contains("db")
});

app.Run();