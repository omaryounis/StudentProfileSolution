using Newtonsoft.Json;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.Entity;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using StudentProfile.Components.Helpers;
using System.Collections.Generic;

namespace StudentProfile.Controllers
{
    public partial class MusaferController : Controller
    {
        private notify notify = new notify();
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();



        [HttpPost]
        ///UploadImage
        public ActionResult UploadImage()
        {
            try
            {
                if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{int.Parse(Session["UserId"].ToString())}")))
                    Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{int.Parse(Session["UserId"].ToString())}"));

                if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{int.Parse(Session["UserId"].ToString())}/المستندات")))
                    Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{int.Parse(Session["UserId"].ToString())}/المستندات"));

                HttpPostedFileBase file = Request.Files[0];
                file.SaveAs(Server.MapPath($"~/Content/UserFiles/{int.Parse(Session["UserId"].ToString())}/المستندات/Update[{int.Parse(Session["UserId"].ToString())}]Passport.jpg"));

                if (!Directory.Exists(Server.MapPath($"~/Content/tempfiles")))
                    Directory.CreateDirectory(Server.MapPath($"~/Content/tempfiles"));

                HttpPostedFileBase fileTemp = Request.Files[0];
                fileTemp.SaveAs(Server.MapPath($"~/Content/tempfiles/Update[{int.Parse(Session["UserId"].ToString())}]Passport.jpg"));

                return Json(notify = new notify() { Message = "تم رفع الصورة بانتظار مراجعتها من قبل الادارة", Type = "seccess", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(notify = new notify() { Message = "خطأ اثناءرفع الصورة", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }



        #region Verify
        [HttpGet]
        public ActionResult Index(int? adID)
        {
            //var json = new JavaScriptSerializer().Serialize(GetPermissions(41).Data);
            //var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            //ViewBag.View = permissions.View;
            //ViewBag.Read = permissions.Read;
            //ViewBag.SendCode = permissions.SendCode;
            //ViewBag.UploadeImage = permissions.UploadeImage;
            //ViewBag.ConfirmCode = permissions.ConfirmCode;
            //ViewBag.AddRequest = permissions.AddRequest;

            //if (permissions.View)
            //{
            return View();
            //}
            //return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult SendEmailConfirmedCode()
        {
            VerificationCode VerificationCode = new VerificationCode();
            CustomMailMessage CustomMailMessage = new CustomMailMessage();
            try
            {
                //Generate Code 
                string GenerateCode = VerificationCode.Generate(6);
                Session["MailCode"] = GenerateCode.ToString();
                if (Session["UserId"] != null)
                {
                    //Student Data 
                    var UserID = int.Parse(Session["UserId"].ToString());//234530970
                    var StData = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == UserID);

                    CustomMailMessage.SendMailConfirmationCode(StData.EMAIL, StData.STUDENT_NAME, GenerateCode);
                }
            }
            catch (Exception)
            {
                return Json(notify = new notify() { Message = "خطأ اثناء ارسال كود البريد الالكتروني", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "تم ارسال كود البريد الالكتروني بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendSMSConfirmedCode()
        {
            VerificationCode VerificationCode = new VerificationCode();
            SendMessage SendMessage = new SendMessage();
            try
            {
                //Generate Code 
                string GenerateCode = VerificationCode.Generate(6);
                Session["MobileCode"] = GenerateCode.ToString();
                if (Session["UserId"] != null)
                {
                    //Student Data 
                    DashBoard_Users user = new DashBoard_Users();
                    if (Session["UserId"] != null)
                    {
                        user = Session["UserId"] as DashBoard_Users;
                    }
                    int UserID = user.ID;
                    var StData = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == UserID);

                    SendMessage._SendMessage("كود التفعيل الخاصة بالطالب : " + GenerateCode, StData.MOBILE_PHONE);
                }
            }
            catch (Exception)
            {
                return Json(notify = new notify() { Message = "خطأ اثناء ارسال كود الجوال", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "تم ارسال كود الجوال بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        //Get Student Passport Image
        public ActionResult GetStudentPassportImage()
        {
            if (Session["UserId"] != null)
            {
                //Student Data 
                DashBoard_Users user = new DashBoard_Users();
                if (Session["UserId"] != null)
                {
                    user = Session["UserId"] as DashBoard_Users;
                }
                int StID = int.Parse(user.Username);

                var ImgPath = $"../Content/UserFiles/{StID}/المستندات/";

                var StPassportImage = ImgPath + dbSC.UniversityStudents.FirstOrDefault(x => x.Student_ID == StID)?.StudentDocuments.FirstOrDefault(x => x.Document_ID == 2)?.DocumentImage;

                return Json(StPassportImage, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmCode(ConfirmtionCodeVM model)
        {
            if (Session["MailCode"] != null && Session["MobileCode"] != null)
            {
                string MailCode = Session["MailCode"].ToString();
                string MobileCode = Session["MobileCode"].ToString();

                if (MailCode != model.MailCode)
                {
                    return Json(notify = new notify() { Message = "كود تاكيد البريد الالكتروني غير صحيح", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
                else if (MobileCode != model.MobileCode)
                {
                    return Json(notify = new notify() { Message = "كود تاكيد الجوال غير صحيح", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
                else if (MailCode == model.MailCode && MobileCode == model.MobileCode)
                {
                    //Student Data 
                    DashBoard_Users user = new DashBoard_Users();
                    if (Session["UserId"] != null)
                    {
                        user = Session["UserId"] as DashBoard_Users;
                    }
                    int UserID = user.ID;
                    var CurrentStudent = dbSC.INTEGRATION_All_Students.SingleOrDefault(x => x.STUDENT_ID == UserID);
                    var StData = dbSC.Sp_GetStudentDataForTravel(CurrentStudent.NATIONAL_ID)
                        .Select(x => new
                        {
                            STUDENT_NAME = x.STUDENT_NAME,
                            EMAIL = x.EMAIL,
                            NATIONAL_ID = x.NATIONAL_ID,
                            NATIONALITY_DESC = x.NATIONALITY_DESC,
                            DocImagePath = $"../Content/UserFiles/{UserID}/المستندات/" + x.DocImagePath,
                            UserImage = x.DocImagePath
                        }).First();
                    if (StData != null)
                    {
                        return Json(StData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(notify = new notify() { Message = "حدث خطأ اثناء عرض بيانات الطالب", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(null);
                }
            }
            else
            {
                return Json(notify = new notify() { Message = "خطا اثناء التاكيد", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        public class ConfirmtionCodeVM
        {
            public string MailCode { get; set; }
            public string MobileCode { get; set; }
        }
        #endregion

        #region Send Request
        public async Task<ActionResult> GetCountries()
        {
            var db = new SchoolAccGam3aEntities();
            var user = new DashBoard_Users();

            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }

            int UserID = user.ID;

            var student = dbSC.UniversityStudents.SingleOrDefault(x => x.Student_ID == UserID);
            if (student != null)
            {
                //var Nationality = dbSC.Nationality.SingleOrDefault(x => x.ID == student.Nationality_ID);
                return Json(await Task.Run(() => db.Country.Where(x => x.ID == student.Country_ID || x.Nationality.FirstOrDefault().ID == student.Nationality_ID).Select(x => new SelectListItem { Text = x.CountryNameEn, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList()),
                JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(await Task.Run(() => db.Country.Select(x => new SelectListItem { Text = x.CountryNameEn, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList()),
                JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> GetTransportationTracking(string FlightsType)
        {
            var db = new SchoolAccGam3aEntities();
            var user = new DashBoard_Users();

            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }

            int UserID = user.ID;

            var student = dbSC.INTEGRATION_All_Students.Where(x => x.ID == UserID).FirstOrDefault();
            if (student != null)
            {
                var Query = db.TransportationTracking.Where(m => m.Nationality_ID == student.NATIONALITY_CODE && m.IsActive == true && m.FlightsType == FlightsType).Select(x => new SelectListItem { Text = x.Tracking, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList();
                return Json(await Task.Run(() => Query),
                JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(await Task.Run(() => db.TransportationTracking.Where(m => m.IsActive == true && m.FlightsType == FlightsType).Select(x => new SelectListItem { Text = x.Tracking, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList()),
              JsonRequestBehavior.AllowGet);
                //return Json(await Task.Run(() => db.Country.Select(x => new SelectListItem { Text = x.CountryNameEn, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList()),
                //JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetTransportationTrackingByStudent(string FlightsType, string StudentID)
        {
            var db = new SchoolAccGam3aEntities();
            var user = new DashBoard_Users();

            decimal stdid = decimal.Parse(StudentID);
            var student = dbSC.INTEGRATION_All_Students.Where(m => m.STUDENT_ID == stdid).FirstOrDefault();
            if (student != null)
            {
                var Query = db.TransportationTracking.Where(m => m.Nationality_ID == student.NATIONALITY_CODE && m.FlightsType == FlightsType).Select(x => new SelectListItem { Text = x.Tracking, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList();
                return Json(await Task.Run(() => Query),
                JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(await Task.Run(() => db.TransportationTracking.Where(m => m.FlightsType == FlightsType).Select(x => new SelectListItem { Text = x.Tracking, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList()),
              JsonRequestBehavior.AllowGet);
                //return Json(await Task.Run(() => db.Country.Select(x => new SelectListItem { Text = x.CountryNameEn, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList()),
                //JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetAirports(int id)
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.AirPort.Where(x => x.CountryID == id).Select(x => new SelectListItem { Text = x.AirPortNameEn, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList()),
             JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetLevels()
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.config.Where(x => x.Kay == "Level")
            .Select(x => new SelectListItem { Text = x.Value, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList()),
            JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetActiveAdvertisement()
        {
            var db = new SchoolAccGam3aEntities();
            return Json(await Task.Run(() => db.TravelAdvertisement.Where(x => x.IsActive == true && x.AdEndDate >= DateTime.Now).Select(x => new SelectListItem
            {
                Text = x.AdName,
                Value = x.ID.ToString()
            }).ToList()), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetStudentByadvertisementIdForAdmin(int advertisementId)
        {
            var advertisement = dbSC.TravelAdvertisement.Single(x => x.ID == advertisementId);

            var advertisementNationalities = advertisement.NationalityID.Split(',').Select(decimal.Parse).ToList();
            var advertisementDegrees = advertisement.DegreeID.Split(',').Select(decimal.Parse).ToList();
            var advertisementStatuses = advertisement.StStatusID.Split(',').Select(decimal.Parse).ToList();


            var registeredStudentInTheAdvertisement = dbSC.StTravelRequest
                .Where(x => x.AdvertisementID == advertisementId).Select(x => (decimal)x.StudentID).ToList();

            var activeAdvertisements = (from student in dbSC.SP_GetAllStudents_Customize(null)
                                        where advertisementNationalities.Contains(student.NATIONALITY_CODE) &&
                                              advertisementDegrees.Contains(student.DEGREE_CODE) &&
                                              advertisementStatuses.Contains(student.STATUS_CODE) &&
                                              !registeredStudentInTheAdvertisement.Contains(student.STUDENT_ID)

                                        select new SelectListItem
                                        {
                                            Value = student.STUDENT_ID.ToString(),
                                            Text = student.STUDENT_NAME
                                        }).ToList();



            return Json(activeAdvertisements, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SaveStudentDesiresByAdmin(int? StudentDesireId, int studentId, int travelClasses, string flightType,
            DateTime departureDate, DateTime? returnDate, int travelPurpose, int? advertisementId,
            bool isPassportVerified, int TransportationTrackingID)
        {
            var db = new SchoolAccGam3aEntities();

            if (StudentDesireId != null)
            {

                var model = db.StTravelRequest.FirstOrDefault(x => x.ID == StudentDesireId);
                if (model != null)
                {
                    model.StudentID = studentId;
                    model.TravelClass = travelClasses;
                    model.FlightsType = flightType;
                    model.Departing = departureDate;
                    model.Returning = returnDate;
                    model.TravelPurpose = travelPurpose;
                    model.AdvertisementID = advertisementId;
                    //model.IsPassportVerified = isPassportVerified;
                    //model.AirPortFrom = airportFrom;
                    //model.AirPortTo = airportTo;
                    model.UserId = ((DashBoard_Users)HttpContext.Session["UserId"]).ID;
                    model.InsertDate = DateTime.Now;
                    model.TransportationTrackingID = TransportationTrackingID;
                    db.SaveChanges();
                }
                else
                {
                    return Content(@"حدث خطأ");
                }
            }
            else
            {
                if (advertisementId != null)
                {

                    if (db.StTravelRequest.Any(x => x.StudentID == studentId && x.AdvertisementID == advertisementId))
                    {
                        return Content(@"عفوا الطالب مسجل على نفس الاعلان من قبل");
                    }
                }

                if (db.StTravelRequest.Any(x => x.StudentID == studentId && departureDate <= x.Departing && departureDate >= x.Returning))
                {
                    return Content(@"عفوا هذا الطالب له طلبات سفر فى نفس المدة");
                }

                if (returnDate != null)
                {
                    if (db.StTravelRequest.Any(x => x.StudentID == studentId && returnDate <= x.Departing && returnDate >= x.Returning))
                    {
                        return Content(@"عفوا هذا الطالب له طلبات سفر فى نفس المدة");
                    }
                }

                var stTravelRequest = new StTravelRequest
                {
                    StudentID = studentId,
                    TravelClass = travelClasses,
                    FlightsType = flightType,
                    Departing = departureDate,
                    Returning = returnDate,
                    TravelPurpose = travelPurpose,
                    AdvertisementID = advertisementId,
                    IsPassportVerified = isPassportVerified,
                    //AirPortFrom = airportFrom,
                    //AirPortTo = airportTo,
                    UserId = ((DashBoard_Users)HttpContext.Session["UserId"]).ID,
                    TransportationTrackingID = TransportationTrackingID,
                    InsertDate = DateTime.Now
                };

                db.StTravelRequest.Add(stTravelRequest);

            }
            db.SaveChanges();

            return Content("");
        }

        [HttpGet]
        public ActionResult GetStudentDesireDetails(int StudentDesireId)
        {
            var model = dbSC.StTravelRequest.Where(x => x.ID == StudentDesireId)
                                .Select(x => new
                                {
                                    x.ID,
                                    x.StudentID,
                                    x.TravelClass,
                                    x.FlightsType,
                                    Departing = x.Departing.Month + "/" + x.Departing.Day + "/" + x.Departing.Year,
                                    Returning = x.Returning != null ? x.Returning.Value.Month + "/" + x.Returning.Value.Day + "/" + x.Returning.Value.Year : null,
                                    x.TravelPurpose,
                                    x.AdvertisementID,
                                    x.IsPassportVerified,
                                    x.TransportationTrackingID,
                                    //CountryFromID = x.AirPort.CountryID,
                                    //x.AirPortFrom,
                                    //x.AirPortTo,
                                    //CountryToID = x.AirPort1.CountryID,
                                    x.IsActive
                                }).FirstOrDefault();
            if (model != null)
            {
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            {
                return Json(notify = new notify() { Message = "لا توجد بيانات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStudents()
        {

            var students = dbSC.INTEGRATION_All_Students.Where(x => x.JOIN_DATE > new DateTime(2011, 01, 01)).Select(x => new
            {
                Text = x.STUDENT_NAME,
                Value = x.STUDENT_ID.ToString(),
                NationalID = x.NATIONAL_ID
            }).ToList();

            return this.JsonMaxLength(students);
        }


        [HttpDelete]
        public ActionResult DeleteStudentDesire(int StudentDesireId)
        {
            var db = new SchoolAccGam3aEntities();

            var model = db.StTravelRequest.ToList().Where(x => x.ID == StudentDesireId).FirstOrDefault();
            if (model == null)
            {
                return Json("لايمكن إتمام عملية الحذف", JsonRequestBehavior.AllowGet);
            }

            db.StTravelRequest.Remove(model);
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);

        }
        public ActionResult SaveRequest(/*StTravelRequest model*/ int ID, int TransportationTrackingID, string Departing, string Returning, int? AdvertisementID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ID == null || ID == 0)
                    {

                        var user = Session["UserId"] as DashBoard_Users;
                        StTravelRequest model = new StTravelRequest();
                        model.StudentID = int.Parse(user.Username);
                        var advertisement = dbSC.TravelAdvertisement.Find(AdvertisementID);
                        if (advertisement != null)
                        {
                            model.TravelClass = advertisement.TravelClass;
                            model.TravelPurpose = advertisement.PurposeID;
                            model.FlightsType = advertisement.flightsType;
                        }
                        model.InsertDate = DateTime.Now;
                        model.IsActive = true;
                        model.AdvertisementID = AdvertisementID;
                        model.Departing = DateTime.ParseExact(Departing, "MM/dd/yyyy", null);
                        model.TransportationTrackingID = TransportationTrackingID;
                        if(Returning!=null)
                        model.Returning = DateTime.ParseExact(Returning, "MM/dd/yyyy", null);
                        dbSC.Entry(model).State = EntityState.Added;
                        int x = dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var request = dbSC.StTravelRequest.Find(ID);
                        request.Departing = DateTime.ParseExact(Departing, "MM/dd/yyyy", null);
                        if (request.FlightsType == "R")
                        {
                            request.Returning = DateTime.ParseExact(Returning, "MM/dd/yyyy", null);
                        }
                        request.AdvertisementID = AdvertisementID;
                        request.TransportationTrackingID = TransportationTrackingID;
                        dbSC.Entry(request).State = EntityState.Modified;
                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم التعديل بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    //if (model.ID == 0 || model.ID == null)
                    //{
                    //    var user = Session["UserId"] as DashBoard_Users;


                    //    model.StudentID = int.Parse(user.Username);
                    //    var advertisement = dbSC.TravelAdvertisement.Find(model.AdvertisementID);
                    //    if (advertisement != null)
                    //    {
                    //        model.TravelClass = advertisement.TravelClass;
                    //        model.TravelPurpose = advertisement.PurposeID;
                    //        model.FlightsType = advertisement.flightsType;
                    //    }
                    //    model.InsertDate = DateTime.Now;
                    //    model.IsActive = true;
                    //    dbSC.StTravelRequest.Add(model);
                    //    dbSC.SaveChanges();
                    //    return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    var request = dbSC.StTravelRequest.Find(model.ID);
                    //    request.Departing = model.Departing;
                    //    if(request.FlightsType == "R")
                    //    {
                    //        request.Returning = model.Returning;
                    //    }
                    //    request.TransportationTrackingID = model.TransportationTrackingID;
                    //    dbSC.Entry(request).State = EntityState.Modified;
                    //    dbSC.SaveChanges();
                    //    return Json(notify = new notify() { Message = "تم التعديل بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    //}

                }
                catch (Exception ex)
                {

                    return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region StudentAdvertisement
        /********* StudentAdvertisement **********/
        public ActionResult StudentAdvertisement()
        {
            DashBoard_Users user = new DashBoard_Users();
            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }
            if (user.IsStudent)
            {
                ViewBag.View = true;
                ViewBag.Read = true;
                ViewBag.Delete = true;
                ViewBag.Update = true;
                ViewBag.Create = true;
            }
            else
            {
                var json = new JavaScriptSerializer().Serialize(GetAdStudentPermissions(43).Data);
                var permissions = JsonConvert.DeserializeObject<AdStudentPermissions>(json);
                ViewBag.View = permissions.View;
                ViewBag.Read = permissions.Read;
                ViewBag.Delete = permissions.Delete;
                ViewBag.Update = permissions.Update;
                ViewBag.Create = permissions.Create;
            }
            //if (permissions.View)
            //{
            return View();
            //}
            // return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult GetStudentAdvertisement()
        {
            //Student Data 
            DashBoard_Users user = new DashBoard_Users();
            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }
            int studentid = Convert.ToInt32(user.Username);
            var StData = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentid);
            if (StData != null)
            {
                string NationalityID = StData.NATIONALITY_CODE.ToString();
                string DegreeID = StData.DEGREE_CODE.ToString();
                string StStatusID = StData.STATUS_CODE.ToString();
                string userid = Session["UserId"].ToString();
                List<int> CustomFieldIdList = dbSC.StudentsCustomFields.Where(x => x.StudentId == user.Username).Select(x=>x.CustomFieldId).ToList();
              
                if (CustomFieldIdList != null)
                {
                    List<string> fields = CustomFieldIdList.ConvertAll<string>(x => x.ToString());
                    var data = dbSC.TravelAdvertisement
                   .Select(x => new TravelAdvertisementVM
                   {
                       ID = x.ID,
                       StudentId = studentid,
                       AdName = x.AdName,
                       AdDate = "(" + x.AdStartDate + ")" + "-" + "(" + x.AdEndDate + ")",
                       DepartingDate = "(" + x.DepartingStart + ")" + "-" + "(" + x.DepartingEnd + ")",
                       ReturningDate = "(" + x.ReturningStart + ")" + "-" + "(" + x.ReturningEnd + ")",
                       IsActive = x.IsActive,
                       TicketNumber = x.StTravelRequest.FirstOrDefault(o => o.StudentID == studentid).StudentsTravelOrder.FirstOrDefault().TicketNumber,
                       NationalityID = x.NationalityID,
                       DegreeID = x.DegreeID,
                       StStatusID = x.StStatusID,
                       CustomFieldId = x.CustomFieldId,
                       AdEndDate = x.AdEndDate.Day + "/" + x.AdEndDate.Month + "/" + x.AdEndDate.Year,
                       TravelOrder = x.StTravelRequest.FirstOrDefault(o => o.StudentID == studentid).StudentsTravelOrder.Count(),
                       TravelRequestId = x.StTravelRequest.FirstOrDefault(c => c.StudentID == studentid && c.AdvertisementID == x.ID) != null ? x.StTravelRequest.FirstOrDefault(c => c.StudentID == studentid && c.AdvertisementID == x.ID).ID : 0,
                       ShowDetalies = x.StTravelRequest.FirstOrDefault(c => c.StudentID == studentid && c.AdvertisementID == x.ID) != null ? true : false,
                   }).ToList()
                      .Where(m => m.NationalityID.Split(',').Any(p => p == NationalityID) && m.DegreeID.Split(',').Any(p => p == DegreeID)
                       && m.StStatusID.Split(',').Any(p => p == StStatusID));
                    // && (m.CustomFieldId == null || m.CustomFieldId.Split(',').Any(p => p == fields.First(z => z == p))));
                    List<TravelAdvertisementVM> lst = new List<TravelAdvertisementVM>();
                    foreach(var item in data)
                    {
                        if(item.CustomFieldId == null)
                        {
                            lst.Add(item);
                        }
                        if (fields.Contains(item.CustomFieldId))
                        {
                            lst.Add(item);
                        }
                    }
                    if (data.Count() > 0)
                        return this.JsonMaxLength(lst, JsonRequestBehavior.AllowGet);

                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = dbSC.TravelAdvertisement
                                    .Select(x => new
                                    {
                                        ID = x.ID,
                                        StudentId = studentid,
                                        AdName = x.AdName,
                                        AdDate = "(" + x.AdStartDate + ")" + "-" + "(" + x.AdEndDate + ")",
                                        DepartingDate = "(" + x.DepartingStart + ")" + "-" + "(" + x.DepartingEnd + ")",
                                        ReturningDate = "(" + x.ReturningStart + ")" + "-" + "(" + x.ReturningEnd + ")",
                                        IsActive = x.IsActive,
                                        TicketNumber = x.StTravelRequest.FirstOrDefault(o => o.StudentID == studentid).StudentsTravelOrder.FirstOrDefault().TicketNumber,
                                        NationalityID = x.NationalityID,
                                        DegreeID = x.DegreeID,
                                        StStatusID = x.StStatusID,
                                        CustomFieldId = x.CustomFieldId,
                                        AdEndDate = x.AdEndDate.Day + "/" + x.AdEndDate.Month + "/" + x.AdEndDate.Year,
                                        TravelOrder = x.StTravelRequest.FirstOrDefault(o => o.StudentID == studentid).StudentsTravelOrder.Count(),
                                        TravelRequestId = x.StTravelRequest.FirstOrDefault(c => c.StudentID == studentid && c.AdvertisementID == x.ID) != null ? x.StTravelRequest.FirstOrDefault(c => c.StudentID == studentid && c.AdvertisementID == x.ID).ID : 0,
                                        ShowDetalies = x.StTravelRequest.FirstOrDefault(c => c.StudentID == studentid && c.AdvertisementID == x.ID) != null ? true : false,
                                    }).ToList().Where(m => m.NationalityID.Split(',').Any(p => p == NationalityID) && m.DegreeID.Split(',').Any(p => p == DegreeID)
                                        && m.StStatusID.Split(',').Any(p => p == StStatusID)
                                         
                                        );

                    if (data.Count() > 0)
                        return this.JsonMaxLength(data, JsonRequestBehavior.AllowGet);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }



        public ActionResult IsValidStudentData()
        {
            var Expired = false;
            //Student Data 
            DashBoard_Users user = new DashBoard_Users();
            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }

            int studentid = Convert.ToInt32(user.Username);
            var StData = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentid);
            if (StData != null)
            {
                string NationalityID = StData.NATIONALITY_CODE.ToString();
                string DegreeID = StData.DEGREE_CODE.ToString();
                string StStatusID = StData.STATUS_CODE.ToString();
                string userid = Session["UserId"].ToString();
                var CurrentDate = DateTime.Now;
                var data = dbSC.TravelAdvertisement.ToList().Where(m => m.NationalityID.Split(',').Any(p => p == NationalityID) && m.DegreeID.Split(',').Any(p => p == DegreeID)
                        && m.StStatusID.Split(',').Any(p => p == StStatusID) && CurrentDate < m.AdEndDate).Count();
                if (data == 0)
                {
                    return Json(notify = new notify() { Message = "لا يوجد إعلانات الأن وتم إغلاق الرغبات", Type = "error", status = 100 }, JsonRequestBehavior.AllowGet);

                }

            }
                var UniversityStudent = dbSC.UniversityStudents.Where(m => m.Student_ID == studentid).FirstOrDefault();
            
            if (UniversityStudent != null)
            {

                int configval = int.Parse(dbSC.config.Where(m => m.Kay == "PassportExpirePeriod").Select(p => p.Value).FirstOrDefault());
                var row = dbSC.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent.ID && x.Document_ID == 2 && x.IsExpired != true).ToList();
                if (row.Count != 0)
                {
                    if ((Math.Abs((DateTime.Now - row.LastOrDefault().ExpiryDate).Value.Days) <= configval))
                    {
                        Expired = true;
                    }
                }
                if (UniversityStudent != null)
                {
                    if (dbSC.StudentDocuments.Where(m => m.UniversityStudent_ID == UniversityStudent.ID).Count() == 0)
                    {

                        return Json(notify = new notify() { Message = "يرجي تحديث البيانات والوثائق أولا " + "\n" + "  ولذلك لن يظهر لك اى اعلان للسفر إلا بعد تحديث البيانات بالكامل", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                    }
                    else if (dbSC.StudentDocuments.Where(m => m.UniversityStudent_ID == UniversityStudent.ID && (m.IsActive == false || m.IsNew == true)).Count() == 0)
                    {
                        return Json(notify = new notify() { Message = "تم اعتماد البيانات من قبل ادارة السفر يمكنك إدخال الرغبات", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);

                    }
                    else if (dbSC.StudentDocuments.Any(m => m.UniversityStudent_ID == UniversityStudent.ID && (m.IsActive == false || m.IsNew == true)))
                    {
                        return Json(notify = new notify() { Message = "يرجي مراجعة شاشة المستندات الخاصه بك والتأكد من جميع مستنداتك وعدم وجود تكرار في المستندات علي أن يكون مستند للهوية( واحد فقط )واحد فقط ومستند للجواز (واحد فقط ) ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                    }
                    else if (Expired == true)
                        return Json(notify = new notify() { Message = "  برجاء ادخال صورة جواز سفر صالح من صفحة البيانات ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                   }
                return Json(notify = new notify() { Message = "يرجي تحديث البيانات والوثائق أولا ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(notify = new notify() { Message = "ولذلك لن يظهر لك اى اعلان للسفر إلا بعد تحديث البيانات بالكامل يرجي تحديث البيانات والوثائق أولا", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

            }


        }

        public ActionResult GetAdvertisementById(int ID)
        {
            if (ID > 0)
            {
                var User = (DashBoard_Users)Session["UserID"];
                var stdid = dbSC.INTEGRATION_All_Students.Where(m => m.ID == User.ID).Select(p => p.STUDENT_ID).FirstOrDefault();
                var singleRecord = dbSC.StTravelRequest.Where(m => m.TravelAdvertisement.ID == ID && m.StudentID == stdid).FirstOrDefault();
                int? TransportationTrackingID = null;
                if (singleRecord != null)
                {
                    TransportationTrackingID = singleRecord.TransportationTrackingID;
                }
                var Advertisement = dbSC.TravelAdvertisement.ToList().Where(x => x.ID == ID).Select(x => new
                {
                    DepartingStart = x.DepartingStart.ToString("MM/dd/yyyy"),
                    DepartingEnd = x.DepartingEnd.ToString("MM/dd/yyyy"),
                    ReturningStart = x.ReturningStart == null ? null : x.ReturningStart.Value.ToString("MM/dd/yyyy"),
                    ReturningEnd = x.ReturningEnd == null ? null : x.ReturningEnd.Value.ToString("MM/dd/yyyy"),
                    AdName = x.AdName,
                    TravelClass = x.TravelClass,
                    FlightsType = x.flightsType,
                    Purpose = x.PurposeID,
                    TransportationTrackingID = TransportationTrackingID

                }).FirstOrDefault();

                return Json(Advertisement, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }



        public ActionResult CheckAllowStudent()
        {
            //Student Data 
            DashBoard_Users user = new DashBoard_Users();
            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }
            int studentid = Convert.ToInt32(user.Username);
            var StData = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentid);
            var UnStData = dbSC.UniversityStudents.FirstOrDefault(x => x.Student_ID == studentid);
            if (UnStData != null)
            {
                var ResidentStData = dbSC.AllResidents.FirstOrDefault(x => x.IDNumber == UnStData.National_ID && x.Status == "إقامة صالحة");
                if (ResidentStData != null)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(notify = new notify() { Message = "الطالب غير موجود", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckStudentData(int id /*==>advertisementID*/)
        {
            try
            {
                var advDepartingEnd = dbSC.TravelAdvertisement.Find(id).DepartingEnd;
                var TravelAdvertisement = dbSC.TravelAdvertisement.Find(id);
                //Student Data
                DashBoard_Users user = new DashBoard_Users();
                if (Session["UserId"] != null)
                {
                    user = Session["UserId"] as DashBoard_Users;
                }
                int studentID = int.Parse(user.Username);

                //

                var UnStData = dbSC.UniversityStudents.FirstOrDefault(x => x.Student_ID == studentID);
                if (UnStData != null)
                {
                    //Check Verify Code
                    if (UnStData.LastEmailVerificationDate == null && UnStData.LastPhoneVerificationDate == null)
                    {
                        return Json(notify = new notify() { Message = "برجاء تاكيد البريد الالكتروني ورقم الهاتف من صفحة البيانات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                    }

                    //Check Passport Expire Period
                    int PassportExpireDays = int.Parse(dbSC.config.FirstOrDefault(m => m.Kay == "PassportExpirePeriod").Value);
                    DateTime allowExpireDate = DateTime.Now.AddDays(PassportExpireDays);
                    var CheckPassportData = dbSC.StudentDocuments.FirstOrDefault(x => x.UniversityStudent_ID == UnStData.ID && x.Document_ID == 2 && (x.ExpiryDate > allowExpireDate));

                    if (CheckPassportData == null)
                        return Json(notify = new notify() { Message = "برجاء ادخال صورة جواز سفر صالح من صفحة البيانات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                    var CheckPassport = dbSC.StudentDocuments.FirstOrDefault(x => x.UniversityStudent_ID == UnStData.ID && x.Document_ID == 2 /*&& x.IsActive == true &&x.IsNew == false*/ && x.ExpiryDate > advDepartingEnd /*&& x.ExpiryDate > ExpDate*/);
                    if (CheckPassport == null)
                        return Json(notify = new notify() { Message = "برجاء ادخال صورة جواز سفر صالح من صفحة البيانات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                    var CheckPassportNew = dbSC.StudentDocuments.FirstOrDefault(x => x.UniversityStudent_ID == UnStData.ID && x.Document_ID == 2 && x.IsActive == false && x.IsNew == true && x.ExpiryDate > advDepartingEnd /*&& x.ExpiryDate > ExpDate*/);
                    if (CheckPassportNew != null)
                        return Json(notify = new notify() { Message = "برجاء مراجعة الادارة لمراجعة صورة جواز السفر", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);


                    var CheckIdentity = dbSC.StudentDocuments.FirstOrDefault(x => x.UniversityStudent_ID == UnStData.ID && x.Document_ID == 1 /*&& x.IsActive == true && x.IsNew == false*/ && x.ExpiryDate >= DateTime.Now && x.IsExpired != true);
                    if (CheckIdentity == null)
                        return Json(notify = new notify() { Message = "برجاء ادخال صورة اقامة صالحة من صفحة البيانات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                    var CheckIdentityNew = dbSC.StudentDocuments.FirstOrDefault(x => x.UniversityStudent_ID == UnStData.ID && x.Document_ID == 1 && x.IsActive == false && x.IsNew == true && x.ExpiryDate > advDepartingEnd);
                    if (CheckIdentityNew != null)
                        return Json(notify = new notify() { Message = "برجاء مراجعة الادارة لمراجعة صورة الاقامة", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);


                    //var CheckVisa = dbSC.StudentDocuments.FirstOrDefault(x => x.UniversityStudent_ID == UnStData.ID && x.Document_ID == 3 /*&& x.IsActive == true && x.IsNew == false*/ && x.ExpiryDate > advDepartingEnd);
                    //if (CheckVisa == null)
                    //    return Json(notify = new notify() { Message = "برجاء ادخال صورة تاشيرة صالحة من صفحة البيانات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                    var CheckVisaNew = dbSC.StudentDocuments.FirstOrDefault(x => x.UniversityStudent_ID == UnStData.ID && x.Document_ID == 3 && x.IsActive == false && x.IsNew == true && x.ExpiryDate > advDepartingEnd);
                    if (CheckVisaNew != null)
                        return Json(notify = new notify() { Message = "برجاء مراجعة الادارة لمراجعة صورة التاشيرة", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                    if (dbSC.TravelAdvertisement.Find(id).IsAppearToExpectedGradutedPeople != true)
                    {
                        ObjectResult<Nullable<int>> result = dbSC.Usp_IsStudentGradted(UnStData.Student_ID.ToString());
                        foreach (var data in result.ToList())
                        {
                            if (data > 0 && TravelAdvertisement.flightsType == "R")
                                return Json(notify = new notify() { Message = "عفوا لا يمكن تقديم الرغبه لهذا الإعلان نظرا لانك من المتوقع تخرجهم", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                        }
                    }
                    var CustomFieldID = dbSC.TravelAdvertisement.Find(id).CustomFieldId;
                    if (CustomFieldID != null)
                    {
                        var CustomFieldIDList = dbSC.TravelAdvertisement.Find(id).CustomFieldId.Split(',').ToList();
                        string StdID = studentID.ToString();
                        var StudentCustomFieldID = dbSC.StudentsCustomFields.Where(m => m.StudentId == StdID).FirstOrDefault()?.CustomFieldId;
                        if (StudentCustomFieldID == null)
                        {
                            return Json(notify = new notify() { Message = "لايمكنك التقديم حيث ان شروط الاعلان لاتنطبق عليك", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                        }else if (StudentCustomFieldID!=null && !CustomFieldIDList.Contains(CustomFieldID))
                        {
                            return Json(notify = new notify() { Message = "لايمكنك التقديم حيث ان شروط الاعلان لاتنطبق عليك", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                        }

                    }
                    if(TravelAdvertisement.flightsType == "O")
                    {

                        var studentAdvancePaymentDetails = dbSC.AdvancePaymentMaster.Where(x => x.Student_Id == studentID).
                                                      SelectMany(x => x.AdvancePaymentDetails).ToList();

                        if (studentAdvancePaymentDetails != null)
                        {
                            // مجموع  قيمةالسلف المصروفة للطالب
                            var TotalAdvancePayment = studentAdvancePaymentDetails.Where(x => x.AdvanceRequests.AdvanceSettings.AdvanceType == "A").Sum(p => p.NetValue);

                            // مجموع  قيمةالسلف التي سددها الطالب للطالب
                            var TotalAdvanceReceive = dbSC.AdvanceReceiveDetails.AsEnumerable().Where(x => studentAdvancePaymentDetails.Any(p => p.ID == x.AdvancePaymentDetails_Id)).Sum(x => x.NetValue);

                            if ((TotalAdvancePayment - TotalAdvanceReceive) != 0)
                            {
                                return Json(notify = new notify() { Message = "لا يمكن إتمام عملية الحفظ حتي يسدد الطالب جميع السلف السابقة", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                 
                }
                else
                    return Json(notify = new notify() { Message = "برجاء تحديث البيانات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //التحقق من ان الطالب غير مسجل في اعلان في فترة اعلان اخر
        public ActionResult CheckStudentAllowAdvertisement(int id)
        {
            DashBoard_Users user = new DashBoard_Users();
            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }
            int UserID = int.Parse(user.Username);

            var Advertisement = dbSC.TravelAdvertisement.Find(id);
            var checkAdv = dbSC.StTravelRequest.Any(x =>
            (x.AdvertisementID != Advertisement.ID && x.StudentID == UserID && Advertisement.AdStartDate >= x.TravelAdvertisement.AdStartDate && Advertisement.AdEndDate
            <= x.TravelAdvertisement.AdEndDate) ||
            (x.AdvertisementID != Advertisement.ID && x.StudentID == UserID && Advertisement.DepartingStart >= x.TravelAdvertisement.DepartingStart && Advertisement.DepartingEnd <= x.TravelAdvertisement.DepartingEnd)
            && x.IsActive == true);

            if (checkAdv == false)
            {
                return Json(notify = new notify() { Message = "", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(notify = new notify() { Message = "", Type = "success", status = 400 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckStudentTicketAdvertisement(int id)
        {
            DashBoard_Users user = new DashBoard_Users();
            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }
            bool isExpiredAd = false;
            int count = 0;
            int UserID = int.Parse(user.Username);
            var CurrentDate = DateTime.Now;
            var Ad = dbSC.TravelAdvertisement.Find(id);
            var canEdit = dbSC.StudentsTravelOrder.Select(m => m.StTravelRequest) != null ? dbSC.StudentsTravelOrder.Where(m => m.StTravelRequest.AdvertisementID == id && m.StudentID == UserID  ).Count() > 0 ? false : true : true;
            var Check = dbSC.StTravelRequest.FirstOrDefault(x => x.StudentID == UserID && x.AdvertisementID == id );
            if(Ad.AdEndDate < CurrentDate)
            {
                isExpiredAd = true;
            }
            var AdStartDate = Ad.AdStartDate;
            var AdEndDate = Ad.AdEndDate;
            var StData = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == UserID);
            if (StData != null)
            {
                string NationalityID = StData.NATIONALITY_CODE.ToString();
                string DegreeID = StData.DEGREE_CODE.ToString();
                string StStatusID = StData.STATUS_CODE.ToString();
                string userid = Session["UserId"].ToString();
                
          

                count = (from t in dbSC.TravelAdvertisement.ToList().Where(m => m.NationalityID.Split(',').Any(p => p == NationalityID) && m.DegreeID.Split(',').Any(p => p == DegreeID)
                          && m.StStatusID.Split(',').Any(p => p == StStatusID) && (AdStartDate >= m.AdStartDate && AdStartDate <= m.AdEndDate) ||
                                                                 (AdEndDate >= m.AdStartDate && AdEndDate <= m.AdEndDate) ||
                                                                 (m.AdStartDate >= AdStartDate && m.AdStartDate <= AdEndDate))
                           join st in dbSC.StTravelRequest on t.ID equals st.AdvertisementID
                           where st.StudentID == UserID
                           select t.ID
                          ).Count();
           
            }


            if (Check != null)
            {
                return Json(new { Message = "تم التقديم علي هذا الاعلان مسبقا يمكنك تعديل الرغبات", Type = "error", status = 500, canEdit = canEdit,isActive= Ad.IsActive, isExpiredAd= isExpiredAd }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new { Message = "", Type = "success", status = 200, canEdit = canEdit , count = count, isActive = Ad.IsActive, isExpiredAd= isExpiredAd }, JsonRequestBehavior.AllowGet);
            }
        }




        public ActionResult GetStudentTravelRequestData(int ID)
        {
            DashBoard_Users user = new DashBoard_Users();
            if (Session["UserId"] != null)
                user = Session["UserId"] as DashBoard_Users;

            int StudentId = int.Parse(user.Username);

            var data = dbSC.Usp_GetStudentTravelOrder(ID, StudentId).ToList()
                     .Select(x => new
                     {
                         TravelRequestId= x.TravelRequestId,
                         Departing = x.Departing != null ? x.Departing.ToString("MM/dd/yyyy") : "",
                         Returning = x.Returning != null ? x.Returning.Value.ToString("MM/dd/yyyy") : "",
                         InsertDate = x.InsertDate != null ? x.InsertDate.ToString("MM/dd/yyyy") : "",
                         TravelOrder = x.TravelOrder != null ? "تم" : "جاري المراجعه",
                         GivenAmount = x.GivenAmount != null ? "تم" : "",
                         AgentRefNumber = x.AgentRefNumber != null ? "تم" : "",
                         IsTicketNumber = x.IsApproved == true ? "تم" : "",
                         TicketNumber = x.TicketNumber != "[]" ? x.TicketNumber : "",
                         Tracking = x.Tracking
                     }).ToList();

            return this.JsonMaxLength(data);

        }
        [HttpPost]
        public ActionResult DeleteStudentTravelRequestData(int travelRequestId)
        {
            var travelRequest = dbSC.StTravelRequest.Find(travelRequestId);
            if (travelRequest != null)
            {
                if (travelRequest.IsActive == true && travelRequest.StudentsTravelOrder.Count() > 0)
                {
                    return Json(notify = new notify() { Message = "لا يمكن حذف الرغبة لارتباطها بأمر إركاب", Type = "info", status = 500 }, JsonRequestBehavior.AllowGet);
                }
                dbSC.StTravelRequest.Remove(travelRequest);
                try
                {
                    dbSC.SaveChanges();
                    return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 500 }, JsonRequestBehavior.AllowGet);
                }

                catch (Exception)
                {

                    return Json(notify = new notify() { Message = "حدث خطأ أثناء الحذف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(notify = new notify() { Message = "لا توجد بيانات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudentTicketAdvertisementForEdit(int id)
        {
            DashBoard_Users user = new DashBoard_Users();
            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }
            int UserID = int.Parse(user.Username);
            var data = dbSC.StTravelRequest.ToList().Where(x => x.StudentID == UserID && x.AdvertisementID == id).Select(x => new
            {
                x.ID,
                //Country = dbSC.AirPort.FirstOrDefault(c => c.ID == x.AirPortTo).CountryID,
                //x.AirPortFrom,
                //x.AirPortTo,
                x.IsActive,
                Departing = x.Departing.ToString("MM/dd/yyyy"),
                Returning = x.Returning == null ? null : x.Returning.Value.ToString("MM/dd/yyyy"),
                TransportationTrackingID = x.TransportationTrackingID
                //  Nationality_ID = dbSC.INTEGRATION_All_Students.Where(m => m.ID == UserID).Select(o => o.NATIONAL_ID).FirstOrDefault()
            }).FirstOrDefault();
            if (data != null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(notify = new notify() { Message = "لا توجد بيانات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        //GetStudentAllowTicketByAdvertisementId
        public ActionResult GetStudentAllowTicketByAdvertisementId(int id)
        {
            var Advertisement = dbSC.TravelAdvertisement.Find(id);
            var NationalityID = Advertisement.NationalityID.Split(',').Select(decimal.Parse).ToList();
            var DegreeID = Advertisement.DegreeID.Split(',').Select(decimal.Parse).ToList();
            var StStatusID = Advertisement.StStatusID.Split(',').Select(decimal.Parse).ToList();

            var students = dbSC.INTEGRATION_All_Students.Where(x => x.JOIN_DATE > new DateTime(2011, 01, 01) &&
            x.STATUS_CODE == 1 &&

            NationalityID.Any(p => p == x.NATIONALITY_CODE) &&
           DegreeID.Any(p => p == x.DEGREE_CODE) &&
           StStatusID.Any(p => p == x.STATUS_CODE)).Select(x => new
           {
               Text = x.STUDENT_NAME,
               Value = x.STUDENT_ID.ToString(),
               NationalID = x.NATIONAL_ID
           }).ToList();


            return this.JsonMaxLength(students);
        }

        public ActionResult CheckStudentPayAdvanceAmount(int id)
        {
            //==============================================
            //التاكد  من ان الطالب ليس علية سلف غير مسددة
            //==============================================
            //تفاصيل طلب السلفة            
            var studentAdvancePaymentDetails = dbSC.AdvancePaymentMaster.Where(x => x.Student_Id == id).SelectMany(x => x.AdvancePaymentDetails).ToList();

            // مجموع  قيمةالسلف المصروفة للطالب
            var TotalAdvancePayment = studentAdvancePaymentDetails.Sum(p => p.NetValue);

            // مجموع  قيمةالسلف التي سددها الطالب للطالب
            var TotalAdvanceReceive = dbSC.AdvanceReceiveDetails.AsEnumerable().Where(x => studentAdvancePaymentDetails.Any(p => p.ID == x.AdvancePaymentDetails_Id)).Sum(x => x.NetValue);

            if ((TotalAdvancePayment - TotalAdvanceReceive) != 0)
            {
                return Json(notify = new notify() { Message = "عفوا لايمكن اتمام العملية حتي يتم الطالب سدادما علية من سلف ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);

        }

        #endregion



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

            }

            return permissions;
        }
        #region TravelRequestReview
        public ActionResult TravelRequestReview()
        {
            var permissions = GetPermissionsFn(122);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.View = permissions.View;
            if (permissions.View || permissions.Read)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }

        [HttpGet]
        public ActionResult GetTravelAdvertisementNames()
        {
            var data = dbSC.TravelAdvertisement.Where(x => x.AdStartDate <= DateTime.Now && x.DepartingStart > DateTime.Now && x.StTravelRequest.Count() > 0)
                .Select(x => new SelectListItem { Text = x.AdName, Value = x.ID.ToString() }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetTravelAdvertisement()
        {
            var data = dbSC.TravelAdvertisement.Where(x => /*x.AdStartDate <= DateTime.Now  &&*/ x.StTravelRequest.Count() > 0)
                .Select(x => new SelectListItem { Text = x.AdName, Value = x.ID.ToString() }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetTravelRequests(string TravelAdvertisementIds)
        {
            var travelAdvertisementIds = string.IsNullOrEmpty(TravelAdvertisementIds) ? new string[0] : TravelAdvertisementIds.Split(',');

            var data = dbSC.StTravelRequest.Where(x => travelAdvertisementIds.Any(p => p == x.AdvertisementID.ToString()) && x.StudentsTravelOrder.Count() == 0)
            .Select(x => new
            {
                x.ID,
                x.AdvertisementID,
                x.TravelAdvertisement.AdName,
                DepartingDate = x.Departing.Month + "/" + x.Departing.Day + "/" + x.Departing.Year,
                ReturningDate = x.Returning != null ? x.Returning.Value.Month + "/" + x.Returning.Value.Day + "/" + x.Returning.Value.Year : null,
                Student_ID = x.StudentID,
                Student_Name = dbSC.UniversityStudents.FirstOrDefault(c => c.Student_ID == x.StudentID).NamePerPassport_Ar,
                Faculty_Name = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).FACULTY_NAME,
                Level_Desc = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).LEVEL_DESC,
                NATIONALITY_DESC = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).NATIONALITY_DESC,
                National_ID = dbSC.UniversityStudents.FirstOrDefault(c => c.Student_ID == x.StudentID).National_ID,
                InsertDate = x.InsertDate != null ? x.InsertDate.Month + "/" + x.InsertDate.Day + "/" + x.InsertDate.Year : null,
                Tracking = x.TransportationTracking.Tracking,
                STATUS_DESC = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).STATUS_DESC,
                REMAININGCREDITHOURSCOUNT = dbSC.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).REMAININGCREDITHOURSCOUNT,


            }).ToList();

            return this.JsonMaxLength(data);
        }
        #endregion


        #region Permissions

        public JsonResult GetPermissions(int screenId)
        {
            DashBoard_Users user = new DashBoard_Users();
            if (Session["UserId"] != null)
            {
                user = Session["UserId"] as DashBoard_Users;
            }
            int UserID = user.ID;

            var perm = CheckPermissions.IsAuthorized(UserID, screenId);

            var permissions = new Permissions();
            foreach (var permission in perm)
            {
                if (permission == "ارسال الكود")
                {
                    permissions.SendCode = true;
                }
                else if (permission == "تاكيد الكود")
                {
                    permissions.ConfirmCode = true;
                }
                else if (permission == "رفع الصورة")
                {
                    permissions.UploadeImage = true;
                }
                else if (permission == "اضافة الرغبات")
                {
                    permissions.AddRequest = true;
                }
                else if (permission == "عرض")
                {
                    permissions.View = true;
                }
                else if (permission == "قراءة")
                {
                    permissions.Read = true;
                }
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        public class Permissions
        {
            public bool SendCode { get; set; }
            public bool ConfirmCode { get; set; }
            public bool UploadeImage { get; set; }
            public bool Read { get; set; }
            public bool View { get; set; }
            public bool AddRequest { get; set; }
            public bool Create { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }
        }

        public JsonResult GetAdStudentPermissions(int screenId)
        {
            var permissions = new AdStudentPermissions();
            if (Session["UserId"] != null)
            {


                var user = Session["UserId"] as DashBoard_Users;

                var perm = CheckPermissions.IsAuthorized(user.ID, screenId);


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
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        private class AdStudentPermissions
        {
            public bool Create { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }
            public bool Read { get; set; }
            public bool View { get; set; }
        }
        #endregion

        public ActionResult StudentDesiresByAdmin()
        {
            var permissions = CheckPermissionsfn(116);
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

        [HttpGet]
        public ActionResult EditStudentDesireByAdmin(int StudentDesireId)
        {
            ViewBag.StudentDesireId = StudentDesireId;
            return View();
        }



        #region UploadImagePassportByAdmin
        public ActionResult UploadImagePassportByAdmin(int id)
        {
            try
            {
                if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{id}")))
                    Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{id}"));

                if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{id}/المستندات")))
                    Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{id}/المستندات"));

                HttpPostedFileBase file = Request.Files[0];
                file.SaveAs(Server.MapPath($"~/Content/UserFiles/{id}/المستندات/Update[{id}]Passport.jpg"));

                if (!Directory.Exists(Server.MapPath($"~/Content/tempfiles")))
                    Directory.CreateDirectory(Server.MapPath($"~/Content/tempfiles"));

                HttpPostedFileBase fileTemp = Request.Files[0];

                Session["PassportFile"] = fileTemp;

                return Json(notify = new notify() { Message = "تم رفع الصورة", Type = "seccess", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(notify = new notify() { Message = "خطأ اثناءرفع الصورة", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        //Save Update Passport
        public ActionResult SaveUpdatePassport(StudentDocumentsVM model)
        {

            if (Session["UserId"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (model != null)
                        {
                            HttpPostedFileBase PassportFile = null;
                            if (Session["PassportFile"] != null)
                            {
                                PassportFile = (HttpPostedFileBase)Session["PassportFile"];
                                PassportFile.SaveAs(Server.MapPath($"~/Content/UserFiles/{model.Student_ID}/المستندات/Update[{model.Student_ID}]Passport.jpg"));

                                model.DocumentImage = $"Update[{model.Student_ID}]Passport.jpg";
                            }

                            //int stID = user.ID;
                            int UniversityStudent_ID = 0;
                            var UniversityStudent = dbSC.UniversityStudents.FirstOrDefault(x => x.Student_ID == model.Student_ID);
                            if (UniversityStudent == null)
                                return Json(notify = new notify() { Message = "عفوا الطالب لم يقم بتحديث بياناته يرجى تحديث بياناته اولا", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                            else
                                UniversityStudent_ID = UniversityStudent.ID;

                            dbSC.StudentDocuments.Add(new StudentDocuments
                            {
                                Document_ID = 2,
                                UniversityStudent_ID = UniversityStudent_ID,
                                IdentityNumber = model.IdentityNumber,
                                ExpiryDate = model.ExpiryDate,
                                IssueDate = DateTime.Now,
                                IsActive = false,
                                IsNew = true,
                                DocumentImage = model.DocumentImage
                            });
                            dbSC.SaveChanges();
                        }

                        return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {

                        return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        //private string HigriSemesterDate(int semester)
        //{
        //    string yearSuffix = semester.ToString().Substring(0, 1);
        //    string semesterPrefix = semester.ToString().Substring(2, 2);
        //    string hijriYear = $"14{semesterPrefix }"
        //}
    }

    public partial class StudentDocumentsVM
    {
        public int StudentDocumentID { get; set; }
        public int Document_ID { get; set; }
        public int UniversityStudent_ID { get; set; }
        public string IdentityNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string DocumentImage { get; set; }
        public bool? IsActive { get; set; }
        public decimal Student_ID { get; set; }


    }


    public partial class TravelAdvertisementVM
    {
        public int ID { get; set; }
        public int StudentId { get; set; }
        public string AdName { get; set; }
        public string AdDate { get; set; }
        public string DepartingDate { get; set; }
        public string ReturningDate { get; set; }
        public bool? IsActive { get; set; }
        public string TicketNumber { get; set; }
        public string NationalityID { get; set; }
        public string DegreeID { get; set; }
        public string StStatusID { get; set; }
        public string CustomFieldId { get; set; }
        public string AdEndDate { get; set; }
        public int TravelOrder { get; set; }
        public int TravelRequestId { get; set; }
        public bool ShowDetalies { get; set; }

    }

}

