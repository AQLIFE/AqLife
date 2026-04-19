
using MyLife.Core.Provider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.apu();
builder.BindLogger().BindStoragePolicy();

// register file policy using dedicated extension method
//builder.AddFilePolicyFromEnvironment();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.InitCheckDatabaseConnection();

app.UseAuthorization();

app.MapControllers();

app.Run();
