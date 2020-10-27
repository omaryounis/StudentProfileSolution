using System;

namespace StudentProfile.DAL.Models
{
    public class MyDataTable
    {
        public string IdNumber { get; set; }
        public int DocumentId { get; set; }
        public string IdentityNum { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpDate { get; set; }
        public bool IsActive { get; set; }
    }
}