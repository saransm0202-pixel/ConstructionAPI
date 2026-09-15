namespace SSConstructions.Repository.Models
{
    // Request models
    public class EmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public int AccountId { get; set; } = 0;
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public int AccountId { get; set; } = 0;
    }
}
