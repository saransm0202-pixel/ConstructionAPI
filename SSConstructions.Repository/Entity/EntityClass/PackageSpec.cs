using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SSConstructions.Repository.Entity.EntityClass;

public partial class PackageSpec
{
    [Key]
    public int PackageSpecId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string SpecLabel { get; set; } = null!;

    [StringLength(2500)]
    public string SpecValue { get; set; } = null!;

    public int PackageId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }
    public int? SpecTypeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("SpecTypeId")]
    [InverseProperty("PackageSpecs")]
    public virtual MasterPackageSpecType? SpecType { get; set; }

    [ForeignKey("PackageId")]
    [InverseProperty("PackageSpecs")]
    public virtual Package Package { get; set; } = null!;
}
