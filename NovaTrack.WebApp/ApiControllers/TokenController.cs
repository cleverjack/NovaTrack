using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovaTrack.WebApp.Controllers;
using NovaTrack.WebApp.Data;
using NovaTrack.WebApp.Models;
using NovaTrack.WebApp.Services;

namespace NovaTrack.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly JWT jwt;

        public AccountController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JWT jwt) : base(db)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.jwt = jwt;
        }

        [HttpGet]
        [Route("signin")]
        public async Task<ActionResult> SignIn(string userName, string password)
        {
            if (userName == null || password == null)
            {
                return Unauthorized();
            }

            var appUser = await userManager.FindByNameAsync(userName);
            if (appUser == null || !appUser.IsActive)
            {
                return Unauthorized();
            }

            var res = await signInManager.CheckPasswordSignInAsync(appUser, password, false);
            if (res.Succeeded)
            {
                var token = jwt.GenerateJwtToken(appUser);
                var claims = await userManager.GetClaimsAsync(appUser);
                var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                return Ok(new { token, role });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("foo")]
        public async Task<ActionResult> Foo(string email, string password, string fullName, string role)
        {
            var usr = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };
            var res = await userManager.CreateAsync(usr);
            if (res.Succeeded)
            {
                var passwordToken = await userManager.GeneratePasswordResetTokenAsync(usr);
                var r1 = await userManager.ResetPasswordAsync(usr, passwordToken, password);
                var r2 = await userManager.AddClaimAsync(usr, new Claim(ClaimTypes.Role, role));
                return Ok("done");
            }
            else
            {
                return Ok(res);
            }
        }
    }
}