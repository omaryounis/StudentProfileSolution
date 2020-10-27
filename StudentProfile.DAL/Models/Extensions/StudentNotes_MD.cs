using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models
{
    [MetadataType(typeof(StudentNotes_MD))]
    public partial class StudentNotes
    {
    }
    class StudentNotes_MD
    {

        public int NoteId { get; set; }
        public int StDetailId { get; set; }
        [Required(ErrorMessage = "من فضلك أدخل تفاصيل الملاحظة")]
        public string NoteDetails { get; set; }
        public bool? IsSecret { get; set; }
        [Required(ErrorMessage = "من فضلك أدخل اختر نوع الملاحظة")]
        public Nullable<int> IssueId { get; set; }
        [Required(ErrorMessage = "من فضلك أدخل تاريخ الملاحظة")]
        public Nullable<System.DateTime> NoteDate { get; set; }
    }
}