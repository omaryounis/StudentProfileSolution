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
    using System.Collections.Generic;
    
    public partial class AcademicCommittee
    {
        public int Id { get; set; }
        public decimal StudentId { get; set; }
        public string EmployeeNote { get; set; }
        public string AcademicNote { get; set; }
        public Nullable<bool> IsAccept { get; set; }
        public Nullable<System.DateTime> EmployeeDate { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<System.DateTime> AcademicDate { get; set; }
        public Nullable<int> AcademicId { get; set; }
        public Nullable<int> DecisionNumber { get; set; }
        public string DecisionFile { get; set; }
    }
}
