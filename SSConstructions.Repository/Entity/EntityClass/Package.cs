using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SSConstructions.Repository.Entity.EntityClass;

public partial class Package
{
    [Key]
    public int PackageId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string PackageName { get; set; } = null!;

    public string? Description { get; set; }

    public int? PackagePrice { get; set; }

    public int? AccountId { get; set; }

    public int? ConstructionTypeId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Packages")]
    public virtual Account? Account { get; set; }

    [ForeignKey("ConstructionTypeId")]
    [InverseProperty("Packages")]
    public virtual MasterConstructionType? ConstructionType { get; set; }

    [InverseProperty("Package")]
    public virtual ICollection<PackageFeature> PackageFeatures { get; set; } = new List<PackageFeature>();
    [InverseProperty("Package")]
    public virtual ICollection<PackageSpec> PackageSpecs { get; set; } = new List<PackageSpec>();
}
