using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
// ReSharper disable SpecifyACultureInStringConversionExplicitly


namespace StudentProfile.Web.Controllers
{

    public partial class CalculationBonusController : Controller
    {
        private notify _notify;
        private readonly SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();

        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(50);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read && _payrollPhaseUser != null)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult GetFaculties()
        {
            return Json(db.usp_getFaculties()
                                            .Select(selector: x => new SelectListItem
                                            {
                                                Text = x.FACULTY_NAME,
                                                Value = x.FACULTY_NO.ToString()
                                            }).ToList(),
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDegrees()
        {
            return Json(db.usp_getDegrees()
                                            .Select(selector: x => new SelectListItem
                                            {
                                                Text = x.DEGREE_DESC,
                                                Value = x.DEGREE_CODE.ToString()
                                            })
                                            .OrderBy(keySelector: x => x.Text).ToList(),
                       JsonRequestBehavior.AllowGet);
        }

        //Please DO NOT Change this Action
        public ActionResult GetStudents(string degreeIds, string facultyIds , int? skip, int? take ,int? studentID)
        {

            var students = db.SP_GetCurrentandLatestGraduatedStudents(facultyIds, degreeIds, (DateTime.Now.Year).ToString()).Where(x => studentID == null || x.STUDENT_ID == studentID).ToList();
            return Json(students.Skip(skip.HasValue ? skip.Value : 0).Take(take.HasValue ? take.Value : students.Count())
                 , JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStudentsData(string degreeIds, string facultyIds,string StdCodes)
        {
            if (String.IsNullOrEmpty(StdCodes))
            {

                var students = db.SP_GetCurrentandLatestGraduatedStudents(facultyIds, degreeIds, (DateTime.Now.Year).ToString()).Select(s => s.STUDENT_ID).ToList();
                return Json(students
                     , JsonRequestBehavior.AllowGet);
            }
            else
            {

                var students = db.SP_GetNotCurrentStudents(facultyIds, degreeIds, StdCodes).Select(s => s.STUDENT_ID).ToList();
                return Json(students
                     , JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPayRollNumber()
        {
            StringBuilder builder = new StringBuilder();
            //var toDay = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            //builder.Append(toDay);
            builder.Append("(");
            var payRollNumber = db.Payroll.Count() > 0 ? db.Payroll.Max(x => x.PayrollNumber) + 1 : 1;
            builder.Append(payRollNumber.ToString());
            builder.Append(")");

            return Json(data: builder.ToString(), behavior: JsonRequestBehavior.AllowGet);
        }

        //GET: AllowancesOrRewardItems
        [HttpGet]
        public async Task<ActionResult> GetAllRewardItems(string RewardItemExpensesType)
        {
            var db = new SchoolAccGam3aEntities();

            if (string.IsNullOrWhiteSpace(RewardItemExpensesType))
            {
                return Json(await Task.Run(() => db.RewardItems.Where(x => x.RewardItemExpensesTypes.RewardItemExpensesTypeName != "عند الطلب").Select(x => new
                {
                    AllowanceId = x.ID,
                    AllowanceName = x.RewardItemName_Arb + "-" + x.RewardItemTypes.RewardItemTypeName,
                    RewardItemExpensesTypeName = x.RewardItemExpensesTypes.RewardItemExpensesTypeName
                }).Distinct().ToList()), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(await Task.Run(() => db.RewardItems.Where(x => x.RewardItemExpensesTypes.RewardItemExpensesTypeName == RewardItemExpensesType).Select(x => new
                {
                    AllowanceId = x.ID,
                    AllowanceName = x.RewardItemName_Arb + "-" + x.RewardItemTypes.RewardItemTypeName,
                    RewardItemExpensesTypeName = x.RewardItemExpensesTypes.RewardItemExpensesTypeName
                }).Distinct().ToList()), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CalculateStudentReward(CalculationRewardVm model)
        {
            try
            {
                var listStudentPayroll = new List<StudentPayroll>();
                var listPayrollRefusal = new List<PayrollRefusal>();

                var payrollDate = Convert.ToDateTime(model.BounsDate);
                db.Database.CommandTimeout = 0;
                var stds = string.Join(",", model.StudentIds);
                var result = db.Usp_CalcStudentsRewards(stds, payrollDate, model.RewardItems ).FirstOrDefault();
            
                return Json(_notify = new notify() { Message = $"{ result }", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(_notify = new notify() { Message = "حدث خطأ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CalculateStudentRewardForAcaedmy(CalculationRewardVm model)
        {
            try
            {
                var listStudentPayroll = new List<StudentPayroll>();
                var listPayrollRefusal = new List<PayrollRefusal>();

                var payrollDate = Convert.ToDateTime(model.BounsDate);
                db.Database.CommandTimeout = 0;
                var stds = string.Join(",", model.StudentIds);
                //  var result = db.Usp_CalcRewardsForNonUsualStudents(stds, payrollDate, model.RewardItems).FirstOrDefault();

                var result = db.Usp_CalcRewardsForNonUsualStudents(stds, payrollDate, model.RewardItems);
                return Json(_notify = new notify() { Message = $"{ result }", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(_notify = new notify() { Message = "حدث خطأ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }
        private PayrollRefusal GetPayrollRefusal(decimal studentId, DateTime payrollDate, int payrollNumber, int? Item_ID, string refusalReason)
        {
            var payrollRefusal = new PayrollRefusal
            {
                StudentId = (int)studentId,
                PayrollDate = payrollDate,
                InsertDate = DateTime.Now,
                PayrollNumber = payrollNumber,
                Item_ID = Item_ID
            };
            payrollRefusal.RefusalReason = refusalReason;
            return payrollRefusal;
        }

        private int? CheckExpensesTypeById(int RewardItemId)
        {
            var db = new SchoolAccGam3aEntities();
            return db.RewardItems.FirstOrDefault(x => x.ID == RewardItemId).RewardItemTypes.RewardItemType;
        }

        public bool IsAvailiable()
        {
            if (Session["UserId"] != null)
            {
                var CurrentUser = Session["UserId"] as DashBoard_Users;
                var CheckUsers = db.PayrollPhasesUsers.Include("PayrollPhases").Where(m => m.UserID == CurrentUser.ID && m.IsActive == true && m.PayrollPhases.IsActive ==true && m.PayrollPhases.PhaseOrder == 1).Count();
                if (CheckUsers >0 )
                    return true;
                else
                    return false;
            }
            return false;

        }

        public ActionResult GetStudentPayroll()
        {
            List<PayRollStudentsList> studentPayrollList = new List<PayRollStudentsList>();
            if (IsAvailiable())
            {
                studentPayrollList = db.Payroll.Where(x => ((x.PayrollApproval.Any(p => p.IsRefused == true) && x.IsPosted == true) || (x.PayrollApproval.Count() == 0 && x.IsPosted != true)) && x.IsActive == true).AsEnumerable()
                  .Select(g => new PayRollStudentsList
                  {
                      ID = g.ID,
                      PayrollNumber = g.PayrollNumber,
                      Payrollltem = g.IssueDate.Day + "-" + g.IssueDate.Month + "-" + g.IssueDate.Year + "-(" + g.PayrollNumber + ")",
                      Date = g.IssueDate.ToString("dd/MM/yyyy"),
                      TotalValue = g.TotalAmount,
                      AdvancesValue = g.StudentPayroll.Where(x=>x.RewardItems.IsAdvanceReturn==true).Sum(x=>x.Value),
                      StudentsCount = g.StudentPayroll.Select(x=>x.StudentID).Distinct().Count(),
                      MinisterValue = g.IsMinister==true?"مسير وزارة":"مسير طلاب",
                      Notes = g.PayrollApproval.Where(x => x.IsRefused == true).OrderByDescending(x => x.ID).LastOrDefault()?.Notes,
                      IsRefused = (g.PayrollApproval.Count > 0 && g.PayrollApproval.Where(x => x.IsRefused == true).OrderByDescending(x => x.ID).LastOrDefault().IsRefused == true) ? true : false
                  }).ToList();
            }
          

            return Json(data: studentPayrollList, behavior: JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckIfAdvanceItemAlreadySelected(List<int> selectedItemList)
        {
            return Json(db.RewardItems.Any(x => selectedItemList.Any(p => p == x.ID && x.IsAdvanceReturn == true)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PostPayrollForApproval(int payrollNumber)
        {
            string message = "";

            try
            {
                var payroll = db.Payroll.Where(x => x.PayrollNumber == payrollNumber).SingleOrDefault();
                payroll.IsPosted = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = "حدث خطأ أثناء الحفظ";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult ExportStudentPayrollPDF()
        {
            List<PayRollStudentsList> PayRollStudentsList = new List<PayRollStudentsList>();


            var studentPayrollList = db.Payroll.Select(x => new PayRollStudentsList
            {
                PayrollNumber = x.PayrollNumber,
                IssueDatePayroll = x.IssueDate,
                Payrollltem = x.IssueDate.Day + "-" + x.IssueDate.Month + "-" + x.IssueDate.Year + "-(" + x.PayrollNumber + ")"
            }).Distinct().ToList();

            PayRollStudentsList.AddRange(studentPayrollList);

            return PartialView("_ExportStudentPayroll", PayRollStudentsList);
        }

        //public ActionResult StudentPayrollPDF()
        //{
        //    return new Rotativa.ActionAsPdf("ExportStudentPayrollPDF");
        //}

        public ActionResult DeletePayroll(int payrollNumber)
        {
            int result = 0;
            if (payrollNumber > 0)
            {
                var payroll = db.Payroll.Where(x => x.PayrollNumber == payrollNumber).SingleOrDefault();
                var payrollApproval = payroll.PayrollApproval.FirstOrDefault();
                
                if (payrollApproval != null)
                {
                    payroll.IsActive = false;
                    payrollApproval.IsApproved = false;
                    payrollApproval.IsRefused = true;
                    payrollApproval.Notes = $"تم حذف المسير من قبل {_payrollPhaseUser.DashBoard_Users.Name}";

                    dbSC.Entry(payroll).State = EntityState.Modified;
                    dbSC.Entry(payrollApproval).State = EntityState.Modified;
                    dbSC.SaveChanges();
                }
                else
                {
                      result=dbSC.usp_delete_payroll(payroll.ID);
                }
                if(result > 0)
                return Json(_notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
               else  return Json(_notify = new notify() { Message = "خطأ اثناء حذف المسير", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(_notify = new notify() { Message = "خطأ اثناء حذف المسير", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllRegisterTypes()
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.usp_getStatus().Where(m=>m.STATUS_CODE!=1).Select(x => new
            {
                RegisterTypeId = x.STATUS_CODE.ToString(),
                RegisterTypeName = x.STATUS_DESC
            }).ToList()), JsonRequestBehavior.AllowGet);
        }

        private bool CheckIfAdvanceItem(int itemid)
        {
            return db.RewardItems.Any(x => x.ID == itemid && x.IsAdvanceReturn == true);
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
        private PayrollPhasesUsers _payrollPhaseUser
        {
            get
            {
                if (Session["UserId"] != null)
                {
                    var CurrentUser = Session["UserId"] as DashBoard_Users;
                    return db.PayrollPhasesUsers.Where(x => x.UserID == CurrentUser.ID && (x.PayrollPhases.PhaseOrder == 1 && x.PayrollPhases.IsActive == true)).SingleOrDefault();
                };
                return null;
            }
        }

        private List<AdvanceStudentReturn> AdvancePartReturningByPayroll(int payrollID, int? studentID)
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
                   var advances = db.AdvancePaymentDetails.Where(p => p.AdvanceRequests.AdvanceSettings.AdvanceType=="A"
                   && p.AdvancePaymentMaster.Student_Id == x.Key && (p.AdvanceReceiveDetails.Count()==0 || p.AdvanceReceiveDetails
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

    public class CalculationRewardVm
    {
        public List<decimal> StudentIds { get; set; }
        public string BounsDate { get; set; }
        public string RewardItems { get; set; }
    }
    public class AdvanceStudentReturn
    {
        public int StudentId { get; set; }
        public int AdvancePaymentDetailsId { get; set; }
        public int AdvanceReceiveMaster_Id { get; set; }
        public decimal? Value { get; set; }
    }

    public class PayRollStudentsList
    {
        public int ID { get; set; }
        public int? PayrollNumber { get; set; }
        public DateTime IssueDatePayroll { get; set; }
        public string Payrollltem { get; set; }
        public decimal TotalValue { get; set; }
        public decimal? AdvancesValue { get; set; }
        public decimal? RealValue { get { return this.TotalValue - this.AdvancesValue; } }
        public string Date { get; set; }
        public decimal StudentID { get; set; }
        public string Notes { get; set; }
        public int? StudentsCount { get; set; }
        public bool IsRefused { get; set; }
        public string MinisterValue { get; set; }

        public string DafPayNo { get; set; }

    }

}