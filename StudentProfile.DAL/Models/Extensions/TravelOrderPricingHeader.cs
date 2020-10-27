using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models
{
    public class TravelOrderHeader
    {
        public int TripGroupID { get; set; }
        public string TripPath { get; set; }
        public int TripPathCount { get; set; }
        public int TripStudentsCount { get; set; }
        public decimal? AmountPerTripPath { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}