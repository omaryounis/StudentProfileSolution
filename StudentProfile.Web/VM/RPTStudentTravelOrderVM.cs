using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.Web.VM
{
    public class RPTStudentTravelOrderVM
    {
        public string TRIP_PATH { get; set; }
        public decimal STUDENT_ID { get; set; }
        public string FROM_DATE { get; set; }
        public string END_DATE { get; set; }
        public string STUDENT_NAME { get; set; }
        public string STUDENT_NAME_S { get; set; }
        public string FACULTY_NAME { get; set; }
        public string LEVEL_DESC { get; set; }
        public string NATIONALITY_DESC { get; set; }
        public decimal NATIONALITY_CODE { get; set; }
        public string TripNumber { get; set; }
        public string AgentRefNumber { get; set; }
        public string TicketNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? TicketNumberDate { get; set; }
    }
}