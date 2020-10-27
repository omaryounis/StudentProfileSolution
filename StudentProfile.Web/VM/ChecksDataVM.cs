using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProfile.Web.VM
{
    public class ChecksDataVM
    {
        public string CheckNumber { get; set; }
        public string BenefName { get; set; }
        public string ExportDate { get; set; }
        public decimal? CheckValue { get; set; }
        public string IsActive { get; set; }
        public string IsReceived { get; set; }
        public string ReceiveDate { get; set; }
        public string PayrollNumber { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string SandFileName { get; set; }
        public string SandFilePath { get; set; }
    }
}
