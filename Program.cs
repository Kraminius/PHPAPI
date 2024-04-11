using PeopleHelpPeople.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<MongoDBService>();

builder.Services.AddControllers();

//Add chat
builder.Services.AddSingleton<WebSocketMessageHandler>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Insert mock data if needed
var serviceProvider = app.Services.CreateScope().ServiceProvider;
var mongoDBService = serviceProvider.GetService<MongoDBService>();

if (mongoDBService != null)
{
    await mongoDBService.InsertMockDataIfNeededAsync();
}

// Testing closest target
double latitude = 53;
double longitude = 6;

Console.WriteLine("Searching for " + latitude + ", " + longitude);

var nearestUser = await mongoDBService?.FindNearestAsync(latitude, longitude, 100000000);
if (nearestUser != null)
{
    Console.WriteLine($"Nearest User ID: {nearestUser.UserId}, Location: {nearestUser.Location.Coordinates.Latitude}, {nearestUser.Location.Coordinates.Longitude}");
}
else
{
    Console.WriteLine("No users found nearby.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//Using WebSockets in the program
app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketMessageHandler>();
            var userId = Guid.NewGuid();
            await webSocketHandler.Handle(userId, webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
