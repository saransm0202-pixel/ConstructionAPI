using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSConstructions.Repository.Entity.EntityClass;

public partial class Account
{
    [Key]
    public int AccountId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string AccountName { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<AppConfig> AppConfigs { get; set; } = new List<AppConfig>();
    [InverseProperty("Account")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [InverseProperty("Account")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
