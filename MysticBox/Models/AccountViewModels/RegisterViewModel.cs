﻿using System.ComponentModel.DataAnnotations;

namespace MysticBox.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(16, ErrorMessage = "Your username can't be longer than 16 characters long.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Invitation")]
        public string InvitationCode { get; set; }
    }
}
