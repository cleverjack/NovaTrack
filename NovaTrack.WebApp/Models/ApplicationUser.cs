using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NovaTrack.WebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();
        public string FullName { get; set; }
        public bool IsActive { get; set; } = true;

        public string[] Roles
        {
            get
            {
                return Claims?.Where(c => c.ClaimType == ClaimTypes.Role)?.Select(c => c.ClaimValue)?.ToArray();
            }
        }
    }
}
