using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StudentProfile.DAL.Models.DataAnnotations;

namespace StudentProfile.DAL.Models
{
    [MetadataType(typeof(EmpDocumentMD))]
    public partial class EmpDocument
    {
    }

    class EmpDocumentMD
    {
        public int ID { get; set; }
        public int EmpID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [Display(Name = "نوع المستند")]
        public int? DocumentID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [Display(Name = "رقم المستند")]
        public string IdentityNum { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [Display(Name = "صورة المستند")]
        public string DocImagePath { get; set; }

        public string BarCode { get; set; }

        [StartEndDateValidator]
        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [DataType(DataType.Date, ErrorMessage = "تاريخ خاطئ")]
        [Display(Name = "تاريخ الاصدار")]
        public DateTime? IssueDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [DataType(DataType.Date, ErrorMessage = "تاريخ خاطئ")]
        [Display(Name = "تاريخ الانتهاء")]
        public DateTime? ExpDate { get; set; }

        public bool? IsActive { get; set; }
    }
}