//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudentProfile.DAL.Models
{
    using System;
    
    public partial class Usp_GetAdvancesPaymentByUsers_Result
    {
        public string ResponsibleName { get; set; }
        public string STUDENT_NAME { get; set; }
        public int AdvanceTypeId { get; set; }
        public string AdvanceSettingName { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string DocNumber { get; set; }
        public decimal Student_ID { get; set; }
        public string NATIONAL_ID { get; set; }
        public decimal TotalAdvanceValue { get; set; }
        public Nullable<decimal> MonthlyValue { get; set; }
    }
}
