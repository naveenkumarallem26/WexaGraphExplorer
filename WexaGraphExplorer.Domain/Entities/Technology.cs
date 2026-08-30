namespace WexaGraphExplorer.Domain.Entities;

public class Technology
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
}