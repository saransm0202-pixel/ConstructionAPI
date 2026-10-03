using System.Collections.Generic;

namespace SSConstructions.Repository.Models;

public class PackageModel
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public string? Description { get; set; }

    public int? PackagePrice { get; set; }

    public int? AccountId { get; set; }

    public int? ConstructionTypeId { get; set; }

    public string? ConstructionType { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public System.DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public System.DateTime? ModifiedDate { get; set; }

    public List<PackageFeatureModel> Features { get; set; } = new List<PackageFeatureModel>();

    public List<PackageSpecModel> Specs { get; set; } = new List<PackageSpecModel>();
}