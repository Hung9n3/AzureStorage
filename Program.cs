
using Azure.Storage;
using Azure.Storage.Blobs;
using AzureStorage.AppSetting;
using AzureStorage.Core.Database;
using AzureStorage.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AzureStorageConfig>(builder.Configuration.GetSection("AzureStorageConfig"));
var AzureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorageConnectionString");
builder.Services.AddSingleton((provider) =>
{
    var config = provider.GetRequiredService<IOptionsMonitor<AzureStorageConfig>>().CurrentValue;
    return new StorageSharedKeyCredential(config.AccountName, config.AccountKey);
});
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnectionString")));
builder.Services.AddTransient<Func<string, BlobContainerClient>>(provider => containerName =>
{
    var config = provider.GetRequiredService<IOptionsMonitor<AzureStorageConfig>>().CurrentValue;
    return new BlobContainerClient(AzureStorageConnectionString, containerName);
});
builder.Services.AddTransient<Func<string, BlobServiceClient>>(provider => container =>
{
    BlobServiceClient service = new BlobServiceClient(AzureStorageConnectionString);
    return service ;
});
builder.Services.AddSingleton<AzureStorageSevice>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
    app.UseSwagger();
    app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
