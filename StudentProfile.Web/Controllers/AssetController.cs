using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class AssetController : Controller
    {
        private SchoolAccGam3aEntities _schoolCtx = new SchoolAccGam3aEntities();
        EsolERPEntities _accountingCtx = new EsolERPEntities();

        // GET: Asset
        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(58);
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

        public ActionResult _LocationsTree()
        {
            var modelList = _accountingCtx.GetLocationsTree_Sp().ToList();
            return PartialView("_LocationsTree", modelList);
        }

        public JsonResult GetAssetBranchies()
        {
            //var user = Session["UserId"] as DashBoard_Users;

            var BranchiesQ = (from p in _accountingCtx.AssetLocationsWithLevelsSelect_Vw
                              join y in _accountingCtx.COM on p.Com_ID equals y.COMID
                              join c in _accountingCtx.Con_Company on y.COMID equals c.ComID
                              where p.level == 1 //&& c.CON_ID == user.ID
                              select new { p.ID,  p.LocationName,y.COMName }).Distinct().ToList();
            return this.Json(BranchiesQ, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetAssetDepartment(int BranchId)
        {
            var DepartmentsQ = (from p in _accountingCtx.AssetLocationsWithLevelsSelect_Vw
                                join y in _accountingCtx.COM on p.Com_ID equals y.COMID
                                where p.ParentID == BranchId && p.level == 2
                                select new { p.ID, p.LocationName }).ToList();
            return this.Json(DepartmentsQ, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetFloors(int DepartmentId)
        {
            var DepartmentsQ = (from p in _accountingCtx.AssetLocationsWithLevelsSelect_Vw
                                join y in _accountingCtx.COM on p.Com_ID equals y.COMID
                                where p.ParentID == DepartmentId && p.level == 3
                                select new { p.ID, p.LocationName }).ToList();
            return this.Json(DepartmentsQ, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFloorSite(int _SiteId)
        {
            var SiteList = _accountingCtx.FloorDepComForSiteSelect_Sp(_SiteId).SingleOrDefault();

            return Json(SiteList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationsTree()
        {
            var LocationTreeQ = (from p in _accountingCtx.Ast_Location select new { p.ID, p.ParentID, p.LocationName }).ToList();

            return this.Json(LocationTreeQ, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAssetLocationClass()
        {
            var AssetLocations = _accountingCtx.Ast_LocationClass
                .Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();

            return Json(AssetLocations, JsonRequestBehavior.AllowGet);
        }

        public int GetDefaultCompany()
        {
            if (Session["HeaderCompanyID"] != null)
            {
                return int.Parse(Session["HeaderCompanyID"].ToString());
            }
            else
            {
                var user = Session["UserId"] as DashBoard_Users;

                int comId = _accountingCtx.Usp_Company_Select_ByUserID(user.ID).FirstOrDefault().COMID;
                return comId;
            }
        }

        public JsonResult GetAstNames()
        {
            _schoolCtx.Configuration.ProxyCreationEnabled = false;
            var astNames = _accountingCtx.AstNames.Select(x => new { x.ID, x.Name, x.Cat_Id, x.ComID, x.CAT.CAT_Name }).ToList();
            return Json(astNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoa()
        {
            var CoaList = (from p in _accountingCtx.COA where p.COA_Type == "S" select new { p.COAID, p.COADescription }).ToList();
            return this.Json(CoaList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoaAssets()
        {
            string MainAssetAcc = _accountingCtx.usp_GetConfigValues("JournalEntryOperation").FirstOrDefault().ConfigValue;//_accountingCtx.Config.Where(x => x.ConfigKey == "FixedAssets").FirstOrDefault().ConfigValue;

            if (MainAssetAcc != null)
            {
                string mainAstsCode = _accountingCtx.COA.Where(x => x.COAID.ToString() == MainAssetAcc).SingleOrDefault().COACode;
                var CoaList = _accountingCtx.COA.Where(x => x.COACode.StartsWith(mainAstsCode) && x.COA_Type == "M")
                    .Select(x => new { x.COAID, x.COADescription }).ToList();

                return this.Json(CoaList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var CoaList = string.Empty;
                return this.Json(CoaList, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCoaAsset()
        {
            var CoaList = (from p in _accountingCtx.COA where p.COAID == 166 select new { p.COAID, p.COADescription }).ToList();
            return this.Json(CoaList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanies()
        {
            var user = Session["UserId"] as DashBoard_Users;

            var CompaniesList = (_accountingCtx.Usp_Company_Select_ByUserID(user.ID)).ToList();
            return this.Json(CompaniesList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCat()
        {
            var CatList = (from p in _accountingCtx.CAT select new { p.CAT_ID, p.CAT_Name, p.COA_ID, p.DEP_Percent, p.DepYears })
                .ToList();
            return this.Json(CatList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCatById(int? id)
        {
            List<object> result = new List<object>();
            var model = _accountingCtx.CAT.Where(x => x.CAT_ID == id).Select(x =>
                    new { x.CAT_ID, x.CAT_Name, x.COA_ID, x.MainDepAcc, x.MainTotalDepAcc, x.DEP_Percent, x.DepYears })
                .SingleOrDefault();


            var CoaList = _accountingCtx.COA.Where(x => x.COAID == model.COA_ID).SingleOrDefault();
            var _CoaList = _accountingCtx.COA.Where(x => x.COAID == CoaList.COA_ID).Select(x => new { x.COAID, x.COADescription })
                .ToList();

            result.Add(model);
            result.Add(_CoaList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCatFilter(int? prmComId, int? prmDepartmentId, int? prmFloorId, int? prmSiteId,
            int? prmCatId)
        {
            int? ComId = null;
            int? DepartmentId = null;
            int? FloorId = null;
            int? SiteId = null;
            int? CatId = null;


            if (prmComId != null)
            {
                ComId = prmComId;
            }

            if (prmDepartmentId != null)
            {
                DepartmentId = prmDepartmentId;
            }

            if (prmFloorId != null)
            {
                FloorId = prmFloorId;
            }

            if (prmSiteId != null)
            {
                SiteId = prmSiteId;
            }


            var CatList =
                (from p in _accountingCtx.FilterAssets_Sp(ComId, DepartmentId, FloorId, SiteId, CatId, null,null)
                 select new { p.CAT_ID, p.CAT_Name }).Distinct().ToList();
            return this.Json(CatList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetAssetsFilter(int? prmComId, int? prmDepartmentId, int? prmFloorId, int? prmSiteId,
            int? prmCatId)
        {
            int? ComId = null;
            int? DepartmentId = null;
            int? FloorId = null;
            int? SiteId = null;
            int? CatId = null;


            if (prmComId != null)
            {
                ComId = prmComId;
            }

            if (prmDepartmentId != null)
            {
                DepartmentId = prmDepartmentId;
            }

            if (prmFloorId != null)
            {
                FloorId = prmFloorId;
            }

            if (prmSiteId != null)
            {
                SiteId = prmSiteId;
            }

            if (prmCatId != null)
            {
                CatId = prmCatId;
            }


            var AssetList =
                (from p in _accountingCtx.FilterAssets_Sp(ComId, DepartmentId, FloorId, SiteId, CatId, null, null)
                 select new { p.AstNameId, p.AstName }).Distinct().ToList();
            return this.Json(AssetList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveSite(string LocationName, int FloorId, int BranchId, int ClassID, int Height, int Width, int Depth,int BedsPerRoom,string SiteNumber)
        {        
                var SiteQ = (from p in _accountingCtx.Ast_Location
                             where p.LocationName == LocationName && p.ParentID == FloorId
                             select new { p.LocationName }).ToList();
                if (SiteQ.Any())
                {
                    return Content("عفوا تم اضافة نفس الموقع على نفس الدور سابقا و لا يمكن اضافتة مرة اخرى");
                }


                _accountingCtx.SiteInsert_Sp(BranchId, FloorId, LocationName, ClassID, Width,Height, Depth);
                _accountingCtx.SaveChanges();

            //اضافة السعة السريرية [HousingLocationFurniture]
            var user = Session["UserId"] as DashBoard_Users;

                //int code = 1;
                for (int i = 0; i < BedsPerRoom; i++)
                {
                    //var HousingFurnitureCount = _schoolCtx.HousingLocationFurniture.ToList().LastOrDefault();
                    //if (HousingFurnitureCount != null)
                    //    code = int.Parse(HousingFurnitureCount.Barcode) + 1;

                    HousingLocationFurniture housingLocationFurniture = new HousingLocationFurniture()
                    {
                        FurnitureId = null,
                        LocationId = _accountingCtx.Ast_Location.Max(x=>x.ID),
                        userId = user.ID,
                        Barcode = (i+1).ToString(),//code.ToString(),
                        InsertionDate = DateTime.Now.Date
                    };
                    _schoolCtx.HousingLocationFurniture.Add(housingLocationFurniture);
                    _schoolCtx.SaveChanges();

                }

            return Content(string.Empty);
        }

        public ActionResult UpdateSite(string LocationName, int FloorId, int SiteId, int ClassID, int Height, int Width, int Depth,int BedsPerRoom ,string SiteNumber)
        {
            var SiteQ = (from p in _accountingCtx.Ast_Location
                         where p.LocationName == LocationName && p.ParentID == FloorId && p.ID != SiteId
                         select new { p.LocationName }).ToList();
            if (SiteQ.Any())
            {
                return Content("عفوا تم اضافة نفس الموقع على نفس الدور سابقا و لا يمكن اضافتة مرة اخرى");
            }


            Ast_Location _Ast_Location = _accountingCtx.Ast_Location.Single(x => x.ID == SiteId);
            _Ast_Location.LocationName = LocationName;
            _Ast_Location.ParentID = FloorId;
            _Ast_Location.ClassID = ClassID;
            _Ast_Location.width = Width;
            _Ast_Location.Height = Height;
            _Ast_Location.Depth = Depth;
            _Ast_Location.BedsPerRoom = BedsPerRoom;
            _Ast_Location.UnitNumber = SiteNumber;

            _accountingCtx.SaveChanges();
            return Content(string.Empty);
        }

        public ActionResult AddNewBranchToAssetLocations(int BranchId, string BranchName, string UnitNumber, bool IsFamilial)
        {
            var BranchLocationQ = (from p in _accountingCtx.Ast_Location
                                   where p.ParentID == null && p.Com_ID == BranchId
                                   select new { p.LocationName }).ToList();

            if (BranchLocationQ.Any())
            {
                string OldBranchLocationName = BranchLocationQ.First().LocationName;
                return Content(" عفوا الوحدة الإدارية مسجلة من قبل فى دليل المواقع باسم " + OldBranchLocationName);
            }


            Ast_Location _Ast_Location = new Ast_Location();
            _Ast_Location.LocationName = BranchName;
            _Ast_Location.ParentID = null;
            _Ast_Location.IsDefault = null;
            _Ast_Location.IsShown = null;
            _Ast_Location.IsMain = true;
            _Ast_Location.Com_ID = BranchId;
            _Ast_Location.UnitNumber = UnitNumber;
            _Ast_Location.IsFamilial = IsFamilial;

            _accountingCtx.Ast_Location.Add(_Ast_Location);
            _accountingCtx.SaveChanges();


            return Content(string.Empty);
        }

        public ActionResult UpDateNewBranchToAssetLocations(int BranchLocationId, string BranchLocationName)
        {
            var BranchLocationNameQ = (from p in _accountingCtx.Ast_Location
                                       where p.ID != BranchLocationId && p.ParentID == null && p.LocationName == BranchLocationName
                                       select new { p.LocationName }).ToList();
            if (BranchLocationNameQ.Any())
            {
                return Content(
                    "عفوا اسم الوحدة الإدارية فى دليل مواقع الاصول الذى تم ادخالة للتعديل موجودة فى الدليل على نفس المستوى من فضلك قم بتغير الاسم");
            }

            Ast_Location _Ast_Location = _accountingCtx.Ast_Location.Single(x => x.ID == BranchLocationId);
            _Ast_Location.LocationName = BranchLocationName;
            _accountingCtx.SaveChanges();


            return Content(string.Empty);
        }

        public ActionResult DeleteNewBranchToAssetLocations(int BranchLocationId)
        {
            try
            {
                var BranchQ = (from p in _accountingCtx.Ast_Location where p.ParentID == BranchLocationId select new { p.ParentID })
                    .ToList();
                if (BranchQ.Any())
                {
                    return Content("عفوا الموقع المواد حذفة به اقسام مسجلة ");
                }


                _accountingCtx.Ast_Location.RemoveRange(_accountingCtx.Ast_Location.Where(x => x.ID == BranchLocationId).ToList());
                _accountingCtx.SaveChanges();
            }
            catch
            {
                return Content("عفوا لا يمكن حذف الموقع");
            }

            return Content(string.Empty);
        }

        public ActionResult UpDateDepartment(int DepartmentId, int BranchId, string DepartmentName)
        {
            var DepartmentQ = (from p in _accountingCtx.Ast_Location
                               where p.ID != DepartmentId && p.ParentID == BranchId && p.LocationName == DepartmentName
                               select new { p.LocationName }).ToList();

            if (DepartmentQ.Any())
            {
                return Content("عفوا اسم القسم الذى قمت بادخالة مسجل على نفس الوحدة الإدارية");
            }


            Ast_Location _Ast_Location = _accountingCtx.Ast_Location.Single(x => x.ID == DepartmentId);
            _Ast_Location.ParentID = BranchId;
            _Ast_Location.LocationName = DepartmentName;
            _accountingCtx.SaveChanges();
            return Content(string.Empty);
        }

        public ActionResult DeleteDepartment(int DepartmentId)
        {
            var DepartmentQ = (from p in _accountingCtx.Ast_Location where p.ParentID == DepartmentId select new { p.LocationName })
                .ToList();

            if (DepartmentQ.Any())
            {
                return Content("عفوا القسم المراد حذفة به ادوار مسجلة");
            }

            _accountingCtx.Ast_Location.RemoveRange(_accountingCtx.Ast_Location.Where(x => x.ID == DepartmentId).ToList());
            _accountingCtx.SaveChanges();
            return Content(string.Empty);
        }

        public ActionResult InsertDepartment(int BranchId, string DepartmentName)
        {
            var DepartmentQ = (from p in _accountingCtx.Ast_Location
                               where p.ParentID == BranchId && p.LocationName == DepartmentName
                               select new { p.LocationName }).ToList();

            if (DepartmentQ.Any())
            {
                return Content("عفوا اسم القسم الذى قمت بادخالة مسجل على نفس الوحدة الإدارية");
            }


            int BranchOriginal = _accountingCtx.Ast_Location.Single(x => x.ID == BranchId).Com_ID.Value;

            Ast_Location _Ast_Location = new Ast_Location();
            _Ast_Location.ParentID = BranchId;
            _Ast_Location.LocationName = DepartmentName;
            _Ast_Location.IsDefault = null;
            _Ast_Location.IsShown = null;
            _Ast_Location.IsMain = true;
            _Ast_Location.Com_ID = BranchOriginal;
            _accountingCtx.Ast_Location.Add(_Ast_Location);
            _accountingCtx.SaveChanges();


            return Content(string.Empty);
        }


        public ActionResult InsertFloor(int DepartmentId, string FloorName ,string FloorNumber)
        {
            var FloorQ = (from p in _accountingCtx.Ast_Location
                          where p.ParentID == DepartmentId && p.LocationName == FloorName
                          select new { p.LocationName }).ToList();
            if (FloorQ.Any())
            {
                return Content("عفوا الدور مسجل بالفعل على نفس القسم");
            }

            Ast_Location _Ast_Location = new Ast_Location();
            _Ast_Location.LocationName = FloorName;
            _Ast_Location.UnitNumber = FloorNumber;
            _Ast_Location.ParentID = DepartmentId;
            _Ast_Location.IsMain = true;

            _Ast_Location.IsDefault = null;
            _Ast_Location.IsShown = null;
            _Ast_Location.Com_ID = _accountingCtx.Ast_Location.Single(x => x.ID == DepartmentId).Com_ID.Value;

            _accountingCtx.Ast_Location.Add(_Ast_Location);
            _accountingCtx.SaveChanges();


            return Content(string.Empty);
        }

        public ActionResult UpDateFloor(int FloorId, int DepartmentId, string FloorName, string FloorNumber)
        {
            var FloorQ = (from p in _accountingCtx.Ast_Location
                          where p.ParentID == DepartmentId && p.LocationName == FloorName && p.ID != FloorId
                          select new { p.LocationName }).ToList();

            if (FloorQ.Any())
            {
                return Content("عفوا اسم الدور الذى تم ادخالة موجود مسبقا على نفس الدور");
            }


            Ast_Location _Ast_Location = _accountingCtx.Ast_Location.Single(x => x.ID == FloorId);
            _Ast_Location.LocationName = FloorName;
            _Ast_Location.UnitNumber = FloorNumber;
            _Ast_Location.ParentID = DepartmentId;
            _Ast_Location.IsMain = true;

            _Ast_Location.IsDefault = null;
            _Ast_Location.IsShown = null;
            _Ast_Location.Com_ID = _accountingCtx.Ast_Location.Single(x => x.ID == DepartmentId).Com_ID.Value;
            _accountingCtx.SaveChanges();


            return Content(string.Empty);
        }


        public ActionResult DeleteFloor(int FloorId)
        {
            var FloorQ = (from p in _accountingCtx.Ast_Location where p.ParentID == FloorId select new { p.LocationName }).ToList();
            if (FloorQ.Any())
            {
                return Content("عفوا الدور مسجل به مواقع فرعية ولا يمكن حذف");
            }

            _accountingCtx.Ast_Location.RemoveRange(_accountingCtx.Ast_Location.Where(x => x.ID == FloorId).ToList());
            _accountingCtx.SaveChanges();


            return Content(string.Empty);
        }


        public ActionResult DeleteSite(int SiteId)
        {
            try
            {
                _accountingCtx.Ast_Location.RemoveRange(_accountingCtx.Ast_Location.Where(x => x.ID == SiteId));
                _accountingCtx.SaveChanges();
            }
            catch
            {
                return Content("عفوا لايمكن حذف الموقع لارتباطة بسجلات اخرى");
            }


            return Content(string.Empty);
        }

        public JsonResult GetAssetBranchiesNotInAssetLocations()
        {
            var user = Session["UserId"] as DashBoard_Users;
            var BranchiesNotInQ = (_accountingCtx.SelectBranchesNotInAssetLocations_Sp(user.ID)).ToList();
            var BranchiesInQ = (_accountingCtx.SelectBranchesInAssetLocations_Sp(user.ID)).ToList();
            return this.Json(new { BranchiesNotInQ, BranchiesInQ }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveAssetLocationClass(string Name)
        {
            notify notify = new notify();
            var AllLocationClass = _accountingCtx.Ast_LocationClass.Select(x => x.Name).ToList();
            if (AllLocationClass.Contains(Name))
            {
                return Json(notify = new notify() { Message = "اسم التصنيف موجود بالفعل", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }

            Ast_LocationClass LocationClass = new Ast_LocationClass()
            {
                Name = Name
            };
            _accountingCtx.Ast_LocationClass.Add(LocationClass);
            _accountingCtx.SaveChanges();
            return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        //HosuingFurniture
        public ActionResult HosuingFurnitureCategory()
        {
            var FurnitureCategory = _schoolCtx.HousingFurnitureCategories.Select(x => new SelectListItem { Text = x.Name, Value = 
x.Id.ToString() }).OrderBy(x => x.Text).ToList();

            return Json(FurnitureCategory, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HosuingFurnitureByCategory(int id)
        {
            var Furniture = _schoolCtx.HousingFurnitures.Where(x => x.CategoryId == id).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

            return Json(Furniture, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveHosuingFurniture(int LocationId,int FurnitureId , int Count)
        {
            if (Session["UserId"] != null)
            {

                var user = Session["UserId"] as DashBoard_Users;

                int code = 1;
                //var FurniturePrefix = _schoolCtx.HousingFurnitures.Find(FurnitureId).HousingFurnitureCategories.Prefix;

                for (int i = 0; i < Count; i++)
                {
                    var HousingFurnitureCount = _schoolCtx.HousingLocationFurniture.ToList().LastOrDefault();
                    if (HousingFurnitureCount != null)
                        code = int.Parse(HousingFurnitureCount.Barcode) + 1;

                    //var Barcode = (FurniturePrefix + "-" + code).ToString();

                    HousingLocationFurniture housingLocationFurniture = new HousingLocationFurniture()
                    {
                        FurnitureId = FurnitureId,
                        LocationId = LocationId,
                        userId = user.ID,
                        Barcode = code.ToString(),
                        InsertionDate = DateTime.Now.Date
                    };
                    _schoolCtx.HousingLocationFurniture.Add(housingLocationFurniture);
                    _schoolCtx.SaveChanges();

                }
                return Content(string.Empty);

            }
            return Content("خطأ اثناء الحفظ");
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


    }
}