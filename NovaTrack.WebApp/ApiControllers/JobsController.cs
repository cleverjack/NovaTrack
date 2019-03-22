using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovaTrack.WebApp.Data;
using NovaTrack.WebApp.Models;

namespace NovaTrack.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class JobsController : BaseController
    {
        public JobsController(ApplicationDbContext db) : base(db)
        {
        }

        [HttpPost]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult Post(List<Job> jobs)
        {
            if (jobs == null)
            {
                return BadRequest();
            }

            if (jobs.Count == 0)
            {
                return NoContent();
            }

            var incomingIds = jobs.Select(j => j.Id).ToList();
            var existingIds = DB.Jobs.Where(j => incomingIds.Contains(j.Id)).Select(j => j.Id).ToList();

            jobs = jobs.Where(j => !existingIds.Contains(j.Id)).Select(j =>
            {
                j.JobType = JobType.Uploaded;
                j.User = this.ApplicationUser;
                return j;
            }).ToList();

            DB.Jobs.AddRange(jobs);
            DB.SaveChanges();
            return NoContent();
        }
    }
}