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
    
    public partial class COA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COA()
        {
            this.AccountSecurity = new HashSet<AccountSecurity>();
            this.CAT = new HashSet<CAT>();
            this.CAT1 = new HashSet<CAT>();
            this.CAT2 = new HashSet<CAT>();
            this.COA1 = new HashSet<COA>();
            this.GJD = new HashSet<GJD>();
            this.Treasuries = new HashSet<Treasuries>();
            this.Treasuries1 = new HashSet<Treasuries>();
        }
    
        public int COAID { get; set; }
        public Nullable<int> COA_ID { get; set; }
        public string COACode { get; set; }
        public string COADescription { get; set; }
        public string COAPlusSide { get; set; }
        public short COASystemPlusSide { get; set; }
        public bool COAStatus { get; set; }
        public string COASystemAccount { get; set; }
        public Nullable<int> COM_ID { get; set; }
        public Nullable<int> COAAccountType { get; set; }
        public string COANotes { get; set; }
        public Nullable<int> ACC_MainType { get; set; }
        public string COA_Type { get; set; }
        public Nullable<decimal> OpenDebit { get; set; }
        public Nullable<decimal> OpenCredit { get; set; }
        public Nullable<decimal> DebitAmount { get; set; }
        public Nullable<decimal> CreditAmount { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccountSecurity> AccountSecurity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAT> CAT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAT> CAT1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAT> CAT2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COA> COA1 { get; set; }
        public virtual COA COA2 { get; set; }
        public virtual COM COM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GJD> GJD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Treasuries> Treasuries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Treasuries> Treasuries1 { get; set; }
    }
}