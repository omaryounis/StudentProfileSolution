using DevExpress.Web.Mvc;
using DevExpress.XtraReports.UI;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using StudentProfile.Web.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Entity;
using DevExpress.XtraPrinting;

namespace StudentProfile.Web.Controllers
{
    public partial class PayRollStudentsController : Controller
    {
        private notify _notify;
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();

        public List<PayRollStudentsList> GetStudentPayroll()
        {
            List<PayRollStudentsList> PayRollStudentsList = new List<PayRollStudentsList>();


            var studentPayrollList = dbSC.Payroll.Select(x => new PayRollStudentsList
            {
                ID = x.ID,
                PayrollNumber = x.PayrollNumber,
                IssueDatePayroll = x.IssueDate,
                Payrollltem = x.IssueDate.Day + "-" + x.IssueDate.Month + "-" + x.IssueDate.Year + "(" + x.PayrollNumber + ")"
            }).Distinct().ToList();

            PayRollStudentsList.AddRange(studentPayrollList);

            return PayRollStudentsList.ToList();
        }
        public List<PayRollStudentsList> GetStudentPayrollAfterExportPaymentOrder()
        {
            List<PayRollStudentsList> PayRollStudentsList = new List<PayRollStudentsList>();


            var studentPayrollList = dbSC.Payroll.Where(x => x.IsActive == true).Select(x => new PayRollStudentsList
            {
                ID = x.ID,
                DafPayNo = x.DafPayNo,
                PayrollNumber = x.PayrollNumber,
                IssueDatePayroll = x.IssueDate,
                Payrollltem = x.IssueDate.Day + "-" + x.IssueDate.Month + "-" + x.IssueDate.Year + "(" + x.PayrollNumber + ")"
            }).Distinct().ToList();

            PayRollStudentsList.AddRange(studentPayrollList);

            return PayRollStudentsList.ToList();
        }
        public List<PayRollStudentsList> GetDelverPayroll()
        {
            List<PayRollStudentsList> PayRollStudentsList = new List<PayRollStudentsList>();


            var studentPayrollList = dbSC.Payroll.Where(x => x.IsActive == true && x.IsMonetary == true && x.IsPosted == true && x.IsExportedCheck == true && x.PayrollChecks.Any(p => p.IsReceived == true)).Select(x => new PayRollStudentsList
            {
                ID = x.ID,
                DafPayNo = x.DafPayNo,
                PayrollNumber = x.PayrollNumber,
                IssueDatePayroll = x.IssueDate,
                Payrollltem = x.IssueDate.Day + "-" + x.IssueDate.Month + "-" + x.IssueDate.Year + "(" + x.PayrollNumber + ")"
            }).Distinct().ToList();

            PayRollStudentsList.AddRange(studentPayrollList);

            return PayRollStudentsList.ToList();
        }
        public List<PayRollStudentsReportVM> PayrollDataSource(decimal?[] facultiesIds, decimal?[] degreeIds,
         decimal?[] statusesIds, decimal?[] levelsIds, int? yearsId, int? monthId, int? payrollNumber, int? StudentId, List<decimal> studentIds, bool? PaidStutes, bool? PaidType)
        {

            var facultiesIdsString = string.Join(",", facultiesIds);
            var degreesIdsString = string.Join(",", degreeIds);
            var statusesIdsString = string.Join(",", statusesIds);
            var levelsIdsString = string.Join(",", levelsIds);

            List<PayRollStudentsReportVM> payRollStudentsList = new List<PayRollStudentsReportVM>();

            var payRollStudentsFromDb = dbSC.StudentPayroll.Where(x => (payrollNumber == 0 || x.PayrollID == payrollNumber)).Select(x => new PayRollStudentsReportVM
            {
                StudentID = x.StudentID,
                StudentName = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).STUDENT_NAME,
                StudentNameEng = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).STUDENT_NAME_S,
                MobileNumber = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).MOBILE_NO,
                MobilePhone = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).MOBILE_PHONE,
                NationalID = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).NATIONAL_ID,
                StudentFaculty = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).FACULTY_NO,
                Faculty = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).FACULTY_NAME,
                StudentDegree = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).DEGREE_CODE,
                Stutes = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).STATUS_CODE,
                Levels = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).LEVEL_CODE,
                DegreeName = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).DEGREE_DESC,
                AccountNumber = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).ACCOUNT_NO,
                IsAccountNumber = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).ACCOUNT_NO != null ? true : false,
                Years = x.Payroll.IssueDate.Year,
                Month = x.Payroll.IssueDate.Month,
                PayrollNumber = x.Payroll.PayrollNumber,
                PayrollNumberwithDate = (x.Payroll.InsertDate.Day + "-" + x.Payroll.InsertDate.Month + "-" + x.Payroll.InsertDate.Year + "(" + x.Payroll.PayrollNumber + ")"),
                Days = x.NoOfDays,
                Value = x.Value,
                ItemName = x.RewardItems.RewardItemName_Arb,
                ItemType = x.RewardItems.RewardItemTypes.RewardItemType > 0 ? @"الاستحقاقات" : @"الحسميات",
                ItemTypeId = x.RewardItems.RewardItemTypes.RewardItemType,
                IsPaid = x.IsPaid,
                LevelName = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).LEVEL_CODE
            }).Where(x =>
                (facultiesIdsString == "" || facultiesIdsString.Contains(x.StudentFaculty.ToString())) &&
                (degreesIdsString == "" || degreesIdsString.Contains(x.StudentDegree.ToString())) &&
                (statusesIdsString == "" || statusesIdsString.Contains(x.Stutes.ToString())) &&
                (levelsIdsString == "" || levelsIdsString.Contains(x.Levels.ToString())) &&
                (studentIds.Count == 0 || studentIds.Contains(x.StudentID)) &&
                (yearsId == 0 || x.Years == yearsId) &&
                (monthId == 0 || x.Month == monthId) &&
                (StudentId == null || x.StudentID == StudentId) &&
                (PaidStutes == null || x.IsPaid == PaidStutes) &&
                (PaidType == null || x.IsAccountNumber == PaidType)
                ).ToList();


            payRollStudentsList.AddRange(payRollStudentsFromDb);

            var distinctStudentIdsList = payRollStudentsFromDb.GroupBy(x => new { x.StudentID, x.PayrollNumber }).Select(x => new { x.Key }).Distinct().ToList();

            foreach (var item in distinctStudentIdsList)
            {
                var studentRecords = payRollStudentsFromDb.Where(x => x.StudentID == item.Key.StudentID &&
                x.PayrollNumber == item.Key.PayrollNumber).ToList();
                payRollStudentsList.Add(new PayRollStudentsReportVM
                {
                    StudentID = item.Key.StudentID,
                    StudentName = studentRecords.First().StudentName,
                    Days = studentRecords.First().Days,
                    StudentFaculty = studentRecords.First().StudentFaculty,
                    StudentDegree = studentRecords.First().StudentDegree,
                    Faculty = studentRecords.First().Faculty,
                    DegreeName = studentRecords.First().DegreeName,
                    LevelName = studentRecords.First().LevelName,
                    PayrollNumber = studentRecords.First().PayrollNumber,
                    Value = 1,
                    ItemName = "",
                    ItemType = @"ا عدد الطلاب",
                    ItemTypeId = 0
                });
                payRollStudentsList.Add(new PayRollStudentsReportVM
                {
                    StudentID = item.Key.StudentID,
                    StudentName = studentRecords.First().StudentName,
                    Days = studentRecords.First().Days,
                    StudentFaculty = studentRecords.First().StudentFaculty,
                    StudentDegree = studentRecords.First().StudentDegree,
                    Faculty = studentRecords.First().Faculty,
                    DegreeName = studentRecords.First().DegreeName,
                    LevelName = studentRecords.First().LevelName,
                    PayrollNumber = studentRecords.First().PayrollNumber,
                    Value = studentRecords.Where(x => x.ItemTypeId > 0).Sum(x => x.Value),
                    ItemName = "",
                    ItemType = @"اجمالى المستحق",
                    ItemTypeId = 0
                });
                payRollStudentsList.Add(new PayRollStudentsReportVM
                {
                    StudentID = item.Key.StudentID,
                    StudentName = studentRecords.First().StudentName,
                    Days = studentRecords.First().Days,
                    StudentFaculty = studentRecords.First().StudentFaculty,
                    StudentDegree = studentRecords.First().StudentDegree,
                    Faculty = studentRecords.First().Faculty,
                    DegreeName = studentRecords.First().DegreeName,
                    LevelName = studentRecords.First().LevelName,
                    PayrollNumber = studentRecords.First().PayrollNumber,
                    Value = studentRecords.Where(x => x.ItemTypeId < 0).Sum(x => x.Value),
                    ItemName = "",
                    ItemType = @"اجمالى الحسميات",
                    ItemTypeId = 0
                });
                payRollStudentsList.Add(new PayRollStudentsReportVM
                {
                    StudentID = item.Key.StudentID,
                    StudentName = studentRecords.First().StudentName,
                    Days = studentRecords.First().Days,
                    StudentFaculty = studentRecords.First().StudentFaculty,
                    StudentDegree = studentRecords.First().StudentDegree,
                    Faculty = studentRecords.First().Faculty,
                    DegreeName = studentRecords.First().DegreeName,
                    LevelName = studentRecords.First().LevelName,
                    PayrollNumber = studentRecords.First().PayrollNumber,
                    Value = studentRecords.Sum(x => (x.Value * x.ItemTypeId)),
                    ItemName = "",
                    ItemType = @"الصافي المستحق",
                });
                payRollStudentsList.Add(new PayRollStudentsReportVM
                {
                    StudentID = item.Key.StudentID,
                    StudentName = studentRecords.First().StudentName,
                    Days = studentRecords.First().Days,
                    StudentFaculty = studentRecords.First().StudentFaculty,
                    Faculty = studentRecords.First().Faculty,
                    StudentDegree = studentRecords.First().StudentDegree,
                    DegreeName = studentRecords.First().DegreeName,
                    LevelName = studentRecords.First().LevelName,
                    PayrollNumber = studentRecords.First().PayrollNumber,
                    Value = -1,
                    ItemName = "",
                    ItemType = @"توقيع",
                    ItemTypeId = 0
                });
            }

            return payRollStudentsList;
        }


        // GET: PayRollStudents
        public ActionResult Index()
        {
            ViewBag.Faculties = dbSC.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Degrees = dbSC.usp_getDegrees().Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Stutes = dbSC.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.STATUS_DESC, Value = x.STATUS_CODE.ToString() }).Where(x => x.Text != null).Distinct().ToList();

            ViewBag.Levels = dbSC.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.LEVEL_DESC, Value = x.LEVEL_CODE.ToString() }).Where(x => x.Text != null).OrderBy(x => x.Value).Distinct().ToList();

            List<int> Years = new List<int>();
            DateTime startYear = DateTime.Now;
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                Years.Add(i);
            }
            ViewBag.Years = Years;


            var permissions = GetPermissionsFn(51);
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

        public ActionResult GetMonths(int YearsComboBox)
        {
            var Months = dbSC.Payroll.Where(x => x.IssueDate.Year == YearsComboBox).Select(x => new SelectListItem { Text = x.IssueDate.Month.ToString(), Value = x.IssueDate.Month.ToString() }).OrderBy(x => x.Text).Distinct().ToList();
            return Json(Months, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPayrollNumbers(int YearsComboBox, int MonthComboBox)
        {
            var PayRollNumbers = GetStudentPayrollAfterExportPaymentOrder().Where(x => x.IssueDatePayroll.Year == YearsComboBox && x.IssueDatePayroll.Month == MonthComboBox).Select(x => new SelectListItem { Text = x.Payrollltem, Value = x.ID.ToString() }).ToList();

            return Json(PayRollNumbers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPayrollNumbersForSaraf(int YearsComboBox, int MonthComboBox)
        {
            var PayRollNumbers = GetStudentPayrollAfterExportPaymentOrder().Where(x => x.DafPayNo != null && x.IssueDatePayroll.Year == YearsComboBox && x.IssueDatePayroll.Month == MonthComboBox).Select(x => new SelectListItem { Text = x.Payrollltem, Value = x.ID.ToString() }).ToList();

            return Json(PayRollNumbers, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PayRollStudentsReport(decimal?[] facultiesCheckListBox, decimal?[] degreeCheckListBox,
            decimal?[] stutesCheckListBox, decimal?[] levelsCheckListBox, int? YearsComboBox, int? MonthComboBox, int? PayrollComboBox, bool? PaidStutesComboBox, bool? PaidTypeComboBox)
        {


            decimal? faculties = facultiesCheckListBox.Length > 0 ? facultiesCheckListBox[0] : null;
            decimal? degree = degreeCheckListBox.Length > 0 ? degreeCheckListBox[0] : null;
            decimal? levels = levelsCheckListBox.Length > 0 ? levelsCheckListBox[0] : null;
            decimal? stutes = stutesCheckListBox.Length > 0 ? stutesCheckListBox[0] : null;

            var PayRollStudentsList = PayrollDataSource(facultiesCheckListBox, degreeCheckListBox, stutesCheckListBox, levelsCheckListBox, YearsComboBox, MonthComboBox, PayrollComboBox, null, new List<decimal>(), PaidStutesComboBox, PaidTypeComboBox);

            PayRollStudentsList.RemoveAll(x => x.ItemType == "ا عدد الطلاب");

            Session["PayRollStudentsData"] = PayRollStudentsList;

            var report = new PayRollStudentsRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                if (facultiesCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Faculty"].Value = dbSC.usp_getFaculties().FirstOrDefault(x => x.FACULTY_NO == faculties).FACULTY_NAME;
                }
                else
                {
                    ((XtraReport)s).Parameters["Faculty"].Value = "";
                }

                if (degreeCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Degree"].Value = dbSC.usp_getDegrees().FirstOrDefault(x => x.DEGREE_CODE == degree).DEGREE_DESC;
                }
                else
                {
                    ((XtraReport)s).Parameters["Degree"].Value = "";
                }

                if (levelsCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Level"].Value = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.LEVEL_CODE == levels).LEVEL_DESC;
                }
                else
                {
                    ((XtraReport)s).Parameters["Level"].Value = "";
                }

                if (stutesCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Stutes"].Value = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STATUS_CODE == stutes).STATUS_DESC;
                }
                else
                {
                    ((XtraReport)s).Parameters["Stutes"].Value = "";
                } ((XtraReport)s).Parameters["Year"].Value = YearsComboBox;
                ((XtraReport)s).Parameters["Month"].Value = MonthComboBox;
                ((XtraReport)s).Parameters["PayrollNumber"].Value = GetStudentPayroll().FirstOrDefault(x => x.ID == PayrollComboBox).Payrollltem;
                ((XtraReport)s).DataSource = PayRollStudentsList;
            };

            return PartialView("_PayRollStudentsReportDesignerPartial", report);
        }


        public ActionResult PayRollStudentsReportExport(string facultiesCheckListBox, string degreeCheckListBox,
           string stutesCheckListBox, string levelsCheckListBox, int? YearsComboBox, int? MonthComboBox, int? PayrollComboBox)
        {
            var report = new PayRollStudentsRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["PayRollStudentsData"];
            };
            return DocumentViewerExtension.ExportTo(report, Request);
        }

        public ActionResult PayRollStudentsReportCallBack(string facultiesCheckListBox, string degreeCheckListBox,
                 string stutesCheckListBox, string levelsCheckListBox, int? YearsComboBox, int? MonthComboBox, int? PayrollComboBox)
        {
            var report = new PayRollStudentsRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["PayRollStudentsData"];
            };

            return PartialView("_PayRollStudentsReportDesignerPartial", report);
        }



        //PayRoll Students By Degree
        public ActionResult PayRollByDegreeIndex()
        {
            ViewBag.Faculties = dbSC.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Degrees = dbSC.usp_getDegrees().Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Stutes = dbSC.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.STATUS_DESC, Value = x.STATUS_CODE.ToString() }).Where(x => x.Text != null).Distinct().ToList();

            ViewBag.Levels = dbSC.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.LEVEL_DESC, Value = x.LEVEL_CODE.ToString() }).Where(x => x.Text != null).OrderBy(x => x.Value).Distinct().ToList();


            List<int> Years = new List<int>();
            DateTime startYear = DateTime.Now;
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                Years.Add(i);
            }
            ViewBag.Years = Years;

            var permissions = GetPermissionsFn(53);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult PayRollByDegreeReport(decimal?[] facultiesCheckListBox, decimal?[] degreeCheckListBox,
    decimal?[] stutesCheckListBox, decimal?[] levelsCheckListBox, int? YearsComboBox, int? MonthComboBox, int? PayrollComboBox, bool? PaidStutesComboBox, bool? PaidTypeComboBox)
        {

            decimal? faculties = facultiesCheckListBox.Length > 0 ? facultiesCheckListBox[0] : null;
            decimal? degree = degreeCheckListBox.Length > 0 ? degreeCheckListBox[0] : null;
            decimal? stutes = stutesCheckListBox.Length > 0 ? stutesCheckListBox[0] : null;

            var PayRollStudentsList = PayrollDataSource(facultiesCheckListBox, degreeCheckListBox, stutesCheckListBox, new decimal?[] { }, YearsComboBox, MonthComboBox, PayrollComboBox, null, new List<decimal>(), PaidStutesComboBox, PaidTypeComboBox);

            PayRollStudentsList.RemoveAll(x => x.ItemType == "توقيع");


            Session["PayRollByDegreeData"] = PayRollStudentsList;

            var report = new PayRollStudentsByDegreesRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                if (facultiesCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Faculty"].Value = dbSC.usp_getFaculties().FirstOrDefault(x => x.FACULTY_NO == faculties).FACULTY_NAME;
                }
                else
                {
                    ((XtraReport)s).Parameters["Faculty"].Value = "";
                }

                if (degreeCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Degree"].Value = dbSC.usp_getDegrees().FirstOrDefault(x => x.DEGREE_CODE == degree).DEGREE_DESC;
                }
                else
                {
                    ((XtraReport)s).Parameters["Degree"].Value = "";
                }

                if (stutesCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Stutes"].Value = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STATUS_CODE == stutes).STATUS_DESC;
                }
                else
                {
                    ((XtraReport)s).Parameters["Stutes"].Value = "";
                }
                ((XtraReport)s).Parameters["Year"].Value = YearsComboBox;
                ((XtraReport)s).Parameters["Month"].Value = MonthComboBox;
                ((XtraReport)s).Parameters["PayrollNumber"].Value = GetStudentPayroll().FirstOrDefault(x => x.ID == PayrollComboBox).Payrollltem;

                ((XtraReport)s).DataSource = PayRollStudentsList;
            };

            return PartialView("_PayRollByDegreeReportDesignerPartial", report);
        }

        public ActionResult PayRollByDegreeReportExport(string facultiesCheckListBox, string degreeCheckListBox,
           string stutesCheckListBox, int? YearsComboBox, int? MonthComboBox, int? PayrollComboBox)
        {
            var report = new PayRollStudentsByDegreesRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["PayRollByDegreeData"];
            };
            return DocumentViewerExtension.ExportTo(report, Request);
        }

        public ActionResult PayRollByDegreeReportCallBack(string facultiesCheckListBox, string degreeCheckListBox,
         string stutesCheckListBox, int? YearsComboBox, int? MonthComboBox, int? PayrollComboBox)
        {
            var report = new PayRollStudentsByDegreesRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["PayRollByDegreeData"];
            };

            return PartialView("_PayRollByDegreeReportDesignerPartial", report);
        }

        //PayRoll Students By Faculties
        public ActionResult PayRollByFacultiesIndex()
        {
            ViewBag.Faculties = dbSC.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Degrees = dbSC.usp_getDegrees().Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Stutes = dbSC.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.STATUS_DESC, Value = x.STATUS_CODE.ToString() }).Where(x => x.Text != null).Distinct().ToList();

            ViewBag.Levels = dbSC.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.LEVEL_DESC, Value = x.LEVEL_CODE.ToString() }).Where(x => x.Text != null).OrderBy(x => x.Value).Distinct().ToList();


            List<int> Years = new List<int>();
            DateTime startYear = DateTime.Now;
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                Years.Add(i);
            }
            ViewBag.Years = Years;

            var permissions = GetPermissionsFn(55);
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

        public ActionResult PayRollByFacultiesReport(decimal?[] facultiesCheckListBox, decimal?[] degreeCheckListBox,
        decimal?[] stutesCheckListBox, decimal?[] levelsCheckListBox, int? YearsComboBox, int? MonthComboBox, int? PayrollComboBox, bool? PaidStutesComboBox, bool? PaidTypeComboBox)
        {

            decimal? faculties = facultiesCheckListBox.Length > 0 ? facultiesCheckListBox[0] : null;
            decimal? degree = degreeCheckListBox.Length > 0 ? degreeCheckListBox[0] : null;
            decimal? stutes = stutesCheckListBox.Length > 0 ? stutesCheckListBox[0] : null;

            var PayRollStudentsList = PayrollDataSource(facultiesCheckListBox, degreeCheckListBox, stutesCheckListBox, new decimal?[] { }, YearsComboBox, MonthComboBox, PayrollComboBox, null, new List<decimal>(), PaidStutesComboBox, PaidTypeComboBox);

            PayRollStudentsList.RemoveAll(x => x.ItemType == "توقيع");

            Session["PayRollByFacultiesData"] = PayRollStudentsList;

            var report = new PayRollStudentsByFacultiesRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                if (facultiesCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Faculty"].Value = dbSC.usp_getFaculties().FirstOrDefault(x => x.FACULTY_NO == faculties).FACULTY_NAME;
                }
                else
                {
                    ((XtraReport)s).Parameters["Faculty"].Value = "";
                }

                if (degreeCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Degree"].Value = dbSC.usp_getDegrees().FirstOrDefault(x => x.DEGREE_CODE == degree).DEGREE_DESC;
                }
                else
                {
                    ((XtraReport)s).Parameters["Degree"].Value = "";
                }

                if (stutesCheckListBox.Length == 1)
                {
                    ((XtraReport)s).Parameters["Stutes"].Value = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STATUS_CODE == stutes).STATUS_DESC;
                }
                else
                {
                    ((XtraReport)s).Parameters["Stutes"].Value = "";
                } ((XtraReport)s).Parameters["Year"].Value = YearsComboBox;
                ((XtraReport)s).Parameters["Month"].Value = MonthComboBox;
                ((XtraReport)s).Parameters["PayrollNumber"].Value = GetStudentPayroll().FirstOrDefault(x => x.ID == PayrollComboBox).Payrollltem;
                ((XtraReport)s).DataSource = PayRollStudentsList;
            };

            return PartialView("_PayRollByFacultiesReportDesignerPartial", report);
        }

        public ActionResult PayRollByFacultiesReportExport(string facultiesCheckListBox, string degreeCheckListBox,
           string stutesCheckListBox, int? YearsComboBox, int? MonthComboBox, int? PayrollComboBox)
        {

            var report = new PayRollStudentsByFacultiesRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["PayRollByFacultiesData"];
            };
            return DocumentViewerExtension.ExportTo(report, Request);
        }

        public ActionResult PayRollByFacultiesReportCallBack(string facultiesCheckListBox, string degreeCheckListBox,
        string stutesCheckListBox, int? YearsComboBox, int? MonthComboBox, int? PayrollComboBox)
        {
            var report = new PayRollStudentsByFacultiesRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["PayRollByFacultiesData"];
            };

            return PartialView("_PayRollByFacultiesReportDesignerPartial", report);
        }



        //PayRoll Students By Student
        public ActionResult PayRollByStudentIndex()
        {
            var permissions = GetPermissionsFn(46);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        public PartialViewResult StudentsComboBoxPartial(string Text)
        {
            var students = dbSC.INTEGRATION_All_Students.Where(x => x.STATUS_CODE == 1 && x.STUDENT_NAME.Contains(Text)
                            || x.STUDENT_ID.ToString() == Text || x.NATIONAL_ID == Text)
                                    .Select(x => new SelectListItem
                                    {
                                        Value = x.STUDENT_ID.ToString(),
                                        Text = x.STUDENT_NAME
                                    }).ToList();
            return PartialView("_StudentsComboBoxPartial", students);
        }

        public ActionResult PayRollByStudentReport(string StudentsComboBox)
        {
            var PayRollStudentsList = PayrollDataSource(new decimal?[] { }, new decimal?[] { }, new decimal?[] { }, new decimal?[] { }, 0, 0, 0, int.Parse(StudentsComboBox), new List<decimal>(), null, null);


            PayRollStudentsList.RemoveAll(x => x.ItemType == "ا عدد الطلاب");
            PayRollStudentsList.RemoveAll(x => x.ItemType == "توقيع");

            Session["PayRollByStudentData"] = PayRollStudentsList;
            var report = new PayRollStudentsByStudentRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).Parameters["StudentId"].Value = dbSC.INTEGRATION_All_Students.SingleOrDefault(x => x.STUDENT_ID.ToString() == StudentsComboBox).STUDENT_ID;

                ((XtraReport)s).Parameters["StudentName"].Value = dbSC.INTEGRATION_All_Students.SingleOrDefault(x => x.STUDENT_ID.ToString() == StudentsComboBox).STUDENT_NAME;

                ((XtraReport)s).Parameters["NationalId"].Value = dbSC.INTEGRATION_All_Students.SingleOrDefault(x => x.STUDENT_ID.ToString() == StudentsComboBox).NATIONAL_ID;

                ((XtraReport)s).Parameters["Faculty"].Value = dbSC.INTEGRATION_All_Students.SingleOrDefault(x => x.STUDENT_ID.ToString() == StudentsComboBox).FACULTY_NAME;

                ((XtraReport)s).Parameters["Degree"].Value = dbSC.INTEGRATION_All_Students.SingleOrDefault(x => x.STUDENT_ID.ToString() == StudentsComboBox).DEGREE_DESC;

                ((XtraReport)s).Parameters["Level"].Value = dbSC.INTEGRATION_All_Students.SingleOrDefault(x => x.STUDENT_ID.ToString() == StudentsComboBox).LEVEL_DESC;

                ((XtraReport)s).DataSource = PayRollStudentsList;
            };

            return PartialView("_PayRollByStudentReportDesignerPartial", report);
        }

        public ActionResult PayRollByStudentReportExport(string StudentsComboBox)
        {
            var report = new PayRollStudentsByStudentRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["PayRollByStudentData"];
            };
            return DocumentViewerExtension.ExportTo(report, Request);
        }

        public ActionResult PayRollByStudentReportCallBack(string StudentsComboBox)
        {
            var report = new PayRollStudentsByStudentRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["PayRollByStudentData"];
            };

            return PartialView("_PayRollByStudentReportDesignerPartial", report);
        }

        public ActionResult DownLoadSarafFile()
        {
            return View();
        }

        public ActionResult DownLoadSaraf(decimal?[] facultiesCheckListBox, decimal?[] degreeCheckListBox, decimal?[] StudentsCheckListBox,
         int? YearsComboBox, int? MonthComboBox, int PayrollComboBox, DateTime CachCalender, string CachTypeComboBox)
        {

            var errorMessage = "";
            var sarafResult = CalculateSaraf(facultiesCheckListBox, degreeCheckListBox, StudentsCheckListBox, new decimal?[] { }, YearsComboBox, MonthComboBox, PayrollComboBox, ref errorMessage, CachCalender, CachTypeComboBox, StudentsCheckListBox.ToList(), null, null);




            if (errorMessage != "")
                return Content(errorMessage);





            var memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream, Encoding.Default);

            tw.WriteLine(sarafResult.ToString());
            tw.Flush();
            tw.Close();

            return File(memoryStream.GetBuffer(), "text/plain", "file.txt");

        }



        public StringBuilder CalculateSaraf(decimal?[] facultiesIds, decimal?[] degreeIds,
         decimal?[] statusesIds, decimal?[] levelsIds, int? yearsId, int? monthId, int payrollNumber, ref string errorMessage, DateTime paymentDate, string operationId, List<decimal?> studentsId, bool? PaidStutesComboBox, bool? PaidTypeComboBox)

        {
            var payroll = PayrollDataSource(facultiesIds, degreeIds, statusesIds, levelsIds, yearsId, monthId, payrollNumber, null, new List<decimal>(), PaidStutesComboBox, PaidTypeComboBox);



            payroll.RemoveAll(x => x.AccountNumber == null || x.NationalID == null);


            if (payroll.Count < 1)
            {
                errorMessage = @"عفوا لا يوجد طلبه";
                return new StringBuilder();
            }


            var studentsDistinctIds = payroll.Select(x => x.StudentID).Distinct();


            var payRollStringBuilder = new StringBuilder();

            var sarafList = new List<DAL.Models.SarafDownloaded>();


            foreach (var studentId in studentsDistinctIds)
            {

                var payRollLineStringBuilder = new StringBuilder();
                var sarafRow = new DAL.Models.SarafDownloaded();
                sarafRow.PayrollNumber = payrollNumber;
                sarafRow.StudentId = studentId.ToString();
                //(12) الرقم الوظيفى

                payRollLineStringBuilder.Append(AppendZeros(12, studentId.ToString()));
                sarafRow.NationalId = payroll.First(x => x.StudentID == studentId).NationalID.Trim();


                //(5) خمسه اصفار
                payRollLineStringBuilder.Append(new string('0', 5));




                //(19) رقم بطاقه الصراف

                payRollLineStringBuilder.Append(AppendZeros(19, payroll.First(x => x.StudentID == studentId).AccountNumber.Trim()));
                sarafRow.AccountNumber = payroll.First(x => x.StudentID == studentId).AccountNumber.Trim();

                //(50) الاسم

                var studentName = /*payroll.First(x => x.StudentID == studentId).StudentName.Trim() +*/
                                  payroll.First(x => x.StudentID == studentId).StudentNameEng.Trim();
                payRollLineStringBuilder.Append(AppendSpaces(50, studentName));
                sarafRow.StudentName = studentName;




                //(10) رقم الهويه
                payRollLineStringBuilder.Append(AppendSpaces(10, payroll.First(x => x.StudentID == studentId).NationalID.Trim()));



                //(13) اجمالى المبلغ
                var salaryPaid = payroll.Single(x => x.StudentID == studentId && x.ItemName == @"الصافي المستحق").Value;
                salaryPaid = Math.Round(Convert.ToDecimal(salaryPaid), 0, MidpointRounding.AwayFromZero);
                payRollLineStringBuilder.Append(GetDecimalString(13, (decimal)salaryPaid));
                sarafRow.NetValue = (decimal)salaryPaid;





                //(2) صفرين
                payRollLineStringBuilder.Append(new string('0', 2));



                //(8) التاريخ التالى للتاريخ المسجل بمعنى اذا تم تسجيل تاريخ 3\2 يكتب 4\2
                payRollLineStringBuilder.Append(paymentDate.Date.AddDays(1).ToString("yyyyMMdd"));
                sarafRow.TomorowDate = paymentDate.Date.AddDays(1);


                //(1) كود العمليه ( يوجد اكثر من عمليه ويوجد لها كود ثابت مذكوره فى اسفل الملف ) وهذه ستكون فى شاشه تصدير ملف الصراف عند اختيار الكفاله والشهر والبنك والحساب ونوع العمليه ونخليها افتراضى على تغذيه البطاقه
                payRollLineStringBuilder.Append(operationId);


                //(6) سته اصفار
                payRollLineStringBuilder.Append(new string('0', 6));


                //(10) مسافات
                payRollLineStringBuilder.Append(new string(' ', 10));



                var PhoneNumber = "";

                if (!string.IsNullOrWhiteSpace(payroll.First(x => x.StudentID == studentId).MobileNumber))
                    PhoneNumber = payroll.First(x => x.StudentID == studentId).MobileNumber.Trim();
                else
                {
                    if (!string.IsNullOrWhiteSpace(payroll.First(x => x.StudentID == studentId).MobilePhone))
                        PhoneNumber = payroll.First(x => x.StudentID == studentId).MobilePhone.Trim();
                }


                if (PhoneNumber.Length > 9)
                    PhoneNumber = PhoneNumber.Substring(PhoneNumber.Length - 9);


                payRollLineStringBuilder.Append(AppendSpaces(10, PhoneNumber));
                sarafRow.MobileNumber = PhoneNumber;


                var basicSalary = (decimal)0;

                basicSalary = Math.Round(basicSalary, 0, MidpointRounding.AwayFromZero);
                payRollLineStringBuilder.Append(GetDecimalString(10, basicSalary));
                sarafRow.BasicSalary = basicSalary;



                //(2) صفرين
                payRollLineStringBuilder.Append(new string('0', 2));




                //(10) بدل السكن
                decimal allowanceHouse = 0;
                allowanceHouse = Math.Round(allowanceHouse, 0, MidpointRounding.AwayFromZero);
                payRollLineStringBuilder.Append(GetDecimalString(10, allowanceHouse));
                sarafRow.AllowanceHouse = 0;
                //(2) صفرين
                payRollLineStringBuilder.Append(new string('0', 2));


                //(10) بدلات اخرى
                decimal OtherAllowence = 0;

                OtherAllowence = Math.Round(OtherAllowence, 0, MidpointRounding.AwayFromZero);
                payRollLineStringBuilder.Append(GetDecimalString(10, OtherAllowence));
                sarafRow.OtherAllowence = OtherAllowence;




                //(2) صفرين
                payRollLineStringBuilder.Append(new string('0', 2));



                //(10) الاستقطاعات

                var deductions = (decimal)0;
                deductions = Math.Round(deductions, 0, MidpointRounding.AwayFromZero);
                payRollLineStringBuilder.Append(GetDecimalString(10, deductions));
                sarafRow.deductions = deductions;



                //(2) صفرين
                payRollLineStringBuilder.Append(new string('0', 2));




                payRollStringBuilder.Append(payRollLineStringBuilder);
                payRollStringBuilder.Append(Environment.NewLine);

                sarafRow.LineAsDownloaded = payRollLineStringBuilder.ToString();
                sarafList.Add(sarafRow);

            }

            dbSC.SarafDownloaded.AddRange(sarafList);
            dbSC.SaveChanges();

            return payRollStringBuilder;
        }






        private string AppendZeros(int originalLength, string text)
        {
            int textLength = text.Length;
            string newtext = new string('0', originalLength - textLength);
            newtext += text;
            return newtext;
        }


        private string AppendSpaces(int length, string value)
        {
            string Text = value;
            string _Text = new string(' ', length - Text.Length);
            Text += _Text;
            return Text;
        }


        private string GetDecimalString(int length, decimal value)
        {
            var totalAmountParts = $"{value:0.00}";
            var parts = totalAmountParts.Split('.');
            var integerValue = int.Parse(parts[0]);
            var decimalFraction = parts[1];
            decimalFraction = decimalFraction.Substring(0, 2);
            var _TotalAmount = new string('0', length - ((integerValue + decimalFraction).Length));
            _TotalAmount += (integerValue + decimalFraction);
            return _TotalAmount;
        }

        //عرض بيانات الصرف

        public ActionResult SarafFileIndex()
        {
            ViewBag.Faculties = dbSC.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Degrees = dbSC.usp_getDegrees().Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Stutes = dbSC.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.STATUS_DESC, Value = x.STATUS_CODE.ToString() }).Where(x => x.Text != null).Distinct().ToList();

            ViewBag.Levels = dbSC.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.LEVEL_DESC, Value = x.LEVEL_CODE.ToString() }).Where(x => x.Text != null).OrderBy(x => x.Value).Distinct().ToList();

            ViewBag.Students = dbSC.INTEGRATION_All_Students.Where(x => x.STATUS_CODE == 1).Select(x => new SelectListItem { Value = x.STUDENT_ID.ToString(), Text = x.STUDENT_NAME }).ToList();


            List<int> Years = new List<int>();
            DateTime startYear = DateTime.Now;
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                Years.Add(i);
            }
            ViewBag.Years = Years;

            var permissions = GetPermissionsFn(48);
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
                else if (permission == "قراءة" )
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



    public class PayRollStudentsReportVM
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string StudentNameEng { get; set; }
        public string NationalID { get; set; }
        public decimal? StudentFaculty { get; set; }
        public decimal? StudentDegree { get; set; }
        public decimal Stutes { get; set; }
        public decimal? Levels { get; set; }
        public int Years { get; set; }
        public int Month { get; set; }
        public int Days { get; set; }
        public decimal Value { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int ItemTypeId { get; set; }

        public string DegreeName { get; set; }
        public decimal? LevelName { get; set; }

        public string Faculty { get; set; }
        public string Degree { get; set; }
        public int? PayrollNumber { get; set; }
        public string PayrollNumberwithDate { get; set; }
        public string MobileNumber { get; set; }
        public string MobilePhone { get; set; }
        public string Level { get; set; }
        public string AccountNumber { get; set; }

        public bool? IsPaid { get; set; }
        public bool? IsAccountNumber { get; set; }

    }
}