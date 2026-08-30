namespace WexaGraphExplorer.Application.Graph;

public sealed record GraphHealthDto(
    string Status,
    string Message);

public sealed record GraphSummaryDto(
    string Label,
    long Count);

public sealed record MissingTalentDto(
    string DeveloperId,
    string DeveloperName,
    string? Company,
    IReadOnlyList<string> MatchingSkills,
    long WorkCount);

public sealed record ProjectDependencyDto(
    string ConnectedProject,
    string SharedDeveloper,
    IReadOnlyList<string> SharedSkills,
    IReadOnlyList<string> DependencyChain);