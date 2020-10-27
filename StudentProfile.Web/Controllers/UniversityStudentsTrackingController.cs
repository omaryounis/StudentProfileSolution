
using StudentProfile.DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DevExpress.Data.ODataLinq.Helpers;
using System.Text;
using StudentProfile.Components;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    //[StudentProfile.Components.CustomAuthorizeHelper]
    public class UniversityStudentsTrackingController : Controller
    {
        #region UniversityStudentsTracking

        private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();


          public class Permissions
    {
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Save { get; set; }
    }

         
           
        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(1012);
            ViewBag.Read = permissions.Read;

            if (permissions.Read)
            {
                return View();
            }

            return RedirectToAction("NotAuthorized");
        }

        public DashBoard_Users CurrentUser
        {
            get
            {
                var user = HttpContext.Session["UserId"] as DashBoard_Users;
                if (user != null && user.ID != 0)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        public Permissions GetPermissionsFn(int screenId)
        {
            if (CurrentUser == null || CurrentUser?.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new Permissions();
            foreach (var permission in perm)
            {
                if (permission == "اضافة")
                {
                    permissions.Create = true;
                }
                else if (permission == "قراءة")
                {
                    permissions.Read = true;
                }
                else if (permission == "تعديل")
                {
                    permissions.Update = true;
                }
                else if (permission == "حذف")
                {
                    permissions.Delete = true;
                }
                else if (permission == "عرض")
                {
                    permissions.View = true;
                }
                else if (permission == "حفظ")
                {
                    permissions.Save = true;
                }
            }

            return permissions;
        }

        //[CustomAuthorizeHelper(ScreenId = 14)]
        public ActionResult GetUniversityStudents()
        {
            var UniversityStudents = db.UniversityStudents.Select(x => new
            {
                x.ID,
                x.Nationality.NationalityName,
                x.MobileNumber,
                x.Student_ID,
                x.NamePerIdentity_Ar,
                x.National_ID,
                x.PersonalEmail,
                x.InsertDate
            }).OrderBy(x=>x.National_ID);
            return this.JsonMaxLength(UniversityStudents.ToList(), JsonRequestBehavior.AllowGet);
        }

         
        [HttpPost]
        public ActionResult DeleteStudent(int UniversityStudentID)
        {
            try
            {
                var obj = db.UniversityStudents.Find(UniversityStudentID);
                if (obj == null) return Content("عفوا لا يوجد طالب بهذه البيانات");
                db.StudentDocuments.RemoveRange(db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudentID));
                db.UniversityStudents.Remove(obj);
                db.SaveChanges();
                return Content("");
            }
            catch (DbUpdateException e)
            {
                return Content("يوجد خطأ");
            }
        } 

        public ActionResult NotAuthorized()
        {
            return View();
        }

        #endregion

         
    }

}