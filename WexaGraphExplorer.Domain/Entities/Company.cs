namespace WexaGraphExplorer.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Industry { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
}