using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
// ReSharper disable All

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class RewardsController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(47);
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

        // GET: PayrollRefusalSearch
        public ActionResult PayrollRefusalSearch()
        {

            var permissions = GetPermissionsFn(128);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.View = permissions.View;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read || permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        // GET: Nationalities
        [HttpGet]
        public async Task<ActionResult> GetAllNationalities()
        {
            var db = new SchoolAccGam3aEntities();

            //db.Configuration.ProxyCreationEnabled = false;
            return Json(await Task.Run(() => db.usp_getNationalities()
                .Select(x => new { NationalityName = x.NATIONALITY_DESC, NationalityId = x.NATIONALITY_CODE.ToString() })
                .ToList()), JsonRequestBehavior.AllowGet);

        }

        // GET: Reward Categories Received From Integration Table ...
        [HttpGet]
        public async Task<ActionResult> GetAllRewardcategories_Integration()
        {
            var db = new SchoolAccGam3aEntities();

            return Json(await Task.Run(() => db.usp_GetRewardCategory_Intg().Select(x => new
            {
                Cat_Code_Integration = x.CATEGORY_CODE,
                Cat_Name_Integration = x.CATEGORY_NAME,
            }).ToList()), JsonRequestBehavior.AllowGet);
        }

        // GET: Reward Categories ...
        [HttpGet]
        public async Task<ActionResult> GetAllRewardcategories()
        {
            var db = new SchoolAccGam3aEntities();

            return Json(await Task.Run(() => db.vw_GetAllRewardCategories.Select(x => new
            {
                RewardCategoryId = x.RewardCategoryId,
                RewardCategoryName = x.RewardCategoryName,
                RewardCategoryNameEng = x.RewardCategoryNameEng,
                NationalityID = x.NationalityID,
                NationalityDesc = x.NationalityDesc,
            }).ToList()), JsonRequestBehavior.AllowGet);

        }

        // GET: Reward Categories ...
        [HttpGet]
        public async Task<ActionResult> GetAllRewardcategoriesDistinct()
        {
            var db = new SchoolAccGam3aEntities();
            var model = new List<vw_GetAllRewardCategories>();

            var NationalityListFunc = await Task.Run(() => db.usp_getNationalities().Select(x => Convert.ToInt32(x.NATIONALITY_CODE)).ToList());
            var NationalityListInDB = await Task.Run(() => db.vw_GetAllRewardCategories);

            var distinctRewardCategoryID = NationalityListInDB.Select(x => x.RewardCategoryId).Distinct().ToList();
            foreach (var RewardCategoryID in distinctRewardCategoryID)
            {
                var NationalityList = NationalityListInDB.Where(x => x.RewardCategoryId == RewardCategoryID).Select(x => x.NationalityID).Distinct().ToList();

                model.Add(new vw_GetAllRewardCategories
                {
                    RewardCategoryId = RewardCategoryID,
                    RewardCategoryNameEng = NationalityListInDB.FirstOrDefault(x => x.RewardCategoryId == RewardCategoryID).RewardCategoryNameEng,
                    RewardCategoryName = NationalityListInDB.FirstOrDefault(x => x.RewardCategoryId == RewardCategoryID).RewardCategoryName,
                    NationalityDesc = NationalityListFunc.Except(NationalityList).ToList().Count == 0 ? "كل الجنسيات" : NationalityList.SkipWhile(element => element == 1).Count() == 0 ? "السعودية فقط" : NationalityList.Contains(1) ? "السعودية وبعض الجنسيات الأخري" : "غير السعودية فقط",
                    Cat_Code_Integration = NationalityListInDB.FirstOrDefault(x => x.RewardCategoryId == RewardCategoryID).Cat_Code_Integration,
                    Cat_Name_Integration = NationalityListInDB.FirstOrDefault(x => x.RewardCategoryId == RewardCategoryID).Cat_Name_Integration
                });

            }
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        //Get: RewardCategoryById
        [HttpGet]
        public async Task<ActionResult> GetRewardcategoryById(int RewardcategoryId)
        {
            try
            {
                var db = new SchoolAccGam3aEntities();
                // Its because it is trying to load child objects and it may be creating some circular loop that will never ending(a => b, b => c, c => d, d => a)
                db.Configuration.ProxyCreationEnabled = false;
                var model = db.RewardCategory.Where(x => x.ID == RewardcategoryId).Include(x => x.RewardNationaltity).ToList();
                if (model != null)
                {
                    return Json(await Task.Run(() => model.Select(x => new
                    {
                        RewardNationaltities = x.RewardNationaltity.Select(o => Convert.ToString(o.NationalityId)),
                        CategoryName_Arb = x.CategoryName_Arb,
                        CategoryName_Eng = x.CategoryName_Eng,
                        Cat_Code_Integration = x.Cat_Code_Integration,
                        Cat_Name_Integration = x.Cat_Name_Integration
                    }).FirstOrDefault()), JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message + " حدث خطأ ", JsonRequestBehavior.AllowGet);
            }

        }

        // GET: Faculties
        [HttpGet]
        public async Task<ActionResult> GetAllFaculties()
        {

            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.usp_getFaculties().Select(x => new
            {
                FacultyBranchId = x.FACULTY_NO.ToString(),
                FacultyBranchName = x.FACULTY_NAME
            }).ToList()), JsonRequestBehavior.AllowGet);

        }

        // GET: LevelsOrDegres
        [HttpGet]
        public async Task<ActionResult> GetAllEducationLevels()
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.usp_getDegrees().Select(x => new
            {
                EducationLevelId = x.DEGREE_CODE.ToString(),
                EducationLevelName = x.DEGREE_DESC
            }).ToList()), JsonRequestBehavior.AllowGet);
        }

        // GET: StudyTypes
        [HttpGet]
        public async Task<ActionResult> GetAllEducationTypes()
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.usp_getStudyTypes().Select(x => new
            {
                EducationTypeId = x.STUDY_CODE,
                EducationTypeName = x.STUDY_DESC
            }).ToList()), JsonRequestBehavior.AllowGet);
        }

        // GET: RegisterTypes
        [HttpGet]
        public async Task<ActionResult> GetAllRegisterTypes()
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.usp_getStatus().Select(x => new
            {
                RegisterTypeId = x.STATUS_CODE.ToString(),
                RegisterTypeName = x.STATUS_DESC
            }).ToList()), JsonRequestBehavior.AllowGet);
        }

        //GET: AllowancesOrRewardItems
        [HttpGet]
        public async Task<ActionResult> GetAllRewardItems()
        {
            var db = new SchoolAccGam3aEntities();

            return Json(await Task.Run(() => db.RewardItems.Select(x => new
            {
                AllowanceId = x.ID,
                AllowanceName = x.RewardItemName_Arb,
                RewardItemName_Eng = x.RewardItemName_Eng,
                RewardItemTypeName = x.RewardItemTypes.RewardItemTypeName,
                RewardItemExpensesTypeName = x.RewardItemExpensesTypes.RewardItemExpensesTypeName,
                AfterCheckingResult = x.AfterCheckingResult,
                x.IsAdvanceReturn
            }).Distinct().ToList()), JsonRequestBehavior.AllowGet);

        }

        //GET: RewardItemById
        [HttpGet]
        public async Task<ActionResult> GetRewardItemById(int RewardItemId)
        {
            try
            {
                var db = new SchoolAccGam3aEntities();

                db.Configuration.ProxyCreationEnabled = false;
                var model = await Task.Run(() => db.RewardItems.Where(x => x.ID == RewardItemId).FirstOrDefault());
                if (model != null)
                {

                    return Json(model, JsonRequestBehavior.AllowGet);
                }

                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message + " حدث خطأ ", JsonRequestBehavior.AllowGet);
            }

        }

        //GET: RewardDetails
        [HttpGet]
        public async Task<ActionResult> GetAllRewardDetails()

        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.vw_GetAllRewards.ToList()), JsonRequestBehavior.AllowGet);

        }

        //GET: RewardDetailsById
        [HttpGet]
        public async Task<ActionResult> GetRewardDetailsById(int RewardDetailsId)
        {
            var db = new SchoolAccGam3aEntities();

            return Json(await Task.Run(() => db.RewardDetails.ToList().Where(x => x.ID == RewardDetailsId).Select(x => new
            {
                RewardDetailsId = x.ID,
                RewardCategoryId = x.RewardMaster.RewardCategory.ID,
                FacultyId = x.RewardMaster.Faculties_ID.ToString(),
                EducationLevelId = x.RewardMaster.EducationLevel_ID.ToString(),
                StudentStatusAcademyId = x.RewardMaster.StudentStatusAcademyId.ToString(),
                ItemId = x.RewardItems.ID,
                Amount = x.Amount,
                EducationTypeId = x.EducationType,
                AmountTypeId = x.AmountType_ID,
                FromRate = x.FromRate,
                ToRate = x.ToRate,
                RegisterTypeId = x.RegisterTypeId,
                MiniHoursNo = x.MiniHoursNo,
                ClassesNo = x.ClassesNo,
                DuesNo = x.DuesNo,
                DependOnEducationDays = x.DependOnEducationDays,
                DependOnEducationPeriod = x.DependOnEducationPeriod,
                RateTypeId = x.RateType
            }).FirstOrDefault()), JsonRequestBehavior.AllowGet);
        }

        //GET: RewardItemTypes
        [HttpGet]
        public async Task<ActionResult> GetAllRewardItemTypes()

        {
            var db = new SchoolAccGam3aEntities();
            db.Configuration.ProxyCreationEnabled = false;
            return Json(await Task.Run(() => db.RewardItemTypes.Distinct().ToList()), JsonRequestBehavior.AllowGet);

        }

        //GET: RewardItemExpensesTypes
        [HttpGet]
        public async Task<ActionResult> GetAllRewardItemExpensesTypes()

        {
            var db = new SchoolAccGam3aEntities();
            db.Configuration.ProxyCreationEnabled = false;
            return Json(await Task.Run(() => db.RewardItemExpensesTypes.Distinct().ToList()), JsonRequestBehavior.AllowGet);

        }

        //GET: CheckExpensesType
        [HttpGet]
        public async Task<ActionResult> CheckExpensesTypeById(int RewardItemId)
        {
            var db = new SchoolAccGam3aEntities();

            //db.Configuration.ProxyCreationEnabled = false;
            var model = await Task.Run(() => db.RewardItems.Where(x => x.ID == RewardItemId).FirstOrDefault());
            if (model != null)
            {
                var ExpensesType = model.RewardItemExpensesTypes.RewardItemExpensesTypeName;
                return Json(ExpensesType, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }

        //GET: Students
        [HttpGet]
        public async Task<ActionResult> GetAllStudents()
        {

            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.SP_GetAllStudentNames().ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPayrollRefusal(string FacultyIds, string NationalityIds, string RewardCategoryIds,
                                                          string RewardItemIds, string StudentDegrees, string StudentIds,
                                                          string StudentStatusAcademyIds , DateTime PayRollDateFrom, DateTime PayRollDateTo)
        {
            var db = new SchoolAccGam3aEntities();
            return new JsonResult()
            {
                Data = db.SP_PayrollRefusal(FacultyIds,
                                            StudentDegrees,
                                            StudentIds,
                                            StudentStatusAcademyIds,
                                            NationalityIds,
                                            RewardItemIds,
                                            RewardCategoryIds,
                                            PayRollDateFrom,
                                            PayRollDateTo).ToList(),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult UploadBankFiles()
        {
            var permissions = GetPermissionsFn(49);
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
        public ActionResult UploadAcceptedFiles()
        {
            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                string fileName = DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + file.FileName;
                string path = Path.Combine(Server.MapPath("~/Attachments/BankFiles/AcceptedFiles/"), Path.GetFileName(fileName));
                try
                {

                    file.SaveAs(path);
                    DataTable dt = ConvertToDataTable(path, 1);
                    if (dt.Rows.Count > 0)
                    {
                        int userId = UserID;
                        var FilePath = "~/Attachments/BankFiles/AcceptedFiles/" + Path.GetFileName(fileName);
                        var db = new SchoolAccGam3aEntities();
                        foreach (DataRow Row in dt.Rows)
                        {
                            if (string.IsNullOrWhiteSpace(Row[0].ToString())) { continue; }
                            var LineAsUploaded = Row[0].ToString().Substring(2).Trim();
                            var CodeSymbol = Row[0].ToString().Substring(0, 2);

                            var SarafDownloadedInDB = db.SarafDownloaded.Where(x => x.LineAsDownloaded == LineAsUploaded).FirstOrDefault();
                            if (SarafDownloadedInDB != null)
                            {
                                var BankAttachmentsInDB = db.BankAttachments.Where(x => x.LineAsUploaded == LineAsUploaded).FirstOrDefault();
                                if (BankAttachmentsInDB == null)
                                {
                                    BankAttachments bankAttachments = new BankAttachments
                                    {
                                        User_Id = UserID,
                                        InsertDate = DateTime.Now,
                                        BankCode_Id = db.BankCodes.Where(x => x.CodeSymbol == CodeSymbol).Select(x => x.ID).FirstOrDefault(),
                                        FilePath = FilePath,
                                        LineAsUploaded = LineAsUploaded,
                                        SarafDownloaded_ID = SarafDownloadedInDB.Id
                                    };
                                    db.BankAttachments.Add(bankAttachments);
                                }
                                else if (BankAttachmentsInDB.BankCode_Id != 1)
                                {
                                    BankAttachmentsInDB.User_Id = UserID;
                                    BankAttachmentsInDB.InsertDate = DateTime.Now;
                                    BankAttachmentsInDB.SarafDownloaded_ID = SarafDownloadedInDB.Id;
                                    BankAttachmentsInDB.BankCode_Id = db.BankCodes.Where(x => x.CodeSymbol == CodeSymbol).Select(x => x.ID).FirstOrDefault();
                                    BankAttachmentsInDB.FilePath = FilePath;
                                    db.Entry(BankAttachmentsInDB).State = System.Data.Entity.EntityState.Modified;
                                }

                            }
                        }
                        db.SaveChanges();
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);


                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }

            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);

        }

        public DataTable ConvertToDataTable(string filePath, int numberOfColumns)
        {
            DataTable tbl = new DataTable();

            for (int col = 0; col < numberOfColumns; col++)
            {
                DataColumn column = new DataColumn("FileBankItem");
                column.DataType = System.Type.GetType("System.String");
                tbl.Columns.Add(column);
            }

            string[] lines = System.IO.File.ReadAllLines(filePath, System.Text.Encoding.Default);

            foreach (string line in lines)
            {
                DataRow dr = tbl.NewRow();
                dr[0] = line;
                tbl.Rows.Add(dr);
            }

            return tbl;
        }

        //Post: RewardCaterory
        [HttpPost]
        public ActionResult SaveRewardCaterory(int? RewardCategoryId, string CategoryName_Arb, string CategoryName_Eng, string Nationality_ID, int Cat_Code_Integration)
        {
            try
            {
                var db = new SchoolAccGam3aEntities();
                List<int> NationalityIdsList = Nationality_ID.Split(',').Select(int.Parse).ToList();

                if (RewardCategoryId != null && RewardCategoryId > 0)
                {
                    var model = db.RewardCategory.Where(x => x.ID == RewardCategoryId).FirstOrDefault();
                    if (model != null)// تعديل
                    {
                        var NationalitiesInDB = db.RewardNationaltity.Where(x => x.RewardCategory_Id == model.ID);

                        if (NationalitiesInDB != null)
                        {
                            db.RewardNationaltity.RemoveRange(NationalitiesInDB);
                            db.SaveChanges();
                        }

                        model.CategoryName_Arb = CategoryName_Arb;
                        model.CategoryName_Eng = CategoryName_Eng;
                        model.Cat_Code_Integration = Cat_Code_Integration;
                        model.Cat_Name_Integration = db.usp_GetRewardCategory_Intg().Where(x => x.CATEGORY_CODE == Cat_Code_Integration).FirstOrDefault().CATEGORY_NAME;
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;

                        foreach (var item in NationalityIdsList)
                        {
                            RewardNationaltity rewardNationaltity = new RewardNationaltity()
                            {
                                NationalityId = item,
                                RewardCategory_Id = model.ID
                            };
                            db.RewardNationaltity.Add(rewardNationaltity);
                        }
                        db.SaveChanges();


                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                    return Json("", JsonRequestBehavior.AllowGet);
                }// تعديل


                bool isExisting = db.RewardCategory.Any(x => x.CategoryName_Arb == CategoryName_Arb);
                if (isExisting == true)
                {
                    return Json("عفوا إسم الفئة موجودة من قبل ", JsonRequestBehavior.AllowGet);
                }

                var rewardCategory = new RewardCategory()
                {
                    CategoryName_Arb = CategoryName_Arb,
                    CategoryName_Eng = CategoryName_Eng,
                    Cat_Code_Integration = Cat_Code_Integration,
                    Cat_Name_Integration = db.usp_GetRewardCategory_Intg().Where(x => x.CATEGORY_CODE == Cat_Code_Integration).FirstOrDefault().CATEGORY_NAME
                };
                db.RewardCategory.Add(rewardCategory);

                foreach (var item in NationalityIdsList)
                {
                    RewardNationaltity rewardNationaltity = new RewardNationaltity()
                    {
                        NationalityId = item,
                        RewardCategory_Id = rewardCategory.ID
                    };
                    db.RewardNationaltity.Add(rewardNationaltity);
                }

                db.SaveChanges();
                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message + " حدث خطأ ", JsonRequestBehavior.AllowGet);
            }

        }

        //Post: RewardItem
        [HttpPost]
        public ActionResult SaveRewardItem(int? RewardItemsId, string RewardItemName_Arb,
            string RewardItemName_Eng, int? RewardItemTypeId, int RewardItemExpensesTypeId, bool AfterCheckingResult,bool IsAdvanceReturn)
        {
            try
            {
                var db = new SchoolAccGam3aEntities();

                var model = db.RewardItems.Where(x => x.ID == RewardItemsId || x.RewardItemName_Arb == RewardItemName_Arb).FirstOrDefault();
                if (model != null)
                {
                    model.RewardItemName_Arb = RewardItemName_Arb;
                    model.RewardItemName_Eng = RewardItemName_Eng;
                    model.RewardItemType_Id = RewardItemTypeId;
                    model.RewardItemExpensesType_Id = RewardItemExpensesTypeId;
                    model.AfterCheckingResult = AfterCheckingResult;
                    model.IsAdvanceReturn = IsAdvanceReturn;

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                var rewardItems = new RewardItems()
                {
                    RewardItemName_Arb = RewardItemName_Arb,
                    RewardItemName_Eng = RewardItemName_Eng,
                    RewardItemType_Id = RewardItemTypeId,
                    RewardItemExpensesType_Id = RewardItemExpensesTypeId,
                    AfterCheckingResult = AfterCheckingResult,
                     IsAdvanceReturn = IsAdvanceReturn

            };
                db.RewardItems.Add(rewardItems);
                db.SaveChanges();
                return Json("", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message + " حدث خطأ ", JsonRequestBehavior.AllowGet);
            }

        }

        //Post: SaveRewardDetails
        [HttpPost]
        public ActionResult SaveRewardDetails(int? RewardDetailsId, string RewardCategory, string FacultyBranch,
            string EducationLevel, int Allowance, decimal Amount, int AmountType, int EducationType,
        decimal FromRate, int? RateType, decimal ToRate, int RegisterTypeId, int MiniHoursNo,
        int? ClassesNo, int? DuesNo, bool DependOnEducationDays, bool DependOnEducationPeriod, string StudentStatusAcademyId)
        {
            try
            {
                List<int> RewardCategoryIdsList = RewardCategory.Split(',').Select(int.Parse).ToList();
                List<int> FacultyIdsList = FacultyBranch.Split(',').Select(int.Parse).ToList();
                List<int> EducationLevelIdsList = EducationLevel.Split(',').Select(int.Parse).ToList();
                List<int> StudentStatusAcademyIdsList = StudentStatusAcademyId.Split(',').Select(int.Parse).ToList();

                var db = new SchoolAccGam3aEntities();

                foreach (var _RewardCategoryId in RewardCategoryIdsList)
                {
                    foreach (var _FacultyId in FacultyIdsList)
                    {
                        foreach (var _EducationLevelId in EducationLevelIdsList)
                        {
                            foreach (var _StudentStatusAcademyId in StudentStatusAcademyIdsList)
                            {
                                var _RewardMasterModel = db.RewardMaster.Where(x => x.RewardCategory_ID == _RewardCategoryId &&
                                                                                    x.EducationLevel_ID == _EducationLevelId &&
                                                                                    x.Faculties_ID == _FacultyId &&
                                                                                    x.StudentStatusAcademyId == _StudentStatusAcademyId
                                                                               ).FirstOrDefault();
                                if (_RewardMasterModel != null)
                                {
                                    var _RewardDetailsModel = db.RewardDetails.Where(x =>
                                                                            x.RewardMaster_id == _RewardMasterModel.ID &&
                                                                            x.Allowance_ID == Allowance).FirstOrDefault();
                                    if (_RewardDetailsModel != null)
                                    {
                                        _RewardDetailsModel.Allowance_ID = Allowance;
                                        _RewardDetailsModel.Amount = Amount;
                                        _RewardDetailsModel.AmountType_ID = AmountType;
                                        _RewardDetailsModel.EducationType = EducationType;
                                        _RewardDetailsModel.RateType = RateType;
                                        _RewardDetailsModel.FromRate = FromRate;
                                        _RewardDetailsModel.ToRate = ToRate;
                                        _RewardDetailsModel.MiniHoursNo = MiniHoursNo;
                                        _RewardDetailsModel.ClassesNo = ClassesNo;
                                        _RewardDetailsModel.DuesNo = DuesNo;
                                        _RewardDetailsModel.DependOnEducationDays = DependOnEducationDays;
                                        _RewardDetailsModel.DependOnEducationPeriod = DependOnEducationPeriod;
                                        _RewardDetailsModel.RegisterTypeId = RegisterTypeId;
                                        db.Entry(_RewardDetailsModel).State = System.Data.Entity.EntityState.Modified;
                                    }
                                    else
                                    {
                                        var _rewardDetails = new RewardDetails()
                                        {
                                            RewardMaster_id = _RewardMasterModel.ID,
                                            Allowance_ID = Allowance,
                                            Amount = Amount,
                                            AmountType_ID = AmountType,
                                            EducationType = EducationType,
                                            FromRate = FromRate,
                                            ToRate = ToRate,
                                            RegisterTypeId = RegisterTypeId,
                                            MiniHoursNo = MiniHoursNo,
                                            ClassesNo = ClassesNo,
                                            DuesNo = DuesNo,
                                            DependOnEducationDays = DependOnEducationDays,
                                            DependOnEducationPeriod = DependOnEducationPeriod,
                                            RateType = RateType
                                        };
                                        db.RewardDetails.Add(_rewardDetails);
                                    }

                                }
                                else
                                {
                                    var rewardMaster = new RewardMaster()
                                    {
                                        RewardCategory_ID = _RewardCategoryId,
                                        Faculties_ID = _FacultyId,
                                        EducationLevel_ID = _EducationLevelId,
                                        StudentStatusAcademyId = _StudentStatusAcademyId,
                                        UserId = UserID
                                    };
                                    db.RewardMaster.Add(rewardMaster);

                                    var rewardDetails = new RewardDetails()
                                    {
                                        //RewardMaster_id = rewardMaster.ID,
                                        RewardMaster = rewardMaster,
                                        Allowance_ID = Allowance,
                                        Amount = Amount,
                                        AmountType_ID = AmountType,
                                        EducationType = EducationType,
                                        FromRate = FromRate,
                                        ToRate = ToRate,
                                        RegisterTypeId = RegisterTypeId,
                                        MiniHoursNo = MiniHoursNo,
                                        ClassesNo = ClassesNo,
                                        DuesNo = DuesNo,
                                        DependOnEducationDays = DependOnEducationDays,
                                        DependOnEducationPeriod = DependOnEducationPeriod,
                                        RateType = RateType
                                    };
                                    db.RewardDetails.Add(rewardDetails);

                                }
                            }
                        }
                    }

                }
                db.SaveChanges();
                return Json("", JsonRequestBehavior.AllowGet);


                //if (RewardDetailsId != null && RewardDetailsId > 0)
                //{
                //    var RewardDetailModel = db.RewardDetails.Where(x => x.ID == RewardDetailsId).SingleOrDefault();
                //    if (RewardDetailModel != null)
                //    {
                //        var _RewardMasterModel = db.RewardMaster.Where(x => x.ID == RewardDetailModel.RewardMaster_id).SingleOrDefault();
                //        if (_RewardMasterModel != null)
                //        {
                //            _RewardMasterModel.RewardCategory_ID = RewardCategory;
                //            _RewardMasterModel.Faculties_ID = FacultyBranch;
                //            _RewardMasterModel.EducationLevel_ID = EducationLevel;
                //            _RewardMasterModel.StudentStatusAcademyId = StudentStatusAcademyId;

                //            db.Entry(_RewardMasterModel).State = System.Data.Entity.EntityState.Modified;

                //            RewardDetailModel.Allowance_ID = Allowance;
                //            RewardDetailModel.Amount = Amount;
                //            RewardDetailModel.AmountType_ID = AmountType;
                //            RewardDetailModel.EducationType = EducationType;
                //            RewardDetailModel.RateType = RateType;
                //            RewardDetailModel.FromRate = FromRate;
                //            RewardDetailModel.ToRate = ToRate;
                //            //RewardDetailModel.RegisterTypeId = RegisterTypeId;
                //            //RewardDetailModel.HoursNo = HoursNo;
                //            RewardDetailModel.MiniHoursNo = MiniHoursNo;
                //            RewardDetailModel.ClassesNo = ClassesNo;
                //            RewardDetailModel.DuesNo = DuesNo;
                //            RewardDetailModel.DependOnEducationDays = DependOnEducationDays;
                //            RewardDetailModel.DependOnEducationPeriod = DependOnEducationPeriod;
                //            RewardDetailModel.RegisterTypeId = RegisterTypeId;
                //            db.Entry(RewardDetailModel).State = System.Data.Entity.EntityState.Modified;

                //            db.SaveChanges();
                //            return Json("", JsonRequestBehavior.AllowGet);
                //        }
                //        else
                //        {
                //            return Json("حدث خطأ", JsonRequestBehavior.AllowGet);
                //        }// الأب مش موجود على الرغم من أن له أبناء
                //    }
                //    else
                //    {
                //        return Json("حدث خطأ برجاء إعادة المحاولة مرة أخري", JsonRequestBehavior.AllowGet);
                //    }

                //}// Edit
                //else
                //{
                //    var RewardMasterModel = db.RewardMaster.Where(x => x.RewardCategory_ID == RewardCategory &&
                //                                                       x.Faculties_ID == FacultyBranch &&
                //                                                       x.EducationLevel_ID == EducationLevel &&
                //                                                       x.StudentStatusAcademyId == StudentStatusAcademyId).SingleOrDefault();

                //    if (RewardMasterModel != null)
                //    {
                //        RewardMasterModel.RewardCategory_ID = RewardCategory;
                //        RewardMasterModel.Faculties_ID = FacultyBranch;
                //        RewardMasterModel.EducationLevel_ID = EducationLevel;
                //        RewardMasterModel.StudentStatusAcademyId = StudentStatusAcademyId;
                //        db.Entry(RewardMasterModel).State = System.Data.Entity.EntityState.Modified;
                //        db.SaveChanges();

                //        var rewardDetails = new RewardDetails()
                //        {
                //            RewardMaster_id = RewardMasterModel.ID,
                //            Allowance_ID = Allowance,
                //            Amount = Amount,
                //            AmountType_ID = AmountType,
                //            EducationType = EducationType,
                //            FromRate = FromRate,
                //            ToRate = ToRate,
                //            RegisterTypeId = RegisterTypeId,
                //            //StudentStatusAcademyId = StudentStatusAcademyId,
                //            MiniHoursNo = MiniHoursNo,
                //            ClassesNo = ClassesNo,
                //            DuesNo = DuesNo,
                //            DependOnEducationDays = DependOnEducationDays,
                //            DependOnEducationPeriod = DependOnEducationPeriod,
                //            RateType = RateType

                //        };
                //        db.RewardDetails.Add(rewardDetails);
                //        db.SaveChanges();
                //        return Json("", JsonRequestBehavior.AllowGet);
                //    } // تحديث الاب وإضافة الأبناء
                //    else
                //    {
                //        var rewardMaster = new RewardMaster()
                //        {
                //            RewardCategory_ID = RewardCategory,
                //            Faculties_ID = FacultyBranch,
                //            EducationLevel_ID = EducationLevel,
                //            StudentStatusAcademyId = StudentStatusAcademyId
                //        };
                //        db.RewardMaster.Add(rewardMaster);
                //        //db.SaveChanges();

                //        var rewardDetails = new RewardDetails()
                //        {
                //            RewardMaster_id = rewardMaster.ID,
                //            Allowance_ID = Allowance,
                //            Amount = Amount,
                //            AmountType_ID = AmountType,
                //            EducationType = EducationType,
                //            FromRate = FromRate,
                //            ToRate = ToRate,
                //            RegisterTypeId = RegisterTypeId,
                //            //StudentStatusAcademyId = StudentStatusAcademyId,
                //            MiniHoursNo = MiniHoursNo,
                //            ClassesNo = ClassesNo,
                //            DuesNo = DuesNo,
                //            DependOnEducationDays = DependOnEducationDays,
                //            DependOnEducationPeriod = DependOnEducationPeriod,
                //            RateType = RateType
                //        };
                //        db.RewardDetails.Add(rewardDetails);
                //        db.SaveChanges();
                //        return Json("", JsonRequestBehavior.AllowGet);
                //    } // إضافة الأب والأبناء
                //}// Add
            }
            catch (Exception ex)
            {
                return Json(ex.Message + " حدث  خطأ ", JsonRequestBehavior.AllowGet);
            }

        }

        //Delete: RewardDetails
        [HttpDelete]
        public ActionResult DeleteRewardDetails(int RewardDetailsId)
        {
            var db = new SchoolAccGam3aEntities();

            var model = db.RewardDetails.Where(x => x.ID == RewardDetailsId).FirstOrDefault();
            if (model == null)
            {
                return Json("لايمكن إتمام عملية الحذف", JsonRequestBehavior.AllowGet);
            }

            if (db.StudentPayroll.Any(x => x.Item_ID == model.Allowance_ID) == false)
            {
                db.RewardDetails.Remove(model);
                db.SaveChanges();
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json("لايمكن الحذف لإرتباطه بسجلات أخري ", JsonRequestBehavior.AllowGet);
        }

        //Delete: RewardItem
        [HttpDelete]
        public ActionResult DeleteRewardItem(int RewardItemId)
        {
            var db = new SchoolAccGam3aEntities();

            var model = db.RewardItems.Where(x => x.ID == RewardItemId).FirstOrDefault();
            if (model == null)
            {
                return Json("لايمكن إتمام عملية الحذف", JsonRequestBehavior.AllowGet);
            }

            if (db.RewardDetails.Any(x => x.Allowance_ID == model.ID) == false)
            {
                db.RewardItems.Remove(model);
                db.SaveChanges();
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json("لايمكن الحذف لإرتباطه بسجلات أخري ", JsonRequestBehavior.AllowGet);
        }

        //Delete: RewardCategory
        [HttpDelete]
        public ActionResult DeleteRewardCategory(int RewardCategoryId)
        {
            var db = new SchoolAccGam3aEntities();

            var model = db.RewardCategory.Where(x => x.ID == RewardCategoryId).FirstOrDefault();
            if (model == null)
            {
                return Json("لايمكن إتمام عملية الحذف", JsonRequestBehavior.AllowGet);
            }

            if (db.RewardMaster.Any(x => x.RewardCategory_ID == model.ID) == false)
            {
                var RewardNationaltityModel = db.RewardNationaltity.Where(x => x.RewardCategory_Id == model.ID);
                if (RewardNationaltityModel != null)
                {
                    db.RewardNationaltity.RemoveRange(RewardNationaltityModel);
                }
                db.RewardCategory.Remove(model);
                db.SaveChanges();
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json("لايمكن الحذف لإرتباطه بسجلات أخري ", JsonRequestBehavior.AllowGet);
        }

        #region Permissions
        private int UserID
        {
            get
            {
                var user = HttpContext.Session["UserId"] as DashBoard_Users;

                if (user != null && user.ID != 0)
                {
                    return user.ID;
                }
                else
                {
                    return 0;
                }
            }

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


        #endregion

    }
}