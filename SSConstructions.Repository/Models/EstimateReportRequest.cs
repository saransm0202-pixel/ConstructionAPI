using System.ComponentModel.DataAnnotations;

namespace SSConstructions.Repository.Models
{
    public class EstimateReportRequest
    {
        [Required]
        public int AccountId { get; set; }

        [StringLength(150)]
        public string? CustomerName { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? CustomerEmail { get; set; }

        [StringLength(20)]
        public string? CustomerPhone { get; set; }

        [StringLength(50)]
        public string Category { get; set; } = "Residential";

        [StringLength(150)]
        public string PackageName { get; set; } = string.Empty;

        public decimal RatePerSqft { get; set; }

        public decimal PlotAreaSqft { get; set; }

        public decimal TotalBuiltUpSqft { get; set; }

        [StringLength(50)]
        public string Configuration { get; set; } = string.Empty;

        public int DurationMonths { get; set; }

        public decimal BaseCost { get; set; }

        public decimal ExtrasCost { get; set; }

        public decimal TotalCost { get; set; }

        public decimal EmiMonthly { get; set; }

        [StringLength(160)]
        public string? FileName { get; set; }
    }
}