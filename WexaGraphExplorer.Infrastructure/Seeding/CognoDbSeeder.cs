using Neo4j.Driver;
using WexaGraphExplorer.Infrastructure.CognoDb;
using WexaGraphExplorer.Infrastructure.Configuration;

namespace WexaGraphExplorer.Infrastructure.Seeding;

public sealed class CognoDbSeeder
{
    public async Task SeedAsync(
        CognoDbSettings settings,
        string seedFilePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (!File.Exists(seedFilePath))
        {
            throw new FileNotFoundException(
                "Seed file was not found.",
                seedFilePath);
        }

        await using var driver =
            CognoDbDriverFactory.Create(settings);

        await driver.VerifyConnectivityAsync();

        var cypher =
            await File.ReadAllTextAsync(
                seedFilePath,
                cancellationToken);

        await using var session =
            driver.AsyncSession();

        // ----------------------------------------------------
        // Execute each statement independently.
        //
        // IMPORTANT:
        // The seed file must use MATCH/MERGE inside each
        // relationship statement because Neo4j variables
        // do not survive between queries.
        // ----------------------------------------------------

        var statements = cypher
            .Split(
                ';',
                StringSplitOptions.RemoveEmptyEntries)
            .Select(statement => statement.Trim())
            .Where(statement =>
                !string.IsNullOrWhiteSpace(statement))
            .Where(statement =>
                !IsCommentOnlyStatement(statement))
            .ToList();

        var executedStatements = 0;

        foreach (var statement in statements)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var cursor =
                await session.RunAsync(statement);

            await cursor.ConsumeAsync();

            executedStatements++;
        }

        Console.WriteLine(
            $"CognoDB seed completed. " +
            $"Executed {executedStatements} statements.");
    }

    private static bool IsCommentOnlyStatement(
        string statement)
    {
        var lines = statement
            .Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line));

        return !lines.Any(line =>
            !line.StartsWith("//"));
    }
}