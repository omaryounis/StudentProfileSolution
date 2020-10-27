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
    
    public partial class TravelAdvertisement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TravelAdvertisement()
        {
            this.StTravelRequest = new HashSet<StTravelRequest>();
        }
    
        public int ID { get; set; }
        public string AdName { get; set; }
        public int TravelClass { get; set; }
        public string flightsType { get; set; }
        public int AgentID { get; set; }
        public int PurposeID { get; set; }
        public System.DateTime AdStartDate { get; set; }
        public System.DateTime AdEndDate { get; set; }
        public System.DateTime DepartingStart { get; set; }
        public System.DateTime DepartingEnd { get; set; }
        public Nullable<System.DateTime> ReturningStart { get; set; }
        public Nullable<System.DateTime> ReturningEnd { get; set; }
        public string NationalityID { get; set; }
        public string DegreeID { get; set; }
        public string StStatusID { get; set; }
        public string ScholarshipType { get; set; }
        public int UserID { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsAppearToExpectedGradutedPeople { get; set; }
        public string CustomFieldId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StTravelRequest> StTravelRequest { get; set; }
        public virtual DashBoard_Users DashBoard_Users { get; set; }
    }
}