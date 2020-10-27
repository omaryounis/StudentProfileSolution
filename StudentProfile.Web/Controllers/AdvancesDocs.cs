using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using StudentProfile.DAL.Models.VM;
using StudentProfile.Web.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace StudentProfile.Web.Controllers
{
    public partial class AdvancesController : Controller
    {

        #region شاشة طباعة سندات الصرف والقبض النقدي للسلف  

        public ActionResult AdvanceCashMovements()
        {
            var json = new JavaScriptSerializer().Serialize(GetPermissionsJson(87).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        public ViewResult PrintDocAdvance(string DocMasterIds)
        {
            PaidDocRPT report = new PaidDocRPT();
            var data = PaidDocsPrintDataSource(DocMasterIds);
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = data;
            };
            return View(report);
        }

        public ViewResult ReceiveDocAdvance(string DocMasterIds)
        {
            ReceivedDocRPT report = new ReceivedDocRPT();
            var data = ReceivedDocsPrintDataSource(DocMasterIds);
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = data;
            };
            return View(report);
        }


        [HttpGet]
        public JsonResult AdvanceCashMovementsData()
        {
            return this.JsonMaxLength(dbSch.Usp_CashAdvances_DataSource().ToList().Select(x => new
            {
                x.AdvanceRequestID,
                x.AdvanceSettingName,
                x.DocHeader,
                x.DocName,
                x.DocNotes,
                x.DocNumber,
                x.DocType,
                x.ID,
                x.StudentName,
                x.TotalValue,
                InsertionDate = x.InsertionDate.ToShortDateString()
            }).ToList());
        }

        public List<PaidDocsPrintVM> PaidDocsPrintDataSource(string DocMasterIds)
        {
            if (string.IsNullOrEmpty(DocMasterIds))
                return null;

            //=========================================
            // دي السندات المصروفة اللي هيتم طباعتها 
            //==========================================
            else
            {
                var docMasterIds = DocMasterIds.Split(',').Select(x => Convert.ToInt32(x)).ToList();

                return dbSch.AdvancePaymentDetails
                            .Where(x => docMasterIds.Any(p => p == x.AdvancePaymentMaster.ID))
                            .Select(x => new PaidDocsPrintVM
                            {
                                AdvanceName = x.AdvanceRequests.AdvanceSettings.AdvanceSettingName,
                                InsertionDate = x.AdvancePaymentMaster.InsertionDate,
                                DocNumber = DateTime.Now.Year + " - " + x.AdvancePaymentMaster.DocNumber+"-"+x.AdvanceRequests_Id,
                                DocHeader = x.AdvancePaymentMaster.DocHeader,
                                DocNotes = x.AdvancePaymentMaster.DocNotes,
                                Student_Id = x.AdvancePaymentMaster.Student_Id,
                                DocTotalValue = x.AdvancePaymentMaster.TotalValue,
                                PaidValue = x.NetValue,
                                DocType = "سند صرف نقدي",
                                StudentName = dbSch.INTEGRATION_All_Students
                                                   .FirstOrDefault(s => s.STUDENT_ID == x.AdvancePaymentMaster.Student_Id).STUDENT_NAME
                            }).ToList();
            }

        }

        public List<ReceivedDocsPrintVM> ReceivedDocsPrintDataSource(string DocMasterIds)
        {
            //=========================================
            // دي السندات المسددة اللي هيتم طباعتها 
            //==========================================
            var docMasterIds = DocMasterIds.Split(',').Select(x => Convert.ToInt32(x)).ToList();

            var AdvanceReceiveMasterList = dbSch.AdvanceReceiveMaster
                                                .Where(x => docMasterIds.Any(p => p == x.ID));

            var ReceivedDocsPrintVMList = new List<ReceivedDocsPrintVM>();
            AdvanceReceiveMasterList.ToList().ForEach(advanceReceiveMaster =>
            {
                ReceivedDocsPrintVMList.AddRange(
                    advanceReceiveMaster.AdvanceReceiveDetails.Select(x => new ReceivedDocsPrintVM
                    {
                        DocType = "سند قبض نقدي",
                        AdvanceName = x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName,
                        InsertionDate = x.AdvanceReceiveMaster.InsertionDate,
                        DocNumber = DateTime.Now.Year + " - " + x.AdvanceReceiveMaster.DocNumber +"-"+ x.AdvancePaymentDetails.AdvanceRequests_Id,
                        DocHeader = x.AdvanceReceiveMaster.DocHeader,
                        DocNotes = x.AdvanceReceiveMaster.DocNotes,
                        PaidValue = x.AdvancePaymentDetails.NetValue,
                        ReceiveValue = x.NetValue,
                        DocTotalValue = x.AdvanceReceiveMaster.TotalValue,
                        Student_Id = x.AdvanceReceiveMaster.Student_Id,
                        StudentName = dbSch.INTEGRATION_All_Students
                                           .FirstOrDefault(s => s.STUDENT_ID == x.AdvanceReceiveMaster.Student_Id).STUDENT_NAME,

                        TotalRemainBeforeDocDate = x.AdvancePaymentDetails.NetValue -
                                                   dbSch.AdvanceReceiveDetails
                                                        .Where(s => s.AdvancePaymentDetails_Id == x.AdvancePaymentDetails_Id &&
                                                                    s.AdvanceReceiveMaster.InsertionDate < x.AdvanceReceiveMaster.InsertionDate
                                                              ).Select(s => s.NetValue).DefaultIfEmpty(0).Sum()
                    }).ToList()
                );

            });
            return ReceivedDocsPrintVMList;
        }

        #endregion

    }

    public class AdvanceReceivedFilter
    {
        public List<string> DocNumbers { get; set; }
        public List<AdvanceSettings> AdvanceNames { get; set; }
    }
    public class PaidDocsPrintVM
    {
        public string AdvanceName { get; set; }
        public DateTime InsertionDate { get; set; }
        public string DocNumber { get; set; }
        public string DocHeader { get; set; }
        public string DocNotes { get; set; }
        public int Student_Id { get; set; }
        public decimal DocTotalValue { get; set; }
        public decimal PaidValue { get; set; }
        public string DocType { get; set; }
        public string StudentName { get; set; }
    }

    public class ReceivedDocsPrintVM
    {
        public string AdvanceName { get; set; }
        public DateTime InsertionDate { get; set; }
        public string DocNumber { get; set; }
        public string DocHeader { get; set; }
        public string DocNotes { get; set; }
        public int Student_Id { get; set; }
        public decimal DocTotalValue { get; set; }
        public decimal PaidValue { get; set; }
        public decimal ReceiveValue { get; set; }
        public string DocType { get; set; }
        public string StudentName { get; set; }
        public decimal TotalRemainBeforeDocDate { get; set; }
    }
}


//public ActionResult _PrintDoc(string DocMasterIds)
//{
//    PaidDocRPT report = new PaidDocRPT();
//    var data = PaidDocsPrintDataSource(DocMasterIds);
//    report.DataSourceDemanded += (s, e) =>
//    {
//        //((XtraReport)s)
//        ((XtraReport)s).DataSource = data;
//    };
//    ViewData["Report"] = report;

//    return PartialView(report);
//}