using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ABGAssignment.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        // Name must contain 200 characters with character only
        [Required]
        [MaxLength(200)]
        [RegularExpression("^[a-zA-Z ]+$")]
        public string Name { get; set; }

        // Email id must be validated
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string EmailId { get; set; }

        // Mobile number must be number
        [Required]
        [MaxLength(15)]
        public string MobileNumber { get; set; }

        // User id must be max 20 characters and only alpha numeric
        [Required]
        [MaxLength(20)]
        [RegularExpression("^[a-zA-Z0-9]+$")]
        public string UserId { get; set; }

        // Password must be hashed
        [Required]
        public string Password { get; set; }
    }
}
