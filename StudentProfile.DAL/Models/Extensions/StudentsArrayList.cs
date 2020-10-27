using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models
{
    public class StudentsAlertList
    {
        public long StudentId { get; set; }
        public string IdNumber { get; set; }
        public string StudentName { get; set; }

        public string IdentityNum { get; set; }

        public DateTime? IdentityExpireDate { get; set; }
        public int IdentityId { get; set; }


    }
}