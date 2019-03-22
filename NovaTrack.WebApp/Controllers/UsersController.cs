using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaTrack.WebApp.Data;
using NovaTrack.WebApp.Models;
using NovaTrack.WebApp.ViewModels;

namespace NovaTrack.WebApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }
        public IActionResult Index(string message)
        {
            var users = db.Users.Include(u => u.Claims).ToList().OrderByDescending(u => u.Roles.Length).OrderByDescending(u => u.Roles.FirstOrDefault())?.OrderBy(u => u.Email).ToList();
            var vm = new UsersIndexVM { Users = users, Message = message };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var vm = new UsersEditVM();

            if (id != null)
            {
                var user = db.Users.Include(u => u.Claims).FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound();
                }
                vm.UserId = user.Id;
                vm.Email = user.Email;
                vm.FullName = user.FullName;
                vm.Role = user.Roles.FirstOrDefault();
                vm.IsActive = user.IsActive;
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UsersEditVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            if (vm.UserId == null)
            {
                if (string.IsNullOrWhiteSpace( vm.Password ))
                {
                    ModelState.AddModelError("Password", "The password is required!");
                    return View(vm);
                }
                var usr = new ApplicationUser
                {
                    UserName = vm.Email,
                    Email = vm.Email,
                    FullName = vm.FullName,
                    IsActive = vm.IsActive
                };

                var res = await userManager.CreateAsync(usr);

                if (res.Succeeded)
                {
                    var passwordToken = await userManager.GeneratePasswordResetTokenAsync(usr);
                    var r1 = await userManager.ResetPasswordAsync(usr, passwordToken, vm.Password);
                    var r2 = await userManager.AddClaimAsync(usr, new Claim(ClaimTypes.Role, vm.Role));
                    return RedirectToAction(nameof(Index), new { message = "USER_SAVED" });

                }
                else
                {
                    foreach (var error in res.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return View(vm);
                }
            }
            else
            {
                var user = await userManager.FindByIdAsync(vm.UserId);
                //var user = db.Users.Include(u => u.Claims).FirstOrDefault(u => u.Id == vm.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = vm.FullName;
                user.IsActive = vm.IsActive;

                var res = await userManager.UpdateAsync(user);
                if (!res.Succeeded)
                {
                    foreach (var error in res.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return View(vm);
                }


                // role claim
                var roleClaim = (await userManager.GetClaimsAsync(user)).FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (roleClaim == null || roleClaim.Value != vm.Role)
                {
                    if (roleClaim != null) await userManager.RemoveClaimAsync(user, roleClaim);
                    await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, vm.Role));
                }


                if (!string.IsNullOrWhiteSpace(vm.Password))
                {
                    var passwordToken = await userManager.GeneratePasswordResetTokenAsync(user);
                    var r1 = await userManager.ResetPasswordAsync(user, passwordToken, vm.Password);
                }

                return RedirectToAction(nameof(Index), new { message = "USER_SAVED" });
            }

        }
    }
}
