using Neo4j.Driver;
using WexaGraphExplorer.Api.Endpoints;
using WexaGraphExplorer.Application.Graph;
using WexaGraphExplorer.Infrastructure.CognoDb;
using WexaGraphExplorer.Infrastructure.Configuration;
using WexaGraphExplorer.Infrastructure.Graph;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// Services
// --------------------------------------------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// --------------------------------------------------------
// CORS
// --------------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWexaFrontend", policy =>
    {
        policy
            .WithOrigins(
                "https://wexa-graph-web.onrender.com",
                "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// --------------------------------------------------------
// CognoDB configuration
// --------------------------------------------------------

var cognoDbSettings = new CognoDbSettings
{
    Uri = Environment.GetEnvironmentVariable("COGNODB_URI")
        ?? string.Empty,

    Username = Environment.GetEnvironmentVariable("COGNODB_USERNAME")
        ?? string.Empty,

    Password = Environment.GetEnvironmentVariable("COGNODB_PASSWORD")
        ?? string.Empty
};

if (string.IsNullOrWhiteSpace(cognoDbSettings.Uri) ||
    string.IsNullOrWhiteSpace(cognoDbSettings.Username) ||
    string.IsNullOrWhiteSpace(cognoDbSettings.Password))
{
    Console.WriteLine(
        "WARNING: CognoDB environment variables are not fully configured.");
}
else
{
    builder.Services.AddSingleton(cognoDbSettings);

    // Create and register the CognoDB / Neo4j driver.
    builder.Services.AddSingleton<IDriver>(_ =>
        CognoDbDriverFactory.Create(cognoDbSettings));

    // Register the graph repository implementation.
    builder.Services.AddScoped<
        IGraphExplorerRepository,
        CognoDbGraphExplorerRepository>();

    // Register the application service.
    builder.Services.AddScoped<GraphExplorerService>();
}

// --------------------------------------------------------
// Build application
// --------------------------------------------------------

var app = builder.Build();

// --------------------------------------------------------
// Swagger
// --------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --------------------------------------------------------
// Middleware
// --------------------------------------------------------

app.UseCors("AllowWexaFrontend");

app.UseAuthorization();

// --------------------------------------------------------
// Controllers
// --------------------------------------------------------

app.MapControllers();

// --------------------------------------------------------
// Minimal API graph endpoints
// --------------------------------------------------------

app.MapGraphExplorerEndpoints();

// --------------------------------------------------------
// Run
// --------------------------------------------------------

app.Run();