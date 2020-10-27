using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using StudentProfile.DAL.Models.VM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
namespace StudentProfile.Web.Controllers
{
    public class ExchangeOrderObject
    {

        public ExchangeOrder ExchangeOrder { get; set; }


         public  List<int?> payllosId { get; set; }


        public int? ExchangeOrderID { get; set; }
    }
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public partial class PayRollStudentsController : Controller
    {

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
        public ActionResult CancelChecks()
        {
            var permissions = GetPermissionsFn(113);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }


        public ActionResult PayrollTracking()
        {
            var permissions = GetPermissionsFn(113);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;

            if (permissions.Read || permissions.View)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }
        public string GetFilePath(HttpPostedFileBase file, string folderName)
        {

            // file
            var rootFolder = Server.MapPath("~/Content/PayRollStudentsFiles/");
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


            var subFolder = Path.Combine(rootFolder, folderName);

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

            string filename =  String.Format("{0}_{1}.{2}", file.FileName.Split('.')[0],
                DateTime.Now.Ticks, file.FileName.Split('.')[1]);

            string path = Path.Combine(subFolder, filename);


            Stream stream = file.InputStream;
            byte[] bytes = ReadToEnd(stream);
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();

            return MapPathReverse(path);
        }
        public string GetFilePath(HttpPostedFileBase file, string folderName,int StudentID,int PayrollID)
        {

            // file
            var rootFolder = Server.MapPath("~/Content/PayRollStudentsFiles/");
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


            var subFolder = Path.Combine(rootFolder, folderName);

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
                StudentID+"_"+ PayrollID, file.FileName.Split('.')[1]);

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
        #region privateMembers
      
        private List<PayrollPhasesUsers> PayrollUserPhases
        {
            get
            {
                if (Session["UserId"] != null)
                {
                    var CurrentUser = Session["UserId"] as DashBoard_Users;
                    var PayrollPhasesUsers= dbSC.PayrollPhasesUsers.Where(x => x.UserID == CurrentUser.ID && x.IsActive == true && x.PayrollPhases.IsActive == true).ToList(); 
                    return PayrollPhasesUsers;
                };
                return null;
            }
        }
        private List<int> PayrollUserPhasesOrders
        {
             get {
                    var PayrollUserPhasesRecs = PayrollUserPhases.Select(x => x.PayrollPhases.PhaseOrder).ToList();
                    return PayrollUserPhasesRecs;
                }
        }
        private List<AdvanceStudentReturn> AdvancePartReturningByPayroll(int payrollID, int? studentID)
        {
            var AvailableAdvancesToselect = new List<AdvanceStudentReturn>();
            var details = dbSC.Payroll.Where(x => x.ID == payrollID).SingleOrDefault()
               .StudentPayroll.Where(x => (studentID == null || x.StudentID == studentID)).ToList();
            if (details.Any(x => x.RewardItems.IsAdvanceReturn == true)) { 
            details.GroupBy(x => x.StudentID).ToList().ForEach(x =>
            {
                if (x.Any(p => p.RewardItems.IsAdvanceReturn == true))
                {
                    List<AdvanceStudentReturn> advancesToPay = new List<AdvanceStudentReturn>();
                    decimal RewardValue = x.Where(mm => mm.RewardItems.IsAdvanceReturn != true)
                    .Sum(c => c.Value * c.RewardItems.RewardItemTypes.RewardItemType);
                    var advances = dbSC.AdvancePaymentDetails.Where(p => p.AdvanceRequests.AdvanceSettings.AdvanceType == "A"
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
                                AdvanceReceiveMaster_Id =  0,
                                Value = desiredValue
                            });
                        }
                    });
                    AvailableAdvancesToselect.AddRange(advancesToPay);
                }
            });
            }
            return AvailableAdvancesToselect;
        }
        #endregion
        #region PrivateMethods
        private int currentPhaseUser(int order)
        {
            return PayrollUserPhases.Where(x => x.PayrollPhases.PhaseOrder == order).SingleOrDefault().ID;
        }
        private string currentPhase(int order)
        {
            return dbSC.PayrollPhases.Where(x => x.PhaseOrder == order).SingleOrDefault().PhaseName;
        }
        private PayrollPhasesUsers currentPhaseUser(int order,int UserId)
        {
            var rec= dbSC.PayrollPhasesUsers.Include("PayrollPhases").Where(x => x.PayrollPhases.PhaseOrder == order && x.PayrollPhases.IsActive==true && x.UserID ==UserId ).SingleOrDefault();
            return rec;
        }
        public bool IsAvailable(int payrollID)
        {

            var CurrentUser = Session["UserId"] as DashBoard_Users;
            int currentPhaseorder = 0;

            currentPhaseorder = dbSC.PayrollApproval.Where(x => x.PayrollID == payrollID && x.IsApproved == true).Count() + 2;
            //if (dbSC.PayrollApproval.Where(x => x.PayrollID == payrollID && x.IsApproved==true).Count() == 0)
            //{
            //    currentPhaseorder=dbSC.PayrollApproval.Where(x => x.PayrollID == payrollID && x.IsApproved == true).Count() +2;
            //}
            //else
            //{
            //    currentPhaseorder = dbSC.PayrollApproval.Where(x => x.PayrollID == payrollID && x.IsApproved == true).Count() + 1;
            //}

            PayrollPhasesUsers currentPhaseUserRec = currentPhaseUser(currentPhaseorder, CurrentUser.ID);
            if (currentPhaseUserRec != null)
            {

                if (currentPhaseUserRec.IsActive == true)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
      
        #endregion
        #region Views
        public ActionResult PayrollApproval()
        {
            var permissions = GetPermissionsFn(105);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read && PayrollUserPhases.Count > 0)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }
        public ActionResult PayrollApprovalWithMoney()
        {
            var permissions = GetPermissionsFn(106);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;
            if (permissions.View && PayrollUserPhases.Count > 0)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }
        public ActionResult ExchangeOrder()
        {
            var permissions = GetPermissionsFn(77);
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
        public ActionResult Test2()
        {
            return View();
        }
        public ActionResult Test3()
        {
            return View();
        }
        #endregion
        #region ViewActions
        public JsonResult GetPayRolls()
        {
            List<PayrollModel> payrolls = new List<PayrollModel>();

            int currentPhase = 0;
            var list = dbSC.Payroll.Where(x => x.IsActive == true && x.IsPosted == true && x.DafPayNo == null && x.PayrollApproval.All(p => p.IsRefused == false ) ).ToList();

            foreach(var item in list)
            {
                if (IsAvailable(item.ID))
                {
                    PayrollModel payroll = new PayrollModel();
                    currentPhase = dbSC.PayrollApproval.Where(x => x.PayrollID == item.ID).Count() + 2;
                    payroll.IsFinancialApprove = dbSC.PayrollPhases.Where(x => x.PhaseOrder == currentPhase).Select(m => m.IsFinancialApprove).FirstOrDefault();
                    payroll.IssuingExchangeOrder = dbSC.PayrollPhases.Where(x => x.PhaseOrder == currentPhase).Select(m => m.IssuingExchangeOrder).FirstOrDefault();
                    payroll.IssuingPaymentOrder = dbSC.PayrollPhases.Where(x => x.PhaseOrder == currentPhase).Select(m => m.IssuingPaymentOrder).FirstOrDefault();
                    payroll.ID = item.ID;
                    payroll.InsertDate = item.InsertDate.ToShortDateString();
                    payroll.IssueDate = item.IssueDate.ToShortDateString();
                    payroll.PayrollNumber = "(" + item.PayrollNumber + ")-" + item.IssueDate.ToString("dd-MM-yyyy");
                    payroll.TotalAmount = item.TotalAmount;
                    payroll.DafPayNo = item.DafPayNo;
                    payroll.DafPayNo2 = item.DafPayNo2;
                    payroll.PayNo = item.PayNo;
                    payroll.MinistrVal = item.IsMinister == true ? "مسير وزارة" : "مسير طلاب";
                    payroll.AdvancesValues =item.IsMinister==true?0: dbSC.StudentPayroll.Where(m => m.PayrollID == item.ID).Count() > 0 ?dbSC.StudentPayroll.Where(m => m.PayrollID == item.ID && m.RewardItems.IsAdvanceReturn == true)!=null? dbSC.StudentPayroll.Where(m => m.PayrollID == item.ID && m.RewardItems.IsAdvanceReturn == true).Sum(o => o.Value):0:0; //AdvancePartReturningByPayroll(item.ID, null).Sum(p => p.Value);
                    payroll.RealValues = payroll.TotalAmount - payroll.AdvancesValues;  //item.TotalAmount - AdvancePartReturningByPayroll(item.ID, null).Sum(p => p.Value);
                    payroll.StudentsCount = item.StudentPayroll.Select(p => p.StudentID).Distinct().Count();
                    payrolls.Add(payroll);
                }
            }
            return Json(payrolls
            , JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivePayRolls()
        {
           var list = dbSC.Payroll.Where(x => x.IsActive == true && x.IsPosted == true).ToList().Select(x => new SelectListItem { Text = "("+x.PayrollNumber+")-"+x.IssueDate.ToString("dd-MM-yyyy"), Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList();
             return Json(list
            , JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPayRollStatge(int payrollID)
        {
            var CurrentPhaseOrder= dbSC.PayrollApproval.Where(x => x.PayrollID == payrollID).Count() + 1;
            var Phases = dbSC.PayrollPhases.Where(m => m.IsActive == true /*&& m.PhaseOrder !=1*/).OrderBy(m=>m.PhaseOrder).ToList();
            List<Usp_GetPayRollStatge_Result> usp_GetPayRollStatge_ResultList = new List<Usp_GetPayRollStatge_Result>();
            foreach (var item in Phases)
            {
                Usp_GetPayRollStatge_Result obj = new Usp_GetPayRollStatge_Result();
                if (item.PhaseOrder <= CurrentPhaseOrder)
                {
                    obj.PhaseName = item.PhaseName;
                    if (item.PhaseOrder == 1)
                    {
                        var user = Session["UserId"] as DashBoard_Users;
                        obj.UserName = user.Username;
                        obj.InsertDate = dbSC.Payroll.Find(payrollID).IssueDate.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        obj.UserName = dbSC.PayrollApproval.Where(m => m.PayrollID == payrollID && m.IsApproved == true && m.PayrollPhasesUsers.PayrollPhases.PhaseOrder == item.PhaseOrder).Select(p => p.PayrollPhasesUsers.DashBoard_Users.Name).FirstOrDefault();
                        obj.InsertDate = dbSC.PayrollApproval.Where(m => m.PayrollID == payrollID && m.IsApproved == true && m.PayrollPhasesUsers.PayrollPhases.PhaseOrder == item.PhaseOrder).ToList().Select(p => p.InsertDate.Value.ToString("dd/MM/yyyy")).FirstOrDefault();

                    }
                }
                else {
                    obj.PhaseName = item.PhaseName;
                }
                usp_GetPayRollStatge_ResultList.Add(obj);
            }
            return this.JsonMaxLength(usp_GetPayRollStatge_ResultList);
        }
        public JsonResult GetPayRollsMoney()
        {
            return this.JsonMaxLength(
               dbSC.Usp_GetPayRollsMoney().ToList()
                .Select(x => new Usp_GetPayRollsMoney_Result
                {
                    PayrollID =x.PayrollID,
                    PayrollNumber=x.PayrollNumber,
                    PayNumber=x.PayNumber,
                    DafPayNumber = x.DafPayNumber,
                    PayrollMoneyValue=x.PayrollMoneyValue,
                    StudentNumber=x.StudentNumber,
                    LastStageDate=x.LastStageDate
                }).ToList());
        }
        public ActionResult PayrollApprovalWithMoneyForIssuingChecks()
        {
            var permissions = GetPermissionsFn(107);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            ViewBag.View = permissions.View;
            if (permissions.View && PayrollUserPhases.Count > 0)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }

        public ActionResult CheckFiles()
        {
            var permissions = GetPermissionsFn(108);
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
            return RedirectToAction("NotAuthorized", "Security");
        }
        public ActionResult ReceiveChecks()
        {

            var permissions = GetPermissionsFn(109);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;
            if (PayrollUserPhases.Count > 0 && permissions.View)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }
        public ActionResult DelevirPayroll()
        {
            var permissions = GetPermissionsFn(110);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;

            if (PayrollUserPhases.Count > 0 && (permissions.View|| permissions.Read))
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }
        public ActionResult GetPayrollWithNotReceiveChecksYet()
        {
            var users = dbSC.Usp_GetPayRollsWithNotReceivedChecks().ToList().Select(x => new SelectListItem { Text = x.PayrollNumber, Value = x.PayrollID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPayRollsWithNotReceivedAfterExportedChecks()
        {
            var users = dbSC.Usp_GetPayRollsWithNotReceivedAfterExportedChecks().ToList().Select(x => new SelectListItem { Text = x.PayrollNumber, Value = x.PayrollID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetYearsData()
        {
            //List<int> Years = new List<int>();
            //for (int i = DateTime.Now.Year; i >= 2000; i--)
            //{
            //    Years.Add(i);
            //}
            //var years = Years.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            var Years = dbSC.Payroll.Where(m => m.IsMonetary == true && m.PayNo != null && m.DafPayNo != null && m.IsExportedCheck == true && m.PayrollChecks.Any(p => p.IsReceived == true)).Select(m => m.IssueDate.Year).ToList();
            var years = Years.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            return Json(years, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMonthsData(int year)
        {
            var Months = dbSC.Payroll.Where(x => x.IssueDate.Year == year && x.IsMonetary==true && x.PayNo!=null && x.DafPayNo!=null && x.IsExportedCheck ==true && x.PayrollChecks.Any(p=>p.IsReceived==true)).ToList();
            var months = Months.Select(x => new SelectListItem { Text = x.IssueDate.Month.ToString(), Value = x.IssueDate.Month.ToString() }).OrderBy(x => x.Text).Distinct().ToList();
            return Json(months, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPayrollNumberData(int year, int month)
        {
            var PayRollNumbers = GetDelverPayroll().Where(x => x.DafPayNo != null   && x.IssueDatePayroll.Year == year && x.IssueDatePayroll.Month == month).Select(x => new SelectListItem { Text = x.Payrollltem, Value = x.ID.ToString() }).ToList();

            return Json(PayRollNumbers, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPayrollUsers(int payrollID)
        {
            var users = dbSC.Usp_GetPayRollsUsersChecks(payrollID).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.UserID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPayrollMutiUsers(string payrollID)
        {
            var users = dbSC.Usp_GetPayRollsMultiUsersChecks(payrollID).ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.UserID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetChecksData(int userID,int payrollID)
        {
            return this.JsonMaxLength(
                dbSC.Usp_GetPayRollsExportedChecks_Details(payrollID, userID).ToList()
                 .Select(x => new Usp_GetPayRollsExportedChecks_Details_Result
                 {
                     ID=x.ID,
                     PayrollID = x.PayrollID,
                     PayrollNumber = x.PayrollNumber,
                     DafPayNumber = x.DafPayNumber,
                     PayNumber = x.PayNumber,
                     StudentNumber = x.StudentNumber,
                     Name = x.Name,
                     PayrollMoneyValue = x.PayrollMoneyValue,
                     UserID =x.UserID,
                     TotalChecksValues = x.TotalChecksValues,
                     NoOfChecks=x.NoOfChecks,
                     checkvalue=x.checkvalue,
                     CheckNumber=x.CheckNumber,
                     Exportdate=x.Exportdate,
                     Description=x.Description,
                     Filepath=x.Filepath

                 }).ToList());
        }

        public ActionResult GetPayRollsExportedChecksForManyUsers(string userID, string payrollID)
        {
            return this.JsonMaxLength(
                dbSC.Usp_GetPayRollsExportedChecksForManyUsers_Details(payrollID, userID).ToList()
                 .Select(x => new Usp_GetPayRollsExportedChecksForManyUsers_Details_Result
                 {
                     ID = x.ID,
                     PayrollID = x.PayrollID,
                     PayrollNumber = x.PayrollNumber,
                     DafPayNumber = x.DafPayNumber,
                     PayNumber = x.PayNumber,
                     StudentNumber = x.StudentNumber,
                     Name = x.Name,
                     PayrollMoneyValue = x.PayrollMoneyValue,
                     UserID = x.UserID,
                     checkvalue = x.checkvalue,
                     CheckNumber = x.CheckNumber,
                     Exportdate = x.Exportdate,
                     Description = x.Description,
                     Filepath = x.Filepath

                 }).ToList());
        }

        public ActionResult GetDelevirData(string yearID, string monthID, int? payrollID)
        {
            var user = (DashBoard_Users)Session["UserId"];

            //var IsAdmin = dbSC.DashBoard_Users.Where(m => m.ID == userid).Select(m => m.IsAdmin).FirstOrDefault();
            var IsMoneyExchange = dbSC.AcademicCommitteeUsersPerm.Where(m => m.UserID == user.ID).Select(m => m.IsMoneyExchange).FirstOrDefault();
            if (user.IsAdmin == true)
            {
                return this.JsonMaxLength(
                                dbSC.Usp_DelveringToStudents(payrollID, yearID, monthID).ToList()
                                 .Select(x => new Usp_DelveringToStudents_Result
                                 {
                                     PayrollID = x.PayrollID,
                                     PayrollNumber = x.PayrollNumber,
                                     STUDENT_ID = x.STUDENT_ID,
                                     NATIONAL_ID = x.NATIONAL_ID,
                                     student_name = x.student_name,
                                     LastStageDate = x.LastStageDate,
                                     PayrollMoneyValue = x.PayrollMoneyValue,
                                     UserID = x.UserID,
                                     FACULTY_NAME = x.FACULTY_NAME,
                                     DEGREE_DESC = x.DEGREE_DESC,
                                     Name = x.Name

                                 }).ToList());
            }
            else if (user.IsAdmin != true && IsMoneyExchange == true)
            {
                return this.JsonMaxLength(
                              dbSC.Usp_DelveringToStudents(payrollID, yearID, monthID).Where(m => m.UserID == user.ID).ToList()
                               .Select(x => new Usp_DelveringToStudents_Result
                               {
                                   PayrollID = x.PayrollID,
                                   PayrollNumber = x.PayrollNumber,
                                   STUDENT_ID = x.STUDENT_ID,
                                   NATIONAL_ID = x.NATIONAL_ID,
                                   student_name = x.student_name,
                                   LastStageDate = x.LastStageDate,
                                   PayrollMoneyValue = x.PayrollMoneyValue,
                                   UserID = x.UserID,
                                   FACULTY_NAME = x.FACULTY_NAME,
                                   DEGREE_DESC = x.DEGREE_DESC,
                                   Name = x.Name

                               }).ToList());
            }
            else
            {
                return null;
            }

        }


        public JsonResult GetPayRollsMoneyForIssuingChecks()
        {
            return this.JsonMaxLength(
               dbSC.Usp_GetPayRollsMoneyForIssuingChecks().ToList()
                .Select(x => new Usp_GetPayRollsMoneyForIssuingChecks_Result
                {
                    PayrollID = x.PayrollID,
                    PayrollNumber = x.PayrollNumber,
                    PayNumber = x.PayNumber,
                    DafPayNumber = x.DafPayNumber,
                    PayrollMoneyValue = x.PayrollMoneyValue,
                    StudentNumber = x.StudentNumber,
                    LastStageDate = x.LastStageDate,
                    TotalChecksValues =x.TotalChecksValues
                }).ToList());
        }
        public JsonResult GetPayRollsMoneyAfterBeingMonterayDetails(int payrollID)
        {
            var ChecksNotHaveFilesCount = dbSC.PayrollChecks.Where(m => m.PayrollID == payrollID && m.IsActive == true && m.IsReceived != true && m.FilePath == null).Count();
            var ChecksIsNotPrintedCount = dbSC.PayrollChecks.Where(m => m.PayrollID == payrollID && m.IsActive == true && m.IsReceived != true && m.IsPrinted!=true ).Count();
            
            return this.JsonMaxLength(
               dbSC.Usp_GetPayRollsMoneyAfterIsBeingMonetary_Details(payrollID).ToList()
                .Select(x => new Usp_GetPayRollsMoneyAfterIsBeingMonetary_Details_Result
                {
                    PayrollID = x.PayrollID,
                    PayrollNumber = x.PayrollNumber,
                    DafPayNumber = x.DafPayNumber,
                    PayNumber = x.PayNumber,
                    StudentNumber = x.StudentNumber,
                    Name = x.Name,
                    PayrollMoneyValue = x.PayrollMoneyValue,
                    UserID = x.UserID,
                    TotalChecksValues =x.TotalChecksValues,
                    NoOfChecks = x.NoOfChecks,
                    RemainValue =x.RemainValue,
                    CheckNumbers =x.CheckNumbers,
                    ChecksNotHaveFilesCount= ChecksNotHaveFilesCount,
                    ChecksIsNotPrinted= ChecksIsNotPrintedCount
                }).ToList());
        }


        public JsonResult GetFacultiesThatNotRegisteredWithMoneyExchangers(int payrollID)
        {
            var facultyname = dbSC.Usp_GetFacultiesThatNotRegisteredWithMoneyExchangers(payrollID);
            return this.JsonMaxLength((facultyname).ToList());
        }


        public JsonResult GetPayRollsMoneyDetails(int payrollID)
        {
            return this.JsonMaxLength(
               dbSC.Usp_GetPayRollsMoney_Details(payrollID).ToList()
                .Select(x => new Usp_GetPayRollsMoney_Details_Result
                {
                    PayrollID = x.PayrollID,
                    PayrollNumber = x.PayrollNumber,
                    DafPayNumber = x.DafPayNumber,
                    PayNumber = x.PayNumber,
                    StudentNumber = x.StudentNumber,
                    Name = x.Name,
                    PayrollMoneyValue = x.PayrollMoneyValue,
                    UserID=x.UserID
                }).ToList());
        }

        public JsonResult GetPayRollsMoneyUsersDetails(int payrollID,int userID)
        {
            return this.JsonMaxLength(
               dbSC.Usp_GetPayRollsMoneyUsers_Details(payrollID, userID).ToList()
                .Select(x => new Usp_GetPayRollsMoneyUsers_Details_Result
                {
                    PayrollID = x.PayrollID,
                    PayrollNumber = x.PayrollNumber,
                    faculty_name = x.faculty_name,
                    DEGREE_DESC = x.DEGREE_DESC,
                    PayrollMoneyValue = x.PayrollMoneyValue,
                    StudentNumber = x.StudentNumber,
                    name = x.name
                }).ToList());
        }

        public JsonResult GetPayRollsMoneyUsersAfterBeingMonterayDetails(int payrollID, int userID)
        {
            return this.JsonMaxLength(
               dbSC.Usp_GetPayRollsMoneyUsersAfterBeingMonteray_Details(payrollID, userID).ToList()
                .Select(x => new Usp_GetPayRollsMoneyUsersAfterBeingMonteray_Details_Result
                {
                    PayrollID = x.PayrollID,
                    PayrollNumber = x.PayrollNumber,
                    faculty_name = x.faculty_name,
                    DEGREE_DESC = x.DEGREE_DESC,
                    PayrollMoneyValue = x.PayrollMoneyValue,
                    StudentNumber = x.StudentNumber,
                    name = x.name
                }).ToList());
        }
        public JsonResult GetPayRollChecks(int payrollID, int userID)
        {
            var list = dbSC.PayrollChecks.Where(m => m.PayrollID == payrollID && m.BeneficiaryID == userID).ToList().Select(m=>new {
                ID =m.ID,
                BeneficiaryID = m.BeneficiaryID,
                PayrollID=m.PayrollID,
                Name=m.DashBoard_Users.Name,
                ExportDate=m.ExportDate.Value.ToString("dd/MM/yyyy"),
                CheckNumber=m.CheckNumber,
                CheckValue=m.CheckValue,
                Description = m.Description,
                IsReceived = m.IsReceived ==true?"مستلم":"غير مستلم",
                IsPrinted= m.IsPrinted,
                IsActive=m.IsActive,
                FilePath=m.FilePath
            }).ToList();
            return this.JsonMaxLength(list);
        }
        public JsonResult GetPayrollCheckById(int ID)
        {
            var obj = dbSC.PayrollChecks.Where(m => m.ID == ID).Select(x=>new { x.ID ,x.PayrollID ,x.BeneficiaryID ,x.CheckNumber,x.CheckValue,x.Description,x.FacultyID,x.IsPrinted ,x.IsActive,x.IsReceived}).FirstOrDefault();
            return this.JsonMaxLength(obj);
        }
        public JsonResult GetChecksValues(int payrollID, int userID)
        {
            return this.JsonMaxLength(
          dbSC.Usp_GetPayRollIntialChecksValues(payrollID, userID).ToList()
           .Select(x => new Usp_GetPayRollIntialChecksValues_Result
           {
               ID = payrollID,
               PayrollNumber = x.PayrollNumber,
               PayrollMoneyValue = x.PayrollMoneyValue,
               totalcheckesvalueforuser = x.totalcheckesvalueforuser,
               totalvaluewithoutchecks = x.totalvaluewithoutchecks,
               CheckValue = x.CheckValue,
               username = x.username
           }).ToList());
        }

        public ActionResult GetFaculties(int userID,int payrollID)
        {
            var Faculties = dbSC.Usp_GetFacultyWithUserAndPayroll(payrollID,userID).Select(x => new SelectListItem { Text = x.faculty_name, Value = x.faculty_no.ToString() }).ToList();
            return Json(Faculties, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFacultiesForCheck(int userID, int payrollID,int checkID)
        {
            var Faculties = dbSC.Usp_GetFacultyWithUserAndPayrollForCheck(payrollID, userID, checkID).Select(x => new SelectListItem { Text = x.faculty_name, Value = x.faculty_no.ToString() }).ToList();
            return Json(Faculties, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult saveCheck(int ID, int BeneficiaryID,int PayrollID,string FacultyID,string CheckNumber,decimal CheckValue,string Description)
        {
            var CheckNumberChecked= dbSC.PayrollChecks.Where(m=>m.CheckNumber ==  CheckNumber  && m.ID != ID).FirstOrDefault() ;
            if (CheckNumberChecked != null )
                return Content("يوجد شيك بنفس الرقم من قبل");
       
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(CheckNumber))
                    return Content("رقم الشيك مطلوب");
                if (CheckValue ==0)
                    return Content("قيمة الشيك مطلوب");
                if (string.IsNullOrEmpty(FacultyID))
                    return Content("الكليات مطلوبة");
               
                if (ID == 0)
                {
                    PayrollChecks payrollChecks = new PayrollChecks();
                    payrollChecks.ID = ID;
                    payrollChecks.PayrollID = PayrollID;
                    payrollChecks.CheckNumber = CheckNumber;
                    payrollChecks.CheckValue = CheckValue;
                    payrollChecks.ExportDate = DateTime.Now;
                    payrollChecks.BeneficiaryID  = BeneficiaryID;
                    payrollChecks.FacultyID = FacultyID;
                    payrollChecks.IsActive = true;
                    payrollChecks.Description = Description;
                    dbSC.PayrollChecks.Add(payrollChecks);
                    dbSC.SaveChanges();
                }
                else
                {
                 
                    var row = dbSC.PayrollChecks.Find(ID);
                    row.FacultyID = FacultyID;
                    row.CheckNumber = CheckNumber;
                    row.CheckValue = CheckValue;
                    row.Description = Description;
                    dbSC.Entry(row).State = EntityState.Modified;
                    dbSC.SaveChanges();
                }
            }
            return Content("");
        }

        [HttpPost]
        public ActionResult cancelCheck(int ID)
        {
           
            if (ModelState.IsValid)
            {
               
                    var row = dbSC.PayrollChecks.Find(ID);
                    row.IsActive = false;
                    dbSC.Entry(row).State = EntityState.Modified;
                    dbSC.SaveChanges();
               
            }
            return Content("");
        }

        [HttpGet]
        public ActionResult DownloadFile(int PayrollApprovalId)
        {
            var FileVirtualPath = dbSC.PayrollApproval
                                       .Where(x => x.ID == PayrollApprovalId)
                                       .FirstOrDefault()?.FilePath;

            if (FileVirtualPath != null)
            {
                if (!System.IO.File.Exists(HostingEnvironment.MapPath(FileVirtualPath)))
                {
                    return Content("");
                }
                byte[] FileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(FileVirtualPath));
                return File(FileBytes, "application/pdf");

            }
            return Content("");
        }
        [HttpGet]
        public ActionResult DownloadCheckFile(int ID)
        {
            var rec = dbSC.PayrollChecks
                                       .Where(x => x.ID == ID)
                                       .FirstOrDefault();
            var FileVirtualPath = rec?.FilePath;
            var ext = rec?.FileName.Split('.')[1];
            if (FileVirtualPath != null)
            {
                if (!System.IO.File.Exists(HostingEnvironment.MapPath(FileVirtualPath)))
                {
                    return Content("");
                }
                byte[] FileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(FileVirtualPath));
                return File(FileVirtualPath, "image/"+ ext);
            }
            return Content("");
        }
        public JsonResult GetPayRollFiles(int payrollID)
        {
            var result = dbSC.PayrollApproval.Where(m => m.PayrollID == payrollID).Include(m=>m.PayrollPhasesUsers).ToList()
                .Select(x => new
                {

                    ID = x.ID,
                    InsertDate = x.InsertDate.Value.ToString("dd/MM/yyyy"),
                    PhaseName = x.PayrollPhasesUsers.PayrollPhases.PhaseName,
                    FileName=x.FileName ,
                    FilePath=x.FilePath
                    
                }).ToList();
            return Json(result
            , JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPayRollsDetails(int? payrollID)
        {
            var param = new SqlParameter("@PayrollID", System.Data.SqlDbType.Int);
            param.Value = payrollID;

            DataSet dataSet = SqlHelper.ExecuteDataset(SqlHelper.Gam3aConnectionString, "Usp_RewardsPayrollsPivot_Banki", param);
            if (dataSet.Tables.Count > 0)
            {
                DataTable datatable = dataSet.Tables[0];
                var dt = JsonConvert.SerializeObject(datatable);
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPayRollsMonetaryDetails(int? payrollID)
        {


            var param = new SqlParameter("@PayrollID", System.Data.SqlDbType.Int);
            param.Value = payrollID;

            DataSet dataSet = SqlHelper.ExecuteDataset(SqlHelper.Gam3aConnectionString, "Usp_RewardsPayrollsPivot_Monetary", param);
            if(dataSet.Tables.Count > 0)
            {

                var dt = JsonConvert.SerializeObject(dataSet.Tables[0]);

                return Json(dt, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult UploadFiles()
        {
            Session["PayRollStudentsFile"] = null;
            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentType != "application/pdf")
                {
                    return new HttpStatusCodeResult(400);
                }
                else
                {
                    Session["PayRollStudentsFile"] = file;
                }
            }
            return Json(0);
        }
        [HttpPost]
        public ActionResult UploadSandFiles()
        {
            Session["SandFiles"] = null;
            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentType != "application/pdf")
                {
                    return new HttpStatusCodeResult(400);
                }
                else
                {
                    Session["SandFiles"] = file;
                }
            }
            return Json(0);
        }
        [HttpPost]
        public ActionResult UploadDelevirFiles()
        {
            Session["DelevirFiles"] = null;
            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentType != "application/pdf")
                {
                    return new HttpStatusCodeResult(400);
                }
                else
                {
                    Session["DelevirFiles"] = file;
                }
            }
            return Json(0);
        }

        [HttpPost]
        public ActionResult UploadCheckFiles()
        {
            Session["CheckFile"] = null;
            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                //if (  (file.ContentType.ToLower() != "image/jpeg")
                //        //|| (file.ContentType.ToLower() != "image/jpg")
                //        //|| (file.ContentType.ToLower() != "image/tif")
                //        //|| (file.ContentType.ToLower() != "image/png")
                //        //|| (file.ContentType.ToLower() != "image/gif")

                //{
                //    return new HttpStatusCodeResult(400);
                //}
                //else
                //{
                Session["CheckFile"] = file;
                // }
                return Json(0);
            }
            else
            {
                return new HttpStatusCodeResult(400);
            }
        }
        public JsonResult GetPayrollApprovalHistory(int payrollID)
        {

            var currentPayrollHistory = dbSC.Payroll.Where(p => p.ID == payrollID).SingleOrDefault().PayrollApproval;
            var ApprovedPhases = currentPayrollHistory.Where(x => x.IsRefused != true).ToList();
            var current = ApprovedPhases.Count() + 1;
            var result = dbSC.PayrollPhases.Where(x => x.IsActive == true && x.PhaseOrder == current).FirstOrDefault();
            string PhaseName = "";
            if (result != null)
            {
                PhaseName = result.PhaseName;
            }
            string notes = "";
            var LastPayrollApproval = dbSC.PayrollApproval.Where(p => p.PayrollID == payrollID).OrderByDescending(m => m.ID).FirstOrDefault();
            if (LastPayrollApproval != null)
            {
                notes = LastPayrollApproval.Notes;
            }
           // return Json(notes, JsonRequestBehavior.AllowGet);
            //var result = dbSC.PayrollPhases.Where(x => x.IsActive == true).ToList().Select(x => new
            //{
            //    PhaseName = x.PhaseName,
            //    PhaseID = x.ID,
            //    IsApproved = ApprovedPhases.Count() >= x.PhaseOrder ? true : false,
            //    IsCurrent = x.PhaseOrder == ApprovedPhases.Count() + 1 ? true : false,
            //    IsNext = x.PhaseOrder > ApprovedPhases.Count() + 1 ? true : false
            //}).ToList();
            return Json(new { PhaseName = PhaseName ,Notes= notes }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastApprovalNotes(int payrollID)
        {
            string notes = "";
            var LastPayrollApproval = dbSC.PayrollApproval.Where(p => p.PayrollID == payrollID).OrderByDescending(m=>m.ID).FirstOrDefault();
            if (LastPayrollApproval != null)
            {
                notes = LastPayrollApproval.Notes;
            }
            return Json(notes, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PayrollApprovallAction(int payrollID, string notes,string PayNo,string DafPayNo, string DafPayNo2, bool isApproved)
        {
            var CurrentUser = Session["UserId"] as DashBoard_Users;
            int currentPhase = dbSC.PayrollApproval.Where(x => x.PayrollID == payrollID).Count() + 2;
            PayrollApproval payrollApproval = new PayrollApproval();
            payrollApproval.IsApproved = isApproved;
            payrollApproval.IsRefused = !isApproved;
            payrollApproval.InsertDate = DateTime.Now;
            payrollApproval.PayrollID = payrollID;
            payrollApproval.Notes = notes;
            payrollApproval.PayrollPhaseUserID = currentPhaseUser(currentPhase);
            var file = (HttpPostedFileBase)Session["PayRollStudentsFile"];
            if (file == null && isApproved==true)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع الملف المرفق");
            }
            else if (file != null && (isApproved == true || isApproved ==false))
            {

                payrollApproval.FileName = file.FileName;
                payrollApproval.FilePath = GetFilePath(file, $"ApprovedFiles");
            }
            dbSC.PayrollApproval.Add(payrollApproval);
            dbSC.SaveChanges();
            Payroll payroll = new Payroll();
            payroll = dbSC.Payroll.Find(payrollID);
            payroll.PayNo = PayNo==""?null:PayNo;
            payroll.DafPayNo = DafPayNo == "" ? null : DafPayNo;
            payroll.DafPayNo2 = DafPayNo2 == "" ? null : DafPayNo2;
            dbSC.Entry(payroll).State = EntityState.Modified;
            dbSC.SaveChanges();
            try
            {  
               Session["PayRollStudentsFile"] = null;
                //في حالة مرحلة اعتماد مالي يتم خصم أقساط السلف المستحقة
                if (
                    dbSC.PayrollPhases.Where(x=>x.PhaseOrder == currentPhase).Any(x => x.IsFinancialApprove==true)
                    && 
                    isApproved && payroll.IsMinister !=true
                   )
                {
                    var result = dbSC.Usp_CalcStudentsRewardWithAdvances(payrollID, CurrentUser.ID);
                    
                    //var payrollAdvances = AdvancePartReturningByPayroll(payrollID, null);
                    //payrollAdvances.GroupBy(x => new { x.StudentId, x.AdvancePaymentDetailsId }).ToList()
                    //    .ForEach(x =>
                    //    {
                    //        var recieveMaster = new AdvanceReceiveMaster()
                    //        {
                    //            COA_ID = dbSC.AdvancePaymentDetails.Where(p => p.ID == x.Key.AdvancePaymentDetailsId).SingleOrDefault()
                    //            .AdvanceRequests.AdvanceSettings.CoaId_RecieveFromPayroll.Value,
                    //            DocNumber = "0",
                    //            DocNotes = "",
                    //            DocHeader = "",
                    //            InsertionDate = DateTime.Now,
                    //            IsPrinted = false,
                    //            PayrollId = payrollID,
                    //            Student_Id = x.Key.StudentId,
                    //            TotalValue = x.Sum(p => p.Value.HasValue ? p.Value.Value : 0),
                    //            User_Id = CurrentUser.ID

                    //        };
                    //        dbSC.AdvanceReceiveMaster.Add(recieveMaster);
                    //        dbSC.SaveChanges();

                    //        dbSC.AdvanceReceiveDetails.AddRange(x.Select(p => new AdvanceReceiveDetails
                    //        {
                    //            NetValue = p.Value.HasValue ? p.Value.Value : 0,
                    //            AdvancePaymentDetails_Id = p.AdvancePaymentDetailsId,
                    //            AdvanceReceiveMaster_Id = recieveMaster.ID,

                    //        }));
                    //        dbSC.SaveChanges();
                    //    });


                    ////  var payrollAdvances =dbSC.StudentPayroll.Where(m=>m.PayrollID == payrollID && m.RewardItems.IsAdvanceReturn ==true) //AdvancePartReturningByPayroll(payrollID, null);
                    ////  payrollAdvances.GroupBy(x => new { x.StudentId,x.AdvancePaymentDetailsId }).ToList()
                    //var payrollAdvances = dbSC.StudentPayroll.Where(m => m.PayrollID == payrollID && m.RewardItems.IsAdvanceReturn == true);//.GroupBy(x => new { x.StudentID, x.AdvancePaymentDetailsId }).ToList().ForEach(x =>
                    //    {
                    //        var recieveMaster = new AdvanceReceiveMaster()
                    //        {
                    //            COA_ID = dbSC.AdvancePaymentDetails.Where(p => p.ID == x.Key.AdvancePaymentDetailsId).SingleOrDefault()
                    //            .AdvanceRequests.AdvanceSettings.CoaId_RecieveFromPayroll.Value,
                    //            DocNumber = "0",
                    //            DocNotes = "",
                    //            DocHeader = "",
                    //            InsertionDate = DateTime.Now,
                    //            IsPrinted = false,
                    //            PayrollId = payrollID,
                    //            Student_Id = x.Key.StudentId,
                    //            TotalValue = x.Sum(p => p.Value.HasValue ? p.Value.Value : 0),
                    //            User_Id = CurrentUser.ID

                    //        };
                    //        dbSC.AdvanceReceiveMaster.Add(recieveMaster);
                    //        dbSC.SaveChanges();

                    //        dbSC.AdvanceReceiveDetails.AddRange(x.Select(p=> new AdvanceReceiveDetails {
                    //            NetValue = p.Value.HasValue? p.Value.Value:0,
                    //            AdvancePaymentDetails_Id = p.AdvancePaymentDetailsId,
                    //            AdvanceReceiveMaster_Id = recieveMaster.ID,

                    //        }));
                    //        dbSC.SaveChanges();
                    //    });
                    //var StudentPayrollHaveAdvances= dbSC.StudentPayroll.Where(m => m.PayrollID == payrollID && m.RewardItems.IsAdvanceReturn == true && m.StudentID == 412043673).ToList();

                    //foreach(var item in StudentPayrollHaveAdvances)
                    //{

                    //    var advances = dbSC.AdvancePaymentDetails.Where(
                    //                     p => p.AdvanceRequests.AdvanceSettings.AdvanceType == "A"
                    //                     && p.AdvancePaymentMaster.Student_Id == item.StudentID && item.RewardItems.IsAdvanceReturn ==true
                    //                     && (p.AdvanceReceiveDetails.Count() == 0 || p.AdvanceReceiveDetails
                    //                    .Sum(c => c.NetValue) < p.NetValue)).OrderBy(c => c.AdvancePaymentMaster.InsertionDate).GroupBy(m=>new { item.StudentID,m.ID}).ToList();
                    //    foreach(var item2 in advances)
                    //    {
                    //        var recieveMaster = new AdvanceReceiveMaster() {
                    //            COA_ID = item2.Where(m=>m.ID == item2.Key.ID).SingleOrDefault().AdvanceRequests.AdvanceSettings.CoaId_RecieveFromPayroll.Value,
                    //            DocNumber = "0",
                    //            DocNotes = "",
                    //            DocHeader = "",
                    //            InsertionDate = DateTime.Now,
                    //            IsPrinted = false,
                    //            PayrollId = payrollID,
                    //            Student_Id = item.StudentID,
                    //            TotalValue = StudentPayrollHaveAdvances.Sum(o=>o.Value),
                    //            User_Id = CurrentUser.ID
                    //        };

                    //    }
                    //    //var recieveMaster = new AdvanceReceiveMaster()
                    //    //    recieveMaster.COA_ID = 
                    //}
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json("حدث خطأ أثناء الحفظ", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult SaveCheckFiles(int ID)
        {
            var file = (HttpPostedFileBase)Session["CheckFile"];
            if (file == null)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع الملف المرفق");
            }
            else if (file != null)
            {
                var row = dbSC.PayrollChecks.Find(ID);
                row.FileName = file.FileName;
                row.FilePath = GetFilePath(file, $"Checks");
                dbSC.Entry(row).State = EntityState.Modified;
            }
            try
            {
                dbSC.SaveChanges();
                Session["CheckFile"] = null;
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json("حدث خطأ أثناء الحفظ", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult PayrollApprovallActionMonetary(int payrollID, string notes, string PayNo, string DafPayNo, bool isApproved,bool? isexportedcheck)
        {
            var CurrentUser = Session["UserId"] as DashBoard_Users;
            int currentPhase = dbSC.PayrollApproval.Where(x => x.PayrollID == payrollID).Count() + 2;
            PayrollApproval payrollApproval = new PayrollApproval();
            payrollApproval.IsApproved = isApproved;
            payrollApproval.IsRefused = !isApproved;
            payrollApproval.InsertDate = DateTime.Now;
            payrollApproval.PayrollID = payrollID;
            payrollApproval.Notes = notes;
            payrollApproval.PayrollPhaseUserID = currentPhaseUser(currentPhase);
            var file = (HttpPostedFileBase)Session["PayRollStudentsFile"];
            if (file == null && isApproved == true)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع الملف المرفق");
            }
            else if (file != null && (isApproved == true || isApproved == false))
            {

                payrollApproval.FileName = file.FileName;
                payrollApproval.FilePath = GetFilePath(file, $"ApprovedFiles");
            }
            dbSC.PayrollApproval.Add(payrollApproval);

            Payroll payroll = new Payroll();
            payroll = dbSC.Payroll.Find(payrollID);
            payroll.IsMonetary = true;
            payroll.IsExportedCheck = isexportedcheck;
            dbSC.Entry(payroll).State = EntityState.Modified;
            try
            {
                dbSC.SaveChanges();
                Session["PayRollStudentsFile"] = null;
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json("حدث خطأ أثناء الحفظ", JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public ActionResult UpdateChecksState(int[] PayrollCheckIDS,int PayrollID)
        {
            try
            {
                var Payroll = dbSC.Payroll.Find(PayrollID);

                for (int i = 0; i < PayrollCheckIDS.Length; i++)
                {

                    PayrollChecks PayrollChecks = dbSC.PayrollChecks.Find(PayrollCheckIDS[i]);
                    PayrollChecks.IsReceived = true;
                    PayrollChecks.ReceiveDate = DateTime.Now;
                    var file = (HttpPostedFileBase)Session["SandFiles"];
                    if (file == null)
                    {
                        return Content("لا يمكن إتمام عملية تسليم الشيك حتي يتم رفع الملف المرفق");
                    }
                    else if (file != null)
                    {

                        PayrollChecks.SandFileName = file.FileName;
                        PayrollChecks.SandFilePath = GetFilePath(file, $"SandFiles");
                    }
                    dbSC.Entry(PayrollChecks).State = EntityState.Modified;
                    dbSC.SaveChanges();
                }
                Session["SandFiles"] = null;
                var IsNotReceivedyet = dbSC.PayrollChecks.Where(m => m.IsReceived != true && m.IsActive == true).Count();
                if(IsNotReceivedyet == 0)
                {
                    var CurrentUser = Session["UserId"] as DashBoard_Users;
                    int currentPhase = dbSC.PayrollApproval.Where(x => x.PayrollID == PayrollID).Count() + 2;
                    PayrollApproval payrollApproval = new PayrollApproval();
                    payrollApproval.IsApproved = true;
                    payrollApproval.IsRefused = false;
                    payrollApproval.InsertDate = DateTime.Now;
                    payrollApproval.PayrollID = PayrollID;
                    payrollApproval.PayrollPhaseUserID = currentPhaseUser(currentPhase);
                    dbSC.PayrollApproval.Add(payrollApproval);
                    dbSC.SaveChanges();
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("حدث خطأ أثناء الحفظ", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult UpdateStudentPayrollState(int[] studentids,int[] payrollids , int [] peneficiaryIds,int payrollID,int yearID,int monthID)
        {
             try
            {
              
                for (int i = 0; i < studentids.Length; i++)
                {
                    int userid = peneficiaryIds[i];
                    int stdid = studentids[i];
                    List<StudentPayroll> StudentPayrollList = dbSC.StudentPayroll.Where(m=>m.StudentID == stdid && m.PayrollID == payrollID).ToList();
                    foreach(var StudentPayroll in StudentPayrollList)
                    {
                        StudentPayroll.IsPaid = true;
                        StudentPayroll.BeneficiaryID = peneficiaryIds[i];
                        StudentPayroll.DeliveryDate = DateTime.Now;
                        var file = (HttpPostedFileBase)Session["DelevirFiles"];
                        if (file == null)
                        {
                            return Content("لا يمكن إتمام عملية تسليم الشيك حتي يتم رفع الملف المرفق");
                        }
                        else if (file != null)
                        {

                            StudentPayroll.DelevirFileName = file.FileName;
                            StudentPayroll.DelevirFilePath = GetFilePath(file, $"DelevirFiles", stdid, payrollID);
                        }
                        dbSC.Entry(StudentPayroll).State = EntityState.Modified;
                        dbSC.SaveChanges();
                    }
                  
                }
                var IsNotPaidCount = (from s in dbSC.INTEGRATION_All_Students.Where(m => m.ACCOUNT_NO == null)
                                      join sp in dbSC.StudentPayroll.Where(m => m.PayrollID == payrollID && m.IsPaid != true)
                                      on s.STUDENT_ID equals sp.StudentID
                                      select s.STUDENT_ID
                         ).Distinct().Count();
                if (IsNotPaidCount == 0)
                {
                    var CurrentUser = Session["UserId"] as DashBoard_Users;
                    int currentPhase = dbSC.PayrollApproval.Where(x => x.PayrollID == payrollID).Count() + 2;
                    PayrollApproval payrollApproval = new PayrollApproval();
                    payrollApproval.IsApproved = true;
                    payrollApproval.IsRefused = false;
                    payrollApproval.InsertDate = DateTime.Now;
                    payrollApproval.PayrollID = payrollID;
                    payrollApproval.PayrollPhaseUserID = currentPhaseUser(currentPhase);
                    dbSC.PayrollApproval.Add(payrollApproval);
                    dbSC.SaveChanges();
                }


                Session["DelevirFiles"] = null;
               
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("حدث خطأ أثناء الحفظ", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult GetAllPayRolls(int? exchangeOrderId)
        {
            var allApprovalPhases = dbSC.PayrollPhases.Select(x => x.ID).ToList();
            var result = dbSC.Payroll.Where(x =>
            allApprovalPhases.All(p=> x.PayrollApproval.Where(r=>r.IsApproved==true)
            .Select(c=> c.PayrollPhasesUsers.PayrollPhaseID).Contains(p))
            && (x.ExchangeOrderID == null || x.ExchangeOrderID==exchangeOrderId))
            .Select(x => new { x.ID, x.PayrollNumber ,x.IsActive}).Where(x=>x.IsActive == true).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


         [HttpGet]
        public ActionResult GetAllPayRollExchaneOrders()
        {
            var result = dbSC.ExchangeOrder.ToList().Select(x => new
            {
                x.ID,
                x.OrderNumber,
                x.IsActive,
                x.IsTotallyInChecks,
                Payrolls = string.Join("," , x.Payroll.ToList().Select(b=>b.PayrollNumber))
            }).ToList();        
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        public ActionResult addNewPayRollExchaneOrders(ExchangeOrderObject exchangeOrderObject)
        {
                if (exchangeOrderObject.ExchangeOrderID== null)
                {
                    int? payrollSingleExchangeOrder = null;
                    for (int i = 0; i < exchangeOrderObject.payllosId.Count; i++)
                    {
                        payrollSingleExchangeOrder = dbSC.Payroll.ToList().Where(x => x.ID == exchangeOrderObject.payllosId[i]).SingleOrDefault().ExchangeOrderID;
                        if (payrollSingleExchangeOrder != null)
                            return Json("عفوا يوجد امر صرف مسجل بالفعل", JsonRequestBehavior.AllowGet);
                    }

                    if (dbSC.ExchangeOrder.Any(x => x.OrderNumber == exchangeOrderObject.ExchangeOrder.OrderNumber))
                        return Json("هذا الامر مسجل من قبل", JsonRequestBehavior.AllowGet);

                    exchangeOrderObject.ExchangeOrder.IsActive = true;
                    exchangeOrderObject.ExchangeOrder.CreatedAt = DateTime.Now;
                    var CurrentUser = Session["UserId"] as DashBoard_Users;
                    exchangeOrderObject.ExchangeOrder.CreatedBy = CurrentUser.ID;
                    exchangeOrderObject.ExchangeOrder.TotalValue = dbSC.Payroll.Where(x => exchangeOrderObject.payllosId.Any(p => p == x.ID)).ToList().Sum(x => x.TotalAmount);
                    exchangeOrderObject.ExchangeOrder.IsTotallyInChecks = false;
                    exchangeOrderObject.ExchangeOrder.IsActive = true;
                    dbSC.ExchangeOrder.Add(exchangeOrderObject.ExchangeOrder);
                    dbSC.SaveChanges();
                    dbSC.Payroll.Where(x => exchangeOrderObject.payllosId.Any(p => p == x.ID)).ToList().ForEach(x => x.ExchangeOrderID = exchangeOrderObject.ExchangeOrder.ID);
                    dbSC.SaveChanges();


                }

                else
                {
                    var exchangeOrderTobeDeActivated = dbSC.ExchangeOrder.FirstOrDefault(x => x.ID == exchangeOrderObject.ExchangeOrderID);
                    if (exchangeOrderTobeDeActivated != null)
                    exchangeOrderTobeDeActivated.OrderNumber = exchangeOrderObject.ExchangeOrder.OrderNumber;
                    dbSC.SaveChanges();

                    dbSC.Payroll.Where(x => x.ExchangeOrderID == exchangeOrderObject.ExchangeOrderID).ToList().ForEach(x => x.ExchangeOrderID = null);
                    dbSC.SaveChanges();
                    dbSC.Payroll.Where(x => exchangeOrderObject.payllosId.Any(p => p == x.ID)).ToList().ForEach(x => x.ExchangeOrderID = exchangeOrderObject.ExchangeOrderID);
                    dbSC.SaveChanges();
              
                  
                return Json("", JsonRequestBehavior.AllowGet);

            }


            return Content("");
            
        }
        [HttpPost]
        public ActionResult ActiveAndNotActive(ExchangeOrder exchangeOrder)
        {
           
            var exchangeOrderTobeDeActivated = dbSC.ExchangeOrder.Where(x => x.ID == exchangeOrder.ID).SingleOrDefault();
            if(exchangeOrderTobeDeActivated.IsActive != null)
            {
                if (exchangeOrderTobeDeActivated.IsActive == false)
                {
                    exchangeOrderTobeDeActivated.IsActive = true;
                    dbSC.SaveChanges();
                    return Content("تم الايقاف");
                    
                }
                else
                {
                    exchangeOrderTobeDeActivated.IsActive = false;
                    dbSC.SaveChanges();
                    return Content("تم التنشيط");

                }
               
               
            }
            return Content("");
        }

        [HttpPost]
        public JsonResult GetEditExChangeOrder(int ID)
        {
            var result = dbSC.ExchangeOrder.Where(x => x.ID == ID).SingleOrDefault().OrderNumber;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPayrollNumbers(int ID)
        {
            var result = dbSC.Payroll.Where(x => x.ExchangeOrder.ID == ID).Select(x=>x.PayrollNumber).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion



    }
}