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
    
    public partial class AdvancesPremiums
    {
        public int ID { get; set; }
        public int Advances_Id { get; set; }
        public decimal NetValue { get; set; }
        public System.DateTime PayrollDate { get; set; }
        public string PayRollNumber { get; set; }
        public string PaymentType { get; set; }
        public int User_Id { get; set; }
        public Nullable<int> COA_ID { get; set; }
        public Nullable<int> GJH_ID { get; set; }
        public Nullable<bool> IsPosted { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> InsertionDate { get; set; }
    }
}
