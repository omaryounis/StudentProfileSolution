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
    
    public partial class AirPort
    {
        public int ID { get; set; }
        public string AirPortNameAr { get; set; }
        public string AirPortNameEn { get; set; }
        public int CountryID { get; set; }
        public string Code { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CName { get; set; }
        public string Woeid { get; set; }
        public string Tz { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string Runway_length { get; set; }
        public string Elev { get; set; }
        public string Icao { get; set; }
        public string Direct_flights { get; set; }
        public string Carriers { get; set; }
    
        public virtual Country Country { get; set; }
    }
}
