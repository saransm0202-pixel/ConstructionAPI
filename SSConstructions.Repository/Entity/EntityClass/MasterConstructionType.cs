using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SSConstructions.Repository.Entity.EntityClass;

[Table("Master.ConstructionType")]
public partial class MasterConstructionType
{
    [Key]
    public int ConstructionTypeId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ConstructionType { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [InverseProperty("ConstructionType")]
    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
}
