namespace SSConstructions.Repository.Models;

public class PackageSpecTypeModel
{
    public int PackageSpecTypeId { get; set; }

    public string? PackageSpecType { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}