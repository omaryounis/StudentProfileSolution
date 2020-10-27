using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models.VM
{
    public class ResidentReportVM
    {
        public int IdNumber { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Type { get; set; }
        public bool IsValid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime HijriStartDate { get; set; }
        public DateTime HijriEndDate { get; set; }
        public string Hash { get; set; }
    }

}