using Neo4j.Driver;
using WexaGraphExplorer.Api.Endpoints;
using WexaGraphExplorer.Application.Graph;
using WexaGraphExplorer.Infrastructure.CognoDb;
using WexaGraphExplorer.Infrastructure.Configuration;
using WexaGraphExplorer.Infrastructure.Graph;
using WexaGraphExplorer.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Services
// ------------------------------------------------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://localhost:4201")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ------------------------------------------------------------
// CognoDB Configuration
// ------------------------------------------------------------

var cognoDbSettings = new CognoDbSettings
{
    Uri = Environment.GetEnvironmentVariable("COGNODB_URI")
        ?? string.Empty,

    Username = Environment.GetEnvironmentVariable("COGNODB_USERNAME")
        ?? string.Empty,

    Password = Environment.GetEnvironmentVariable("COGNODB_PASSWORD")
        ?? string.Empty
};

var hasCognoDbConfig =
    !string.IsNullOrWhiteSpace(cognoDbSettings.Uri) &&
    !string.IsNullOrWhiteSpace(cognoDbSettings.Username) &&
    !string.IsNullOrWhiteSpace(cognoDbSettings.Password);

if (!hasCognoDbConfig)
{
    Console.WriteLine(
        "WARNING: CognoDB environment variables are not fully configured.");
}

// ------------------------------------------------------------
// Dependency Injection
// ------------------------------------------------------------

if (hasCognoDbConfig)
{
    builder.Services.AddSingleton<IDriver>(_ =>
        CognoDbDriverFactory.Create(cognoDbSettings));

    builder.Services.AddScoped<
        IGraphExplorerRepository,
        CognoDbGraphExplorerRepository>();

    builder.Services.AddScoped<GraphExplorerService>();

    builder.Services.AddScoped<CognoDbSeeder>();
}

// ------------------------------------------------------------
// Build
// ------------------------------------------------------------

var app = builder.Build();

// ------------------------------------------------------------
// Swagger
// ------------------------------------------------------------

// Swagger is enabled in all environments so it is available
// when the application is deployed to Render.
app.UseSwagger();
app.UseSwaggerUI();

// ------------------------------------------------------------
// HTTP Pipeline
// ------------------------------------------------------------

app.UseCors("Angular");

app.MapControllers();

app.MapGraphExplorerEndpoints();

// ------------------------------------------------------------
// CognoDB Initialization
// ------------------------------------------------------------

if (hasCognoDbConfig)
{
    try
    {
        var connectionTest = new CognoDbConnectionTest();

        await connectionTest.TestConnectionAsync(
            cognoDbSettings);

        var seedFilePath = Path.GetFullPath(
            Path.Combine(
                app.Environment.ContentRootPath,
                "..",
                "scripts",
                "seed.cypher"));

        using var scope = app.Services.CreateScope();

        var seeder = scope.ServiceProvider
            .GetRequiredService<CognoDbSeeder>();

        await seeder.SeedAsync(
            cognoDbSettings,
            seedFilePath);

        Console.WriteLine(
            "CognoDB initialization completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"CognoDB initialization failed: {ex.Message}");

        Console.WriteLine(
            "The API will remain running. " +
            "Graph operations may return service-unavailable errors.");
    }
}

app.Run();