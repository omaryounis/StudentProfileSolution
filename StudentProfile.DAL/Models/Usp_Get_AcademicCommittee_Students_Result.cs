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
    
    public partial class Usp_Get_AcademicCommittee_Students_Result
    {
        public decimal StudentID { get; set; }
        public string StudentName { get; set; }
        public string NationalID { get; set; }
        public string StutesName { get; set; }
        public string FacultyName { get; set; }
        public string DegreeName { get; set; }
        public decimal StutesCode { get; set; }
        public Nullable<decimal> FacultyCode { get; set; }
        public decimal DegreeCode { get; set; }
        public string EmployeeNote { get; set; }
        public Nullable<System.DateTime> EmployeeDate { get; set; }
        public string EmployeeUser { get; set; }
        public Nullable<System.DateTime> AcademicDate { get; set; }
        public string AcademicUser { get; set; }
        public string AcademicNote { get; set; }
        public Nullable<int> DecisionNumber { get; set; }
        public string DecisionFile { get; set; }
        public Nullable<int> RowLevel { get; set; }
        public Nullable<int> Houres { get; set; }
        public Nullable<bool> IsAccept { get; set; }
    }
}