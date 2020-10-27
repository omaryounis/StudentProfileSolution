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
    
    public partial class HousingFurnitures
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HousingFurnitures()
        {
            this.HousingFurnituresOfStudent = new HashSet<HousingFurnituresOfStudent>();
            this.HousingLocationFurniture = new HashSet<HousingLocationFurniture>();
        }
    
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int UserId { get; set; }
        public System.DateTime InsertDate { get; set; }
        public Nullable<int> LastEdittedBy { get; set; }
        public Nullable<System.DateTime> LastEdittingDate { get; set; }
        public bool IsBed { get; set; }
    
        public virtual HousingFurnitureCategories HousingFurnitureCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HousingFurnituresOfStudent> HousingFurnituresOfStudent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HousingLocationFurniture> HousingLocationFurniture { get; set; }
    }
}
