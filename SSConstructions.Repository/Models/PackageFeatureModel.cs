namespace SSConstructions.Repository.Models;

public class PackageFeatureModel
{
    public int PackageFeatureId { get; set; }

    public int PackageId { get; set; }

    public string? Feature { get; set; }

    public bool IsActive { get; set; } = true;
}