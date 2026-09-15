using System.ComponentModel.DataAnnotations;

namespace SSConstructions.Repository.Models
{
    public class ConsultationRequest
    {
        [Required]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Please enter a valid name.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plot location is required.")]
        [StringLength(200)]
        public string PlotLocation { get; set; } = string.Empty;

        [StringLength(100)]
        public string? PlotArea { get; set; }

        [StringLength(100)]
        public string? Package { get; set; }

        [StringLength(1000)]
        public string? Message { get; set; }
    }
}