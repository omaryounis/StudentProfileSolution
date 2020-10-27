using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models
{
    [MetadataType(typeof(RelativesMD))]
    public partial class Relatives
    {
    }

    public class RelativesMD
    {
        public int ID { get; set; }
        public int? EmpID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [Display(Name = "صلة القرابة")]
        public string Relationship { get; set; }

        public string IDNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [Display(Name = "رقم جواز السفر")]
        public string PassportNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "حقل اجبارى")]
        [Display(Name = "تاريخ انتهاء جواز السفر")]
        public DateTime? PassExpDate { get; set; }

        public string PassImage { get; set; }
    }
}