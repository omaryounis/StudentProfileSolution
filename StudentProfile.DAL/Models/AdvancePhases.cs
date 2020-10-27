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
    
    public partial class AdvancePhases
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdvancePhases()
        {
            this.AdvanceApprovedPhases = new HashSet<AdvanceApprovedPhases>();
            this.AdvanceUsers = new HashSet<AdvanceUsers>();
        }
    
        public int ID { get; set; }
        public string PhaseName { get; set; }
        public byte Order { get; set; }
        public Nullable<bool> IsFinancialPhase { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdvanceApprovedPhases> AdvanceApprovedPhases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdvanceUsers> AdvanceUsers { get; set; }
    }
}