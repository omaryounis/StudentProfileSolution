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
    
    public partial class ViolationOfStudents
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ViolationOfStudents()
        {
            this.ViolationForwords = new HashSet<ViolationForwords>();
            this.ViolationOfStudentsAttachment = new HashSet<ViolationOfStudentsAttachment>();
        }
    
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int IssueId { get; set; }
        public string ViolationDescription { get; set; }
        public System.DateTime ViolationDate { get; set; }
        public System.DateTime InsertDate { get; set; }
        public int SupervisorId { get; set; }
        public Nullable<int> DecisionId { get; set; }
        public string Recommendations { get; set; }
        public string ViolationNumber { get; set; }
        public Nullable<int> DecisionTakerId { get; set; }
        public Nullable<System.DateTime> DecisionDate { get; set; }
        public bool IsRefused { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsForword { get; set; }
        public string FilePath { get; set; }
    
        public virtual Issues Issues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ViolationForwords> ViolationForwords { get; set; }
        public virtual ViolationsDecisions ViolationsDecisions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ViolationOfStudentsAttachment> ViolationOfStudentsAttachment { get; set; }
        public virtual DashBoard_Users DashBoard_Users { get; set; }
        public virtual DashBoard_Users DashBoard_Users1 { get; set; }
    }
}
