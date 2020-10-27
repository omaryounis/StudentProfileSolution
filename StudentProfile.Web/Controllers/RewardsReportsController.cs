using DevExpress.XtraReports.UI;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using StudentProfile.Web.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using StudentProfile.Web.Helper;
namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class RewardsReportsController : Controller
    {
        private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();
        public async Task<ActionResult> StatementOfSarfRewardIndex()
        {

            ViewBag.Faculties = await Task.Run(() => db.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Text != null).ToList());

            ViewBag.Degrees = await Task.Run(() => db.usp_getDegrees().Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList());

            ViewBag.Stutes = await Task.Run(() => db.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.STATUS_DESC, Value = x.STATUS_CODE.ToString() }).Where(x => x.Text != null).Distinct().ToList());

            ViewBag.Levels = await Task.Run(() => db.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.LEVEL_DESC, Value = x.LEVEL_CODE.ToString() }).Where(x => x.Text != null).OrderBy(x => x.Value).Distinct().ToList());

            ViewBag.StudentNames = await Task.Run(() => (from studentFromIntegration in db.INTEGRATION_All_Students
                                                         join studentFromStudentPayroll in db.StudentPayroll
                                                         on studentFromIntegration.STUDENT_ID equals studentFromStudentPayroll.StudentID
                                                         select new SelectListItem
                                                         {
                                                             Text = String.Concat(studentFromIntegration.STUDENT_NAME, " _ ",
                                                                    studentFromIntegration.NATIONAL_ID, " _ ", studentFromIntegration.STUDENT_ID),
                                                             Value = studentFromIntegration.STUDENT_ID.ToString()
                                                         }).Distinct().ToList());

            var Years = new List<int>();
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                Years.Add(i);
            }
            ViewBag.Years = Years;

            var permissions = GetPermissionsFn(57);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.View = permissions.View;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read|| permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        public async Task<ActionResult> GetMonths(int YearsComboBox)
        {
            return Json(await Task.Run(() => db.Payroll.Where(x => x.IssueDate.Year == YearsComboBox).Select(x => new SelectListItem { Text = x.IssueDate.Month.ToString(), Value = x.IssueDate.Month.ToString() }).OrderBy(x => x.Text).Distinct().ToList()), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetPayrollNumbers(int YearsComboBox, int MonthComboBox)
        {
            return Json(await Task.Run(() => GetStudentPayroll().Where(x => x.IssueDatePayroll.Year == YearsComboBox && x.IssueDatePayroll.Month == MonthComboBox).Select(x => new SelectListItem { Text = x.Payrollltem, Value = x.PayrollNumber.ToString() }).Distinct().ToList()), JsonRequestBehavior.AllowGet);
        }

        public List<PayRollStudentsList> GetStudentPayroll()
        {
            return db.Payroll.Where(x => x.IsActive == true).Select(x => new PayRollStudentsList
            {
                PayrollNumber = x.PayrollNumber,
                IssueDatePayroll = x.IssueDate,
                Payrollltem = x.IssueDate.Day + "-" + x.IssueDate.Month + "-" + x.IssueDate.Year + "-(" + x.PayrollNumber + ")"
            }).Distinct().ToList();
        }

        public ActionResult StatementOfSarfRewardReport(decimal[] facultiesCheckListBox, decimal[] degreeCheckListBox,
        decimal[] stutesCheckListBox, decimal[] levelsCheckListBox, int YearsComboBox, int MonthComboBox, int? PayrollComboBox, int? RecevieStatusComboBox, decimal[] StudentsCheckListBox)
        {
            string degree = degreeCheckListBox.Length > 0 ? string.Join(",", degreeCheckListBox) : null;
            string status = stutesCheckListBox.Length > 0 ? string.Join(",", stutesCheckListBox) : null;
            string levels = levelsCheckListBox.Length > 0 ? string.Join(",", levelsCheckListBox) : null;
            string faculties = facultiesCheckListBox.Length > 0 ? string.Join(",", facultiesCheckListBox) : null;
            string StudentNames = StudentsCheckListBox.Length > 0 ? string.Join(",", StudentsCheckListBox) : null;

            string PayrollNumbers = PayrollComboBox == null ? string.Join(",", db.Payroll.Where(x => x.IssueDate.Year == YearsComboBox && x.IssueDate.Month == MonthComboBox).Select(x => x.PayrollNumber.ToString()).Distinct().ToList()) : string.Join(",", PayrollComboBox);

            var report = new StatementOfSarfRewardRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = db.Sp_StatementOfSarfReward(faculties, degree, status, StudentNames, levels, PayrollNumbers, RecevieStatusComboBox).ToList();
            };

            return PartialView("_StatementOfSarfRewardPartial", report);
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

            Permissions permissions = new Permissions();
            foreach (var item in perm)
            {
                if (item.Contains("ÇÖÇÝÉ"))
                {
                    permissions.Create = true;
                }
                 if (item.Contains("ÞÑÇÁÉ"))
                {
                    permissions.Read = true;
                }
                 if (item.Contains("ÊÚÏíá"))
                {
                    permissions.Update = true;
                }
                 if (item.Contains("ÍÐÝ"))
                {
                    permissions.Delete = true;
                }
                 if (item.Contains("ÚÑÖ"))
                {
                    permissions.View = true;
                }
                 if (item.Contains("ÍÝÙ"))
                {
                    permissions.Save = true;
                }
            }

            return permissions;
        }
    }
}