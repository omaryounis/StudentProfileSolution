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
    
    public partial class PR_StudentsCourses_SelectAll_Result
    {
        public decimal Student_ID { get; set; }
        public string CourseName { get; set; }
        public string CourseType_Name { get; set; }
        public string Student_Name { get; set; }
        public Nullable<int> CourseDays { get; set; }
        public string Degree { get; set; }
        public Nullable<int> DegreePercentage { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}
