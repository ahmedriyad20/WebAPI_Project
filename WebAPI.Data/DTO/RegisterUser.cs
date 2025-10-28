using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Data.DTO
{
    public class RegisterUser
    {
        [Required]
        public string Username { get; set; }


        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{4,}$", 
        //    ErrorMessage = "Password must be at least 4 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [Required]
        [MinLength(4)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }
        }
}
