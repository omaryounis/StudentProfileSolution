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
    
    public partial class AdvanceApprovedPhases
    {
        public int ID { get; set; }
        public int AdvanceRequested_ID { get; set; }
        public int AdvancePhase_ID { get; set; }
        public int UserID { get; set; }
        public System.DateTime ResponseDate { get; set; }
        public string Reason { get; set; }
        public Nullable<bool> ApprovedStatus { get; set; }
        public Nullable<decimal> ApprovedValue { get; set; }
    
        public virtual AdvancePhases AdvancePhases { get; set; }
        public virtual AdvanceRequests AdvanceRequests { get; set; }
        public virtual DashBoard_Users DashBoard_Users { get; set; }
    }
}
