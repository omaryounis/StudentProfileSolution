using Newtonsoft.Json;
using StudentProfile.Components;
using StudentProfile.Components.Helpers;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static StudentProfile.Web.Controllers.AdvancePaymentController;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public partial class AdvancesController : Controller
    {
        private notify _notify;
        private readonly EsolERPEntities dbAcc;
        private readonly SchoolAccGam3aEntities dbSch;

        public AdvancesController()
        {
            dbAcc = new EsolERPEntities();
            dbSch = new SchoolAccGam3aEntities();
        }

        #region تهيئة حسابات السلف والإعانات

        #region Permissions

        [HttpGet]
        public JsonResult GetStudentAdvanceConfigPermissions(int screenId)
        {

            if (CurrentUser.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new StudentAdvanceConfigPermission();

            foreach (var permission in perm)
            {
                if (permission == "ايقاف وتفعيل البند")
                {
                    permissions.OnOffBand = true;
                }
                else if (permission == "عرض")
                {
                    permissions.Show = true;
                }
                else if (permission == "تعديل البند")
                {
                    permissions.EditBand = true;
                }
                else if (permission == "اضافة نوع سلفة")
                {
                    permissions.AddBandType = true;
                }
                else if (permission == "اضافة نوع اعانة")
                {
                    permissions.AddHelpType = true;
                }

            }

            return this.JsonMaxLength(permissions);
        }

        public class StudentAdvanceConfigPermission
        {
            public bool Show { get; set; }
            public bool OnOffBand { get; set; }
            public bool EditBand { get; set; }
            public bool AddBandType { get; set; }
            public bool AddHelpType { get; set; }
        }
        #endregion

        [HttpGet]
        public ActionResult StudentAdvanceConfig()
        {
            //var json = new JavaScriptSerializer().Serialize(GetStudentAdvanceConfigPermissions(79).Data);
            //var permissions = JsonConvert.DeserializeObject<StudentAdvanceConfigPermission>(json);

            //if (permissions.Show)
            //{
            //    return View();
            //}

            //return RedirectToAction("NoPermissions", "Security");

            var permissions = GetPermissionsFn(79);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read || permissions.View)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }

        [HttpGet]
        public ActionResult StudentAdvanceConfigSandok()
        {
            var json = new JavaScriptSerializer().Serialize(GetStudentAdvanceConfigPermissions(130).Data);
            var permissions = JsonConvert.DeserializeObject<StudentAdvanceConfigPermission>(json);

            if (permissions.Show)
            {
                return this.JsonMaxLength(true);
            }

            return this.JsonMaxLength(true);
        }
        [HttpGet]
        public ActionResult StudentAdvanceConfigPerm()
        {
            var json = new JavaScriptSerializer().Serialize(GetStudentAdvanceConfigPermissions(79).Data);
            var permissions = JsonConvert.DeserializeObject<StudentAdvanceConfigPermission>(json);
            if (permissions.Show)
            {
                return this.JsonMaxLength(new
                {
                    Show = permissions.Show,
                    OnOffBand = permissions.OnOffBand,
                    EditBand = permissions.EditBand,
                    AddBandType = permissions.AddBandType,
                    AddHelpType = permissions.AddHelpType
                });
            }
            return this.JsonMaxLength(new
            {
                Show = permissions.Show,
                OnOffBand = permissions.OnOffBand,
                EditBand = permissions.EditBand,
                AddBandType = permissions.AddBandType,
                AddHelpType = permissions.AddHelpType
            });
        }
        [HttpGet]
        public ActionResult GetAdvancesConfigDataSource()
        {
            return this.JsonMaxLength(dbSch.vw_Get_AdvanceSetting_DSource.ToList());
        }


        [HttpGet]
        public ActionResult GetChildAcctounts()
        {
            var UserInAcc = dbAcc.Users.FirstOrDefault(x => x.ID == CurrentUser.AccID);
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


        [HttpPost]
        public ActionResult AddNewSubsidyType(int? advanceSettingId, string subsidyTypeName,
                                              int subsidyCoaId, decimal maxRequestValue, bool IsStudentVisible)
        {
            if (string.IsNullOrEmpty(subsidyTypeName))
            {
                return Json("الاسم إجبارى", JsonRequestBehavior.AllowGet);
            }

            if (advanceSettingId == null)
            {
                if (dbSch.AdvanceSettings.Any(x => x.AdvanceSettingName == subsidyTypeName))
                {
                    return Json("هذا الإسم موجود من قبل", JsonRequestBehavior.AllowGet);
                }
                var advanceSetting = new AdvanceSettings
                {
                    IsActive = true,
                    ValueType = "V",
                    AdvanceType = "S",
                    COAID = subsidyCoaId,
                    MaxRequestValue = maxRequestValue,
                    IsStudentVisible = IsStudentVisible,
                    AdvanceSettingName = subsidyTypeName
                };
                dbSch.AdvanceSettings.Add(advanceSetting);
            }
            else
            {
                var model = dbSch.AdvanceSettings.FirstOrDefault(x => x.AdvanceSettingId == advanceSettingId);
                if (model == null)
                {
                    return Json("حدث خطأ برجاء تحديث بيانات الصفحة مرة أخري", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.ValueType = "V";
                    model.IsActive = true;
                    model.COAID = subsidyCoaId;
                    model.MaxRequestValue = maxRequestValue;
                    model.IsStudentVisible = IsStudentVisible;
                    model.AdvanceSettingName = subsidyTypeName;

                    dbSch.Entry(model).State = EntityState.Modified;
                }
            }
            dbSch.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddNewAdvanceType(int? advanceSettingId, string advanceTypeName, int advanceCoaId,
                                              decimal advanceValue, string advanceValueType, decimal maxRequestValue,
                                              int CoaId_RecieveFromPayroll, bool IsConditional, bool IsStudentVisible)
        {
            if (string.IsNullOrEmpty(advanceTypeName))
            {
                return Json("اسم السلفة إجبارى", JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(advanceValueType))
            {
                return Json("نوع القيمة إجبارى", JsonRequestBehavior.AllowGet);
            }

            if (advanceCoaId < 1)
            {
                return Json("حساب السلفة إجباري", JsonRequestBehavior.AllowGet);
            }

            if (CoaId_RecieveFromPayroll < 1)
            {
                return Json("حساب التحصيل من المسير إجباري", JsonRequestBehavior.AllowGet);
            }

            if (advanceSettingId == null)
            {
                if (dbSch.AdvanceSettings.Any(x => x.AdvanceSettingName == advanceTypeName))
                {
                    return Json("هذا الإسم موجود من قبل", JsonRequestBehavior.AllowGet);
                }
                var advanceSetting = new AdvanceSettings
                {
                    IsActive = true,
                    AdvanceType = "A",
                    COAID = advanceCoaId,
                    Value = advanceValue,
                    ValueType = advanceValueType,
                    IsConditional = IsConditional,
                    MaxRequestValue = maxRequestValue,
                    IsStudentVisible = IsStudentVisible,
                    AdvanceSettingName = advanceTypeName,
                    CoaId_RecieveFromPayroll = CoaId_RecieveFromPayroll

                };
                dbSch.AdvanceSettings.Add(advanceSetting);
            }
            else
            {
                var model = dbSch.AdvanceSettings.FirstOrDefault(x => x.AdvanceSettingId == advanceSettingId);
                if (model == null)
                {
                    return Json("حدث خطأ برجاء تحديث بيانات الصفحة مرة أخري", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.IsActive = true;
                    model.COAID = advanceCoaId;
                    model.Value = advanceValue;
                    model.ValueType = advanceValueType;
                    model.IsConditional = IsConditional;
                    model.MaxRequestValue = maxRequestValue;
                    model.IsStudentVisible = IsStudentVisible;
                    model.AdvanceSettingName = advanceTypeName;
                    model.CoaId_RecieveFromPayroll = CoaId_RecieveFromPayroll;

                    dbSch.Entry(model).State = EntityState.Modified;
                }
            }
            dbSch.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult EditAdvanceConfigStatus(int advanceConfigId, bool status)
        {
            var model = dbSch.AdvanceSettings.FirstOrDefault(x => x.AdvanceSettingId == advanceConfigId);
            if (model != null)
            {
                if (model.IsActive == status)
                {
                    return Content("");
                }
                model.IsActive = status;

                dbSch.Entry(model).State = System.Data.Entity.EntityState.Modified;
                dbSch.SaveChanges();

                return Content("");
            }
            return Json("حدث خطأ برجاء تحديث بيانات الصفحة مرة أخري", JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult GetAdvanceConfigById(int advanceConfigId)
        {
            dbSch.Configuration.LazyLoadingEnabled = false;
            return this.JsonMaxLength(dbSch.AdvanceSettings.FirstOrDefault(x => x.AdvanceSettingId == advanceConfigId));
        }

        [HttpGet]
        public ActionResult GetRewardItemTypeName()
        {
            var record = dbSch.RewardItems.FirstOrDefault(x => x.ID == 21);
            return this.JsonMaxLength(new { rewardItemName =record.RewardItemName_Arb,coaid=record.COAID ,
                                            coaidReceive = record.CoaId_RecieveFromPayroll });
        }

        [HttpPost]
        public ActionResult UpdateRewardItemType(int RewardItemCOAID,int RewardItemRecieveFromPayroll_COAID)
        {
            try
            {

                var ItemType = dbSch.RewardItems.FirstOrDefault(m => m.ID == 21);
                ItemType.COAID = RewardItemCOAID;
                ItemType.CoaId_RecieveFromPayroll = RewardItemRecieveFromPayroll_COAID;
                dbSch.Entry(ItemType).State = EntityState.Modified;
                dbSch.SaveChanges();
                return Json("", JsonRequestBehavior.AllowGet);
            }catch(Exception e)
            {
                return Json("حدث خطأ اثناء التعديل", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region شاشة تهيئة ضوابط عدم استحقاق السلف
        #region Permissions

        [HttpGet]
        public JsonResult GetStudentAdvanceSettingConfigPermissions(int screenId)
        {

            if (CurrentUser.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new Permissions();

            foreach (var permission in perm)
            {
                if (permission == "عرض")
                {
                    permissions.View = true;
                }
                else if (permission == "حفظ")
                {
                    permissions.Save = true;
                }

            }

            return this.JsonMaxLength(permissions);
        }


        #endregion

        [HttpGet]
        public ActionResult StudentAdvanceSettingConfig()
        {

            var json = new JavaScriptSerializer().Serialize(GetPermissionsJson(80).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        [HttpGet]
        public ActionResult GetAdvanceSettingConfig()
        {
            return this.JsonMaxLength(dbSch.AdvanceSettingConfig
                                           .Where(x => x.IsVisible == true).ToList()
                                           .Select(P => new
                                           {
                                               P.ID,
                                               P.IsVisible,
                                               P.KeyAr,
                                               P.Key,
                                               P.Type,
                                               Value = string.IsNullOrEmpty(P.Value) ? null : P.Value.Split(',').Select(o => Convert.ToString(o))
                                           })
                                     );

        }

        public ActionResult GetLevels()
        {
            return this.JsonMaxLength(dbSch.usp_getLevels()
                                           .ToList()
                                           .Select(x => new
                                           {
                                               LEVEL_CODE = x.LEVEL_CODE.Value.ToString(),
                                               LEVEL_DESC = x.LEVEL_DESC
                                           }).ToList()
                                    );
        }

        [HttpPost]
        public ActionResult UpdateAdvanceSettingConfig(AdvanceSettingConfig model)
        {
            var advanceSettingConfig = dbSch.AdvanceSettingConfig.FirstOrDefault(x => x.Key == model.Key);
            if (advanceSettingConfig != null)
            {
                if (model.Value == null)
                {
                    return Json("يجب ادخال قيمة مناسبة", JsonRequestBehavior.AllowGet);
                }

                advanceSettingConfig.Value = model.Value;
                dbSch.Entry(advanceSettingConfig).State = EntityState.Modified;

                dbSch.SaveChanges();

                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json("حدث خطأ اثناء التعديل", JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region طلب السلفة من ناحية الطالب

        [HttpGet]
        public ViewResult StudentAdvanceRequests()
        {
            // لا يوجد صلاحيات لشاشات الدخول من عند الطالب
            return View();
        }


        [HttpGet]
        public ActionResult DataSourceAdvanceRequestsGrid(string type)
        {
            var advanceRequests = dbSch.AdvanceRequests.Where(x => x.Student_Id == CurrentStudentId &&
                                                                   (x.IsCanceled == false || x.IsCanceled == null) &&
                                                                   x.AdvanceSettings.AdvanceType == type);

            return this.JsonMaxLength(advanceRequests.Select(P => new
            {
                P.Student_Id,
                P.StatusNotes,
                P.RequestNotes,
                P.ApprovedValue,
                P.RequestedValue,
                P.AdvanceSettings_Id,
                AdvanceRequestId = P.ID,
                AdvanceName = P.AdvanceSettings.AdvanceSettingName,
                Status = P.ApprovedbyID != null ? "معتمد" : P.RefusedbyID != null ? "مرفوض" : "تحت المراجعة",
                InsertionDate = P.InsertionDate.Day + "/" + P.InsertionDate.Month + "/" + P.InsertionDate.Year,
                AdvanceIsPaid = P.AdvancePaymentDetails.Count() > 0 ? "تم الصرف" : "لم يصرف بعد"

            }).ToList());
        }


        [HttpPost]
        public ActionResult SaveAdvanceRequestByStudent(int advanceTypeId, decimal advanceValue, string advanceRequestNotes)
        {
            if (CurrentStudentId <= 0)
            {
                return RedirectToAction("Login", "Login");
            }

            if (dbSch.AdvanceRequests.Any(x => x.Student_Id == CurrentStudentId && x.AdvanceSettings_Id == advanceTypeId && x.IsCanceled != true && x.ApprovedbyID == null && x.RefusedbyID == null))
            {
                return Content("عفوا لقد تم إدخال هذا الطلب من قبل وجاري مراجعته");
            }

            if (advanceValue <= 0)
            {
                return Content("لا يمكن أن يكون القيمة المطلوبة أقل من أو تساوي صفر");
            }

            var type = dbSch.AdvanceSettings.SingleOrDefault(x => x.AdvanceSettingId == advanceTypeId)?.AdvanceType;

            //if (type == "S")
            //{
                var file = (HttpPostedFileBase)Session["AdvanceRequestFile"];

                if (file == null || file.ContentLength == 0)
                {
                    return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع المرفق");
                }
            //  }
            if (type == "S")
            {
                if (dbSch.AdvanceRequests.Where(a => a.Student_Id == CurrentStudentId &&  a.RefusedbyID == null && a.ApprovedbyID != null).Count() > 0)
                {

                    var Query = dbSch.AdvancePaymentDetails.Where(x => x.AdvanceRequests.Student_Id == CurrentStudentId  && x.AdvanceRequests.RefusedbyID == null && x.AdvanceRequests.ApprovedbyID != null).Count();
                    if (Query == 0)
                    {

                        return Content("يوجد إعانه معتمده في إنتظار صرفها لذلك لن تتمكن من تقديم طلب أخر إلا بعد صرف هذا الطلب");
                    }
                }

            }
            if (type != "S")
            {
                if (dbSch.AdvanceRequests.Where(a => a.Student_Id == CurrentStudentId /* && a.AdvanceSettings_Id == advanceTypeId*/ && a.RefusedbyID == null && a.ApprovedbyID != null).Count() > 0)
                {

                    var Query = dbSch.AdvancePaymentDetails.Where(x => x.AdvanceRequests.Student_Id == CurrentStudentId /*&& x.AdvanceRequests.AdvanceSettings_Id == advanceTypeId*/ && x.AdvanceRequests.RefusedbyID == null && x.AdvanceRequests.ApprovedbyID != null).Count();
                    if (Query == 0)
                    {

                        return Content("يوجد سلفة معتمده في إنتظار صرفها لذلك لن تتمكن من تقديم طلب أخر إلا بعد صرف هذا الطلب");
                    }
                }

                //var Query = dbSch.AdvanceRequests.Where(x => x.Student_Id == CurrentStudentId && x.AdvanceSettings_Id == advanceTypeId).sele

                //var studentAdvancePaymentDetails = dbSch.AdvancePaymentMaster.Where(x => x.Student_Id == CurrentStudentI).
                //                                  SelectMany(x => x.AdvancePaymentDetails).ToList();

                //==============================================
                //التاكد  من ان الطالب ليس علية سلف غير مسددة
                //==============================================


                //تفاصيل طلب السلفة            
                var studentAdvancePaymentDetails = dbSch.AdvancePaymentMaster.Where(x => x.Student_Id == CurrentStudentId).
                                                   SelectMany(x => x.AdvancePaymentDetails).ToList();

              // var Query=dbSch.AdvanceRequests.Where(x=>x.Student_Id == CurrentStudentId && x.AdvanceSettings_Id == advanceTypeId && )

                // مجموع  قيمةالسلف المصروفة للطالب
                var TotalAdvancePayment = studentAdvancePaymentDetails.Where(x=>x.AdvanceRequests.AdvanceSettings.AdvanceType =="A").Sum(p => p.NetValue);

                // مجموع  قيمةالسلف التي سددها الطالب للطالب
                var TotalAdvanceReceive = dbSch.AdvanceReceiveDetails.AsEnumerable().Where(x => studentAdvancePaymentDetails.Any(p => p.ID == x.AdvancePaymentDetails_Id)).Sum(x => x.NetValue);

                if ((TotalAdvancePayment - TotalAdvanceReceive) != 0)
                {
                    return Content("لا يمكن إتمام عملية الحفظ حتي يسدد الطالب جميع السلف السابقة");
                }

                //===================================
                //التاكد من ان الطالب يستحق مكافئات
                //===================================
                var studentRewards = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == CurrentStudentId);
                if (studentRewards != null)
                {
                    if (studentRewards.CATEGORY_CODE == null || studentRewards.CATEGORY_CODE < 0)
                    {
                        return Content("لا يمكن إتمام عملية الحفظ لان الطالب لا يستحق مكافئات");
                    }
                }
            }

            var advanceRequests = new AdvanceRequests
            {
                AdvanceSettings_Id = advanceTypeId,
                InsertionDate = DateTime.Now,
                Student_Id = CurrentStudentId,
                RequestNotes = advanceRequestNotes,
                RequestedValue = advanceValue,
                ApprovedValue = null
            };
            dbSch.AdvanceRequests.Add(advanceRequests);
            dbSch.SaveChanges();

            //if (type == "S")
            //{
                //var file = (HttpPostedFileBase)Session["AdvanceRequestFile"];

                // file
                var rootFolder = Server.MapPath("~/Content/UserFiles/");
                var studentFolder = Path.Combine(rootFolder, CurrentStudentId.ToString());
                var currentFolder = new DirectoryInfo(studentFolder);

                if (currentFolder.Exists == false)
                {
                    Directory.CreateDirectory(studentFolder);
                }

                var dSecurity = currentFolder.GetAccessControl();


                dSecurity.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                    PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                currentFolder.SetAccessControl(dSecurity);


                var stAdvancesFolder = Path.Combine(rootFolder, CurrentStudentId.ToString(), "Advances");

                if (!new DirectoryInfo(stAdvancesFolder).Exists)
                {
                    Directory.CreateDirectory(stAdvancesFolder);
                }


                var dSecurityForAdvances = new DirectoryInfo(stAdvancesFolder).GetAccessControl();

                dSecurityForAdvances.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                    PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                new DirectoryInfo(stAdvancesFolder).SetAccessControl(dSecurityForAdvances);


                // Start From Here

                string filename = String.Format("{0}_{1}.{2}", file.FileName.Split('.')[0],
                    DateTime.Now.ToString("dd-MM-yyyy-HH-mm"), file.FileName.Split('.')[1]);

                string path = Path.Combine(stAdvancesFolder, filename);


                Stream stream = file.InputStream;
                byte[] bytes = ReadToEnd(stream);
                FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();


                var advanceRequestsAttachment = new AdvanceRequestsAttachment
                {
                    FileName = filename,
                    Path = MapPathReverse(path),
                    InsertDate = DateTime.Now,
                    UserId = CurrentUser.ID,
                    AdvanceRequests_Id = advanceRequests.ID
                };
                dbSch.AdvanceRequestsAttachment.Add(advanceRequestsAttachment);
                dbSch.SaveChanges();

                Session["AdvanceRequestFile"] = null;
           // }
            return Content("");
        }


        [HttpGet]
        public ActionResult GetAdvancesTypes(string type)
        {
            var advancesTypes = dbSch.AdvanceSettings.Where(x => x.AdvanceType == type && x.IsActive == true);

            advancesTypes = CurrentUser.IsStudent == true ? advancesTypes.Where(x => x.IsStudentVisible == true) : advancesTypes;

            return this.JsonMaxLength(advancesTypes.Select(x => new
            {
                Value = SqlFunctions.StringConvert((double)x.AdvanceSettingId).Trim(),
                Text = x.AdvanceSettingName,
                x.MaxRequestValue,
                x.IsConditional
            }).ToList());
        }

        [HttpGet]
        public ActionResult GetAdvanceRemainAmount(int advanceRequestId)
        {
            var advanceRequest = dbSch.AdvanceRequests.SingleOrDefault(x => x.ID == advanceRequestId);
            if (advanceRequest == null)
            {
                return this.JsonMaxLength("");
            }

            var advanceReceiveAmount = dbSch.AdvanceReceiveDetails.Where(x => x.AdvancePaymentDetails.AdvanceRequests_Id == advanceRequestId)
                                                                  .Select(x => x.NetValue)
                                                                  .DefaultIfEmpty(0).Sum();

            return this.JsonMaxLength(new[]
            {
               new {
                    AdvanceRequestId = advanceRequestId,
                    AdvanceApprovedValue = advanceRequest.ApprovedValue,
                    AdvanceRequestValue = advanceRequest.RequestedValue,
                    PaidAmount = advanceReceiveAmount,
                    RemainAmount = (advanceRequest.ApprovedValue.GetValueOrDefault(0) - advanceReceiveAmount) < 0 ? 0 : advanceRequest.ApprovedValue - advanceReceiveAmount
                   }
            });
        }

        [HttpGet]
        public ActionResult GetAdvancePaidAmount(int advanceRequestId)
        {
            return this.JsonMaxLength(dbSch.AdvanceReceiveDetails
                                           .Where(x => x.AdvancePaymentDetails.AdvanceRequests_Id == advanceRequestId)
                                           .Select(x => new
                                           {
                                               PaidAmount = x.NetValue,
                                               AdvanceRequestId = advanceRequestId,
                                               PayRollNumber = x.AdvanceReceiveMaster.PayrollId != null ? x.AdvanceReceiveMaster.Payroll.PayrollNumber.ToString() : x.AdvanceReceiveMaster.DocNumber,
                                               PayrollDate = x.AdvanceReceiveMaster.InsertionDate.Day + "/" + x.AdvanceReceiveMaster.InsertionDate.Month + "/" + x.AdvanceReceiveMaster.InsertionDate.Year,
                                               PaymentType = x.AdvanceReceiveMaster.PayrollId != null ? "الخصم من مسير المكافأت" : "السداد النقدي"
                                           }).ToList());

        }

        [HttpDelete]
        public ActionResult DeleteAdvanceRequest(int advanceRequestId)
        {
            var model = dbSch.AdvanceRequests.Where(x => x.ID == advanceRequestId).FirstOrDefault();
            if (model == null)
            {
                return Json("لايمكن إتمام عملية الحذف", JsonRequestBehavior.AllowGet);
            }
            if (dbSch.AdvanceApprovedPhases.Any(x => x.AdvanceRequested_ID == model.ID))
            {
                return Json("لايمكن إتمام عملية الحذف حيث أن هذا الطلب دخل ضمن مراحل الإعتماد", JsonRequestBehavior.AllowGet);
            }

            var Attachments = dbSch.AdvanceRequestsAttachment.Where(x => x.AdvanceRequests_Id == model.ID);
            if (Attachments != null)
            {
                dbSch.AdvanceRequestsAttachment.RemoveRange(Attachments);
                dbSch.SaveChanges();
            }
            model.IsCanceled = true;
            dbSch.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbSch.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAdvancesTotalRemainForStudent()
        {
            if (CurrentStudentId == 0)
            {
                return RedirectToAction("Login", "Login");
            }
            var totalPayment = dbSch.AdvancePaymentMaster
                                    .Where(x => x.Student_Id == CurrentStudentId &&
                                                x.AdvancePaymentDetails.Any(c => c.AdvanceRequests.AdvanceSettings.AdvanceType == "A"))
                                    .Select(p => p.TotalValue)
                                    .DefaultIfEmpty(0).Sum();

            var totalReceive = dbSch.AdvanceReceiveMaster
                                    .Where(x => x.Student_Id == CurrentStudentId)
                                    .Select(p => p.TotalValue)
                                    .DefaultIfEmpty(0).Sum();

            return this.JsonMaxLength(new { TotalRemain = (totalPayment - totalReceive) <= 0 ? 0 : (totalPayment - totalReceive) });
        }

        #endregion

        #region طلبات الإعانة من ناحية الطالب
        [HttpGet]
        public ActionResult StudentSubsidyRequests()
        {
            // لا يوجد صلاحيات لشاشات الدخول من عند الطالب
            return View();
        }

        #region FileUploader

        [HttpPost]
        public ActionResult UploadFiles()
        {
            Session["AdvanceRequestFile"] = null;
            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentType != "application/pdf")
                {
                    return new HttpStatusCodeResult(400);
                }
                else
                {
                    Session["AdvanceRequestFile"] = file;
                }
            }
            return Json(0);
        }

        public string MapPathReverse(string fullServerPath)
        {
            return @"~\" + fullServerPath.Replace(Request.PhysicalApplicationPath, string.Empty);
        }

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

        [HttpGet]
        //[CryptoValueProvider]
        public ActionResult DownloadAdvanceAttachment(int advanceRequestId)
        {
            string FileVirtualPath = null;
            var dbSch = new SchoolAccGam3aEntities();
            if (CurrentUser.IsStudent)
            {
                int studentID = Convert.ToInt32(CurrentUser.Username);
                FileVirtualPath = dbSch.AdvanceRequestsAttachment.Where(x => x.AdvanceRequests_Id == advanceRequestId
                && x.AdvanceRequests.Student_Id == studentID).FirstOrDefault()?.Path;
            }
            else
            {
                var addAdvSubPermission = GetPermissionsFn(84).Read;
                var monitorAdvSubPermission = GetPermissionsFn(104).Read;
                var ApproveAdvPermission = GetPermissionsFn(82).Read;
                var approveSubPermission = GetPermissionsFn(83).Read;
                if (addAdvSubPermission || monitorAdvSubPermission || ApproveAdvPermission || approveSubPermission)
                {
                    FileVirtualPath = dbSch.AdvanceRequestsAttachment.Where(x => x.AdvanceRequests_Id == advanceRequestId).FirstOrDefault()?.Path;
                }
            }
            if (FileVirtualPath != null)
            {
                if (!System.IO.File.Exists(HostingEnvironment.MapPath(FileVirtualPath)))
                {
                    return Content("");
                }

                byte[] FileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(FileVirtualPath));
                return File(FileBytes, "application/pdf");

                //===============================================================================================
                // ** File As DownLoad ** ...
                // return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
                //===============================================================================================
            }
            return Content("");
        }

        #endregion




        #endregion

        #region تهيئة مراحل إعتماد طلبات السلف والإعانات 
        #region Permissions

        public class StudentAdvancePhasesPermission
        {
            public bool View { get; set; }
            public bool OnOffPhase { get; set; }
            public bool EditPhase { get; set; }
            public bool AddNewPhase { get; set; }
            public bool ReorderPhases { get; set; }
            public bool AssignUsers { get; set; }
        }

        [HttpGet]
        public JsonResult GetStudentAdvancePhasesPermissions(int screenId)
        {

            if (CurrentUser.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new StudentAdvancePhasesPermission();

            foreach (var permission in perm)
            {
                if (permission == " ايقاف وتغعيل المرحلة")
                {
                    permissions.OnOffPhase = true;
                }
                else if (permission == "عرض")
                {
                    permissions.View = true;
                }
                else if (permission == " تعديل المرحلة")
                {
                    permissions.EditPhase = true;
                }
                else if (permission == " اضافة مرحلة جديدة")
                {
                    permissions.AddNewPhase = true;
                }
                else if (permission == "تعديل ترتيب المراحل")
                {
                    permissions.ReorderPhases = true;
                }
                else if (permission == "اسناد المستخدمين")
                {
                    permissions.AssignUsers = true;
                }

            }

            return this.JsonMaxLength(permissions);
        }

        #endregion

        [HttpGet]
        public ActionResult StudentAdvancePhases()
        {
            var json = new JavaScriptSerializer().Serialize(GetStudentAdvancePhasesPermissions(81).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        [HttpGet]
        public ActionResult GetAllUsers()
        {
            //=======================================================================================================
            //  عرض المستخدمين الموجود ليهم صلاحية علي شاشة متابعة طلبات السلف أوالإعانات فقط وليس كل المستخدمين
            //=======================================================================================================

            return this.JsonMaxLength(dbSch.Usp_GetAllUsers_ToDecisionTakingForAdvances().ToList());

        }


        [HttpGet]
        public async Task<ActionResult> GetAllAdvancePhasesDDL()
        {
            return this.JsonMaxLength(await dbSch.AdvancePhases
                                                .Select(x => new
                                                {
                                                    ID = x.ID,
                                                    PhaseName = x.PhaseName,
                                                    Order = "المرحلة رقم" + " : " + x.Order
                                                }).ToListAsync());
        }


        [HttpGet]
        public async Task<ActionResult> GetAllAdvancePhasesGridSource()
        {
            var advanceUsers = await dbSch.AdvanceUsers.Include(x => x.DashBoard_Users)
                                                       .Include(x => x.AdvancePhases).ToListAsync();

            return this.JsonMaxLength(advanceUsers.Select(x => new
            {
                AdvanceUserId = x.ID,
                AdvancePhases_Id = x.AdvancePhase_ID,
                PhaseName = x.AdvancePhases.PhaseName,
                Order = x.AdvancePhases.Order,
                UserName = x.DashBoard_Users.Name,
                IsAdmin = x.DashBoard_Users.IsAdmin != true ? false : true,
                UserStatus = x.IsActive != true ? false : true
            }).ToList());
        }

        [HttpGet]
        public async Task<ActionResult> GetAdvanceUser(int advanceUserId)
        {
            return this.JsonMaxLength(await dbSch.AdvanceUsers.Where(x => x.ID == advanceUserId && x.IsActive == true)
                                                    .Select(x => new
                                                    {
                                                        AdvancePhases_Id = x.AdvancePhase_ID,
                                                        UserID = x.UserID
                                                    }).FirstOrDefaultAsync());
        }


        [HttpGet]
        public async Task<ActionResult> GetAdvancePhaseById(int advancePhaseId)
        {
            return this.JsonMaxLength(await dbSch.AdvancePhases
                                                 .Where(x => x.ID == advancePhaseId)
                                                 .Select(x => new
                                                 {
                                                     x.ID,
                                                     x.Order,
                                                     x.PhaseName,
                                                     x.IsFinancialPhase
                                                 }).FirstOrDefaultAsync());
        }

        [HttpPost]
        public ActionResult SaveAdvancePhase(int? AdvancePhaseId, string AdvancePhaseName, byte phaseOrder, bool isFinancialPhase)
        {
            try
            {
                if (string.IsNullOrEmpty(AdvancePhaseName))
                {
                    return Content("لا يمكن إتمام عملية الحفظ حتي يتم إدخال إسم المرحلة");
                }

                if (phaseOrder < 1)
                {
                    return Content("لا يمكن إتمام عملية الحفظ حتي يتم إدخال ترتيب المرحلة");
                }

                if (dbSch.AdvancePhases.Where(x => x.PhaseName == AdvancePhaseName && x.ID != AdvancePhaseId).FirstOrDefault() != null)
                {
                    return Content("لا يمكن إتمام عملية الحفظ لوجود مرحلة بنفس الإسم");
                }

                var AdvancePhaseInDB = dbSch.AdvancePhases.Where(x => x.ID == AdvancePhaseId).FirstOrDefault();
                if (AdvancePhaseInDB != null)
                {
                    var financialPhase = dbSch.AdvancePhases.Where(x => x.IsFinancialPhase == true && x.ID != AdvancePhaseId).FirstOrDefault();
                    if (financialPhase != null)
                    {
                        if (isFinancialPhase == true)
                        {
                            return Content($"لايمكن إتمام عملية الحفظ حيث أن مرحلة {financialPhase.PhaseName} هي مرحلة إعتماد مالي");
                        }
                    }

                    //===============================================
                    //هل المستخدم دا عدل ترتيب مرحلة الإعتماد ولا لأه
                    //===============================================
                    var SecondAdvancePhaseInDB = dbSch.AdvancePhases.Where(x => x.Order == phaseOrder).FirstOrDefault();
                    if (SecondAdvancePhaseInDB == null)
                    {
                        return Content("لا يمكن إتمام عملية الحفظ لعدم وجود مرحلة بنفس الترتيب حتي تتم عملية التبديل بينهم");
                    }

                    if (AdvancePhaseInDB.ID != SecondAdvancePhaseInDB.ID)
                    {
                        //====================================
                        // عملية التبديل بين مرحلتي الإعتماد
                        //====================================
                        SecondAdvancePhaseInDB.Order = AdvancePhaseInDB.Order;
                        dbSch.Entry(SecondAdvancePhaseInDB).State = System.Data.Entity.EntityState.Modified;


                        AdvancePhaseInDB.Order = phaseOrder;
                    }

                    AdvancePhaseInDB.PhaseName = AdvancePhaseName;
                    AdvancePhaseInDB.IsFinancialPhase = isFinancialPhase;
                    dbSch.Entry(AdvancePhaseInDB).State = System.Data.Entity.EntityState.Modified;
                    dbSch.SaveChanges();
                    return Content("");
                }
                else
                {

                    if (dbSch.AdvancePhases.Where(x => x.IsFinancialPhase == true && x.ID != AdvancePhaseId).FirstOrDefault() != null)
                    {
                        if (isFinancialPhase == true)
                        {
                            return Content("لا يمكن إتمام عملية الحفظ لوجود مرحلة إعتماد مالي بالفعل");
                        }
                    }

                    if (dbSch.AdvancePhases.Where(x => x.Order == (phaseOrder - 1)).FirstOrDefault() != null || phaseOrder == 1)
                    {

                        var AdvancePhases = new AdvancePhases()
                        {
                            Order = phaseOrder,
                            PhaseName = AdvancePhaseName,
                            IsFinancialPhase = isFinancialPhase
                        };
                        dbSch.AdvancePhases.Add(AdvancePhases);
                        dbSch.SaveChanges();

                        return Content("");
                    }
                    return Content("لا يمكن إتمام عملية الحفظ لتخطى أرقام ترتيب المراحل");
                }
            }
            catch (Exception ex)
            {
                return Content("حدث خطأ أثناء الحفظ " + " " + ex.Message);
            }
        }


        [HttpPost]
        public ActionResult SaveAdvanceUser(int? AdvanceUserId, int AdvancePhaseId, int userId)
        {

            var AdvanceUserInDB = dbSch.AdvanceUsers.Where(x => x.ID == AdvanceUserId).FirstOrDefault();
            if (AdvanceUserInDB != null)
            {
                if (dbSch.AdvanceUsers.Where(x => x.AdvancePhase_ID == AdvancePhaseId && x.UserID == userId && x.ID != AdvanceUserId).FirstOrDefault() != null)
                {
                    return Content("لا يمكن إتمام عملية الحفظ نظرا لان هذا المسؤول مضاف علي هذه المرحلة بالفعل من قبل");
                }

                AdvanceUserInDB.UserID = userId;
                AdvanceUserInDB.AdvancePhase_ID = AdvancePhaseId;
                dbSch.Entry(AdvanceUserInDB).State = System.Data.Entity.EntityState.Modified;
                dbSch.SaveChanges();

                return Content("");
            }
            else
            {
                if (dbSch.AdvanceUsers.Where(x => x.AdvancePhase_ID == AdvancePhaseId && x.UserID == userId).FirstOrDefault() != null)
                {
                    return Content("لا يمكن إتمام عملية الحفظ نظرا لان هذا المسؤول مضاف علي هذه المرحلة بالفعل من قبل");
                }

                var AdvanceUsers = new AdvanceUsers()
                {

                    UserID = userId,
                    IsActive = true,
                    AdvancePhase_ID = AdvancePhaseId
                };
                dbSch.AdvanceUsers.Add(AdvanceUsers);
                dbSch.SaveChanges();

                return Content("");
            }
        }

        [HttpPost]
        public ActionResult EditadvanceUserStatus(int advanceUserId, bool status)
        {
            var db = new SchoolAccGam3aEntities();

            var advanceUsersInDB = db.AdvanceUsers.Where(x => x.ID == advanceUserId).FirstOrDefault();
            if (advanceUsersInDB != null)
            {
                if (advanceUsersInDB.IsActive == status)
                {
                    return Content("");
                }
                advanceUsersInDB.IsActive = status;
                db.Entry(advanceUsersInDB).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Content("");
            }
            return Content("لا يمكن تعديل حالة المسؤول نظرا لإرتباطه بسجلات أخري");

        }
        #endregion

        #region شاشة إعتماد طلبات السلف

        #region Permissions

        public class AdvanceDecisionTakingPermission
        {
            public bool View { get; set; }
            public bool Check { get; set; }
        }

        [HttpGet]
        public JsonResult GetAdvanceDecisionTakingPermissions(int screenId)
        {

            if (CurrentUser.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new AdvanceDecisionTakingPermission();

            foreach (var permission in perm)
            {
                if (permission == "معاينة")
                {
                    permissions.Check = true;
                }
                else if (permission == "عرض")
                {
                    permissions.View = true;
                }

            }

            return this.JsonMaxLength(permissions);
        }

        #endregion


        [HttpGet]
        public ActionResult AdvanceDecisionTaking()
        {
            var json = new JavaScriptSerializer().Serialize(GetAdvanceDecisionTakingPermissions(82).Data);
            var permissions = JsonConvert.DeserializeObject<AdvanceDecisionTakingPermission>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }


        [HttpGet]
        public ActionResult GetAdvanceRequestsByUserId(string type)
        {

            dbSch.Configuration.LazyLoadingEnabled = false;
            // مراحل الموافقة التي يقع فيها هذا المستخدم
            var userAuthorizedPhases = dbSch.AdvanceUsers.Where(x => x.UserID == CurrentUser.ID && x.IsActive == true).Select(x => x.AdvancePhases.Order);

            var advanceRequests = dbSch.AdvanceRequests.Include(x => x.AdvanceSettings)
                                                       .Include(x => x.AdvanceApprovedPhases)
                                                       .Include(x => x.DashBoard_Users)
                                                       .Where(x =>
                                                                 ((x.IsCanceled == false || x.IsCanceled == null) &&
                                                                  x.RefusedbyID == null &&
                                                                  x.StatusNotes == null &&
                                                                  x.ApprovedbyID == null &&
                                                                  x.AdvanceSettings.AdvanceType == type) &&
                                                                  userAuthorizedPhases.Any(p => p == x.AdvanceApprovedPhases.Count() + 1)
                                                              );

            var students = dbSch.INTEGRATION_All_Students.Where(x => advanceRequests.Any(p => p.Student_Id == x.STUDENT_ID));

            var AdvanceRequests = advanceRequests.ToList();

            var studentAcademicData = students.Select(x => new
            {
                x.STUDENT_NAME,
                x.NATIONAL_ID,
                x.STUDENT_ID,
                x.NATIONALITY_DESC,
                x.FACULTY_NAME,
                x.DEGREE_DESC,
                LEVEL_DESC = x.LEVEL_DESC == null || x.LEVEL_DESC == "" ? "غير متوفر" : x.LEVEL_DESC,
                x.STUDY_DESC,
                x.STATUS_DESC,
            }).ToList();

            var data = AdvanceRequests.Select(t => new
            {
                AdvanceRequestId = t.ID,
                AdvanceName = t.AdvanceSettings.AdvanceSettingName,
                RequestedDate = t.InsertionDate.Hour + ":" + t.InsertionDate.Minute + " " + t.InsertionDate.Day + "/" + t.InsertionDate.Month + "/" + t.InsertionDate.Year,
                AdvanceSettingId = t.AdvanceSettings.AdvanceSettingId,
                t.AdvanceSettings.AdvanceType,
                t.RequestedValue,
                StudentAcademicData = studentAcademicData.FirstOrDefault(x => x.STUDENT_ID == t.Student_Id),
                studentId = t.Student_Id,
                IsExcluded = ExcludeAdvanceOrdersForStudent(t.Student_Id),
                IsApprovedPhase = t.AdvanceApprovedPhases.Count() > 0 ? true : false,
                CreatedBy = t.User_Id == null ? "الطالب" : t.DashBoard_Users.Name,
                RequestNotes = t.RequestNotes,
                ID=t.ID
            }).ToList();

            return this.JsonMaxLength(data);
        }

        public ActionResult ExcludeAdvanceOrdersForStudentResons(decimal studentId, int AdvanceId)
        {
            if (studentId != 0)
            {
                if (dbSch.AdvanceSettings.Find(AdvanceId).IsConditional == false)
                {
                    //تفاصيل طلب السلفة            
                    var studentAdvancePaymentDetails = dbSch.AdvancePaymentMaster.Where(x => x.Student_Id == studentId).SelectMany(x => x.AdvancePaymentDetails).Where(x=>x.AdvanceRequests.AdvanceSettings.AdvanceType == "A").ToList();
                    // مجموع  قيمةالسلف المصروفة للطالب
                    var TotalAdvancePayment = studentAdvancePaymentDetails.Sum(p => p.NetValue);
                    // مجموع  قيمةالسلف التي سددها الطالب للطالب
                    var TotalAdvanceReceive = dbSch.AdvanceReceiveDetails.AsEnumerable().Where(x => studentAdvancePaymentDetails.Any(p => p.ID == x.AdvancePaymentDetails_Id)).Sum(x => x.NetValue);
                    if ((TotalAdvancePayment - TotalAdvanceReceive) != 0)
                    {
                        return Json(Tuple.Create(true, $"* يوجد رصيد سابق لم يسدد بقيمة {(TotalAdvancePayment - TotalAdvanceReceive)}"), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(ExcludeAdvanceOrdersForStudent(studentId), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(Tuple.Create(false, ""), JsonRequestBehavior.AllowGet);
        }

        private Tuple<bool, string> ExcludeAdvanceOrdersForStudent(decimal studentId)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"");

            //==========================================
            //التاكد ان الطالب ليس علية سلف غير مسددة
            //==========================================

            //تفاصيل طلب السلفة            
            var studentAdvancePaymentDetails = dbSch.AdvancePaymentMaster.Where(x => x.Student_Id == studentId).SelectMany(x => x.AdvancePaymentDetails).Where(x => x.AdvanceRequests.AdvanceSettings.AdvanceType == "A").ToList();

            // مجموع  قيمةالسلف المصروفة للطالب
            var TotalAdvancePayment = studentAdvancePaymentDetails.Sum(p => p.NetValue);

            // مجموع  قيمةالسلف التي سددها الطالب للطالب
            var TotalAdvanceReceive = dbSch.AdvanceReceiveDetails.AsEnumerable().Where(x => studentAdvancePaymentDetails.Any(p => p.ID == x.AdvancePaymentDetails_Id)).Sum(x => x.NetValue);

            if ((TotalAdvancePayment - TotalAdvanceReceive) != 0)
            {
                builder.Append($"* يوجد رصيد سابق لم يسدد بقيمة {(TotalAdvancePayment - TotalAdvanceReceive)}");
            }

            //المعدل التراكمي للطالب
            var studentCumGpa = dbSch.ST_AcademicData.Where(x => x.STUDENT_ID == studentId && x.SEMESTER_GPA != 0 && x.SEMESTER_GPA != null).OrderByDescending(x => x.SEMESTER).FirstOrDefault();
            if (studentCumGpa != null)
            {
                if (studentCumGpa.CUM_GPA != null)
                {
                    //قيمة المعدل التراكمي من ال config
                    var MiniGPA = dbSch.AdvanceSettingConfig.FirstOrDefault(x => x.Key == "MiniGPA").Value;
                    // التاكد من المجموع التراكمي اقل من القيمة المدخلة في التهيئة
                    if (studentCumGpa.CUM_GPA < double.Parse(MiniGPA))
                    {
                        builder.Append(Environment.NewLine + $"* المجموع التراكمي للطالب {studentCumGpa.CUM_GPA} وهو اقل من المسموح");
                    }
                }
                else
                {
                    builder.Append(Environment.NewLine + $"* المجموع التراكمي للطالب {studentCumGpa.CUM_GPA} وهو اقل من المسموح");
                }
            }


            //بيانات الطالب
            var studentData = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentId);
            if (studentData != null)
            {
                //المستويات المستثناة من ال config
                var Levels = dbSch.AdvanceSettingConfig.FirstOrDefault(x => x.Key == "Levels").Value.Split(',');

                // التاكد من الطالب لا يقع في المستويات المستثناة
                if (Levels.Any(p => p == studentData.LEVEL_CODE.ToString()))
                {
                    builder.Append(Environment.NewLine + $" * مستوي الطالب {studentData.LEVEL_CODE.ToString()} وهو يقع ضمن شروط عدم استحقاق السلفة");
                }
            }

            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                return Tuple.Create(true, builder.ToString());
            }

            return Tuple.Create(false, "");
        }

        [HttpDelete]
        public ActionResult DeleteAdvanceRequests(int Id)
        {
            if (Id > 0)
            {
                if (dbSch.AdvanceApprovedPhases.Any(x => x.AdvanceRequested_ID == Id))
                {
                    return Json(_notify = new notify() { Message = "لايمكن إتمام عملية الحذف حيث أن هذا الطلب دخل ضمن مراحل الإعتماد", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }

                //Remove Requests Attachment
                dbSch.AdvanceRequestsAttachment.RemoveRange(dbSch.AdvanceRequestsAttachment.Where(x => x.AdvanceRequests_Id == Id));
                try
                {
                    dbSch.SaveChanges();
                    //Remove Advance Requests
                    dbSch.AdvanceRequests.Remove(dbSch.AdvanceRequests.Find(Id));
                    dbSch.SaveChanges();
                }
                catch (Exception)
                {
                    return Json(_notify = new notify() { Message = "لايمكن الحذف للارتباط بعمليات اخري", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
                return Json(_notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(_notify = new notify() { Message = "خطأ اثناء الحذف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> StudentAdvancesDetailDataSource(int advanceRequestId)
        {
            // طلب السلفةالمراد معرفة تفاصيله
            var advanceRequest = dbSch.AdvanceRequests
                                      .Include(x => x.AdvanceSettings)
                                      .FirstOrDefault(x =>
                                                            ((x.IsCanceled == false || x.IsCanceled == null) &&
                                                            x.RefusedbyID == null &&
                                                            x.StatusNotes == null &&
                                                            x.ApprovedbyID == null) &&
                                                            x.ID == advanceRequestId);

            // المعدل التراكمي لدرجات الطالب
            var dbSchInstance1 = new SchoolAccGam3aEntities();
            var CumGpa = dbSchInstance1.ST_AcademicData.Where(x => x.STUDENT_ID == advanceRequest.Student_Id && x.SEMESTER_GPA != 0 && x.SEMESTER_GPA != null).OrderByDescending(x => x.SEMESTER).FirstOrDefault();
            //var studentAverageDegree = dbSchInstance1.ST_AcademicData
            //                                         .Where(x => advanceRequest.Student_Id == x.student_id)
            //                                         .AverageAsync(p => p.semester_gpa);


            //طلبات السلفة التي تم إعتمادها وتخص هذا الطالب    
            var dbSchInstance2 = new SchoolAccGam3aEntities();
            var advancesApprovedRequests = dbSchInstance2.AdvancePaymentDetails
                                                         .Where(x => x.AdvancePaymentMaster.Student_Id == advanceRequest.Student_Id)
                                                         .Select(x => new
                                                         {
                                                             x.NetValue,
                                                             ApprovedDate = x.AdvancePaymentMaster.InsertionDate.Day + "/" + x.AdvancePaymentMaster.InsertionDate.Month + "/" + x.AdvancePaymentMaster.InsertionDate.Year,
                                                             x.AdvanceRequests.RequestedValue,
                                                             x.AdvanceRequests.AdvanceSettings.AdvanceSettingName,
                                                             PaidAmount = x.AdvanceReceiveDetails.Select(p => p.NetValue).DefaultIfEmpty(0).Sum(),
                                                             RemainAmount = x.NetValue - x.AdvanceReceiveDetails.Select(p => p.NetValue).DefaultIfEmpty(0).Sum()
                                                         }).ToListAsync();

            //مراحل إعتماد هذا الطلب     
            var dbSchInstance3 = new SchoolAccGam3aEntities();
            var advanceApprovedPhases = dbSchInstance3.AdvanceApprovedPhases
                                                      .Where(x => x.AdvanceRequested_ID == advanceRequest.ID)
                                                      .Select(x => new
                                                      {
                                                          x.Reason,
                                                          x.ApprovedValue,
                                                          x.DashBoard_Users.Name,
                                                          x.AdvancePhases.PhaseName,
                                                          RequestedValue = advanceRequest.RequestedValue.HasValue ? advanceRequest.RequestedValue.Value : 0,
                                                          ResponseDate = x.ResponseDate.Day + "/" + x.ResponseDate.Month + "/" + x.ResponseDate.Year,
                                                      }).ToListAsync();

            // بيانات الطالب الأكاديمية
            var studentBasicData = dbSch.INTEGRATION_All_Students
                                         .FirstOrDefault(x => x.STUDENT_ID == advanceRequest.Student_Id);

            //طلب السلفة بالشكل المراد عرضه         
            var studentAdvanceRequest = new
            {
                advanceRequest.AdvanceSettings.MaxRequestValue,
                AdvanceName = advanceRequest.AdvanceSettings.AdvanceSettingName,
                RequestedDate = advanceRequest.InsertionDate.Hour + ":" + advanceRequest.InsertionDate.Minute + " " +  advanceRequest.InsertionDate.Day + "/" + advanceRequest.InsertionDate.Month + "/" + advanceRequest.InsertionDate.Year,
                advanceRequest.AdvanceSettings.AdvanceType,
                advanceRequest.RequestedValue,
                advanceRequest.ApprovedValue,
                advanceRequest.RequestNotes,
                advanceRequestId
            };

            var NextApprovePhase = GetAdvanceRequestNextApprovePhase(advanceRequestId) ?? new AdvancePhases();

            //await Task.WhenAll(studentAverageDegree, advanceApprovedPhases, advancesApprovedRequests);

            // عمل دمج بين بيانات الطالب والمعدل التراكمي
            var studentAcademicData = new
            {
                ACADEMIC_AVG = CumGpa != null ? CumGpa.CUM_GPA : 0,
                StudentName = studentBasicData.STUDENT_NAME + " - " + "ق.هـ" + " ( " + studentBasicData.NATIONAL_ID + " ) " + " - " + "ق.جـ" + " ( " + studentBasicData.STUDENT_ID + " ) ",
                studentBasicData.NATIONALITY_DESC,
                studentBasicData.FACULTY_NAME,
                studentBasicData.DEGREE_DESC,
                LEVEL_DESC = studentBasicData.LEVEL_DESC == null || studentBasicData.LEVEL_DESC == "" ? "غير متوفر" : studentBasicData.LEVEL_DESC,
                studentBasicData.STUDY_DESC,
                studentBasicData.STATUS_DESC,
            };

            return this.JsonMaxLength(new[]
            {
               new{

                   advanceRequestId,
                   studentAcademicData,
                   studentAdvanceRequest,
                   advanceApprovedPhases = advanceApprovedPhases.Result,
                   advancesApprovedRequests = advancesApprovedRequests.Result,
                   nextApprovePhase = new {
                       NextApprovePhase.Order,
                       NextApprovePhase.PhaseName,
                       NextApprovePhase.IsFinancialPhase
                   }
                 }
            });
        }
        
        [HttpGet]
        public ActionResult CheckValidationOfAdvance(int advanceRequestId)
        {
            var CurrentStudentId = dbSch.AdvanceRequests.Find(advanceRequestId).Student_Id;
            //تفاصيل طلب السلفة            
            var studentAdvancePaymentDetails = dbSch.AdvancePaymentMaster.Where(x => x.Student_Id == CurrentStudentId).
                                               SelectMany(x => x.AdvancePaymentDetails).ToList();



            // مجموع  قيمةالسلف المصروفة للطالب
            var TotalAdvancePayment = studentAdvancePaymentDetails.Where(x => x.AdvanceRequests.AdvanceSettings.AdvanceType == "A").Sum(p => p.NetValue);

            // مجموع  قيمةالسلف التي سددها الطالب للطالب
            var TotalAdvanceReceive = dbSch.AdvanceReceiveDetails.AsEnumerable().Where(x => studentAdvancePaymentDetails.Any(p => p.ID == x.AdvancePaymentDetails_Id)).Sum(x => x.NetValue);

            if ((TotalAdvancePayment - TotalAdvanceReceive) != 0)
            {
                return Content("لا يمكن إتمام عملية الحفظ حتي يسدد الطالب جميع السلف السابقة");
            }

            //===================================
            //التاكد من ان الطالب يستحق مكافئات
            //===================================
            var studentRewards = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == CurrentStudentId);
            if (studentRewards != null)
            {
                if (studentRewards.CATEGORY_CODE == null || studentRewards.CATEGORY_CODE < 0)
                {
                    return Content("لا يمكن إتمام عملية الحفظ لان الطالب لا يستحق مكافئات");
                }
            }
            return Content("");
        }

        [HttpPost]
        public ActionResult ConfirmAdvanceRequest(int advanceRequestId, string recommendationsNotes, string type, decimal approvedValue)
        {

            var advanceRequestInDB = dbSch.AdvanceRequests.SingleOrDefault(x => x.ID == advanceRequestId);
            if (advanceRequestInDB == null)
            {
                return Content("لا يمكن إتمام عملية الحفظ نظرا لإرتباطه بسجلات أخري");
            }

            if (type == "R")
            {
                if (string.IsNullOrEmpty(recommendationsNotes))
                {
                    return Content("عفواادخل توصيات رفض الطلب");
                }

                advanceRequestInDB.User_Id = CurrentUser.ID;
                advanceRequestInDB.RefusedbyID = CurrentUser.ID;
                advanceRequestInDB.StatusNotes = recommendationsNotes;
                dbSch.Entry(advanceRequestInDB).State = System.Data.Entity.EntityState.Modified;


                var advanceApprovedPhases = new AdvanceApprovedPhases()
                {
                    Reason = recommendationsNotes,
                    ResponseDate = DateTime.Now,
                    UserID = CurrentUser.ID,
                    ApprovedStatus = false,
                    ApprovedValue = 0,
                    AdvancePhase_ID = GetAdvanceRequestNextApprovePhase(advanceRequestInDB.ID).ID,
                    AdvanceRequested_ID = advanceRequestInDB.ID
                };

                dbSch.AdvanceApprovedPhases.Add(advanceApprovedPhases);
                dbSch.SaveChanges();
                return Json("");

            }
            var AdvanceApprovedPhases = new AdvanceApprovedPhases()
            {
                Reason = recommendationsNotes ?? "تم الإعتماد",
                ApprovedValue = approvedValue,
                ResponseDate = DateTime.Now,
                UserID = CurrentUser.ID,
                ApprovedStatus = true,
                AdvancePhase_ID = GetAdvanceRequestNextApprovePhase(advanceRequestInDB.ID).ID,
                AdvanceRequested_ID = advanceRequestInDB.ID
            };

            dbSch.AdvanceApprovedPhases.Add(AdvanceApprovedPhases);
            dbSch.SaveChanges();

            //هل دي أخر مرحلة ولا إيه ؟
            if (GetAdvanceRequestNextApprovePhase(advanceRequestInDB.ID)?.AdvanceUsers == null)
            {
                advanceRequestInDB.ApprovedbyID = CurrentUser.ID;
                advanceRequestInDB.ApprovedValue = approvedValue;
                advanceRequestInDB.StatusNotes = recommendationsNotes;

                dbSch.Entry(advanceRequestInDB).State = System.Data.Entity.EntityState.Modified;
            }

            dbSch.SaveChanges();
            return Json("");
        }


        public AdvancePhases GetAdvanceRequestNextApprovePhase(int AdvanceRequestId)
        {
            var maxApprovedPhase = dbSch.AdvanceApprovedPhases
                                        .Where(x => x.AdvanceRequested_ID == AdvanceRequestId).OrderByDescending(x => x.ID).ToList()?.FirstOrDefault();


            if (maxApprovedPhase != null)
            {
                var nextApprovePhaseOrder = dbSch.AdvancePhases
                                                 .SingleOrDefault(x => x.ID == maxApprovedPhase.AdvancePhase_ID).Order + 1;
                return dbSch.AdvancePhases
                            .SingleOrDefault(x => x.Order == nextApprovePhaseOrder);
            }

            return dbSch.AdvancePhases.Single(x => x.Order == 1);
        }



        #endregion

        #region شاشة إعتماد طلبات الإعانات
        #region Permissions

        public class SubsidyDecisionTakingPermission
        {
            public bool View { get; set; }
            public bool Check { get; set; }
            public bool Attach { get; set; }
        }

        [HttpGet]
        public JsonResult GetSubsidyDecisionTakingPermissions(int screenId)
        {

            if (CurrentUser == null || CurrentUser?.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new SubsidyDecisionTakingPermission();

            foreach (var permission in perm)
            {
                if (permission == "معاينة")
                {
                    permissions.Check = true;
                }
                else if (permission == "عرض")
                {
                    permissions.View = true;
                }
                else if (permission == "ارفاق")
                {
                    permissions.Attach = true;
                }
            }

            return this.JsonMaxLength(permissions);
        }

        #endregion

        [HttpGet]
        public ActionResult SubsidyDecisionTaking()
        {
            var json = new JavaScriptSerializer().Serialize(GetSubsidyDecisionTakingPermissions(83).Data);
            var permissions = JsonConvert.DeserializeObject<SubsidyDecisionTakingPermission>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }


        #endregion

        #region  شاشة طلبات السلف والإعانات من عند الأدمن
        #region Permissions

        [HttpGet]
        public JsonResult GetStudentAdvanceOperationsPermissions(int screenId)
        {

            if (CurrentUser.ID == 0)
            {
                return null;
            }

            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

            var permissions = new StudentAdvanceOperationsPermission();

            foreach (var permission in perm)
            {
                if (permission == "عرض")
                {
                    permissions.View = true;
                }
                else if (permission == "طلب سلفة")
                {
                    permissions.AdvenceRequest = true;
                }
                else if (permission == "طلب اعانة")
                {
                    permissions.SubsidyRequest = true;
                }
                else if (permission == "معاينة السلف")
                {
                    permissions.CheckAdvence = true;
                }
                else if (permission == "معاينة الاعانة")
                {
                    permissions.CheckSubsidy = true;
                }
            }

            return this.JsonMaxLength(permissions);
        }


        public class StudentAdvanceOperationsPermission
        {
            public bool View { get; set; }
            public bool AdvenceRequest { get; set; }
            public bool SubsidyRequest { get; set; }
            public bool CheckAdvence { get; set; }
            public bool CheckSubsidy { get; set; }
        }

        #endregion

        [HttpGet]
        public ActionResult StudentAdvanceOperations()
        {
            var json = new JavaScriptSerializer().Serialize(GetStudentAdvanceOperationsPermissions(84).Data);
            var permissions = JsonConvert.DeserializeObject<StudentAdvanceOperationsPermission>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }


        [HttpGet]
        public async Task<ActionResult> GetStudentDataByID(int studentId)
        {
            var dbInstance1 = new SchoolAccGam3aEntities();
            var studentBasicDataAsTask = dbInstance1.INTEGRATION_All_Students
                                                    .Where(x => x.STUDENT_ID == studentId)
                                                    .Select(x => new
                                                    {
                                                        NationalityName = x.NATIONALITY_DESC,
                                                        FacultiyName = x.FACULTY_NAME,
                                                        StudentName = x.STUDENT_NAME + " - " + "ق.هـ" + " ( " + x.NATIONAL_ID + " ) " + " - " + "ق.جـ" + " ( " + x.STUDENT_ID + " ) ",
                                                        StudyTypeName = x.STUDY_DESC,
                                                        National_Id = x.NATIONAL_ID,
                                                        Student_Id = x.STUDENT_ID,
                                                        DegreeName = x.DEGREE_DESC,
                                                        StatusName = x.STATUS_DESC,
                                                        LevelName = x.LEVEL_DESC == null || x.LEVEL_DESC == "" ? "غير متوفر" : x.LEVEL_DESC

                                                    }).FirstOrDefaultAsync();

            //var dbInstance2 = new SchoolAccGam3aEntities();
            var CumGpa = dbSch.ST_AcademicData.Where(x => x.STUDENT_ID == studentId && x.SEMESTER_GPA != 0 && x.SEMESTER_GPA != null).OrderByDescending(x => x.SEMESTER).FirstOrDefault();
            var ImageName = dbSch.UniversityStudents.FirstOrDefault(x => x.Student_ID == studentId)?.ImageName;

            //var studentAverageDegreeAsTask = dbInstance2.ST_AcademicData
            //                                            .Where(x => x.student_id == studentId)
            //                                            .AverageAsync(p => p.semester_gpa);

            //await Task.WhenAll(studentBasicDataAsTask, studentAverageDegreeAsTask);
            return this.JsonMaxLength(new[]
            {
               new{
                   ImageName,
                   StudentBasicData = studentBasicDataAsTask.Result,
                   StudentAverageDegree = CumGpa?.CUM_GPA == null ? "غير متوفر" : CumGpa.CUM_GPA.ToString()
                  }
            });
        }

        [HttpGet]
        public ActionResult GetAllStudents()
        {
            return this.JsonMaxLength(dbSch.SP_GetAllStudent_Names_Customize().ToList());
        }

        [HttpGet]
        public async Task<ActionResult> AdvancesDataSource(int studentId, string type)
        {
            // كل الإعانات الخاصة بالطالب
            var db = new SchoolAccGam3aEntities();

            var data = await db.AdvanceRequests
                                               .Where(x => (x.IsCanceled == false || x.IsCanceled == null) &&
                                                           x.Student_Id == studentId &&
                                                           x.AdvanceSettings.AdvanceType == type)
                                               .Select(x => new
                                               {
                                                   x.ApprovedValue,
                                                   x.RequestedValue,
                                                   AdvanceRequestId = x.ID,
                                                   x.AdvanceSettings.AdvanceSettingName,
                                                   x.AdvanceSettings.AdvanceSettingId,
                                                   AdvanceIsPaid = x.AdvancePaymentDetails.Count() > 0 ? "تم الصرف" : "لم يصرف بعد",
                                                   InsertionDate = x.InsertionDate.Hour+":"+x.InsertionDate.Minute+":" +" "+ x.InsertionDate.Day + "/" + x.InsertionDate.Month + "/" + x.InsertionDate.Year,
                                                   ApprovedStatus = x.ApprovedbyID != null ? "معتمد" : x.RefusedbyID != null ? "مرفوض" : "تحت المراجعة",
                                                   IsApprovedPhase = x.AdvanceApprovedPhases.Count() > 0 ? true : false
                                               }).ToListAsync();

            return this.JsonMaxLength(data);
        }


        [HttpGet]
        public async Task<ActionResult> StudentAdvanceDetailDataSource(int advanceRequestId)
        {
            // طلب السلفةالمراد معرفة تفاصيله
            var advanceRequest = dbSch.AdvanceRequests
                                      .FirstOrDefault(x =>
                                                            (x.IsCanceled == false || x.IsCanceled == null) &&
                                                            x.ID == advanceRequestId);

            //طلبات السلفة التي تم إعتمادها وصرفها وتمت عملية سدادها بالفعل وتخص هذا الطالب    


            var dbSchInstance2 = new SchoolAccGam3aEntities();
            var advancePremiums = dbSchInstance2.AdvanceReceiveDetails
                                                .Where(x => x.AdvancePaymentDetails.AdvanceRequests_Id == advanceRequestId)
                                                .Select(x => new
                                                {
                                                    PaidAmount = x.NetValue,
                                                    PayRollNumber = x.AdvanceReceiveMaster.PayrollId != null ? x.AdvanceReceiveMaster.Payroll.PayrollNumber.ToString() : x.AdvanceReceiveMaster.DocNumber,
                                                    PayrollDate = x.AdvanceReceiveMaster.InsertionDate.Day + "/" + x.AdvanceReceiveMaster.InsertionDate.Month + "/" + x.AdvanceReceiveMaster.InsertionDate.Year,
                                                    PaymentType = x.AdvanceReceiveMaster.PayrollId != null ? "الخصم من مسير المكافأت" : "السداد النقدي"
                                                }).ToListAsync();


            //مراحل إعتماد هذا الطلب     
            var dbSchInstance3 = new SchoolAccGam3aEntities();
            var advanceApprovedPhases = dbSchInstance3.AdvanceApprovedPhases
                                                      .Where(x => x.AdvanceRequested_ID == advanceRequest.ID)
                                                      .Select(x => new
                                                      {
                                                          x.Reason,
                                                          x.ApprovedValue,
                                                          x.DashBoard_Users.Name,
                                                          x.AdvancePhases.PhaseName,
                                                          advanceRequest.RequestedValue,
                                                          ResponseDate = +x.ResponseDate.Day + "/" + x.ResponseDate.Month + "/" + x.ResponseDate.Year+":" + x.ResponseDate.Hour + ":" + x.ResponseDate.Minute,
                                                      }).ToListAsync();

            //طلب السلفة بالشكل المراد عرضه         
            var studentAdvanceRequest = new[]
            {
                new {
                    AdvanceName = advanceRequest.AdvanceSettings.AdvanceSettingName,
                    RequestedDate =  advanceRequest.InsertionDate.Hour + ":" + advanceRequest.InsertionDate.Minute + " " +advanceRequest.InsertionDate.Day + "/" + advanceRequest.InsertionDate.Month + "/" + advanceRequest.InsertionDate.Year,
                    advanceRequest.AdvanceSettings.AdvanceType,
                    advanceRequest.RequestedValue,
                    advanceRequest.ApprovedValue,
                    advanceRequest.RequestNotes,
                    approvedStatus = advanceRequest.ApprovedbyID != null ? "معتمد" : advanceRequest.RefusedbyID != null ? "مرفوض" : "تحت المراجعة"
                }
             };

            var NextApprovePhase = GetAdvanceRequestNextApprovePhase(advanceRequestId) ?? new AdvancePhases();

            await Task.WhenAll(advanceApprovedPhases, advancePremiums);



            return this.JsonMaxLength(new[]
            {
               new{

                   advanceRequestId,
                   studentAdvanceRequest,
                   advanceApprovedPhases = advanceApprovedPhases.Result,
                   advancePremiums = advancePremiums.Result,
                   nextApprovePhase = new {
                       NextApprovePhase.Order,
                       NextApprovePhase.PhaseName,
                       NextApprovePhase.IsFinancialPhase
                   }
                 }
            });
        }


        [HttpPost]
        public ActionResult SaveAdvanceRequestByAdmin(int studentId, int advanceTypeId, decimal advanceValue, string advanceRequestNotes)
        {

            if (dbSch.AdvanceRequests.Any(x => x.Student_Id == studentId && x.AdvanceSettings_Id == advanceTypeId && x.IsCanceled != true && x.ApprovedbyID == null && x.RefusedbyID == null))
            {
                return Content("عفوا لقد تم إدخال هذا الطلب من قبل وجاري مراجعته");
            }

            if (advanceValue <= 0)
            {
                return Content("لا يمكن أن يكون القيمة المطلوبة أقل من أو تساوي صفر");
            }

            var type = dbSch.AdvanceSettings.SingleOrDefault(x => x.AdvanceSettingId == advanceTypeId)?.AdvanceType;

            //if (type == "S")
            //{
                var file = (HttpPostedFileBase)Session["AdvanceRequestFile"];

                if (file == null)
                {
                    return Content("لا يمكن إتمام عملية الحفظ حتي يتم رفع مرفق الطلب ");
                }
            //}


            //==========================================
            //التاكد ان الطالب يستحق مكافئات
            //==========================================
            var studentRewards = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentId);
            if (studentRewards != null)
            {
                if (studentRewards.CATEGORY_CODE == null || studentRewards.CATEGORY_CODE < 0)
                {
                    return Content("لا يمكن إتمام عملية الحفظ لان الطالب لا يستحق مكافئات");
                }
            }

            var advanceRequests = new AdvanceRequests
            {
                AdvanceSettings_Id = advanceTypeId,
                InsertionDate = DateTime.Now,
                Student_Id = studentId,
                RequestNotes = advanceRequestNotes,
                RequestedValue = advanceValue,
                User_Id = CurrentUser.ID
            };
            dbSch.AdvanceRequests.Add(advanceRequests);
            dbSch.SaveChanges();

            //if (type == "S")
            //{
              //  var file = (HttpPostedFileBase)Session["AdvanceRequestFile"];

                // file
                var rootFolder = Server.MapPath("~/Content/UserFiles/");
                var studentFolder = Path.Combine(rootFolder, (studentId>0?studentId:CurrentStudentId).ToString());
                var currentFolder = new DirectoryInfo(studentFolder);

                if (currentFolder.Exists == false)
                {
                    Directory.CreateDirectory(studentFolder);
                }

                var dSecurity = currentFolder.GetAccessControl();


                dSecurity.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                    PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                currentFolder.SetAccessControl(dSecurity);


                var stAdvancesFolder = Path.Combine(rootFolder, CurrentStudentId.ToString(), "Advances");

                if (!new DirectoryInfo(stAdvancesFolder).Exists)
                {
                    Directory.CreateDirectory(stAdvancesFolder);
                }


                var dSecurityForAdvances = new DirectoryInfo(stAdvancesFolder).GetAccessControl();

                dSecurityForAdvances.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                    PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                new DirectoryInfo(stAdvancesFolder).SetAccessControl(dSecurityForAdvances);


                // Start From Here

                string filename = String.Format("{0}_{1}.{2}", file.FileName.Split('.')[0],
                    DateTime.Now.ToString("dd-MM-yyyy-HH-mm"), file.FileName.Split('.')[1]);

                string path = Path.Combine(stAdvancesFolder, filename);


                Stream stream = file.InputStream;
                byte[] bytes = ReadToEnd(stream);
                FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();


                var advanceRequestsAttachment = new AdvanceRequestsAttachment
                {
                    FileName = filename,
                    Path = MapPathReverse(path),
                    InsertDate = DateTime.Now,
                    UserId = CurrentUser.ID,
                    AdvanceRequests_Id = advanceRequests.ID,

                };
                dbSch.AdvanceRequestsAttachment.Add(advanceRequestsAttachment);
                dbSch.SaveChanges();

                Session["AdvanceRequestFile"] = null;
            //}
            return Content("");
        }

        [HttpGet]
        public ActionResult IsDueAdvance(int? studentId)
        {
            studentId = studentId ?? CurrentStudentId;
            var student = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentId);

            if (student == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (student.CATEGORY_CODE == null)
            {
                return this.JsonMaxLength($"لا يمكن طلب سلفة لأن بيانات فئة المكافأة الخاصة بالطالب غير متوفرة");
            }

            int Temp_nationalityId;
            if (!int.TryParse(student.NATIONALITY_CODE.ToString(), out Temp_nationalityId))
            {
                return this.JsonMaxLength($"لا يمكن طلب سلفة لأن بيانات الجنسية غير صحيحة");
            }

            if (student.LEVEL_CODE == null)
            {
                return this.JsonMaxLength($"لا يمكن طلب سلفة لأن بيانات المستوي الأكاديمي غير متوفرة");
            }

            var excludedLevels = dbSch.AdvanceSettingConfig.FirstOrDefault(x => x.Key == "Levels").Value
                                                           .Split(',').Select(x => Convert.ToInt32(x)).ToList();
            if (excludedLevels.Exists(x => x == student.LEVEL_CODE))
            {
                return this.JsonMaxLength($"لا يمكن طلب سلفة حيث أن الطالب يقع ضمن المستويات الغير مستحقة لطلب سلف");
            }

            //================================
            // المعدل التراكمي لدرجات الطالب
            //================================
            var CumGpa = dbSch.ST_AcademicData.Where(x => x.STUDENT_ID == studentId && x.SEMESTER_GPA != 0 && x.SEMESTER_GPA != null).OrderByDescending(x => x.SEMESTER).FirstOrDefault();

            //var studentAverageDegree = dbSch.ST_AcademicData
            //                                .Where(x => student.STUDENT_ID == x.student_id)
            //                                .Average(p => p.semester_gpa);

            if (CumGpa == null)
            {
                return this.JsonMaxLength($"لا يمكن طلب سلفة لأن بيانات المعدل التراكمي غير متوفرة");
            }
            var miniGPA = dbSch.AdvanceSettingConfig.FirstOrDefault(x => x.Key == "MiniGPA").Value ?? "0";




            if (CumGpa.CUM_GPA < Convert.ToDouble(miniGPA))
            {
                return this.JsonMaxLength($"لا يمكن طلب سلفة حيث أن المعدل التراكمي لدرجات الطالب أقل من الحد الأدني الذي تم تهيئته لاستحقاق طلب سلفة");
            }

            var rewardDuesList = dbSch.SP_GetRewardDetailsList(
                                    (int)student.FACULTY_NO,
                                    (int)student.DEGREE_CODE,
                                    (int)student.STATUS_CODE,
                                    (int)student.NATIONALITY_CODE,
                                    (int)student.STUDY_CODE,
                                    (int)student.CATEGORY_CODE,
                                    string.Join(",", dbSch.RewardItems.Select(x => x.ID).ToList())).ToList();

            if (rewardDuesList.Count() == 0)
            {
                return this.JsonMaxLength($"لا يمكن طلب سلفة لأنه لا يستحق أى بند من بنود المكافئات التي تم تهيئتها");
            }
            //======================================================
            // بند المكافأة رقم 26 هو بند المكافأة الشهري الثابت
            //======================================================
            else if (rewardDuesList.Where(x => x.Allowance_ID == 26).Count() == 0)
            {
                return this.JsonMaxLength($"لا يمكن طلب سلفة لأنه لا يستحق بند المكافأة الشهرية");
            }

            return this.JsonMaxLength("");
        }

        #endregion

        #region شاشة التحصيل النقدي للسلف  
        #region Permissions
        [HttpGet]
        public JsonResult GetAdvancePaymentPermissions(int screenId)
        {

            if (CurrentUser == null || CurrentUser?.ID == 0)
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
                else if (permission == "تحصيل السلف المختارة" || permission == "ترحيل قيد مجمع بالسندات" || permission == "ترحيل الاقساط المحددة")
                {
                    permissions.ExchangeSelected = true;
                }
                else if (permission == "معاينة القيد")
                {
                    permissions.CheckVoucher = true;
                }

            }

            return this.JsonMaxLength(permissions);
        }


        //public class AdvancePaymentPermission
        //{
        //    public bool View { get; set; }
        //    public bool ExchangeSelected { get; set; }
        //}


        #endregion

        [HttpGet]
        public ActionResult AdvanceCashPayment()
        {
            var json = new JavaScriptSerializer().Serialize(GetAdvancePaymentPermissions(94).Data);
            var permissions = JsonConvert.DeserializeObject<AdvancePaymentPermission>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult GetTreasuryAccount()
       {
            var UserInAcc = dbAcc.Users.FirstOrDefault(x => x.ID == CurrentUser.AccID);
            if (UserInAcc == null)
            {
                return this.JsonMaxLength("");
            }
            var data = dbAcc.Treasuries
                                           .Where(x => x.BranchId == UserInAcc.School_ID)
                                           .Select(x => new
                                           {
                                               COAID = x.CashAccount,
                                               IsAccountant = UserInAcc.EnableUserAccounts
                                           }).FirstOrDefault();

            return this.JsonMaxLength(data);
        }



        [HttpGet]
        public ActionResult DataSourcePaidAdvancesRequest()
        {

            var advances = dbSch.AdvancePaymentDetails
                                .Where(x => x.AdvanceRequests.AdvanceSettings.AdvanceType == "A" &&
                                           (x.NetValue - x.AdvanceReceiveDetails.Select(P => P.NetValue).DefaultIfEmpty(0).Sum()) > 0)
                                .GroupBy(x => new
                                {
                                    x.ID,
                                    ApprovedValue = x.NetValue,
                                    x.AdvanceRequests.Student_Id,
                                    x.AdvancePaymentMaster.InsertionDate,
                                    AdvanceRequests_Id = x.AdvanceRequests.ID,
                                    x.AdvanceRequests.AdvanceSettings.AdvanceSettingName,
                                    ReceiveAmount = x.AdvanceReceiveDetails.Select(p => p.NetValue).DefaultIfEmpty(0).Sum(),
                                });


            var studentAcademicData = dbSch.INTEGRATION_All_Students
                                           .Select(x => new
                                           {
                                               x.STUDENT_ID,
                                               x.STUDENT_NAME,
                                               x.NATIONAL_ID,
                                               x.NATIONALITY_DESC,
                                               x.FACULTY_NAME,
                                               x.DEGREE_DESC,
                                               LEVEL_DESC = x.LEVEL_DESC == null || x.LEVEL_DESC == "" ? "غير متوفر" : x.LEVEL_DESC,
                                               x.STUDY_DESC,
                                               x.STATUS_DESC,
                                           });

            return this.JsonMaxLength(
                        advances.Select(x => new
                        {
                            x.Key.ReceiveAmount,
                            x.Key.ApprovedValue,
                            AdvanceName = x.Key.AdvanceSettingName,
                            AdvanceRequestId = x.Key.AdvanceRequests_Id,
                            RemainAmount = x.Key.ApprovedValue - x.Key.ReceiveAmount,
                            StudentAcademicData = studentAcademicData.FirstOrDefault(p => p.STUDENT_ID == x.Key.Student_Id),
                            PaidDate = x.Key.InsertionDate.Day + "/" + x.Key.InsertionDate.Month + "/" + x.Key.InsertionDate.Year,
                            LastReceiveDate = dbSch.AdvanceReceiveDetails
                                                   .Where(c => c.AdvancePaymentDetails_Id == x.Key.ID)
                                                   .Max(o => o.AdvanceReceiveMaster.InsertionDate.Day + "/" +
                                                             o.AdvanceReceiveMaster.InsertionDate.Month + "/" +
                                                             o.AdvanceReceiveMaster.InsertionDate.Year)

                        }).ToList()
                );
        }



        public ActionResult PayAdvances(string AdvanceRequestIds,
                                        int? PaymentCoaId, decimal PaymentValue,
                                        string DocDesc, bool PayAllRemain, bool postJournal, DateTime PaymentDate)
        {
            dbAcc.Configuration.LazyLoadingEnabled = false;
            dbSch.Configuration.LazyLoadingEnabled = false;

            //===============================================
            // بيانات المستخدم الحالي بداخل نظام الحسابات 
            //===============================================

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

            //============================
            // دي السلف اللي بيتم سدادها 
            //============================
            var advanceRequestIds = AdvanceRequestIds.Split(',').Select(x => Convert.ToInt32(x)).ToList();

            var advancePaymentDetails = dbSch.AdvancePaymentDetails
                                             .Include(x => x.AdvanceRequests)
                                             .Include(x => x.AdvancePaymentMaster)
                                             .Include(x => x.AdvanceReceiveDetails)
                                             .Include(x => x.AdvanceRequests.AdvanceSettings)
                                             .Where(x => x.AdvanceRequests.AdvanceSettings.AdvanceType == "A" &&
                                                         advanceRequestIds.Any(p => p == x.AdvanceRequests_Id) &&
                                                         (x.NetValue - x.AdvanceReceiveDetails.Select(P => P.NetValue).DefaultIfEmpty(0).Sum() > 0));

            //======================================
            // كدا معانا البيانات الأكاديمية للطلاب  
            //======================================
            var listOfStudents = dbSch.INTEGRATION_All_Students
                                      .Where(s => advancePaymentDetails.Any(x => x.AdvancePaymentMaster.Student_Id == s.STUDENT_ID)).ToList();

            var returnedMasterIds = new List<int>();
            listOfStudents.ForEach(studentItem =>
            {
                var LastDocNumber = dbSch.usp_GetLastDocNumber().FirstOrDefault().Value.ToString();
                var listOfAdvanceReceiveDetails = new List<AdvanceReceiveDetails>();

                var advanceReceiveMaster = new AdvanceReceiveMaster
                {
                    DocNotes = DocDesc,
                    User_Id = CurrentUser.ID,
                    DocNumber = LastDocNumber,
                    COA_ID = (int)PaymentCoaId,
                    Student_Id = (int)studentItem.STUDENT_ID,
                    InsertionDate = DateTime.Now,
                };
                dbSch.AdvanceReceiveMaster.Add(advanceReceiveMaster);
                //======================================
                //لازم أعمل حفظ عشان رقم السند مش يتكرر
                //======================================
                dbSch.SaveChanges();

                returnedMasterIds.Add(advanceReceiveMaster.ID);

                advancePaymentDetails.Where(x => x.AdvancePaymentMaster.Student_Id == studentItem.STUDENT_ID).ToList()
                .ForEach(advancePaymentDetailsItem =>
                {

                    var RemainAmount = PayAllRemain ? (advancePaymentDetailsItem.NetValue - advancePaymentDetailsItem.AdvanceReceiveDetails.Select(p => p.NetValue).DefaultIfEmpty(0).Sum()) : PaymentValue;
                    var AdvanceName = advancePaymentDetailsItem.AdvanceRequests.AdvanceSettings.AdvanceSettingName;

                    listOfAdvanceReceiveDetails.Add(
                         new AdvanceReceiveDetails
                         {
                             AdvancePaymentDetails_Id = advancePaymentDetailsItem.ID,
                             AdvanceReceiveMaster_Id = advanceReceiveMaster.ID,
                             NetValue = RemainAmount
                         });
                });
                advanceReceiveMaster.TotalValue = listOfAdvanceReceiveDetails.Sum(x => x.NetValue);

                dbSch.AdvanceReceiveDetails.AddRange(listOfAdvanceReceiveDetails);

                if (postJournal)
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
                    GJHDescription.Append($" تحصيل أقساط السلف عن حساب الطالب ");
                    GJHDescription.Append($"{  studentItem.STUDENT_NAME + "-" + "ق.هـ" + " ( " + studentItem.NATIONAL_ID + " ) " + "-" + "ق.جـ" + " ( " + studentItem.STUDENT_ID + " ) " }");

                    GJH GJHmodel = new GJH();
                    GJHmodel.OPT_ID = BCDID;
                    GJHmodel.GJHStatus = true;
                    GJHmodel.USR_ID = UserInAcc.ID;
                    GJHmodel.GJHRefranceNumber = "";
                    GJHmodel.InsertDate = DateTime.Now;
                    GJHmodel.COM_ID = UserInAcc.School_ID;
                    GJHmodel.GJHOperationDate = PaymentDate;
                    GJHmodel.GJHSystemNumber = LastSystemNumber;
                    GJHmodel.GJHDescription = GJHDescription.ToString();
                    GJHmodel.GJHAmount = advanceReceiveMaster.TotalValue;
                    GJHmodel.JournalNo = dbAcc.NextJournalEntryNo_Sp(UserInAcc.School_ID, fsyid).Single().Value;

                    dbAcc.GJH.Add(GJHmodel);
                    dbAcc.SaveChanges();

                    advanceReceiveMaster.IsPosted = true;
                    advanceReceiveMaster.GJH_ID = (int)GJHmodel.GJHID;

                    var _GJDDescription = new StringBuilder();
                    _GJDDescription.Append($" المحصل من الطالب ");
                    _GJDDescription.Append($" { studentItem.STUDENT_NAME + "-" + "ق.هـ" + "( " + studentItem.NATIONAL_ID + " )"}");


                    listOfAdvanceReceiveDetails.ForEach(AdvanceReceiveDetailItem =>
                    {
                        var AdvanceName = AdvanceReceiveDetailItem.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName;
                        var AdvanceCoa = AdvanceReceiveDetailItem.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.COAID.Value;

                        var GJDDescription = new StringBuilder();
                        GJDDescription.Append($" المحصل من الطالب ");
                        GJDDescription.Append($" { studentItem.STUDENT_NAME + "-" + "ق.هـ" + "( " + studentItem.NATIONAL_ID + " )"}");
                        GJDDescription.Append($" بتاريخ {AdvanceReceiveDetailItem.AdvanceReceiveMaster.InsertionDate.ToShortDateString()}");

                        GJDDescription.Append($" من قسط ");
                        GJDDescription.Append($"{AdvanceName}");

                        _GJDDescription.Append($" من قسط ");
                        _GJDDescription.Append($"- {AdvanceName} ");

                        GJD GJDmodel = new GJD();
                        GJDmodel.JOB_ID = null;
                        GJDmodel.Fsy_ID = fsyid;
                        GJDmodel.GJDStatus = true;
                        GJDmodel.GJDDebitAmount = 0;
                        GJDmodel.GJH_ID = GJHmodel.GJHID;
                        GJDmodel.Com_ID = UserInAcc.School_ID;
                        GJDmodel.GJDCreditAmount = AdvanceReceiveDetailItem.NetValue;
                        GJDmodel.GJDDescrition = GJDDescription.ToString();
                        GJDmodel.COA_ID = AdvanceCoa;

                        dbAcc.GJD.Add(GJDmodel);
                    });

                    GJD _GJDmodel = new GJD();
                    _GJDmodel.JOB_ID = null;
                    _GJDmodel.Fsy_ID = fsyid;
                    _GJDmodel.GJDStatus = true;
                    _GJDmodel.GJDCreditAmount = 0;
                    _GJDmodel.GJH_ID = GJHmodel.GJHID;
                    _GJDmodel.Com_ID = UserInAcc.School_ID;
                    _GJDmodel.COA_ID = advanceReceiveMaster.COA_ID;
                    _GJDmodel.GJDDescrition = _GJDDescription.ToString();
                    _GJDmodel.GJDDebitAmount = advanceReceiveMaster.TotalValue;
                    dbAcc.GJD.Add(_GJDmodel);

                    advanceReceiveMaster.DocHeader = _GJDDescription.ToString();
                }
                else
                {
                    var Description = new StringBuilder();
                    Description.Append($" المحصل من الطالب ");
                    Description.Append($" { studentItem.STUDENT_NAME + "-" + "ق.هـ" + "( " + studentItem.NATIONAL_ID + " )"}");
                    Description.Append($" من قسط ");
                    Description.Append($" {string.Join(" و ", listOfAdvanceReceiveDetails.Select(x => x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName).Distinct().ToList())}");
                    Description.Append($" بتاريخ {advanceReceiveMaster.InsertionDate.ToShortDateString()}");
                    advanceReceiveMaster.DocHeader = Description.ToString();
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
            var uspConfigSelectResult = dbAcc.usp_GetConfigValues("JournalEntryOperation").FirstOrDefault();//CashReciveOperation

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

        #endregion

        #region شاشة ترحيل القيود المجمعة لسندات صرف السلف    

        [HttpGet]
        public ActionResult PaidAdvancesAggregateJournal()
        {
            var json = new JavaScriptSerializer().Serialize(GetAdvancePaymentPermissions(95).Data);
            var permissions = JsonConvert.DeserializeObject<AdvancePaymentPermission>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        [HttpGet]
        public ActionResult GetDocsOfAdvances(string type)
        {

            return this.JsonMaxLength(dbSch.AdvancePaymentDetails
                                           .Where(x => x.AdvancePaymentMaster.IsPosted != true && x.AdvanceRequests.AdvanceSettings.AdvanceType == type)
                                           .ToList().Select(x => new
                                           {
                                               x.AdvancePaymentMaster.DocNotes,
                                               x.AdvancePaymentMaster.DocNumber,
                                               x.AdvancePaymentMaster.DocHeader,
                                               MasterID = x.AdvancePaymentMaster_Id,
                                               InsertionDate = x.AdvancePaymentMaster.InsertionDate.Month + "/" + x.AdvancePaymentMaster.InsertionDate.Day + "/" + x.AdvancePaymentMaster.InsertionDate.Year,
                                               DocTotalValue = x.AdvancePaymentMaster.TotalValue,
                                               PaidAccountName = GetCoaName(x.AdvancePaymentMaster.COA_ID),
                                               StudentAcademicData = dbSch.INTEGRATION_All_Students.Where(p => p.STUDENT_ID == x.AdvancePaymentMaster.Student_Id)
                                                                    .Select(p => new
                                                                    {
                                                                        p.STUDENT_ID,
                                                                        p.STUDENT_NAME,
                                                                        p.FACULTY_NAME,
                                                                        p.NATIONAL_ID
                                                                    }).FirstOrDefault()
                                           }).Distinct().ToList());


        }

        [HttpGet]
        public ActionResult GetDocDetails(int masterId, string type)
        {
            var advancePaymentDetailsList = dbSch.AdvancePaymentDetails
                                                 .Where(x => x.AdvancePaymentMaster_Id == masterId);

            var StudentAcademicData = dbSch.INTEGRATION_All_Students
                                           .Where(p => advancePaymentDetailsList.Any(x => x.AdvancePaymentMaster.Student_Id == p.STUDENT_ID))
                                           .Select(p => new
                                           {
                                               p.STUDENT_ID,
                                               p.STUDENT_NAME,
                                               p.FACULTY_NAME,
                                               p.NATIONAL_ID
                                           }).FirstOrDefault();

            var AdvanceCoa = advancePaymentDetailsList.Select(x => x.AdvanceRequests.AdvanceSettings.COAID.Value).FirstOrDefault();

            var GJDDescription = new StringBuilder();
            var customGJDList = new List<CustomGJD>();

            advancePaymentDetailsList.ToList().ForEach(x =>
            {
                var advanceName = x.AdvanceRequests.AdvanceSettings.AdvanceSettingName;

                GJDDescription.Append($" صرف ");
                GJDDescription.Append($" {advanceName} ");
                GJDDescription.Append($" لحساب الطالب ");
                GJDDescription.Append($" { StudentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + "( " + StudentAcademicData.NATIONAL_ID + " )"}");
                GJDDescription.Append($" بتاريخ {x.AdvancePaymentMaster.InsertionDate.ToShortDateString()}");

                customGJDList.Add(new CustomGJD
                {
                    GJDCreditAmount = 0.00M,
                    GJDDebitAmount = x.NetValue,
                    AccountName = GetCoaName(AdvanceCoa),
                    GJDDescrition = GJDDescription.ToString()
                });

            });

            var _GJDDescription = new StringBuilder();
            _GJDDescription.Append($" مجموع ما تم صرفه لحساب الطالب ");
            _GJDDescription.Append($"{  StudentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + "( " + StudentAcademicData.NATIONAL_ID + " )"}");
            _GJDDescription.Append($" فيما يخص ");
            _GJDDescription.Append($"{ string.Join(" و ", advancePaymentDetailsList.Select(x => x.AdvanceRequests.AdvanceSettings.AdvanceSettingName).Distinct().ToList())}");

            customGJDList.Add(new CustomGJD
            {
                RandomKey = Guid.NewGuid().ToString("n"),
                GJDDebitAmount = 0.00M,
                GJDCreditAmount = customGJDList.Select(x => x.GJDDebitAmount).DefaultIfEmpty(0).Sum(),
                AccountName = GetCoaName(advancePaymentDetailsList.Select(x => x.AdvancePaymentMaster.COA_ID).FirstOrDefault()),
                GJDDescrition = _GJDDescription.ToString()

            });
            return this.JsonMaxLength(customGJDList.ToList());
        }

        public string GetCoaName(int coaId)
        {
            return dbSch.usp_COA_Select_ByCoaID(coaId).FirstOrDefault()?.COAName ?? "لم يتم تهيئة الحساب بعد";
        }

        [HttpPost]
        public ActionResult PostAggregateJournalForPaidDocs(string DocMasterIds, string Type, DateTime paramPostedDate)
        {
            //===============================================
            // بيانات المستخدم الحالي بداخل نظام الحسابات 
            //===============================================
            if (paramPostedDate == null)
            {
                return this.JsonMaxLength("عفوا لا يمكن إتمام عملية الحفظ حتي يتم إدخال تاريخ الترحيل");
            }

            var UserInAcc = dbAcc.Users.FirstOrDefault(x => x.ID == CurrentUser.AccID);
            if (UserInAcc == null)
            {
                return this.JsonMaxLength("عفوا لا يمكن إتمام عملية الحفظ حتي يتم ربط بيانات المستخدم بنظام الحسابات العامة");
            }

            int? fsyid = dbAcc.FSY.SingleOrDefault(x => x.COM_ID == UserInAcc.School_ID && x.Is_Open == true &&
                                                        x.FSYStartDate <= paramPostedDate && x.FSYEndDate >= paramPostedDate)?.FSYID;
            if (fsyid == null)  
            {
                return this.JsonMaxLength($" عفوا لا يمكن إتمام عملية الحفظ حيث لا يوجد أي سنة مالية مفتوحة حاليا بداخل نظام الحسابات العامة " +
                                           "أو ربما تاريخ الترحيل يقع خارج نطاق السنة المفتوحة حاليا");
            }

            //=====================================
            // نوع العملية هنا اللي هى قيد يومية 
            //=====================================
            var OperationId = GetOperationId().Value;
            var existedBCD = GetBcdId(OperationId);
            if (existedBCD == null)
            {
                return this.JsonMaxLength("برجاء مراجعة تهيئة العمليات في الحسابات العامة");
            }

            var BCDID = existedBCD.BCDID;

            //=============================================================
            // دا رقم العملية الأهم اللي هما بيعتبروها مش متكررة ع الإطلاق 
            //=============================================================
            var LastSystemNumber = GetLastSystemNumber((int)fsyid, BCDID);


            //===================================================
            // دي السندات المصروفة اللي هيتم عمل قيد مجمع بيها 
            //===================================================
            var docMasterIds = DocMasterIds.Split(',').Select(x => Convert.ToInt32(x)).ToList();


            var advancePaymentDetailsList = dbSch.AdvancePaymentDetails
                                                 .Where(x => docMasterIds.Any(p => p == x.AdvancePaymentMaster_Id));

            //==============================
            // دي البيانات الأكاديمية للطلاب
            //==============================
            var StudentAcademicDataList = dbSch.INTEGRATION_All_Students
                                               .Where(p => advancePaymentDetailsList.Any(x => x.AdvancePaymentMaster.Student_Id == p.STUDENT_ID))
                                               .Select(p => new
                                               {
                                                   p.STUDENT_ID,
                                                   p.STUDENT_NAME,
                                                   p.FACULTY_NAME,
                                                   p.NATIONAL_ID
                                               }).Distinct().ToList();

            //======================================================
            // دي الحسابات اللي علي أساسها هيتم عمل القيد المجمع
            //======================================================
            var distinctAdvancePaymentDetailsCoa = advancePaymentDetailsList.ToList().Select(x => new
            {
                CoaId = x.AdvancePaymentMaster.COA_ID
            }).Distinct().ToList();


            var GJHDescription = new StringBuilder();
            var typeName = Type == "A" ? "سلف" : "إعانات";
            GJHDescription.Append($" قيد صرف {typeName} مجمع للطلاب وبيانه صرف ");

            StudentAcademicDataList.ForEach(Student =>
            {
                GJHDescription.Append($"{ string.Join(" و ", advancePaymentDetailsList.Where(x => x.AdvancePaymentMaster.Student_Id == Student.STUDENT_ID).Select(x => x.AdvanceRequests.AdvanceSettings.AdvanceSettingName).Distinct().ToList())}");
                GJHDescription.Append($" لحساب الطالب ");
                GJHDescription.Append($" {  Student.STUDENT_NAME + "-" + "ق.هـ" + "( " + Student.NATIONAL_ID + " )"} ");
            });

            GJH GJHmodel = new GJH();
            GJHmodel.OPT_ID = BCDID;
            GJHmodel.GJHStatus = true;
            GJHmodel.USR_ID = UserInAcc.ID;
            GJHmodel.GJHRefranceNumber = "";
            GJHmodel.InsertDate = DateTime.Now;
            GJHmodel.COM_ID = UserInAcc.School_ID;
            GJHmodel.GJHOperationDate = paramPostedDate;
            GJHmodel.GJHSystemNumber = LastSystemNumber;
            GJHmodel.GJHDescription = GJHDescription.ToString();
            GJHmodel.GJHAmount = advancePaymentDetailsList.Select(x => x.NetValue).DefaultIfEmpty(0).Sum();
            GJHmodel.JournalNo = dbAcc.NextJournalEntryNo_Sp(UserInAcc.School_ID, fsyid).FirstOrDefault().Value;

            dbAcc.GJH.Add(GJHmodel);
            dbAcc.SaveChanges();

            var GJDList = new List<GJD>();
            // الطرف المدين من القيد المجمع
            advancePaymentDetailsList.ToList().ForEach(advancePaymentDetailsItem =>
            {
                var advanceSetting = advancePaymentDetailsItem.AdvanceRequests.AdvanceSettings;
                var StudentAcademicData = StudentAcademicDataList.SingleOrDefault(x => x.STUDENT_ID == advancePaymentDetailsItem.AdvancePaymentMaster.Student_Id);

                var DebitGJDDescription = new StringBuilder();

                DebitGJDDescription.Append($" صرف {advanceSetting.AdvanceSettingName} لحساب الطالب");
                DebitGJDDescription.Append($" { StudentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + " ( " + StudentAcademicData.NATIONAL_ID + " )" }");
                DebitGJDDescription.Append($" بتاريخ {advancePaymentDetailsItem.AdvancePaymentMaster.InsertionDate.ToShortDateString()}");

                GJDList.Add(new GJD
                {
                    GJDCreditAmount = 0,
                    GJDDebitAmount = advancePaymentDetailsItem.NetValue,
                    COA_ID = (int)advanceSetting.COAID,
                    GJDDescrition = DebitGJDDescription.ToString(),
                    JOB_ID = null,
                    Fsy_ID = fsyid,
                    GJDStatus = true,
                    GJH_ID = GJHmodel.GJHID,
                    Com_ID = UserInAcc.School_ID
                });
            });

            // الطرف الدائن من القيد المجمع
            distinctAdvancePaymentDetailsCoa.ToList().ForEach(AdvancePaymentDetailsCoaItem =>
            {
                var advancePaymentDetails = advancePaymentDetailsList.Where(x => x.AdvancePaymentMaster.COA_ID == AdvancePaymentDetailsCoaItem.CoaId);

                var CreditGJDDescription = new StringBuilder();
                CreditGJDDescription.Append($" مجموع ما تم صرفه للطلاب المذكورين فيما يخص ");
                CreditGJDDescription.Append($"{ string.Join(" و ", advancePaymentDetails.Select(x => x.AdvanceRequests.AdvanceSettings.AdvanceSettingName).Distinct().ToList())}");

                GJDList.Add(new GJD
                {
                    GJDCreditAmount = advancePaymentDetails.Select(x => x.NetValue).DefaultIfEmpty(0).Sum(),
                    GJDDebitAmount = 0,
                    COA_ID = AdvancePaymentDetailsCoaItem.CoaId,
                    GJDDescrition = CreditGJDDescription.ToString(),
                    JOB_ID = null,
                    Fsy_ID = fsyid,
                    GJDStatus = true,
                    GJH_ID = GJHmodel.GJHID,
                    Com_ID = UserInAcc.School_ID
                });
            });

            var AdvancePaymentMasters = dbSch.AdvancePaymentMaster.Where(f => docMasterIds.Contains(f.ID)).ToList();
            AdvancePaymentMasters.ForEach(AdvancePaymentMaster =>
            {
                AdvancePaymentMaster.IsPosted = true;
                AdvancePaymentMaster.GJH_ID = (int)GJHmodel.GJHID;
                dbSch.Entry(AdvancePaymentMaster).State = EntityState.Modified;
            });

            dbAcc.GJD.AddRange(GJDList);

            var dbSchTask = dbSch.SaveChangesAsync();
            var dbAccTask = dbAcc.SaveChangesAsync();

            Task.WhenAll(dbSchTask, dbAccTask);

            return this.JsonMaxLength("");

        }


        #endregion

        #region شاشة ترحيل القيود المجمعة لسندات صرف الإعانات    

        [HttpGet]
        public ActionResult PaidSubsidiesAggregateJournal()
        {
            var json = new JavaScriptSerializer().Serialize(GetAdvancePaymentPermissions(96).Data);
            var permissions = JsonConvert.DeserializeObject<AdvancePaymentPermission>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }


        #endregion

        #region  شاشة ترحيل القيود المجمعة لسندات تحصيل السلف   
        public ActionResult ReceiveAdvancesAggregateJournal()
        {
            var json = new JavaScriptSerializer().Serialize(GetAdvancePaymentPermissions(97).Data);
            var permissions = JsonConvert.DeserializeObject<AdvancePaymentPermission>(json);

            if (permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        [HttpGet]
        public ActionResult GetDocsOfCollectiblAdvances()
        {
            return this.JsonMaxLength(dbSch.AdvanceReceiveMaster.Where(x => x.IsPosted != true && x.PayrollId == null).ToList().Select(x => new
            {
                x.DocNotes,
                x.DocNumber,
                x.DocHeader,
                MasterID = x.ID,
                DocTotalValue = x.TotalValue,
                InsertionDate = x.InsertionDate.Day + "/" + x.InsertionDate.Month + "/" + x.InsertionDate.Year,
                AccountName = GetCoaName(x.COA_ID),
                StudentAcademicData = dbSch.INTEGRATION_All_Students.Where(p => p.STUDENT_ID == x.Student_Id)
                                                                   .Select(p => new
                                                                   {
                                                                       p.STUDENT_ID,
                                                                       p.STUDENT_NAME,
                                                                       p.FACULTY_NAME,
                                                                       p.NATIONAL_ID
                                                                   }).FirstOrDefault()
            }).ToList());

        }

        [HttpGet]
        public ActionResult GetDocDetailsReceiveAdvance(int masterId, string type)
        {

            var advanceReceiveDetailsList = dbSch.AdvanceReceiveDetails
                                                 .Where(x => x.AdvanceReceiveMaster_Id == masterId);

            var StudentAcademicData = dbSch.INTEGRATION_All_Students
                                           .Where(p => advanceReceiveDetailsList.Any(x => x.AdvanceReceiveMaster.Student_Id == p.STUDENT_ID))
                                           .Select(p => new
                                           {
                                               p.STUDENT_ID,
                                               p.STUDENT_NAME,
                                               p.FACULTY_NAME,
                                               p.NATIONAL_ID
                                           }).FirstOrDefault();

            var AdvanceCoa = advanceReceiveDetailsList.Select(x => x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.COAID.Value).FirstOrDefault();

            var GJDDescription = new StringBuilder();
            var customGJDList = new List<CustomGJD>();

            //==============
            //الجانب الدائن
            //==============
            advanceReceiveDetailsList.ToList().ForEach(x =>
            {
                var advanceName = x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName;

                GJDDescription.Append($" تحصيل قسط ");
                GJDDescription.Append($" {advanceName} ");
                GJDDescription.Append($" من الطالب ");
                GJDDescription.Append($" { StudentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + "( " + StudentAcademicData.NATIONAL_ID + " )"}");
                GJDDescription.Append($" بتاريخ {x.AdvanceReceiveMaster.InsertionDate.ToShortDateString()}");

                customGJDList.Add(new CustomGJD
                {
                    GJDCreditAmount = x.NetValue,
                    GJDDebitAmount = 0.00M,
                    AccountName = GetCoaName(AdvanceCoa),
                    GJDDescrition = GJDDescription.ToString()
                });

            });

            //==============
            //الجانب المدين
            //==============
            var _GJDDescription = new StringBuilder();
            _GJDDescription.Append($" مجموع ما تم تحصيله من الطالب ");
            _GJDDescription.Append($"{  StudentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + "( " + StudentAcademicData.NATIONAL_ID + " )"}");
            _GJDDescription.Append($" فيما يخص قسط ");
            _GJDDescription.Append($"{ string.Join(" و ", advanceReceiveDetailsList.Select(x => x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName).Distinct().ToList())}");

            customGJDList.Add(new CustomGJD
            {
                GJDDebitAmount = customGJDList.Select(x => x.GJDCreditAmount).DefaultIfEmpty(0).Sum(),
                GJDCreditAmount = 0.00M,
                AccountName = GetCoaName(advanceReceiveDetailsList.Select(x => x.AdvanceReceiveMaster.COA_ID).FirstOrDefault()),
                GJDDescrition = _GJDDescription.ToString()
            });

            return this.JsonMaxLength(customGJDList.ToList());
        }

        [HttpPost]
        public ActionResult PostAggregateJournalForReceiveDocs(string DocMasterIds, DateTime paramPostedDate)
        {
            if (paramPostedDate == null)
            {
                return this.JsonMaxLength(@"عفوا لا يمكن إتمام عملية الحفظ حتي يتم إدخال تاريخ الترحيل");
            }
            //===============================================
            // بيانات المستخدم الحالي بداخل نظام الحسابات 
            //===============================================

            var userInAcc = dbAcc.Users.FirstOrDefault(x => x.ID == CurrentUser.AccID);
            if (userInAcc == null)
            {
                return this.JsonMaxLength(@"عفوا لا يمكن إتمام عملية الحفظ حتي يتم ربط بيانات المستخدم بنظام الحسابات العامة");
            }

            int? fsyid = dbAcc.FSY.SingleOrDefault(x => x.COM_ID == userInAcc.School_ID && x.Is_Open == true &&
                                                        x.FSYStartDate <= paramPostedDate && x.FSYEndDate >= paramPostedDate)?.FSYID;
            if (fsyid == null)
            {
                return this.JsonMaxLength($" عفوا لا يمكن إتمام عملية الحفظ حيث لا يوجد أي سنة مالية مفتوحة حاليا بداخل نظام الحسابات العامة " +
                                           "أو ربما تاريخ الترحيل يقع خارج نطاق السنة المفتوحة حاليا");
            }

            //=====================================
            // نوع العملية هنا اللي هى قيد يومية 
            //=====================================
            var operationId = GetOperationId().Value;
            var bcdid = GetBcdId(operationId).BCDID;


            //=============================================================
            // دا رقم العملية الأهم اللي هما بيعتبروها مش متكررة ع الإطلاق 
            //=============================================================
            var lastSystemNumber = GetLastSystemNumber((int)fsyid, operationId);


            //===================================================
            // دي السندات المصروفة اللي هيتم عمل قيد مجمع بيها 
            //===================================================
            var docMasterIds = DocMasterIds.Split(',').Select(x => Convert.ToInt32(x)).ToList();



            var advanceReceiveDetailsList = dbSch.AdvanceReceiveDetails
                                                 .Where(x => docMasterIds.Any(p => p == x.AdvanceReceiveMaster_Id));

            //==============================
            // دي البيانات الأكاديمية للطلاب
            //==============================
            var studentAcademicDataList = dbSch.INTEGRATION_All_Students
                                               .Where(p => advanceReceiveDetailsList.Any(x => x.AdvanceReceiveMaster.Student_Id == p.STUDENT_ID))
                                               .Select(p => new
                                               {
                                                   p.STUDENT_ID,
                                                   p.STUDENT_NAME,
                                                   p.FACULTY_NAME,
                                                   p.NATIONAL_ID
                                               }).Distinct().ToList();

            //======================================================
            // دي الحسابات اللي علي أساسها هيتم عمل القيد المجمع
            //======================================================
            var distinctAdvancePaymentDetailsCoa = advanceReceiveDetailsList.ToList().Select(x => new
            {
                CoaId = x.AdvanceReceiveMaster.COA_ID
            }).Distinct().ToList();

            var GJHDescription = new StringBuilder();
            GJHDescription.Append($" قيد تحصيل أقساط السلف من الطلاب مجمع وبيانه ");
            GJHDescription.Append($" تحصيل قسط ");

            studentAcademicDataList.ForEach(Student =>
            {
                GJHDescription.Append($"{ string.Join(" و ", advanceReceiveDetailsList.Where(x => x.AdvanceReceiveMaster.Student_Id == Student.STUDENT_ID).Select(x => x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName).Distinct().ToList())}");
                GJHDescription.Append($" من الطالب ");
                GJHDescription.Append($"{  Student.STUDENT_NAME + "-" + "ق.هـ" + " ( " + Student.NATIONAL_ID + " ) " }");
            });

            GJH gjHmodel = new GJH();
            gjHmodel.OPT_ID = bcdid;
            gjHmodel.GJHStatus = true;
            gjHmodel.USR_ID = userInAcc.ID;
            gjHmodel.GJHRefranceNumber = "";
            gjHmodel.InsertDate = DateTime.Now;
            gjHmodel.COM_ID = userInAcc.School_ID;

            gjHmodel.GJHOperationDate = paramPostedDate;
            gjHmodel.GJHSystemNumber = lastSystemNumber;
            gjHmodel.GJHDescription = GJHDescription.ToString();
            gjHmodel.GJHAmount = advanceReceiveDetailsList.Select(x => x.NetValue).DefaultIfEmpty(0).Sum();
            gjHmodel.JournalNo = dbAcc.NextJournalEntryNo_Sp(userInAcc.School_ID, fsyid).Single().Value;

            dbAcc.GJH.Add(gjHmodel);
            dbAcc.SaveChanges();

            var GJDList = new List<GJD>();

            // الطرف الدائن من القيد المجمع
            advanceReceiveDetailsList.ToList().ForEach(advanceReceiveDetailsItems =>
            {
                var advanceSetting = advanceReceiveDetailsItems.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings;
                var studentAcademicData = studentAcademicDataList.SingleOrDefault(x => x.STUDENT_ID == advanceReceiveDetailsItems.AdvancePaymentDetails.AdvancePaymentMaster.Student_Id);

                var creditGjdDescription = new StringBuilder();

                creditGjdDescription.Append($" تحصيل قسط ");
                creditGjdDescription.Append($" {advanceSetting.AdvanceSettingName} ");
                creditGjdDescription.Append($" من الطالب ");
                creditGjdDescription.Append($"{  studentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + " ( " + studentAcademicData.NATIONAL_ID + " )" } بتاريخ {advanceReceiveDetailsItems.AdvanceReceiveMaster.InsertionDate}");

                GJDList.Add(new GJD
                {
                    GJDCreditAmount = advanceReceiveDetailsItems.NetValue,
                    GJDDebitAmount = 0,
                    COA_ID = (int)advanceSetting.COAID,
                    GJDDescrition = creditGjdDescription.ToString(),
                    JOB_ID = null,
                    Fsy_ID = fsyid,
                    GJDStatus = true,
                    GJH_ID = gjHmodel.GJHID,
                    Com_ID = userInAcc.School_ID
                });
            });

            // الطرف المدين من القيد المجمع
            distinctAdvancePaymentDetailsCoa.ToList().ForEach(AdvancePaymentDetailsCoaItem =>
            {
                var advanceReceiveDetails = advanceReceiveDetailsList.Where(x => x.AdvanceReceiveMaster.COA_ID == AdvancePaymentDetailsCoaItem.CoaId);

                var debitGjdDescription = new StringBuilder();
                debitGjdDescription.Append($" مجموع ما تم تحصيله من قسط ");
                debitGjdDescription.Append($"{ string.Join(" و ", advanceReceiveDetails.Select(x => x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName).ToList())}");

                GJDList.Add(new GJD
                {
                    GJDCreditAmount = 0,
                    GJDDebitAmount = advanceReceiveDetails.Select(x => x.NetValue).DefaultIfEmpty(0).Sum(),
                    COA_ID = AdvancePaymentDetailsCoaItem.CoaId,
                    GJDDescrition = debitGjdDescription.ToString(),
                    JOB_ID = null,
                    Fsy_ID = fsyid,
                    GJDStatus = true,
                    GJH_ID = gjHmodel.GJHID,
                    Com_ID = userInAcc.School_ID
                });
            });

            var advanceReceiveMasters = dbSch.AdvanceReceiveMaster.Where(f => docMasterIds.Contains(f.ID)).ToList();
            advanceReceiveMasters.ForEach(advanceReceiveMaster =>
            {
                advanceReceiveMaster.IsPosted = true;
                advanceReceiveMaster.GJH_ID = (int)gjHmodel.GJHID;
                dbSch.Entry(advanceReceiveMaster).State = EntityState.Modified;
            });

            dbAcc.GJD.AddRange(GJDList);

            var dbSchTask = dbSch.SaveChangesAsync();
            var dbAccTask = dbAcc.SaveChangesAsync();

            Task.WhenAll(dbSchTask, dbAccTask);

            return this.JsonMaxLength("");
        }

        [HttpGet]
        public ActionResult PreviewReceiveDocs(string DocMasterIds)
        {
            //===================================================
            // دي السندات المصروفة اللي هيتم عمل قيد مجمع بيها 
            //===================================================
            var docMasterIds = DocMasterIds.Split(',').Select(x => Convert.ToInt32(x)).ToList();


            var advanceReceiveDetailsList = dbSch.AdvanceReceiveDetails
                                                 .Where(x => docMasterIds.Any(p => p == x.AdvanceReceiveMaster_Id));

            //==============================
            // دي البيانات الأكاديمية للطلاب
            //==============================
            var studentAcademicDataList = dbSch.INTEGRATION_All_Students
                                               .Where(p => advanceReceiveDetailsList.Any(x => x.AdvanceReceiveMaster.Student_Id == p.STUDENT_ID))
                                               .Select(p => new
                                               {
                                                   p.STUDENT_ID,
                                                   p.STUDENT_NAME,
                                                   p.FACULTY_NAME,
                                                   p.NATIONAL_ID
                                               }).Distinct().ToList();

            //======================================================
            // دي الحسابات اللي علي أساسها هيتم عمل القيد المجمع
            //======================================================
            var distinctAdvancePaymentDetailsCoa = advanceReceiveDetailsList.ToList().Select(x => new
            {
                CoaId = x.AdvanceReceiveMaster.COA_ID
            }).Distinct().ToList();

            var GJHDescription = new StringBuilder();
            GJHDescription.Append($" قيد تحصيل أقساط السلف من الطلاب مجمع وبيانه ");
            GJHDescription.Append($" تحصيل قسط ");

            studentAcademicDataList.ForEach(Student =>
            {
                GJHDescription.Append($"{ string.Join(" و ", advanceReceiveDetailsList.Where(x => x.AdvanceReceiveMaster.Student_Id == Student.STUDENT_ID).Select(x => x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName).Distinct().ToList())}");
                GJHDescription.Append($" من الطالب ");
                GJHDescription.Append($"{  Student.STUDENT_NAME + "-" + "ق.هـ" + " ( " + Student.NATIONAL_ID + " ) " }");
            });

            GJH gjHmodel = new GJH();
            gjHmodel.GJHDescription = GJHDescription.ToString();
            gjHmodel.GJHAmount = advanceReceiveDetailsList.Select(x => x.NetValue).DefaultIfEmpty(0).Sum();

            var GJDList = new List<GJD>();

            // الطرف الدائن من القيد المجمع
            advanceReceiveDetailsList.ToList().ForEach(advanceReceiveDetailsItems =>
            {
                var advanceSetting = advanceReceiveDetailsItems.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings;
                var studentAcademicData = studentAcademicDataList.SingleOrDefault(x => x.STUDENT_ID == advanceReceiveDetailsItems.AdvancePaymentDetails.AdvancePaymentMaster.Student_Id);

                var CreditGjdDescription = new StringBuilder();

                CreditGjdDescription.Append($" تحصيل قسط ");
                CreditGjdDescription.Append($" {advanceSetting.AdvanceSettingName} ");
                CreditGjdDescription.Append($" من الطالب ");
                CreditGjdDescription.Append($"{  studentAcademicData.STUDENT_NAME + "-" + "ق.هـ" + " ( " + studentAcademicData.NATIONAL_ID + " )" } بتاريخ {advanceReceiveDetailsItems.AdvanceReceiveMaster.InsertionDate.ToShortDateString()}");

                GJDList.Add(new GJD
                {
                    GJDCreditAmount = advanceReceiveDetailsItems.NetValue,
                    GJDDebitAmount = 0,
                    COA_ID = (int)advanceSetting.COAID,
                    GJDDescrition = CreditGjdDescription.ToString(),
                    GJH_ID = gjHmodel.GJHID,
                });
            });

            // الطرف المدين من القيد المجمع
            distinctAdvancePaymentDetailsCoa.ToList().ForEach(AdvancePaymentDetailsCoaItem =>
            {
                var advanceReceiveDetails = advanceReceiveDetailsList.Where(x => x.AdvanceReceiveMaster.COA_ID == AdvancePaymentDetailsCoaItem.CoaId);

                var creditGjdDescription = new StringBuilder();
                creditGjdDescription.Append($" مجموع ما تم تحصيله من قسط ");
                creditGjdDescription.Append($"{ string.Join(" و ", advanceReceiveDetails.Select(x => x.AdvancePaymentDetails.AdvanceRequests.AdvanceSettings.AdvanceSettingName).ToList())}");

                GJDList.Add(new GJD
                {
                    GJDCreditAmount = 0,
                    GJDDebitAmount = advanceReceiveDetails.Select(x => x.NetValue).DefaultIfEmpty(0).Sum(),
                    COA_ID = AdvancePaymentDetailsCoaItem.CoaId,
                    GJDDescrition = creditGjdDescription.ToString(),
                });
            });

            var gjdPreviewList = GJDList.Select(x => new GjdPreview
            {
                AccountName = dbAcc.COA.FirstOrDefault(c => c.COAID == x.COA_ID).COADescription,
                Dr = x.GJDDebitAmount,
                Cr = x.GJDCreditAmount,
                Notes = x.GJDDescrition
            });

            return this.JsonMaxLength(gjdPreviewList);
        }


        #endregion



        #region متابعة السلف والاعانات وحالات الاعتماد

        public ActionResult AdvancesRequestTraking()
        {
            var permissions = GetPermissionsFn(104);
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

        [HttpPost]
        public ActionResult GetAdvancesRequestTraking(AdvancesRequestTrakingVM model)
        {
            if(model.RequestValue == 1)
            {
                var lst = dbSch.AdvanceRequests.Where(a=>a.IsCanceled ==true
                
                
                ).Select(x => new
                {
                    MobileNumber = dbSch.UniversityStudents.FirstOrDefault(c => c.Student_ID == x.Student_Id).MobileNumber,
                    STUDENT_ID = x.Student_Id,
                    STUDENT_NAME = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).STUDENT_NAME,
                    NATIONAL_ID = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).NATIONAL_ID,
                    NATIONALITY_DESC = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).NATIONALITY_DESC,
                    AdvanceRequestId = x.ID,
                    AdvanceName = x.AdvanceSettings.AdvanceSettingName,
                    RequestedDate = x.InsertionDate.Hour + ":" + x.InsertionDate.Minute + " " + x.InsertionDate.Day + "/" + x.InsertionDate.Month + "/" + x.InsertionDate.Year,
                    InsertionDate = x.InsertionDate,
                    AdvanceSettingId = x.AdvanceSettings.AdvanceSettingId,
                    AdvanceType = x.AdvanceSettings.AdvanceType,
                    x.RequestedValue,
                    x.ApprovedValue,
                    CreatedBy = x.User_Id == null ? "الطالب" : x.DashBoard_Users.Name,
                    AdvanceIsPaid = x.AdvancePaymentDetails.Count() > 0 ? "تم الصرف" : "لم يصرف",
                    ApprovedStatus = x.ApprovedbyID != null ? "معتمد" : x.RefusedbyID != null ? "مرفوض" : "تحت المراجعة",
                    ApprovedPhase = x.AdvanceApprovedPhases.OrderByDescending(c => c.AdvancePhase_ID).FirstOrDefault().AdvancePhases.PhaseName
                }).Where(x =>
                (model.StudentId == null || x.STUDENT_ID == model.StudentId) &&
                (model.AdvancesType == null || x.AdvanceType == model.AdvancesType) &&
                (model.DateFrom == null || DbFunctions.TruncateTime(x.InsertionDate) >= model.DateFrom) &&
                (model.DateTo == null || DbFunctions.TruncateTime(x.InsertionDate) <= model.DateTo)).ToList();

                return this.JsonMaxLength(lst);
            }else if(model.RequestValue == 2)
            {
                var lst = dbSch.AdvanceRequests.Where(a => a.RequestedValue!=null && a.IsCanceled!=true &&a.User_Id ==null).Select(x => new
                {
                    MobileNumber = dbSch.UniversityStudents.FirstOrDefault(c => c.Student_ID == x.Student_Id).MobileNumber,
                    STUDENT_ID = x.Student_Id,
                    STUDENT_NAME = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).STUDENT_NAME,
                    NATIONAL_ID = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).NATIONAL_ID,
                    NATIONALITY_DESC = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).NATIONALITY_DESC,
                    AdvanceRequestId = x.ID,
                    AdvanceName = x.AdvanceSettings.AdvanceSettingName,
                    RequestedDate = x.InsertionDate.Hour + ":" + x.InsertionDate.Minute + " " + x.InsertionDate.Day + "/" + x.InsertionDate.Month + "/" + x.InsertionDate.Year,
                    InsertionDate = x.InsertionDate,
                    AdvanceSettingId = x.AdvanceSettings.AdvanceSettingId,
                    AdvanceType = x.AdvanceSettings.AdvanceType,
                    x.RequestedValue,
                    x.ApprovedValue,
                    CreatedBy = x.User_Id == null ? "الطالب" : x.DashBoard_Users.Name,
                    AdvanceIsPaid = x.AdvancePaymentDetails.Count() > 0 ? "تم الصرف" : "لم يصرف",
                    ApprovedStatus = x.ApprovedbyID != null ? "معتمد" : x.RefusedbyID != null ? "مرفوض" : "تحت المراجعة",
                    ApprovedPhase = x.AdvanceApprovedPhases.OrderByDescending(c => c.AdvancePhase_ID).FirstOrDefault().AdvancePhases.PhaseName
                }).Where(x =>
                (model.StudentId == null || x.STUDENT_ID == model.StudentId) &&
                (model.AdvancesType == null || x.AdvanceType == model.AdvancesType) &&
                (model.DateFrom == null || DbFunctions.TruncateTime(x.InsertionDate) >= model.DateFrom) &&
                (model.DateTo == null || DbFunctions.TruncateTime(x.InsertionDate) <= model.DateTo)).ToList();
                return this.JsonMaxLength(lst);
            }

            var data = dbSch.AdvanceRequests.Where(x=>x.IsCanceled!=true && x.User_Id == null).Select(x => new
            {
                MobileNumber = dbSch.UniversityStudents.FirstOrDefault(c => c.Student_ID == x.Student_Id).MobileNumber,
                STUDENT_ID = x.Student_Id,
                STUDENT_NAME = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).STUDENT_NAME,
                NATIONAL_ID = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).NATIONAL_ID,
                NATIONALITY_DESC = dbSch.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).NATIONALITY_DESC,
                AdvanceRequestId = x.ID,
                AdvanceName = x.AdvanceSettings.AdvanceSettingName,
                RequestedDate = x.InsertionDate.Hour + ":" + x.InsertionDate.Minute + " " + x.InsertionDate.Day + "/" + x.InsertionDate.Month + "/" + x.InsertionDate.Year,
                InsertionDate = x.InsertionDate,
                AdvanceSettingId = x.AdvanceSettings.AdvanceSettingId,
                AdvanceType = x.AdvanceSettings.AdvanceType,
                x.RequestedValue,
                x.ApprovedValue,
                CreatedBy = x.User_Id == null ? "الطالب" : x.DashBoard_Users.Name,
                AdvanceIsPaid = x.AdvancePaymentDetails.Count() > 0 ? "تم الصرف" : "لم يصرف",
                ApprovedStatus = x.ApprovedbyID != null ? "معتمد" : x.RefusedbyID != null ? "مرفوض" : "تحت المراجعة",
                ApprovedPhase = x.AdvanceApprovedPhases.OrderByDescending(c => c.AdvancePhase_ID).FirstOrDefault().AdvancePhases.PhaseName
            }).Where(x =>
            (model.StudentId == null || x.STUDENT_ID == model.StudentId) &&
            (model.AdvancesType == null || x.AdvanceType == model.AdvancesType) &&
            (model.DateFrom == null || DbFunctions.TruncateTime(x.InsertionDate) >= model.DateFrom) &&
            (model.DateTo == null || DbFunctions.TruncateTime(x.InsertionDate) <= model.DateTo)).ToList();

            return this.JsonMaxLength(data);
        }


        public class AdvancesRequestTrakingVM
        {
            public decimal? StudentId { get; set; }
            public DateTime? DateFrom { get; set; }
            public DateTime? DateTo { get; set; }
            public string AdvancesType { get; set; }
            public int RequestValue { get; set; }
        }


        #endregion

        #region Permissions       

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

        private int CurrentStudentId
        {
            get
            {
                var user = HttpContext.Session["UserId"] as DashBoard_Users;

                if (user != null && user.ID != 0)
                {
                    if (user.IsStudent == true)
                    {
                        return Convert.ToInt32(user.Username);
                    }
                    return 0;
                }
                else
                {
                    return 0;
                }
            }

        }

        [HttpPost]
        public JsonResult GetPermissionsJson(int screenId)
        {
            return this.JsonMaxLength(GetPermissionsFn(screenId));
        }

        public Permissions GetPermissionsFn(int screenId)
        {
            if (CurrentUser == null || CurrentUser?.ID == 0)
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

        #endregion
    }

}

public class CustomGJD
{
    public string RandomKey { get; set; }
    public decimal GJDDebitAmount { get; set; }
    public decimal GJDCreditAmount { get; set; }
    public string AccountName { get; set; }
    public string GJDDescrition { get; set; }
}
public class GjdPreview
{
    public string AccountName { get; set; }
    public decimal Dr { get; set; }
    public decimal Cr { get; set; }
    public string Notes { get; set; }

}

#region السلف نظام قديم ملغى

//private EsolERPEntities dbAcc = new EsolERPEntities();
//private iu_projectEntities db_iu = new iu_projectEntities();
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]

//public ActionResult Index()
//{
//    return View();
//}
//[HttpGet]
//public ActionResult GetEmployees(string searchValue, int skip, int take)
//{
//    var name = "";
//    if (!string.IsNullOrEmpty(searchValue))
//    {
//        name = searchValue.Substring(1, searchValue.Length - 2);
//    }
//    var students = dbSch.Employees.Where(x => x.empFileNum != null && x.Name.Contains(name)).OrderBy(x => x.Name)
//        .Distinct()
//        .Skip(skip)
//        .Take(take)
//        .Select(x =>
//            new SelectListItem
//            {
//                Text = x.Name,
//                Value = x.ID.ToString()
//            });
//    return Json(students.ToList(), JsonRequestBehavior.AllowGet);
//}
//[HttpGet]
//public ActionResult GetAdvancesTypes()
//{
//    var types = dbSch.AdvanceSettings.Select(x => new SelectListItem
//    {
//        Text = x.AdvanceSettingName.ToString(),
//        Value = x.AdvanceSettingId.ToString()
//    });
//    return Json(types.ToList(), JsonRequestBehavior.AllowGet);
//}
//[HttpGet]
//public ActionResult GetActiveYearMonthes()
//{
//    var activeYear = dbSch.Year.FirstOrDefault(x => x.IsActive.Value);
//    var monthes = new List<Month>();
//    if (activeYear != null)
//    {
//        monthes = dbSch.Month.Where(x => x.YearID == activeYear.ID).ToList();
//    }
//    return new JsonNetResult
//    {
//        Data = monthes,
//        JsonRequestBehavior = JsonRequestBehavior.AllowGet
//    };
//}
////احتساب تلقائى
//[HttpPost]
//public ActionResult AddAutoCalcAdvance(int studentId, int advanceType, DateTime dueDate, DateTime date,
//    int value, int[] monthsIds)
//{
//    var employee = dbSch.Employees.SingleOrDefault(x => x.ID == studentId);
//    if (employee == null)
//    {
//        return Json("عفواً، لا يوجد طالب بهذه البيانات");
//    }
//    var advanceSettings = dbSch.AdvanceSettings.SingleOrDefault(x => x.AdvanceSettingId == advanceType);
//    var monthlyPremium = value / monthsIds.Length;
//    var advanceRequest = new AdvanceRequest
//    {
//        EmpID = studentId,
//        AdvanceValue = value,
//        DueDate = dueDate,
//        MonthsNumbers = string.Join(",", monthsIds),
//        MonthlyPremium = monthlyPremium,
//        Date = date,
//        AdvanceName = $"{advanceSettings?.AdvanceSettingName}_ للطالب_{employee.Name}"
//    };
//    dbSch.AdvanceRequest.Add(advanceRequest);
//    dbSch.SaveChanges();
//    foreach (var monthId in monthsIds)
//    {
//        var advanceRequestDetails = new AdvanceRequestDetails
//        {
//            AdvanceRequestId = advanceRequest.ID,
//            MonthId = monthId,
//            MonthlyValue = monthlyPremium,
//            AdvanceId = null
//        };
//        dbSch.AdvanceRequestDetails.Add(advanceRequestDetails);
//    }
//    dbSch.SaveChanges();
//    return Json("");
//}
////احتساب مخصص
//[HttpPost]
//public ActionResult AddCustomCalcAdvance(int studentId, int advanceType, DateTime dueDate, DateTime date,
//    int value, MonthlyPremium[] monthlyPremiums)
//{
//    var employee = dbSch.Employees.SingleOrDefault(x => x.ID == studentId);
//    if (employee == null)
//    {
//        return Json("عفواً، لا يوجد طالب بهذه البيانات");
//    }
//    var advanceSettings = dbSch.AdvanceSettings.SingleOrDefault(x => x.AdvanceSettingId == advanceType);
//    var advanceRequest = new AdvanceRequest
//    {
//        EmpID = studentId,
//        AdvanceValue = value,
//        DueDate = dueDate,
//        Date = date,
//        MonthsNumbers = string.Join(",", monthlyPremiums.Select(x => x.MonthId)),
//        AdvanceName = $"{advanceSettings?.AdvanceSettingName}_ للطالب_{employee.Name}",
//    };
//    dbSch.AdvanceRequest.Add(advanceRequest);
//    dbSch.SaveChanges();
//    foreach (var monthlyPremium in monthlyPremiums)
//    {
//        var advanceRequestDetails = new AdvanceRequestDetails
//        {
//            AdvanceRequestId = advanceRequest.ID,
//            MonthId = monthlyPremium.MonthId,
//            MonthlyValue = monthlyPremium.Value,
//            AdvanceId = null
//        };
//        dbSch.AdvanceRequestDetails.Add(advanceRequestDetails);
//    }
//    dbSch.SaveChanges();
//    return Json("");
//}


//[HttpPost]
//public ActionResult AddAdvance(string studentId, int advanceType, DateTime advanceDate, int value)
//{
//    var user = HttpContext.Session["UserId"] as DashBoard_Users;

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json("برجاء إختيار الطالب أولا");
//    }
//    var studentID = Convert.ToInt32(studentId);
//    //var employee = dbSch.Employees.SingleOrDefault(x => x.empFileNum == studentId);
//    //if (employee == null)
//    //{
//    //    return Json("عفواً لا يوجد بيانات لهذا الطالب فى الـ HR");
//    //}

//    //التحقق من انه ليس له طلبات معلقه حالياً
//    var anyHoldingRequest = dbSch.AdvanceRequest.Any(ar =>
//        ar.EmpID == studentID && ar.ApprovedBy == null && ar.RefusedBy == null);
//    if (anyHoldingRequest)
//    {
//        return Json("عفواً لايمكن طلب سلفة جديد، هناك طلب قيد الانتظار");
//    }

//    //التحقق من انه ليس عليه اقساط حالياً
//    //1. الحصول على اخر سلفة
//    var lastAdvance = dbSch.Advances.Where(x => x.EmpID == studentID).OrderByDescending(x => x.Date).FirstOrDefault();
//    if (lastAdvance != null)
//    {
//        //2. جمع الاقساط اللى سددها الطالب لاخر سلفة
//        var sumOfPaidPremiums =
//            dbSch.AdvancesPremiums.Where(x => x.AdvID == lastAdvance.ID).Select(x => x.NetValue)
//                .DefaultIfEmpty(0).Sum(x => x.Value);

//        //3. التحقق من ان مجموع الاقساط المسددة يساوى مبلغ السلفة
//        var difference = lastAdvance.AdvanceValue - sumOfPaidPremiums;
//        if (difference != 0)
//        {
//            return Json(
//                $"عفواً، لا يمكن اضافة طلب سلفة لهذا الطالب بسبب وجود اقساط متأخرة، قيمة اخر سلفة {lastAdvance.AdvanceValue}");
//        }
//    }


//    //حتى الأن كله تمام ومن حق الطالب يطلب سلفة
//    var advanceSettings = dbSch.AdvanceSettings.SingleOrDefault(x => x.AdvanceSettingId == advanceType);

//    //قيمة القسط الشهرى 200
//    var numberOfMonthes = value / 200;
//    var remainder = value % 200;
//    var monthsIds = new List<int>();
//    for (var i = 1; i <= numberOfMonthes; i++)
//    {

//        var month = new DateTime(advanceDate.AddMonths(1).Year, advanceDate.AddMonths(1).Month, 1);
//        var monthHr = dbSch.Month.SingleOrDefault(x => x.StartDate == month);
//        if (monthHr != null)
//        {
//            monthsIds.Add(monthHr.ID);
//        }
//    }

//    switch (remainder)
//    {
//        //للارقام 250-450-650-850
//        case 50:
//            {
//                var months = advanceDate.AddMonths(monthsIds.Count + 1);
//                var month = new DateTime(months.Year, months.Month, 1);
//                var monthHr = dbSch.Month.SingleOrDefault(x => x.StartDate == month);
//                if (monthHr != null)
//                {
//                    monthsIds.Add(monthHr.ID);
//                }

//                break;
//            }
//        //للارقام 100 - 300 - 500 - 700 - 900
//        case 100:
//            {
//                var months = advanceDate.AddMonths(monthsIds.Count + 1);
//                var month = new DateTime(months.Year, months.Month, 1);
//                var monthHr = dbSch.Month.SingleOrDefault(x => x.StartDate == month);
//                if (monthHr != null)
//                {
//                    monthsIds.Add(monthHr.ID);
//                }

//                break;
//            }
//        //للارقام 150 - 350 - 550 - 750 - 950
//        case 150:
//            {
//                var months = advanceDate.AddMonths(monthsIds.Count + 1);
//                var month = new DateTime(months.Year, months.Month, 1);
//                var monthHr = dbSch.Month.SingleOrDefault(x => x.StartDate == month);
//                if (monthHr != null)
//                {
//                    monthsIds.Add(monthHr.ID);
//                }

//                break;
//            }
//    }

//    var student = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentID);

//    var advanceRequest = new AdvanceRequest
//    {
//        EmpID = studentID,
//        AdvanceValue = value,
//        RequestType = "A", //نوع الطلب سلفة
//        DueDate = new DateTime(advanceDate.AddMonths(1).Year, advanceDate.AddMonths(1).Month, 1),
//        MonthsNumbers = string.Join(",", monthsIds),
//        MonthlyPremium = 200,
//        Date = advanceDate,
//        AdvanceSettingId = advanceSettings?.AdvanceSettingId,
//        AdvanceName = $"{advanceSettings?.AdvanceSettingName}_ للطالب_{student.STUDENT_NAME}"
//    };
//    dbSch.AdvanceRequest.Add(advanceRequest);
//    dbSch.SaveChanges();

//    for (var i = 1; i <= monthsIds.Count; i++)
//    {
//        //شهر بداية الخصم
//        var month = new DateTime(advanceDate.AddMonths(1).Year, advanceDate.AddMonths(1).Month, 1);
//        var monthHr = dbSch.Month.SingleOrDefault(x => x.StartDate == month);
//        if (monthHr != null)
//        {
//            var advanceRequestDetails = new AdvanceRequestDetails();
//            if (remainder == 50 && i == monthsIds.Count)
//            {
//                //للارقام 250-350-450-550-650-750-850-950

//                advanceRequestDetails.AdvanceRequestId = advanceRequest.ID;
//                advanceRequestDetails.MonthId = monthHr.ID;
//                advanceRequestDetails.MonthlyValue = 50;
//                advanceRequestDetails.AdvanceId = null;
//            }
//            else if (remainder == 100 && i == monthsIds.Count)
//            {
//                //للارقام 100و300و500و700و900

//                advanceRequestDetails.AdvanceRequestId = advanceRequest.ID;
//                advanceRequestDetails.MonthId = monthHr.ID;
//                advanceRequestDetails.MonthlyValue = 100;
//                advanceRequestDetails.AdvanceId = null;
//            }
//            else if (remainder == 150 && i == monthsIds.Count)
//            {
//                //للارقام 150
//                advanceRequestDetails.AdvanceRequestId = advanceRequest.ID;
//                advanceRequestDetails.MonthId = monthHr.ID;
//                advanceRequestDetails.MonthlyValue = 150;
//                advanceRequestDetails.AdvanceId = null;
//            }
//            else
//            {
//                advanceRequestDetails = new AdvanceRequestDetails
//                {
//                    AdvanceRequestId = advanceRequest.ID,
//                    MonthId = monthHr.ID,
//                    MonthlyValue = 200,
//                    AdvanceId = null
//                };
//            }

//            dbSch.AdvanceRequestDetails.Add(advanceRequestDetails);
//        }
//    }

//    dbSch.SaveChanges();
//    return Json("");
//}



//[HttpGet]
//public ActionResult HasPendingAdvancePayment(string studentId)
//{

//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json("برجاء إختيار الطالب أولا");
//    }
//    var studentID = Convert.ToInt32(studentId);

//    //الطالب عنده اقساط متاخرة وبسدد فيها
//    // var emp = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    var anyPendingAdvancePayment = false;
//    anyPendingAdvancePayment = dbSch.Advances.Any(a => a.EmpID == studentID && a.IsValied == false);

//    return Json(anyPendingAdvancePayment, JsonRequestBehavior.AllowGet);
//}

//[HttpGet]
//public ActionResult HasAnyAdvanceRequest(string studentId)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json(true, JsonRequestBehavior.AllowGet);
//    }
//    var studentID = Convert.ToInt32(studentId);

//    //هل الطالب مقدم على طلب سلفة
//    //هذا الطلب يمكن ان يكون معلق او الطالب بيسدد فى قرض الان
//    // فى كلتا الحالتين لا يسمح له بطلب قرض جديد

//    //var emp = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    //اى طلبات مؤجلة لم يتم رفضها او قبولها
//    //طلب معلق
//    var result = true;
//    //if (emp == null)
//    //{
//    //    return Json(true, JsonRequestBehavior.AllowGet);
//    //}

//    {
//        var anyPendingAdvanceRequest =
//            dbSch.AdvanceRequest.Any(ar => ar.EmpID == studentID && ar.ApprovedBy == null && ar.RefusedBy == null);
//        //طلب تم الموافقة عليه ويتم سداده
//        var runningAdvance = dbSch.Advances.Any(x => x.EmpID == studentID && x.IsValied == false);
//        result = anyPendingAdvanceRequest || runningAdvance;
//    }
//    return Json(result, JsonRequestBehavior.AllowGet);
//}





//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]
//public ActionResult IndexNew()
//{
//    return View();
//}



//[HttpGet]
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]
//public ActionResult Search(int? Nationality, int? Degree, int? Level, decimal? StudentId, string NationalId,
//    DataSourceLoadOptions loadOptions)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }
//    dbSch.Database.CommandTimeout = 6 * 60;
//    dbSch.Database.CommandTimeout = 6 * 60;
//    var json = new JavaScriptSerializer().Serialize(GetAdvancePermissions(27).Data);
//    var permissions = JsonConvert.DeserializeObject<AdvancePermissions>(json);
//    LoadResult loadResult;
//    if (NationalId == "")
//    {
//        NationalId = null;
//    }

//    if (CurrentUser != null && CurrentUser.IsStudent)
//    {
//        StudentId = decimal.Parse(CurrentUser.ID.ToString());
//    }

//    if (permissions.Exception)
//    {
//        if (Nationality == null && NationalId == null && Degree == null && Level == null && StudentId == null)
//        {
//            loadResult = DataSourceLoader.Load(new List<INTEGRATION_All_Students>(), loadOptions);
//            return Json(loadResult, JsonRequestBehavior.AllowGet);
//        }
//        //الطلبة اللى عليه اقساط حالية

//        //له صلاحية اسثتناء الشروط
//        if (StudentId == null)
//        {
//            var filteredStudents = dbSch.INTEGRATION_All_Students
//                .Where(x =>
//                    x.STATUS_CODE == 1 &&
//                    (NationalId == null || x.NATIONAL_ID == NationalId) &&
//                    (Nationality == null || x.NATIONALITY_CODE == Nationality) &&
//                    (Degree == null || x.DEGREE_CODE == Degree) &&
//                    (Level == null && x.LEVEL_CODE != null || x.LEVEL_CODE == Level))
//                .Distinct().Select(x => new
//                {
//                    x.STUDENT_ID,
//                    x.STUDENT_NAME,
//                    x.NATIONAL_ID,
//                    x.DEGREE_DESC,
//                    x.STATUS_CODE,
//                    x.NATIONALITY_CODE,
//                    x.DEGREE_CODE,
//                    x.LEVEL_CODE,
//                    x.LEVEL_DESC,
//                    GPA = dbSch.ST_AcademicData.Where(y => y.student_id == x.STUDENT_ID && y.cum_gpa != null)
//                        .OrderByDescending(y => y.semester).FirstOrDefault().cum_gpa
//                }).OrderBy(x => x.STUDENT_NAME);
//            loadResult = DataSourceLoader.Load(filteredStudents, loadOptions);
//        }
//        else
//        {
//            //بحث بالرقم الاكاديمى
//            var filteredStudents = dbSch.INTEGRATION_All_Students
//                .Where(x =>
//                    (StudentId == null || x.STUDENT_ID == StudentId) &&
//                    (NationalId == null || x.NATIONAL_ID == NationalId)
//                    && x.STATUS_CODE == 1)
//                .Distinct().Select(x => new
//                {
//                    x.STUDENT_ID,
//                    x.STUDENT_NAME,
//                    x.NATIONAL_ID,
//                    x.DEGREE_DESC,
//                    x.STATUS_CODE,
//                    x.NATIONALITY_CODE,
//                    x.DEGREE_CODE,
//                    x.LEVEL_CODE,
//                    x.LEVEL_DESC,
//                    GPA = dbSch.ST_AcademicData.Where(y => y.student_id == x.STUDENT_ID && y.cum_gpa != null)
//                        .OrderByDescending(y => y.semester).FirstOrDefault().cum_gpa
//                }).OrderBy(x => x.STUDENT_NAME);
//            loadResult = DataSourceLoader.Load(filteredStudents, loadOptions);
//        }

//        return Json(loadResult, JsonRequestBehavior.AllowGet);
//    }

//    //العادى
//    //only students with CumGPA >=2.5
//    // levels : 2-7 && 9-10
//    var studentIdsWithGpa =
//        dbSch.ST_AcademicData.Where(x => x.cum_gpa >= 2.5m).Select(x => x.student_id);
//    if (StudentId == null)
//    {
//        var filteredStudents = dbSch.INTEGRATION_All_Students.Where(x =>
//                (NationalId == null || x.NATIONAL_ID == NationalId) &&
//                x.STATUS_CODE == 1 &&
//                studentIdsWithGpa.Contains(x.STUDENT_ID) &&
//                x.LEVEL_CODE >= 2 && x.LEVEL_CODE <= 7 ||
//                x.LEVEL_CODE >= 9 && x.LEVEL_CODE <= 10 &&
//                (Nationality == null || x.NATIONALITY_CODE == Nationality) &&
//                (Degree == null || x.DEGREE_CODE == Degree) &&
//                (Level == null && x.LEVEL_CODE != null || x.LEVEL_CODE == Level))
//            .Distinct().Select(x => new
//            {
//                x.STUDENT_ID,
//                x.STUDENT_NAME,
//                x.NATIONAL_ID,
//                x.DEGREE_DESC,
//                x.STATUS_CODE,
//                x.NATIONALITY_CODE,
//                x.DEGREE_CODE,
//                x.LEVEL_CODE,
//                x.LEVEL_DESC,
//                GPA = dbSch.ST_AcademicData.Where(y => y.student_id == x.STUDENT_ID && y.cum_gpa != null)
//                    .OrderByDescending(y => y.semester).FirstOrDefault().cum_gpa
//            }).OrderBy(x => x.STUDENT_NAME);


//        var query = dbSch.INTEGRATION_All_Students.Where(x =>
//                (NationalId == null || x.NATIONAL_ID == NationalId) &&
//                x.STATUS_CODE == 1 &&
//                studentIdsWithGpa.Contains(x.STUDENT_ID) &&
//                x.LEVEL_CODE >= 2 && x.LEVEL_CODE <= 7 ||
//                x.LEVEL_CODE >= 9 && x.LEVEL_CODE <= 10 &&
//                (Nationality == null || x.NATIONALITY_CODE == Nationality) &&
//                (Degree == null || x.DEGREE_CODE == Degree) &&
//                (Level == null && x.LEVEL_CODE != null || x.LEVEL_CODE == Level))
//            .Distinct().Select(x => new
//            {
//                x.STUDENT_ID,
//                x.STUDENT_NAME,
//                x.NATIONAL_ID,
//                x.DEGREE_DESC,
//                x.STATUS_CODE,
//                x.NATIONALITY_CODE,
//                x.DEGREE_CODE,
//                x.LEVEL_CODE,
//                x.LEVEL_DESC,
//                GPA = dbSch.ST_AcademicData.Where(y => y.student_id == x.STUDENT_ID && y.cum_gpa != null)
//                    .OrderByDescending(y => y.semester).FirstOrDefault().cum_gpa
//            }).OrderBy(x => x.STUDENT_NAME).ToString();

//        loadResult = DataSourceLoader.Load(filteredStudents, loadOptions);
//    }
//    else
//    {
//        //بحث بالرقم الاكاديمى
//        var filteredStudents = dbSch.INTEGRATION_All_Students.Where(x =>
//                x.STUDENT_ID == StudentId && x.STATUS_CODE == 1)
//            .Distinct().Select(x => new
//            {
//                x.STUDENT_ID,
//                x.STUDENT_NAME,
//                x.NATIONAL_ID,
//                x.DEGREE_DESC,
//                x.STATUS_CODE,
//                x.NATIONALITY_CODE,
//                x.DEGREE_CODE,
//                x.LEVEL_CODE,
//                x.LEVEL_DESC,
//                GPA = dbSch.ST_AcademicData.Where(y => y.student_id == x.STUDENT_ID && y.cum_gpa != null)
//                    .OrderByDescending(y => y.semester).FirstOrDefault().cum_gpa
//            }).OrderBy(x => x.STUDENT_NAME);
//        loadResult = DataSourceLoader.Load(filteredStudents, loadOptions);
//    }

//    return Json(loadResult, JsonRequestBehavior.AllowGet);
//}




//[HttpGet]
//public ActionResult GetAdvanceForEdit(string studentId)
//{
//    // var userId = HttpContext.Session["UserId"];
//    //if (userId == null)
//    //{
//    //    RedirectToAction("Login", "Login");
//    //}

//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return null;
//    }
//    var studentID = Convert.ToInt32(studentId);

//    //var emp = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    //if (emp == null)
//    //{
//    //    return null;
//    //}

//    //الحصول على اخر طلب قرض
//    var advanceRequest =
//        dbSch.AdvanceRequest.Where(x => x.EmpID == studentID && x.ApprovedBy == null && x.RefusedBy == null)
//            .Select(
//                x => new
//                {
//                    x.ID,
//                    x.AdvanceValue,
//                    x.Date
//                });
//    return Json(advanceRequest, JsonRequestBehavior.AllowGet);
//}



//[HttpGet]
//public JsonResult GetAdvancePermissions(int screenId)
//{


//    var CurrentUser = HttpContext.Session["UserId"] as DashBoard_Users;
//    if (CurrentUser != null)
//    {

//        if (CurrentUser.ID == 0)
//        {
//            //return RedirectToAction("Login", "Login");
//            return Json(0, JsonRequestBehavior.AllowGet);
//        }
//    }

//    var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);
//    var permissions = new AdvancePermissions();
//    foreach (var permission in perm)
//    {
//        if (permission == "اضافة")
//        {
//            permissions.Create = true;
//        }
//        else if (permission == "قراءة")
//        {
//            permissions.Read = true;
//        }
//        else if (permission == "تعديل")
//        {
//            permissions.Update = true;
//        }
//        else if (permission == "حذف")
//        {
//            permissions.Delete = true;
//        }
//        else if (permission == "عرض")
//        {
//            permissions.View = true;
//        }
//        else if (permission == "حفظ")
//        {
//            permissions.Save = true;
//        }
//        else if (permission == "استثناء")
//        {
//            permissions.Exception = true;
//        }
//        else if (permission == "موافقة / رفض")
//        {
//            permissions.AcceptReject = true;
//        }
//    }

//    return Json(permissions, JsonRequestBehavior.AllowGet);
//}


//public AdvancePermissions GetAdvancePermissionsfn(int screenId)
//{


//    var CurrentUser = HttpContext.Session["UserId"] as DashBoard_Users;
//    if (CurrentUser != null)
//    {

//        if (CurrentUser.ID == 0)
//        {
//            //return RedirectToAction("Login", "Login");
//            return null;
//        }
//    }

//    var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);
//    var permissions = new AdvancePermissions();
//    foreach (var permission in perm)
//    {
//        if (permission == "اضافة")
//        {
//            permissions.Create = true;
//        }
//        else if (permission == "قراءة")
//        {
//            permissions.Read = true;
//        }
//        else if (permission == "تعديل")
//        {
//            permissions.Update = true;
//        }
//        else if (permission == "حذف")
//        {
//            permissions.Delete = true;
//        }
//        else if (permission == "عرض")
//        {
//            permissions.View = true;
//        }
//        else if (permission == "حفظ")
//        {
//            permissions.Save = true;
//        }
//        else if (permission == "استثناء")
//        {
//            permissions.Exception = true;
//        }
//        else if (permission == "موافقة / رفض")
//        {
//            permissions.AcceptReject = true;
//        }
//    }

//    return permissions;
//}


//#region dataSources
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]
//public JsonResult GetNationalities()
//{
//    var modelList = dbSch.usp_getNationalities()
//        .Select(x => new { NATIONALITY_CODE = x.NATIONALITY_CODE, NATIONALITY_DESC = x.NATIONALITY_DESC })
//        .OrderBy(x => x.NATIONALITY_DESC).ToList();
//    return Json(modelList, JsonRequestBehavior.AllowGet);
//}

//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]
//public JsonResult GetDegrees()
//{
//    var modelList = dbSch.usp_getDegrees()
//        .Select(x => new { DEGREE_CODE = x.DEGREE_CODE, DEGREE_DESC = x.DEGREE_DESC })
//        .OrderBy(x => x.DEGREE_CODE).ToList();
//    return Json(modelList, JsonRequestBehavior.AllowGet);
//}

//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]
//public JsonResult GetLevels()
//{
//    var modelList = dbSch.usp_getLevels().Select(x => new { LEVEL_CODE = x.LEVEL_CODE, LEVEL_DESC = x.LEVEL_DESC })
//        .OrderBy(x => x.LEVEL_CODE).ToList();
//    return Json(modelList, JsonRequestBehavior.AllowGet);
//}
//المبالغ المتبقية
//[HttpGet]
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]
//public ActionResult GetRemainingPremiums(string studentId)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json(null, JsonRequestBehavior.AllowGet);
//    }
//    var studentID = Convert.ToInt32(studentId);

//    //var employee = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    //if (employee == null)
//    //{
//    //    return Json(null, JsonRequestBehavior.AllowGet);
//    //}

//    //مجموع السلف
//    var advances = dbSch.Advances.Where(x => x.EmpID == studentID);
//    var advancesSum = dbSch.Advances.Where(x => x.EmpID == studentID).Select(x => x.AdvanceValue)
//        .DefaultIfEmpty(0).Sum();

//    //مجموع الاقساط المسددة
//    var paidPremiums = dbSch.AdvancesPremiums.Where(x => advances.Any(a => a.ID == x.AdvID))
//        .Select(x => x.NetValue).DefaultIfEmpty(0).Sum();
//    //الباقى
//    var difference = advancesSum - paidPremiums;
//    return Json(difference, JsonRequestBehavior.AllowGet);
//}

//بيانات اخر طلب معلق
//[HttpGet]
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]
//public ActionResult GetLatestAdvanceRequestData(string studentId)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json(null, JsonRequestBehavior.AllowGet);
//    }
//    var studentID = Convert.ToInt32(studentId);

//    //var employee = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    //if (employee == null)
//    //{
//    //    return Json(null, JsonRequestBehavior.AllowGet);
//    //}

//    //اخر طلب معلق لم يتم رفضة او قبوله
//    var latestAdvanceRequest = dbSch.AdvanceRequest
//        .Where(x => x.EmpID == studentID && x.RefusedBy == null && x.ApprovedBy == null)
//        .OrderByDescending(x => x.Date).FirstOrDefault();
//    return Json(latestAdvanceRequest, JsonRequestBehavior.AllowGet);
//}

////تعديل اخر طلب
//[HttpPost]
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]
//public ActionResult EditAdvanceRequest(int advanceRequestId, string studentId, int advanceType,
//    DateTime advanceDate, int value)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json("عفواً، لا يوجد طالب بهذه البيانات", JsonRequestBehavior.AllowGet);
//    }
//    var studentID = Convert.ToInt32(studentId);

//    var advanceRequest = dbSch.AdvanceRequest.FirstOrDefault(x => x.ID == advanceRequestId);
//    if (advanceRequest == null)
//    {
//        return Json("عفواً، لا يوجد طلب بهذه البيانات", JsonRequestBehavior.AllowGet);
//    }

//    //var employee = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    //if (employee == null)
//    //{
//    //    return Json("عفواً، لا يوجد طالب بهذه البيانات", JsonRequestBehavior.AllowGet);
//    //}

//    //حذف التفاصيل القديمة
//    var oldAdvanceRequestDetails =
//        dbSch.AdvanceRequestDetails.Where(x => x.AdvanceRequestId == advanceRequestId);
//    dbSch.AdvanceRequestDetails.RemoveRange(oldAdvanceRequestDetails);

//    //قيمة القسط الشهرى 200
//    var numberOfMonthes = value / 200;
//    var remainder = value % 200;
//    var monthsIds = new List<int>();
//    //بيقسم الاقساط على الشهور قيمة القسط 200
//    for (var i = 1; i <= numberOfMonthes; i++)
//    {
//        var month = new DateTime(advanceDate.AddMonths(1).Year, advanceDate.AddMonths(1).Month, 1);
//        var monthHr = dbSch.Month.FirstOrDefault(x => x.StartDate == month);
//        if (monthHr != null)
//        {
//            monthsIds.Add(monthHr.ID);
//        }
//    }

//    //لو فى باقى 50 | 100 | 150 هيحطه فى شهر زيادة
//    switch (remainder)
//    {
//        //للارقام 250-450-650-850
//        case 50:
//            {
//                var month = new DateTime(advanceDate.Year, advanceDate.Month + monthsIds.Count + 1, 1);
//                var monthHr = dbSch.Month.SingleOrDefault(x => x.StartDate == month);
//                if (monthHr != null)
//                {
//                    monthsIds.Add(monthHr.ID);
//                }

//                break;
//            }
//        //للارقام 100 - 300 - 500 - 700 - 900
//        case 100:
//            {
//                var month = new DateTime(advanceDate.Year, advanceDate.Month + monthsIds.Count + 1, 1);
//                var monthHr = dbSch.Month.SingleOrDefault(x => x.StartDate == month);
//                if (monthHr != null)
//                {
//                    monthsIds.Add(monthHr.ID);
//                }

//                break;
//            }
//        //للارقام 150 - 350 - 550 - 750 - 950
//        case 150:
//            {
//                var month = new DateTime(advanceDate.Year, advanceDate.Month + monthsIds.Count + 1, 1);
//                var monthHr = dbSch.Month.SingleOrDefault(x => x.StartDate == month);
//                if (monthHr != null)
//                {
//                    monthsIds.Add(monthHr.ID);
//                }

//                break;
//            }
//    }

//    var student = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentID);

//    var advanceSettings = dbSch.AdvanceSettings.SingleOrDefault(x => x.AdvanceSettingId == advanceType);
//    advanceRequest.AdvanceSettingId = advanceType;
//    advanceRequest.Date = advanceDate;
//    advanceRequest.AdvanceValue = value;
//    advanceRequest.AdvanceName = $"{advanceSettings?.AdvanceSettingName}_ للطالب_{student.STUDENT_NAME}";
//    advanceRequest.MonthsNumbers = string.Join(",", monthsIds);
//    advanceRequest.DueDate = new DateTime(advanceDate.AddMonths(1).Year, advanceDate.AddMonths(1).Month, 1);
//    dbSch.Entry(advanceRequest).State = EntityState.Modified;

//    //اضافة التفاصيل الجديدة
//    for (var i = 1; i <= monthsIds.Count; i++)
//    {
//        //شهر بداية الخصم
//        var month = new DateTime(advanceDate.AddMonths(1).Year, advanceDate.AddMonths(1).Month, 1);
//        var monthHr = dbSch.Month.SingleOrDefault(x => x.StartDate == month);
//        if (monthHr != null)
//        {
//            var advanceRequestDetails = new AdvanceRequestDetails();
//            if (remainder == 50 && i == monthsIds.Count)
//            {
//                //للارقام 250-450-650-850
//                advanceRequestDetails.AdvanceRequestId = advanceRequest.ID;
//                advanceRequestDetails.MonthId = monthHr.ID;
//                advanceRequestDetails.MonthlyValue = 50;
//                advanceRequestDetails.AdvanceId = null;
//            }
//            else if (remainder == 100 && i == monthsIds.Count)
//            {
//                //للارقام 100 - 300 - 500 - 700 - 900
//                advanceRequestDetails.AdvanceRequestId = advanceRequest.ID;
//                advanceRequestDetails.MonthId = monthHr.ID;
//                advanceRequestDetails.MonthlyValue = 100;
//                advanceRequestDetails.AdvanceId = null;
//            }
//            else if (remainder == 150 && i == monthsIds.Count)
//            {
//                //للارقام 150 - 350 - 550 - 750 - 950
//                advanceRequestDetails.AdvanceRequestId = advanceRequest.ID;
//                advanceRequestDetails.MonthId = monthHr.ID;
//                advanceRequestDetails.MonthlyValue = 150;
//                advanceRequestDetails.AdvanceId = null;
//            }
//            else
//            {
//                advanceRequestDetails = new AdvanceRequestDetails
//                {
//                    AdvanceRequestId = advanceRequest.ID,
//                    MonthId = monthHr.ID,
//                    MonthlyValue = 200,
//                    AdvanceId = null
//                };
//            }

//            dbSch.AdvanceRequestDetails.Add(advanceRequestDetails);
//        }
//    }

//    dbSch.SaveChanges();
//    return Json("", JsonRequestBehavior.AllowGet);
//}

////هل يمكن حذف اخر طلب للطالب
//[HttpGet]
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]

//public ActionResult CanDeleteLastAdvanceRequest(string studentId)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json(false, JsonRequestBehavior.AllowGet);
//    }
//    var studentID = Convert.ToInt32(studentId);

//    //var employee = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    //if (employee == null)
//    //{
//    //    return Json(false, JsonRequestBehavior.AllowGet);
//    //}

//    // لم ياخذ موافقةاخر طلب للطالب
//    var lastAdvanceRequest = dbSch.AdvanceRequest
//        .Where(x => x.EmpID == studentID && x.RequestType == "A" && x.ApprovedBy == null)
//        .OrderBy(x => x.Date)
//        .FirstOrDefault();
//    return Json(lastAdvanceRequest != null, JsonRequestBehavior.AllowGet);
//}

////حذف اخرطلب سلفة
//public ActionResult DeleteLastAdvanceReuqst(string studentId)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json("لابد من اختيار الطالب أولا", JsonRequestBehavior.AllowGet);
//    }
//    var studentID = Convert.ToInt32(studentId);

//    //var employee = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    //if (employee == null)
//    //{
//    //    return Json("عفواً، لا يوجد بيانات لهذا الطالب فى الـ HR", JsonRequestBehavior.AllowGet);
//    //}

//    // لم ياخذ موافقة اخر طلب للطالب
//    var lastAdvanceRequest = dbSch.AdvanceRequest
//        .Where(x => x.EmpID == studentID && x.RequestType == "A" && x.ApprovedBy == null)
//        .OrderBy(x => x.Date)
//        .FirstOrDefault();
//    if (lastAdvanceRequest == null)
//    {
//        return Json("عفواً، لا يوجد طلب سلفة بهذه البيانات", JsonRequestBehavior.AllowGet);
//    }

//    //تفاصيل الطلب
//    var advanceRequestDetails =
//        dbSch.AdvanceRequestDetails.Where(x => x.AdvanceRequestId == lastAdvanceRequest.ID);
//    //حذف التفاصيل
//    dbSch.AdvanceRequestDetails.RemoveRange(advanceRequestDetails);
//    //حذف الطلب نفسه
//    dbSch.AdvanceRequest.Remove(lastAdvanceRequest);
//    dbSch.SaveChanges();

//    return Json("", JsonRequestBehavior.AllowGet);
//}

////هل يمكن حذف اخر طلب للطالب
//[HttpGet]
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]

//public ActionResult CanDeleteLastSubsidyRequest(string studentId)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json(false, JsonRequestBehavior.AllowGet);
//    }
//    var studentID = Convert.ToInt32(studentId);

//    //var employee = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    //if (employee == null)
//    //{
//    //    return Json(false, JsonRequestBehavior.AllowGet);
//    //}

//    // لم ياخذ موافقةاخر طلب للطالب
//    var lastAdvanceRequest = dbSch.AdvanceRequest
//        .Where(x => x.EmpID == studentID && x.RequestType == "S" && x.ApprovedBy == null)
//        .OrderBy(x => x.Date)
//        .FirstOrDefault();
//    return Json(lastAdvanceRequest != null, JsonRequestBehavior.AllowGet);
//}

////حذف اخرطلب اعانة
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 27)]

//public ActionResult DeleteLastSubsidyReuqst(string studentId)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json("برجاء اختيار الطالب أولا", JsonRequestBehavior.AllowGet);
//    }
//    var studentID = Convert.ToInt32(studentId);

//    //var employee = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    //if (employee == null)
//    //{
//    //    return Json("عفواً، لا يوجد بيانات لهذا الطالب فى الـ HR", JsonRequestBehavior.AllowGet);
//    //}

//    // لم ياخذ موافقة اخر طلب للطالب
//    var lastSubsidyRequest = dbSch.AdvanceRequest
//        .Where(x => x.EmpID == studentID && x.RequestType == "S" && x.ApprovedBy == null)
//        .OrderBy(x => x.Date)
//        .FirstOrDefault();
//    if (lastSubsidyRequest == null)
//    {
//        return Json("عفواً، لا يوجد طلب اعانة بهذه البيانات", JsonRequestBehavior.AllowGet);
//    }

//    //حذف الطلب نفسه
//    dbSch.AdvanceRequest.Remove(lastSubsidyRequest);
//    dbSch.SaveChanges();

//    return Json("", JsonRequestBehavior.AllowGet);
//}


//#region الإعانات




//public ActionResult Subsidies()
//{
//    //إضافة طلب اعانة
//    var json = new JavaScriptSerializer().Serialize(GetAdvancePermissions(31).Data);
//    var permissions = JsonConvert.DeserializeObject<AdvancePermissions>(json);

//    if (permissions.Read == false)
//    {
//        return RedirectToAction("NotAuthorized", "Security");
//    }

//    return View();
//}

//[HttpGet]

//public ActionResult SubsidiesSearch(int? Nationality, int? Degree, int? Level, decimal? StudentId,
//    string NationalId,
//    DataSourceLoadOptions loadOptions)
//{
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    var studentID = Convert.ToInt32(StudentId);

//    //var CurrentUser = Session["UserId"] as DashBoard_Users;

//    //    if (Session["UserId"] == null)
//    //{
//    //    return RedirectToAction("Login", "Login");
//    //}

//    //إضافة طلب اعانة
//    var json = new JavaScriptSerializer().Serialize(GetAdvancePermissions(31).Data);
//    var permissions = JsonConvert.DeserializeObject<AdvancePermissions>(json);

//    if (permissions.Read == false)
//    {
//        return RedirectToAction("NotAuthorized", "Security");
//    }

//    if (CurrentUser != null && CurrentUser.IsStudent)
//    {
//        StudentId = decimal.Parse(CurrentUser.ID.ToString());
//    }

//    dbSch.Database.CommandTimeout = 6 * 60;
//    dbSch.Database.CommandTimeout = 6 * 60;

//    LoadResult loadResult;
//    if (NationalId == "")
//    {
//        NationalId = null;
//    }

//    if (Nationality == null && NationalId == null && Degree == null && Level == null && StudentId == null)
//    {
//        loadResult = DataSourceLoader.Load(new List<INTEGRATION_All_Students>(), loadOptions);
//        return Json(loadResult, JsonRequestBehavior.AllowGet);
//    }

//    if (StudentId == null)
//    {
//        var filteredStudents = dbSch.INTEGRATION_All_Students.Where(x =>
//                (NationalId == null || x.NATIONAL_ID == NationalId) &&
//                x.STATUS_CODE == 1 &&
//                (Nationality == null || x.NATIONALITY_CODE == Nationality) &&
//                (Degree == null || x.DEGREE_CODE == Degree) &&
//                (Level == null && x.LEVEL_CODE != null || x.LEVEL_CODE == Level))
//            .Distinct().Select(x => new
//            {
//                x.STUDENT_ID,
//                x.STUDENT_NAME,
//                x.NATIONAL_ID,
//                x.DEGREE_DESC,
//                x.STATUS_CODE,
//                x.NATIONALITY_CODE,
//                x.DEGREE_CODE,
//                x.LEVEL_CODE,
//                x.LEVEL_DESC,
//                GPA = dbSch.ST_AcademicData.Where(y => y.student_id == x.STUDENT_ID && y.cum_gpa != null)
//                    .OrderByDescending(y => y.semester).FirstOrDefault().cum_gpa
//            }).OrderBy(x => x.STUDENT_NAME);
//        loadResult = DataSourceLoader.Load(filteredStudents, loadOptions);
//    }
//    else
//    {
//        //بحث بالرقم الاكاديمى
//        var filteredStudents = dbSch.INTEGRATION_All_Students.Where(x =>
//                x.STUDENT_ID == StudentId && x.STATUS_CODE == 1)
//            .Distinct().Select(x => new
//            {
//                x.STUDENT_ID,
//                x.STUDENT_NAME,
//                x.NATIONAL_ID,
//                x.DEGREE_DESC,
//                x.STATUS_CODE,
//                x.NATIONALITY_CODE,
//                x.DEGREE_CODE,
//                x.LEVEL_CODE,
//                GPA = dbSch.ST_AcademicData.Where(y => y.student_id == x.STUDENT_ID && y.cum_gpa != null)
//                    .OrderByDescending(y => y.semester).FirstOrDefault().cum_gpa
//            }).OrderBy(x => x.STUDENT_NAME);
//        loadResult = DataSourceLoader.Load(filteredStudents, loadOptions);
//    }

//    return Json(loadResult, JsonRequestBehavior.AllowGet);
//}

//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 31)]
//public ActionResult GetSubsidiesTypes()
//{
//    var types = dbSch.AdvanceSettings.Where(x => x.AdvanceType == "S").Select(x => new SelectListItem
//    {
//        Text = x.AdvanceSettingName.ToString(),
//        Value = x.AdvanceSettingId.ToString()
//    });
//    return Json(types.ToList(), JsonRequestBehavior.AllowGet);
//}


//[HttpPost]
////[Dashboard_StudentProfile.Helpers.CustomValidateAntiForgeryToken()]
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 31)]
//public ActionResult AddSubsidy(string studentId, DateTime advanceDate, int value, int subsidyType)
//{
//    //if (Session["UserId"] == null)
//    //{
//    //    return RedirectToAction("Login", "Login");
//    //}
//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json("برجاء اختيار الطالب أولا");
//        //var currentUser = Session["UserId"] as DashBoard_Users;
//        //studentId = currentUser.ID.ToString();
//    }
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }

//    var studentID = Convert.ToInt32(studentId);

//    //var employee = dbSch.Employees.SingleOrDefault(x => x.empFileNum == studentId);
//    //if (employee == null)
//    //{
//    //    return Json("عفواً لا يوجد بيانات لهذا الطالب فى الـ HR");
//    //}

//    var advanceSetting = dbSch.AdvanceSettings.FirstOrDefault(x => x.AdvanceSettingId == subsidyType);
//    var student = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentID);

//    var advanceRequest = new AdvanceRequest
//    {

//        EmpID = studentID,
//        AdvanceValue = value,
//        RequestType = "S", //نوع الطلب اعانة
//        DueDate = new DateTime(advanceDate.Year, advanceDate.Month + 1, 1),
//        MonthsNumbers = null,
//        MonthlyPremium = 200,
//        Date = advanceDate,
//        AdvanceSettingId = advanceSetting?.AdvanceSettingId,
//        AdvanceName = $"{advanceSetting?.AdvanceSettingName}_ للطالب_{student?.STUDENT_NAME}"
//    };
//    dbSch.AdvanceRequest.Add(advanceRequest);
//    dbSch.SaveChanges();

//    try
//    {
//        var attachments = Request.Files;
//        foreach (var key in attachments.AllKeys)
//        {
//            var directory = Server.MapPath($"~/Content/UserFiles/{studentId}/الاعانات");
//            var exists = Directory.Exists(directory);
//            if (!exists)
//            {
//                Directory.CreateDirectory(directory);
//            }

//            var fileName = $"{advanceRequest.ID}_{attachments[key]?.FileName}";
//            attachments[key]?.SaveAs($"{directory}/{fileName}");
//            advanceRequest.Attachments += $"{fileName};";
//        }

//        dbSch.Entry(advanceRequest).State = EntityState.Modified;
//        dbSch.SaveChanges();
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine(e);
//        throw;
//    }

//    return Json("");
//}

//#endregion

//#region حالة الطلب

//[HttpGet]
//public ActionResult RequestStauts()
//{
//    var json = new JavaScriptSerializer().Serialize(GetAdvancePermissions(39).Data);
//    var permissions = JsonConvert.DeserializeObject<AdvancePermissions>(json);
//    if (permissions.Read)
//    {
//        return View();
//    }

//    return RedirectToAction("NotAuthorized", "Security");
//}

//[HttpGet]
//[StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 39)]
//public ActionResult RequestStautsSearch(string requestType, int? reuqestStatus, string studentId,
//    DataSourceLoadOptions loadOptions)
//{
//    if (string.IsNullOrEmpty(requestType) && string.IsNullOrEmpty(studentId) && reuqestStatus == null)
//    {
//        return Json(DataSourceLoader.Load(new List<AdvanceRequest>(), loadOptions),
//            JsonRequestBehavior.AllowGet);
//    }

//    if (string.IsNullOrEmpty(studentId))
//    {
//        return Json("برجاء اختيار الطالب أولا");
//    }
//    if (CurrentUser == null)
//    {
//        RedirectToAction("Login", "Login");
//    }
//    var studentID = Convert.ToInt32(studentId);

//    IQueryable<AdvanceRequest> requests;
//    switch (reuqestStatus)
//    {
//        case 0:
//            //الطلبات اللى تم رفضها
//            requests = dbSch.AdvanceRequest.Where(x => x.RefusedBy != null);
//            break;
//        case 1:
//            //الطلبات اللى اخدت موافقة المستوى الاول 
//            requests = dbSch.AdvanceRequest.Where(x => x.FirstApprove != null);
//            break;
//        case 2:
//            //الطلبات اللى اخدت موافقة المستوى الثانى 
//            requests = dbSch.AdvanceRequest.Where(x => x.ScondApprove != null);
//            break;
//        case 3:
//            //الطلبات اللى اخدت موافقة المستوى الثالث 
//            requests = dbSch.AdvanceRequest.Where(x => x.ApprovedBy != null);
//            break;
//        default:
//            // جميع الطلبات
//            requests = dbSch.AdvanceRequest;
//            break;
//    }

//    switch (requestType)
//    {
//        case "A":
//            //نوع الطلب سلفة
//            requests = requests.Where(x => x.RequestType == "A");
//            break;
//        case "S":
//            //نوع الطلب اعانة
//            requests = requests.Where(x => x.RequestType == "S");
//            break;
//        default:
//            // السلف والاعانات
//            break;
//    }

//    //var employee = dbSch.Employees.FirstOrDefault(x => x.empFileNum == studentId);
//    requests = requests.Where(x => x.EmpID == studentID);
//    var student = dbSch.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentID);

//    var selectedData = requests.Select(x => new
//    {
//        x.AdvanceValue,
//        x.Date,
//        studentName = student.STUDENT_NAME,
//        studentId = student.STUDENT_ID,
//        firstApproval = x.ScondApprove != null,
//        secondApproval = x.FirstApprove != null,
//        thirdApproval = x.ApprovedBy != null,
//        type = x.RequestType == "A" ? "سلفة" : "اعانة"
//    }).OrderBy(x => x.studentName);
//    var loadResult = DataSourceLoader.Load(selectedData, loadOptions);

//    return Json(loadResult, JsonRequestBehavior.AllowGet);
#endregion



