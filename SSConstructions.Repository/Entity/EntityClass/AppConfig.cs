using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SSConstructions.Repository.Entity.EntityClass;

[Table("AppConfig")]
public partial class AppConfig
{
    [Key]
    public int AppConfigId { get; set; }

    public int? AccountId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string AppName { get; set; } = null!;

    public string? AppDescription { get; set; }
    public string? AppLogo { get; set; }
    public string? AccountAddress { get; set; }
    public string? LocationLink { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string? ContactNumber { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string? ContactMail { get; set; }

    public bool? EnableFacebook { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? FacebookLink { get; set; }

    public bool? EnableWhatsapp { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? WhatsappLink { get; set; }

    public bool? EnableInstagram { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? InstagramLink { get; set; }

    public bool? EnableYoutube { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string? YoutubeLink { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("AppConfigs")]
    public virtual Account? Account { get; set; }
}
