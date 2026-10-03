namespace SSConstructions.Repository.Models;

public class ConstructionTypeModel
{
    public int ConstructionTypeId { get; set; }

    public string ConstructionType { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}