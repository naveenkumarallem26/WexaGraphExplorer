namespace WexaGraphExplorer.Application.Graph;

public sealed class GraphExplorerService(
    IGraphExplorerRepository repository)
{
    public Task<GraphHealthDto> CheckHealthAsync(
        CancellationToken cancellationToken = default)
    {
        return repository.CheckHealthAsync(
            cancellationToken);
    }

    public Task<IReadOnlyList<GraphSummaryDto>> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        return repository.GetSummaryAsync(
            cancellationToken);
    }

    public Task<IReadOnlyList<MissingTalentDto>> GetMissingTalentAsync(
        string projectName,
        CancellationToken cancellationToken = default)
    {
        return repository.GetMissingTalentAsync(
            projectName,
            cancellationToken);
    }

    public Task<IReadOnlyList<ProjectDependencyDto>> GetDependenciesAsync(
        string projectName,
        CancellationToken cancellationToken = default)
    {
        return repository.GetDependenciesAsync(
            projectName,
            cancellationToken);
    }
}