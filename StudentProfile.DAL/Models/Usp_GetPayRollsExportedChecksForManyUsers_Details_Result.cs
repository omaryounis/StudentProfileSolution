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
    
    public partial class Usp_GetPayRollsExportedChecksForManyUsers_Details_Result
    {
        public int PayrollID { get; set; }
        public string PayrollNumber { get; set; }
        public string DafPayNumber { get; set; }
        public string PayNumber { get; set; }
        public Nullable<int> StudentNumber { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> PayrollMoneyValue { get; set; }
        public int UserID { get; set; }
        public int ID { get; set; }
        public Nullable<decimal> checkvalue { get; set; }
        public string CheckNumber { get; set; }
        public string Exportdate { get; set; }
        public string Description { get; set; }
        public string Filepath { get; set; }
    }
}
