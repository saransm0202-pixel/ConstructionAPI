using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SSConstructions.Repository.Entity.EntityClass;

public partial class PackageFeature
{
    [Key]
    public int PackageFeatureId { get; set; }

    [StringLength(2500)]
    public string? Feature { get; set; }

    public int PackageId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("PackageId")]
    [InverseProperty("PackageFeatures")]
    public virtual Package Package { get; set; } = null!;
}
