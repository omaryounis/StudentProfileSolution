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
    
    public partial class proc_GetAdvancesReport_ForEmp_Result
    {
        public int AdvID { get; set; }
        public string AdvName { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> EmpID { get; set; }
        public string EmpName { get; set; }
        public Nullable<decimal> MonthlyPremium { get; set; }
        public string MonthsNumbers { get; set; }
        public Nullable<decimal> AdvanceValue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Rest { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public string AdvancesState { get; set; }
    }
}
