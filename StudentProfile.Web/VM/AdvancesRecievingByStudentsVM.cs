using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.Web.VM
{
    public class AdvancesRecievingByStudentsVM
    {
        public DateTime? AdvanceStartDate { get; set; }
        public DateTime? AdvanceEndDate { get; set; }
        public DateTime? ReturnStartDate { get; set; }
        public DateTime?  ReturnEndDate{ get; set; }
        public string StudentIDs { get; set; }
        public string AdvanceTypeIDs { get; set; }
    }
}