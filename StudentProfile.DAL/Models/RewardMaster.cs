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
    
    public partial class RewardMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RewardMaster()
        {
            this.RewardDetails = new HashSet<RewardDetails>();
        }
    
        public int ID { get; set; }
        public Nullable<int> RewardCategory_ID { get; set; }
        public Nullable<int> Faculties_ID { get; set; }
        public Nullable<int> EducationLevel_ID { get; set; }
        public int StudentStatusAcademyId { get; set; }
        public int UserId { get; set; }
    
        public virtual RewardCategory RewardCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RewardDetails> RewardDetails { get; set; }
    }
}
