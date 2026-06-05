
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.StaticFiles;
using MyLife.Data.Entities;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Service.ServiceInterfaces;
using MyLife.Service.ServiceInterfaces.IStrategy;
using MyLife.Service.StrategiesService;
using MyLife.Shared.DTOs;
using MyLife.Web.BusinessInitialization;
using MyLife.Web.BusinessInitialization.Policy;
using MyLife.Web.BusinessSecurity.RuntimeCheck;

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyLifeAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://192.168.0.55:5173","http://localhost:5173","http://127.0.0.1:5173") // 允许你的 Vue 开发服务器地址
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // 如果后续涉及 Cookie/Auth，建议开启
              .WithExposedHeaders("Authorization");
    });
});

builder.AddSerilog().AddConfiguration().AddFilePolicy().AddJwtPolicy();

builder.Services.AddGlobalExceptionPolicy(builder.Environment).AddDataLayer(builder.Configuration).AddRouteAdapter();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();// 框架内置服务
builder.Services.AddHttpContextAccessor();// 框架内置服务

#region 文件相关服务注册
builder.Services.AddSingleton<IJwtProvider<AccountEntity>, JwtProvider>();
builder.Services.AddScoped<FileUploadFilter>();

builder.Services.AddScoped<IUploadStrategy, UploadPermissionCheck>();
builder.Services.AddScoped<IUploadStrategy, UploadSizeCheck>();
builder.Services.AddScoped<IUploadStrategyAsync, UploadFileEffectivenessCheck>();
builder.Services.AddScoped<IDownloadStrategy, DownloadPermissionCheck>();

builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<UploadContext>();
builder.Services.AddSingleton<FileMapper>();
#endregion

#region 账户相关服务注册
builder.Services.AddScoped<ISubscriptionUploadStrategy, ValidityStrategy>();
builder.Services.AddScoped<ISubscriptionUpdateStrategy, ValidityIndexStrategy>();

builder.Services.AddScoped<IAccountUploadStrategy, DuplicateNameCheckStrategy>();
builder.Services.AddScoped<IAccountUploadStrategy, AccountSubscriptionCheckStrategy>();

builder.Services.AddScoped<IAccountUpdateStrategy,AccountAvatarValidCheckStrategy>();
builder.Services.AddScoped<IAccountUpdateStrategy, AccountSubscriptionAvatarValidCheckStrategy>();

builder.Services.AddSingleton<SubscriptionMapper>();
builder.Services.AddScoped<SubscriptionService>(); 
builder.Services.AddSingleton<AccountMapper>();
builder.Services.AddScoped<AccountService>();

#endregion
builder.Services.AddSingleton<TodoMapper>();

var app = builder.Build();

app.InitCheckDatabaseConnection();

app.UseCors("MyLifeAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseStaticFiles();
app.MapControllers(); app.UseExceptionHandler();



app.MapHealthChecks("/api/health");
app.MapHealthChecks("/api/health/quick", new HealthCheckOptions
{
    // 只运行标记为 "db" 的轻量检查
    Predicate = (check) => check.Tags.Contains("db")
});


app.Run();