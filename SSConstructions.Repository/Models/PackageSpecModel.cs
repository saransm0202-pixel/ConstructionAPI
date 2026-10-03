namespace SSConstructions.Repository.Models;

public class PackageSpecModel
{
    public int PackageSpecId { get; set; }

    public int PackageId { get; set; }

    public string? SpecLabel { get; set; }

    public string? SpecValue { get; set; }

    public int? SpecTypeId { get; set; }

    public bool IsActive { get; set; } = true;
}