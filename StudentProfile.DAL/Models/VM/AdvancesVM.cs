using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models.VM
{
    public class AdvancesVM
    {
        public string Name { get; set; }
        public string date { get; set; }
        public decimal? AdvanceValue { get; set; }
        public decimal? Paid { get; set; }
        public decimal? reset { get; set; }
        public int StudentId { get; set; }
        public string AdvanceType { get; set; }
        public IEnumerable<string> Attachements { get; set; }
    }
}