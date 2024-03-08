using PeopleHelpPeople.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;


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
var mongoDBService = serviceProvider.GetService<MongoDBService>();

Console.WriteLine(mongoDBService);

mongoDBService?.InsertMockDataIfNeededAsync().GetAwaiter().GetResult();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Console.WriteLine("SOMETHING");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
