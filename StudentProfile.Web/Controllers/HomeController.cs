using StudentProfile.DAL.Models;
using DevExpress.Web.Mvc;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using StudentProfile.Components;

namespace StudentProfile.Controllers
{

   [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]

    public class HomeController : Controller
    {
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();

        private HRMadinaEntities dbHR = new HRMadinaEntities();

        // GET: Home

        [CustomAuthorizeHelper]
        public ActionResult Index(string section)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}

            //اشعارات المخالفات السلوكية
           
            var violationAlerts = GetPermissionsfn(19);
            ViewBag.violationAlertsRead = violationAlerts.Read;

            //اشعارات الإقامة
           
            var resdintialAlerts = GetPermissionsfn(24);
            ViewBag.resdintialAlertsRead = resdintialAlerts.Read;

            //اشعارات جوازات السفر
           
            var passportAlerts = GetPermissionsfn(20);
            ViewBag.passportAlertsRead = passportAlerts.Read;

            //اشعارات المخالفات المرورية
           
            var trafficAlerts = GetPermissionsfn(21);
            ViewBag.trafficAlertsRead = trafficAlerts.Read;

            //اشعارات التأشيرات
          
            var visaAlerts = GetPermissionsfn(22);
            ViewBag.visaAlertsRead = visaAlerts.Read;

            //اشعارات الهروب
          
            var runawayAlerts = GetPermissionsfn(23);
            ViewBag.runawayAlertsRead = runawayAlerts.Read;

            ViewBag.PartialName = section;
            return View();
        }

        [HttpPost]
        public JsonResult GetPermissions(int screenId)
        {

            var user = HttpContext.Session["UserId"] as DashBoard_Users;
           
            if (user!=null)
            {
             

                if (user.ID == 0)
                {
                    //return RedirectToAction("Login", "Login");
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

        public Permissions GetPermissionsfn(int screenId)
        {

            var user = HttpContext.Session["UserId"] as DashBoard_Users;

            if (user != null)
            {


                if (user.ID == 0)
                {
                    //return RedirectToAction("Login", "Login");
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
                else if (permission == "تعديل" || permission== "تحديث")
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

        //اشعارات الاقامة
        [CustomAuthorizeHelper(ScreenId =24)]
        public ActionResult _ResdintialAlerts(int? pageIndex, int? rowsCount)
        {
            //var jsonPermission = new JavaScriptSerializer().Serialize(GetPermissions(24).Data);
            //var permissions = JsonConvert.DeserializeObject<Permissions>(jsonPermission);

            //if (!permissions.Read)
            //{
            //    RedirectToAction("NotAuthorized", "Security");
            //}

            if (pageIndex == null)
            {
                pageIndex = 0;
            }

            if (rowsCount == null)
            {
                rowsCount = 25;
            }

            //rowsCount = rowsCount.Value == null ? rowsCount.Value : 25;
            IQueryable<proc_EmpDocumentsDetails_Exp_New_Result> modelList = dbHR.proc_EmpDocumentsDetails_Exp_New(1)
                .Where(x => x.studentid != null)
                .OrderBy(x => x.ExpDate).AsQueryable();

            return PartialView(modelList);
        }

        [CustomAuthorizeHelper(ScreenId= 20)]
        //اشعارات جواز السفر
        public ActionResult _PassportAlerts(int? page)
        {
            //var jsonPermission = new JavaScriptSerializer().Serialize(GetPermissions(20).Data);
            //var permissions = JsonConvert.DeserializeObject<Permissions>(jsonPermission);
            //ViewBag.resdintialAlertsRead = permissions.Read;
            //if (!permissions.Read)
            //{
            //    RedirectToAction("NotAuthorized", "Security");
            //}

            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IEnumerable<proc_EmpDocumentsDetails_Exp_New_Result> modelList = dbHR.proc_EmpDocumentsDetails_Exp_New(2)
                .Where(x => x.studentid != null)
                .OrderBy(x => x.ExpDate).ToList();
            return PartialView(modelList);
        }

        //اشعارات السكن

        [CustomAuthorizeHelper()]
        public ActionResult _HostelAlerts()
        {
            var studentList = dbSC.usp_Students_Select_byUnitID(null, null, 0).ToList();
            List<StudentsAlertList> modelList = new List<StudentsAlertList>();
            foreach (var item in studentList)
            {
                StudentsAlertList model = new StudentsAlertList();
                var student = dbSC.INTEGRATION_All_Students.SingleOrDefault(x => x.NATIONAL_ID == item.IDNumber);
                if (student != null)
                {
                    model.StudentId = (long) student.STUDENT_ID;
                    model.IdNumber = item.IDNumber;
                    model.StudentName = student.STUDENT_NAME;

                    model.IdentityExpireDate = item.OutDate;
                    model.IdentityId = 0;
                    modelList.Add(model);
                }
            }

            return PartialView(modelList);
        }
        [CustomAuthorizeHelper(ScreenId = 18)]
        public ActionResult _TrafficAlerts(int? page)
        {
            //var jsonPermission = new JavaScriptSerializer().Serialize(GetPermissions(18).Data);
            //var permissions = JsonConvert.DeserializeObject<Permissions>(jsonPermission);
            //ViewBag.resdintialAlertsRead = permissions.Read;
            //if (!permissions.Read)
            //{
            //    RedirectToAction("NotAuthorized", "Security");
            //}

            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var modelList = dbSC.proc_GetStudentsViolations(null).ToList();
            return PartialView(modelList);
        }
        [CustomAuthorizeHelper(ScreenId = 19)]
        public ActionResult _ViolationAlerts()
        {
            //var jsonPermission = new JavaScriptSerializer().Serialize(GetPermissions(19).Data);
            //var permissions = JsonConvert.DeserializeObject<Permissions>(jsonPermission);
            //ViewBag.resdintialAlertsRead = permissions.Read;
            //if (!permissions.Read)
            //{
            //    return RedirectToAction("NotAuthorized", "Security");
            //}

            var modelList = dbSC.INTEGRATION_All_Students.Where(x =>
                    x.STATUS_CODE == 1 && dbSC.V_SIS_VIOLATION_SINGLE.Any(p => p.STUDENT_ID.Value == x.STUDENT_ID))
                .ToList();
            return PartialView(modelList);
        }

        public ActionResult _RunawayAlerts(decimal? idNumber)
        {
            var modelList = dbSC.proc_GetRunawayAliens().ToList();

            if (idNumber != null)
            {
                modelList = modelList.Where(x => x.studentid == idNumber).ToList();
            }

            return PartialView(modelList);
        }

        public ActionResult _VisaAlerts(string idNumber)
        {
            var modelList = new  List<ReservedVisa>();
            if (!string.IsNullOrEmpty(idNumber))
            {
                modelList = dbSC.ReservedVisa.Where(x => x.IdNumber == idNumber).ToList();
            }

            return PartialView(modelList);
        }
        [HttpPost]
        public JsonResult GetUpdateDataPermissions(int screenId)
        {

            var user = HttpContext.Session["UserId"] as DashBoard_Users;
          
            if (user!=null )
            {
             

                if (user.ID == 0)
                {
                    //return RedirectToAction("Login", "Login");
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }


            var perm = CheckPermissions.IsAuthorized(user.ID, screenId);

            var permissions = new Permissions();
            foreach (var permission in perm)
            {
                if (permission == "تحديث")
                {
                    permissions.Update = true;
                }
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        //تفاصيل هويات الطالب
        public ActionResult _StudentIdentities(decimal? studentId)
        {
            if (studentId != null && studentId > 0)
            {
                var documentList = dbHR.PR_StudentDocuments_SelectAll((int) studentId).OrderBy(x => x.ExpDate).ToList();
                var student = dbSC.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == studentId).SingleOrDefault();
                ViewBag.Faculty = student.FACULTY_NAME;
                ViewBag.Major = student.MAJOR_NAME;
                ViewBag.Level = student.LEVEL_DESC;
                ViewBag.Nationality = student.NATIONALITY_DESC;
                return PartialView(documentList);
            }

            return PartialView();
        }

        public ActionResult smsPopUp(string type)
        {
            string msg = "";
            if (type != "" && type != null)
            {
                string key = dbSC.config.Where(x => x.Kay == type).FirstOrDefault()?.Value;

                msg = dbSC.ScMessages.Where(x => x.ID.ToString() == key).FirstOrDefault()?.MessageText;
            }

            ViewBag.Message = msg;
            return PartialView();
        }

        public ActionResult UpdateDate()
        {


            var permissions = GetPermissionsfn(38); 
            ViewBag.CanUpdateData = permissions.Update;
            var permissions1 = GetPermissionsfn(131);
            ViewBag.CanUpdateData1 = permissions1.Update;
            return View();
        }

        public ActionResult UpdateByAdmin()
        {
            var permissions = GetPermissionsfn(131);
            ViewBag.CanUpdateData = permissions.Update;
            return View();
        }
        public ActionResult _NavLeft()
        {
           
            //اشعارات المخالفات السلوكية
           
            var violationAlerts = GetPermissionsfn(19);
            ViewBag.violationAlertsRead = violationAlerts.Read;

            //اشعارات الإقامة
            
            var resdintialAlerts = GetPermissionsfn(24);
            ViewBag.resdintialAlertsRead = resdintialAlerts.Read;

            //اشعارات جوازات السفر
            
            var passportAlerts = GetPermissionsfn(20);
            ViewBag.passportAlertsRead = passportAlerts.Read;

            //اشعارات المخالفات المرورية
          
            var trafficAlerts = GetPermissionsfn(21);
            ViewBag.trafficAlertsRead = trafficAlerts.Read;

            //اشعارات التأشيرات
         
            var visaAlerts = GetPermissionsfn(22);
            ViewBag.visaAlertsRead = visaAlerts.Read;

            //اشعارات الهروب
       
            var runawayAlerts = GetPermissionsfn(23);
            ViewBag.runawayAlertsRead = runawayAlerts.Read;

            List<proc_GetAlertsCount_Result> model = dbSC.proc_GetAlertsCount().ToList();
            return PartialView("_NavLeft", model);
        }

        public bool send_Message(string message, List<NotifiedStudents> students)
        {
            try
            {
                foreach (var item in students)
                {
                    if (item.Phone != null && item.Phone.Length > 8)
                    {
                        item.Phone = item.Phone.Replace("+", "");
                        if (!item.Phone.StartsWith("966"))
                        {
                            item.Phone = "966" + item.Phone;
                        }

                        int? SchoolID = dbSC.StudentBasicData.Where(x => x.IDNumber == item.IdNumber).FirstOrDefault()
                            ?.School_ID;
                        string sender = string.Empty;
                        string UserName = string.Empty;
                        string Password = string.Empty;
                        var dtschool = dbSC.usp_School_Select(SchoolID);
                        sender = "iu-edu-s-AD";
                        UserName = "iuedusa";
                        Password = "135246";

                        HttpWebRequest request = (HttpWebRequest) WebRequest.Create
                        (System.Configuration.ConfigurationManager.AppSettings["SMS.Service.Url"] +"? username=" + UserName + "&password=" + Password +
                         "&lang=ar&numbers=" + item.Phone.Trim() + "&sender=esol&message=" + message);
                        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                        Stream responseStream = response.GetResponseStream();
                        StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);
                        string strSMSResponseString = readStream.ReadToEnd();
                        MessagesLog msgLog = new MessagesLog();
                        msgLog.MessageText = message;
                        msgLog.National_Id = item.IdNumber;
                        msgLog.StDetailID = dbSC.StDetails.Where(x => x.StudentBasicData.IDNumber == item.IdNumber)
                            .FirstOrDefault()?.StDetailID;
                        msgLog.Student_Id = item.Id;
                        msgLog.Student_Name = item.Name;
                        msgLog.Student_Mobile = item.Phone;
                        msgLog.SendDate = DateTime.Now;
                        dbSC.MessagesLog.Add(msgLog);
                    }
                }

                dbSC.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class NotifiedStudents
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IdNumber { get; set; }
        public string Phone { get; set; }
    }
}