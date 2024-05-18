using PHPAPI.Model;
using Microsoft.AspNetCore.HttpOverrides;
using PHPAPI.Services;
using PHPAPI.Controllers;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDBSettings>(
builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<MongoDBService>();


builder.Services.AddScoped<MongoDBContext, MongoDBContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();


builder.Services.AddControllers()
        .AddApplicationPart(typeof(AuthController).Assembly);

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully.");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("OnChallenge error: " + context.ErrorDescription);
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(RsaKeyProvider.PublicKey),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


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

// Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapControllers();

app.Run();
