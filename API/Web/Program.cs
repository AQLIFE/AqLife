
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.StaticFiles;
using MyLife.Data.Entities;
using MyLife.Service.Implementations;
using MyLife.Service.Interfaces;
using MyLife.Service.Mappings;
using MyLife.Service.Strategies;
using MyLife.Shared.DTOs;
using MyLife.Web.Extensions;
using MyLife.Web.Infrastructure;
using MyLife.Web.Policy;

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyLifeAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // 允许你的 Vue 开发服务器地址
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // 如果后续涉及 Cookie/Auth，建议开启
              .WithExposedHeaders("Authorization");
    });
});

builder.AddSerilog().AddConfiguration().AddFilePolicy().AddJwtPolicy();

builder.Services.AddGlobalExceptionPolicy(builder.Environment).AddDataLayer(builder.Configuration).AddRouteAdapter();

builder.Services.AddScoped<IFileSearchStrategy, SearchByIdStrategy>();
builder.Services.AddScoped<IFileSearchStrategy, SearchByTitleStrategy>();

builder.Services.AddScoped<IAccountStrategy, AccountNameStrategy>();

builder.Services.AddScoped<IUploadCheckStrategyAsync, UploadPermissionCheck>();
builder.Services.AddScoped<IUploadCheckStrategyAsync, ExtensionCheck>();
builder.Services.AddScoped<IUploadCheckStrategyAsync, SizeCheck>();
builder.Services.AddScoped<IUploadCheckStrategyAsync, UploadFileEffectivenessCheck>();


builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

builder.Services.AddScoped<IFileSearch, FileSearch>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<UploadContext>();
builder.Services.AddScoped<AccountService>();



builder.Services.AddSingleton<IGenericsMapper<SubscriptionEntity, SubscriptionDto>, SubscriptionMapper>();
builder.Services.AddSingleton<IGenericsMapper<AccountEntity, AccountDto>, AccountMapper>();  
builder.Services.AddSingleton<IGenericsMapper<FileMetaEntity, FileDto>, FileMapper>();
builder.Services.AddSingleton<TodoMapper>();
// builder.Services.AddSingleton<AccountMapper>();
builder.Services.AddSingleton<IJwtProvider<AccountEntity>, JwtProvider>();


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
app.UseStaticFiles();
app.MapControllers(); app.UseExceptionHandler();



app.MapHealthChecks("/api/health");
app.MapHealthChecks("/api/health/quick", new HealthCheckOptions
{
    // 只运行标记为 "db" 的轻量检查
    Predicate = (check) => check.Tags.Contains("db")
});


app.Run();