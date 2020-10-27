using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models.VM
{
    public class StudentDocumentsVM
    {
        public int Document_ID { get; set; }
        public int StudentDocumentID { get; set; }
        public int Student_ID { get; set; }
        public string IdentityNo { get; set; }
        public string PassportNo { get; set; }
        public int UniversityStudentID { get; set; }
        public int IdentityID { get; set; }
        public int PassportID { get; set; }
        public int VisaID { get; set; }
        public string IdentityNumber { get; set; }
        public DateTime? IdentityExpiryDate { get; set; }
        public string IdentityImageName { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
        public string PassportImageName { get; set; }
        public string VisaNumber { get; set; }
        public DateTime? VisaExpiryDate { get; set; }
        public string VisaImageName { get; set; }
    }
}