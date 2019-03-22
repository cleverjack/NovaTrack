using NovaTrack.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NovaTrack.WebApp.ViewModels
{
    public class UsersIndexVM
    {
        public List<ApplicationUser> Users { get; set; }
        public string Message { get; set; }
    }

    public class UsersEditVM
    {
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        //[DisplayName("Email")]
        public string Email { get; set; }
        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [Required]
        [DisplayName("Role")]
        public string Role { get; set; }
        public string Password { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        public bool IsNew
        {
            get
            {
                return this.UserId == null;
            }
        }
    }
}
