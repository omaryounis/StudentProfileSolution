using StudentProfile.Components;
using StudentProfile.DAL.Models;
using StudentProfile.Web.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class StudentsStudiesController : Controller
    {
        private notify _notify;
        private readonly SchoolAccGam3aEntities _dbSc = new SchoolAccGam3aEntities();
        // GET: StudentsStudies
        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(1009);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }


        public ActionResult GetFaculties()
        {
            var Faculties = _dbSc.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Value != "210" && x.Value != "220").ToList();
            return Json(Faculties, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDegrees()
        {
            var Degrees = _dbSc.usp_getDegrees().Where(x => (x.DEGREE_CODE == 5 || x.DEGREE_CODE == 6 || x.DEGREE_CODE == 7)).Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList();

            return Json(Degrees, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartments()
        {
            var Departments = _dbSc.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.DEPT_NAME, Value = x.DEPT_CODE.ToString() }).Distinct().Where(x => x.Text != null).ToList();

            return Json(Departments, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMajors()
        {
            var Majors = _dbSc.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.MAJOR_NAME, Value = x.MAJOR_NO.ToString() }).Distinct().Where(x => x.Text != null).ToList();

            return Json(Majors, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStatus()
        {
            var Status = _dbSc.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.STATUS_DESC, Value = x.STATUS_CODE.ToString() }).Distinct().Where(x => x.Text != null).ToList();

            return Json(Status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudies()
        {
            var Status = _dbSc.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.STUDY_DESC, Value = x.STUDY_CODE.ToString() }).Distinct().Where(x => x.Text != null).ToList();

            return Json(Status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStudentsForStudentsStudies(StudentsVm model)
        {
            //var faculities = string.IsNullOrEmpty(model.facultyIds) ? new string[0] : model.facultyIds.Split(',');

            //var degrees = string.IsNullOrEmpty(model.degreeIds) ? new string[0] : model.degreeIds.Split(',');

            var Depratments = string.IsNullOrEmpty(model.DepartmentIds) ? new string[0] : model.DepartmentIds.Split(',');

            //var Majors = string.IsNullOrEmpty(model.MajorIds) ? new string[0] : model.MajorIds.Split(',');

            //var Status = string.IsNullOrEmpty(model.StatusIds) ? new string[0] : model.StatusIds.Split(',');

            //var Study = string.IsNullOrEmpty(model.StudyIds) ? new string[0] : model.StudyIds.Split(',');

            //var StudentID = string.IsNullOrEmpty(model.StudentID) ? new string[0] : model.StudentID.Split(',');
            if (Depratments.Count() > 0)
            {
                var students = _dbSc.USP_GRAD_STUDENTS(model.facultyIds, model.degreeIds, model.DepartmentIds, model.MajorIds, model.StatusIds, model.StudyIds, model.StudentID)
                    .Where(s => Depratments.Any(p => p == s.DEPT_CODE))
                    .Select(x => new
                    {
                        x.STUDENT_ID,
                        x.STUDENT_NAME,
                        x.NATIONAL_ID,
                        x.BIRTH_DATE,
                        JOIN_DATE = x.JOIN_DATE.Year + "/" + x.JOIN_DATE.Month + "/" + x.JOIN_DATE.Day,

                        x.REMAININGCREDITHOURSCOUNT,
                        //x.NATIONALITY_DESC,
                        x.JOIN_SEMESTER,
                        x.REWARD_PLAN, //_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).TOTAL_OFFICIAL_SEMESTER,
                        GRADUATE_DATE =x.GRADUATE_DATE,
                        DEPT_CODE =x.DEPT_CODE,
                        TOTAL_WARNINGS = x.TOTAL_WARNINGS,
                        MessageDate = (
                                 x.PRESENTATION_DATE != null ?
                               x.PRESENTATION_DATE :
                               x.DEAN_MEETING_DATE != null ?
                               x.DEAN_MEETING_DATE :
                               x.FORMATION_DATE != null ?
                               x.FORMATION_DATE :
                               x.DELIVERY_DATE != null ?
                               x.DELIVERY_DATE : null),

                        MessageStauts =
                        (
                        x.PRESENTATION_DATE != null ? "تم مناقشة الرسالة" :
                        x.DEAN_MEETING_DATE != null ? "تم اعتماد الخطة" :
                        x.FORMATION_DATE != null ? "تم تشكيل اللجنة" :
                        x.DELIVERY_DATE != null ? "تم التسليم" : ""),
                        //بيانات الرسالة
                        THESIS_LABEL = x.THESIS_LABEL,
                        SUPERVISOR_NAME = x.SUPERVISOR_NAME,
                        DELIVERY_DATE = x.DELIVERY_DATE,
                        PRESENTATION_DATE = x.PRESENTATION_DATE,
                        DEAN_MEETING_DATE = x.DEAN_MEETING_DATE,
                        FORMATION_DATE = x.FORMATION_DATE,


                    }).AsEnumerable()
                           .Select(x => new
                           {
                               x.STUDENT_ID,
                               x.STUDENT_NAME,
                               x.NATIONAL_ID,
                               x.BIRTH_DATE,
                               x.JOIN_DATE,
                               x.JOIN_SEMESTER,
                               x.REMAININGCREDITHOURSCOUNT,
                               GRADUATE_DATE = x.GRADUATE_DATE != null ? x.GRADUATE_DATE.Value.Date.Day + "/" + x.GRADUATE_DATE.Value.Date.Month + "/" + x.GRADUATE_DATE.Value.Date.Year : null,
                               x.DEPT_CODE,                                //DELIVERY_DATE = x.DELIVERY_DATE != null ? x.DELIVERY_DATE.Value.Day + "/" + x.DELIVERY_DATE.Value.Month + "/" + x.DELIVERY_DATE.Value.Year : null,
                               PRESENTATION_DATE = x.PRESENTATION_DATE != null ? x.PRESENTATION_DATE.Value.Day + "/" + x.PRESENTATION_DATE.Value.Month + "/" + x.PRESENTATION_DATE.Value.Year : null,
                               DEAN_MEETING_DATE = x.DEAN_MEETING_DATE != null ? x.DEAN_MEETING_DATE.Value.Day + "/" + x.DEAN_MEETING_DATE.Value.Month + "/" + x.DEAN_MEETING_DATE.Value.Year : null,
                               FORMATION_DATE = x.FORMATION_DATE != null ? x.FORMATION_DATE.Value.Day + "/" + x.FORMATION_DATE.Value.Month + "/" + x.FORMATION_DATE.Value.Year : null,
                               x.TOTAL_WARNINGS,
                               MessageDate = x.MessageDate != null ? x.MessageDate.Value.Date.Day + "/" + x.MessageDate.Value.Date.Month + "/" + x.MessageDate.Value.Date.Year : null,
                               x.MessageStauts,
                               x.THESIS_LABEL,
                               x.SUPERVISOR_NAME,
                               x.REWARD_PLAN,

                           }).ToList();

                return this.JsonMaxLength(students);
            }
            else
            {
                var students = _dbSc.USP_GRAD_STUDENTS(model.facultyIds, model.degreeIds, model.DepartmentIds, model.MajorIds, model.StatusIds, model.StudyIds, model.StudentID)
                
                     .Select(x => new
                     {
                         x.STUDENT_ID,
                         x.STUDENT_NAME,
                         x.NATIONAL_ID,
                         x.BIRTH_DATE,
                         JOIN_DATE = x.JOIN_DATE.Year + "/" + x.JOIN_DATE.Month + "/" + x.JOIN_DATE.Day,

                         x.REMAININGCREDITHOURSCOUNT,
                        //x.NATIONALITY_DESC,
                        x.JOIN_SEMESTER,
                         x.REWARD_PLAN, //_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).TOTAL_OFFICIAL_SEMESTER,
                        GRADUATE_DATE = x.GRADUATE_DATE,
                         DEPT_CODE = x.DEPT_CODE,
                         TOTAL_WARNINGS = x.TOTAL_WARNINGS,
                         MessageDate = (
                                  x.PRESENTATION_DATE != null ?
                                x.PRESENTATION_DATE :
                                x.DEAN_MEETING_DATE != null ?
                                x.DEAN_MEETING_DATE :
                                x.FORMATION_DATE != null ?
                                x.FORMATION_DATE :
                                x.DELIVERY_DATE != null ?
                                x.DELIVERY_DATE : null),

                         MessageStauts =
                         (
                         x.PRESENTATION_DATE != null ? "تم مناقشة الرسالة" :
                         x.DEAN_MEETING_DATE != null ? "تم اعتماد الخطة" :
                         x.FORMATION_DATE != null ? "تم تشكيل اللجنة" :
                         x.DELIVERY_DATE != null ? "تم التسليم" : ""),
                        //بيانات الرسالة
                        THESIS_LABEL = x.THESIS_LABEL,
                         SUPERVISOR_NAME = x.SUPERVISOR_NAME,
                         DELIVERY_DATE = x.DELIVERY_DATE,
                         PRESENTATION_DATE = x.PRESENTATION_DATE,
                         DEAN_MEETING_DATE = x.DEAN_MEETING_DATE,
                         FORMATION_DATE = x.FORMATION_DATE,


                     }).AsEnumerable()
                            .Select(x => new
                            {
                                x.STUDENT_ID,
                                x.STUDENT_NAME,
                                x.NATIONAL_ID,
                                x.BIRTH_DATE,
                                x.JOIN_DATE,
                                x.JOIN_SEMESTER,
                                x.REMAININGCREDITHOURSCOUNT,
                                GRADUATE_DATE = x.GRADUATE_DATE != null ? x.GRADUATE_DATE.Value.Date.Day + "/" + x.GRADUATE_DATE.Value.Date.Month + "/" + x.GRADUATE_DATE.Value.Date.Year : null,
                                x.DEPT_CODE,                                //DELIVERY_DATE = x.DELIVERY_DATE != null ? x.DELIVERY_DATE.Value.Day + "/" + x.DELIVERY_DATE.Value.Month + "/" + x.DELIVERY_DATE.Value.Year : null,
                               PRESENTATION_DATE = x.PRESENTATION_DATE != null ? x.PRESENTATION_DATE.Value.Day + "/" + x.PRESENTATION_DATE.Value.Month + "/" + x.PRESENTATION_DATE.Value.Year : null,
                                DEAN_MEETING_DATE = x.DEAN_MEETING_DATE != null ? x.DEAN_MEETING_DATE.Value.Day + "/" + x.DEAN_MEETING_DATE.Value.Month + "/" + x.DEAN_MEETING_DATE.Value.Year : null,
                                FORMATION_DATE = x.FORMATION_DATE != null ? x.FORMATION_DATE.Value.Day + "/" + x.FORMATION_DATE.Value.Month + "/" + x.FORMATION_DATE.Value.Year : null,
                                x.TOTAL_WARNINGS,
                                MessageDate = x.MessageDate != null ? x.MessageDate.Value.Date.Day + "/" + x.MessageDate.Value.Date.Month + "/" + x.MessageDate.Value.Date.Year : null,
                                x.MessageStauts,
                                x.THESIS_LABEL,
                                x.SUPERVISOR_NAME,
                                x.REWARD_PLAN,

                            }).ToList();
                return this.JsonMaxLength(students);
            }


            //var students = _dbSc.INTEGRATION_All_Students
            //                .Where(x => faculities.Any(p => p == x.FACULTY_NO.ToString() || faculities.Count() == 0) &&
            //                 Depratments.Any(p => p == x.DEPT_CODE || Depratments.Count() == 0) &&
            //                 Majors.Any(p => p == x.MAJOR_NO.ToString() || Majors.Count() == 0) &&
            //                 Status.Any(p => p == x.STATUS_CODE.ToString() || Status.Count() == 0) &&
            //                 Study.Any(p => p == x.STUDY_CODE.ToString() || Study.Count() == 0) &&
            //                 StudentID.Any(p => p == x.STUDENT_ID.ToString() || StudentID.Count() == 0) &&
            //                 degrees.Any(p => p == x.DEGREE_CODE.ToString() || degrees.Count() == 0) &&
            //                 x.JOIN_DATE > new DateTime(2011, 01, 01)
            //                 )
            //                .Select(x => new
            //                {
            //                    x.STUDENT_ID,
            //                    x.STUDENT_NAME,
            //                    x.NATIONAL_ID,
            //                    x.BIRTH_DATE,
            //                    JOIN_DATE = x.JOIN_DATE.Year + "/" + x.JOIN_DATE.Month + "/" + x.JOIN_DATE.Day,
            //                  //  x.STATUS_DESC,
            //                    //x.DEPT_NAME,
            //                   // x.FACULTY_NAME,
            //                  //  x.MAJOR_NAME,
            //                   // x.DEGREE_DESC,
            //                    //x.STUDY_DESC,
            //                    x.REMAININGCREDITHOURSCOUNT,
            //                    //x.NATIONALITY_DESC,
            //                    x.JOIN_SEMESTER,
            //                    x.REWARD_PLAN, //_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).TOTAL_OFFICIAL_SEMESTER,
            //                   // GRADUATE_DATE = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).GRADUATE_DATE,
            //                   // DEPT_CODE = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).DEPT_CODE,
            //                    TOTAL_WARNINGS = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).TOTAL_WARNINGS,
            //                    //MessageDate = (
            //                    //  _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).PRESENTATION_DATE != null ?
            //                    //_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).PRESENTATION_DATE :
            //                    // _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).DEAN_MEETING_DATE != null ?
            //                    //_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).DEAN_MEETING_DATE :
            //                    // _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).FORMATION_DATE != null ?
            //                    //_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).FORMATION_DATE :
            //                    //_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).DELIVERY_DATE != null ?
            //                    // _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).DELIVERY_DATE : null),

            //                    //MessageStauts =
            //                    //_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).PRESENTATION_DATE != null ? "تم مناقشة الرسالة" :
            //                    //_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).DEAN_MEETING_DATE != null ? "تم اعتماد الخطة" :
            //                    // _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).FORMATION_DATE != null ? "تم تشكيل اللجنة" :
            //                    // (_dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).DELIVERY_DATE != null ? "تم التسليم" : ""),
            //                    ////بيانات الرسالة
            //                    //THESIS_LABEL = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).THESIS_LABEL,
            //                    //SUPERVISOR_NAME = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).SUPERVISOR_NAME,
            //                    //DELIVERY_DATE = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).DELIVERY_DATE,
            //                    //PRESENTATION_DATE = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).PRESENTATION_DATE,
            //                    //DEAN_MEETING_DATE = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).DEAN_MEETING_DATE,
            //                    //FORMATION_DATE = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).FORMATION_DATE,
            //                }).AsEnumerable()
            //                .Select(x => new
            //                {
            //                    x.STUDENT_ID,
            //                    x.STUDENT_NAME,
            //                    x.NATIONAL_ID,
            //                    x.BIRTH_DATE,
            //                    x.JOIN_DATE,
            //                    //x.STATUS_DESC,
            //                    //x.DEPT_NAME,
            //                    //x.FACULTY_NAME,
            //                    //x.MAJOR_NAME,
            //                    //x.DEGREE_DESC,
            //                    //x.STUDY_DESC,
            //                    //x.REMAININGCREDITHOURSCOUNT,
            //                    //x.NATIONALITY_DESC,
            //                    x.JOIN_SEMESTER,
            //                    //GRADUATE_DATE = x.GRADUATE_DATE != null ? x.GRADUATE_DATE.Value.Date.Day + "/" + x.GRADUATE_DATE.Value.Date.Month + "/" + x.GRADUATE_DATE.Value.Date.Year : null,
            //                    //x.DEPT_CODE,                                //DELIVERY_DATE = x.DELIVERY_DATE != null ? x.DELIVERY_DATE.Value.Day + "/" + x.DELIVERY_DATE.Value.Month + "/" + x.DELIVERY_DATE.Value.Year : null,
            //                    //PRESENTATION_DATE = x.PRESENTATION_DATE != null ? x.PRESENTATION_DATE.Value.Day + "/" + x.PRESENTATION_DATE.Value.Month + "/" + x.PRESENTATION_DATE.Value.Year : null,
            //                    //DEAN_MEETING_DATE = x.DEAN_MEETING_DATE != null ? x.DEAN_MEETING_DATE.Value.Day + "/" + x.DEAN_MEETING_DATE.Value.Month + "/" + x.DEAN_MEETING_DATE.Value.Year : null,
            //                    //FORMATION_DATE = x.FORMATION_DATE != null ? x.FORMATION_DATE.Value.Day + "/" + x.FORMATION_DATE.Value.Month + "/" + x.FORMATION_DATE.Value.Year : null,
            //                    x.TOTAL_WARNINGS,
            //                    //MessageDate = x.MessageDate != null ? x.MessageDate.Value.Date.Day + "/" + x.MessageDate.Value.Date.Month + "/" + x.MessageDate.Value.Date.Year : null,
            //                    //x.MessageStauts,
            //                    //x.THESIS_LABEL,
            //                    //x.SUPERVISOR_NAME,
            //                    x.REWARD_PLAN,

            //                }).ToList();


            //return this.JsonMaxLength(students);
        }

        public ActionResult GetStudentWarning(decimal id)
        {
            var data = _dbSc.INTEGRATION_All_STUDENTS_GRADUATES_WARNING.Where(x => x.STUDENT_ID == id)
                            .Select(x => new
                            {
                                x.ID,
                                x.STUDENT_ID,
                                x.REASON,
                                x.NOTE,
                                ENTRY_DATE = x.ENTRY_DATE != null ? x.ENTRY_DATE.Value.Day + "/" + x.ENTRY_DATE.Value.Month + "/" + x.ENTRY_DATE.Value.Year : null,
                            }).ToList();
            return this.JsonMaxLength(data);
        }

        public ActionResult UpdateStudentsData()
        {
            int result = _dbSc.updateGraduateStudentsFromLinkedServer();
            if (result == 0)
                return Json(_notify = new notify() { Message = "لم يتم تحديث البيانات !", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            else
                return Json(_notify = new notify() { Message = "تم تحديث البيانات بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }


        public Permissions GetPermissionsFn(int screenId)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            // var userId = 0;
            if (user != null)
            {
                //   userId = int.Parse(HttpContext.Session["UserId"].ToString());

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

    }
}