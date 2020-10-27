using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.Web.VM
{
    public class PayrollCheckVM
    {
        public int id { get; set; }
        public string faculty_name { get; set; }
        public string DafPayNumber { get; set; }
        public string CheckNumber { get; set; }
        public string ExportDate { get; set; }
        public string ReceiveDate { get; set; }
        public string UserName { get; set; }
        public string FilePath { get; set; }
    }
}