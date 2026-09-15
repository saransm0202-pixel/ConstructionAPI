using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SSConstructions.Repository.Models;

public class AppConfigModel
{
    public int AppConfigId { get; set; }
    public int AccountId { get; set; }
    public string AppName { get; set; } = null!;
    public string? AppDescription { get; set; }
    public string? AccountAddress { get; set; }
    public string? LocationLink { get; set; }
    public string? AppLogo { get; set; }
    public string? ContactNumber { get; set; }
    public string? ContactMail { get; set; }
    public bool? EnableFacebook { get; set; }
    public string? FacebookLink { get; set; }
    public bool? EnableWhatsapp { get; set; }
    public string? WhatsappLink { get; set; }
    public bool? EnableInstagram { get; set; }
    public string? InstagramLink { get; set; }
    public bool? EnableYoutube { get; set; }
    public string? YoutubeLink { get; set; }
}
