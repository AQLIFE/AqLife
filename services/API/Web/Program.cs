using AqLife.Extensions.Application;
using AqLife.Extensions.Authentication;
using AqLife.Extensions.Configurations;
using AqLife.Extensions.Database;
using AqLife.Extensions.Exception;
using AqLife.Extensions.Infrastructure;
using AqLife.Extensions.Logging;
using AqLife.Extensions.Routing;
using AqLife.Shared.Exceptions;
using AqLife.Web.Middlewares;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddAqLifeConfiguration(
    builder.Environment.ContentRootPath,
    builder.Environment.EnvironmentName);

builder.Services.AddAqLifeSerilog();
builder.Services.AddSwaggerGen();

builder.Services.AddFilePolicy(
    builder.Configuration,
    builder.Environment.ContentRootPath);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddGlobalExceptionPolicy<BaseExceptionHandler>();
builder.Services.AddRouteAdapter();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<FileUploadFilter>();

var corsOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
    ?? throw new ConfigurationNotFoundException(
        "未找到 Cors 配置");

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AqLifeAllowSpecificOrigins",
        policy =>
        {
            policy
                .WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("Authorization");
        });
});

var app = builder.Build();

app.UseExceptionHandler();
app.CheckDatabaseConnection();

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
