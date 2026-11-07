using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SSConstructions.Repository.Entity.EntityClass;

public partial class Extra
{
    [Key]
    public int ExtrasId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string ExtrasName { get; set; } = null!;

    public string? Description { get; set; }

    public int? ExtrasPrice { get; set; }

    public bool? IsExtrasExtra { get; set; }

    [StringLength(550)]
    [Unicode(false)]
    public string? ExtrasExtrasPlaceHolder { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? ExtrasExtrasUnit { get; set; }

    public int? AccountId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Extras")]
    public virtual Account? Account { get; set; }
}
