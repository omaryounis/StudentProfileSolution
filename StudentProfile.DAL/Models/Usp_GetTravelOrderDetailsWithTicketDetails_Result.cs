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
    
    public partial class Usp_GetTravelOrderDetailsWithTicketDetails_Result
    {
        public int TravelOrderID { get; set; }
        public int ID { get; set; }
        public int StudentID { get; set; }
        public string ConcatinatedTicketNumbers { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public string TicketNumber { get; set; }
        public Nullable<decimal> TicketValue { get; set; }
        public string Student_Name { get; set; }
        public string National_ID { get; set; }
        public string NATIONALITY_DESC { get; set; }
        public string TripNumber { get; set; }
        public string TripPath { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public Nullable<decimal> GivenAmount { get; set; }
        public Nullable<int> StudentTicketDetailID { get; set; }
    }
}