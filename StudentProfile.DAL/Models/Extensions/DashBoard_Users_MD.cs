using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace StudentProfile.DAL.Models
{
   // [MetadataType(typeof(DashBoard_Users_MD))]
    public partial class DashBoard_Users
    {
        public bool IsStudent { get; set; }

    }
    class DashBoard_Users_MD
    {
        public int ID { get; set; }
        [Display(Name = "اسم المستخدم")]
        [Required(ErrorMessage = "من فضلك أدخل اسم المستخدم الخاص بك")]
        public string Username { get; set; }
        [Display(Name = "كلمة المرور")]
        [Required(ErrorMessage = "من فضلك أدخل كلمة المرور الخاصة بك")]
        public string Password { get; set; }
        [Required(ErrorMessage = "من فضلك أدخل رقم الجوال الخاص بك")]
        [Display(Name = "رقم الجوال")]
        [DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "من فضلك أدخل رقم جوال متاح")]
        public string Mobile { get; set; }
        public bool? IsAdmin { get; set; }

       

    }

    //public partial class DbContext
    //{
    //    public DbContext()
    //    {
    //        var objectContext = (this as IObjectContextAdapter).ObjectContext;
    //        objectContext.CommandTimeout = 0;
    //    }
    //    protected virtual void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        throw new UnintentionalCodeFirstException();
    //    }
    //}
}