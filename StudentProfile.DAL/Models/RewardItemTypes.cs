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
    
    public partial class RewardItemTypes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RewardItemTypes()
        {
            this.RewardItems = new HashSet<RewardItems>();
        }
    
        public int ID { get; set; }
        public int RewardItemType { get; set; }
        public string RewardItemTypeName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RewardItems> RewardItems { get; set; }
    }
}
