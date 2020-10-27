using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models.VM
{
    public class LoginViewModel
    {
        [Display(Name = "اسم المستخدم")]
        [Required(ErrorMessage = "من فضلك أدخل اسم المستخدم الخاص بك")]
        public string Username { get; set; }
        [Display(Name = "كلمة المرور")]
        [Required(ErrorMessage = "من فضلك أدخل كلمة المرور الخاصة بك")]
        public string Password { get; set; }


        public int ID { get; set; }

    }
}