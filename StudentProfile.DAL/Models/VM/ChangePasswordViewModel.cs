using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.DAL.Models.VM
{
    public class ChangePasswordViewModel
    {

        [Display(Name = "كلمة المرور القديمة")]
        [Required(ErrorMessage = "كلمة المرور القديمة مطلوبة")]

        [Remote("IsValidOldPassword", "Login",HttpMethod ="Post", ErrorMessage = "كلمة المرور القديمة غير صحيحة")]
        public string OldPassword { get; set; }
        [Display(Name = "كلمة المرور الجديدة")]

        [StringLength(50, ErrorMessage = "كلمة المرور  قصيره جدا", MinimumLength = 6)]

        [Required(ErrorMessage = "من فضلك أدخل كلمة المرور الجديدة")]
        public string NewPassword { get; set; }
        [Display(Name = "تأكيد كلمة المرور")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "كلمة المرور غير مطابقة")]
        public string NewPasswordConfirm { get; set; }
        public int ID { get; set; }


        public string UserName { get; set; }
    }
}