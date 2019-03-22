using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaTrack.WebApp.Data;
using NovaTrack.WebApp.Models;
using System.Linq;
using System.Security.Claims;

namespace NovaTrack.WebAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        public BaseController(ApplicationDbContext db)
        {
            DB = db;
        }

        public ApplicationDbContext DB { get; }

        private ApplicationUser _applicationUser;
        public ApplicationUser ApplicationUser
        {
            get
            {
                if (_applicationUser == null)
                {
                    var userName = this.User?.Identity?.Name;
                    if (userName != null)
                    {
                        _applicationUser = DB.Users.Include(u => u.Claims).FirstOrDefault(u => u.UserName == userName);
                    }
                }
                return _applicationUser;
            }
        }
    }
}