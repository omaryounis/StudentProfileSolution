using Newtonsoft.Json;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public partial class AdvancePaymentController : Controller
    {
        private notify _notify;
        private readonly SchoolAccGam3aEntities dbSch = new SchoolAccGam3aEntities();
        private readonly EsolERPEntities dbAcc = new EsolERPEntities();


        // صرف السلف للطلاب
        public ActionResult Index()
        {
            var json = new JavaScriptSerializer().Serialize(GetAdvancePaymentPermissions(92).Data);
            var permissions = JsonConvert.DeserializeObject<AdvancePaymentPermission>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        // صرف الإعانات للطلاب
        public ActionResult PaySubsidy()
        {
            var json = new JavaScriptSerializer().Serialize(GetAdvancePaymentPermissions(93).Data);
            var permissions = JsonConvert.DeserializeObject<AdvancePaymentPermission>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult GetAdvanceSettings()
        {
            var AdvanceSettings = dbSch.AdvanceSettings.Where(x => x.AdvanceType == "A").Select(x => new SelectListItem { Text = x.AdvanceSettingName, Value = x.AdvanceSettingId.ToString() }).ToList();
            return Json(AdvanceSettings, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAdvanceRequests(string Type,int? StudentID)
        {
            var query = (from advanceRequest in dbSch.AdvanceRequests
                                                                .Where(x => x.IsCanceled != true &&
                                                                            x.RefusedbyID == null &&
                                                                            x.ApprovedbyID != null &&
                                                                            x.AdvanceSettings.AdvanceType == Type &&
                                                                            !x.AdvancePaymentDetails.Any(p => p.AdvanceRequests_Id == x.ID)
                                                                       )
                         join student in dbSch.INTEGRATION_All_Students.Where(m=>m.STUDENT_ID == StudentID)
                         on advanceRequest.Student_Id equals student.STUDENT_ID
                         select new
                         {
                             advanceRequest.ID,
                             student.STUDENT_NAME,
                             student.STUDENT_ID,
                             student.NATIONAL_ID,
                             AdvanceSettingName = advanceRequest.AdvanceSettings.AdvanceSettingName,
                             advanceRequest.RequestedValue,
                             advanceRequest.ApprovedValue,
                             InsertionDate = advanceRequest.InsertionDate.Day + "/" + advanceRequest.InsertionDate.Month + "/" + advanceRequest.InsertionDate.Year,
                             UserName = dbSch.DashBoard_Users.FirstOrDefault(p => p.ID == advanceRequest.ApprovedbyID).Name,
                             advanceRequest.RequestNotes
                         });

            return this.JsonMaxLength(query.ToList());
        }




        public ActionResult GetAllStudentsHaveAdvances(string Type)
        {
            var query = (from advanceRequest in dbSch.AdvanceRequests
                                                                .Where(x => x.IsCanceled != true &&
                                                                            x.RefusedbyID == null &&
                                                                            x.ApprovedbyID != null &&
                                                                            x.AdvanceSettings.AdvanceType == Type &&
                                                                            !x.AdvancePaymentDetails.Any(p => p.AdvanceRequests_Id == x.ID)
                                                                       )
                         join student in dbSch.INTEGRATION_All_Students
                         on advanceRequest.Student_Id equals student.STUDENT_ID
                         select new
                         {
                             STUDENT_NAME = student.STUDENT_NAME,
                             STUDENT_ID = student.STUDENT_ID
                             //,
                             //DegreeName =student.DEGREE_DESC,
                             //National_Id=student.NATIONAL_ID,
                             //StatusName=student.STATUS_CODE,
                             //FacultiyName=student.FACULTY_NAME,
                             //StudyTypeName=student.STUDY_DESC,
                             //NationalityName=student.NATIONALITY_DESC,
                             //StudentAverageDegree=student.DEGREE_CODE
                         });

            return this.JsonMaxLength(query.ToList());
        }


        public ActionResult GetAdvanceApprovedPhasesByRequestId(int id)
        {

            var AdvanceRequests = dbSch.AdvanceApprovedPhases.Where(x => x.AdvanceRequested_ID == id)
                            .Select(x => new
                            {
                                x.ID,
                                x.AdvanceRequested_ID,
                                PhaseName = x.AdvancePhases.PhaseName,
                                x.AdvancePhases.Order,
                                UserName = x.DashBoard_Users.Name,
                                ResponseDate = x.ResponseDate.Day + "/" + x.ResponseDate.Month + "/" + x.ResponseDate.Year,
                                x.Reason,
                                x.ApprovedValue,
                                ApprovedStatus = x.ApprovedStatus == true ? "موافقة" : x.ApprovedStatus == false ? "رفض" : ""
                            }).OrderBy(x => x.Order).ToList();
            return Json(data: AdvanceRequests, behavior: JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAcctounts()
        {
            var user = HttpContext.Session["UserId"] as DashBoard_Users;
            var UserInAcc = dbAcc.Users.FirstOrDefault(x => x.ID == user.AccID);
            if (UserInAcc == null)
            {
                return this.JsonMaxLength(new SelectListItem());
            }

            return this.JsonMaxLength(dbAcc.COA.Where(
                                                       x => x.COA_Type == "S" &&
                                                       (x.COM_ID == UserInAcc.School_ID || x.COM_ID == null) &&
                                                       x.AccountSecurity.Any(y => y.UserId == UserInAcc.ID)
                                                     )
                                               .Select(x => new
                                               {
                                                   Value = x.COAID,
                                                   Text = x.COACode + "-" + x.COADescription
                                               }).ToList());

        }


        public ActionResult SaveAdvances(SaveAdvancesVM model)
        {
            dbAcc.Configuration.LazyLoadingEnabled = false;
            dbSch.Configuration.LazyLoadingEnabled = false;


            var UserInAcc = dbAcc.Users.FirstOrDefault(x => x.ID == CurrentUser.AccID);
            if (UserInAcc == null)
            {
                return this.JsonMaxLength("عفوا لا يمكن إتمام عملية الحفظ حتي يتم ربط بيانات المستخدم بنظام الحسابات العامة");
            }

            int fsyid = dbAcc.FSY.Where(x => x.COM_ID == UserInAcc.School_ID && x.Is_Open == true)
                                .SingleOrDefault().FSYID;
            if (fsyid == 0)
            {
                return this.JsonMaxLength("عفوا لا يمكن إتمام عملية الحفظ حيث لا يوجد أي سنة مالية مفتوحة حاليا بداخل بنظام الحسابات العامة");
            }

            //=====================================
            // دي السلف والإعانات اللي بيتم صرفها 
            //=====================================
            var advanceRequestIds = model.AdvanceRequests.Split(',').Select(x => Convert.ToInt32(x)).ToList();

            var advanceRequests = dbSch.AdvanceRequests
                                       .Include(x => x.AdvanceSettings)
                                       .Where(x => x.IsCanceled != true &&
                                                    x.RefusedbyID == null &&
                                                    x.ApprovedbyID != null &&
                                                    advanceRequestIds.Any(c => c == x.ID) &&
                                                    !x.AdvancePaymentDetails.Any(p => p.AdvanceRequests_Id == x.ID)
                                             );


            //======================================
            // كدا معانا البيانات الأكاديمية للطلاب  
            //======================================
            var ListOfStudentsAsDistinct = dbSch.INTEGRATION_All_Students
                                                .Where(s => advanceRequests.Any(x => x.Student_Id == s.STUDENT_ID))
                                                .Select(s => new
                                                {
                                                    s.STUDENT_NAME,
                                                    s.NATIONAL_ID,
                                                    s.STUDENT_ID
                                                }).Distinct().ToList();

            var returnedMasterIds = new List<int>();
            ListOfStudentsAsDistinct.ForEach(studentItem =>
            {
                var LastDocNumber = dbSch.usp_GetLastDocNumber().FirstOrDefault().Value.ToString();

                var listOfAdvancePaymentDetails = new List<AdvancePaymentDetails>();
                var advancePaymentMaster = new AdvancePaymentMaster
                {
                    DocNotes = model.Notes,
                    User_Id = CurrentUser.ID,
                    COA_ID = model.AccountId,
                    DocNumber = LastDocNumber,
                    InsertionDate = DateTime.Now,
                    Student_Id = (int)studentItem.STUDENT_ID,
                };
                dbSch.AdvancePaymentMaster.Add(advancePaymentMaster);

                //======================================
                //لازم أعمل حفظ عشان رقم السند مش يتكرر
                //======================================
                dbSch.SaveChanges();

                returnedMasterIds.Add(advancePaymentMaster.ID);

                advanceRequests.Where(x => x.Student_Id == studentItem.STUDENT_ID).ToList()
                               .ForEach(advanceRequestItem =>
                               {
                                   listOfAdvancePaymentDetails.Add(
                                           new AdvancePaymentDetails
                                           {
                                               NetValue = (decimal)advanceRequestItem.ApprovedValue,
                                               AdvancePaymentMaster_Id = advancePaymentMaster.ID,
                                               AdvanceRequests_Id = advanceRequestItem.ID
                                           });
                               });
                advancePaymentMaster.TotalValue = listOfAdvancePaymentDetails.Sum(x => x.NetValue);

                dbSch.AdvancePaymentDetails.AddRange(listOfAdvancePaymentDetails);


                if (model.PostJournal)
                {
                    //=====================================
                    // نوع العملية هنا اللي هى قيد يومية 
                    //=====================================
                    var OperationId = GetOperationId().Value;
                    var BCDID = GetBcdId(OperationId).BCDID;


                    //=============================================================
                    // دا رقم العملية الأهم اللي هما بيعتبروها مش متكررة ع الإطلاق 
                    //=============================================================
                    var LastSystemNumber = GetLastSystemNumber(fsyid, OperationId);

                    var GJHDescription = new StringBuilder();
                    if (advanceRequests.FirstOrDefault().AdvanceSettings.AdvanceType == "A")
                    {
                        GJHDescription.Append($" صرف السلف لحساب الطالب ");
                    }
                    else
                    {
                        GJHDescription.Append($" صرف الإعانات لحساب الطالب ");
                    }

                    GJHDescription.Append($"{  studentItem.STUDENT_NAME + "-" + "ق.هـ" + " ( " + studentItem.NATIONAL_ID + " ) " + "-" + "ق.جـ" + " ( " + studentItem.STUDENT_ID + " ) " }");

                    GJH GJHmodel = new GJH();
                    GJHmodel.OPT_ID = BCDID;
                    GJHmodel.GJHStatus = true;
                    GJHmodel.USR_ID = UserInAcc.ID;
                    GJHmodel.GJHRefranceNumber = "";
                    GJHmodel.InsertDate = DateTime.Now;
                    GJHmodel.COM_ID = UserInAcc.School_ID;
                    GJHmodel.GJHSystemNumber = LastSystemNumber;
                    GJHmodel.GJHOperationDate = model.PaymentDate;
                    GJHmodel.GJHDescription = GJHDescription.ToString();
                    GJHmodel.GJHAmount = advancePaymentMaster.TotalValue;
                    GJHmodel.JournalNo = dbAcc.NextJournalEntryNo_Sp(UserInAcc.School_ID, fsyid).Single().Value;

                    dbAcc.GJH.Add(GJHmodel);
                    dbAcc.SaveChanges();

                    advancePaymentMaster.IsPosted = true;
                    advancePaymentMaster.GJH_ID = (int)GJHmodel.GJHID;

                    var _GJDDescription = new StringBuilder();
                    _GJDDescription.Append($" المصروف للطالب ");
                    _GJDDescription.Append($"{  studentItem.STUDENT_NAME + "-" + "ق.هـ" + " ( " + studentItem.NATIONAL_ID + " ) " + "-" + "ق.جـ" + " ( " + studentItem.STUDENT_ID + " ) " }");


                    listOfAdvancePaymentDetails.ForEach(AdvancePaymentDetailItem =>
                    {
                        var AdvanceName = AdvancePaymentDetailItem.AdvanceRequests.AdvanceSettings.AdvanceSettingName;
                        var AdvanceCoa = AdvancePaymentDetailItem.AdvanceRequests.AdvanceSettings.COAID.Value;

                        var GJDDescription = new StringBuilder();
                        GJDDescription.Append($" المصروف للطالب ");
                        GJDDescription.Append($"{  studentItem.STUDENT_NAME + "-" + "ق.هـ" + " ( " + studentItem.NATIONAL_ID + " ) " + "-" + "ق.جـ" + " ( " + studentItem.STUDENT_ID + " ) " }");

                        GJDDescription.Append($" من ");
                        _GJDDescription.Append($" من ");

                        GJDDescription.Append($"{AdvanceName}");
                        _GJDDescription.Append($"- {AdvanceName} ");

                        GJD GJDmodel = new GJD();
                        GJDmodel.JOB_ID = null;
                        GJDmodel.Fsy_ID = fsyid;
                        GJDmodel.GJDStatus = true;
                        GJDmodel.GJDCreditAmount = 0;
                        GJDmodel.GJH_ID = GJHmodel.GJHID;
                        GJDmodel.Com_ID = UserInAcc.School_ID;
                        GJDmodel.GJDDebitAmount = AdvancePaymentDetailItem.NetValue;
                        GJDmodel.GJDDescrition = GJDDescription.ToString();
                        GJDmodel.COA_ID = AdvanceCoa;

                        dbAcc.GJD.Add(GJDmodel);
                    });

                    GJD _GJDmodel = new GJD();
                    _GJDmodel.JOB_ID = null;
                    _GJDmodel.Fsy_ID = fsyid;
                    _GJDmodel.GJDStatus = true;
                    _GJDmodel.GJDDebitAmount = 0;
                    _GJDmodel.GJH_ID = GJHmodel.GJHID;
                    _GJDmodel.Com_ID = UserInAcc.School_ID;
                    _GJDmodel.COA_ID = advancePaymentMaster.COA_ID;
                    _GJDmodel.GJDDescrition = _GJDDescription.ToString();
                    _GJDmodel.GJDCreditAmount = advancePaymentMaster.TotalValue;
                    dbAcc.GJD.Add(_GJDmodel);

                    advancePaymentMaster.DocHeader = _GJDDescription.ToString();
                }
                else
                {
                    var Description = new StringBuilder();
                    Description.Append($" المصروف للطالب ");
                    Description.Append($"{  studentItem.STUDENT_NAME + "-" + "ق.هـ" + " ( " + studentItem.NATIONAL_ID + " ) " + "-" + "ق.جـ" + " ( " + studentItem.STUDENT_ID + " ) " }");

                    Description.Append($" من ");
                    Description.Append($" {string.Join(" و ", listOfAdvancePaymentDetails.Select(x => x.AdvanceRequests.AdvanceSettings.AdvanceSettingName).Distinct().ToList())}");
                    Description.Append($" بتاريخ {advancePaymentMaster.InsertionDate.ToShortDateString()}");
                    advancePaymentMaster.DocHeader = Description.ToString();
                }

            });

            var dbSchTask = dbSch.SaveChangesAsync();
            var dbAccTask = dbAcc.SaveChangesAsync();

            Task.WhenAll(dbSchTask, dbAccTask);

            return this.JsonMaxLength(_notify = new notify()
            {
                Message = string.Join(",", returnedMasterIds.Distinct().ToList()),
                Type = "success",
                status = 200
            });
        }


        public int? GetOperationId()
        {
            dbAcc.Configuration.LazyLoadingEnabled = false;
            var uspConfigSelectResult = dbAcc.usp_GetConfigValues("JournalEntryOperation").FirstOrDefault();//.FirstOrDefault(x => x.ConfigKey == "JournalEntryOperation");//CashReciveOperation

            if (uspConfigSelectResult == null)
            {
                return null;
            }

            return int.Parse(uspConfigSelectResult?.ConfigValue);
        }

        public BCD GetBcdId(int operationId)
        {
            dbAcc.Configuration.LazyLoadingEnabled = false;
            return dbAcc.BCD.FirstOrDefault(x => x.BCDID == operationId);
        }

        public string GetLastSystemNumber(int fsyId, int operationId)
        {
            var fsy = dbAcc.FSY.Single(x => x.FSYID == fsyId);
            var prefix = GetBcdId(operationId).Prefix;
            var sysNumbers = dbAcc.usp_GetLastGJHSystemNumber(fsyId, operationId, prefix).SingleOrDefault();
            var lastSysNumber = int.Parse(string.IsNullOrEmpty(sysNumbers) ? "0" : sysNumbers.Split('-')[1]);
            var gjhGjhSystemNumber = prefix + fsy.FsyPrefix + "-" + (lastSysNumber + 1);
            return gjhGjhSystemNumber;
        }

        #region Permissions       

        [HttpPost]
        public JsonResult GetPermissionsJson(int screenId)
        {
            return this.JsonMaxLength(GetPermissionsFn(screenId));
        }

        public Permissions GetPermissionsFn(int screenId)
        {
            if (CurrentUser.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

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

        public DashBoard_Users CurrentUser
        {
            get
            {
                var user = HttpContext.Session["UserId"] as DashBoard_Users;
                if (user != null && user.ID != 0)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpGet]
        public JsonResult GetAdvancePaymentPermissions(int screenId)
        {

            if (CurrentUser == null || CurrentUser.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new AdvancePaymentPermission();

            foreach (var permission in perm)
            {
                if (permission == "عرض")
                {
                    permissions.View = true;
                }
                else if (permission == "صرف الاعانة المحددة" || permission == "صرف السلف المحددة")
                {
                    permissions.ExchangeSelected = true;
                }

            }

            return this.JsonMaxLength(permissions);
        }


        public class AdvancePaymentPermission
        {
            public bool View { get; set; }
            public bool ExchangeSelected { get; set; }
            public bool CheckVoucher { get; set; }

        }


        #endregion

    }

    public class SaveAdvancesVM
    {
        public int AccountId { get; set; }
        public string AdvanceRequests { get; set; }
        public string Notes { get; set; }
        public bool PostJournal { get; set; }
        public DateTime PaymentDate { get; set; }
    }


}






#region Comments



//var Advance = dbSch.Advances.Select(x => x.AdvanceRequests_Id).ToList();

//var AdvanceRequests = dbSch.AdvanceRequests
//                                    .Where(x => (x.IsCanceled != true &&
//                                    x.RefusedbyID == null && x.ApprovedbyID != null
//                                    && x.AdvanceSettings.AdvanceType == Type)
//                                    && !Advance.Any(p => p == x.ID)
//                  )
//                .Select(x => new
//                {
//                    x.ID,
//                    STUDENT_NAME = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).STUDENT_NAME,
//                    AdvanceSettingName = x.AdvanceSettings.AdvanceSettingName,
//                    x.RequestedValue,
//                    x.ApprovedValue,
//                    InsertionDate = x.InsertionDate.Day + "/" + x.InsertionDate.Month + "/" + x.InsertionDate.Year,
//                    UserName = x.DashBoard_Users.Name,
//                    x.RequestNotes
//                }).ToList();

// var AdvanceRequestIds = string.IsNullOrEmpty(model.AdvanceRequests) ? new string[0] : model.AdvanceRequests.Split(',');

//for (int i = 0; i < AdvanceRequestIds.Length; i++)
//{
//    var CurrentRequest = dbSch.AdvanceRequests.Find(int.Parse(AdvanceRequestIds[i]));
//    dbSch.Advances.Add(new Advances
//    {
//        Student_Id = CurrentRequest.Student_Id,
//        ApprovedValue = (decimal)CurrentRequest.ApprovedValue,
//        COA_ID = model.AccountId,
//        AdvanceRequests_Id = CurrentRequest.ID,
//        Notes = model.Notes,
//        User_Id = user.ID,
//        InsertionDate = DateTime.Now
//    });
//}
//dbSch.SaveChanges();
// return Json(_notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);

#endregion