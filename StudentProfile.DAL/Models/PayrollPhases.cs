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
    
    public partial class PayrollPhases
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PayrollPhases()
        {
            this.PayrollPhasesUsers = new HashSet<PayrollPhasesUsers>();
        }
    
        public int ID { get; set; }
        public string PhaseName { get; set; }
        public int PhaseOrder { get; set; }
        public Nullable<bool> IsFinancialApprove { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IssuingPaymentOrder { get; set; }
        public Nullable<bool> IssuingExchangeOrder { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PayrollPhasesUsers> PayrollPhasesUsers { get; set; }
    }
}