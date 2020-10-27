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
namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public partial class CalculationBonusController : Controller
    {

        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();
        public JsonResult GetPayRollsDetails(int payrollID)
        {
            var advances = AdvancePartReturningByPayrollWithoutStudents(payrollID, null);
            //advances.GroupBy(m=>m)
            //var payrollNumber = dbSC.Payroll.Where(x => x.ID == payrollID).SingleOrDefault();
            //var studentIds = payrollNumber.StudentPayroll.Select(x => x.StudentID).ToList();
            //var studentsData = dbSC.INTEGRATION_All_Students.Where(x => studentIds.Any(p => p == x.STUDENT_ID)).ToList();
            //return Json(payrollNumber.StudentPayroll.GroupBy(x => x.StudentID).Select(x => new
            //{

            //    ID = payrollID,
            //    StudentID = x.Key,
            //    StudentName = studentsData.Where(p => p.STUDENT_ID == x.Key).FirstOrDefault().STUDENT_NAME,
            //    StudentNationality = studentsData.Where(p => p.STUDENT_ID == x.Key).FirstOrDefault().NATIONALITY_DESC,

            //    StudentFaculty = studentsData.Where(p => p.STUDENT_ID == x.Key).FirstOrDefault().FACULTY_NAME,

            //    StudentDegree = studentsData.Where(p => p.STUDENT_ID == x.Key).FirstOrDefault().DEGREE_DESC,
            //    StudentNationalID = studentsData.Where(p => p.STUDENT_ID == x.Key).FirstOrDefault().NATIONAL_ID,
            //    StudentStatus = studentsData.Where(p => p.STUDENT_ID == x.Key).FirstOrDefault().STATUS_DESC,
            //    TotalValue = x.Where(cc => cc.RewardItems.IsAdvanceReturn != true).Sum(g => (g.RewardItems.RewardItemTypes.RewardItemType * g.Value)),
            //    AdvanceValue = advances.Where(p => p.StudentId == x.Key).Sum(p => p.Value),
            //    RealValue = x.Where(cc => cc.RewardItems.IsAdvanceReturn != true).Sum(g => (g.RewardItems.RewardItemTypes.RewardItemType * g.Value)) - advances.Where(p => p.StudentId == x.Key).Sum(p => p.Value)

            //}).ToList(), JsonRequestBehavior.AllowGet);
            return null;
        }


        private List<AdvanceStudentReturn> AdvancePartReturningByPayrollWithoutStudents(int payrollID, int? studentID)
        {
            var AvailableAdvancesToselect = new List<AdvanceStudentReturn>();
            var details = db.Payroll.Where(x => x.ID == payrollID).SingleOrDefault()
               .StudentPayroll.Where(x => (studentID == null || x.StudentID == studentID)).ToList();
            details.GroupBy(x => x.StudentID).ToList().ForEach(x =>
            {
                if (x.Any(p => p.RewardItems.IsAdvanceReturn == true))
                {
                    List<AdvanceStudentReturn> advancesToPay = new List<AdvanceStudentReturn>();
                    decimal RewardValue = x.Where(mm => mm.RewardItems.IsAdvanceReturn != true)
                    .Sum(c => c.Value * c.RewardItems.RewardItemTypes.RewardItemType);
                    var advances = db.AdvancePaymentDetails.Where(p => p.AdvanceRequests.AdvanceSettings.AdvanceType == "A"
                    && p.AdvancePaymentMaster.Student_Id == x.Key && (p.AdvanceReceiveDetails.Count() == 0 || p.AdvanceReceiveDetails
                    .Sum(c => c.NetValue) < p.NetValue)).OrderBy(c => c.AdvancePaymentMaster.InsertionDate).ToList();
                    advances.ForEach(p =>
                    {
                        decimal? desiredValue = p.AdvanceRequests.AdvanceSettings.ValueType == "V" ?
                       p.AdvanceRequests.AdvanceSettings.Value > (p.NetValue - p.AdvanceReceiveDetails.Sum(nn => nn.NetValue)) ?
                       (p.NetValue - p.AdvanceReceiveDetails.Sum(nn => nn.NetValue)) : p.AdvanceRequests.AdvanceSettings.Value :
                       p.AdvanceRequests.AdvanceSettings.ValueType == "P" ?
                       (RewardValue * p.AdvanceRequests.AdvanceSettings.Value / 100) >
                        (p.NetValue - p.AdvanceReceiveDetails.Sum(nn => nn.NetValue)) ?
                       (p.NetValue - p.AdvanceReceiveDetails.Sum(nn => nn.NetValue)) :
                        (RewardValue * p.AdvanceRequests.AdvanceSettings.Value / 100) : 0;
                        if (desiredValue > 0 && desiredValue <= RewardValue)
                        {
                            RewardValue = RewardValue - (desiredValue.HasValue ? desiredValue.Value : 0);
                            advancesToPay.Add(new AdvanceStudentReturn
                            {
                                StudentId = x.Key,
                                AdvancePaymentDetailsId = p.ID,
                                AdvanceReceiveMaster_Id = 0,
                                Value = desiredValue
                            });
                        }
                    });
                    AvailableAdvancesToselect.AddRange(advancesToPay);
                }
            });
            return AvailableAdvancesToselect;
        }
    }
}