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
    
    public partial class SP_GetRewardDetailsListForSelectedStudents_Result
    {
        public decimal student_id { get; set; }
        public Nullable<int> Allowance_ID { get; set; }
        public decimal Amount { get; set; }
        public Nullable<int> DuesNo { get; set; }
        public int RewardItemType { get; set; }
        public string RewardItemExpensesTypeName { get; set; }
        public string RewardItemName_Arb { get; set; }
        public Nullable<bool> AfterCheckingResult { get; set; }
        public Nullable<int> RateType { get; set; }
        public Nullable<int> ClassesNo { get; set; }
        public decimal FromRate { get; set; }
        public decimal ToRate { get; set; }
    }
}
