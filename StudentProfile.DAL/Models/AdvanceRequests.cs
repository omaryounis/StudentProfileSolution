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
    
    public partial class AdvanceRequests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdvanceRequests()
        {
            this.AdvanceApprovedPhases = new HashSet<AdvanceApprovedPhases>();
            this.AdvancePaymentDetails = new HashSet<AdvancePaymentDetails>();
            this.AdvanceRequestsAttachment = new HashSet<AdvanceRequestsAttachment>();
        }
    
        public int ID { get; set; }
        public int Student_Id { get; set; }
        public int AdvanceSettings_Id { get; set; }
        public Nullable<decimal> RequestedValue { get; set; }
        public Nullable<decimal> ApprovedValue { get; set; }
        public string StatusNotes { get; set; }
        public string RequestNotes { get; set; }
        public Nullable<int> RefusedbyID { get; set; }
        public Nullable<int> ApprovedbyID { get; set; }
        public Nullable<bool> IsCanceled { get; set; }
        public Nullable<int> User_Id { get; set; }
        public System.DateTime InsertionDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdvanceApprovedPhases> AdvanceApprovedPhases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdvancePaymentDetails> AdvancePaymentDetails { get; set; }
        public virtual AdvanceSettings AdvanceSettings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdvanceRequestsAttachment> AdvanceRequestsAttachment { get; set; }
        public virtual DashBoard_Users DashBoard_Users { get; set; }
    }
}
