using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SSConstructions.Repository.Entity.EntityClass;

[Table("Master.PackageSpecType")]
public partial class MasterPackageSpecType
{
    [Key]
    public int PackageSpecTypeId { get; set; }

    [Column("[PackageSpecType")]
    [StringLength(100)]
    [Unicode(false)]
    public string PackageSpecType { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }
    [InverseProperty("SpecType")]
    public virtual ICollection<PackageSpec> PackageSpecs { get; set; } = new List<PackageSpec>();
}
