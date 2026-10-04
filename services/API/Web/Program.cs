using AqLife.Application;
using AqLife.Infrastructure;
using AqLife.Shared.Exceptions;
using AqLife.Web.Extensions.Authentication;
using AqLife.Web.Extensions.Configurations;
using AqLife.Web.Extensions.Database;
using AqLife.Web.Extensions.Exception;
using AqLife.Web.Extensions.Logging;
using AqLife.Web.Extensions.Routing;
using AqLife.Web.Middlewares;
using Microsoft.AspNetCore.StaticFiles;

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddSwaggerGen();// 启用 OPEN API 的支持,NET 8 SDK

builder.Host.InitialSerilog();// 配置 SeriLog
builder.Configuration.ImportConfiguration(builder.Environment);// 读取所有配置文件并载入系统
builder.Services.BindFilePolicy(builder.Configuration, builder.Environment);
builder.Services.BindJwtPolicy(builder.Configuration);

builder.Services.AddApplicationLayer().AddInfrastructure(builder.Environment);
builder.Services.AddGlobalExceptionPolicy(builder.Environment).AddDataLayer(builder.Configuration).AddRouteAdapter();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();// 框架内置服务
builder.Services.AddHttpContextAccessor();// 框架内置服务
builder.Services.AddScoped<FileUploadFilter>();

var corsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? throw new ConfigurationNotFoundException("未找到 Cors 配置");


builder.Services.AddCors(options =>
{
    options.AddPolicy("AqLifeAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(corsOrigins) // 允许你的 Vue 开发服务器地址
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // 如果后续涉及 Cookie/Auth，建议开启
              .WithExposedHeaders("Authorization");
    });
});

var app = builder.Build();
app.UseExceptionHandler();
app.InitCheckDatabaseConnection();

app.UseCors("AqLifeAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();



app.Run();