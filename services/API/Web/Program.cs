using AqLife.Application;
using AqLife.Infrastructure;
using AqLife.Shared.Exceptions;
using AqLife.Web.Extensions;
using AqLife.Web.Middlewares;
using Microsoft.AspNetCore.StaticFiles;

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.AddSerilog().AddConfiguration().AddFilePolicy().AddJwtPolicy();
builder.Services.AddApplicationLayer().AddInfrastructure();
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
//app.UseStaticFiles();
app.MapControllers();



app.Run();