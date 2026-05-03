
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyLife.Core.Func;
using MyLife.Core.Provider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers(options =>
{
    // 统一添加 api 前缀
    options.Conventions.Add(new ApiPrefixConvention("api"));
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.BindLogger().BindConfiguration().BindFilePolicy().BindStoragePolicy().BindGlobalExceptionPolicy();
builder.Services.AddScoped<IFileSearchStrategy, SearchByIdStrategy>();
builder.Services.AddScoped<IFileSearchStrategy, SearchByTitleStrategy>();
// 手动注册（或者使用反射批量注册）
builder.Services.AddScoped<IUploadCheckStrategy, UploadPermissionCheck>();
builder.Services.AddScoped<IUploadCheckStrategy, ExtensionCheck>();
builder.Services.AddScoped<IUploadCheckStrategy, SizeCheck>();

//curl -v http://127.0.0.1:5000/api/health
var app = builder.Build();

app.InitCheckDatabaseConnection();

app.UseExceptionHandler().UseAuthorization();

app.MapHealthChecks("/api/health");
app.MapHealthChecks("/api/health/quick", new HealthCheckOptions
{
    // 只运行标记为 "db" 的轻量检查
    Predicate = (check) => check.Tags.Contains("db")
});
app.MapControllers();

app.Run();