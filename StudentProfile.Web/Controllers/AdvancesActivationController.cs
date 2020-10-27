using StudentProfile.DAL;
using System;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.Entity.Core.EntityClient;

using StudentProfile.DAL.Models.VM;
using System.IO;
using System.IO.Compression;
using StudentProfile.DAL.Models;
using StudentProfile.Components;

namespace StudentProfile.Web.Controllers
{

    public partial class AdvancesController : Controller
    {        
        //private HRMadinaEntities dbSch = new HRMadinaEntities();
        //EsolERPEntities dbAcc = new EsolERPEntities();

        // GET: AdvancesActivation

        #region AdvancesRequests

        //public ActionResult AdvancesRequestsList()
        //{
        //    var user = Session["UserId"] as DashBoard_Users;
        //    ViewBag.ConfigError = null;

        //    var permissions = GetAdvancePermissionsfn(28);
        //    if (permissions.Read == false)
        //    {
        //        return RedirectToAction("NotAuthorized", "Security");
        //    }

        //    ViewBag.AcceptRejectPermission = permissions.AcceptReject;
        //    int userid = user.ID;
        //    if (dbSch.Dashboard_FilterGroupUsers.Where(x => x.userID == userid).Count() > 0)
        //    {
        //        var AdvancesConfig = dbSch.config.Where(x => x.Kay == "AdvancesConfig").FirstOrDefault();
        //        if (AdvancesConfig == null || AdvancesConfig.Value == null)
        //        {
        //            ViewBag.ConfigError = "من فضلك قم بتهيئة حساب السلف أولا";
        //        }

        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("NotAuthorized", "Security");
        //    }

        //}

        //public ActionResult _AdvancesRequestsList()
        //{
        //    var user = Session["UserId"] as DashBoard_Users;

        //    var permissions = GetAdvancePermissionsfn(28);
        //    if (permissions.Read == false)
        //    {
        //        return RedirectToAction("NotAuthorized", "Security");
        //    }

        //    ViewBag.AcceptRejectPermission = permissions.AcceptReject;
        //    int userid = user.ID;
        //    var userGroups = dbSch.Dashboard_FilterGroupUsers.Where(x => x.userID == userid);
        //    var orders = userGroups?.Select(x => x.Dashboard_FilterGroups.orders).ToList();
        //    ViewBag.PermissionFilters = orders;
        //    var modelList = new List<AdvanceRequest>();
        //    if (orders.Contains(1))
        //    {
        //        var firstLevel = dbSch.AdvanceRequest.Where(x =>
        //                dbSch.AdvanceRequestDetails.Any(p => p.AdvanceRequestId == x.ID) && x.FirstApprove == null &&
        //                x.ScondApprove == null && x.RefusedBy == null && x.ApprovedBy == null && x.PaidBy == null &&
        //                x.RequestType == "A")
        //            .ToList();
        //        modelList.AddRange(firstLevel);
        //    }

        //    if (orders.Contains(2))
        //    {
        //        var firstLevel = dbSch.AdvanceRequest.Where(x =>
        //                dbSch.AdvanceRequestDetails.Any(p => p.AdvanceRequestId == x.ID) && x.FirstApprove != null &&
        //                x.ScondApprove == null && x.RefusedBy == null && x.ApprovedBy == null && x.PaidBy == null &&
        //                x.RequestType == "A")
        //            .ToList();
        //        modelList.AddRange(firstLevel);
        //    }

        //    if (orders.Contains(3))
        //    {
        //        var firstLevel = dbSch.AdvanceRequest.Where(x =>
        //                dbSch.AdvanceRequestDetails.Any(p => p.AdvanceRequestId == x.ID) && x.FirstApprove != null &&
        //                x.ScondApprove != null && x.RefusedBy == null && (x.ApprovedBy == null || x.PaidBy == null) &&
        //                x.RequestType == "A")
        //            .ToList();
        //        modelList.AddRange(firstLevel);
        //    }

        //    if (orders.Contains(4))
        //    {
        //        var firstLevel = dbSch.AdvanceRequest.Where(x =>
        //                dbSch.AdvanceRequestDetails.Any(p => p.AdvanceRequestId == x.ID) && x.FirstApprove != null &&
        //                x.ScondApprove != null && x.RefusedBy == null && x.ApprovedBy != null && x.PaidBy == null &&
        //                x.RequestType == "A")
        //            .ToList();
        //        modelList.AddRange(firstLevel);
        //    }

        //    return PartialView(modelList);
        //}

        //public ActionResult _AdvanceRequestDetails(int id)
        //{
        //    var permissions = GetAdvancePermissionsfn(28);
        //    if (permissions.Read == false)
        //    {
        //        return RedirectToAction("NotAuthorized", "Security");
        //    }

        //    ViewBag.AcceptRejectPermission = permissions.AcceptReject;
        //    var modelList = dbSch.AdvanceRequestDetails.Where(x => x.AdvanceRequestId == id).ToList();
        //    return PartialView(modelList);
        //}

        //public int AcceptAdvance(int id)
        //{
        //    var user = Session["UserId"] as DashBoard_Users;
        //    var advanceRequest = dbSch.AdvanceRequest.Where(x => x.ID == id).FirstOrDefault();
        //    if (advanceRequest != null)
        //    {
        //        if (user != null && user.ID > 0)
        //        {
        //            try
        //            {
        //                int userid = user.ID;
        //                //Update HR Records
        //                advanceRequest.ApprovedBy = userid;
        //                dbSch.Entry(advanceRequest).State = System.Data.Entity.EntityState.Modified;
        //                dbSch.SaveChanges();
        //                return 1;
        //            }
        //            catch (Exception ex)
        //            {
        //                return 0;
        //            }
        //        }
        //    }

        //    return 0;
        //}


        //public int PayAdvance(int id)
        //{
        //    var user = Session["UserId"] as DashBoard_Users;
        //    var advanceRequest = dbSch.AdvanceRequest.Where(x => x.ID == id).FirstOrDefault();
        //    if (advanceRequest != null)
        //    {
        //        if (user != null && user.ID > 0)
        //        {
        //            var dafters = dbAcc.Dafters.Where(x => x.BranchId == 30 && x.DafterTypeId == 43).ToList()
        //                ?.Select(x => x.DafterId);
        //            var doc = new DaftersDocuments();
        //            if (dafters != null && dafters.Count() > 0)
        //            {
        //                doc = dbAcc.DaftersDocuments.Where(x => dafters.Contains(x.DafterId) && x.IsUsed == false)
        //                    .FirstOrDefault();
        //            }

        //            var coaRow = dbSch.AdvanceSettingConfig
        //                .Where(x => x.AdvanceSettingId == advanceRequest.AdvanceSettingId).FirstOrDefault();
        //            if (coaRow == null)
        //            {
        //                return -2;
        //            }

        //            if (doc != null)
        //            {
        //                try
        //                {
        //                    int userid = user.ID;
        //                    //Update HR Records
        //                    advanceRequest.ApprovedBy = userid;
        //                    //Add New Advance Row
        //                    var advance = new Advances();
        //                    advance.Name = advanceRequest.AdvanceName;
        //                    advance.EmpID = advanceRequest.EmpID;
        //                    advance.Date = advanceRequest.Date;
        //                    advance.DueDate = advanceRequest.DueDate;
        //                    advance.AdvanceValue = advanceRequest.AdvanceValue;
        //                    advance.MonthlyPremium = advanceRequest.MonthlyPremium;
        //                    advance.MonthsNumbers = advanceRequest.MonthsNumbers;
        //                    advance.IsValied = true;
        //                    dbSch.Advances.Add(advance);
        //                    dbSch.SaveChanges();
        //                    //Add New AdvanceRequestDetails Rows
        //                    var advanceRequestDetails =
        //                        dbSch.AdvanceRequestDetails.Where(x => x.AdvanceRequestId == id).ToList();
        //                    if (advanceRequestDetails != null && advanceRequestDetails.Count > 0)
        //                    {
        //                        advanceRequestDetails.ForEach(x => x.AdvanceId = advance.ID);
        //                    }

        //                    dbSch.SaveChanges();
        //                    //
        //                    //Save Exchange Cash Movement in Accounting DataBase
        //                    string username = dbSch.DashBoard_Users.Where(x => x.ID == userid).FirstOrDefault().Username;
        //                    int? comid = 30;
        //                    int fsyid = dbAcc.FSY.Where(x => x.COM_ID == comid && x.Is_Open == true).SingleOrDefault()
        //                        .FSYID;
        //                    string bcd_prefix = dbAcc.BCD.Where(x => x.BCDID == 43).SingleOrDefault().Prefix;
        //                    string fsy_prefix = dbAcc.FSY.Where(x => x.COM_ID == comid && x.Is_Open == true)
        //                        .SingleOrDefault().FsyPrefix;
        //                    string system_prefix = bcd_prefix + "-" + fsy_prefix;
        //                    //var employee = dbSch.Employees.FirstOrDefault(x => x.ID == advanceRequest.EmpID);
        //                    var student = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == advanceRequest.EmpID);

        //                    //Add New GJH Row
        //                    GJH GJHmodel = new GJH();
        //                    GJHmodel.COM_ID = comid;
        //                    GJHmodel.USR_ID = 23;
        //                    GJHmodel.GJHAmount = advance.AdvanceValue.Value;
        //                    GJHmodel.GJHDescription =
        //                        $"{advance.Name} - {username} للطالب رقم اكاديمي {student?.STUDENT_ID} رقم جوال {student?.MOBILE_NO}";
        //                    GJHmodel.GJHOperationDate = DateTime.Now;
        //                    GJHmodel.GJHStatus = true;
        //                    GJHmodel.OPT_ID = 43;
        //                    GJHmodel.GJHSystemNumber = null;
        //                    GJHmodel.GJHRefranceNumber = "";
        //                    GJHmodel.InsertDate = DateTime.Now;
        //                    GJHmodel.JournalNo = dbAcc.NextJournalEntryNo_Sp(comid, fsyid).Single().Value;
        //                    dbAcc.GJH.Add(GJHmodel);
        //                    dbAcc.SaveChanges();

        //                    //string studentName = dbSch.Employees.Where(x => x.ID == advanceRequest.EmpID)
        //                    //    .FirstOrDefault()?.Name;

        //                    //Add New GJD Rows
        //                    GJD GJDmodel = new GJD();
        //                    GJDmodel.GJH_ID = GJHmodel.GJHID;
        //                    GJDmodel.JOB_ID = null;
        //                    GJDmodel.GJDStatus = true;
        //                    GJDmodel.COA_ID = coaRow.COAID;
        //                    GJDmodel.Com_ID = comid;
        //                    GJDmodel.Fsy_ID = fsyid;
        //                    GJDmodel.GJDDescrition = student.STUDENT_NAME;
        //                    GJDmodel.GJDDebitAmount = advance.AdvanceValue.Value;
        //                    GJDmodel.GJDCreditAmount = 0;
        //                    dbAcc.GJD.Add(GJDmodel);
        //                    GJDmodel = new GJD();
        //                    GJDmodel.GJH_ID = GJHmodel.GJHID;
        //                    GJDmodel.JOB_ID = null;
        //                    GJDmodel.GJDStatus = true;
        //                    GJDmodel.COA_ID = 5435;
        //                    GJDmodel.Com_ID = comid;
        //                    GJDmodel.Fsy_ID = fsyid;
        //                    GJDmodel.GJDDescrition = "إذن صرف نقدي بتاريخ-" + DateTime.Now.ToString("dd/MM/yyyy");
        //                    GJDmodel.GJDDebitAmount = 0;
        //                    GJDmodel.GJDCreditAmount = advance.AdvanceValue.Value;
        //                    dbAcc.GJD.Add(GJDmodel);
        //                    //
        //                    //Add New CashMovement Row
        //                    CashMovement cashModel = new CashMovement();
        //                    cashModel.Amount = advance.AdvanceValue.Value;
        //                    cashModel.BCD_ID = 43;
        //                    cashModel.BeneficiaryName = student.STUDENT_NAME;
        //                    cashModel.GJHID = GJHmodel.GJHID;
        //                    cashModel.MovementDate = DateTime.Now;
        //                    cashModel.TreasuryId = dbAcc.Treasuries.Where(x => x.BranchId == comid).FirstOrDefault()
        //                        .TreasuryId;
        //                    cashModel.Notes =
        //                        $"إذن صرف نقدي بتاريخ- {DateTime.Now:dd/MM/yyyy} للطالب رقم اكاديمى {student?.STUDENT_ID} رقم جوال {student?.MOBILE_NO}";
        //                    cashModel.DocId = doc.DocId;
        //                    dbAcc.CashMovement.Add(cashModel);
        //                    dbAcc.DaftersDocuments.Where(x => x.DocId == cashModel.DocId).ToList()
        //                        .ForEach(x => x.IsUsed = true);
        //                    dbAcc.SaveChanges();
        //                    return 1;
        //                }
        //                catch (Exception ex)
        //                {
        //                    return 0;
        //                }
        //            }
        //        }
        //    }

        //    return 0;
        //}

        #endregion

        #region SubsidiesRequests

        public ActionResult SubsidiesRequestsList()
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;

            ViewBag.ConfigError = null;

            var permissions = GetSubsidiesRequestsListPermissionsfn(30);
            if (permissions.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            ViewBag.AcceptRejectPermission = permissions.AcceptReject;
            int userid = user.ID;
            if (dbSch.Dashboard_FilterGroupUsers.Where(x => x.userID == userid).Count() > 0)
            {
                var SubsidiesConfig = dbSch.config.Where(x => x.Kay == "SubsidiesConfig").FirstOrDefault();
                if (SubsidiesConfig == null || SubsidiesConfig.Value == null)
                {
                    ViewBag.ConfigError = "من فضلك قم بتهيئة حساب الإعانات أولا";
                }

                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

        }

        public ActionResult _SubsidiesRequestsList()
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;

            var permissions = GetSubsidiesRequestsListPermissionsfn(30);
            if (permissions.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            ViewBag.AcceptRejectPermission = permissions.AcceptReject;
            int userid = user.ID;
            var userGroups = dbSch.Dashboard_FilterGroupUsers.Where(x => x.userID == userid);
            var orders = userGroups?.Select(x => x.Dashboard_FilterGroups.orders).ToList();
            ViewBag.PermissionFilters = orders;
            var modelList = new List<AdvanceRequest>();
            if (orders.Contains(1))
            {
                var firstLevel = dbSch.AdvanceRequest.Where(x =>
                    x.FirstApprove == null && x.ScondApprove == null && x.RefusedBy == null && x.ApprovedBy == null &&
                    x.PaidBy == null &&
                    x.RequestType == "S").ToList();
                modelList.AddRange(firstLevel);
            }

            if (orders.Contains(2))
            {
                var firstLevel = dbSch.AdvanceRequest.Where(x =>
                    x.FirstApprove != null && x.ScondApprove == null && x.RefusedBy == null && x.ApprovedBy == null &&
                    x.PaidBy == null &&
                    x.RequestType == "S").ToList();
                modelList.AddRange(firstLevel);
            }

            if (orders.Contains(3))
            {
                var firstLevel = dbSch.AdvanceRequest.Where(x =>
                    x.FirstApprove != null && x.ScondApprove != null && x.RefusedBy == null &&
                    (x.ApprovedBy == null || x.PaidBy == null) &&
                    x.RequestType == "S").ToList();
                modelList.AddRange(firstLevel);
            }

            if (orders.Contains(4))
            {
                var firstLevel = dbSch.AdvanceRequest.Where(x =>
                    x.FirstApprove != null && x.ScondApprove != null && x.RefusedBy == null && x.ApprovedBy != null &&
                    x.PaidBy == null &&
                    x.RequestType == "S").ToList();
                modelList.AddRange(firstLevel);
            }

            return PartialView(modelList);
        }

        public int AcceptSubsidy(int id)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            var subsidyRequest = dbSch.AdvanceRequest.Where(x => x.ID == id).FirstOrDefault();
            if (subsidyRequest != null)
            {
                if (user != null && user.ID > 0)
                {
                    try
                    {
                        //update AdvanceRequest Table in HR DataBase
                        int userid = user.ID;
                        subsidyRequest.ApprovedBy = userid;
                        dbSch.Entry(subsidyRequest).State = System.Data.Entity.EntityState.Modified;
                        dbSch.SaveChanges();

                        return 1;
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
            }


            return 0;
        }

        //public int PaySubsidy(int id)
        //{
        //    var user = HttpContext.Session["UserId"] as DashBoard_Users;
        //    var subsidyRequest = dbSch.AdvanceRequest.Where(x => x.ID == id).FirstOrDefault();
        //    if (subsidyRequest != null)
        //    {
        //        if (user != null && user.ID > 0)
        //        {
        //            var dafters = dbAcc.Dafters.Where(x => x.BranchId == 30 && x.DafterTypeId == 43).ToList()
        //                ?.Select(x => x.DafterId);
        //            var doc = new DaftersDocuments();
        //            if (dafters != null && dafters.Count() > 0)
        //            {
        //                doc = dbAcc.DaftersDocuments.Where(x => dafters.Contains(x.DafterId) && x.IsUsed == false)
        //                    .FirstOrDefault();
        //            }

        //            var coaRow = dbSch.AdvanceSettingConfig
        //                .Where(x => x.AdvanceSettingId == subsidyRequest.AdvanceSettingId).FirstOrDefault();
        //            if (coaRow == null)
        //            {
        //                return -2;
        //            }

        //            if (doc != null)
        //            {
        //                try
        //                {
        //                    //update AdvanceRequest Table in HR DataBase
        //                    int userid = user.ID;
        //                    subsidyRequest.PaidBy = userid;
        //                    dbSch.SaveChanges();
        //                    //Save Exchange Cash Movement in Accounting DataBase
        //                    string username = dbSch.DashBoard_Users.Where(x => x.ID == userid).FirstOrDefault().Username;
        //                    int? comid = 30;
        //                    int fsyid = dbAcc.FSY.Where(x => x.COM_ID == comid && x.Is_Open == true).SingleOrDefault()
        //                        .FSYID;
        //                    string bcd_prefix = dbAcc.BCD.Where(x => x.BCDID == 43).SingleOrDefault().Prefix;
        //                    string fsy_prefix = dbAcc.FSY.Where(x => x.COM_ID == comid && x.Is_Open == true)
        //                        .SingleOrDefault().FsyPrefix;
        //                    string system_prefix = bcd_prefix + "-" + fsy_prefix;
        //                    //var employee = dbSch.Employees.FirstOrDefault(x => x.ID == subsidyRequest.EmpID);
        //                    var student = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == subsidyRequest.EmpID);

        //                    //Add New GJH Row
        //                    GJH GJHmodel = new GJH();
        //                    GJHmodel.COM_ID = comid;
        //                    GJHmodel.USR_ID = 23;
        //                    GJHmodel.GJHAmount = subsidyRequest.AdvanceValue.Value;
        //                    GJHmodel.GJHDescription =
        //                        $"{subsidyRequest.AdvanceName} - {username} للطالب رقم اكاديمي {student?.STUDENT_ID} رقم جوال {student?.MOBILE_NO} ";
        //                    GJHmodel.GJHOperationDate = DateTime.Now;
        //                    GJHmodel.GJHStatus = true;
        //                    GJHmodel.OPT_ID = 43;
        //                    GJHmodel.GJHSystemNumber = null;
        //                    GJHmodel.GJHRefranceNumber = "";
        //                    GJHmodel.InsertDate = DateTime.Now;
        //                    GJHmodel.JournalNo = dbAcc.NextJournalEntryNo_Sp(comid, fsyid).Single().Value;
        //                    dbAcc.GJH.Add(GJHmodel);
        //                    dbAcc.SaveChanges();

        //                    //string studentName = dbSch.Employees.Where(x => x.ID == subsidyRequest.EmpID)
        //                    //    .FirstOrDefault()?.Name;

        //                    //Add New GJD Rows
        //                    GJD GJDmodel = new GJD();
        //                    GJDmodel.GJH_ID = GJHmodel.GJHID;
        //                    GJDmodel.JOB_ID = null;
        //                    GJDmodel.GJDStatus = true;
        //                    GJDmodel.COA_ID = coaRow.COAID;
        //                    GJDmodel.Com_ID = comid;
        //                    GJDmodel.Fsy_ID = fsyid;
        //                    GJDmodel.GJDDescrition = student.STUDENT_NAME;
        //                    GJDmodel.GJDDebitAmount = subsidyRequest.AdvanceValue.Value;
        //                    GJDmodel.GJDCreditAmount = 0;
        //                    dbAcc.GJD.Add(GJDmodel);
        //                    GJDmodel = new GJD();
        //                    GJDmodel.GJH_ID = GJHmodel.GJHID;
        //                    GJDmodel.JOB_ID = null;
        //                    GJDmodel.GJDStatus = true;
        //                    GJDmodel.COA_ID = 5435;
        //                    GJDmodel.Com_ID = comid;
        //                    GJDmodel.Fsy_ID = fsyid;
        //                    GJDmodel.GJDDescrition = "إذن صرف نقدي بتاريخ-" + DateTime.Now.ToString("dd/MM/yyyy");
        //                    GJDmodel.GJDDebitAmount = 0;
        //                    GJDmodel.GJDCreditAmount = subsidyRequest.AdvanceValue.Value;
        //                    dbAcc.GJD.Add(GJDmodel);
        //                    //Add New CashMovement Row
        //                    CashMovement cashModel = new CashMovement();
        //                    cashModel.Amount = subsidyRequest.AdvanceValue.Value;
        //                    cashModel.BCD_ID = 43;
        //                    cashModel.BeneficiaryName = student.STUDENT_NAME;
        //                    cashModel.GJHID = GJHmodel.GJHID;
        //                    cashModel.MovementDate = DateTime.Now;
        //                    cashModel.TreasuryId = dbAcc.Treasuries.Where(x => x.BranchId == comid).FirstOrDefault()
        //                        .TreasuryId;
        //                    cashModel.Notes =
        //                        $"إذن صرف نقدي بتاريخ- {DateTime.Now:dd/MM/yyyy} للطالب رقم اكاديمى {student?.STUDENT_ID} رقم جوال {student?.MOBILE_NO}";
        //                    cashModel.DocId = doc.DocId;
        //                    dbAcc.CashMovement.Add(cashModel);
        //                    dbAcc.DaftersDocuments.Where(x => x.DocId == cashModel.DocId).ToList()
        //                        .ForEach(x => x.IsUsed = true);
        //                    dbAcc.SaveChanges();
        //                    return 1;
        //                }
        //                catch (Exception ex)
        //                {
        //                    return 0;
        //                }
        //            }
        //        }
        //    }

        //    return 0;
        //} 
        public ActionResult DownloadAttachments(int id, int studentid)
        {
            var files = dbSch.AdvanceRequests.Where(x => x.ID == id && x.Student_Id ==studentid).SingleOrDefault()?.AdvanceRequestsAttachment.FirstOrDefault();
            if (files != null)
            {
                string filenames = files.FileName;
                if (filenames != null)
                {

                    var filesCol = GetFiles(filenames, studentid.ToString()).ToList();

                    var checkisExist = Server.MapPath("/Content/" + id + ".zip");
                    if (!Directory.Exists(checkisExist))
                    {
                        ZipArchive zipPth = ZipFile.Open(Server.MapPath("/Content/" + id + ".zip"), ZipArchiveMode.Create);
                        foreach (var file in filesCol)
                        {
                            zipPth.CreateEntryFromFile(Server.MapPath
                                ("~/Content/UserFiles/" + studentid + "/الاعانات/" + file.FileName), file.FileName);
                        }

                        zipPth.Dispose();
                    }
                  
                    return Content("/Content/" + id + ".zip");
                }
                else
                {
                    return Content("");
                }
            }
            else
            {
                return Content("");
            }

        }

        #endregion

        #region GlobalActions

        public int FirstAccept(int id)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            var advanceRequest = dbSch.AdvanceRequest.Where(x => x.ID == id).FirstOrDefault();
            if (advanceRequest != null)
            {
                if (user != null && user.ID > 0)
                {
                    advanceRequest.FirstApprove = user.ID;
                    try
                    {
                        dbSch.SaveChanges();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return -1;
            }

            return 0;
        }

        public int SecondAccept(int id)
        {

            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            var advanceRequest = dbSch.AdvanceRequest.Where(x => x.ID == id).FirstOrDefault();
            if (advanceRequest != null)
            {
                if (user != null && user.ID > 0)
                {
                    advanceRequest.ScondApprove = user.ID;
                    try
                    {
                        dbSch.SaveChanges();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return -1;
            }

            return 0;
        }

        public int Refuse(int id)
        {
            var advanceRequest = dbSch.AdvanceRequest.Where(x => x.ID == id).FirstOrDefault();

            var user = HttpContext.Session["UserId"] as DashBoard_Users;


            if (advanceRequest != null)
            {
                if (user != null && user.ID > 0)
                {
                    advanceRequest.RefusedBy = user.ID;
                    try
                    {
                        dbSch.SaveChanges();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return -1;
            }

            return 0;
        }

        [HttpGet]
        public JsonResult GetAdvancesRequestsListPermissions(int screenId)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;

            if (user != null)
            {
                // userId = int.Parse(HttpContext.Session["UserId"].ToString());
                if (user.ID == 0)
                {
                    //return RedirectToAction("Login", "Login");
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }

            var perm = CheckPermissions.IsAuthorized(user.ID, screenId);
            var permissions = new AdvancesRequestsListPermissions();
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
                else if (permission == "موافقة / رفض")
                {
                    permissions.AcceptReject = true;
                }
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubsidiesRequestsListPermissions(int screenId)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            var userId = 0;
            if (user != null)
            {
                // userId = int.Parse(HttpContext.Session["UserId"].ToString());
                if (user.ID == 0)
                {
                    //return RedirectToAction("Login", "Login");
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }

            var perm = CheckPermissions.IsAuthorized(user.ID, screenId);
            var permissions = new AdvancesRequestsListPermissions();
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
                else if (permission == "موافقة / رفض")
                {
                    permissions.AcceptReject = true;
                }
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        public AdvancesRequestsListPermissions GetSubsidiesRequestsListPermissionsfn(int screenId)
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            var userId = 0;
            if (user != null)
            {
                // userId = int.Parse(HttpContext.Session["UserId"].ToString());
                if (user.ID == 0)
                {
                    //return RedirectToAction("Login", "Login");
                    return null;
                }
            }

            var perm = CheckPermissions.IsAuthorized(user.ID, screenId);
            var permissions = new AdvancesRequestsListPermissions();
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
                else if (permission == "موافقة / رفض")
                {
                    permissions.AcceptReject = true;
                }
            }

            return permissions;
        }

        public List<FileInfos> GetFiles(string filenames, string studentid)
        {
            List<FileInfos> listFiles = new List<FileInfos>();
            var files = filenames.Split(';');
            var fileSavePath = Server.MapPath("~/Content/UserFiles/" + studentid + "/الاعانات/");
            if (System.IO.Directory.Exists(fileSavePath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);
                if (files != null)
                {
                    int i = 0;
                    foreach (var name in files)
                    {
                        if (System.IO.File.Exists(fileSavePath + "/" + name))
                        {
                            listFiles.Add(new FileInfos
                            {
                                FileId = i + 1,
                                FileName = name,
                                FilePath = dirInfo.FullName + @"\" + name
                            });
                        }

                        i = i + 1;
                    }
                }
            }

            return listFiles;
        }

        #endregion
    }


    public class AdvancesRequestsListPermissions
    {
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Save { get; set; }
        public bool Exception { get; set; }
        public bool AcceptReject { get; set; }
    }

    public class SubsidiesRequestsListPermissions
    {
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Save { get; set; }
        public bool Exception { get; set; }
        public bool AcceptReject { get; set; }
    }
}