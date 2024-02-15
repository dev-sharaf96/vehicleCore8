using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tamkeen.bll.Model
{
    /// <summary>
    /// User Login Request Message ..
    /// </summary>
    public class LoginRequestMessage
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public String email { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [RegularExpression("^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,}$", ErrorMessage = "Password must contain at least one letter, at least one number, and be longer than six charaters.")]
        public String password { get; set; }
        [Compare("password")]
        public String confirmPassword { get; set; }
        public bool rememberme { get; set; }
    }
    /// <summary>
    /// Login Response Message .. 
    /// </summary>
    /// TODO Edit XML Comment Template for LoginResponseMessage
    public class LoginResponseMessage
    {
        public int status { get; set; }
        public String userId { get; set; }
        public String name { get; set; }
        public String image { get; set; }
        public String errorMessage { get; set; }
        public List<ValidationResult> error { get; set; }
    }
}
