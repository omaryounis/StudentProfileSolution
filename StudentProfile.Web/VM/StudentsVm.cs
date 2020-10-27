using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.Web.VM
{
    public class StudentsVm
    {
        public string degreeIds { get; set; }
        public string facultyIds { get; set; }
        public string DepartmentIds { get; set; }
        public string MajorIds { get; set; }
        public string StatusIds { get; set; }
        public string StudyIds { get; set; }
        public string StudentID { get; set; }
    }
    public class TravelOrdersVM
    {
        public string AgentRefNumber1 { get; set; }
        public string Nationality { get; set; }
        public string TripType { get; set; }
        public int ID { get; set; }
        public string TravelType { get; set; }
        public string TripNumber { get; set; }
        public string TripPath { get; set; }
        public DateTime CreationDate { get; set; }
        public int AgentRefNumber { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string Notes { get; set; }
        public bool IsReturned { get; set; }
        public int StudentsCount { get; set; }
        public DateTime TravelOrderDate { get; set; }
        public decimal? SumGivenAmount { get; set; }
        public bool? IsCanceled { get; set; }
        public int StudentCount { get; set; }
    }

}