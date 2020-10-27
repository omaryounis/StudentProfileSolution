using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProfile.DAL.Models.VM
{
    public class GetAdvancesRecievingVM
    {
        public string STUDENT_NAME { get; set; }
        public int AdvanceTypeId { get; set; }
        public string AdvanceSettingName { get; set; }
        public Nullable<System.DateTime> InsertedDate { get; set; }
        public string DocNumber { get; set; }
        public decimal Student_ID { get; set; }
        public string NATIONAL_ID { get; set; }
        public decimal TotalAdvanceValue { get; set; }
        public decimal ReturnedValue { get; set; }
        public Nullable<decimal> RemainingValue { get; set; }
        public string ReturnMethod { get; set; }
        public string PayRollNumber { get; set; }
    }
}
