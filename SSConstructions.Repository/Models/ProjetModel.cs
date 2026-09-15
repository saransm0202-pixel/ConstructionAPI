namespace SSConstructions.Repository.Models;

public class ProjectModel
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string ProjectStatus { get; set; } = null!;

    public string ProjectLocation { get; set; } = null!;

    public string ProjectType { get; set; } = null!;

    public int ProjectArea { get; set; }

    public bool IsActive { get; set; }
    public int? AccountId { get; set; }
}
