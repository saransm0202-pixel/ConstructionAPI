using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSConstructions.Repository.Entity.EntityClass;

public partial class Project
{
    [Key]
    public int ProjectId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string ProjectStatus { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string ProjectLocation { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string ProjectType { get; set; } = null!;

    public int ProjectArea { get; set; }

    public int? AccountId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Projects")]
    public virtual Account? Account { get; set; }
}
