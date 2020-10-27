using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using StudentProfile.DAL.Models;
using DevExpress.Web;
using StudentProfile.Components;

namespace StudentProfile.Controllers
{
   [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class ConfigController : Controller
    {
        private EsolERPEntities dbAcc = new EsolERPEntities();
        private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();

        //private HRMadinaEntities dbHR = new HRMadinaEntities();
        //private iu_projectEntities db_iu = new iu_projectEntities();

        public JsonResult GetPermissions(int screenId)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            if (user != null)
            {

                if (user.ID == 0)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }


            var perm = CheckPermissions.IsAuthorized(user.ID, screenId);

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

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        public class CustomFieldsPermissions
        {
            public bool CreateParent { get; set; }
            public bool CreateChild { get; set; }
            public bool Read { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }
            public bool View { get; set; }
            public bool Save { get; set; }
        }

        public JsonResult GetCustomFieldsPermissions(int screenId)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            if (user!=null)
            {
               
                if (user.ID == 0)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }


            var perm = CheckPermissions.IsAuthorized(user.ID, screenId);

            var permissions = new CustomFieldsPermissions();
            foreach (var permission in perm)
            {
                if (permission == "قراءة")
                {
                    permissions.View = true;
                }
                else if (permission == "اضافة حقل مخصص رئيسي")
                {
                    permissions.CreateParent = true;
                }
                else if (permission == "اضافة حقل مخصص فرعي")
                {
                    permissions.CreateChild = true;
                }
                else if (permission == "تعديل")
                {
                    permissions.Update = true;
                }
                else if (permission == "حذف")
                {
                    permissions.Delete = true;
                }
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }
        public CustomFieldsPermissions GetCustomFieldsPermissionsFn(int screenId)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            if (user != null)
            {

                if (user.ID == 0)
                {
                    return null;
                }
            }


            var perm = CheckPermissions.IsAuthorized(user.ID, screenId);

            var permissions = new CustomFieldsPermissions();
            foreach (var permission in perm)
            {
                if (permission == "قراءة")
                {
                    permissions.View = true;
                }
                else if (permission == "اضافة حقل مخصص رئيسي")
                {
                    permissions.CreateParent = true;
                }
                else if (permission == "اضافة حقل مخصص فرعي")
                {
                    permissions.CreateChild = true;
                }
                else if (permission == "تعديل")
                {
                    permissions.Update = true;
                }
                else if (permission == "حذف")
                {
                    permissions.Delete = true;
                }
            }

            return permissions;
        }



        public Permissions GetPermissionsFn(int screenId)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            if (user != null)
            {

                if (user.ID == 0)
                {
                    return null;
                }
            }


            var perm = CheckPermissions.IsAuthorized(user.ID, screenId);

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







        #region SMS Config
        public ActionResult Semesters()
        {
            ViewBag.FirstSemesterStartDate = db.config.SingleOrDefault(x => x.Kay == "FirstSemesterStartDate")?.Value;
            ViewBag.FirstSemesterEndDate = db.config.SingleOrDefault(x => x.Kay == "FirstSemesterEndDate")?.Value;
            ViewBag.SecondSemesterStartDate = db.config.SingleOrDefault(x => x.Kay == "SecondSemesterStartDate")?.Value;
            ViewBag.SecondSemesterEndDate = db.config.SingleOrDefault(x => x.Kay == "SecondSemesterEndDate")?.Value;
            return View();
        }
        [HttpPost]
        public ActionResult SemestersPost(string FirstSemesterStartDate, string FirstSemesterEndDate, string SecondSemesterStartDate, string SecondSemesterEndDate)
        {
           var firstSemesterStart= db.config.SingleOrDefault(x => x.Kay == "FirstSemesterStartDate");
           if(firstSemesterStart !=null) firstSemesterStart.Value = FirstSemesterStartDate;
            var firstSemesterEnd = db.config.SingleOrDefault(x => x.Kay == "FirstSemesterEndDate");
            if (firstSemesterEnd != null) firstSemesterEnd.Value = FirstSemesterEndDate;
            var secondSemesterStart = db.config.SingleOrDefault(x => x.Kay == "SecondSemesterStartDate");
            if (secondSemesterStart != null) secondSemesterStart.Value = SecondSemesterStartDate;
            var secondSemesterEnd = db.config.SingleOrDefault(x => x.Kay == "SecondSemesterEndDate");
            if (secondSemesterEnd != null) secondSemesterEnd.Value = SecondSemesterEndDate;
            db.SaveChanges();
            return RedirectToAction("Semesters");
        }

        public ActionResult Sms()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}

            //تهيئة الرسائل
           
            var permissionsSms = GetPermissionsFn(17);
            ViewBag.SMSRead = permissionsSms.Read;
            if (permissionsSms.Read)
            {
                var residenceExpirationConfigValue = db.config.SingleOrDefault(x => x.Kay == "ResidenceExpiration");
                var residenceMessageTemplate = db.ScMessages.AsEnumerable().SingleOrDefault(x =>
                    residenceExpirationConfigValue != null && x.ID == int.Parse(residenceExpirationConfigValue.Value));
                if (residenceMessageTemplate != null)
                {
                    ViewData["residenceExpirationValue"] = residenceMessageTemplate.ID;
                    ViewData["residenceExpirationText"] = residenceMessageTemplate.MsgTitle;
                }

                var passportExpirationConfigValue = db.config.SingleOrDefault(x => x.Kay == "PassportExpiration");
                var passportMessageTemplate = db.ScMessages.AsEnumerable().SingleOrDefault(x =>
                    passportExpirationConfigValue != null && x.ID == int.Parse(passportExpirationConfigValue.Value));
                if (passportMessageTemplate != null)
                {
                    ViewData["passportValue"] = passportMessageTemplate.ID;
                    ViewData["passportText"] = passportMessageTemplate.MsgTitle;
                }

                var trafficIssuesConfigValue = db.config.SingleOrDefault(x => x.Kay == "TrafficIssues");
                var trafficIssuesMessageTemplate = db.ScMessages.AsEnumerable().SingleOrDefault(x =>
                    trafficIssuesConfigValue != null && x.ID == int.Parse
                        (trafficIssuesConfigValue.Value));
                if (trafficIssuesMessageTemplate != null)
                {
                    ViewData["trafficIssuesValue"] = trafficIssuesMessageTemplate.ID;
                    ViewData["trafficIssuesText"] = trafficIssuesMessageTemplate.MsgTitle;
                }

                return View();
            }

            return RedirectToAction("NotAuthorized", "Security");
        }


        [ValidateInput(false)]
        public ActionResult SMSGridViewPartial()
        {
            //تهيئة الرسائل
        
            var permissionsSms = GetPermissionsFn(17);
            ViewBag.SMSCreate = permissionsSms.Create;
            ViewBag.SMSRead = permissionsSms.Read;
            ViewBag.SMSUpdate = permissionsSms.Update;
            ViewBag.SMSDelete = permissionsSms.Delete;
            ViewBag.SMSSave = permissionsSms.Save;

            var model = db.ScMessages;
            return PartialView("_SMSGridViewPartial", model.Where(x => x.ID > 7).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SMSGridViewPartialAddNew(ScMessages item)
        {
          
            var permissionsSms = GetPermissionsFn(17);
            ViewBag.SMSCreate = permissionsSms.Create;
            ViewBag.SMSRead = permissionsSms.Read;
            ViewBag.SMSUpdate = permissionsSms.Update;
            ViewBag.SMSDelete = permissionsSms.Delete;
            ViewBag.SMSSave = permissionsSms.Save;

            var model = db.ScMessages;
            if (ModelState.IsValid)
            {
                try
                {
                    item.School_ID = 1;
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "من فضلك حاول تصحيح الاخطاء والمحاولة مرة أخرى.";

            return RedirectToAction("Sms");

            return PartialView("_SMSGridViewPartial", model.Where(x => x.ID > 7).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SMSGridViewPartialUpdate(ScMessages item)
        {
          
            var permissionsSms = GetPermissionsFn(17);
            ViewBag.SMSCreate = permissionsSms.Create;
            ViewBag.SMSRead = permissionsSms.Read;
            ViewBag.SMSUpdate = permissionsSms.Update;
            ViewBag.SMSDelete = permissionsSms.Delete;
            ViewBag.SMSSave = permissionsSms.Save;
            var model = db.ScMessages;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.ID == item.ID);
                    if (modelItem != null)
                    {
                        item.School_ID = 1;
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "من فضلك حاول تصحيح الاخطاء والمحاولة مرة أخرى.";

            return RedirectToAction("Sms");
            return PartialView("_SMSGridViewPartial", model.Where(x => x.ID > 7).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SMSGridViewPartialDelete(int id)
        {
          
            var permissionsSms = GetPermissionsFn(17);
            ViewBag.SMSCreate = permissionsSms.Create;
            ViewBag.SMSRead = permissionsSms.Read;
            ViewBag.SMSUpdate = permissionsSms.Update;
            ViewBag.SMSDelete = permissionsSms.Delete;
            ViewBag.SMSSave = permissionsSms.Save;
            var model = db.ScMessages;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.ID == id);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            return RedirectToAction("Sms");

            return PartialView("_SMSGridViewPartial", model.Where(x => x.ID > 7).ToList());
        }

        [HttpGet]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId =17)]
        public ActionResult AddSmsTempalte()
        {
            return PartialView();
        }

       
        [HttpPost]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        public ActionResult AddSmsTempalte(string title, string text)
        {
            db.ScMessages.Add(new ScMessages
            {
                MsgTitle = title,
                MessageText = text,
                School_ID = 1
            });
            db.SaveChanges();
            return View();
        }

        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        public static List<ListEditItem> GetSmsTemplates()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                var templates = db.ScMessages.Where(x => x.ID > 7).Select(x =>
                    new ListEditItem {Value = x.ID.ToString(), Text = x.MsgTitle.ToString()});
                return templates.ToList();
            }
        }

        [HttpPost]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        public ActionResult SaveSmsTemplateConfig(string residenceExpiration, string passportExpiration,
            string trafficIssues)
        {
            var residenceExpirationItem = db.config.SingleOrDefault(x => x.Kay == "ResidenceExpiration");
            if (residenceExpirationItem != null)
            {
                residenceExpirationItem.Value = residenceExpiration;
                db.Entry(residenceExpirationItem).State = EntityState.Modified;
            }

            var passportExpirationItem = db.config.SingleOrDefault(x => x.Kay == "PassportExpiration");
            if (passportExpirationItem != null)
            {
                passportExpirationItem.Value = passportExpiration;
                db.Entry(passportExpirationItem).State = EntityState.Modified;
            }


            var trafficIssuesItem = db.config.SingleOrDefault(x => x.Kay == "TrafficIssues");
            if (trafficIssuesItem != null)
            {
                trafficIssuesItem.Value = trafficIssues;
                db.Entry(trafficIssuesItem).State = EntityState.Modified;
            }

            db.SaveChanges();
            return RedirectToAction("Sms");
        }

        #endregion


        #region Custom Fields

        public ActionResult CustomFields()
        {

            var permissions = GetCustomFieldsPermissionsFn(25);
            ViewBag.CreateParent = permissions.CreateParent;
            ViewBag.CreateChild = permissions.CreateChild;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (!permissions.View)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            return View();
        }

        [ValidateInput(false)]
        public ActionResult CustomFieldsTreeListPartial()
        {
          
            var permissions = GetCustomFieldsPermissionsFn(25);
            ViewBag.CreateParent = permissions.CreateParent;
            ViewBag.CreateChild = permissions.CreateChild;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;


            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}

            if (!permissions.View)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            return PartialView("_CustomFieldsTreeListPartial", db.CustomFields.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomFieldsTreeListPartialAddNew(CustomFields item)
        {
            
            var permissions = GetCustomFieldsPermissionsFn(25);
            ViewBag.CreateParent = permissions.CreateParent;
            ViewBag.CreateChild = permissions.CreateChild;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (!permissions.View)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.CustomFields.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "من فضلك، حاول تصحيح الأخطاء والمحاولة مرة أخرى.";
            }

            return PartialView("_CustomFieldsTreeListPartial", db.CustomFields.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomFieldsTreeListPartialUpdate(CustomFields item)
        {
        
            var permissions = GetCustomFieldsPermissionsFn(25);
            ViewBag.CreateParent = permissions.CreateParent;
            ViewBag.CreateChild = permissions.CreateChild;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;


            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}

            if (!permissions.View)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = db.CustomFields.FirstOrDefault(it => it.CustomFieldId == item.CustomFieldId);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "من فضلك، حاول تصحيح الأخطاء والمحاولة مرة أخرى.";
            }


            return PartialView("_CustomFieldsTreeListPartial", db.CustomFields.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomFieldsTreeListPartialDelete(int CustomFieldId)
        {
          
            var permissions = GetCustomFieldsPermissionsFn(25);
            ViewBag.CreateParent = permissions.CreateParent;
            ViewBag.CreateChild = permissions.CreateChild;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;

            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}

            if (!permissions.View)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            if (db.StudentsCustomFields.Where(x => x.CustomFieldId == CustomFieldId)?.Count() > 0)
            {
                ViewData["EditError"] = "عفوا لا يمكن حذف هذا الحقل لارتباطه بعناصر أخرى";
                return Json("عفوا لا يمكن حذف هذا الحقل لارتباطه بعناصر أخرى", JsonRequestBehavior.AllowGet);
            }

            try
            {
                var item = db.CustomFields.FirstOrDefault(it => it.CustomFieldId == CustomFieldId);
                if (item != null)
                {
                    db.CustomFields.Remove(item);
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
                return Json("حدث خطأ أثناء الحذف" + e.Message, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

            //return PartialView("_CustomFieldsTreeListPartial", db.CustomFields.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomFieldsTreeListPartialMove(int CustomFieldId, int ParentId)
        {
         
            var permissions = GetCustomFieldsPermissionsFn(25);
            ViewBag.CreateParent = permissions.CreateParent;
            ViewBag.CreateChild = permissions.CreateChild;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;

            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}

            if (!permissions.View)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            try
            {
                var item = db.CustomFields.FirstOrDefault(it => it.CustomFieldId == CustomFieldId);
                if (item != null)
                    item.ParentId = ParentId;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            return PartialView("_CustomFieldsTreeListPartial", db.CustomFields.ToList());
        }

        #endregion

        [HttpPost]
        public ActionResult UpdateDataBase()
        {
            var DBTrans = db.Database.BeginTransaction();
            try
            {
                var checkData = db.UpdateStudentLog.ToList().Where(m => m.InsertDate.Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")).Count();
                if (checkData > 0)
                {
                    var LastUpdate = db.UpdateStudentLog.ToList().Where(m => m.InsertDate.Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")).Select(p => p.InsertDate).Max();

                    return Json(new { isValid = false, msg = "لايمكن تحديث البيانات اليوم" + "\n"+ "("+LastUpdate + ")" +" حيث تمت عمليه التحديث بتوقيت" +"\n"+ "وفى حاله الرغبه فى الاستمرار فى التحديث يرجى مراجعه مدير النظام" }, JsonRequestBehavior.AllowGet);
                }
                string connectionString = System.Configuration.ConfigurationManager.AppSettings["UpdateDataBase.ConnectionString"];

                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    var cmd = new SqlCommand
                    {
                        Connection = sqlConnection,
                        CommandTimeout = 0,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "updateFromLinkedServer"
                    };
                    var cmd1 = new SqlCommand
                    {
                        Connection = sqlConnection,
                        CommandTimeout = 0,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "updateGraduateStudentsFromLinkedServer"
                    };
                    var count = cmd.ExecuteNonQuery();
                    var count1 = cmd1.ExecuteNonQuery();
                    sqlConnection.Close();
                    if (count > 0 && count1 > 0)
                    {
                        var CurrentUser = HttpContext.Session["UserId"] as DashBoard_Users;
                        UpdateStudentLog updateStudentLog = new UpdateStudentLog();
                        updateStudentLog.InsertDate = DateTime.Now;
                        updateStudentLog.UserID = CurrentUser.ID;
                        db.UpdateStudentLog.Add(updateStudentLog);
                        db.SaveChanges();
                        DBTrans.Commit();
                    }
                    return Json(new { isValid = true, msg = "تم تحديث البيانات بنجاح" });

                }
            }
            catch (SqlException ex)
            {
                DBTrans.Rollback();
                return Json(new { isValid = false, msg ="يوجد خطأ بالإتصال بقاعدة البيانات" });
               
            }
            catch (Exception e)
            {

                DBTrans.Rollback();
                return Json(new { isValid = false, msg ="يوجد خطأ"});
            }

            //var checkData = db.UpdateStudentLog.ToList().Where(m => m.InsertDate.Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")).Count();
            //if (checkData > 0)
            //{
            //    var LastUpdate=db.UpdateStudentLog.ToList().Where(m => m.InsertDate.Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")).Select(p=>p.InsertDate).Max();

            //    return Json(new { isValid=false,msg= LastUpdate + " " + "يوجد تحديث بتاريخ اليوم" }, JsonRequestBehavior.AllowGet);
            //}
            //string connectionString = System.Configuration.ConfigurationManager.AppSettings["UpdateDataBase.ConnectionString"];
              
            //using (var sqlConnection = new SqlConnection(connectionString))
            //{
            //    sqlConnection.Open();
            //    var cmd = new SqlCommand
            //    {
            //        Connection = sqlConnection,
            //        CommandTimeout = 0,
            //        CommandType = CommandType.StoredProcedure,
            //        CommandText = "updateFromLinkedServer"
            //    };
            //    var count = cmd.ExecuteNonQuery();
            //    sqlConnection.Close();
            //    if(count > 0)
            //    {
            //        var CurrentUser = HttpContext.Session["UserId"] as DashBoard_Users;
            //        UpdateStudentLog updateStudentLog = new UpdateStudentLog();
            //        updateStudentLog.InsertDate = DateTime.Now;
            //        updateStudentLog.UserID = CurrentUser.ID;
            //        db.UpdateStudentLog.Add(updateStudentLog);
            //        db.SaveChanges();
            //    }
            //    if(count > 0)
            //    {
            //        return Json(new { isValid = true, msg = "تم تحديث البيانات بنجاح" });
            //    }
            //    else
            //    {

            //        return Json(new { isValid = true, msg ="" });
            //    }
           // }
        }


        [HttpPost]
        public ActionResult UpdateDataBaseByAdmin()
        {
            var DBTrans = db.Database.BeginTransaction();
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.AppSettings["UpdateDataBase.ConnectionString"];

                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    var cmd = new SqlCommand
                    {
                        Connection = sqlConnection,
                        CommandTimeout = 0,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "updateFromLinkedServer"
                    };
                    var cmd1 = new SqlCommand
                    {
                        Connection = sqlConnection,
                        CommandTimeout = 0,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "updateGraduateStudentsFromLinkedServer"
                    };
                    var count = cmd.ExecuteNonQuery();
                    var count1 = cmd1.ExecuteNonQuery();
                    sqlConnection.Close();
                    if (count > 0 && count1 >0 )
                    {
                        var CurrentUser = HttpContext.Session["UserId"] as DashBoard_Users;
                        UpdateStudentLog updateStudentLog = new UpdateStudentLog();
                        updateStudentLog.InsertDate = DateTime.Now;
                        updateStudentLog.UserID = CurrentUser.ID;
                        db.UpdateStudentLog.Add(updateStudentLog);
                        db.SaveChanges();
                        DBTrans.Commit();
                    }
                    return Json(new { isValid = true, msg = "تم تحديث البيانات بنجاح" });

                }
            }
            catch (SqlException ex)
            {
                DBTrans.Rollback();
                return Json(new { isValid = false, msg = "يوجد خطأ بالإتصال بقاعدة البيانات" });

            }
            catch (Exception e)
            {

                DBTrans.Rollback();
                return Json(new { isValid = false, msg = "يوجد خطأ" });
            }
            //string connectionString = System.Configuration.ConfigurationManager.AppSettings["UpdateDataBase.ConnectionString"];

            //using (var sqlConnection = new SqlConnection(connectionString))
            //{
            //    sqlConnection.Open();
            //    var cmd = new SqlCommand
            //    {
            //        Connection = sqlConnection,
            //        CommandTimeout = 0,
            //        CommandType = CommandType.StoredProcedure,
            //        CommandText = "updateFromLinkedServer"
            //    };
            //    var count = cmd.ExecuteNonQuery();
            //    sqlConnection.Close();
            //    if (count > 0)
            //    {
            //        var CurrentUser = HttpContext.Session["UserId"] as DashBoard_Users;
            //        UpdateStudentLog updateStudentLog = new UpdateStudentLog();
            //        updateStudentLog.InsertDate = DateTime.Now;
            //        updateStudentLog.UserID = CurrentUser.ID;
            //        db.UpdateStudentLog.Add(updateStudentLog);
            //        db.SaveChanges();
            //    }
            //    if (count > 0)
            //    {
            //        return Json(new { isValid = true, msg = "تم تحديث البيانات بنجاح" });
            //    }
            //    else
            //    {

            //        return Json(new { isValid = true, msg = "" });
            //    }
            //  }
        }


        #region Filter New

        public ActionResult FilterGroupsNew()
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            //صلاحيات موافقات السلف
           
            var permssion = GetPermissionsFn(36);
            if (permssion.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            ViewBag.SavePermission = permssion.Save;
            return View();
        }

        [HttpGet]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 36)]
        public ActionResult GetFilterGroupUsers()
        {
            //if (HttpContext.Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            var users = db.DashBoard_Users.Select(x => new SelectListItem {Value = x.ID.ToString(), Text = x.Username})
                .ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 36)]
        public ActionResult GetFilterGroupsGroups()
        {
            //if (HttpContext.Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            var groups = db.Dashboard_FilterGroups.Select(x =>
                    new SelectListItem {Value = x.filterGropupID.ToString(), Text = x.groupName})
                .ToList();
            return Json(groups, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 36)]
        public ActionResult AddFilterGroups(int groupId, int[] usersIds)
        {
            //if (HttpContext.Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            var groups = db.Dashboard_FilterGroupUsers.Where(x => x.filterGropupID == groupId);
            if (groups.Any())
            {
                db.Dashboard_FilterGroupUsers.RemoveRange(groups);
            }

            foreach (var userid in usersIds)
            {
                db.Dashboard_FilterGroupUsers.Add(new Dashboard_FilterGroupUsers
                {
                    userID = userid,
                    filterGropupID = groupId
                });
            }

            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 36)]
        public ActionResult GetGroupUsers(int groupId)
        {
            //if (HttpContext.Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            var filterGroup = db.Dashboard_FilterGroups.SingleOrDefault(x => x.filterGropupID == groupId);
            if (filterGroup == null)
            {
                return Json("عفواً، لا يوجد مجموعة بهذه البيانات", JsonRequestBehavior.AllowGet);
            }

            var usersIds = filterGroup.Dashboard_FilterGroupUsers.AsEnumerable().Select(x => x.userID.ToString());
            return Json(usersIds, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region نظام قديم تهيئة السلف والاعانات

        //[HttpGet]
        //public ActionResult GetSecondaryAcctounts()
        //{
        //    var items = dbAcc.COA.Where(x => x.COA_Type == "S")
        //        .Select(x => new {Value = x.COAID, Text = x.COADescription});

        //    return Json(items, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Config()
        //{
           
        //    var permssion = GetPermissionsFn(17);

        //    if (permssion.Read == false)
        //    {
        //        return RedirectToAction("NotAuthorized", "Security");
        //    }

        //    return View();
        //}

        //[HttpPost]
        //[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        //public ActionResult SaveAdvancesConfig(int? value)
        //{
        //    var config = db.config.FirstOrDefault(x => x.Kay == "AdvancesConfig");
        //    if (config != null) config.Value = value.ToString();
        //    db.Entry(config).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        //public ActionResult SaveSubsidiesConfig(int? value)
        //{
        //    var config = db.config.FirstOrDefault(x => x.Kay == "SubsidiesConfig");
        //    if (config != null) config.Value = value.ToString();
        //    db.Entry(config).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        //public ActionResult GetAdvancesConfig()
        //{
        //    var config = db.config.FirstOrDefault(x => x.Kay == "AdvancesConfig");
        //    return Content(config?.Value);
        //}

        //[HttpGet]
        //[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        //public ActionResult GetSubsidiesConfig()
        //{
        //    var config = db.config.FirstOrDefault(x => x.Kay == "SubsidiesConfig");
        //    return Content(config?.Value);
        //}

        //[HttpGet]
        //[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        //public ActionResult GetAdvanceSettingConfig()
        //{
        //    var config = db.AdvanceSettingConfig.Select(x => new
        //    {
        //        x.ID,
        //        x.AdvanceSettingId,
        //        x.COAID,
        //        type = db.AdvanceSettings.FirstOrDefault(a => a.AdvanceSettingId == x.AdvanceSettingId).AdvanceType //== "A"? "سلفة": "اعانة"       
        //    }).ToList();
        //    return Json(config, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        //public ActionResult GetAdvanceSettings()
        //{
        //    var settings = db.AdvanceSettings.Select(x => new
        //    {
        //        x.AdvanceSettingId,
        //        x.AdvanceSettingName
        //    });
        //    return Json(settings, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 17)]
        //public ActionResult UpdateAdvanceSettingConfig(int id, int? advanceSettingId, int? coaId)
        //{
        //    var advanceSettingConfig = db.AdvanceSettingConfig.FirstOrDefault(x => x.ID == id);
        //    if (advanceSettingConfig == null)
        //    {
        //        return Json("عفواً، لا يوجد حساب بهذه البيانات", JsonRequestBehavior.AllowGet);
        //    }

        //    if (advanceSettingId != null)
        //    {
        //        advanceSettingConfig.AdvanceSettingId = (int) advanceSettingId;
        //    }

        //    if (coaId != null)
        //    {
        //        advanceSettingConfig.COAID = (int) coaId;
        //    }

        //    db.SaveChanges();

        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        #endregion


        #region تهيئة تصنيفات الملاحظات

        public ActionResult IssuesConfig()
        {
         
            var permssion = GetPermissionsFn(40);
            ViewBag.Create = permssion.Create;
            ViewBag.Update = permssion.Update;
            if (permssion.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            return View();
        }

        [HttpGet]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 40)]
        public ActionResult GetIssues()
        {
            var issues = db.Issues.Select(x => new {x.Id, x.IssueDescription});
            return Json(issues, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddIssue(string issueDescription)
        {
            if (string.IsNullOrEmpty(issueDescription))
            {
                return Json("اسم التصنيف اجبارى", JsonRequestBehavior.AllowGet);
            }

            db.Issues.Add(new Issues
            {
                IssueDescription = issueDescription
            });
            db.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 40)]
        public ActionResult UpdateIssue(int issueId, string issueDescription)
        {
            var issue = db.Issues.FirstOrDefault(x => x.Id == issueId);
            if (issue == null)
            {
                return Json("عفواً، لا يوجد تصنيف بهذه البيانات", JsonRequestBehavior.AllowGet);
            }

            issue.IssueDescription = issueDescription;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteIssue(int issueId)
        {
            var issue = db.Issues.FirstOrDefault(x => x.Id == issueId);
            if (issue == null)
            {
                return Json("عفواً، لا يوجد تصنيف بهذه البيانات", JsonRequestBehavior.AllowGet);
            }

            db.Issues.Remove(issue);
            db.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}