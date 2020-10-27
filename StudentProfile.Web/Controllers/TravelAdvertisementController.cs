
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StudentProfile.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class TravelAdvertisementController : Controller
    {
        private notify notify = new notify();
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();

        // GET: TravelAdvertisement
        public ActionResult Index()
        {
            //var json = new JavaScriptSerializer().Serialize(GetPermissions(42).Data);
            var permissions = CheckPermissionsfn(42);
            ViewBag.View = permissions.View;
            ViewBag.Read = permissions.Read;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Update = permissions.Update;
            ViewBag.Create = permissions.Create;

            if (permissions.Read)
            {
                return View();
            }
            return RedirectToAction("NoPermissions", "Security");
        }

        #region Travel Config
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult SaveTravelConfig(config model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    if (string.IsNullOrEmpty(model.Value))
                    {
                        return Json(notify = new notify() { Message = "ادخل الاسم", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                    }

                    if (model.ID == 0 || model.ID == null)
                    {
                        dbSC.config.Add(model);
                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var ConfigID = dbSC.config.Find(model.ID);
                        ConfigID.Value = model.Value;
                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم التعديل بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult GetTravelConfigByKay(string kay)
        {
            var ConfigKay = dbSC.config.Where(x => x.Kay == kay).Select(x => new { ID = x.ID, Value = x.Value }).ToList();
            return Json(ConfigKay, JsonRequestBehavior.AllowGet);
        }

        #endregion


        public ActionResult TransportationTracking()
        {
            var permissions = CheckPermissionsfn(114);
            ViewBag.View = permissions.View;
            ViewBag.Read = permissions.Read;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Update = permissions.Update;
            ViewBag.Create = permissions.Create;

            if (permissions.Read)
            {
                return View();
            }
            return RedirectToAction("NoPermissions", "Security");
            
        }

        [HttpGet]
        public JsonResult GetAllNationalities()
        {
            var result = dbSC.Nationality.Select(x => new {ID =  x.ID,Name= x.NationalityName }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllTransportationTracking()
        {
            var result = dbSC.TransportationTracking.Select(x => new {ID=  x.ID, Nationality_ID= x.Nationality_ID, NationalityName=x.Nationality.NationalityName, Tracking= x.Tracking,IsActive=x.IsActive==true?"نعم":"لا" , FlightsType  =x.FlightsType =="O"? "ذهاب فقط":"ذهاب وعودة"}).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTransportationTracking(int id)
        {
            if (id > 0)
            {
                var TransportationTracking = dbSC.TransportationTracking.Where(x => x.ID == id).SingleOrDefault();
                if (TransportationTracking != null)
                {
                    dbSC.TransportationTracking.Remove(TransportationTracking);
                }
                dbSC.SaveChanges();
                return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(notify = new notify() { Message = "خطأ اثناء حذف ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetTransportationTrackingByID(int ID)
        {
            if (ID== 0)
            {
                return Content("يوجد خطأ");
            }
            else
            {


                var TransportationTracking = dbSC.TransportationTracking.Where(x => x.ID == ID).Select(o=>new { o.ID ,o.Nationality_ID ,o.Tracking,o.IsActive , o.FlightsType }).SingleOrDefault();
                if (TransportationTracking != null)
                {
                    return Json(TransportationTracking, JsonRequestBehavior.AllowGet);

                }
                return Content("");
            }
        }
        [HttpPost]
        public ActionResult addTransportationTracking(string TrackingName,int? NationalityID,bool IsActive,string FlightsType)
        {


            if (NationalityID == null)
                return Content("يرجى اختيار الجنسيه");
            if (String.IsNullOrEmpty(TrackingName))
                return Content("يرجي كتابة خط السير");
            else
            {
                TransportationTracking transportationTracking = new TransportationTracking();
                transportationTracking.Nationality_ID = NationalityID;
                transportationTracking.Tracking = TrackingName;
                transportationTracking.IsActive = IsActive;
                transportationTracking.FlightsType = FlightsType;
                dbSC.TransportationTracking.Add(transportationTracking);
                dbSC.SaveChanges();
            }
            return Content("");
        }



        [HttpPost]
        public ActionResult updateTransportationTracking(int ID,string TrackingName, int? NationalityID, bool IsActive,string FlightsType)
        {
            if (ID == 0)
                return Content("يوجد خطأ");
            if (NationalityID == null)
                return Content("يرجى اختيار الجنسيه");
            if (String.IsNullOrEmpty(TrackingName))
                return Content("يرجي كتابة خط السير");

            else
            {
                TransportationTracking transportationTracking = new TransportationTracking();
                transportationTracking = dbSC.TransportationTracking.Find(ID);
                transportationTracking.Nationality_ID = NationalityID;
                transportationTracking.Tracking = TrackingName;
                transportationTracking.IsActive = IsActive;
                transportationTracking.FlightsType = FlightsType;
                dbSC.Entry(transportationTracking).State = EntityState.Modified;
                dbSC.SaveChanges();
            }
            return Content("");
        }
        public async Task<ActionResult> GetPurpose()
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.config.Where(x => x.Kay == "purpose").Select(x => new SelectListItem { Text = x.Value, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList()),
            JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult GetTravelAgent()
        {
            var purpose = dbSC.config.Where(x => x.Kay == "Agent").Select(x => new SelectListItem { Text = x.Value, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(purpose, JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult GetScholarships()
        {
            var Scholarships = dbSC.config.Where(x => x.Kay == "Scholarship").Select(x => new SelectListItem { Text = x.Value, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(Scholarships, JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult GetLevels()
        {
            var Level = dbSC.config.Where(x => x.Kay == "Level").Select(x => new SelectListItem { Text = x.Value, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(Level, JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult GetNationalities()
        {
            var Nationalty = dbSC.usp_getNationalities().Select(x => new SelectListItem { Text = x.NATIONALITY_DESC, Value = x.NATIONALITY_CODE.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(Nationalty, JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult GetDegrees()
        {
            var Degrees = dbSC.usp_getDegrees().Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(Degrees, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCustomFields(string ParentId)
        {

             var CustomFields = dbSC.Usp_GetCustomFields(ParentId).Select(x => new SelectListItem { Text = x.Key, Value = x.CustomFieldId.ToString() }).ToList();

            return Json(CustomFields, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MajorGetCustomFields()
        {
            var Degrees = dbSC.CustomFields.Where(x => x.ParentId == null).Select(x => new SelectListItem { Text = x.Key, Value = x.CustomFieldId.ToString() }).OrderBy(x => x.Value).ToList();
            return Json(Degrees, JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult GetStatus()
        {
            var Status = dbSC.usp_getStatus().Select(x => new SelectListItem { Text = x.STATUS_DESC, Value = x.STATUS_CODE.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(Status, JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult SaveTravelAdvertisement(TravelAdvertisement model)
        {
            //if (ModelState.IsValid)
            //{
                try
                {
                    if (model != null)
                    {
                        var CurrentUser = Session["UserId"] as DashBoard_Users;
                        if (model.ID == 0 || model.ID == null)
                        {
                            model.UserID = CurrentUser.ID;
                            model.IsActive = true;
                            dbSC.TravelAdvertisement.Add(model);
                            dbSC.SaveChanges();
                            return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var AdData = dbSC.TravelAdvertisement.Find(model.ID);
                            AdData.AdName = model.AdName;
                            AdData.TravelClass = model.TravelClass;
                            AdData.flightsType = model.flightsType;
                            AdData.AgentID = model.AgentID;
                            AdData.PurposeID = model.PurposeID;
                            AdData.IsAppearToExpectedGradutedPeople = model.IsAppearToExpectedGradutedPeople;
                            AdData.AdStartDate = model.AdStartDate;
                            AdData.AdEndDate = model.AdEndDate;
                            AdData.DepartingStart = model.DepartingStart;
                            AdData.DepartingEnd = model.DepartingEnd;
                            AdData.ReturningStart = model.ReturningStart;
                            AdData.ReturningEnd = model.ReturningEnd;
                            AdData.NationalityID = model.NationalityID;
                            AdData.DegreeID = model.DegreeID;
                            AdData.StStatusID = model.StStatusID;
                            AdData.ScholarshipType = model.ScholarshipType;
                            AdData.Notes = model.Notes;
                            AdData.UserID = CurrentUser.ID;
                            AdData.IsActive = true;

                            dbSC.SaveChanges();
                            return Json(notify = new notify() { Message = "تم التعديل بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                catch (Exception ex)
                {
                    return Json(ex.Message + " " + ex.InnerException);
                }
            //}
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        #region Advertisement List
        //Advertisement  List
        public ActionResult Advertisement()
        {
            //var json = new JavaScriptSerializer().Serialize(GetPermissions(42).Data);
            var permissions = CheckPermissionsfn(42);
            ViewBag.View = permissions.View;
            ViewBag.Read = permissions.Read;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Update = permissions.Update;
            ViewBag.Create = permissions.Create;

            if (permissions.Read || permissions.View)
            {
                return View();
            }
            return RedirectToAction("NoPermissions", "Security");
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult GetAdvertisement()
        {
            var Advertisement = dbSC.TravelAdvertisement.ToList().Select(x => new
            {
                ID = x.ID,
                AdName = x.AdName,
                AdStartDate = x.AdStartDate.ToString("dd/MM/yyyy"),
                AdEndDate = x.AdEndDate.ToString("dd/MM/yyyy"),
                DepartingStart = x.DepartingStart.ToString("dd/MM/yyyy"),
                DepartingEnd = x.DepartingEnd.ToString("dd/MM/yyyy"),
                ReturningStart = x.ReturningStart == null ? null : x.ReturningStart.Value.ToString("dd/MM/yyyy"),
                ReturningEnd = x.ReturningEnd == null ? null : x.ReturningEnd.Value.ToString("dd/MM/yyyy"),
                User = x.DashBoard_Users.Name,
                IsActive = x.IsActive
            }).OrderByDescending(x => x.ID).ToList();
            return Json(Advertisement, JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult ActiveAdvertisement(int id)
        {
            if (ModelState.IsValid)
            {
                var AdvertisementId = dbSC.TravelAdvertisement.Find(id);
                if (AdvertisementId != null)
                {
                    if (AdvertisementId.IsActive == true)
                    {
                        AdvertisementId.IsActive = false;

                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم الايقاف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    if (AdvertisementId.IsActive == false)
                    {
                        AdvertisementId.IsActive = true;

                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم التنشيط بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
        }
        //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper(ScreenId = 42)]
        public ActionResult GetAdvertisementById(int id)
        {
            if (id > 0)
            {
                var Advertisement = dbSC.TravelAdvertisement.ToList().Where(x => x.ID == id).Select(x => new
                {
                    AdName = x.AdName,
                    Notes = x.Notes,
                    flightsType = x.flightsType,
                    TravelClass = x.TravelClass,
                    ScholarshipType = x.ScholarshipType,
                    StStatusID = x.StStatusID,
                    AgentID = x.AgentID,
                    DegreeID = x.DegreeID,
                    NationalityID = x.NationalityID,
                    PurposeID = x.PurposeID,
                    AdStartDate = x.AdStartDate.ToString("MM/dd/yyyy"),
                    AdEndDate = x.AdEndDate.ToString("MM/dd/yyyy"),
                    DepartingStart = x.DepartingStart.ToString("MM/dd/yyyy"),
                    DepartingEnd = x.DepartingEnd.ToString("MM/dd/yyyy"),
                    ReturningStart = x.ReturningStart == null ? null : x.ReturningStart.Value.ToString("MM/dd/yyyy"),
                    ReturningEnd = x.ReturningEnd == null ? null : x.ReturningEnd.Value.ToString("MM/dd/yyyy"),
                    CustomFieldId = x.CustomFieldId,
                    IsAppearToExpectedGradutedPeople = x.IsAppearToExpectedGradutedPeople
                }).ToList().FirstOrDefault();

                return Json(Advertisement, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Permissions

        public JsonResult GetPermissions(int screenId)
        {

            var CurrentUser = Session["UserId"] as DashBoard_Users;

            //int userId = int.Parse(Session["UserId"].ToString());


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
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        public class Permissions
        {
            public bool Create { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }
            public bool Read { get; set; }
            public bool View { get; set; }
        }



        public Permissions CheckPermissionsfn(int screenId)
        {

            var CurrentUser = Session["UserId"] as DashBoard_Users;

            //int userId = int.Parse(Session["UserId"].ToString());


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
            }

            return permissions;
        }




        #endregion

    }
}