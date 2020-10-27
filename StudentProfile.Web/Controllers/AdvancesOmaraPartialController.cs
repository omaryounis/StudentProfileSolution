using Newtonsoft.Json;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static StudentProfile.Web.Controllers.AdvancePaymentController;

namespace StudentProfile.Web.Controllers
{

    public partial class AdvancesController : Controller
    {

        // GET: AdvancesOmaraPartial
        public ActionResult AdvancesPremiumRelay()
        {


            var permissions = GetPermissionsFn(98);
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

        public ActionResult RewardItemPremiumRelay()
        {

            var permissions = GetPermissionsFn(129);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;    
            if (permissions.Read || permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }


        [HttpGet]
        public ActionResult GetAdvancesPremiumRelays()
        {
            var data = dbSch.AdvanceReceiveMaster.Where(x => x.GJH_ID == null && x.IsPosted == null && x.PayrollId != null).ToList().Select(x => new
            {
                x.ID,
                AdvanceReceiveDetailsId = x.AdvanceReceiveDetails.FirstOrDefault().ID,
                AdvanceId = x.AdvanceReceiveDetails.FirstOrDefault().AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingId,
                AdvanceName = x.AdvanceReceiveDetails.FirstOrDefault().AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName,
                x.DocNotes,
                x.DocNumber,
                x.DocHeader,
                MasterID = x.ID,
                InsertionDate = x.InsertionDate.Day + "/" + x.InsertionDate.Month + "/" + x.InsertionDate.Year,
                DocTotalValue = x.TotalValue,
                PaidAccountName = GetCoaName(x.COA_ID),
                StudentAcademicData = dbSch.INTEGRATION_All_Students.Where(p => p.STUDENT_ID == x.Student_Id)
                                                                    .Select(p => new
                                                                    {
                                                                        p.STUDENT_ID,
                                                                        p.STUDENT_NAME,
                                                                        p.FACULTY_NAME,
                                                                        p.NATIONAL_ID
                                                                    }).FirstOrDefault()
            }).ToList();
            return this.JsonMaxLength(data);

        }


        [HttpGet]
        public ActionResult GetRewardItemPremiumRelays()
        { 
            var data = dbSch.USP_GetRewardItemPremiumRelays().Select(x => new
            {
                x.ID,
                ItemName = x.RewardItemName_Arb,
                DocNumber= x.DocNumber ,
                DocHeader=x.DocHeader,
                InsertionDate = x.InsertionDate.ToString("dd/MM/yyyy"),
                DocTotalValue = x.DocTotalValue,
                PaidAccountName = x.COAName,
                STUDENT_ID = x.STUDENT_ID,
                STUDENT_NAME=x.STUDENT_NAME,
                FACULTY_NAME=x.FACULTY_NAME,
                NATIONAL_ID   =x.NATIONAL_ID
            }).ToList();
            return this.JsonMaxLength(data);

        }




        [HttpGet]
        public ActionResult RewardItemPremiumRelaysDetais(int id)
        {
            var GJDDescription = new StringBuilder();
            var _GJDDescription = new StringBuilder();

            GJDDescription.Append($" تحصيل ");

            _GJDDescription.Append($" تحصيل ");

            var customGJDList = new List<CustomGJD>();
            var StudentPayrollList = dbSch.StudentPayroll
                                                 .Where(x => x.ID == id);

            var StudentAcademicData = dbSch.INTEGRATION_All_Students
                                           .Where(p => StudentPayrollList.Any(x => x.StudentID == p.STUDENT_ID))
                                           .Select(p => new
                                           {
                                               p.STUDENT_ID,
                                               p.STUDENT_NAME,
                                               p.FACULTY_NAME,
                                               p.NATIONAL_ID
                                           }).FirstOrDefault();

            var AdvanceCoa = StudentPayrollList.Select(x => x.RewardItems.COAID.Value).FirstOrDefault();

            StudentPayrollList.ToList().ForEach(x =>
            {
                var advanceName = x.RewardItems.RewardItemName_Arb;

                GJDDescription.Append($" {advanceName} ");
                GJDDescription.Append($" لحساب الطالب ");
                GJDDescription.Append($"{  StudentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + " ( " + StudentAcademicData.NATIONAL_ID + " ) " + "-" + "ق.جـ" + " ( " + StudentAcademicData.STUDENT_ID + " ) " }");

                customGJDList.Add(new CustomGJD
                {
                    GJDCreditAmount = 0.00M,
                    GJDDebitAmount = x.Value,
                    AccountName = GetCoaName(AdvanceCoa),
                    GJDDescrition = GJDDescription.ToString()
                });

            });

            _GJDDescription.Append($"{ string.Join(" و ", StudentPayrollList.Select(x => x.RewardItems.RewardItemName_Arb).ToList())}");
            _GJDDescription.Append($" لحساب الطالب ");
            _GJDDescription.Append($"{  StudentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + " ( " + StudentAcademicData.NATIONAL_ID + " ) " + "-" + "ق.جـ" + " ( " + StudentAcademicData.STUDENT_ID + " ) " }");

            customGJDList.Add(new CustomGJD
            {
                GJDDebitAmount = 0.00M,
                GJDCreditAmount = customGJDList.Select(x => x.GJDDebitAmount).DefaultIfEmpty(0).Sum(),
                AccountName = GetCoaName(StudentPayrollList.Select(x =>(int) x.RewardItems.CoaId_RecieveFromPayroll).FirstOrDefault()),
                GJDDescrition = _GJDDescription.ToString()

            });
            return this.JsonMaxLength(customGJDList.ToList());

        }

        [HttpGet]
        public ActionResult AdvancesPremiumRelaysDetais(int id)
        {
            var GJDDescription = new StringBuilder();
            var _GJDDescription = new StringBuilder();

            GJDDescription.Append($" تحصيل ");

            _GJDDescription.Append($" تحصيل ");

            var customGJDList = new List<CustomGJD>();
            var AdvanceReceiveDetailsList = dbSch.AdvanceReceiveDetails
                                                 .Where(x => x.AdvanceReceiveMaster_Id == id);

            var StudentAcademicData = dbSch.INTEGRATION_All_Students
                                           .Where(p => AdvanceReceiveDetailsList.Any(x => x.AdvanceReceiveMaster.Student_Id == p.STUDENT_ID))
                                           .Select(p => new
                                           {
                                               p.STUDENT_ID,
                                               p.STUDENT_NAME,
                                               p.FACULTY_NAME,
                                               p.NATIONAL_ID
                                           }).FirstOrDefault();

            var AdvanceCoa = AdvanceReceiveDetailsList.Select(x => x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.COAID.Value).FirstOrDefault();

            AdvanceReceiveDetailsList.ToList().ForEach(x =>
            {
                var advanceName = x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName;

                GJDDescription.Append($" {advanceName} ");
                GJDDescription.Append($" لحساب الطالب ");
                GJDDescription.Append($"{  StudentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + " ( " + StudentAcademicData.NATIONAL_ID + " ) " + "-" + "ق.جـ" + " ( " + StudentAcademicData.STUDENT_ID + " ) " }");

                customGJDList.Add(new CustomGJD
                {
                    GJDCreditAmount = 0.00M,
                    GJDDebitAmount = x.NetValue,
                    AccountName = GetCoaName(AdvanceCoa),
                    GJDDescrition = GJDDescription.ToString()
                });

            });

            _GJDDescription.Append($"{ string.Join(" و ", AdvanceReceiveDetailsList.Select(x => x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName).ToList())}");
            _GJDDescription.Append($" لحساب الطالب ");
            _GJDDescription.Append($"{  StudentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + " ( " + StudentAcademicData.NATIONAL_ID + " ) " + "-" + "ق.جـ" + " ( " + StudentAcademicData.STUDENT_ID + " ) " }");

            customGJDList.Add(new CustomGJD
            {
                GJDDebitAmount = 0.00M,
                GJDCreditAmount = customGJDList.Select(x => x.GJDDebitAmount).DefaultIfEmpty(0).Sum(),
                AccountName = GetCoaName(AdvanceReceiveDetailsList.Select(x => x.AdvanceReceiveMaster.COA_ID).FirstOrDefault()),
                GJDDescrition = _GJDDescription.ToString()

            });
            return this.JsonMaxLength(customGJDList.ToList());

        }

        [HttpPost]
        public ActionResult SaveAdvancesPremiumRelaysToAccount(AdvancesPremiumRelaysToAccountVN model)
        {
            if (model.paramPostedDate == null)
            {
                return Json(_notify = new notify() { Message = "عفوا لا يمكن إتمام عملية الحفظ حتي يتم إدخال تاريخ ترحيل القيد", Type = "error", status = 500 });
            }

            // بيانات المستخدم الحالي بداخل نظام الحسابات 
            var UserInAcc = dbAcc.Users.FirstOrDefault(x => x.ID == CurrentUser.AccID);
            if (UserInAcc == null)
                return Json(_notify = new notify() { Message = "عفوا لا يمكن إتمام عملية الحفظ حتي يتم ربط بيانات المستخدم بنظام الحسابات العامة", Type = "error", status = 500 });

            // السنة المالية المفتوحة
            int? fsyid = dbAcc.FSY.SingleOrDefault(x => x.COM_ID == UserInAcc.School_ID && x.Is_Open == true &&
                                                        x.FSYStartDate <= model.paramPostedDate && x.FSYEndDate >= model.paramPostedDate)?.FSYID;
            if (fsyid == null)
            {
                return Json(_notify = new notify()
                {
                    Message = " عفوا لا يمكن إتمام عملية الحفظ حيث لا يوجد أي سنة مالية مفتوحة حاليا بداخل نظام الحسابات العامة  " +
                              "أو ربما تاريخ الترحيل يقع خارج نطاق السنة المفتوحة حاليا",
                    Type = "error",
                    status = 500
                });
            }

            // نوع العملية هنا اللي هى قيد يومية 
            var OperationId = GetOperationId().Value;
            var existedBCD = GetBcdId(OperationId);
            if (existedBCD == null)
                return Json(_notify = new notify() { Message = "برجاء مراجعة تهيئة العمليات في الحسابات العامة", Type = "erroe", status = 500 });

            var BCDID = existedBCD.BCDID;

            //  رقم العملية ولا يتكرر 
            var LastSystemNumber = GetLastSystemNumber((int)fsyid, BCDID);

            // السندات التي سوف يتم عمل قيد بها   
            var receiveIds = model.ReceiveIds.Split(',').Select(x => Convert.ToInt32(x)).ToList();

            var advanceReceiveMasterList = dbSch.AdvanceReceiveDetails.Where(x => receiveIds.Any(p => p == x.ID)).ToList();

            // الطرف المدين
            List<GJD> allGJDs = new List<GJD>();
            var debit = advanceReceiveMasterList.GroupBy(x => x.AdvanceReceiveMaster.COA_ID).ToList().Select(x => new GJD
            {
                COA_ID = x.Key,
                Com_ID = UserInAcc.School_ID,
                Fsy_ID = fsyid,
                GJDCreditAmount = 0,
                GJDDebitAmount = x.Sum(p => p.NetValue),
                GJDStatus = true,
                JOB_ID = null,
                GJDDescrition = $"مجموع ما تم سداده من الطلاب المذكورين باجمالي {x.Sum(p => p.NetValue)}"
            });
            allGJDs.AddRange(debit);
            // الطرف الدائن
            var credit = advanceReceiveMasterList.GroupBy(x => new { x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings, x.AdvanceReceiveMaster }).ToList().Select(x =>
              new GJD
              {
                  COA_ID = x.Key.AdvanceSettings.COAID.Value,
                  Com_ID = UserInAcc.School_ID,
                  Fsy_ID = fsyid,
                  GJDCreditAmount = x.Sum(p => p.NetValue),
                  GJDDebitAmount = 0,
                  GJDStatus = true,
                  JOB_ID = null,
                  GJDDescrition = $"سداد {x.Key.AdvanceSettings.AdvanceSettingName} للطالب {GetStudentNameById(x.Key.AdvanceReceiveMaster.Student_Id)} عن مسير {x.Key.AdvanceReceiveMaster.Payroll.PayrollNumber.ToString()} بتاريخ {x.Key.AdvanceReceiveMaster.InsertionDate.ToShortDateString()}"
              });
            allGJDs.AddRange(credit);
            //راس القيد
            GJH gjh = new GJH()
            {
                OPT_ID = BCDID,
                GJHStatus = true,
                USR_ID = UserInAcc.ID,
                GJHRefranceNumber = "",
                InsertDate = DateTime.Now,
                COM_ID = UserInAcc.School_ID,
                GJHSystemNumber = LastSystemNumber,
                GJHOperationDate = model.paramPostedDate,
                GJHDescription = $"اثبات استحقاق اقساط السلف على الطلاب بتاريخ {DateTime.Now.ToShortDateString()}",
                GJHAmount = allGJDs.Sum(c => c.GJDCreditAmount),
                JournalNo = dbAcc.NextJournalEntryNo_Sp(UserInAcc.School_ID, fsyid).Single().Value
            };
            dbAcc.GJH.Add(gjh);
            try
            {
                dbAcc.SaveChanges();
                allGJDs.ForEach(x => x.GJH_ID = gjh.GJHID);
                dbAcc.GJD.AddRange(allGJDs);
                dbAcc.SaveChanges();

                advanceReceiveMasterList.ForEach(x => { x.AdvanceReceiveMaster.GJH_ID = (int?)gjh.GJHID; x.AdvanceReceiveMaster.IsPosted = true; });
                dbSch.SaveChanges();

            }
            catch (Exception ex)
            {
                return Json(_notify = new notify() { Message = "خطأ اثناء ترحيل القيود ", Type = "error", status = 500 });
            }

            return Json(_notify = new notify() { Message = "تم ترحيل القيود بنجاح", Type = "success", status = 200 });
        }

        [HttpPost]
        public ActionResult SaveRewardItemPremiumRelaysToAccount(AdvancesPremiumRelaysToAccountVN model)
        {
            if (model.paramPostedDate == null)
            {
                return Json(_notify = new notify() { Message = "عفوا لا يمكن إتمام عملية الحفظ حتي يتم إدخال تاريخ ترحيل القيد", Type = "error", status = 500 });
            }

            // بيانات المستخدم الحالي بداخل نظام الحسابات 
            var UserInAcc = dbAcc.Users.FirstOrDefault(x => x.ID == CurrentUser.AccID);
            if (UserInAcc == null)
                return Json(_notify = new notify() { Message = "عفوا لا يمكن إتمام عملية الحفظ حتي يتم ربط بيانات المستخدم بنظام الحسابات العامة", Type = "error", status = 500 });

            // السنة المالية المفتوحة
            int? fsyid = dbAcc.FSY.SingleOrDefault(x => x.COM_ID == UserInAcc.School_ID && x.Is_Open == true &&
                                                        x.FSYStartDate <= model.paramPostedDate && x.FSYEndDate >= model.paramPostedDate)?.FSYID;
            if (fsyid == null)
            {
                return Json(_notify = new notify()
                {
                    Message = " عفوا لا يمكن إتمام عملية الحفظ حيث لا يوجد أي سنة مالية مفتوحة حاليا بداخل نظام الحسابات العامة  " +
                              "أو ربما تاريخ الترحيل يقع خارج نطاق السنة المفتوحة حاليا",
                    Type = "error",
                    status = 500
                });
            }

            // نوع العملية هنا اللي هى قيد يومية 
            var OperationId = GetOperationId().Value;
            var existedBCD = GetBcdId(OperationId);
            if (existedBCD == null)
                return Json(_notify = new notify() { Message = "برجاء مراجعة تهيئة العمليات في الحسابات العامة", Type = "erroe", status = 500 });

            var BCDID = existedBCD.BCDID;

            //  رقم العملية ولا يتكرر 
            var LastSystemNumber = GetLastSystemNumber((int)fsyid, BCDID);

            // السندات التي سوف يتم عمل قيد بها   
            var receiveIds = model.ReceiveIds.Split(',').Select(x => Convert.ToInt32(x)).ToList();

            var advanceReceiveMasterList = dbSch.StudentPayroll.Where(x => receiveIds.Any(p => p == x.ID)).ToList();

            // الطرف المدين
            List<GJD> allGJDs = new List<GJD>();
            var debit = advanceReceiveMasterList.GroupBy(x =>new {x.StudentID,x.PayrollID, x.RewardItems.COAID }).ToList().Select(x => new GJD
            {
                COA_ID = x.Key.COAID.Value,
                Com_ID = UserInAcc.School_ID,
                Fsy_ID = fsyid,
                GJDCreditAmount = 0,
                GJDDebitAmount = x.Sum(p => p.Value),
                GJDStatus = true,
                JOB_ID = null,
                GJDDescrition = $"مجموع ما تم سداده من الطلاب المذكورين باجمالي {x.Sum(p => p.Value)}"
            });
            allGJDs.AddRange(debit);
            // الطرف الدائن
            var credit = advanceReceiveMasterList.GroupBy(x => new { x.StudentID, x.PayrollID, x.RewardItems.COAID }).ToList().Select(x =>
              new GJD
              {
                  COA_ID = x.Key.COAID.Value,
                  Com_ID = UserInAcc.School_ID,
                  Fsy_ID = fsyid,
                  GJDCreditAmount = x.Sum(p => p.Value),
                  GJDDebitAmount = 0,
                  GJDStatus = true,
                  JOB_ID = null,
                  GJDDescrition = $"سداد حسم الصندوق للطالب {GetStudentNameById(x.Key.StudentID)} عن مسير {dbSch.Payroll.Find(x.Key.PayrollID).PayrollNumber.ToString()} بتاريخ {dbSch.Payroll.Find(x.Key.PayrollID).InsertDate.ToShortDateString()}"
              });
            allGJDs.AddRange(credit);
            //راس القيد
            GJH gjh = new GJH()
            {
                OPT_ID = BCDID,
                GJHStatus = true,
                USR_ID = UserInAcc.ID,
                GJHRefranceNumber = "",
                InsertDate = DateTime.Now,
                COM_ID = UserInAcc.School_ID,
                GJHSystemNumber = LastSystemNumber,
                GJHOperationDate = model.paramPostedDate,
                GJHDescription = $"اثبات استحقاق اقساط السلف على الطلاب بتاريخ {DateTime.Now.ToShortDateString()}",
                GJHAmount = allGJDs.Sum(c => c.GJDCreditAmount),
                JournalNo = dbAcc.NextJournalEntryNo_Sp(UserInAcc.School_ID, fsyid).Single().Value
            };
            dbAcc.GJH.Add(gjh);
            try
            {
                dbAcc.SaveChanges();
                allGJDs.ForEach(x => x.GJH_ID = gjh.GJHID);
                dbAcc.GJD.AddRange(allGJDs);
                dbAcc.SaveChanges();

                advanceReceiveMasterList.ForEach(x => { x.IsPosted = true; });
                dbSch.SaveChanges();

            }
            catch (Exception ex)
            {
                return Json(_notify = new notify() { Message = "خطأ اثناء ترحيل القيود ", Type = "error", status = 500 });
            }

            return Json(_notify = new notify() { Message = "تم ترحيل القيود بنجاح", Type = "success", status = 200 });
        }


        public string GetStudentNameById(decimal studentId)
        {
            var StudentName = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentId)?.STUDENT_NAME;
            return StudentName;
        }

    }

    public class AdvancesPremiumRelaysToAccountVN
    {
        public string ReceiveIds { get; set; }
        public DateTime paramPostedDate { get; set; }
    }
}