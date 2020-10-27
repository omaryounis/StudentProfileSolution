using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProfile.DAL.Models.VM
{
   public  class PayrollModel
    {
        public int ID { get; set; }
        public string InsertDate { get; set; }
        public string IssueDate { get; set; }
        public string PayrollNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? AdvancesValues { get; set; }
        public decimal? RealValues { get; set; }
        public int StudentsCount { get; set; }
        public bool? IsFinancialApprove { get; set; }
        public bool? IssuingExchangeOrder { get; set; }
        public bool? IssuingPaymentOrder { get; set; }
        public string DafPayNo { get; set; }
        public string DafPayNo2 { get; set; }
        public string PayNo { get; set; }
        public string MinistrVal { get; set; }
    }
}
