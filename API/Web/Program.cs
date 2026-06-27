using Microsoft.AspNetCore.StaticFiles;
using MyLife.Application;
using MyLife.Application.Command;
using MyLife.Data;
using MyLife.Data.Entities;
using MyLife.Service.Features;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.Contracts;
using MyLife.Web.Extensions;
using MyLife.Web.Middlewares;

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyLifeAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://192.168.0.100:5173", "http://localhost:5173", "http://127.0.0.1:5173") // 允许你的 Vue 开发服务器地址
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // 如果后续涉及 Cookie/Auth，建议开启
              .WithExposedHeaders("Authorization");
    });
});

builder.AddSerilog().AddConfiguration().AddFilePolicy().AddJwtPolicy();
builder.Services.AddApplicationLayer();
builder.Services.AddGlobalExceptionPolicy(builder.Environment).AddDataLayer(builder.Configuration).AddRouteAdapter();
builder.Services.AddScoped<IJwtProvider<AccountEntity>, AuthService>();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();// 框架内置服务
builder.Services.AddHttpContextAccessor();// 框架内置服务

builder.Services.AddScoped<FileUploadFilter>();


var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var validators = scope.ServiceProvider.GetServices<IValidator<CreateAccountCommand>>();
//    Console.WriteLine($"--- 共找到 {validators.Count()} 个 CreateAccountCommand 验证器 ---");
//    foreach (var v in validators)
//    {
//        Console.WriteLine($"已成功加载验证器: {v.GetType().Name}");
//    }
//}
app.UseExceptionHandler();
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
app.MapControllers();



app.Run();