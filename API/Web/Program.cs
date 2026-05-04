
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyLife.Service.Implementations;
using MyLife.Service.Interfaces;
using MyLife.Service.Strategies;
using MyLife.Web.Extensions;
using MyLife.Web.Infrastructure;
using MyLife.Web.Provision;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCustomApiConventions();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.AddSerilog().AddConfiguration().AddFilePolicy();

builder.Services.AddGlobalExceptionPolicy(builder.Environment).AddDataLayer(builder.Configuration);

builder.Services.AddScoped<IFileSearchStrategy, SearchByIdStrategy>();
builder.Services.AddScoped<IFileSearchStrategy, SearchByTitleStrategy>();
// 手动注册（或者使用反射批量注册）
builder.Services.AddScoped<IUploadCheckStrategy, UploadPermissionCheck>();
builder.Services.AddScoped<IUploadCheckStrategy, ExtensionCheck>();
builder.Services.AddScoped<IUploadCheckStrategy, SizeCheck>();

builder.Services.AddScoped<IFileSearchProvider, FileSearch>();
builder.Services.AddScoped<FileService>();


var app = builder.Build();

app.InitCheckDatabaseConnection();

app.UseExceptionHandler();

app.MapHealthChecks("/api/health");
app.MapHealthChecks("/api/health/quick", new HealthCheckOptions
{
    // 只运行标记为 "db" 的轻量检查
    Predicate = (check) => check.Tags.Contains("db")
});
app.MapControllers();

app.Run();