
using MediatR;
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
using MyLife.Shared.Command;
using MyLife.Shared.Validator;
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
        policy.WithOrigins("http://192.168.0.100:5173", "http://localhost:5173", "http://127.0.0.1:5173") // 允许你的 Vue 开发服务器地址
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // 如果后续涉及 Cookie/Auth，建议开启
              .WithExposedHeaders("Authorization");
    });
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateAccountHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LoginHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAccountHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(DeleteAccountHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(UpdateAccountProfileHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(UpdateAccountAvatarHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(UpdateAccountSubscriptionsHandler).Assembly);

    cfg.RegisterServicesFromAssembly(typeof(CreateFileHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetFileMetadataHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(PreviewFileHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(DownloadFileHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(DeleteFileHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(UpdateFileHandler).Assembly);

    // 💡 注意顺序：验证管道排在最前面，确保报错时不会浪费数据库资源
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    // 💡 接着是事务管道
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
});


builder.AddSerilog().AddConfiguration().AddFilePolicy().AddJwtPolicy();

builder.Services.AddGlobalExceptionPolicy(builder.Environment).AddDataLayer(builder.Configuration).AddRouteAdapter();
builder.Services.AddScoped<IJwtProvider<AccountEntity>, AuthService>();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();// 框架内置服务
builder.Services.AddHttpContextAccessor();// 框架内置服务
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<AccountService>();


builder.Services.AddSingleton<TodoMapper>();
builder.Services.AddSingleton<FileMapper>();
builder.Services.AddSingleton<SubscriptionMapper>();
builder.Services.AddSingleton<AccountMapper>();


#region 文件相关服务注册
builder.Services.AddScoped<FileUploadFilter>();
builder.Services.AddScoped<UploadContext>();
builder.Services.AddScoped<ISearchStrategy, FilteredFilesSearchStrategy>();
builder.Services.AddScoped<FileSearch>();

builder.Services.AddScoped(typeof(IValidator<>), typeof(ExistenceValidator<>));// 通用资源验证器
builder.Services.AddScoped(typeof(IValidator<>),typeof(FileTypeValidator<>));
builder.Services.AddScoped(typeof(IValidator<>),typeof(FileSizeValidator<>));
builder.Services.AddScoped(typeof(IValidator<>),typeof(FileDuplicateValidator<>));
#endregion

#region 账户相关服务注册
builder.Services.AddScoped<IValidator<LoginCommand>, LoginValidator>();
builder.Services.AddScoped<IValidator<CreateAccountCommand>, AccountNameValidator>();
builder.Services.AddScoped<IValidator<UpdateAccountSubscriptionsCommand>, SubscriptionImageValidator>();
builder.Services.AddScoped<IValidator<UpdateAccountSubscriptionsCommand>, SubscriptionContentValidator>();
#endregion


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



app.Run();