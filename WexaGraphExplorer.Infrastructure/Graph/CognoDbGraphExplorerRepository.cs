using Neo4j.Driver;
using WexaGraphExplorer.Application.Graph;

namespace WexaGraphExplorer.Infrastructure.Graph;

// Repository responsible for communicating with CognoDB
// and executing graph queries required by the application.
public sealed class CognoDbGraphExplorerRepository(
    IDriver driver) : IGraphExplorerRepository
{
    // Checks whether the application can successfully
    // communicate with CognoDB.
    public async Task<GraphHealthDto> CheckHealthAsync(
        CancellationToken cancellationToken = default)
    {
        // Simple Cypher query used to verify that the
        // database can execute a query successfully.
        const string query = """
            RETURN
                'Healthy' AS Status,
                'Successfully connected to CognoDB over Bolt.' AS Message
            """;

        // Creates an asynchronous session using the injected
        // CognoDB/Neo4j driver.
        await using var session = driver.AsyncSession();

        // Executes the health-check Cypher query.
        var cursor = await session.RunAsync(query);

        // Reads the single record returned by the query.
        var record = await cursor.SingleAsync(
            cancellationToken);

        // Maps the database result into the application DTO.
        return new GraphHealthDto(
            record["Status"].As<string>(),
            record["Message"].As<string>());
    }


    // Retrieves a summary containing the number of nodes
    // grouped by their graph label.
    public async Task<IReadOnlyList<GraphSummaryDto>> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        // Cypher query that reads all graph nodes,
        // gets the first label of each node,
        // counts nodes by label, and sorts the result.
        const string query = """
            MATCH (n)
            WITH
                labels(n)[0] AS Label,
                count(n) AS Count

            RETURN
                Label,
                Count

            ORDER BY Label
            """;

        // Creates an asynchronous database session.
        await using var session = driver.AsyncSession();

        // Executes the summary query.
        var cursor = await session.RunAsync(query);

        // Reads all records returned by the database.
        var records = await cursor.ToListAsync(
            cancellationToken);

        // Converts each database record into a GraphSummaryDto.
        return records
            .Select(record =>
                new GraphSummaryDto(
                    record["Label"].As<string>(),
                    record["Count"].As<long>()))
            .ToList();
    }


    // Finds developers who have skills required by a project
    // but have not already worked on that target project.
    public async Task<IReadOnlyList<MissingTalentDto>>
        GetMissingTalentAsync(
            string projectName,
            CancellationToken cancellationToken = default)
    {
        // Parameterized Cypher query for the Talent Finder feature.
        // $projectName is supplied separately as a query parameter,
        // rather than being concatenated into the Cypher string.
        const string query = """
            MATCH (target:Project)
            WHERE toLower(trim(target.name)) =
                  toLower(trim($projectName))

            // Finds the skills required by the target project.
            MATCH (target)-[:USES_SKILL]->(requiredSkill:Skill)

            // Finds developers and the skills they possess.
            MATCH (candidate:Developer)
                  -[:HAS_SKILL]->(candidateSkill:Skill)

            // Compares candidate skills with required project skills
            // using case-insensitive and whitespace-insensitive matching.
            WHERE toLower(trim(candidateSkill.name)) =
                  toLower(trim(requiredSkill.name))

            // Groups matching skills for each developer.
            WITH
                target,
                candidate,
                collect(DISTINCT candidateSkill.name)
                    AS matchingSkills

            // Finds projects previously worked on by each candidate.
            OPTIONAL MATCH
                (candidate)-[:WORKED_ON]->(workedProject:Project)

            // Collects the IDs of projects worked on by the candidate.
            WITH
                target,
                candidate,
                matchingSkills,
                collect(
                    DISTINCT elementId(workedProject)
                ) AS workedProjectIds

            // Excludes developers who already worked on the target project.
            WHERE NOT elementId(target) IN workedProjectIds

            // Calculates how many projects the candidate has worked on.
            WITH
                candidate,
                matchingSkills,
                size(workedProjectIds) AS workCount

            // Finds the company associated with the candidate.
            OPTIONAL MATCH
                (candidate)-[:WORKED_AT]->(company:Company)

            // Collects company names for the candidate.
            WITH
                candidate,
                matchingSkills,
                workCount,
                collect(DISTINCT company.name) AS companies

            // Returns the information required by the Talent Finder UI.
            RETURN
                coalesce(
                    candidate.email,
                    candidate.name
                ) AS DeveloperId,

                candidate.name AS DeveloperName,

                CASE
                    WHEN size(companies) > 0
                    THEN companies[0]
                    ELSE null
                END AS Company,

                matchingSkills AS MatchingSkills,

                workCount AS WorkCount

            // Places candidates with the most matching skills first.
            ORDER BY
                size(matchingSkills) DESC,
                DeveloperName
            """;

        // Creates the query parameter separately from the Cypher query.
        // This prevents user input from being directly concatenated
        // into the query string.
        var parameters = new Dictionary<string, object>
        {
            ["projectName"] = projectName
        };

        // Creates an asynchronous database session.
        await using var session = driver.AsyncSession();

        // Executes the parameterized Cypher query.
        var cursor = await session.RunAsync(
            query,
            parameters);

        // Reads all matching developer records.
        var records = await cursor.ToListAsync(
            cancellationToken);

        // Maps database records into MissingTalentDto objects.
        return records
            .Select(record =>
            {
                // Reads the optional company value.
                var company =
                    record["Company"].As<string?>();

                // Reads the list of skills matching the project requirements.
                var matchingSkills =
                    record["MatchingSkills"]
                        .As<List<string>>();

                // Reads the number of projects worked on by the developer.
                var workCount =
                    record["WorkCount"].As<long>();

                // Creates the DTO returned to the application layer.
                return new MissingTalentDto(
                    record["DeveloperId"].As<string>(),
                    record["DeveloperName"].As<string>(),
                    company,
                    matchingSkills,
                    workCount);
            })
            .ToList();
    }


    // Finds projects connected to the requested project through
    // shared developers and shared skills.
    public async Task<IReadOnlyList<ProjectDependencyDto>>
        GetDependenciesAsync(
            string projectName,
            CancellationToken cancellationToken = default)
    {
        // Parameterized Cypher query for the Project Dependencies feature.
        // The target project is identified using $projectName.
        const string query = """
            MATCH (target:Project)
            WHERE toLower(trim(target.name)) =
                  toLower(trim($projectName))

            // Finds developers who have worked on the target project.
            MATCH (sharedDeveloper:Developer)
                  -[:WORKED_ON]->(target)

            // Finds other projects worked on by the same developer.
            // This creates a multi-hop graph traversal:
            // Project -> Developer -> Project
            MATCH (sharedDeveloper)
                  -[:WORKED_ON]->(connectedProject:Project)

            // Prevents the target project from being returned as its own dependency.
            WHERE connectedProject <> target

            // Finds skills used by the target project.
            MATCH (target)
                  -[:USES_SKILL]->(sharedSkill:Skill)

            // Finds the same skills used by the connected project.
            MATCH (connectedProject)
                  -[:USES_SKILL]->(sharedSkill)

            // Groups the connected project, developer,
            // and shared skills together.
            WITH
                target,
                connectedProject,
                sharedDeveloper,
                collect(DISTINCT sharedSkill.name)
                    AS sharedSkills

            // Returns the connected project, shared developer,
            // and a human-readable dependency chain.
            RETURN
                connectedProject.name AS ConnectedProject,

                sharedDeveloper.name AS SharedDeveloper,

                (
                    ['Project:' + target.name]
                    +
                    [skill IN sharedSkills |
                        'Skill:' + skill]
                    +
                    ['Project:' + connectedProject.name]
                ) AS DependencyChain

            // Sorts results first by connected project
            // and then by developer.
            ORDER BY
                ConnectedProject,
                SharedDeveloper
            """;

        // Supplies the project name as a parameter to the Cypher query.
        var parameters = new Dictionary<string, object>
        {
            ["projectName"] = projectName
        };

        // Creates an asynchronous database session.
        await using var session = driver.AsyncSession();

        // Executes the parameterized dependency query.
        var cursor = await session.RunAsync(
            query,
            parameters);

        // Reads all dependency records returned by CognoDB.
        var records = await cursor.ToListAsync(
            cancellationToken);

        // Maps each database record into a ProjectDependencyDto.
        return records
            .Select(record =>
            {
                // Reads the dependency chain returned by the query.
                var dependencyChain =
                    record["DependencyChain"]
                        .As<List<string>>();

                // Extracts only Skill entries from the dependency chain.
                var sharedSkills =
                    dependencyChain
                        .Where(item =>
                            item.StartsWith(
                                "Skill:",
                                StringComparison.Ordinal))
                        .Select(item =>
                            item["Skill:".Length..])
                        .ToList();

                // Creates the DTO returned to the application layer
                // and ultimately consumed by the frontend.
                return new ProjectDependencyDto(
                    record["ConnectedProject"].As<string>(),
                    record["SharedDeveloper"].As<string>(),
                    sharedSkills,
                    dependencyChain);
            })
            .ToList();
    }
}