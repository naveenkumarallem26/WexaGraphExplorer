using Neo4j.Driver;
using WexaGraphExplorer.Infrastructure.Configuration;

namespace WexaGraphExplorer.Infrastructure.CognoDb;

public static class CognoDbDriverFactory
{
    public static IDriver Create(
        CognoDbSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (string.IsNullOrWhiteSpace(settings.Uri))
        {
            throw new InvalidOperationException(
                "COGNODB_URI is not configured.");
        }

        if (string.IsNullOrWhiteSpace(settings.Username))
        {
            throw new InvalidOperationException(
                "COGNODB_USERNAME is not configured.");
        }

        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            throw new InvalidOperationException(
                "COGNODB_PASSWORD is not configured.");
        }

        return GraphDatabase.Driver(
            settings.Uri,
            AuthTokens.Basic(
                settings.Username,
                settings.Password));
    }
}