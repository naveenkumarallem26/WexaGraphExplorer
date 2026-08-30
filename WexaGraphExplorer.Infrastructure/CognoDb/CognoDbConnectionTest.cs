using Neo4j.Driver;
using WexaGraphExplorer.Infrastructure.Configuration;

namespace WexaGraphExplorer.Infrastructure.CognoDb;

public sealed class CognoDbConnectionTest
{
    public async Task TestConnectionAsync(
        CognoDbSettings settings,
        CancellationToken cancellationToken = default)
    {
        await using var driver =
            CognoDbDriverFactory.Create(settings);

        await driver.VerifyConnectivityAsync();

        await using var session =
            driver.AsyncSession();

        const string query = """
            RETURN
                'CognoDB connection successful' AS message
            """;

        var cursor =
            await session.RunAsync(query);

        var record =
            await cursor.SingleAsync(
                cancellationToken);

        Console.WriteLine(
            record["message"].As<string>());
    }
}