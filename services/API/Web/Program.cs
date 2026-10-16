using AqLife.Extensions.Application;
using AqLife.Extensions.Authentication;
using AqLife.Extensions.Configurations;
using AqLife.Extensions.Database;
using AqLife.Extensions.Exceptions;
using AqLife.Extensions.Infrastructure;
using AqLife.Extensions.Logging;
using AqLife.Extensions.Routing;
using AqLife.Extensions.SwaggerDoc;
using AqLife.Shared.Exceptions;
using AqLife.Web.Middlewares;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAqLifeConfiguration(builder.Environment);

builder.Configuration.AddUserSecrets<Program>(optional: true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services.AddAqLifeSerilog(builder.Environment,builder.Configuration);
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<EnumDescriptionSchemaFilter>();
});

builder.Services.AddFilePolicy(
    builder.Configuration,
    builder.Environment.ContentRootPath);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddInfrastructure(builder.Environment);
builder.Services.AddGlobalExceptionPolicy<BaseExceptionHandler>();
builder.Services.AddRouteAdapter();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<FileUploadFilter>();

if(builder.Environment.IsDevelopment())
{
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
}

var app = builder.Build();

app.UseExceptionHandler();
app.CheckDatabaseConnection();

await app.OutputKeyAsync();

app.UseCors("AqLifeAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
    });
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
