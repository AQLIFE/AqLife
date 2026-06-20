
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.StaticFiles;
using MyLife.Data.Entities;
using MyLife.Service.Behaviors;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Features;
using MyLife.Service.Handlers.Account;
using MyLife.Service.Handlers.File;
using MyLife.Service.Implementations;
using MyLife.Service.Mappings;
using MyLife.Service.ServiceInterfaces;
using MyLife.Service.ServiceInterfaces.IStrategy;
using MyLife.Service.ServiceInterfaces.IStrategy.Strategy.FileSearch;
using MyLife.Service.Validators;
using MyLife.Service.Validators.BusinessValidator;
using MyLife.Shared;
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

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateAccountHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LoginHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAccountHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateFileHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetFileMetadataHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(PreviewFileHandler).Assembly);

    // 💡 注意顺序：验证管道排在最前面，确保报错时不会浪费数据库资源
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

    // 💡 接着是事务管道
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
});


builder.AddSerilog().AddConfiguration().AddFilePolicy().AddJwtPolicy();

builder.Services.AddGlobalExceptionPolicy(builder.Environment).AddDataLayer(builder.Configuration).AddRouteAdapter();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();// 框架内置服务
builder.Services.AddHttpContextAccessor();// 框架内置服务

builder.Services.AddScoped(typeof(IValidator<>), typeof(ExistenceValidator<>));// 通用资源验证器

#region 文件相关服务注册
builder.Services.AddScoped<IJwtProvider<AccountEntity>,AuthService>();

builder.Services.AddScoped<FileUploadFilter>();

builder.Services.AddScoped(typeof(IValidator<>), typeof(FileTypeValidator<>));
builder.Services.AddScoped(typeof(IValidator<>), typeof(FileSizeValidator<>));
builder.Services.AddScoped(typeof(IValidator<>), typeof(FileDuplicateValidator<>));



//builder.Services.AddScoped<DownloadPermissionCheck>();

builder.Services.AddScoped<FileService>();
//builder.Services.AddScoped<ISearchStrategy, AllFilesSearchStrategy>();
builder.Services.AddScoped<ISearchStrategy, FilteredFilesSearchStrategy>();
builder.Services.AddScoped<FileSearch>();
builder.Services.AddScoped<UploadContext>();
builder.Services.AddSingleton<FileMapper>();
#endregion

#region 账户相关服务注册
builder.Services.AddScoped<AbstractValidator<CreateAccountCommand>, AccountNameValidator>();
builder.Services.AddScoped<AbstractValidator<CreateAccountCommand>, SubscriptionAvatarValidator>();
builder.Services.AddScoped<AbstractValidator<CreateAccountCommand>, SubscriptionContentValidator>();
builder.Services.AddScoped<AbstractValidator<CreateAccountCommand>, SubscriptionCountValidator>();

builder.Services.AddScoped<AbstractValidator<UpdateAccountAvatarCommand>, ProfilePictureValidator>();


//builder.Services.AddScoped<AccountAvatarValidCheckStrategy>();
//builder.Services.AddScoped<AccountSubscriptionAvatarValidCheckStrategy>();

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



//app.MapHealthChecks("/api/health");
//app.MapHealthChecks("/api/health/quick", new HealthCheckOptions
//{
//    // 只运行标记为 "db" 的轻量检查
//    Predicate = (check) => check.Tags.Contains("db")
//});


app.Run();