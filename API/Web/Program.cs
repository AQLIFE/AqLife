using Microsoft.AspNetCore.StaticFiles;
using MyLife.Application;
using MyLife.Application.Abstractions.Authentication;
using MyLife.Application.Abstractions.FileStorage;
using MyLife.Domain.Entities;
using MyLife.Infrastructure;
using MyLife.Infrastructure.FileStorage;
using MyLife.Web.Extensions;
using MyLife.Web.Middlewares;

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyLifeAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5200", "http://192.168.0.100:5200", "http://192.168.0.100:5173") // 允许你的 Vue 开发服务器地址
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // 如果后续涉及 Cookie/Auth，建议开启
              .WithExposedHeaders("Authorization");
    });
});

builder.AddSerilog().AddConfiguration().AddFilePolicy().AddJwtPolicy();
builder.Services.AddApplicationLayer();
builder.Services.AddGlobalExceptionPolicy(builder.Environment).AddDataLayer(builder.Configuration).AddRouteAdapter();
builder.Services.AddScoped<ITokenProvider<AccountEntity>, ITokenProvider<AccountEntity>>();
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();// 框架内置服务
builder.Services.AddHttpContextAccessor();// 框架内置服务

builder.Services.AddScoped<FileUploadFilter>();


var app = builder.Build();
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