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
    
    public partial class GJH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GJH()
        {
            this.CashMovement = new HashSet<CashMovement>();
            this.FSY = new HashSet<FSY>();
            this.GJD = new HashSet<GJD>();
        }
    
        public long GJHID { get; set; }
        public int JournalNo { get; set; }
        public Nullable<int> COM_ID { get; set; }
        public System.DateTime GJHOperationDate { get; set; }
        public Nullable<System.DateTime> GJHPostedDate { get; set; }
        public string GJHDescription { get; set; }
        public decimal GJHAmount { get; set; }
        public int OPT_ID { get; set; }
        public string GJHSourceType { get; set; }
        public Nullable<int> GJHSource_ID { get; set; }
        public string GJHSourceNumber { get; set; }
        public int USR_ID { get; set; }
        public Nullable<int> GJHSubAccount_ID { get; set; }
        public string GJHSubAccountType { get; set; }
        public string GJHRefranceNumber { get; set; }
        public string GJHSystemNumber { get; set; }
        public Nullable<decimal> GJHPreviousBalance { get; set; }
        public Nullable<bool> GJHStatus { get; set; }
        public string ISONumber { get; set; }
        public System.DateTime InsertDate { get; set; }
    
        public virtual BCD BCD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CashMovement> CashMovement { get; set; }
        public virtual COM COM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FSY> FSY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GJD> GJD { get; set; }
        public virtual Users Users { get; set; }
    }
}