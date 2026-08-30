using WexaGraphExplorer.Infrastructure.CognoDb;
using WexaGraphExplorer.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// Configure CORS for the Angular frontend.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWexaFrontend", policy =>
    {
        policy
            .WithOrigins(
                "https://wexa-graph-web.onrender.com",
                "http://localhost:4200"
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Read CognoDB configuration from environment variables.
var cognoDbSettings = new CognoDbSettings
{
    Uri = Environment.GetEnvironmentVariable("COGNODB_URI") ?? string.Empty,
    Username = Environment.GetEnvironmentVariable("COGNODB_USERNAME") ?? string.Empty,
    Password = Environment.GetEnvironmentVariable("COGNODB_PASSWORD") ?? string.Empty
};

if (string.IsNullOrWhiteSpace(cognoDbSettings.Uri) ||
    string.IsNullOrWhiteSpace(cognoDbSettings.Username) ||
    string.IsNullOrWhiteSpace(cognoDbSettings.Password))
{
    Console.WriteLine("WARNING: CognoDB environment variables are not fully configured.");
}
else
{
    builder.Services.AddSingleton(cognoDbSettings);
}

var app = builder.Build();

// Swagger is enabled only in development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS must run before the endpoints.
app.UseCors("AllowWexaFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();