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
    
    public partial class RowLevel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RowLevel()
        {
            this.StDetails = new HashSet<StDetails>();
        }
    
        public int ID { get; set; }
        public int RowID { get; set; }
        public int LevelID { get; set; }
        public int Gender { get; set; }
        public Nullable<int> JobID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<bool> IsBrotherDicount { get; set; }
        public Nullable<int> MaxCount { get; set; }
        public Nullable<int> RLOrder { get; set; }
        public Nullable<int> NextRowID { get; set; }
        public Nullable<int> NextLevelID { get; set; }
        public Nullable<int> Com_ID { get; set; }
        public Nullable<int> TempID { get; set; }
        public Nullable<System.DateTime> RegisterDate { get; set; }
        public Nullable<int> StudyTypeID { get; set; }
    
        public virtual Row Row { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StDetails> StDetails { get; set; }
    }
}