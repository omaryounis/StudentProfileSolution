using Newtonsoft.Json;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public partial class AcademicActivitiesController : Controller
    {
        private readonly SchoolAccGam3aEntities dbSch;

        public AcademicActivitiesController()
        {
            dbSch = new SchoolAccGam3aEntities();
        }

        #region المشاركات القديمة

        #region شاشة إضافة مشاركة قديمة من عند الطالب

        [HttpGet]
        public ViewResult StudentActivityRequests()
        {
            // No Permissions For Student Pages ...
            return View();
        }

        [HttpGet]
        public ActionResult GetAllActivitiesByStudent()
        {
            return this.JsonMaxLength(
                dbSch.Usp_GetAllActivities_StudentId(CurrentStudentId).ToList()
                                                                      .Select(x => new
                                                                      {
                                                                          x.Type,
                                                                          x.Name,
                                                                          x.Ratio,
                                                                          x.Status,
                                                                          x.Degree,
                                                                          x.Duration,
                                                                          x.Location,
                                                                          x.Student_ID,
                                                                          x.RefuseReson,
                                                                          x.ApprovedStatus,
                                                                          AcademicActivitiesId = x.ID,
                                                                          EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                                                                          StartDate = x.StartDate.ToString("dd/MM/yyyy")
                                                                      }).ToList());

            #region Old Code
            //dbSch.ActivityRequestsArchive.Where(x => x.Student_ID == CurrentStudentId && (x.IsCanceled == false || x.IsCanceled == null)).ToList().Select(p => new
            //{
            //    AcademicActivitiesId = p.ID,
            //    Student_Id = p.Student_ID,
            //    Degree = p.Degree,
            //    Duration = p.Duration,
            //    Location = p.Location,
            //    Type = p.Type,
            //    EndDate = p.EndDate.ToString("dd/MM/yyyy"),
            //    StartDate = p.StartDate.ToString("dd/MM/yyyy"),
            //    Name = p.Name,
            //    Ratio = p.Ratio,
            //    Status = p.RefusedbyID != null ? "تم الرفض" : p.ApprovedbyID != null ? "تم الإعتماد" : "تحت المراجعة",
            //    RefusalReason = p.RefuseReson
            //}).ToList());
            #endregion
        }

        [HttpPost]
        public ActionResult SaveActivityArchiveByStudent(string Degree,
                                                 string Duration, string Location,
                                                 DateTime StartDate, DateTime EndDate,
                                                 string Type, string Name, decimal Ratio)
        {
            if (CurrentStudentId == 0)
            {
                return RedirectToAction("Login", "Login");
            }

            var student = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == CurrentStudentId);
            if (student == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var file = (HttpPostedFileBase)Session["AcademicActivityFile"];
            if (file == null)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع نموذج المشاركة");
            }

            var activityRequest = new ActivityRequestsArchive
            {
                Type = Type,
                Name = Name,
                Ratio = Ratio,
                User_Id = null,
                Degree = Degree,
                EndDate = EndDate,
                IsCanceled = null,
                RefusedbyID = null,
                RefuseReson = null,
                ApprovedbyID = null,
                Location = Location,
                Duration = Duration,
                StartDate = StartDate,
                InsertionDate = DateTime.Now,
                Student_ID = CurrentStudentId
            };
            dbSch.ActivityRequestsArchive.Add(activityRequest);
            dbSch.SaveChanges();

            var academicActivitiesAttachment = new ActivityRequestsArchiveAttachments
            {
                FileName = file.FileName,
                UserId = null,
                InsertDate = DateTime.Now,
                ActivityRequests_Id = activityRequest.ID,
                Path = GetFilePath(file, $"ActivityRequestArchive", student.NATIONAL_ID.ToString())
            };
            dbSch.ActivityRequestsArchiveAttachments.Add(academicActivitiesAttachment);
            dbSch.SaveChanges();

            Session["AcademicActivityFile"] = null;
            return Content("");
        }


        [HttpDelete]
        public ActionResult DeleteActivityRequestArchive(int AcademicActivityId)
        {
            var model = dbSch.ActivityRequestsArchive.Where(x => x.ID == AcademicActivityId).FirstOrDefault();
            if (model == null)
            {
                return Json("لايمكن إتمام عملية الحذف برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري", JsonRequestBehavior.AllowGet);
            }
            if (dbSch.ActivityApprovedPhases.Any(x => x.ActivityRequested_ID == model.ID))
            {
                return Json("لايمكن إتمام عملية الحذف حيث أن هذه المشاركة دخلت ضمن مراحل الإعتماد", JsonRequestBehavior.AllowGet);
            }

            #region مش هنحذف أي حاجة كل اللي هيتعمل اني هخليها ملغيه بس

            //var attachment = dbSch.ActivityRequestsArchiveAttachments.FirstOrDefault(x => x.ActivityRequests_Id == model.ID);
            //if (attachment != null)
            //{
            //    if (System.IO.File.Exists(Server.MapPath(attachment.Path)))
            //    {
            //        System.IO.File.SetAttributes(Server.MapPath(attachment.Path), FileAttributes.Normal);

            //        System.IO.File.Delete(Server.MapPath(attachment.Path));
            //    }

            //    dbSch.ActivityRequestsArchiveAttachments.Remove(attachment);
            //    dbSch.SaveChanges();
            //}
            #endregion

            model.IsCanceled = true;
            dbSch.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbSch.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult DownloadActivityArchivedAttachment(int activityArchivedId)
        {
            string FileVirtualPath = null;
            if (CurrentUser.IsStudent) {
                int studentID = Convert.ToInt32(CurrentUser.Username);
                FileVirtualPath = dbSch.ActivityRequestsArchiveAttachments
                                       .Where(x => x.ActivityRequests_Id == activityArchivedId
                                       && x.ActivityRequestsArchive.Student_ID== studentID)
                                       .FirstOrDefault()?.Path;
            }
            else
            {
                var addOldActPermission = GetPermissionsFn(71).View;
                var monitorOldActPermission = GetPermissionsFn(73).View;
                var viewNewActPermission = GetPermissionsFn(100).View;
                var approveNewActPermission = GetPermissionsFn(101).View;
                if (addOldActPermission || monitorOldActPermission || viewNewActPermission || approveNewActPermission)
                {
                    FileVirtualPath = dbSch.ActivityRequestsArchiveAttachments
                                       .Where(x => x.ActivityRequests_Id == activityArchivedId)
                                       .FirstOrDefault()?.Path;
                }
            }
            if (FileVirtualPath != null)
            {
                if (!System.IO.File.Exists(HostingEnvironment.MapPath(FileVirtualPath)))
                {
                    return Content("");
                }
                byte[] FileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(FileVirtualPath));
                return File(FileBytes, "application/pdf");

                //===============================================================================================
                // ** File As DownLoad ** ...
                // return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
                //===============================================================================================
            }
            return Content("");
        }

        #endregion

        #region شاشة إضافة مشاركة قديمة من عند الأدمن

        #region Permissions

        [HttpGet]
        public JsonResult GetAdminActivityRequestsPermissions(int screenId)
        {
            if (CurrentUser == null || CurrentUser?.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new AdminActivityRequestsPermissions();
            foreach (var permission in perm)
            {
                if (permission == "عرض")
                {
                    permissions.View = true;
                }
                if (permission == "إضافة مشاركات")
                {
                    permissions.CreateActivityRequest = true;
                }
                else if (permission == "استعراض المشاركات")
                {
                    permissions.SearchActivityRequest = true;
                }
                else if (permission == "حذف المشاركات")
                {
                    permissions.DeleteActivityRequest = true;
                }
            }

            return this.JsonMaxLength(permissions);
        }

        public class AdminActivityRequestsPermissions
        {
            public bool View { get; set; }
            public bool CreateActivityRequest { get; set; }
            public bool SearchActivityRequest { get; set; }
            public bool DeleteActivityRequest { get; set; }
        }

        #endregion

        [HttpGet]
        public ActionResult AdminActivityRequests()
        {
            var json = new JavaScriptSerializer().Serialize(GetAdminActivityRequestsPermissions(71).Data);
            var permissions = JsonConvert.DeserializeObject<AdminActivityRequestsPermissions>(json);
            if (permissions == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.View = permissions.View;

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        [HttpGet]
        public ActionResult GetAllStudents()
        {
            return this.JsonMaxLength(dbSch.SP_GetAllStudent_Names_Customize().ToList());
        }

        [HttpPost]
        public ActionResult SaveActivityArchiveByAdmin(int StudentId, string Degree,
                                               string Duration, string Location,
                                               DateTime StartDate, DateTime EndDate,
                                               string Type, string Name, decimal Ratio)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var student = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == StudentId);
            if (student == null)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم اختيار الطالب");
            }

            var file = (HttpPostedFileBase)Session["AcademicActivityFile"];
            if (file == null)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع نموذج المشاركة");
            }

            var activityRequest = new ActivityRequestsArchive
            {
                Type = Type,
                Name = Name,
                Ratio = Ratio,
                Degree = Degree,
                EndDate = EndDate,
                IsCanceled = null,
                RefusedbyID = null,
                RefuseReson = null,
                ApprovedbyID = null,
                Location = Location,
                Duration = Duration,
                StartDate = StartDate,
                Student_ID = StudentId,
                User_Id = CurrentUser.ID,
                InsertionDate = DateTime.Now,
            };
            dbSch.ActivityRequestsArchive.Add(activityRequest);
            dbSch.SaveChanges();

            var academicActivitiesAttachment = new ActivityRequestsArchiveAttachments
            {
                UserId = CurrentUser.ID,
                FileName = file.FileName,
                InsertDate = DateTime.Now,
                ActivityRequests_Id = activityRequest.ID,
                Path = GetFilePath(file, $"ActivityRequestArchive", student.NATIONAL_ID.ToString())
            };
            dbSch.ActivityRequestsArchiveAttachments.Add(academicActivitiesAttachment);
            dbSch.SaveChanges();

            Session["AcademicActivityFile"] = null;
            return Content("");
        }

        [HttpGet]
        public ActionResult GetActivitiesByAdmin(int studentId)
        {
            return this.JsonMaxLength(
               dbSch.Usp_GetAllActivities_StudentId(studentId).ToList()
                                                              .Select(x => new
                                                              {
                                                                  x.Type,
                                                                  x.Name,
                                                                  x.Ratio,
                                                                  x.Status,
                                                                  x.Degree,
                                                                  x.Duration,
                                                                  x.Location,
                                                                  x.Student_ID,
                                                                  x.RefuseReson,
                                                                  x.ApprovedStatus,
                                                                  AcademicActivitiesId = x.ID,
                                                                  EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                                                                  StartDate = x.StartDate.ToString("dd/MM/yyyy")
                                                              }).ToList());

            #region Old Code
            //dbSch.ActivityRequestsArchive.Where(x => x.Student_ID == studentId).ToList().Select(p => new
            //{
            //    AcademicActivitiesId = p.ID,
            //    Student_Id = p.Student_ID,
            //    Degree = p.Degree,
            //    Duration = p.Duration,
            //    Location = p.Location,
            //    Type = p.Type,
            //    EndDate = p.EndDate.ToString("dd/MM/yyyy"),
            //    StartDate = p.StartDate.ToString("dd/MM/yyyy"),
            //    Name = p.Name,
            //    Ratio = p.Ratio,
            //    Status = p.ApprovedbyID != null ? "معتمد" : p.RefusedbyID != null ? "مرفوض" : "تحت المراجعة",
            //    RefusalReason = p.RefuseReson
            //}).ToList());
            #endregion
        }

        #endregion

        #region شاشة تهيئة مراحل إعتماد المشاركات القديمة

        #region Permissions

        [HttpGet]
        public JsonResult GetActivityPhasesPermissions(int screenId)
        {
            if (CurrentUser == null || CurrentUser?.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new ActivityPhasesPermissions();
            foreach (var permission in perm)
            {
                if (permission == "عرض")
                {
                    permissions.View = true;
                }
                if (permission == "إضافة مراحل اعتماد")
                {
                    permissions.AddActivityPhase = true;
                }
                else if (permission == "تعديل ترتيب المراحل")
                {
                    permissions.EditActivityPhaseOrder = true;
                }
                else if (permission == "اسناد المستخدمين علي مراحل الاعتماد")
                {
                    permissions.AttributeUserToActivityPhase = true;
                }
                else if (permission == "تعديل اسناد المستخدمين علي مراحل الاعتماد")
                {
                    permissions.EditAttributeUserToActivityPhase = true;
                }
                else if (permission == "تفعيل وإيقاف المستخدمين")
                {
                    permissions.ActiveAndStopUser = true;
                }
            }

            return this.JsonMaxLength(permissions);
        }


        public class ActivityPhasesPermissions
        {
            public bool View { get; set; }
            public bool AddActivityPhase { get; set; }
            public bool ActiveAndStopUser { get; set; }
            public bool EditActivityPhaseOrder { get; set; }
            public bool AttributeUserToActivityPhase { get; set; }
            public bool EditAttributeUserToActivityPhase { get; set; }
        }

        #endregion

        [HttpGet]
        public ActionResult ActivityPhases()
        {
            var json = new JavaScriptSerializer().Serialize(GetActivityPhasesPermissions(72).Data);
            var permissions = JsonConvert.DeserializeObject<ActivityPhasesPermissions>(json);
            if (permissions == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Create = permissions.View;

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }


        [HttpGet]
        public ActionResult GetAllUsers()
        {
            //=======================================================================================================
            //  عرض المستخدمين الموجود ليهم صلاحية علي شاشة متابعة طلبات المشاركات فقط وليس كل المستخدمين
            //=======================================================================================================

            return this.JsonMaxLength(dbSch.Usp_GetAllUsers_ToDecisionTakingForActivities().ToList());
        }


        [HttpGet]
        public ActionResult GetAllActivityPhasesDDL()
        {
            return this.JsonMaxLength(dbSch.ActivityPhases
                                           .Select(x => new
                                           {
                                               ID = x.ID,
                                               PhaseName = x.PhaseName,
                                               Order = "المرحلة رقم" + " : " + x.Order
                                           }).ToList());
        }


        [HttpGet]
        public ActionResult GetAllActivityPhasesGridSource()
        {
            return this.JsonMaxLength(dbSch.ActivityUsers.Select(x => new
            {
                ActivityUserId = x.ID,
                ActivityPhases_Id = x.ActivityPhase_ID,
                PhaseName = x.ActivityPhases.PhaseName,
                Order = x.ActivityPhases.Order,
                UserName = x.DashBoard_Users.Name,
                IsAdmin = x.DashBoard_Users.IsAdmin != true ? false : true,
                UserStatus = x.IsActive != true ? false : true
            }).ToList());
        }


        [HttpGet]
        public ActionResult GetActivityUser(int activityUserId)
        {
            return this.JsonMaxLength(dbSch.ActivityUsers.Where(x => x.ID == activityUserId && x.IsActive == true)
                                                         .Select(x => new
                                                         {
                                                             ActivityPhases_Id = x.ActivityPhase_ID,
                                                             UserID = x.UserID
                                                         }).FirstOrDefault());
        }


        [HttpGet]
        public ActionResult GetActivityPhaseById(int activityPhaseId)
        {
            return this.JsonMaxLength(dbSch.ActivityPhases
                                           .Where(x => x.ID == activityPhaseId)
                                           .Select(x => new
                                           {
                                               x.ID,
                                               x.Order,
                                               x.PhaseName
                                           }).FirstOrDefault());
        }


        [HttpPost]
        public ActionResult SaveActivityPhase(int? ActivityPhaseId, string ActivityPhaseName, byte phaseOrder)
        {
            try
            {
                if (string.IsNullOrEmpty(ActivityPhaseName))
                {
                    return Content("لا يمكن إتمام عملية الحفظ حتي يتم إدخال إسم المرحلة");
                }

                if (phaseOrder < 1)
                {
                    return Content("لا يمكن إتمام عملية الحفظ حتي يتم إدخال ترتيب المرحلة");
                }

                if (dbSch.ActivityPhases.Where(x => x.PhaseName == ActivityPhaseName && x.ID != ActivityPhaseId).FirstOrDefault() != null)
                {
                    return Content("لا يمكن إتمام عملية الحفظ لوجود مرحلة بنفس الإسم");
                }

                var ActivityPhaseInDB = dbSch.ActivityPhases.Where(x => x.ID == ActivityPhaseId).FirstOrDefault();
                if (ActivityPhaseInDB != null)
                {

                    //===============================================
                    //هل المستخدم دا عدل ترتيب مرحلة الإعتماد ولا لأه
                    //===============================================
                    var SecondActivityPhaseInDB = dbSch.ActivityPhases.Where(x => x.Order == phaseOrder).FirstOrDefault();
                    if (SecondActivityPhaseInDB == null)
                    {
                        return Content("لا يمكن إتمام عملية الحفظ لعدم وجود مرحلة بنفس الترتيب");
                    }

                    if (ActivityPhaseInDB.ID != SecondActivityPhaseInDB.ID)
                    {
                        //====================================
                        // عملية التبديل بين مرحلتي الإعتماد
                        //====================================
                        SecondActivityPhaseInDB.Order = ActivityPhaseInDB.Order;
                        dbSch.Entry(SecondActivityPhaseInDB).State = System.Data.Entity.EntityState.Modified;


                        ActivityPhaseInDB.Order = phaseOrder;
                    }

                    ActivityPhaseInDB.PhaseName = ActivityPhaseName;
                    dbSch.Entry(ActivityPhaseInDB).State = System.Data.Entity.EntityState.Modified;
                    dbSch.SaveChanges();
                    return Content("");
                }
                else
                {
                    if (dbSch.ActivityPhases.Where(x => x.Order == phaseOrder && x.ID != ActivityPhaseId).FirstOrDefault() != null)
                    {
                        return Content("لا يمكن إتمام عملية الحفظ لوجود مرحلة بنفس الترتيب");
                    }

                    if (dbSch.ActivityPhases.Where(x => x.Order == (phaseOrder - 1)).FirstOrDefault() != null || phaseOrder == 1)
                    {

                        var ActivityPhases = new ActivityPhases()
                        {
                            Order = phaseOrder,
                            PhaseName = ActivityPhaseName
                        };
                        dbSch.ActivityPhases.Add(ActivityPhases);
                        dbSch.SaveChanges();

                        return Content("");
                    }
                    return Content("لا يمكن إتمام عملية الحفظ لتخطى أرقام ترتيب المراحل");
                }
            }
            catch (Exception ex)
            {
                return Content("حدث خطأ أثناء الحفظ " + " " + ex.Message);
            }
        }


        [HttpPost]
        public ActionResult SaveActivityUser(int? ActivityUserId, int ActivityPhaseId, int userId)
        {

            var ActivityUserInDB = dbSch.ActivityUsers.Where(x => x.ID == ActivityUserId).FirstOrDefault();
            if (ActivityUserInDB != null)
            {
                if (dbSch.ActivityUsers.Where(x => x.ActivityPhase_ID == ActivityPhaseId && x.UserID == userId && x.ID != ActivityUserId).FirstOrDefault() != null)
                {
                    return Content("لا يمكن إتمام عملية الحفظ نظرا لان هذا المسؤول مضاف علي هذه المرحلة بالفعل من قبل");
                }

                ActivityUserInDB.UserID = userId;
                ActivityUserInDB.ActivityPhase_ID = ActivityPhaseId;
                dbSch.Entry(ActivityUserInDB).State = System.Data.Entity.EntityState.Modified;
                dbSch.SaveChanges();

                return Content("");
            }
            else
            {
                if (dbSch.ActivityUsers.Where(x => x.ActivityPhase_ID == ActivityPhaseId && x.UserID == userId).FirstOrDefault() != null)
                {
                    return Content("لا يمكن إتمام عملية الحفظ نظرا لان هذا المسؤول مضاف علي هذه المرحلة بالفعل من قبل");
                }

                var ActivityUsers = new ActivityUsers()
                {

                    UserID = userId,
                    IsActive = true,
                    ActivityPhase_ID = ActivityPhaseId
                };
                dbSch.ActivityUsers.Add(ActivityUsers);
                dbSch.SaveChanges();

                return Content("");
            }
        }


        [HttpPost]
        public ActionResult EditactivityUserStatus(int activityUserId, bool status)
        {
            var db = new SchoolAccGam3aEntities();

            var activityUsersInDB = db.ActivityUsers.Where(x => x.ID == activityUserId).FirstOrDefault();
            if (activityUsersInDB != null)
            {
                if (activityUsersInDB.IsActive == status)
                {
                    return Content("");
                }
                activityUsersInDB.IsActive = status;
                db.Entry(activityUsersInDB).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Content("");
            }
            return Content("لا يمكن تعديل حالة المسؤول نظرا لإرتباطه بسجلات أخري");

        }

        #endregion

        #region شاشة متابعة وإعتماد المشاركات القديمة

        #region Permissions

        [HttpGet]
        public JsonResult GetActivityDecisionTakingPermissions(int screenId)
        {
            if (CurrentUser == null || CurrentUser?.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new ActivityDecisionTakingPermissions();
            foreach (var permission in perm)
            {
                if (permission == "عرض")
                {
                    permissions.View = true;
                }
                if (permission == "تعديل بيانات طلب المشاركة")
                {
                    permissions.EditActivityRequest = true;
                }
            }

            return this.JsonMaxLength(permissions);
        }

        public class ActivityDecisionTakingPermissions
        {
            public bool View { get; set; }
            public bool EditActivityRequest { get; set; }
        }

        #endregion

        [HttpGet]
        public ActionResult ActivityDecisionTaking()
        {
            var json = new JavaScriptSerializer().Serialize(GetActivityDecisionTakingPermissions(73).Data);
            var permissions = JsonConvert.DeserializeObject<ActivityDecisionTakingPermissions>(json);
            if (permissions == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Create = permissions.View;
            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }


        [HttpGet]
        public ActionResult GetActivityRequestsByUserId()
        {
            // مراحل الموافقة التي يقع فيها هذا المستخدم
            var userAuthorizedPhases = dbSch.ActivityUsers.Where(x => x.UserID == CurrentUser.ID && x.IsActive == true).Select(x => x.ActivityPhases.Order);

            var activityRequestsArchive = dbSch.ActivityRequestsArchive
                                               .Where(x =>
                                                        ((x.IsCanceled == false || x.IsCanceled == null) &&
                                                        x.RefusedbyID == null &&
                                                        x.RefuseReson == null &&
                                                        x.ApprovedbyID == null) &&
                                                        userAuthorizedPhases.Any(p => p == x.ActivityApprovedPhases.Count() + 1)
                                                     );

            var studentAcademicData = dbSch.INTEGRATION_All_Students
                                           .Where(x => activityRequestsArchive.Any(p => p.Student_ID == x.STUDENT_ID))
                                           .Select(x => new
                                           {
                                               x.STUDENT_NAME,
                                               x.NATIONAL_ID,
                                               x.STUDENT_ID,
                                               x.NATIONALITY_DESC,
                                               x.FACULTY_NAME,
                                               x.DEGREE_DESC,
                                               LEVEL_DESC = x.LEVEL_DESC == null || x.LEVEL_DESC == "" ? "غير متوفر" : x.LEVEL_DESC,
                                               x.STUDY_DESC,
                                               x.STATUS_DESC,
                                           }).ToList();

            var data = activityRequestsArchive.ToList().Select(t => new
            {
                ActivityName = t.Name,
                ActivityRequestId = t.ID,
                studentId = t.Student_ID,
                ActivityLocation = t.Location,
                IsApprovedPhase = t.ActivityApprovedPhases.Count() > 0 ? true : false,
                StudentAcademicData = studentAcademicData.FirstOrDefault(x => x.STUDENT_ID == t.Student_ID),
                RequestedDate = t.InsertionDate.Day + "/" + t.InsertionDate.Month + "/" + t.InsertionDate.Year
            }).ToList();

            return this.JsonMaxLength(data);
        }


        [HttpGet]
        public ActionResult StudentActivityDetailsDataSource(int ActivityRequestId)
        {
            // طلب المشاركة المراد معرفة تفاصيله
            var studentActivityRequest = dbSch.ActivityRequestsArchive
                                              .Where(x =>
                                                            ((x.IsCanceled == false || x.IsCanceled == null) &&
                                                            x.RefusedbyID == null &&
                                                            x.RefuseReson == null &&
                                                            x.ApprovedbyID == null) &&
                                                            x.ID == ActivityRequestId)
                                             .ToList()
                                             .Select(x => new
                                             {
                                                 x.ID,
                                                 x.Student_ID,
                                                 x.Degree,
                                                 x.Duration,
                                                 x.Location,
                                                 x.Type,
                                                 EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                                                 StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                                                 x.Name,
                                                 x.Ratio,
                                             }).SingleOrDefault();

            // المعدل التراكمي لدرجات الطالب
            var CumGpa = dbSch.ESOL_ACADEMIC_RECORDS
                              .Where(x => x.student_id == studentActivityRequest.Student_ID && x.semester_gpa != 0 && x.semester_gpa != null)
                              .OrderByDescending(x => x.semester).FirstOrDefault();


            //طلبات المشاركات التي تم إعتمادها وتخص هذا الطالب    
            var ActivitysApprovedRequests = dbSch.Usp_GetAllApprovedActivities_StudentId(studentActivityRequest?.Student_ID)
                                                 .ToList()
                                                 .Select(x => new
                                                 {
                                                     x.Type,
                                                     x.Name,
                                                     x.Ratio,
                                                     x.Status,
                                                     x.Degree,
                                                     x.Duration,
                                                     x.Location,
                                                     x.Student_ID,
                                                     AcademicActivitiesId = x.ID,
                                                     EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                                                     StartDate = x.StartDate.ToString("dd/MM/yyyy")
                                                 }).ToList();

            //مراحل إعتماد هذا الطلب     
            var ActivityApprovedPhases = dbSch.ActivityApprovedPhases
                                              .Where(x => x.ActivityRequested_ID == studentActivityRequest.ID)
                                              .Select(x => new
                                              {
                                                  x.Reason,
                                                  x.DashBoard_Users.Name,
                                                  x.ActivityPhases.PhaseName,
                                                  ResponseDate = x.ResponseDate.Day + "/" + x.ResponseDate.Month + "/" + x.ResponseDate.Year,
                                              }).ToList();

            // بيانات الطالب الأكاديمية
            var studentBasicData = dbSch.INTEGRATION_All_Students
                                        .FirstOrDefault(x => x.STUDENT_ID == studentActivityRequest.Student_ID);



            var NextApprovePhase = GetActivityRequestNextApprovePhase(ActivityRequestId) ?? new ActivityPhases();

            // عمل دمج بين بيانات الطالب والمعدل التراكمي
            var studentAcademicData = new
            {
                ACADEMIC_AVG = CumGpa?.cum_gpa,
                StudentName = studentBasicData.STUDENT_NAME + " - " + "ق.هـ" + " ( " + studentBasicData.NATIONAL_ID + " ) " + " - " + "ق.جـ" + " ( " + studentBasicData.STUDENT_ID + " ) ",
                studentBasicData.NATIONALITY_DESC,
                studentBasicData.FACULTY_NAME,
                studentBasicData.DEGREE_DESC,
                LEVEL_DESC = studentBasicData.LEVEL_DESC == null || studentBasicData.LEVEL_DESC == "" ? "غير متوفر" : studentBasicData.LEVEL_DESC,
                studentBasicData.STUDY_DESC,
                studentBasicData.STATUS_DESC,
            };

            return this.JsonMaxLength(new[]
            {
               new{

                   ActivityRequestId,
                   studentAcademicData,
                   studentActivityRequest,
                   ActivityApprovedPhases,
                   ActivitysApprovedRequests,
                   nextApprovePhase = new {
                       NextApprovePhase.Order,
                       NextApprovePhase.PhaseName,
                   }
                 }
            });
        }


        [HttpPost]
        public ActionResult ConfirmActivityRequest(int ActivityRequestId, string recommendationsNotes, string type)
        {

            var ActivityRequestInDB = dbSch.ActivityRequestsArchive.FirstOrDefault(x => x.ID == ActivityRequestId);
            if (ActivityRequestInDB == null)
            {
                return Content("لا يمكن إتمام عملية الحفظ نظرا لإرتباطه بسجلات أخري");
            }

            if (type == "R")
            {
                if (string.IsNullOrEmpty(recommendationsNotes))
                {
                    return Content("عفواادخل توصيات رفض الطلب");
                }

                ActivityRequestInDB.User_Id = CurrentUser.ID;
                ActivityRequestInDB.RefusedbyID = CurrentUser.ID;
                ActivityRequestInDB.RefuseReson = recommendationsNotes;
                dbSch.Entry(ActivityRequestInDB).State = System.Data.Entity.EntityState.Modified;


                var activityApprovedPhases = new ActivityApprovedPhases()
                {
                    Reason = recommendationsNotes,
                    ResponseDate = DateTime.Now,
                    UserID = CurrentUser.ID,
                    ApprovedStatus = false,
                    ActivityPhase_ID = GetActivityRequestNextApprovePhase(ActivityRequestInDB.ID).ID,
                    ActivityRequested_ID = ActivityRequestInDB.ID
                };

                dbSch.ActivityApprovedPhases.Add(activityApprovedPhases);
                dbSch.SaveChanges();
                return Json("");

            }
            var ActivityApprovedPhases = new ActivityApprovedPhases()
            {
                Reason = recommendationsNotes ?? "تم الإعتماد",
                ResponseDate = DateTime.Now,
                UserID = CurrentUser.ID,
                ApprovedStatus = true,
                ActivityPhase_ID = GetActivityRequestNextApprovePhase(ActivityRequestInDB.ID).ID,
                ActivityRequested_ID = ActivityRequestInDB.ID
            };

            dbSch.ActivityApprovedPhases.Add(ActivityApprovedPhases);
            dbSch.SaveChanges();

            //هل دي أخر مرحلة ولا إيه ؟
            if (GetActivityRequestNextApprovePhase(ActivityRequestInDB.ID)?.ActivityUsers == null)
            {
                ActivityRequestInDB.ApprovedbyID = CurrentUser.ID;
                ActivityRequestInDB.RefuseReson = recommendationsNotes;

                dbSch.Entry(ActivityRequestInDB).State = System.Data.Entity.EntityState.Modified;
            }

            dbSch.SaveChanges();
            return Json("");
        }


        public ActivityPhases GetActivityRequestNextApprovePhase(int ActivityRequestId)
        {
            var maxApprovedPhase = dbSch.ActivityApprovedPhases
                                        .Where(x => x.ActivityRequested_ID == ActivityRequestId)
                                        .ToList()
                                        .LastOrDefault();


            if (maxApprovedPhase != null)
            {
                var nextApprovePhaseOrder = dbSch.ActivityPhases
                                                 .SingleOrDefault(x => x.ID == maxApprovedPhase.ActivityPhase_ID).Order + 1;
                return dbSch.ActivityPhases
                            .SingleOrDefault(x => x.Order == nextApprovePhaseOrder);
            }

            return dbSch.ActivityPhases.Single(x => x.Order == 1);
        }



        [HttpPost]
        public ActionResult EditActivityRequest(int activityRequestId, string Degree, string Duration, string Location,
           string Type, DateTime StartDate, DateTime EndDate, string Name, Decimal Ratio)
        {
            var ActivityRequestInDB = dbSch.ActivityRequestsArchive.FirstOrDefault(x => x.ID == activityRequestId);
            if (ActivityRequestInDB == null)
            {
                return Content("لا يمكن التعديل نظرا لإرتباطه بسجلات أخري");
            }

            ActivityRequestInDB.Degree = Degree;
            ActivityRequestInDB.Ratio = Ratio;
            ActivityRequestInDB.Type = Type;
            ActivityRequestInDB.Name = Name;
            ActivityRequestInDB.Location = Location;
            ActivityRequestInDB.Duration = Duration;
            ActivityRequestInDB.StartDate = StartDate;
            ActivityRequestInDB.EndDate = EndDate;

            dbSch.Entry(ActivityRequestInDB).State = System.Data.Entity.EntityState.Modified;
            dbSch.SaveChanges();
            return Json("");

        }


        [HttpGet]
        public ActionResult GetActivityRequestById(int activityRequestId)
        {
            return this.JsonMaxLength(dbSch.ActivityRequestsArchive
                                           .Where(x => x.ID == activityRequestId &&
                                                       (x.IsCanceled == false || x.IsCanceled == null) &&
                                                       x.RefusedbyID == null &&
                                                       x.RefuseReson == null &&
                                                       x.ApprovedbyID == null)
                                            .ToList().Select(p => new
                                            {
                                                AcademicActivitiesId = p.ID,
                                                Student_Id = p.Student_ID,
                                                Degree = p.Degree,
                                                Duration = p.Duration,
                                                Location = p.Location,
                                                Type = p.Type,
                                                EndDate = p.EndDate.ToString("MM/dd/yyyy"),
                                                StartDate = p.StartDate.ToString("MM/dd/yyyy"),
                                                Name = p.Name,
                                                Ratio = p.Ratio,
                                            })
                                           .FirstOrDefault());
        }

        //[HttpPost]
        //public ActionResult RefuseActivityRequest(int activityRequestId, string recommendationsOfRefuse)
        //{
        //    if (string.IsNullOrEmpty(recommendationsOfRefuse))
        //    {
        //        return Content("عفواادخل توصيات رفض المشاركة");
        //    }

        //    var db = new SchoolAccGam3aEntities();
        //    var ActivityRequestInDB = db.ActivityRequestsArchive.SingleOrDefault(x => x.ID == activityRequestId);
        //    if (ActivityRequestInDB == null)
        //    {
        //        return Content("لا يمكن إتمام عملية الحفظ نظرا لإرتباطه بسجلات أخري");
        //    }

        //    ActivityRequestInDB.RefusedbyID = CurrentUser.ID;
        //    ActivityRequestInDB.RefuseReson = recommendationsOfRefuse;
        //    db.Entry(ActivityRequestInDB).State = System.Data.Entity.EntityState.Modified;


        //    var activityApprovedPhases = new ActivityApprovedPhases()
        //    {
        //        Reason = recommendationsOfRefuse,
        //        ResponseDate = DateTime.Now,
        //        UserID = CurrentUser.ID,
        //        ApprovedStatus = false,
        //        ActivityPhase_ID = GetActivityRequestNextApprovePhase(ActivityRequestInDB.ID).ID,
        //        ActivityRequested_ID = ActivityRequestInDB.ID
        //    };

        //    db.ActivityApprovedPhases.Add(activityApprovedPhases);
        //    db.SaveChanges();
        //    return Json("");

        //}


        //[HttpPost]
        //public ActionResult ApproveActivityRequest(int activityRequestId, string recommendationsOfAccept)
        //{

        //    var db = new SchoolAccGam3aEntities();
        //    var ActivityRequestInDB = db.ActivityRequestsArchive.SingleOrDefault(x => x.ID == activityRequestId);
        //    if (ActivityRequestInDB == null)
        //    {
        //        return Content("لا يمكن إتمام عملية الحفظ نظرا لإرتباطه بسجلات أخري");
        //    }

        //    var activityApprovedPhases = new ActivityApprovedPhases()
        //    {
        //        Reason = recommendationsOfAccept ?? "تم الإعتماد",
        //        ResponseDate = DateTime.Now,
        //        UserID = CurrentUser.ID,
        //        ApprovedStatus = true,
        //        ActivityPhase_ID = GetActivityRequestNextApprovePhase(ActivityRequestInDB.ID).ID,
        //        ActivityRequested_ID = ActivityRequestInDB.ID
        //    };
        //    db.ActivityApprovedPhases.Add(activityApprovedPhases);
        //    db.SaveChanges();

        //    if (GetActivityRequestNextApprovePhaseCustom(ActivityRequestInDB.ID)?.ActivityUsers == null)
        //    {
        //        ActivityRequestInDB.ApprovedbyID = CurrentUser.ID;
        //        db.Entry(ActivityRequestInDB).State = System.Data.Entity.EntityState.Modified;
        //    }

        //    db.SaveChanges();
        //    return Json("");
        //}


        //public ActivityPhases GetActivityRequestNextApprovePhaseCustom(int activityRequestId)
        //{
        //    var db = new SchoolAccGam3aEntities();
        //    var maxApprovedPhase = db.ActivityApprovedPhases
        //                             .Where(x => x.ActivityRequested_ID == activityRequestId)
        //                             .ToList().LastOrDefault();

        //    if (maxApprovedPhase != null)
        //    {
        //        var ApprovePhaseOrder = db.ActivityPhases.SingleOrDefault(x => x.ID == maxApprovedPhase.ActivityPhase_ID).Order + 1;
        //        return db.ActivityPhases.SingleOrDefault(x => x.Order == ApprovePhaseOrder);
        //    }

        //    // Exception 
        //    return db.ActivityPhases.Single(x => x.Order == 1);
        //}


        //public ActivityPhases GetActivityRequestNextApprovePhase(int activityRequestId)
        //{
        //    var db = new SchoolAccGam3aEntities();
        //    var maxApprovedPhase = db.ActivityApprovedPhases
        //                             .Where(x => x.ActivityRequested_ID == activityRequestId)
        //                             .ToList().LastOrDefault();

        //    if (maxApprovedPhase != null)
        //    {
        //        var nextApprovePhaseOrder = db.ActivityPhases.SingleOrDefault(x => x.ID == maxApprovedPhase.ActivityPhase_ID).Order + 1;
        //        return db.ActivityPhases.SingleOrDefault(x => x.Order == nextApprovePhaseOrder);
        //    }

        //    return db.ActivityPhases.Single(x => x.Order == 1);
        //}


        //[HttpGet]
        //public ActionResult GetActivitiesRequestsByUserId()
        //{
        //    // مراحل الموافقة التي يقع فيها هذا المستخدم
        //    var userAuthorizedPhases = dbSch.ActivityUsers.Where(x => x.UserID == CurrentUser.ID && x.IsActive == true)
        //                                                  .Select(x => x.ActivityPhases.Order).ToList();

        //    var activityRequests = dbSch.ActivityRequestsArchive.Where(x =>
        //                                                                   ((x.IsCanceled == false || x.IsCanceled == null) &&
        //                                                                     x.RefusedbyID == null && x.RefuseReson == null &&
        //                                                                     x.ApprovedbyID == null) &&
        //                                                                     userAuthorizedPhases.Any(p => p == x.ActivityApprovedPhases.Count() + 1)
        //                                                                   );

        //    var students = dbSch.INTEGRATION_All_Students.Where(x => activityRequests.Any(p => p.Student_ID == x.STUDENT_ID));

        //    return this.JsonMaxLength(activityRequests.Select(t => new
        //    {
        //        ActivityRequestId = t.ID,
        //        ActivityName = t.Name,
        //        t.Student_ID,
        //        t.Duration,
        //        t.Location,
        //        t.Type,
        //        t.Degree,
        //        t.Ratio,
        //        StartDate = t.StartDate.Month + "/" + t.StartDate.Day + "/" + t.StartDate.Year,
        //        EndDate = t.EndDate.Month + "/" + t.EndDate.Day + "/" + t.EndDate.Year,
        //        InsertionDate = t.InsertionDate.Month + "/" + t.InsertionDate.Day + "/" + t.InsertionDate.Year,
        //        StudentAcademicData = students.Where(x => x.STUDENT_ID == t.Student_ID).Select(x => new
        //        {
        //            StudentName = x.STUDENT_NAME + " - " + "ق.هـ" + " ( " + x.NATIONAL_ID + " ) " + " - " + "ق.جـ" + " ( " + x.STUDENT_ID + " ) ",
        //            NationalityName = x.NATIONALITY_DESC,
        //            MobileNumber = x.MOBILE_PHONE,
        //            FacultiyName = x.FACULTY_NAME,
        //            BirthCityName = x.BIRTH_CITY_TEXT,
        //            BirthDate = x.BIRTH_DATE,
        //            DegreeName = x.DEGREE_DESC,
        //            LevelName = x.LEVEL_DESC,
        //            StudyTypeName = x.STUDY_DESC,
        //            StatusName = x.STATUS_DESC,
        //            GenderName = x.GENDER == 1 ? "ذكر" : x.GENDER == 2 ? "أنثي" : "غير متوفر"
        //        }).FirstOrDefault()

        //    }).ToList());
        //}




        //[HttpGet]
        //public ActionResult GetAllActivitiesRequests()
        //{
        //    return this.JsonMaxLength(dbSch.ActivityRequestsArchive
        //                                   .Where(x => x.User_Id == CurrentUser.ID).ToList()
        //                                   .Select(p => new
        //                                   {
        //                                       AcademicActivitiesId = p.ID,
        //                                       Student_Id = p.Student_ID,
        //                                       Degree = p.Degree,
        //                                       Duration = p.Duration,
        //                                       Location = p.Location,
        //                                       Type = p.Type,
        //                                       EndDate = p.EndDate.ToString("dd/MM/yyyy"),
        //                                       StartDate = p.StartDate.ToString("dd/MM/yyyy"),
        //                                       Name = p.Name,
        //                                       Ratio = p.Ratio,
        //                                       Status = "جاري التدقيق",
        //                                       RefusalReason = ""
        //                                   }).ToList());
        //}

        #endregion

        #endregion

        #region المشاركات الجديدة

        #region شاشة تهيئة قوائم المشاركات
        [HttpGet]
        public ActionResult ActivityMenusConfig()
        {
            var json = new JavaScriptSerializer().Serialize(GetCreateActivityMenusPermissions(99).Data);
            var permissions = JsonConvert.DeserializeObject<CreateActivityMenusPermissions>(json);
            if (permissions == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Search = permissions.Search;
            ViewBag.CreateActivityMenu = permissions.CreateActivityMenu;
            ViewBag.CreateActivityMenusWithPost = permissions.CreateActivityMenusWithPost;

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }
        #endregion

        #region شاشة إضافة قوائم مشاركات الطلاب

        #region Permissions

        [HttpGet]
        public JsonResult GetCreateActivityMenusPermissions(int screenId)
        {
            if (CurrentUser == null || CurrentUser?.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new CreateActivityMenusPermissions();
            foreach (var permission in perm)
            {
                if (permission == "عرض")
                {
                    permissions.View = true;
                }
                if (permission == "بحث قوائم المشاركات")
                {
                    permissions.Search = true;
                }
                if (permission == "إضافة قائمة مشاركة")
                {
                    permissions.CreateActivityMenu = true;
                }
                if (permission == "إضافة قائمة مشاركة مع توجيه للاعتماد")
                {
                    permissions.CreateActivityMenusWithPost = true;
                }
                if (permission == "إضافة تقييمات")
                {
                    permissions.AddEvaluation = true;
                }
                if (permission == "حذف التقييمات")
                {
                    permissions.DeleteEvaluation = true;
                }
                if (permission == "تهيأة مشاركات جديدة")
                {
                    permissions.AddActivityMaster = true;
                }
                if (permission == "حذف المشاركات المهيأة")
                {
                    permissions.DeleteActivityMaster = true;
                }
                if (permission == "إضافة أسماء مشاركات")
                {
                    permissions.AddActivityName = true;
                }
                if (permission == "حذف أسماء المشاركات")
                {
                    permissions.DeleteActivityName = true;
                }
                if (permission == "إضافة أنواع مشاركات")
                {
                    permissions.AddActivityType = true;
                }
                if (permission == "حذف أنواع المشاركات")
                {
                    permissions.DeleteActivityType = true;
                }
                if (permission == "إضافة أماكن المشاركات")
                {
                    permissions.AddActivityLocation = true;
                }
                if (permission == "حذف أماكن المشاركات")
                {
                    permissions.DeleteActivityLocation = true;
                }
            }

            return this.JsonMaxLength(permissions);
        }

        public class CreateActivityMenusPermissions
        {
            public bool View { get; set; }
            public bool Search { get; set; }
            public bool CreateActivityMenu { get; set; }
            public bool CreateActivityMenusWithPost { get; set; }
            public bool AddEvaluation { get; set; }
            public bool DeleteEvaluation { get; set; }
            public bool AddActivityMaster { get; set; }
            public bool DeleteActivityMaster { get; set; }
            public bool AddActivityName { get; set; }
            public bool DeleteActivityName { get; set; }
            public bool AddActivityType { get; set; }
            public bool DeleteActivityType { get; set; }
            public bool AddActivityLocation { get; set; }
            public bool DeleteActivityLocation { get; set; }
        }

        #endregion

        [HttpGet]
        public ActionResult CreateActivityMenus()
        {
            var json = new JavaScriptSerializer().Serialize(GetCreateActivityMenusPermissions(99).Data);
            var permissions = JsonConvert.DeserializeObject<CreateActivityMenusPermissions>(json);
            if (permissions == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Search = permissions.Search;
            ViewBag.CreateActivityMenu = permissions.CreateActivityMenu;
            ViewBag.CreateActivityMenusWithPost = permissions.CreateActivityMenusWithPost;

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        [HttpGet]
        public ActionResult GetFaculties()
        {
            return this.JsonMaxLength(dbSch.usp_getFaculties()
                                           .Select(selector: x => new SelectListItem
                                           {
                                               Text = x.FACULTY_NAME,
                                               Value = x.FACULTY_NO.ToString()
                                           }).Distinct().ToList());
        }

        [HttpGet]
        public ActionResult GetDegrees()
        {
            return this.JsonMaxLength(dbSch.usp_getDegrees()
                                           .Select(selector: x => new SelectListItem
                                           {
                                               Text = x.DEGREE_DESC,
                                               Value = x.DEGREE_CODE.ToString()
                                           })
                                          .OrderBy(x => x.Text).ToList());
        }

        [HttpGet]
        public ActionResult GetStudents(string degreeId, string facultyId)
        {
            return this.JsonMaxLength(dbSch.SP_GetCurrentandLatestGraduatedStudents(facultyId, degreeId, (DateTime.Now.Year).ToString())
                                           .Select(x => new
                                           {
                                               Text = x.STUDENT_NAME,
                                               Value = x.STUDENT_ID,
                                               NationalID = x.NATIONAL_ID
                                           }).ToList());
        }

        [HttpGet]
        public ActionResult GetActivityConfig(string paramKey)
        {
            return this.JsonMaxLength(dbSch.ActivityConfig
                                            .Where(x => x.Key == paramKey)
                                            .Select(x => new SelectListItem
                                            {
                                                Text = x.Name,
                                                Value = x.ID.ToString(),
                                            }).ToList());
        }

        [HttpGet]
        public ActionResult GetConfigerdActivityNames()
        {
            return this.JsonMaxLength(dbSch.ActivityConfig
                                           .Where(x => x.ActivityRequestMaster2.Count > 0)
                                           .Select(x => new SelectListItem
                                           {
                                                Text = x.Name,
                                                Value = x.ID.ToString()
                                           }).ToList());
        }

        [HttpGet]
        public ActionResult GetActivityNumbers(int paramConfigId)
        {
            return this.JsonMaxLength(dbSch.Usp_GetAvailableActivityNumbers(paramConfigId).ToList());
        }

        [HttpGet]
        public ActionResult GetActivityConfigDataSource(string paramKey)
        {
            return this.JsonMaxLength(dbSch.ActivityConfig
                                           .Where(x => x.Key == paramKey)
                                           .Select(x => new
                                           {
                                               x.Name,
                                               ConfigId = x.ID,
                                               Notes = paramKey == "evaluation" ? x.Notes + "%" : x.Notes
                                           }).ToList());
        }

        [HttpPost]
        public ActionResult SaveActivityConfig(int? paramId, string paramKey, string paramName, string paramNotes)
        {
            if (paramId != null)
            {
                var model = dbSch.ActivityConfig.FirstOrDefault(x => x.ID == paramId);
                if (model != null)
                {
                    model.Name = paramName;
                    model.Notes = paramNotes;
                    dbSch.Entry(model).State = EntityState.Modified;
                    dbSch.SaveChanges();

                    return this.JsonMaxLength("");
                }
                return this.JsonMaxLength("حدث خطأ برجاء عمل تحديث للصفحة");
            }
            else
            {
                var model = dbSch.ActivityConfig.FirstOrDefault(x => x.Key == paramKey && x.Name == paramName);
                if (model == null)
                {

                    dbSch.ActivityConfig.Add(new ActivityConfig
                    {
                        Key = paramKey,
                        Name = paramName,
                        User_ID = CurrentUser.ID,
                        Notes = paramNotes,
                        InsertionDate = DateTime.Now
                    });
                    dbSch.SaveChanges();

                    return this.JsonMaxLength("");
                }
                return this.JsonMaxLength("لا يمكن إتمام عملية الحفظ حيث أن هذا الإسم مسجل من قبل");
            }
        }

        [HttpGet]
        public ActionResult GetNextActivityNo(int paramConfigId)
        {

            var NextActivityNo = dbSch.ActivityRequestMaster
                                       .Where(x => x.Activity_ID == paramConfigId)
                                       .Select(x => x.ActivityNo).ToList()
                                       .Select(x => int.Parse(x)).ToList().DefaultIfEmpty(0).Max() + 1;
            return this.JsonMaxLength("(" + NextActivityNo + ")" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day);
        }


        [HttpPost]
        public ActionResult SaveActivityMaster(int paramHoursNo,
                                               int paramActivityTypeId,
                                               int paramActivityNameId,
                                               int? paramActivityMasterId,
                                               int paramActivityLocationId,
                                               DateTime paramActivityEndDate,
                                               DateTime paramActivityStartDate)
        {
            if (paramActivityMasterId != null)
            {
                var model = dbSch.ActivityRequestMaster.FirstOrDefault(x => x.ID == paramActivityMasterId);
                if (model != null)
                {
                    model.HoursNo = paramHoursNo;
                    model.Type_ID = paramActivityTypeId;
                    model.EndDate = paramActivityEndDate;
                    model.Activity_ID = paramActivityNameId;
                    model.StartDate = paramActivityStartDate;
                    model.Location_ID = paramActivityLocationId;

                    dbSch.Entry(model).State = EntityState.Modified;
                    dbSch.SaveChanges();

                    return this.JsonMaxLength("");
                }
                return this.JsonMaxLength("حدث خطأ برجاء عمل تحديث للصفحة");
            }
            else
            {
                var NextActivityNo = dbSch.ActivityRequestMaster
                                          .Where(x => x.Activity_ID == paramActivityNameId)
                                          .Select(x => x.ActivityNo).ToList()
                                          .Select(x => int.Parse(x)).ToList().DefaultIfEmpty(0).Max() + 1;

                dbSch.ActivityRequestMaster.Add(new ActivityRequestMaster
                {
                    //خد بالك من دي
                    IsPosted = null,
                    IsReturned = null,

                    User_ID = CurrentUser.ID,
                    HoursNo = paramHoursNo,
                    InsertionDate = DateTime.Now,
                    Type_ID = paramActivityTypeId,
                    EndDate = paramActivityEndDate,
                    Activity_ID = paramActivityNameId,
                    StartDate = paramActivityStartDate,
                    YearName = paramActivityEndDate.Year,
                    Location_ID = paramActivityLocationId,
                    ActivityNo = NextActivityNo.ToString()
                });
                dbSch.SaveChanges();

                return this.JsonMaxLength("");

            }
        }

        [HttpGet]
        public ActionResult GetActivityMasterDataSource()
        {
            return this.JsonMaxLength(dbSch.ActivityRequestMaster
                                           .Select(x => new
                                           {
                                               x.ID,
                                               x.HoursNo,
                                               ActivityType = x.ActivityConfig.Name,
                                               ActivityName = x.ActivityConfig2.Name,
                                               ActivityLocation = x.ActivityConfig1.Name,
                                               EndDate = x.EndDate.Day + "/" + x.EndDate.Month + "/" + x.EndDate.Year,
                                               StartDate = x.StartDate.Day + "/" + x.StartDate.Month + "/" + x.StartDate.Year,
                                               ActivityNoFormated = "(" + x.ActivityNo + ")" + x.InsertionDate.Year + "-" + x.InsertionDate.Month + "-" + x.InsertionDate.Day,
                                           }).ToList());
        }

        [HttpGet]
        public ActionResult GetActivityMasterById(int paramActivityMasterId)
        {
            var model = dbSch.ActivityRequestMaster.FirstOrDefault(x => x.ID == paramActivityMasterId);
            if (model == null)
            {
                return this.JsonMaxLength("حدث خطأ برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري");
            }

            return this.JsonMaxLength(new
            {
                model.ID,
                model.HoursNo,
                ActivityTypeID = model.ActivityConfig.ID,
                ActivityNameID = model.ActivityConfig2.ID,
                ActivityName = model.ActivityConfig2.Name,
                ActivityTypeName = model.ActivityConfig.Name,
                ActivityLocationID = model.ActivityConfig1.ID,
                EndDate = model.EndDate.Year + "/" + model.EndDate.Month + "/" + model.EndDate.Day,
                StartDate = model.StartDate.Year + "/" + model.StartDate.Month + "/" + model.StartDate.Day,
                ActivityNoFormated = "(" + model.ActivityNo + ")" + model.InsertionDate.Year + "-" + model.InsertionDate.Month + "-" + model.InsertionDate.Day
            });
        }

        [HttpGet]
        public ActionResult GetActivityConfigById(int paramActivityConfigId)
        {
            var model = dbSch.ActivityConfig.FirstOrDefault(x => x.ID == paramActivityConfigId);
            if (model == null)
            {
                return this.JsonMaxLength("حدث خطأ برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري");
            }

            return this.JsonMaxLength(new
            {
                model.ID,
                model.Name,
                model.Notes
            });
        }

        [HttpPost]
        public ActionResult SaveActivityDetails(bool paramIsPosted, int paramActivityMasterId, List<ParamActivityMasterVm> paramActivityMaster)
        {
            if (CurrentUser.ID == 0)
            {
                RedirectToAction("Login", "Login");
            }

            if (paramActivityMaster.Count == 0)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم إضافة طالب واحد علي الأقل بداخل قائمة المشاركات");
            }

            var file = (HttpPostedFileBase)Session["ActivityRequestMasterFile"];
            if (file == null)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع المرفق");
            }

            var returnedAvtivityMaster = dbSch.ActivityRequestMaster.FirstOrDefault(x => x.ID == paramActivityMasterId);
            if (returnedAvtivityMaster == null)
            {
                return Content("حدث خطأ برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري");
            }

            paramActivityMaster.ForEach(x =>
            {
                dbSch.ActivityRequestDetails.Add(

                    new ActivityRequestDetails
                    {
                        User_ID = CurrentUser.ID,
                        Student_ID = x.StudentId,
                        Degree_ID = x.EvaluationId,
                        InsertionDate = DateTime.Now,
                        ActivityRequestMaster_ID = returnedAvtivityMaster.ID
                    });
            });

            //خد بالك من دي
            returnedAvtivityMaster.IsPosted = paramIsPosted;
            dbSch.Entry(returnedAvtivityMaster).State = EntityState.Modified;

            dbSch.ActivityRequestMasterAttachments.Add(new ActivityRequestMasterAttachments
            {
                User_ID = CurrentUser.ID,
                FileName = file.FileName,
                InsertionDate = DateTime.Now,
                ActivityRequestMaster_ID = returnedAvtivityMaster.ID,
                FilePath = GetFilePath(file, "ActivityRequestMaster", returnedAvtivityMaster.ID.ToString())
            });

            Session["ActivityRequestMasterFile"] = null;
            dbSch.SaveChanges();

            return this.JsonMaxLength("");
        }


        public ActionResult EditActivityDetails(bool paramIsPosted, int paramActivityMasterId, List<ParamActivityMasterVm> paramActivityMaster, int paramRemovedFile)
        {
            if (CurrentUser.ID == 0)
            {
                RedirectToAction("Login", "Login");
            }

            if (paramActivityMaster.Count == 0)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم إضافة طالب واحد علي الأقل بداخل قائمة المشاركات");
            }

            var returnedAvtivityMaster = dbSch.ActivityRequestMaster.FirstOrDefault(x => x.ID == paramActivityMasterId);
            if (returnedAvtivityMaster == null)
            {
                return Content("حدث خطأ برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري");
            }


            var file = (HttpPostedFileBase)Session["ActivityRequestMasterFile"];

            if (file == null &&
                returnedAvtivityMaster.ActivityRequestMasterAttachments.Count() == 0)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع المرفق");
            }

            if (file == null &&
                returnedAvtivityMaster.ActivityRequestMasterAttachments.Count() > 0 && paramRemovedFile > 0)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع المرفق");
            }

            dbSch.ActivityRequestDetails
                 .RemoveRange(dbSch.ActivityRequestDetails
                                   .Where(x => x.ActivityRequestMaster_ID == paramActivityMasterId).ToList());


            if (paramRemovedFile > 0)
            {
                var attachment = dbSch.ActivityRequestMasterAttachments
                                      .FirstOrDefault(x => x.ID == paramRemovedFile);

                if (attachment != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(attachment.FilePath)))
                    {
                        System.IO.File.SetAttributes(Server.MapPath(attachment.FilePath), FileAttributes.Normal);

                        System.IO.File.Delete(Server.MapPath(attachment.FilePath));
                    }

                    dbSch.ActivityRequestMasterAttachments.Remove(attachment);
                }
                dbSch.ActivityRequestMasterAttachments.Remove(attachment);
            }
            try
            {
                dbSch.SaveChanges();
            }
            catch (Exception)
            {
                return Content("عفوا قائمة المشاركة مرتبط بسجلات اخرى و لا يمكن التعديل عليها");
            }

            //خد بالك من دي
            returnedAvtivityMaster.IsPosted = paramIsPosted;
            dbSch.Entry(returnedAvtivityMaster).State = EntityState.Modified;

            paramActivityMaster.ForEach(x =>
            {
                dbSch.ActivityRequestDetails.Add(

                    new ActivityRequestDetails
                    {
                        User_ID = CurrentUser.ID,
                        Student_ID = x.StudentId,
                        Degree_ID = x.EvaluationId,
                        InsertionDate = DateTime.Now,
                        ActivityRequestMaster_ID = returnedAvtivityMaster.ID
                    });
            });

            if (file != null)
            {
                dbSch.ActivityRequestMasterAttachments.Add(new ActivityRequestMasterAttachments
                {
                    User_ID = CurrentUser.ID,
                    FileName = file.FileName,
                    InsertionDate = DateTime.Now,
                    ActivityRequestMaster_ID = returnedAvtivityMaster.ID,
                    FilePath = GetFilePath(file, "ActivityRequestMaster", returnedAvtivityMaster.ID.ToString())
                });

                Session["ActivityRequestMasterFile"] = null;
            }


            dbSch.SaveChanges();

            return this.JsonMaxLength("");
        }


        [HttpDelete]
        public ActionResult DeleteActivityMaster(int paramActivityMasterId)
        {
            var model = dbSch.ActivityRequestMaster.Where(x => x.ID == paramActivityMasterId).FirstOrDefault();
            if (model == null)
            {
                return Json("لايمكن إتمام عملية الحذف برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري", JsonRequestBehavior.AllowGet);
            }
            if (model.ActivityRequestDetails.Count > 1)
            {
                return Json("لايمكن إتمام عملية الحذف حيث أن هذه المشاركة مرتبطة بسجلات أخري", JsonRequestBehavior.AllowGet);
            }

            dbSch.ActivityRequestMaster.Remove(model);
            try
            {
                dbSch.SaveChanges();
            }
            catch (Exception)
            {
                return Json("لايمكن حذف هذه المشاركة لإرتباطها بسجلات أخري", JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        [HttpDelete]
        public ActionResult DeleteActivityConfig(int paramActivityConfigId)
        {
            var model = dbSch.ActivityConfig.Where(x => x.ID == paramActivityConfigId).FirstOrDefault();
            if (model == null)
            {
                return Json("لايمكن إتمام عملية الحذف برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري", JsonRequestBehavior.AllowGet);
            }

            dbSch.ActivityConfig.Remove(model);
            try
            {
                dbSch.SaveChanges();
            }
            catch (Exception)
            {
                return Json("لايمكن حذف هذا البند لارتباطه بسجلات أخري", JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        #region FileUploader

        public string GetFilePath(HttpPostedFileBase file, string folderName, string subFolderName)
        {

            // file
            var rootFolder = Server.MapPath("~/Content/AcademicActivitiesFiles/");
            var targetFolder = Path.Combine(rootFolder, folderName);
            var currentFolder = new DirectoryInfo(targetFolder);

            if (currentFolder.Exists == false)
            {
                Directory.CreateDirectory(targetFolder);
            }

            var dSecurity = currentFolder.GetAccessControl();


            dSecurity.AddAccessRule(new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

            currentFolder.SetAccessControl(dSecurity);


            var subFolder = Path.Combine(rootFolder, folderName, subFolderName);

            if (!new DirectoryInfo(subFolder).Exists)
            {
                Directory.CreateDirectory(subFolder);
            }


            var dSecurityForsubFolder = new DirectoryInfo(subFolder).GetAccessControl();

            dSecurityForsubFolder.AddAccessRule(new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

            new DirectoryInfo(subFolder).SetAccessControl(dSecurityForsubFolder);


            // Start From Here

            string filename = String.Format("{0}_{1}.{2}", file.FileName.Split('.')[0],
                DateTime.Now.ToString("dd-MM-yyyy-HH-mm"), file.FileName.Split('.')[1]);

            string path = Path.Combine(subFolder, filename);


            Stream stream = file.InputStream;
            byte[] bytes = ReadToEnd(stream);
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();

            return MapPathReverse(path);
        }

        public string MapPathReverse(string fullServerPath)
        {
            return @"~\" + fullServerPath.Replace(Request.PhysicalApplicationPath, string.Empty);
        }

        [HttpPost]
        public ActionResult UploadActivityMasterFile()
        {
            Session["ActivityRequestMasterFile"] = null;
            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentType != "application/pdf")
                {
                    return new HttpStatusCodeResult(400);
                }
                else
                {
                    Session["ActivityRequestMasterFile"] = file;
                }
            }
            return Json(0);
        }

        [HttpGet]
        public ActionResult DownloadActivityMasterAttachment(int activityRequestMasterId)
        {
            var FileVirtualPath = dbSch.ActivityRequestMasterAttachments.Where(x => x.ActivityRequestMaster_ID == activityRequestMasterId).FirstOrDefault()?.FilePath;
            if (FileVirtualPath != null)
            {
                if (!System.IO.File.Exists(HostingEnvironment.MapPath(FileVirtualPath)))
                {
                    return Content("");
                }

                byte[] FileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(FileVirtualPath));
                return File(FileBytes, "application/pdf");

                //===============================================================================================
                // ** File As DownLoad ** ...
                // return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
                //===============================================================================================
            }
            return Content("");
        }

        public ActionResult UpdateActivityMenu(int paramMasterId)
        {
            Session["ActivityRequestMasterFile"] = null;
            var json = new JavaScriptSerializer().Serialize(GetCreateActivityMenusPermissions(99).Data);
            var permissions = JsonConvert.DeserializeObject<CreateActivityMenusPermissions>(json);
            if (permissions == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Search = permissions.Search;
            ViewBag.CreateActivityMenu = permissions.CreateActivityMenu;
            ViewBag.CreateActivityMenusWithPost = permissions.CreateActivityMenusWithPost;

            if (permissions.View)
            {
                return RedirectToAction("CreateActivityMenus", new { MasterIdBeEditted = paramMasterId });
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        [HttpGet]
        public ActionResult GetEdittedActivityMenu(int paramActivityMasterId)
        {
            var model = dbSch.ActivityRequestMaster.FirstOrDefault(x => x.ID == paramActivityMasterId);
            if (model == null)
            {
                return this.JsonMaxLength("حدث خطأ برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري");
            }

            var Query = (
                  from RequestDetails in dbSch.ActivityRequestDetails
                  join Students in dbSch.INTEGRATION_All_Students
                  on RequestDetails.Student_ID equals Students.STUDENT_ID
                  where RequestDetails.ActivityRequestMaster_ID == paramActivityMasterId
                  select new
                  {
                      DegreeId = Students.DEGREE_CODE + "",
                      FacultyId = Students.FACULTY_NO + "",
                      StudentId = Students.STUDENT_ID,
                      DegreeName = Students.DEGREE_DESC,
                      FacultyName = Students.FACULTY_NAME,
                      StudentName = Students.STUDENT_NAME,
                      EvaluationId = RequestDetails.ActivityConfig.ID + "",
                      EvaluationName = RequestDetails.ActivityConfig.Name
                  }
               );

            return this.JsonMaxLength(
                new
                {
                    ActivityRequestMasterId = model.ID,
                    ActivityMenuBandList = Query.ToList(),
                    ActivityConfigId = model.ActivityConfig2.ID,
                    FilePath = model.ActivityRequestMasterAttachments.Select(x => new
                    {
                        x.ID,
                        x.FilePath,
                        x.FileName,
                        x.ActivityRequestMaster_ID
                    }).FirstOrDefault(),
                    InsertionDate = model.InsertionDate.Year + "/" + model.InsertionDate.Month + "/" + model.InsertionDate.Day,
                    ActivityNoFormated = "(" + model.ActivityNo + ")" + model.InsertionDate.Year + "-" + model.InsertionDate.Month + "-" + model.InsertionDate.Day

                });

        }

        #endregion

        public class ParamActivityMasterVm
        {
            public int StudentId { get; set; }
            public int EvaluationId { get; set; }
        }
        #endregion

        #region شاشة بحث قوائم مشاركات الطلاب

        #region Permissions

        [HttpGet]
        public JsonResult GetSearchActivityMenusPermissions(int screenId)
        {
            if (CurrentUser == null || CurrentUser?.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new SearchActivityMenusPermissions();
            foreach (var permission in perm)
            {
                if (permission == "عرض")
                {
                    permissions.View = true;
                }
                if (permission == "تعديل قائمة المشاركة")
                {
                    permissions.Edit = true;
                }
                if (permission == "حذف قائمة المشاركة")
                {
                    permissions.Delete = true;
                }
                if (permission == "ترحيل للاعتماد")
                {
                    permissions.Post = true;
                }
            }

            return this.JsonMaxLength(permissions);
        }

        public class SearchActivityMenusPermissions
        {
            public bool View { get; set; }
            public bool Edit { get; set; }
            public bool Post { get; set; }
            public bool Delete { get; set; }
        }

        #endregion

        [HttpGet]
        public ActionResult SearchActivityMenus()
        {
            var json = new JavaScriptSerializer().Serialize(GetCreateActivityMenusPermissions(100).Data);
            var permissions = JsonConvert.DeserializeObject<CreateActivityMenusPermissions>(json);
            if (permissions == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        [HttpGet]
        public ActionResult GetActivityMenusDataSource()
        {
            return this.JsonMaxLength(dbSch.ActivityRequestMaster
                                           .Where(x => x.ActivityRequestDetails.Count > 0)
                                           .Select(x => new
                                           {
                                               x.ID,
                                               x.IsPosted,
                                               x.IsReturned,
                                               x.StatusNotes,
                                               StatusCode = x.ApprovedStatus,
                                               ActivityName = x.ActivityConfig2.Name,
                                               ActivityLocation = x.ActivityConfig1.Name,
                                               StudentsCount = x.ActivityRequestDetails.Count(),
                                               InsertionDate = x.InsertionDate.Year + "/" + x.InsertionDate.Month + "/" + x.InsertionDate.Day,
                                               ActivityNoFormated = "(" + x.ActivityNo + ")" + x.InsertionDate.Year + "-" + x.InsertionDate.Month + "-" + x.InsertionDate.Day,
                                               ApprovedStatus = x.ApprovedStatus == true ? "معتمدة" :
                                                                x.IsPosted == false && x.IsReturned == true && x.ApprovedStatus == false ? "مرفوضة" :
                                                                x.IsPosted == true && x.IsReturned == null && x.ApprovedStatus == null ? "تحت المراجعة" :
                                                                x.IsPosted == false && x.IsReturned == null && x.ApprovedStatus == null ? "لم ترسل للاعتماد" :
                                                                x.IsPosted == true && x.IsReturned == true && x.ApprovedStatus == false ? "مرفوضة وتم توجيهها للإعتماد مرة أخري" : ""

                                           }).ToList());
        }

        [HttpGet]
        public ActionResult GetActivityRequestDetails(int paramActivityMasterId)
        {
            return this.JsonMaxLength(
                         (
                           from RequestDetails in dbSch.ActivityRequestDetails
                           join Students in dbSch.INTEGRATION_All_Students
                           on RequestDetails.Student_ID equals Students.STUDENT_ID
                           where RequestDetails.ActivityRequestMaster_ID == paramActivityMasterId
                           select new
                           {
                               EvaluationName = RequestDetails.ActivityConfig.Name,
                               Student_Id = RequestDetails.Student_ID,
                               RequestDetailsId = RequestDetails.ID,
                               StudentName = Students.STUDENT_NAME,
                               NationalId = Students.NATIONAL_ID
                           }
                        ).ToList());

        }


        [HttpPost]
        public ActionResult PostActivityMenu(int paramActivityMasterId)
        {
            var activityRequestMaster = dbSch.ActivityRequestMaster.FirstOrDefault(x => x.ID == paramActivityMasterId);
            if (activityRequestMaster != null)
            {
                activityRequestMaster.IsPosted = true;
                dbSch.Entry(activityRequestMaster).State = System.Data.Entity.EntityState.Modified;
                dbSch.SaveChanges();
                return Content("");
            }
            return Content("برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري");
        }


        [HttpDelete]
        public ActionResult DeleteActivityMenu(int paramActivityMasterId)
        {
            var model = dbSch.ActivityRequestMaster.Where(x => x.ID == paramActivityMasterId).FirstOrDefault();
            if (model == null)
            {
                return Json("لايمكن إتمام عملية الحذف برجاء عمل تحديث للصفحة وإعادة المحاولة مرة أخري", JsonRequestBehavior.AllowGet);
            }
            if (model.ApprovedStatus == true)
            {
                return Json("لايمكن إتمام عملية الحذف حيث أن هذه المشاركة دخلت ضمن مراحل الإعتماد", JsonRequestBehavior.AllowGet);
            }

            #region عملية الحذف للملفات اللي موجودة علي السيرفر الأول

            var attachment = dbSch.ActivityRequestMasterAttachments.FirstOrDefault(x => x.ActivityRequestMaster_ID == model.ID);
            if (attachment != null)
            {
                if (System.IO.File.Exists(Server.MapPath(attachment.FilePath)))
                {
                    System.IO.File.SetAttributes(Server.MapPath(attachment.FilePath), FileAttributes.Normal);

                    System.IO.File.Delete(Server.MapPath(attachment.FilePath));
                }

                dbSch.ActivityRequestMasterAttachments.Remove(attachment);
                dbSch.SaveChanges();
            }
            #endregion

            dbSch.ActivityRequestDetails.RemoveRange(dbSch.ActivityRequestDetails.Where(x => x.ActivityRequestMaster_ID == model.ID).ToList());
            dbSch.ActivityRequestMaster.Remove(model);
            try
            {
                dbSch.SaveChanges();
            }
            catch (Exception)
            {
                return Json("لايمكن حذف هذه القائمة لإرتباطها بسجلات أخري", JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  شاشة متابعة واعتماد قوائم المشاركات الجديدة


        [HttpGet]
        public ActionResult NewActivityDecisionTaking()
        {
            var json = new JavaScriptSerializer().Serialize(GetPermissionsJson(101).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            if (permissions == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }


        [HttpGet]
        public ActionResult GetNewActivitiesRequests()
        {
            var ActivityRequestMasterList = dbSch.ActivityRequestMaster.Where(m =>
                                            (m.IsPosted == true && m.ApprovedStatus == null && m.IsReturned == null) ||
                                            (m.IsPosted == true && m.ApprovedStatus == false && m.IsReturned == true)).ToList();

            return this.JsonMaxLength(ActivityRequestMasterList.Select(t => new
            {
                IsPosted = t.IsPosted,
                ActivityRequestId = t.ID,
                IsReturned = t.IsReturned,
                StatusNotes = t.StatusNotes,
                ApprovedStatus = t.ApprovedStatus,
                ActivityName = t.ActivityConfig.Name,
                ActivityLocation = t.ActivityConfig1.Name,
                ActivityStudentNo = t.ActivityRequestDetails.Count,
                ActivityStartDate = t.InsertionDate.ToString("dd/MM/yyyy"),
                ActivityNo = "(" + t.ActivityNo + ")" + "-" + t.InsertionDate.ToString("dd/MM/yyyy")
            }).ToList());
        }


        [HttpGet]
        public ActionResult GetActivityDetails(int ActivityRequestId)
        {
            var Query = (from Q in dbSch.ActivityRequestDetails.Where(m => m.ActivityRequestMaster_ID == ActivityRequestId)
                         join std in dbSch.INTEGRATION_All_Students
                         on Q.Student_ID equals std.STUDENT_ID
                         select new
                         {
                             ActivityRequestId = Q.ActivityRequestMaster_ID,
                             FacultyName = std.FACULTY_NAME,
                             DegreeName = std.DEGREE_DESC,
                             FacultyId = std.FACULTY_NO + "",
                             StudentName = std.STUDENT_NAME,
                             EvaluationName = Q.ActivityConfig.Name
                         });


            return this.JsonMaxLength(Query.Select(t => new
            {
                ActivityRequestId = t.ActivityRequestId,
                FacultyName = t.FacultyName,
                DegreeName = t.DegreeName,
                FacultyId = t.FacultyId + "",
                StudentName = t.StudentName,
                EvaluationName = t.EvaluationName
            }).ToList());
        }


        [HttpPost]
        public ActionResult RejectNewActivityRequest(int activityRequestId, string recommendations)
        {
            if (string.IsNullOrEmpty(recommendations))
            {
                return Content("عفواادخل توصيات رفض المشاركة");
            }

            var ActivityRequestInDB = dbSch.ActivityRequestMaster.FirstOrDefault(x => x.ID == activityRequestId);
            if (ActivityRequestInDB == null)
            {
                return Content("لا يمكن إتمام عملية الحفظ ");
            }
            ActivityRequestInDB.ApprovedByUserID = CurrentUser.ID;
            ActivityRequestInDB.StatusNotes = recommendations;
            ActivityRequestInDB.ApprovedStatus = false;
            ActivityRequestInDB.IsReturned = true;
            ActivityRequestInDB.IsPosted = false;

            dbSch.Entry(ActivityRequestInDB).State = System.Data.Entity.EntityState.Modified;
            dbSch.SaveChanges();
            return Json("");
        }


        [HttpPost]
        public ActionResult AcceptActivityRequest(int activityRequestId, string recommendations)
        {

            var ActivityRequestInDB = dbSch.ActivityRequestMaster.FirstOrDefault(x => x.ID == activityRequestId);
            if (ActivityRequestInDB == null)
            {
                return Content("لا يمكن إتمام عملية الحفظ ");
            }
            ActivityRequestInDB.ApprovedByUserID = CurrentUser.ID;
            ActivityRequestInDB.StatusNotes = recommendations;
            ActivityRequestInDB.ApprovedStatus = true;
            ActivityRequestInDB.IsReturned = null;
            ActivityRequestInDB.IsPosted = true;

            dbSch.Entry(ActivityRequestInDB).State = System.Data.Entity.EntityState.Modified;
            dbSch.SaveChanges();
            return Json("");
        }

        #endregion

        #endregion

        #region FileUploader

        [HttpPost]
        public ActionResult UploadFiles()
        {
            Session["AcademicActivityFile"] = null;
            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentType != "application/pdf")
                {
                    return new HttpStatusCodeResult(400);
                }
                else
                {
                    Session["AcademicActivityFile"] = file;
                }
            }
            return Json(0);
        }

        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        #endregion

        #region Permissions       

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

        private int CurrentStudentId
        {
            get
            {
                var user = HttpContext.Session["UserId"] as DashBoard_Users;

                if (user != null && user.ID != 0)
                {
                    if (user.IsStudent == true)
                    {
                        return Convert.ToInt32(user.Username);
                    }
                    return 0;
                }
                else
                {
                    return 0;
                }
            }

        }

        [HttpPost]
        public JsonResult GetPermissionsJson(int screenId)
        {
            return this.JsonMaxLength(GetPermissionsFn(screenId));
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

        #endregion
    }
}