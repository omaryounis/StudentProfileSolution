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
    
    public partial class usp_get_not_take_des_academic_students_Result
    {
        public int Id { get; set; }
        public Nullable<decimal> faculty_no { get; set; }
        public decimal DEGREE_CODE { get; set; }
        public System.DateTime join_date { get; set; }
        public decimal STUDENT_ID { get; set; }
        public string STUDENT_NAME { get; set; }
        public string NATIONAL_ID { get; set; }
        public string STATUS_DESC { get; set; }
        public string FACULTY_NAME { get; set; }
        public string DEGREE_DESC { get; set; }
        public Nullable<int> REMAININGCREDITHOURSCOUNT { get; set; }
        public Nullable<int> JOIN_SEMESTER { get; set; }
        public Nullable<int> TOTALSEMESTER { get; set; }
        public string NATIONALITY_DESC { get; set; }
        public string CATEGORY_NAME { get; set; }
        public Nullable<int> TOTAL_WARNINGS { get; set; }
        public string REASON_NAME { get; set; }
        public string STUDY_DESC { get; set; }
        public Nullable<int> TOTAL_Apology { get; set; }
        public Nullable<int> TOTAL_Separated { get; set; }
        public Nullable<decimal> Expected { get; set; }
        public Nullable<double> cum_gpa { get; set; }
        public string EmployeeNote { get; set; }
        public string EmployeeDate { get; set; }
        public string AcademicNote { get; set; }
        public Nullable<bool> IsAccept { get; set; }
    }
}
