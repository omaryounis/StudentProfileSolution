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
    
    public partial class proc_GetStudentGrades_Result
    {
        public decimal STUDENT_ID { get; set; }
        public string STUDENT_NAME { get; set; }
        public string Nationality_Desc { get; set; }
        public string FACULTY_NAME { get; set; }
        public string STATUS_DESC { get; set; }
        public decimal SEMESTER { get; set; }
        public string COURSE_CODE { get; set; }
        public string COURSE_NAME { get; set; }
        public string COURSE_NAME_S { get; set; }
        public decimal CRD_HRS { get; set; }
        public string LETTER_GRADE { get; set; }
        public string LETTER_GRADE_S { get; set; }
        public Nullable<decimal> CONFIRMED_MARK { get; set; }
        public Nullable<decimal> QUALITY_POINTS { get; set; }
        public Nullable<double> POINTS { get; set; }
        public Nullable<decimal> MAJOR_NO { get; set; }
        public string MAJOR_NAME { get; set; }
        public string STUDY_DESC { get; set; }
        public Nullable<decimal> SEMESTER_GPA { get; set; }
        public Nullable<decimal> CUM_GPA { get; set; }
    }
}