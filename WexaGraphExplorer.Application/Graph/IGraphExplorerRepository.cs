namespace WexaGraphExplorer.Application.Graph;

public interface IGraphExplorerRepository
{
    Task<GraphHealthDto> CheckHealthAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GraphSummaryDto>> GetSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MissingTalentDto>> GetMissingTalentAsync(
        string projectName,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectDependencyDto>> GetDependenciesAsync(
        string projectName,
        CancellationToken cancellationToken = default);
}