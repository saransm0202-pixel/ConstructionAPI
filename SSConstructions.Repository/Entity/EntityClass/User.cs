using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSConstructions.Repository.Entity.EntityClass;

public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? ProfilePic { get; set; }

    public int? RoleId { get; set; }

    public int? AccountId { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    [StringLength(500)]
    public string? RefreshToken { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RefreshTokenExpiry { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Users")]
    public virtual Account? Account { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual MasterRole? Role { get; set; }
}
