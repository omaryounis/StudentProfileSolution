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
    
    public partial class vw_GetAllRewards
    {
        public string FacultyName { get; set; }
        public string EducationLevelName { get; set; }
        public string EducationTypeName { get; set; }
        public string RegisterTypeName { get; set; }
        public int RewardDetailsId { get; set; }
        public Nullable<int> RewardMasterId { get; set; }
        public int HoursNo { get; set; }
        public Nullable<int> ClassesNo { get; set; }
        public Nullable<int> DuesNo { get; set; }
        public Nullable<bool> DependOnEducationDays { get; set; }
        public Nullable<bool> DependOnEducationPeriod { get; set; }
        public Nullable<decimal> MiniHoursNo { get; set; }
        public decimal FromRate { get; set; }
        public decimal ToRate { get; set; }
        public Nullable<int> AmountType { get; set; }
        public decimal Amount { get; set; }
        public Nullable<int> RateType { get; set; }
        public int RewardCategoryId { get; set; }
        public string RewardCategoryArbName { get; set; }
        public string RewardItemName_Arb { get; set; }
    }
}