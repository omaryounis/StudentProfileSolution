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
    
    public partial class StTravelRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StTravelRequest()
        {
            this.StudentsTravelOrder = new HashSet<StudentsTravelOrder>();
        }
    
        public int ID { get; set; }
        public int StudentID { get; set; }
        public int TravelClass { get; set; }
        public string FlightsType { get; set; }
        public System.DateTime Departing { get; set; }
        public Nullable<System.DateTime> Returning { get; set; }
        public int TravelPurpose { get; set; }
        public Nullable<int> AdvertisementID { get; set; }
        public Nullable<bool> IsPassportVerified { get; set; }
        public System.DateTime InsertDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> TransportationTrackingID { get; set; }
    
        public virtual TravelAdvertisement TravelAdvertisement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentsTravelOrder> StudentsTravelOrder { get; set; }
        public virtual TransportationTracking TransportationTracking { get; set; }
    }
}