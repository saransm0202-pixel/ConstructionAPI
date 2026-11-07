using System;

namespace SSConstructions.Repository.Models;

public class ExtraModel
{
    public int ExtrasId { get; set; }

    public string ExtrasName { get; set; } = null!;

    public string? Description { get; set; }

    public int? ExtrasPrice { get; set; }

    public bool? IsExtrasExtra { get; set; }

    public string? ExtrasExtrasPlaceHolder { get; set; }

    public string? ExtrasExtrasUnit { get; set; }

    public int? AccountId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
