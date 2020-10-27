using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models.DataAnnotations
{
    public class ACADEMICAPI
    {
        public decimal STUDENT_ID { get; set; }
        public decimal? REMAIN_HOURS { get; set; }
        public decimal? MIN_SEMESTER { get; set; }
        public decimal? PASSED_SEMESTER { get; set; }

        public DateTime? GRADUATE_DATE { get; set; }

        public bool? IS_SCHOLORSHIP { get; set; }

        public string SPONSOR_TYPE { get; set; }
    }

    public class ACADEMICStatus
    {
        public decimal STUDENT_ID { get; set; }
        public string Student_Name { get; set; }
        public string Degree_Desc { get; set; }
        public DateTime? GRADUATE_DATE { get; set; }

        public string Status_Desc { get; set; }
    }
}