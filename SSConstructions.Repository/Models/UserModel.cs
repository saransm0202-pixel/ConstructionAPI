using System;
using System.Collections.Generic;
using System.Text;

namespace SSConstructions.Repository.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ProfilePic { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsActive { get; set; }
        public string? LoginMsg { get; set; }
        public DateTime? expiresAt { get; set; }
        public int AccountId { get; set; }
    }
}
