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
    
    public partial class usp_School_Select_Result
    {
        public int ID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public byte[] Logo { get; set; }
        public string Fax { get; set; }
        public Nullable<bool> Direct_Post { get; set; }
        public string SmsSender { get; set; }
        public string SmsUsername { get; set; }
        public string SmsPassword { get; set; }
        public string Email { get; set; }
        public Nullable<bool> IsRegstAdjust { get; set; }
        public Nullable<int> Com_ID { get; set; }
        public Nullable<bool> IsOutAdjust { get; set; }
        public string OutMethod { get; set; }
        public string RegForm { get; set; }
        public Nullable<int> F_NumMax { get; set; }
        public Nullable<int> S_NumMax { get; set; }
        public Nullable<System.DateTime> FirstYear_DOB { get; set; }
        public string DocHeader { get; set; }
        public string DocFooter { get; set; }
    }
}
