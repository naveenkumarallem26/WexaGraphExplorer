using Microsoft.AspNetCore.Mvc;
using WexaGraphExplorer.Application.Graph;

namespace WexaGraphExplorer.Api.Endpoints;

public static class GraphExplorerEndpoints
{
    public static void MapGraphExplorerEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/graph")
            .WithTags("Wexa Graph Explorer");

        // --------------------------------------------------------
        // Health
        // --------------------------------------------------------

        group.MapGet(
            "/health",
            async (
                [FromServices] GraphExplorerService service,
                CancellationToken ct) =>
            {
                try
                {
                    var result =
                        await service.CheckHealthAsync(ct);

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"CognoDB health check failed: {ex}");

                    return Results.Problem(
                        "The graph database is currently unavailable.",
                        statusCode:
                            StatusCodes.Status503ServiceUnavailable);
                }
            });

        // --------------------------------------------------------
        // Graph Summary
        // --------------------------------------------------------

        group.MapGet(
            "/summary",
            async (
                [FromServices] GraphExplorerService service,
                CancellationToken ct) =>
            {
                try
                {
                    var result =
                        await service.GetSummaryAsync(ct);

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Database summary query failed: {ex}");

                    return Results.Problem(
                        "Unable to load graph summary.",
                        statusCode:
                            StatusCodes.Status503ServiceUnavailable);
                }
            });

        // --------------------------------------------------------
        // Talent Finder
        // --------------------------------------------------------

        group.MapGet(
            "/projects/{projectName}/missing-talent",
            async (
                string projectName,
                [FromServices] GraphExplorerService service,
                CancellationToken ct) =>
            {
                if (string.IsNullOrWhiteSpace(projectName))
                {
                    return Results.BadRequest(
                        new
                        {
                            message =
                                "Project name is required."
                        });
                }

                try
                {
                    var result =
                        await service.GetMissingTalentAsync(
                            projectName.Trim(),
                            ct);

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Missing-talent query failed: {ex}");

                    return Results.Problem(
                        "Unable to run the talent matching query.",
                        statusCode:
                            StatusCodes.Status503ServiceUnavailable);
                }
            });

        // --------------------------------------------------------
        // Dependencies
        // --------------------------------------------------------

        group.MapGet(
            "/projects/{projectName}/dependencies",
            async (
                string projectName,
                [FromServices] GraphExplorerService service,
                CancellationToken ct) =>
            {
                if (string.IsNullOrWhiteSpace(projectName))
                {
                    return Results.BadRequest(
                        new
                        {
                            message =
                                "Project name is required."
                        });
                }

                try
                {
                    var result =
                        await service.GetDependenciesAsync(
                            projectName.Trim(),
                            ct);

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Project dependency query failed: {ex}");

                    return Results.Problem(
                        "Unable to run the project dependency query.",
                        statusCode:
                            StatusCodes.Status503ServiceUnavailable);
                }
            });
    }
}