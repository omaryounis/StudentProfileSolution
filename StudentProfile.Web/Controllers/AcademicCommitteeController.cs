using DevExpress.Web.Mvc;
using DevExpress.XtraReports.UI;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using StudentProfile.Web.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class AcademicCommitteeController : Controller
    {



        private notify _notify;
        private readonly SchoolAccGam3aEntities _dbSc = new SchoolAccGam3aEntities();

        #region SeparatedStudents
        public ActionResult SeparatedStudents()
        {
            var permissions = GetPermissionsFn(66);
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
        public ActionResult UpdateStudentsData()
        {
            int result = _dbSc.updateGraduateStudentsFromLinkedServer();
            if (result == 0)
                return Json(_notify = new notify() { Message = "لم يتم تحديث البيانات !", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            else
                return Json(_notify = new notify() { Message = "تم تحديث البيانات بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFaculties()
        {
            //var Faculties = _dbSc.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Text != null).ToList();

            var CurrentUser = Session["UserId"] as DashBoard_Users;
           
           var Faculties = _dbSc.USP_GET_ACTIVE_ACADEMICUSERSFACULTIES(CurrentUser.ID).ToList().Select(x => new { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() });

            return Json(Faculties, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDegrees(string FacultionIds)
        {
            var Degrees = _dbSc.usp_getDegreesByFACULTYID(FacultionIds).Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList();

            return Json(Degrees, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudents(string facultyIds, string degreeIds)
        {
            //var faculities = string.IsNullOrEmpty(facultyIds) ? new string[0] : facultyIds.Split(',');
            //var degrees = string.IsNullOrEmpty(degreeIds) ? new string[0] : degreeIds.Split(',');

            //var AcademicCommitteeStudentIds = _dbSc.AcademicCommittee.Where(x => (x.IsAccept == null || x.IsAccept == false)).Select(x => x.StudentId).ToList();

            //var students = _dbSc.INTEGRATION_All_Students
            //                .Where(x => (faculities.Any(p => p == x.FACULTY_NO.ToString()) &&
            //                 degrees.Any(p => p == x.DEGREE_CODE.ToString()) &&
            //                (x.STATUS_CODE == 5 || x.STATUS_CODE == 6) &&
            //                x.JOIN_DATE > new DateTime(2011, 01, 01)) &&
            //                !AcademicCommitteeStudentIds.Contains(x.STUDENT_ID))
            //                .Select(x => new
            //                {
            //                    x.STUDENT_ID,
            //                    x.STUDENT_NAME,
            //                    x.NATIONAL_ID,
            //                    x.STATUS_DESC,
            //                    x.FACULTY_NAME,
            //                    x.DEGREE_DESC,
            //                    x.REMAININGCREDITHOURSCOUNT,
            //                    JOIN_SEMESTER = _dbSc.INTEGRATION_All_STUDENTS_SEMESTER_TRANS.Where(c => c.STUDENT_ID == x.STUDENT_ID).OrderByDescending(m => m.ID).FirstOrDefault().SEMESTER,
            //                    // x.JOIN_SEMESTER,
            //                    x.TOTALSEMESTER,
            //                    x.NATIONALITY_DESC,
            //                    x.CATEGORY_NAME, // نوع المنحة
            //                    //cum_gpa = _dbSc.ESOL_ACADEMIC_RECORDS.FirstOrDefault(c => c.student_id == x.STUDENT_ID).cum_gpa, // المعدل التراكمي
            //                    TOTAL_WARNINGS = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).TOTAL_WARNINGS,
            //                    REASON_NAME = _dbSc.INTEGRATION_All_STUDENTS_SEMESTER_TRANS.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).REASON_NAME,
            //                    STUDY_DESC = _dbSc.INTEGRATION_All_Graduate_Students.FirstOrDefault(c => c.STUDENT_ID == x.STUDENT_ID).STUDY_DESC,
            //                    TOTAL_Apology = _dbSc.INTEGRATION_All_STUDENTS_SEMESTER_TRANS.Where(c => c.STUDENT_ID == x.STUDENT_ID && c.TRANS_NAME == "قرارالاعتذار").Count(),
            //                    TOTAL_Separated = _dbSc.INTEGRATION_All_STUDENTS_SEMESTER_TRANS.Where(c => c.STUDENT_ID == x.STUDENT_ID && c.TRANS_NAME == "قرار طي القيد").Count(),
            //                    Note = "",
            //                    //  Expected = _dbSc.usp_is_expected_graduated(x.STUDENT_ID).ToString()!= null?"نعم":"لا"
            //                }).AsEnumerable().Select(row => new
            //                {
            //                    STUDENT_ID = row.STUDENT_ID,
            //                    STUDENT_NAME = row.STUDENT_NAME,
            //                    NATIONAL_ID = row.NATIONAL_ID,
            //                    STATUS_DESC = row.STATUS_DESC,
            //                    FACULTY_NAME = row.FACULTY_NAME,
            //                    DEGREE_DESC = row.DEGREE_DESC,
            //                    REMAININGCREDITHOURSCOUNT = row.REMAININGCREDITHOURSCOUNT,
            //                    JOIN_SEMESTER = row.JOIN_SEMESTER,
            //                    // x.JOIN_SEMESTER,
            //                    TOTALSEMESTER = row.TOTALSEMESTER,
            //                    NATIONALITY_DESC = row.NATIONALITY_DESC,
            //                    CATEGORY_NAME = row.CATEGORY_NAME,
            //                    TOTAL_WARNINGS = row.TOTAL_WARNINGS,
            //                    REASON_NAME = row.REASON_NAME,
            //                    STUDY_DESC = row.STUDY_DESC,
            //                    TOTAL_Apology = row.TOTAL_Apology,
            //                    TOTAL_Separated = row.TOTAL_Separated,
            //                    Note = "",
            //                    Expected = _dbSc.usp_is_expected_graduated(row.STUDENT_ID).FirstOrDefault() != null ? "نعم" : "لا"// dc.get_User_Image(tmp.AssignTo).FirstOrDefault()
            //                }).ToList();
            var students = _dbSc.usp_get_academic_students(facultyIds, degreeIds).Select(m => new
            {
                m.STUDENT_ID,
                m.STUDENT_NAME,
                m.NATIONAL_ID,
                m.STATUS_DESC,
                m.FACULTY_NAME,
                m.DEGREE_DESC,
                m.REMAININGCREDITHOURSCOUNT,
                m.JOIN_SEMESTER,
                m.TOTALSEMESTER,
                m.NATIONALITY_DESC,
                m.CATEGORY_NAME,
                m.TOTAL_WARNINGS,
                m.REASON_NAME,
                m.STUDY_DESC,
                m.TOTAL_Apology,
                m.TOTAL_Separated,
              cum_gpa= m.cum_gpa,
              Expected = m.Expected != null ? "نعم" : "لا",
                m.Notes
            }).ToList();
            return this.JsonMaxLength(students);
        }

        public ActionResult GetStudentsSemesterTrans(decimal StudentId)
        {
            var data = _dbSc.INTEGRATION_All_STUDENTS_SEMESTER_TRANS.Where(x=>x.STUDENT_ID == StudentId)
                            .Select(x => new
                            {
                                x.ID,
                                x.STUDENT_ID,
                                x.SEMESTER,
                                x.START_SEMESTER,
                                x.END_SEMESTER,
                                ENTRY_DATE = x.ENTRY_DATE != null ? x.ENTRY_DATE.Value.Day + "/" + x.ENTRY_DATE.Value.Month + "/" + x.ENTRY_DATE.Value.Year : null,
                                x.REASON_NAME,
                                x.TRANS_NAME
                            }).ToList();
            return this.JsonMaxLength(data);
        }

        public ActionResult GetStudentsChances(decimal StudentId)
        {
            var data = _dbSc.INTEGRATION_All_STUDENTS_EXTRA_CHANCES.Where(x => x.STUDENT_ID == StudentId)
                            .Select(x => new
                            {
                                x.ID,
                                x.STUDENT_ID,
                                x.SEMESTER,
                                x.START_SEMESTER,
                                x.END_SEMESTER,
                                x.REASON_NAME,
                            }).ToList();
            return this.JsonMaxLength(data);
        }

        public ActionResult GetStudentWarning(decimal StudentId)
        {
            var data = _dbSc.INTEGRATION_All_STUDENTS_GRADUATES_WARNING.Where(x => x.STUDENT_ID == StudentId)
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

        public ActionResult RelaySeparatedStudents(List<AcademicCommitteeVm> model)
        {
            if (ModelState.IsValid)
            {
                var user = HttpContext.Session["UserId"] as DashBoard_Users;

                if (model != null)
                {
                    foreach (var item in model)
                    {
                        if (_dbSc.AcademicCommittee.Any(p => p.StudentId == item.StudentId))
                        {
                            return Json(_notify = new notify() { Message = $"الطالب {item.StudentName} موحود في قائمة الطلاب", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                        }

                        _dbSc.AcademicCommittee.Add(new AcademicCommittee
                        {
                            StudentId = item.StudentId,
                            EmployeeNote = item.EmployeeNote,
                            EmployeeDate = DateTime.Now,
                            EmployeeId = user.ID,
                            IsAccept = null
                        });
                    }
                    _dbSc.SaveChanges();
                    return Json(_notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(_notify = new notify() { Message = "اختر طلاب للحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(_notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AcademicCommittee
        /************************************************** AcademicCommittee ****************************************************/
        public ActionResult AcademicCommittee()
        {
            var permissions = GetPermissionsFn(67);
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


        public ActionResult GetStudentsForAcademicCommittee(string facultyIds, string degreeIds)
        {
            //var faculities = string.IsNullOrEmpty(facultyIds) ? new string[0] : facultyIds.Split(',');

            //var degrees = string.IsNullOrEmpty(degreeIds) ? new string[0] : degreeIds.Split(',');

            //var students = _dbSc.AcademicCommittee.AsEnumerable()
            //                .Select(x => new
            //                {
            //                    x.Id,
            //                    x.StudentId,
            //                    STUDENT_NAME = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).STUDENT_NAME,
            //                    NATIONAL_ID = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).NATIONAL_ID,
            //                    STATUS_DESC = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).STATUS_DESC,
            //                    FACULTY_NAME = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).FACULTY_NAME,
            //                    DEGREE_DESC = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).DEGREE_DESC,
            //                    STATUS_CODE = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).STATUS_CODE,
            //                    FACULTY_NO = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).FACULTY_NO,
            //                    DEGREE_CODE = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).DEGREE_CODE,
            //                    // JOIN_SEMESTER = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).JOIN_SEMESTER,

            //                    JOIN_SEMESTER = _dbSc.INTEGRATION_All_STUDENTS_SEMESTER_TRANS.Where(c => c.STUDENT_ID == x.StudentId).OrderByDescending(m=>m.ID).FirstOrDefault().SEMESTER,
            //                    REMAININGCREDITHOURSCOUNT = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).REMAININGCREDITHOURSCOUNT,
            //                    TOTALSEMESTER = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).TOTALSEMESTER,
            //                    x.EmployeeNote,
            //                    EmployeeDate = x.EmployeeDate != null ? x.EmployeeDate.Value.Date.ToString("dd/MM/yyyy") : null,
            //                    Employee = _dbSc.DashBoard_Users.FirstOrDefault(e => e.ID == x.EmployeeId).Name,
            //                    x.AcademicNote,
            //                    x.IsAccept,
            //                }).Where(x => (faculities.Any(p => p == x.FACULTY_NO.ToString()) &&
            //                 degrees.Any(p => p == x.DEGREE_CODE.ToString()) &&
            //                  (x.STATUS_CODE == 5 || x.STATUS_CODE == 6)) && x.IsAccept == null)
            //                .ToList();
            var students = _dbSc.usp_get_not_take_des_academic_students(facultyIds, degreeIds).Select(m=>new {

                m.Id,
                m.faculty_no,
                m.DEGREE_CODE,
                m.join_date,
                m.STUDENT_ID,
                m.STUDENT_NAME,
                m.NATIONAL_ID,
                m.STATUS_DESC,
                m.FACULTY_NAME,
                m.DEGREE_DESC,
                m.REMAININGCREDITHOURSCOUNT,
                m.JOIN_SEMESTER,
                m.TOTALSEMESTER,
                m.NATIONALITY_DESC,
                m.CATEGORY_NAME,
                m.TOTAL_WARNINGS,
                m.REASON_NAME,
                m.STUDY_DESC,
                m.TOTAL_Apology,
                m.TOTAL_Separated,
                Expected = m.Expected != null ? "نعم" : "لا",
                m.EmployeeNote,
                m.EmployeeDate,
                m.AcademicNote,
                m.IsAccept,
                m.cum_gpa
            }).ToList();
            return Json(data: students, behavior: JsonRequestBehavior.AllowGet);
        }

        //public ActionResult AcceptStudents(AcademicCommitteeDecisionVm model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = HttpContext.Session["UserId"] as DashBoard_Users;

        //        var StudentId = _dbSc.AcademicCommittee.Find(model.Id);
        //        if (StudentId != null)
        //        {
        //            StudentId.IsAccept = true;
        //            StudentId.AcademicNote = model.Note;
        //            StudentId.AcademicDate = DateTime.Now;
        //            StudentId.AcademicId = user.ID;
        //            _dbSc.SaveChanges();
        //            return Json(_notify = new notify() { Message = "تم الموافقة بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return Json(_notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

        //}



        public ActionResult AcceptStudents(AcademicCommitteeDecisionVm [] model)
        {
            if (ModelState.IsValid)
            {
                int result = 0;
                var user = HttpContext.Session["UserId"] as DashBoard_Users;
                foreach(var item in model)
                {
                    var StudentId = _dbSc.AcademicCommittee.Find(item.Id);
                    if (StudentId != null)
                    {
                        StudentId.IsAccept = true;
                        StudentId.AcademicNote = item.Note;
                        StudentId.AcademicDate = DateTime.Now;
                        StudentId.AcademicId = user.ID;
                       result= _dbSc.SaveChanges();
                    }
                }
                if(result > 0)
                {
                    return Json(_notify = new notify() { Message = "تم الموافقة بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(_notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult RejectStudents(AcademicCommitteeDecisionVm model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = HttpContext.Session["UserId"] as DashBoard_Users;

        //        var StudentId = _dbSc.AcademicCommittee.Find(model.Id);
        //        if (StudentId != null)
        //        {
        //            StudentId.IsAccept = false;
        //            StudentId.AcademicNote = model.Note;
        //            StudentId.AcademicDate = DateTime.Now;
        //            StudentId.AcademicId = user.ID;
        //            _dbSc.SaveChanges();
        //            return Json(_notify = new notify() { Message = "تم الرفض بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return Json(_notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult RejectStudents(AcademicCommitteeDecisionVm [] model)
        {
            if (ModelState.IsValid)
            {
                int result = 0;
                var user = HttpContext.Session["UserId"] as DashBoard_Users;
                foreach(var item in model)
                {
                    var StudentId = _dbSc.AcademicCommittee.Find(item.Id);
                    if (StudentId != null)
                    {
                        StudentId.IsAccept = false;
                        StudentId.AcademicNote = item.Note;
                        StudentId.AcademicDate = DateTime.Now;
                        StudentId.AcademicId = user.ID;
                      result=  _dbSc.SaveChanges();
                    }
                }
                if(result > 0)
                {
                    return Json(_notify = new notify() { Message = "تم الرفض بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                }


            }
            return Json(_notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region UploadAcademicDecisionsFiles
        /************************************************** UploadDecisions ****************************************************/
        public void AcademicDecisionsFiles()
        {
            if (!Directory.Exists(Server.MapPath($"~/Content/AcademicDecisionsFiles")))
                Directory.CreateDirectory(Server.MapPath($"~/Content/AcademicDecisionsFiles"));

            Session["AcademicDecisionsFiles"] = Request.Files[0];
        }

        public ActionResult GetAcademicCommitteeStudents()
        {
            var Students = _dbSc.AcademicCommittee.Where(x => x.IsAccept == true && x.DecisionNumber == null).Select(x => new SelectListItem { Text = _dbSc.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentId).STUDENT_NAME, Value = x.StudentId.ToString() }).ToList();
            return Json(Students, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SaveDecisions(DecisionNumberVm model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    var StudentIds = string.IsNullOrEmpty(model.StudentIds) ? new string[0] : model.StudentIds.Split(',');

                    string GuidKey = Guid.NewGuid().ToString();
                    HttpPostedFileBase AcademicDecisionsFiles = null;
                    if (Session["AcademicDecisionsFiles"] != null)
                    {
                        AcademicDecisionsFiles = (HttpPostedFileBase)Session["AcademicDecisionsFiles"];
                        AcademicDecisionsFiles.SaveAs(Server.MapPath($"~/Content/AcademicDecisionsFiles/[{model.DecisionNumber}]{GuidKey}{AcademicDecisionsFiles.FileName}"));
                    }
                    foreach (var item in StudentIds)
                    {
                        var studentId = _dbSc.AcademicCommittee.AsEnumerable().FirstOrDefault(x => x.StudentId == decimal.Parse(item));
                        studentId.DecisionNumber = model.DecisionNumber;
                        studentId.DecisionFile = $"[{model.DecisionNumber}]{GuidKey}{AcademicDecisionsFiles.FileName}".ToString();
                    }
                    _dbSc.SaveChanges();
                    return Json(_notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(_notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Report
        public ActionResult AcademicCommitteeReportIndex()
        {
            ViewBag.Faculties = _dbSc.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Degrees = _dbSc.usp_getDegrees().Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList();

            var permissions = GetPermissionsFn(68);
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

        public ActionResult AcademicCommitteeStudentsReport(decimal[]  facultiesCheckListBox, decimal[] degreeCheckListBox, decimal? StudentsComboBox,
           bool? DecisionStutesComboBox, DateTime? CreateDateFrom, DateTime? CreateDateTo, DateTime? DecisionDateFrom, DateTime? DecisionDateTo
            ,int? IsAll
            )
        {
           
            List<AcademicCommitteeStudentsReportVM> AcademicCommitteeStudentsList = new List<AcademicCommitteeStudentsReportVM>();
            string faculties= facultiesCheckListBox!=null?string.Join(",", facultiesCheckListBox):null;
            string degrees = degreeCheckListBox!=null? string.Join(",", degreeCheckListBox):null;
            bool? isTakeDesciion = null;
            if (DecisionStutesComboBox != null)
            {
                Session["DecisionStutesComboBox"] = DecisionStutesComboBox;
               
                Session["IsAll"] = null;
            }
            if(IsAll == 2)
            {
                Session["DecisionStutesComboBox"] = null;

                Session["IsAll"] = null;
            }
            //else
            //{

            //    Session["DecisionStutesComboBox"] = null;
            //}
            if (Session["IsAll"] != null)
            {
                Session["DecisionStutesComboBox"] = null;
                IsAll = int.Parse(Session["IsAll"].ToString());
            }
            if (Session["DecisionStutesComboBox"] != null){
                isTakeDesciion = (bool)Session["DecisionStutesComboBox"];
            }
            if (IsAll == 1)
            {
                Session["IsAll"] = IsAll;
                AcademicCommitteeStudentsList = _dbSc.Usp_Get_AcademicCommittee_Students(faculties, degrees, StudentsComboBox,
                                            CreateDateFrom, CreateDateTo, DecisionDateFrom, DecisionDateTo, null).Select(x=>new AcademicCommitteeStudentsReportVM
                                            {
                                                StudentID = x.StudentID,
                                                StudentName = x.StudentName,
                                                NationalID = x.NationalID,
                                                StutesName =x.StutesName,
                                                FacultyName = x.FacultyName,
                                                DegreeName = x.DegreeName,
                                                StutesCode = x.StutesCode,
                                                FacultyCode = x.FacultyCode,
                                                DegreeCode = x.DegreeCode,
                                                EmployeeNote = x.EmployeeNote,
                                                EmployeeDate = x.EmployeeDate != null ? x.EmployeeDate : null,
                                                EmployeeUser = x.EmployeeUser,
                                                AcademicDate = x.AcademicDate != null ? x.AcademicDate : null,
                                                AcademicUser = x.AcademicUser,
                                                AcademicNote = x.AcademicNote,
                                                Stutes = x.IsAccept == false ? "رفض" : x.IsAccept == true ? "موافقة" : "لم يتم اتخاذ قرار",
                                                IsAccept = x.IsAccept,
                                                DecisionNumber = x.DecisionNumber,
                                                DecisionFile = x.DecisionFile,
                                                RowLevel = x.RowLevel,
                                                Houres = x.Houres
                                            }).ToList();

                Session["AcademicCommitteeStudentsSession"] = AcademicCommitteeStudentsList;


                var report = new AcademicCommitteeStudentsRPT();

                report.DataSourceDemanded += (s, e) =>
                {
                    ((XtraReport)s).DataSource = AcademicCommitteeStudentsList;
                };

                return PartialView("_AcademicCommitteeStudentsReportDesignerPartial", report);
            }
            else
            {
                AcademicCommitteeStudentsList = _dbSc.Usp_Get_AcademicCommittee_Students(faculties, degrees, StudentsComboBox,
                                            CreateDateFrom, CreateDateTo, DecisionDateFrom, DecisionDateTo, DecisionStutesComboBox).AsEnumerable().Select(x => new AcademicCommitteeStudentsReportVM
                                            {
                                                StudentID = x.StudentID,
                                                StudentName = x.StudentName,
                                                NationalID = x.NationalID,
                                                StutesName = x.StutesName,
                                                FacultyName = x.FacultyName,
                                                DegreeName = x.DegreeName,
                                                StutesCode = x.StutesCode,
                                                FacultyCode = x.FacultyCode,
                                                DegreeCode = x.DegreeCode,
                                                EmployeeNote = x.EmployeeNote,
                                                EmployeeDate = x.EmployeeDate != null ? x.EmployeeDate : null,
                                                EmployeeUser = x.EmployeeUser,
                                                AcademicDate = x.AcademicDate != null ? x.AcademicDate : null,
                                                AcademicUser = x.AcademicUser,
                                                AcademicNote = x.AcademicNote,
                                                Stutes = x.IsAccept == false ? "رفض" : x.IsAccept == true ? "موافقة" : "لم يتم اتخاذ قرار",
                                                IsAccept = x.IsAccept,
                                                DecisionNumber = x.DecisionNumber,
                                                DecisionFile = x.DecisionFile,
                                                RowLevel = x.RowLevel,
                                                Houres = x.Houres
                                            }).Where(x => x.IsAccept == isTakeDesciion).ToList();
                Session["AcademicCommitteeStudentsSession"] = AcademicCommitteeStudentsList;


                var report = new AcademicCommitteeStudentsRPT();

                report.DataSourceDemanded += (s, e) =>
                {
                    ((XtraReport)s).DataSource = AcademicCommitteeStudentsList;
                };

                return PartialView("_AcademicCommitteeStudentsReportDesignerPartial", report);
                //_dbSc.AcademicCommittee.AsEnumerable()
            }


        }

        public ActionResult AcademicCommitteeStudentsReportExport(decimal[] facultiesCheckListBox, decimal[] degreeCheckListBox, decimal? StudentsComboBox,
           bool? DecisionStutesComboBox, DateTime? CreateDateFrom, DateTime? CreateDateTo, DateTime? DecisionDateFrom, DateTime? DecisionDateTo
            , int? IsAll)
        {
            var report = new AcademicCommitteeStudentsRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["AcademicCommitteeStudentsSession"];
            };
            return DocumentViewerExtension.ExportTo(report, Request);
        }


        #endregion

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

        public class AcademicCommitteeVm
        {
            public int Id { get; set; }
            public decimal StudentId { get; set; }
            public string StudentName { get; set; }
            public string EmployeeNote { get; set; }
            public string AcademicNote { get; set; }
            public bool? IsAccept { get; set; }
        }

        public class AcademicCommitteeDecisionVm
        {
            public int Id { get; set; }
            public string Note { get; set; }
        }

        public class DecisionNumberVm
        {
            public int DecisionNumber { get; set; }
            public string StudentIds { get; set; }
        }

        public class AcademicCommitteeStudentsReportVM
        {
            public decimal StudentID { get; set; }
            public string StudentName { get; set; }
            public string NationalID { get; set; }
            public string Stutes { get; set; }
            public decimal? StutesCode { get; set; }
            public string StutesName { get; set; }
            public decimal? DegreeCode { get; set; }
            public string DegreeName { get; set; }
            public decimal? FacultyCode { get; set; }
            public string FacultyName { get; set; }
            public string EmployeeNote { get; set; }
            public string EmployeeUser { get; set; }
            public DateTime? EmployeeDate { get; set; }
            public string AcademicNote { get; set; }
            public string AcademicUser { get; set; }
            public DateTime? AcademicDate { get; set; }
            public int? DecisionNumber { get; set; }
            public string DecisionFile { get; set; }
            public int? RowLevel { get; set; }
            public int? Houres { get; set; }
            public bool? IsAccept { get; set; }
        }

    }
}