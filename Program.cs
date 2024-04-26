using PHPAPI.Model;
using Microsoft.AspNetCore.HttpOverrides;
using PHPAPI.Services;
using PHPAPI.Controllers;
using Azure.Identity;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<MongoDBService>();

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Insert mock data if needed
var serviceProvider = app.Services.CreateScope().ServiceProvider;
MongoDBService? mongoDBService = null;

int retries = 0;
int maxRetries = 10;
int delayMilliseconds = 500;

while (retries < maxRetries)
    try {
    mongoDBService = serviceProvider.GetService<MongoDBService>();
        break;
    } catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        retries++;
        await Task.Delay(delayMilliseconds);
        delayMilliseconds *= 2;
    }




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


/*builder.Configuration.AddAzureKeyVault(
    new Uri("https://phpjwt.vault.azure.net/"),
    new DefaultAzureCredential());

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers()
        .AddApplicationPart(typeof(AuthController).Assembly);*/

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.Run();
