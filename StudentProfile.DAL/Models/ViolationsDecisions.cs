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
    
    public partial class ViolationsDecisions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ViolationsDecisions()
        {
            this.ViolationOfStudents = new HashSet<ViolationOfStudents>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int InsertedBy { get; set; }
        public System.DateTime InsertDate { get; set; }
        public Nullable<int> LastEdittedBy { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ViolationOfStudents> ViolationOfStudents { get; set; }
        public virtual DashBoard_Users DashBoard_Users { get; set; }
        public virtual DashBoard_Users DashBoard_Users1 { get; set; }
    }
}
