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
    
    public partial class TransportationTracking
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TransportationTracking()
        {
            this.StTravelRequest = new HashSet<StTravelRequest>();
        }
    
        public int ID { get; set; }
        public Nullable<int> Nationality_ID { get; set; }
        public string Tracking { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string FlightsType { get; set; }
    
        public virtual Nationality Nationality { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StTravelRequest> StTravelRequest { get; set; }
    }
}
