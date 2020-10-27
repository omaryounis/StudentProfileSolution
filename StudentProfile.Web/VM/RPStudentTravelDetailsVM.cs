using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.Web.VM
{
    public class RPStudentTravelDetailsVM
    {
       
            public decimal STUDENT_ID { get; set; }
            public string STUDENT_NAME { get; set; }
        public string TripNumber { get; set; }
        public decimal DEGREE_CODE { get; set; }
            public string DEGREE_DESC { get; set; }
            public decimal NATIONALITY_CODE { get; set; }
            public string NATIONALITY_DESC { get; set; }
            public decimal? FACULTY_NO { get; set; }
            public string FACULTY_NAME { get; set; }
            public string MobileNumber { get; set; }
            public string UniversityEmail { get; set; }
            public string TripPath { get; set; }
            public string FromDate { get; set; }
            public string ToDate { get; set; }

        }
    
}