using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard_StudentProfile.VM
{
    public class RptGetAllTravelStudentVM
   {
        //public int? StudentID { get; set; }
        //public DateTime Departing { get; set; }
        //public DateTime Returning { get; set; }
        //public string NamePerIdentity_Ar { get; set; }
        //public string NamePerIdentity_En { get; set; }
        //public string Country { get; set; }
        //public string Level { get; set; }
        //public string Faculty { get; set; }

        public decimal REQUEST_SEQ { get; set; }
        public decimal STUDENT_ID { get; set; }
        public decimal? TRAVEL_LINE_NO { get; set; }
        public string TRAVEL_LINE_DESC { get; set; }
        public string FROM_DATE { get; set; }
        public string END_DATE { get; set; }
        public string LAST_UPDATE_DATE { get; set; }
        public string STUDENT_NAME { get; set; }
        public string STUDENT_NAME_S { get; set; }
        public string FACULTY_NAME { get; set; }
        public string LEVEL_DESC { get; set; }
        public string NATIONALITY_DESC { get; set; }
        public decimal NATIONALITY_CODE { get; set; }
    }
}