using NovaTrack.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NovaTrack.WebApp.ViewModels
{
    public class JobsIndexVM
    {
        public List<Job> Jobs { get; set; }
        public List<ApplicationUser> Technicians { get; set; }
        public string UserId { get; set; }
        [DataType(DataType.Date)] public DateTime? From { get; set; }
        [DataType(DataType.Date)] public DateTime? To { get; set; }
    }
}
