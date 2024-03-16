
using PeopleHelpPeople.ChatHub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// app.MapHub<ChatGroupHub>("/ChatGroupHub");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatGroupHub>("/ChatGroupHub");
    // Other endpoint mappings...
});

app.MapControllers();

app.Run();
