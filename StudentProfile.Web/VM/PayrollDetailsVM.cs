using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.Web.VM
{
    public class PayrollDetailsVM
    {
        public int StudentID { get; set; }
        public int NationalID { get; set; }
        public string StudentName { get; set; }
        public string StudentNameEn { get; set; }
        public string MobileNumber { get; set; }
        public string MobilePhone { get; set; }
        public string AccountNumber { get; set; }
        public string FacultyName { get; set; }
        public string StatusName { get; set; }
        public string LevelName { get; set; }
        public string DegreeName { get; set; }
        public string PayrollNumber { get; set; }
        public int YearNumber { get; set; }
        public int MonthNumber { get; set; }
        public int DaysCount { get; set; }
        public List<string> bdlat { get; set; }
        public List<string> estqta3at { get; set; }
        public double cutsvalue { get; set; }
        public double duesvalue { get; set; }
        public double finalvalue { get; set; }
    }
}