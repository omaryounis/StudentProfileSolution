using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProfile.Web.VM
{
    public class StudentRewardsVM
    {
        public string DelevirFileName { get; set; }
        public string DEGREE_DESC { get; set; }
        public string DeleviryPath { get; set; }
        public string DeliveryDate { get; set; }
        public string FACULTY_NAME { get; set; }
        public string IsPaid { get; set; }
        public Nullable<decimal> PayrollMoneyValue { get; set; }
        public string PayrollNumber { get; set; }
        public string STUDENT_NAME { get; set; }
        public string UserName { get; set; }
        public decimal student_id { get; set; }
    }
}
