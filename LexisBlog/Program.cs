using LexisBlog.Models;
using LexisBlog.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// MongoDB Connection Settings
var mongoSettings = new MongoSettings();
builder.Configuration.GetSection("MongoDbSettings").Bind(mongoSettings);

// MongoDB Repositories
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddScoped<UserRepository>(); // Add UserRepository
builder.Services.AddScoped<BlogRepository>(); // Add BlogRepository
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

app.MapControllers();

app.Run();