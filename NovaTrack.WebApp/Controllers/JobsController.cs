using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NovaTrack.WebApp.Data;
using NovaTrack.WebApp.Models;
using NovaTrack.WebApp.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace NovaTrack.WebApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]

    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment hostingEnvironment;
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public JobsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Jobs
        public IActionResult Index(DateTime? from, DateTime? to, string userId, bool excel = false)
        {
            var jobsQuery = _context.Jobs.Include(j => j.User).Where(i => true);

            if (from.HasValue)
            {
                from = from.Value.Date;
                jobsQuery = jobsQuery.Where(j => j.CreateDate.Date >= from);
            }
            if (to.HasValue)
            {
                to = to.Value.Date.AddDays(1);
                jobsQuery = jobsQuery.Where(j => j.CreateDate.Date < to);
            }
            if (!string.IsNullOrWhiteSpace(userId))
            {
                jobsQuery = jobsQuery.Where(j => j.User.Id == userId);
            }

            var jobs = jobsQuery.OrderByDescending(j => j.CreateDate).ToList();

            if (excel)
            {
                var package = CreateExcelPackage(jobs);
                var bytes = package.GetAsByteArray();
                return File(bytes, XlsxContentType, "technicians.xlsx");
            }
            else
            {
                JobsIndexVM vm = new JobsIndexVM
                {
                    From = from,
                    To = to,
                    UserId = string.IsNullOrWhiteSpace(userId) ? null : userId,
                    Jobs = jobs,
                    Technicians = _context.Users.Where(u => u.Claims.Any(c => c.ClaimType == ClaimTypes.Role && c.ClaimValue == Claims.TechnicianRoleValue)).ToList()
                };

                return View(vm);
            }
        }

        private ExcelPackage Excel(List<Job> jobs)
        {
            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.ContentRootPath, "jobstemplate.xlsx"));
            using (var package = new ExcelPackage(fileInfo))
            {
                //int i = 2;
                //foreach (var job in jobs)
                //{
                //    var worksheet = package.Workbook.Worksheets[1];
                //    worksheet.Cells[i, 1].Value = job.Id;
                //    i++;
                //}
                return package;
            }
        }

        private ExcelPackage CreateExcelPackage(List<Job> jobs)
        {
            var package = new ExcelPackage();
            package.Workbook.Properties.Title = "Nova Track Technicians Report";
            package.Workbook.Properties.Author = "Mobile Ap";
            package.Workbook.Properties.Subject = "Nova Track Technicians Report";
            package.Workbook.Properties.Keywords = "Report";


            var worksheet = package.Workbook.Worksheets.Add("Technicians");

            //First add the headers
            worksheet.Cells[1, 1].Value = "Technician";
            worksheet.Cells[1, 2].Value = "Create Date";
            worksheet.Cells[1, 3].Value = "Company Name";
            worksheet.Cells[1, 4].Value = "Premise";
            worksheet.Cells[1, 5].Value = "Premise Other";
            worksheet.Cells[1, 6].Value = "Company Address";
            worksheet.Cells[1, 7].Value = "Company Phone Number";
            worksheet.Cells[1, 8].Value = "Customer Name";
            worksheet.Cells[1, 9].Value = "Asset Barcode";
            worksheet.Cells[1, 10].Value = "Device Barcode";
            worksheet.Cells[1, 11].Value = "Asset Type";
            worksheet.Cells[1, 12].Value = "Device Id";
            worksheet.Cells[1, 13].Value = "Asset Id";
            worksheet.Cells[1, 14].Value = "Latitude";
            worksheet.Cells[1, 15].Value = "Longitude";

            for (var ii = 1; ii <= 15; ii++)
            {
                worksheet.Cells[1, ii].Style.Font.Bold = true;
            }

            worksheet.Cells[1, 1, 1, 15].AutoFitColumns();

            int i = 2;
            foreach (var job in jobs)
            {
                worksheet.Cells[i, 1].Value = job.User.FullName;
                worksheet.Cells[i, 2].Value = job.CreateDate;
                worksheet.Cells[i, 2].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:sss";
                worksheet.Cells[i, 3].Value = job.CompanyName;
                worksheet.Cells[i, 4].Value = job.Premise;
                worksheet.Cells[i, 5].Value = job.OtherPremise;
                worksheet.Cells[i, 6].Value = job.CompanyAddress;
                worksheet.Cells[i, 7].Value = job.CompanyPhoneNumber;
                worksheet.Cells[i, 8].Value = job.CustomerName;
                worksheet.Cells[i, 9].Value = job.AssetBarCode;
                worksheet.Cells[i, 10].Value = job.DeviceBarCode;
                worksheet.Cells[i, 11].Value = job.AssetType;
                worksheet.Cells[i, 12].Value = job.DeviceId;
                worksheet.Cells[i, 13].Value = job.AssetId;
                worksheet.Cells[i, 14].Value = job.Latitude;
                worksheet.Cells[i, 15].Value = job.Longitude;
                i++;
            }

            return package;
        }


        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JobType,CreateDate,CompanyName,Premise,OtherPremise,CompanyAddress,CompanyPhoneNumber,CustomerName,AssetBarCode,DeviceBarCode,AssetType,DeviceId,AssetId")] Job job)
        {
            if (ModelState.IsValid)
            {
                job.Id = Guid.NewGuid();
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,JobType,CreateDate,CompanyName,Premise,OtherPremise,CompanyAddress,CompanyPhoneNumber,CustomerName,AssetBarCode,DeviceBarCode,AssetType,DeviceId,AssetId")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var job = await _context.Jobs.FindAsync(id);
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(Guid id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
