
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using StudentProfile.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using StudentProfile.Web.VM;
using System.Data.Entity;
using System.Threading.Tasks;

namespace StudentProfile.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]

    public partial class TravelOrdersController : Controller
    {
        private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();
        private notify _notify;


        #region Methods
        //Getting Students Travel info by Travel Order ID
        public ActionResult _TravelOrdersDetails(int? id, bool? isTravelAgent)
        {

            var data = db.Usp_GetTravelOrderDetails(id).ToList();
            ViewBag.TripNumber = data.FirstOrDefault()?.TripNumber;
            ViewBag.TripPath = data.FirstOrDefault()?.TripPath;
            ViewData["Hidden"] = isTravelAgent == null ? false : isTravelAgent;

            return PartialView("_TravelOrdersDetails", data);
        }
        public ActionResult _GroupedTravelOrdersDetails(int? TravelOrderID, bool? isTravelAgent)
        {

            var data = db.Usp_GetTravelOrderDetails(TravelOrderID).GroupBy(x => new
            {

                x.StudentID,
                x.FinancialApprovalDate,
                x.ManagerialApprovalDate,
                x.Student_Name,
                x.Student_Name_S,
                x.National_ID,
                x.NATIONALITY_DESC,
                x.Level_Desc,
                x.Faculty_Name,
                x.MOBILE_NO,
                x.EMAIL,
                x.TripNumber,
                x.TripPath,
                x.FromDate,
                x.ToDate
            }).Select(x => new Usp_GetTravelOrderDetails_Result
            {
                ID = x.Count() > 0 ? x.FirstOrDefault().ID : 0,
                StudentID = x.Key.StudentID,
                TicketNumber = "",
                FinancialApprovalDate = x.Key.FinancialApprovalDate,
                ManagerialApprovalDate = x.Key.ManagerialApprovalDate,
                Student_Name = x.Key.Student_Name,
                Student_Name_S = x.Key.Student_Name_S,
                National_ID = x.Key.National_ID,
                NATIONALITY_DESC = x.Key.NATIONALITY_DESC,
                Level_Desc = x.Key.Level_Desc,
                Faculty_Name = x.Key.Faculty_Name,
                MOBILE_NO = x.Key.MOBILE_NO,
                EMAIL = x.Key.EMAIL,
                TripNumber = x.Key.TripNumber,
                TripPath = x.Key.TripPath,
                FromDate = x.Key.FromDate,
                ToDate = x.Key.ToDate,
                GivenAmount = x.Sum(g => g.GivenAmount),
                ApprovedAmount = x.Sum(g => g.ApprovedAmount)
            }).ToList();
            ViewBag.TripNumber = data.FirstOrDefault()?.TripNumber;
            ViewBag.TripPath = data.FirstOrDefault()?.TripPath;
            ViewData["Hidden"] = isTravelAgent == null ? false : isTravelAgent;

            return PartialView("_TravelOrdersDetails", data);
        }
        public ActionResult _TravelOrdersTicketDetails(int? id, bool? isTravelAgent)
        {

            var data = db.Usp_GetTravelOrderDetails(id).GroupBy(x => new
            {

                x.StudentID,
                x.FinancialApprovalDate,
                x.ManagerialApprovalDate,
                x.Student_Name,
                x.Student_Name_S,
                x.National_ID,
                x.NATIONALITY_DESC,
                x.Level_Desc,
                x.Faculty_Name,
                x.MOBILE_NO,
                x.EMAIL,
                x.TripNumber,
                x.TripPath,
                x.FromDate,
                x.ToDate
            }).Select(x => new Usp_GetTravelOrderDetails_Result
            {
                ID = x.Count() > 0 ? x.FirstOrDefault().ID : 0,
                StudentID = x.Key.StudentID,
                TicketNumber = "",
                FinancialApprovalDate = x.Key.FinancialApprovalDate,
                ManagerialApprovalDate = x.Key.ManagerialApprovalDate,
                Student_Name = x.Key.Student_Name,
                Student_Name_S = x.Key.Student_Name_S,
                National_ID = x.Key.National_ID,
                NATIONALITY_DESC = x.Key.NATIONALITY_DESC,
                Level_Desc = x.Key.Level_Desc,
                Faculty_Name = x.Key.Faculty_Name,
                MOBILE_NO = x.Key.MOBILE_NO,
                EMAIL = x.Key.EMAIL,
                TripNumber = x.Key.TripNumber,
                TripPath = x.Key.TripPath,
                FromDate = x.Key.FromDate,
                ToDate = x.Key.ToDate,
                GivenAmount = x.Sum(g => g.GivenAmount),
                ApprovedAmount = x.Sum(g => g.ApprovedAmount)
            }).ToList();
            return PartialView("_TravelOrdersTicketDetails", data);
        }
        //Saving Current Level
        public JsonResult ExportToNextLevel(List<int> selectedOrders)
        {
            bool syncResult = true;
            List<TravelOrders> TravelOrdersList = new List<TravelOrders>();
            if (selectedOrders != null && selectedOrders.Count > 0)
            {

                List<TravelOrderDetails> list = new List<TravelOrderDetails>();
                var students = db.StudentsTravelOrder.Where(x => selectedOrders.Any(p => p == x.TravelOrderID)).ToList();

                if (students.Any(x => x.StudentsTicketsDetails.Count() == 0) || !students.All(x => x.GivenAmount > 0))
                {
                    return Json(_notify = new notify() { Message = "من فضلك تأكد من إدخال بيانات و قيم رحلات الطلاب أولا", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }

                foreach (int item in selectedOrders)
                {
                    TravelOrderDetails model = new TravelOrderDetails();
                    var currentLevel = db.TravelOrderDetails.Where(x => x.TravelOrderID == item).Count();
                    model.TravelOrderID = item;
                    model.Date = DateTime.Now;
                    model.PhaseUserID = GetPhaseUserID(currentLevel + 1);
                    list.Add(model);
                    if (currentLevel == 2)
                    {
                        var currentStudentsAmountList = db.StudentsTravelOrder.Where(x => selectedOrders.Any(p => p == x.TravelOrderID)).ToList();
                        currentStudentsAmountList.ForEach(x => x.ApprovedAmount = x.GivenAmount);
                        decimal TOTAL_TRAVEL_PRICE_1 = 0;
                        TravelOrders travelOrder = db.TravelOrders.Find(item);
                        var AgentRefNumber = db.TravelOrders.Select(x => x.AgentRefNumber).ToList();
                        int temp;
                        int max = AgentRefNumber.Select(n => int.TryParse(n, out temp) ? temp : 8999).Max();
                        int upcomingAgentNumber = max + 1;
                        travelOrder.AgentRefNumber = upcomingAgentNumber.ToString();
                        travelOrder.IsCanceled = false;
                        db.SaveChanges();
                        foreach (var student in db.StudentsTravelOrder.Where(z => z.TravelOrderID == item).ToList())
                        {

                            TOTAL_TRAVEL_PRICE_1 += student.ApprovedAmount.HasValue ? student.ApprovedAmount.Value : student.GivenAmount.Value;

                            var data = new PostData()
                            {
                                FINANCIAL_YEAR_NO = DateTime.Now.Year,
                                DOC_RELATION_NO = upcomingAgentNumber,
                                DOC_RELATION_DATE = DateTime.Now.ToString("dd/MMM/yyyy", new CultureInfo("EN-US")),
                                //DateTime.Now.ToString("dd/MMMM/yyyy",IFormatProvider),
                                CLASS_OK = student.StTravelRequest.TravelClass,
                                //FROM_COUNTRY = student.StTravelRequest.AirPort.CountryID,
                                //to_COUNTRY = student.StTravelRequest.AirPort1.CountryID,
                                ONE_WAY = student.StTravelRequest.TravelAdvertisement.flightsType == "O" ? 1 : 2,
                                ST_ID = student.StudentID,
                                ST_NO = student.StudentID.ToString(),
                                ST_PRICE = student.ApprovedAmount.HasValue ? student.ApprovedAmount.Value : student.GivenAmount.Value,
                                TRAVEL_PRICE_1 = TOTAL_TRAVEL_PRICE_1//student.ApprovedAmount.HasValue ? student.ApprovedAmount.Value : student.GivenAmount.Value
                            };
                            syncResult = ExportToFinancialSystemAsync(data);

                        }

                        
                    }
                }
                try
                {
                    db.TravelOrderDetails.AddRange(list);
                    db.TravelOrders.Where(x => selectedOrders.Any(p => p == x.ID)).ToList().ForEach(x => x.IsCanceled = false);
                    if (syncResult) { db.SaveChanges(); MusaferHub.UpdateFinancialTravelOrders(); }


                    else
                    {
                        return Json(_notify = new notify() { Message = "حدث خطأ أثناء تصدير أوامر الإركاب وترحيلها للنظام المالي للجامعة", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                    }
                }

                catch (Exception)
                {
                    return Json(_notify = new notify() { Message = "حدث خطأ أثناء تصدير أوامر الإركاب برجاء مراجعتها والمتابعة مرة أخرى", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }

                return Json(_notify = new notify() { Message = "تم تصدير أوامر الإركاب بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(_notify = new notify() { Message = "من فضلك أختر أوامر إركاب أولا", Type = "info", status = 200 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckIfHasAmount(int travelOrderID, int levelOrder)
        {
            int count = 0;
            if (levelOrder == 2)
            {
                count = db.StudentsTravelOrder.Where(x => x.TravelOrderID == travelOrderID && (x.GivenAmount == null || x.GivenAmount < 1)).Count();
            }
            else if (levelOrder == 3)
            {
                count = db.StudentsTravelOrder.Where(x => x.TravelOrderID == travelOrderID && (x.ApprovedAmount == null || x.ApprovedAmount < 1)).Count();
            }
            return Json(count, JsonRequestBehavior.AllowGet);
        }
        //Exporting Devexpress Report
        public void WriteDocumentToResponse(byte[] documentData, string format, bool isInline, string fileName)
        {
            string contentType;
            string disposition = (isInline) ? "inline" : "attachment";

            switch (format.ToLower())
            {
                case "xls":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "xlsx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "mht":
                    contentType = "message/rfc822";
                    break;
                case "html":
                    contentType = "text/html";
                    break;
                case "txt":
                case "csv":
                    contentType = "text/plain";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                default:
                    contentType = String.Format("application/{0}", format);
                    break;
            }

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", String.Format("{0}; filename={1}", disposition, fileName));
            Response.BinaryWrite(documentData);
            Response.End();
        }
        //Getting PhaseUserID depending on UserID and currentLevel
        public int GetPhaseUserID(int lvl)
        {
            if (Session["UserId"] != null)
            {
                var user = Session["UserId"] as DashBoard_Users;
                var phaseUser = db.PhasesUsers.Where(x => x.UserID == user.ID && x.TravelOrderPhase.Order == lvl).FirstOrDefault();
                return phaseUser.ID;
            }
            return 15;
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
        #region شاشة انشاء و تصدير امر الاركاب للطيار
        public ActionResult Index()
        {
            var permissions = CheckPermissionsfn(117);
            ViewBag.View = permissions.View;
            ViewBag.Read = permissions.Read;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Update = permissions.Update;
            ViewBag.Create = permissions.Create;


            if (permissions.Read || permissions.View)
            {
                ViewBag.TravelAds = db.TravelAdvertisement.Where(x => x.AdStartDate <= DateTime.Now && x.DepartingStart > DateTime.Now && x.StTravelRequest.Count() > 0)
                 .Select(x => new SelectListItem { Text = x.AdName, Value = x.ID.ToString() }).ToList();

                return View();
            }
            return RedirectToAction("NoPermissions", "Security");
            //Phase One >> إنشاء اوامر الإركاب
            //if (IsAuthorizeToPhase(1))
            //{
            //    ViewBag.TravelAds = db.TravelAdvertisement.Where(x => x.AdStartDate <= DateTime.Now && x.DepartingStart > DateTime.Now && x.StTravelRequest.Count() > 0)
            //        .Select(x => new SelectListItem { Text = x.AdName, Value = x.ID.ToString() }).ToList();
            //    return View();
            //}

            return RedirectToAction("NotAuthorized", "Security");
        }
        public ActionResult _GetTravelOrders(int? AdID, int? radioList)
        {
            //var user = Session["UserId"] as DashBoard_Users;


            var returnData = db.Usp_GetFreeTravelRequestSummery(AdID, radioList).ToList();
            return PartialView("_GetTravelOrders", returnData);
        }
        public JsonResult GenerateNewTravelOrders(List<int> requestIDs, int? type)
        {
            var dt = requestIDs.Select(x => new ListParams { ID = x }).ToList().CopyToDataTable();
            var CurrentUser = Session["UserId"] as DashBoard_Users;
            var connectionString = ConfigurationManager.ConnectionStrings["SchoolAccGam3aEntities"].ConnectionString;

            var builder = new EntityConnectionStringBuilder(connectionString);
            var regularConnectionString = builder.ProviderConnectionString;
            using (var sqlConnection = new SqlConnection(regularConnectionString))
            {
                sqlConnection.Open();
                var cmd = new SqlCommand
                {
                    Connection = sqlConnection,
                    CommandTimeout = 0,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Usp_GenerateNewTravelOrders"
                };
                cmd.Parameters.AddWithValue("@userID", CurrentUser.ID);
                cmd.Parameters.AddWithValue("@type", type.HasValue ? type.Value : (int?)null);
                var list = cmd.Parameters.AddWithValue("@selectedTravelRequests", dt);
                list.SqlDbType = SqlDbType.Structured;
                list.TypeName = "[dbo].[IntegerParams]";
                var result = cmd.ExecuteNonQuery();
                sqlConnection.Close();
                if (result > 0)
                {
                    return Json(_notify = new notify() { Message = "تم  انشاء اوامر الاركاب وتصديرها للطيار بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(_notify = new notify() { Message = "حدث خطأ أثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
                }
            }


        }

        public ActionResult _GetFreeTravelRequests(int? AdID, int? radioList)
        {
            ViewData["type"] = radioList;
            ViewData["AdID"] = AdID;
            return PartialView("_GetFreeTravelRequests", db.Usp_GetFreeTravelRequests(AdID == null ? null : AdID.ToString()).ToList());
        }

        public ActionResult ReturnGrid(int AdID, int? radioList)
        {
            ViewData["type"] = radioList;

            var returnData = db.Usp_GetFreeTravelRequestSummery(AdID, radioList).ToList();
            return PartialView("_GetTravelOrders", returnData);
        }
        #endregion

        #region شاشة تصدير امر الاركاب للمالية
        public ActionResult TravelAgents()
        {
            // تسعير أوامر الإركاب >>Phase 2
            if (IsAuthorizeToPhase(2))
            {
                return View();
            }

            return RedirectToAction("NotAuthorized", "Security");

        }
        public ActionResult _GetTravelOrdersPricing()
        {
            // 
            //=============================================================================================================
            //تم التعديل بناءا علي طلب من أستاذ محمد سمسم لكي تظهر أوامر الإركاب المرفوضة من الإعتماد المالي مرة أخري 
            //=============================================================================================================

            var model = db.TravelOrders.Where(x => // x.IsCanceled == false &&
                                                   x.StudentsTravelOrder.Count() > 0 &&
                                                   x.TravelOrderDetails.Count() == 1)
                                        .Select(x => new
                                        {
                                            x.ID,
                                            x.TripPath,
                                            x.IsCanceled,
                                            x.TripNumber,
                                            StudentsCount = x.StudentsTravelOrder.GroupBy(z => z.StudentID).Count(),
                                            TravelType = x.StudentsTravelOrder.FirstOrDefault().StTravelRequest.FlightsType == "R" ? "ذهاب وعودة" : "ذهاب فقط",
                                            TravelOrderDate = x.TravelOrderDetails.FirstOrDefault(c => c.PhasesUsers.TravelOrderPhase.Order == 1).Date,
                                            TripType = x.TripType == 0 ? "خط السير فقط" : x.TripType == 1 ? "خط السير والجنسية" : "",
                                            Nationality = x.TripType == 1 ? db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentsTravelOrder.FirstOrDefault().StTravelRequest.StudentID).NATIONALITY_DESC : ""
                                        }).ToList();
            return PartialView("_GetTravelOrdersPricing", model);
        }

        public ActionResult _StudentsTicketingDetails(int travelOrderId, bool lastLevel, bool addTicket)
        {
            ViewData["lastLevel"] = lastLevel;
            ViewData["addTicket"] = addTicket;

            var model = db.Usp_GetTravelOrderDetailsWithTicketDetails(travelOrderId).ToList();


            return PartialView("_StudentsTicketingDetails", model);
        }
        public JsonResult SaveTripPricing(List<TravelOrdersAmounts> details)
        {


            foreach (var item in details)
            {
                var result = db.StudentsTravelOrder.Find(item.Id);
                result.GivenAmount = item.Amount;

            }
            try
            {
                db.SaveChanges();
                return Json(_notify = new notify() { Message = "تم تسعير أوامر الإركاب بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(_notify = new notify() { Message = "حدث خطأ أثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion
        #region شاشة تصدير امر الاركاب لمدير السفر

        public ActionResult TravelOrdersFinancial()
        {
            //إعتماد أوامر الإركاب Phase >> 3
            if (IsAuthorizeToPhase(3))
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }

        public ActionResult _GetTravelOrdersFinancial()
        {
            // && x.IsCanceled == false
            var AgentRefNumber = db.TravelOrders.Select(x => x.AgentRefNumber).ToList();
            int temp;
            int max = AgentRefNumber.Select(n => int.TryParse(n, out temp) ? temp : 8999).Max();
            int upcomingAgentNumber = max + 1;

            var list = db.TravelOrders.Where(x => x.TravelOrderDetails.Count() == 2 &&
                                                  x.StudentsTravelOrder.Count() > 0)
            .Select(x => new
            {
                x.ID,
                x.TripNumber,
                x.TripPath,
                x.CreationDate,
                x.AgentRefNumber,
                x.Notes,
                IsReturned = x.StudentsTravelOrder.Any(p => p.ApprovedAmount != p.GivenAmount && p.ApprovedAmount != null),
                StudentsCount = x.StudentsTravelOrder.GroupBy(z => z.StudentID).Count(),
                TravelOrderDate = x.TravelOrderDetails.FirstOrDefault(c => c.PhasesUsers.TravelOrderPhase.Order == 2).Date,
                SumGivenAmount = x.StudentsTravelOrder.Where(c => c.TravelOrderID == x.ID).Sum(c => c.GivenAmount),
                x.IsCanceled
            })
            .ToList();
            var anotherList = new List<TravelOrdersVM>();
            for (int i=0;i<list.Count;i++)
            {
                TravelOrdersVM vm = new TravelOrdersVM();
                if (i == 0)
                {
                    vm.AgentRefNumber = upcomingAgentNumber;
                }
                else
                {

                    vm.AgentRefNumber = upcomingAgentNumber+i;
                }
                vm.ID = list[i].ID;
                vm.TripNumber = list[i].TripNumber;
                vm.TripPath = list[i].TripPath;
                vm.CreationDate = list[i].CreationDate.Value;
                vm.Notes = list[i].Notes;
                vm.IsReturned = list[i].IsReturned;
                vm.StudentsCount = list[i].StudentsCount;
                vm.TravelOrderDate = list[i].TravelOrderDate;
                vm.SumGivenAmount = list[i].SumGivenAmount;
                vm.IsCanceled = list[i].IsCanceled;
                anotherList.Add(vm);
            }

            return PartialView("_GetTravelOrdersFinancial", anotherList);
        }
        public JsonResult SaveTripFinance(List<TravelOrdersAmounts> details)
        {
            foreach (var item in details)
            {
                var result = db.StudentsTravelOrder.Find(item.Id);
                result.ApprovedAmount = item.Amount;

            }
            try
            {

                db.SaveChanges();
                return Json(_notify = new notify() { Message = "تم اعتماد أوامر الإركاب بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(_notify = new notify() { Message = "حدث خطأ أثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
            }
        }

        // الشاشة الخاصة بالاعتماد المالي
        public JsonResult CancelTravelOrder(int id)
        {
            var CurrentUser = Session["UserId"] as DashBoard_Users;


            var TravelOrderDetail = db.TravelOrderDetails
                                      .FirstOrDefault(x => x.TravelOrderID == id &&
                                                           x.PhasesUsers.TravelOrderPhase.Order == db.TravelOrderDetails
                                                                                                     .Where(p => p.TravelOrderID == id)
                                                                                                     .Max(z => z.PhasesUsers.TravelOrderPhase.Order)
                                      );


            db.TravelOrderDetails.Remove(TravelOrderDetail);

            var travelOrder = db.TravelOrders.Find(id);
            travelOrder.IsCanceled = true;
            travelOrder.CanceledBy = CurrentUser.ID;

            //var TravelOrderDetail = db.TravelOrderDetails
            //                          .FirstOrDefault(x => x.TravelOrderID == id && x.PhasesUsers.TravelOrderPhase.Order > 1);

            try
            {
                db.SaveChanges();
                return Json(_notify = new notify { Message = "تم إلغاء أمر الإركاب بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(_notify = new notify() { Message = "حدث خطأ أثناء الإلغاء", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }

        }
        public bool ExportToFinancialSystemAsync(PostData data)
        {
            try
            {
                string url = @"https://api.iu.edu.sa/api/INSERT_STUDENT_TRAVEL";

                // Create a request using a URL that can receive a post.   
                WebRequest request = WebRequest.Create(url);
                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer 07E7E603-8890-4D4A-8F1A-0B4AF158E75A");
                // Set the Method property of the request to POST.  
                request.Method = "POST";

                // Create POST data and convert it to a byte array.  
                string postData = new JavaScriptSerializer().Serialize(data);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                // Set the ContentType property of the WebRequest.  
                request.ContentType = "application/json";
                // Set the ContentLength property of the WebRequest.  
                request.ContentLength = byteArray.Length;

                // Get the request stream.  
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.  
                dataStream.Close();

                // Get the response.  
                WebResponse response = request.GetResponse();

                // Get the stream containing content returned by the server.  
                // The using block ensures the stream is automatically closed.
                using (dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.  
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.  
                    string responseFromServer = reader.ReadToEnd().ToString();
                    // Display the content.  
                    if (responseFromServer != '"' + "Done" + '"')
                    {
                        return false;
                    }
                }

                // Close the response.  
                response.Close();

                return true;
            }
            catch (Exception)
            {

                return false;
            }


            //bool result = false;

            //var client = new HttpClient();
            //var endpoint = "https://api.iu.edu.sa/api/INSERT_STUDENT_TRAVEL";
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", " 07E7E603-8890-4D4A-8F1A-0B4AF158E75A");

            //var response = await client.PostAsJsonAsync(endpoint, data);
            //if (response.IsSuccessStatusCode)
            //{
            //    result = true;
            //}
            //return result;
        }
        #endregion
        #region شاشة متابعة وموافقة مدير السفر

        public ActionResult TravelOrderManager()
        {

            ViewBag.Advertisement = db.TravelAdvertisement.Where(x => x.AdStartDate <= DateTime.Now && x.DepartingStart > DateTime.Now && x.StTravelRequest.Count() > 0)
                .Select(x => new SelectListItem { Text = x.AdName, Value = x.ID.ToString() }).ToList();

            //إعتماد مدير السفر  Phase >>4
            if (IsAuthorizeToPhase(4))
            {
                return View();
            }

            return RedirectToAction("NotAuthorized", "Security");
        }
        public ActionResult _GetTravelOrderManager(int level)
        {
            ViewData["Level"] = level;

            var list = db.TravelOrders.Where(x => x.TravelOrderDetails.Count() == level && x.StudentsTravelOrder.Count() > 0)
                                        .Select(x => new
                                        {
                                            x.ID,
                                            x.TripPath,
                                            x.IsCanceled,
                                            x.TripNumber,
                                            x.AgentRefNumber,
                                            StudentsCount = x.StudentsTravelOrder.GroupBy(z => z.StudentID).Count(),
                                            TravelType = x.StudentsTravelOrder.FirstOrDefault().StTravelRequest.FlightsType == "R" ? "ذهاب وعودة" : "ذهاب فقط",
                                            TravelOrderDate = x.TravelOrderDetails.FirstOrDefault(c => c.PhasesUsers.TravelOrderPhase.Order == 1).Date,
                                            TripType = x.TripType == 0 ? "خط السير فقط" : x.TripType == 1 ? "خط السير والجنسية" : "",
                                            Nationality = x.TripType == 1 ? db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentsTravelOrder.FirstOrDefault().StTravelRequest.StudentID).NATIONALITY_DESC : ""
                                        }).ToList();

            return PartialView("_GetTravelOrderManager", list);
        }

        public ActionResult GetTravelOrderApproved()
        {
            var permissions = GetPermissionsFn(78);
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

        //public ActionResult _GetTravelOrderApproved()
        // {
        //    var phasesCount = db.TravelOrderPhase.Count();
        //    var list =(from t in db.TravelOrders.Where(x => x.TravelOrderDetails.Count() == phasesCount && x.IsCanceled == false).ToList()
        //                select new TravelOrdersVM
        //                {
        //                    TripNumber=t.TripNumber,
        //                    TripPath =t.TripPath , 
        //                    ID=t.ID,
        //                    StudentCount= t.StudentsTravelOrder!=null?t.StudentsTravelOrder.Count:0
        //                }
        //               ).ToList();
        //    return PartialView("_GetTravelOrderApproved", list);
        //}
        public ActionResult _GetTravelOrderApproved(string TripNumber,string AgentRefNumber,DateTime? CreateDateFrom, DateTime? CreateDateTo)
        {
            var phasesCount = db.TravelOrderPhase.Count();
            var listQ = db.TravelOrders.Where(x => x.TravelOrderDetails.Count() == phasesCount && x.IsCanceled == false
                && (TripNumber == null || x.TripNumber ==TripNumber)
                && (AgentRefNumber == null || x.AgentRefNumber == AgentRefNumber)
                && (CreateDateFrom == null || DbFunctions.TruncateTime(x.CreationDate) >= DbFunctions.TruncateTime(CreateDateFrom)) 
                && (CreateDateTo == null || DbFunctions.TruncateTime(x.CreationDate) <= DbFunctions.TruncateTime(CreateDateTo))
            ).Select(x => new TravelOrdersVM
            {
                ID = x.ID,
                TripPath = x.TripPath,
                IsCanceled = x.IsCanceled,
                TripNumber = x.TripNumber,
                AgentRefNumber1 = x.AgentRefNumber,
                CreationDate =x.CreationDate.Value,
                StudentsCount = x.StudentsTravelOrder.Count(),
                ApprovedAmount = x.StudentsTravelOrder.Select(s => s.ApprovedAmount).DefaultIfEmpty(0).Sum(),
                TravelType = x.StudentsTravelOrder.Any() ? x.StudentsTravelOrder.FirstOrDefault().StTravelRequest.FlightsType == "R" ? "ذهاب وعودة" : "ذهاب فقط" : null,
                TravelOrderDate = x.TravelOrderDetails.FirstOrDefault(c => c.PhasesUsers.TravelOrderPhase.Order == 1).Date,
                TripType = x.TripType == 0 ? "خط السير فقط" : x.TripType == 1 ? "خط السير والجنسية" : "",
                Nationality = x.TripType == 1 ? db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentsTravelOrder.FirstOrDefault().StTravelRequest.StudentID).NATIONALITY_DESC : ""
            });
            return PartialView("_GetTravelOrderApproved", listQ.ToList());
        }
        public JsonResult FinishProcess(List<int> selectedOrders)
        {
            if (selectedOrders != null && selectedOrders.Count > 0)
            {
                List<TravelOrderDetails> list = new List<TravelOrderDetails>();
                foreach (int item in selectedOrders)
                {
                    TravelOrderDetails model = new TravelOrderDetails();
                    var currentLevel = db.TravelOrderDetails.Where(x => x.TravelOrderID == item).Count();
                    model.TravelOrderID = item;
                    model.Date = DateTime.Now;
                    model.PhaseUserID = GetPhaseUserID(currentLevel + 1);


                    list.Add(model);
                    TravelOrders _Travelorder = db.TravelOrders.Where(x => x.ID == item).SingleOrDefault();
                    _Travelorder.IsApproved = true;
                }
                try
                {
                    db.TravelOrderDetails.AddRange(list);
                    db.SaveChanges();
                }

                catch (Exception)
                {
                    return Json(_notify = new notify() { Message = "حدث خطأ أثناء اعتماد أوامر الإركاب برجاء مراجعتها والمتابعة مرة أخرى", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }

                return Json(_notify = new notify() { Message = "تم اعتماد أوامر الإركاب بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(_notify = new notify() { Message = "من فضلك أختر أوامر إركاب أولا", Type = "info", status = 200 }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region شاشة طباعة أمر الاركاب

        public ActionResult TravelOrderPrint(int travelOrderID)
        {
            var model = db.TravelOrders.Where(x => x.ID == travelOrderID).SingleOrDefault();

            ViewBag.StudentsTravelOrder = db.StudentsTravelOrder.Where(m=>m.TravelOrderID == travelOrderID).Count();
            var Phase = model.TravelOrderDetails.Where(x => x.PhasesUsers.TravelOrderPhase.Order == 2).FirstOrDefault();
            if (Phase != null)
            {
                ViewBag.PhaseOne = true;
            }
            else
            {
                ViewBag.PhaseOne = false;
            }

            ViewData["WrittenAmount"] = db.Tafkeet_Sp(model.StudentsTravelOrder.Sum(x => x.ApprovedAmount)).SingleOrDefault()?.ToString();
            var TravelClass = model.StudentsTravelOrder.FirstOrDefault();
            var StudentsTravelOrder = TravelClass.StTravelRequest;
            ViewBag.FlightsType = StudentsTravelOrder.FlightsType;
            ViewBag.Level = db.config.AsEnumerable().FirstOrDefault(c => c.ID == StudentsTravelOrder.TravelClass).Value;
            return View(model);
        }

        public ActionResult _TravelOrderStudentsPrint(int? id)
        {

            var data = db.Usp_GetTravelOrderDetails(id).GroupBy(x => new
            {

                x.StudentID,
                x.FinancialApprovalDate,
                x.ManagerialApprovalDate,
                x.Student_Name,
                x.Student_Name_S,
                x.National_ID,
                x.NATIONALITY_DESC,
                x.Level_Desc,
                x.Faculty_Name,
                x.MOBILE_NO,
                x.EMAIL,
                x.TripNumber,
                x.TripPath,
                x.FromDate,
                x.ToDate
            }).Select(x => new Usp_GetTravelOrderDetails_Result
            {
                ID = x.Count() > 0 ? x.FirstOrDefault().ID : 0,
                StudentID = x.Key.StudentID,
                TicketNumber = x.Count() > 0 ? string.Join(" - ", x.Select(p => "[" + p.TicketNumber + "]")) : string.Empty,
                FinancialApprovalDate = x.Key.FinancialApprovalDate,
                ManagerialApprovalDate = x.Key.ManagerialApprovalDate,
                Student_Name = x.Key.Student_Name,
                Student_Name_S = x.Key.Student_Name_S,
                National_ID = x.Key.National_ID,
                NATIONALITY_DESC = x.Key.NATIONALITY_DESC,
                Level_Desc = x.Key.Level_Desc,
                Faculty_Name = x.Key.Faculty_Name,
                MOBILE_NO = x.Key.MOBILE_NO,
                EMAIL = x.Key.EMAIL,
                TripNumber = x.Key.TripNumber,
                TripPath = x.Key.TripPath,
                FromDate = x.Key.FromDate,
                ToDate = x.Key.ToDate,
                GivenAmount = x.Sum(g => g.GivenAmount),
                ApprovedAmount = x.Sum(g => g.ApprovedAmount)
            }).ToList();


            return PartialView("_TravelOrderStudentsPrint", data);
        }

        public ActionResult CanselTravelOrderDetails(int? id)
        {

            var travelDetalies = db.TravelOrderDetails.Where(x => x.TravelOrderID == id).ToList();
            db.TravelOrderDetails.RemoveRange(travelDetalies);
            var studentsTravelOrder = db.StudentsTravelOrder.Where(x => x.TravelOrderID == id).ToList();
            db.StudentsTravelOrder.RemoveRange(studentsTravelOrder);

            try
            {
                db.SaveChanges();

                db.TravelOrders.Remove(db.TravelOrders.Find(id));
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(_notify = new notify() { Message = "خطأ اثناء الغاء امر الاركاب", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }

            return Json(_notify = new notify() { Message = "تم الغاء امر الاركاب بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region شاشة تفاصيل طلاب امر الاركاب كتقرير

        public ActionResult _TravelOrderReport(int? TravelOrderID)
        {

            var report = new InstituteTravelStudentsDetailsReport();

            var RptData = db.RPT_TravelStudentDetailsReport(null, null, TravelOrderID)
                           .Select(x => new
                           {
                               x.DEGREE_DESC,
                               x.FACULTY_NAME,
                               x.EMAIL,
                               x.END_DATE,
                               x.FROM_DATE,
                               x.LAST_UPDATE_DATE,
                               x.MOBILE_NO,
                               x.NATIONALITY_CODE,
                               x.NATIONALITY_DESC,
                               x.REQUEST_SEQ,
                               x.STUDENT_ID,
                               x.STUDENT_NAME,
                               x.TRAVEL_LINE_DESC,
                              x.TRAVEL_LINE_NO,
                               x.V_INTEGRATION_NEW_STUDENT_NAME
                           }).ToList();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = RptData;
            };
            using (MemoryStream ms = new MemoryStream())
            {
                report.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                WriteDocumentToResponse(ms.ToArray(), "pdf", true, "Report.pdf");
                return null;
            }
        }
        #endregion
        #region شاشة اصدار التذاكر من الطيار

        public ActionResult TicketAssigning()
        {
            //اصدار التذاكر من الطيار  Phase >>5
            if (IsAuthorizeToPhase(5))
            {
                return View();
            }

            return RedirectToAction("NotAuthorized", "Security");
        }
        public ActionResult _GetTicketAssigning()
        {
            var phasesCount = db.TravelOrderPhase.Count();
            var list = db.TravelOrders.Where(x => x.TravelOrderDetails.Count() == phasesCount - 1 && x.IsCanceled == false).Select(x =>
                  new
                  {
                      TravelOrderID = x.ID,
                      TripNumber = x.TripNumber,
                      TripPath = x.TripPath,
                      AgentRefNumber = x.AgentRefNumber,
                  }).ToList();
            return PartialView("_GetTicketAssigning", list);
        }
        public ActionResult InsertFlightPopup(int? studentticketDetailID, bool lastLevel)
        {
            ViewData["lastLevel"] = lastLevel;
            var studentDetail = new StudentsTicketsDetails();
            if (studentticketDetailID != null && studentticketDetailID > 0)
            {
                studentDetail = db.StudentsTicketsDetails.SingleOrDefault(x => x.ID == studentticketDetailID);
            }
            return PartialView("InsertFlightPopup", studentDetail);
        }
        public JsonResult SaveTicketNumber(/*int id, string ticketNumber, decimal approvedAmount*/ List<int> tickeModel)
        {
            var _notify = new notify() { Message = "تم حفظ رقم التذكرة بنجاح", Type = "success", status = 200 };
            if (tickeModel != null)
            {
                if (tickeModel.Count > 0)
                {
                    var studentTravelOrderList = db.StudentsTravelOrder.Where(x => tickeModel.Any(p => p == x.ID));
                    var travelOrders = studentTravelOrderList.ToList().Select(x => x.TravelOrders);

                    if (studentTravelOrderList.ToList().Any(x => x.ApprovedAmount != x.GivenAmount))
                    {
                        travelOrders.ToList().ForEach(x =>
                        {
                            x.IsApproved = null;

                        });
                        var travelOrderDetails = travelOrders.SelectMany(x => x.TravelOrderDetails).ToList();
                        db.TravelOrderDetails.RemoveRange(travelOrderDetails.Where(x => x.PhasesUsers.TravelOrderPhase.Order > 2).ToList());
                        _notify = new notify() { Message = "تم إعادة أمر الإركاب للاعتماد المالي لاعتماد قيمة التذكرة المعدلة", Type = "success", status = 200 };
                    }
                    else
                    {
                        if (db.StudentsTicketsDetails.Where(x => (x.TicketNumber == null || x.TicketNumber == "") &&
                                                                 studentTravelOrderList.Any(p => p.ID == x.StudentsTravelOrderID)).Count() > 0 &&
                                                                 studentTravelOrderList.SelectMany(c => c.StudentsTicketsDetails).Count() > 0
                                                            )
                        {
                            return this.JsonMaxLength(new notify() { Message = "برجاء مراجعة أرقام التذاكر قبل عملية الحفظ والمتابعة مرة أخرى", Type = "error", status = 500 });
                        }

                        studentTravelOrderList.ToList().ForEach(x =>
                        {
                            x.TicketNumber = x.StudentsTicketsDetails.Count() > 0 ? string.Join("-", x.StudentsTicketsDetails.Select(p => "[" + p.TicketNumber + "]").ToList()) : string.Empty;
                        });

                        ExportToNextLevel(travelOrders.Select(x => x.ID).Distinct().ToList());
                        _notify = new notify() { Message = "تم حفظ رقم التذكرة بنجاح", Type = "success", status = 200 };

                    }
                    try
                    {
                        db.SaveChanges();

                    }
                    catch (Exception)
                    {
                        _notify = new notify() { Message = "حدث خطأ أثناء حفظ رقم التذكرة برجاء مراجعتها والمتابعة مرة أخرى", Type = "error", status = 500 };
                    }
                }
            }

            return Json(_notify, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveFlights(int StudentsTravelOrderID, string FromCity, string ToCity, string TicketNumber, decimal? TicketValue, int? ID)
        {
            if (ID != null && ID > 0)
            {
                var studentsTicketsDetails = db.StudentsTicketsDetails.SingleOrDefault(x => x.ID == ID);
                if (studentsTicketsDetails != null)
                {
                    studentsTicketsDetails.ToCity = ToCity;
                    studentsTicketsDetails.FromCity = FromCity;
                    studentsTicketsDetails.TicketNumber = TicketNumber;
                    studentsTicketsDetails.TicketValue = TicketValue.HasValue ? TicketValue.Value : 0;

                    db.Entry(studentsTicketsDetails).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    return this.JsonMaxLength("حدث خطأ برجاء تحديث الصفحة");
                }
            }
            else
            {
                StudentsTicketsDetails model = new StudentsTicketsDetails()
                {
                    FromCity = FromCity,
                    ToCity = ToCity,
                    TicketNumber = TicketNumber ?? "",
                    TicketValue = TicketValue.HasValue ? TicketValue.Value : 0,
                    StudentsTravelOrderID = StudentsTravelOrderID
                };
                db.StudentsTicketsDetails.Add(model);
            }
            try
            {
                db.SaveChanges();
                var allStudentFlights = db.StudentsTicketsDetails.Where(x => x.StudentsTravelOrderID == StudentsTravelOrderID).ToList();
                var studentTravelOrder = db.StudentsTravelOrder.Where(x => x.ID == StudentsTravelOrderID).SingleOrDefault();

                studentTravelOrder.TicketNumber = allStudentFlights.Count() > 0 ?
                    string.Join("-", allStudentFlights.Select(x => "[" + x.TicketNumber + "]")) : string.Empty;
                studentTravelOrder.GivenAmount = allStudentFlights.Select(x => x.TicketValue).DefaultIfEmpty(0).Sum();
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                return this.JsonMaxLength("حدث خطأ" + " " + ex.Message);
            }
            return this.JsonMaxLength("");
        }

        public JsonResult DeleteFlight(int ticketId)
        {
            string result = "";
            var FlightToBeDeleted = db.StudentsTicketsDetails.Where(x => x.ID == ticketId).SingleOrDefault();
            try
            {
                db.StudentsTicketsDetails.Remove(FlightToBeDeleted);
                db.SaveChanges();
                var allStudentFlights = db.StudentsTicketsDetails.Where(x => x.StudentsTravelOrderID == FlightToBeDeleted.StudentsTravelOrderID).ToList();
                var studentTravelOrder = db.StudentsTravelOrder.Where(x => x.ID == FlightToBeDeleted.StudentsTravelOrderID).SingleOrDefault();
                studentTravelOrder.TicketNumber = allStudentFlights.Count() > 0 ?
                    string.Join("-", allStudentFlights.Select(x => "[" + x.TicketNumber + "]")) : string.Empty;
                studentTravelOrder.GivenAmount = allStudentFlights.Sum(x => x.TicketValue);
                db.SaveChanges();

            }
            catch (Exception)
            {

                result = "حدث خطأ أثناء الحذف";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult _GetStudentByAdvertisement(decimal?[] AdvertisementCheckListBox)
        {
            var data = db.StTravelRequest.Where(x =>
            AdvertisementCheckListBox.Count() > 0 ? AdvertisementCheckListBox.Any(p => p == x.AdvertisementID) && x.StudentsTravelOrder.FirstOrDefault().StTravelRequestID != x.ID
            : true && x.StudentsTravelOrder.FirstOrDefault().StTravelRequestID != x.ID)
            .Select(x => new
            {
                x.ID,
                x.AdvertisementID,
                x.TravelAdvertisement.AdName,
                DepartingDate = x.Departing.Day + "/" + x.Departing.Month + "/" + x.Departing.Year,
                ReturningDate = x.Returning != null ? x.Returning.Value.Day + "/" + x.Returning.Value.Month + "/" + x.Returning.Value.Year : null,
                Student_ID = x.StudentID,
                Student_Name = db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).STUDENT_NAME,
                Faculty_Name = db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).FACULTY_NAME,
                Level_Desc = db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).LEVEL_DESC,
                NATIONALITY_DESC = db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).NATIONALITY_DESC,
                National_ID = db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.StudentID).NATIONAL_ID
            }).ToList();

            return PartialView("_StudentByAdvertisement", data);
        }


        public bool IsAuthorizeToPhase(int PhaseNumber)
        {
            var CurrentUser = Session["UserId"] as DashBoard_Users;
            var userPahses = db.PhasesUsers.Where(p => p.UserID == CurrentUser.ID && p.TravelOrderPhase.Order == PhaseNumber).ToList();
            if (userPahses.Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
    public class ListParams
    {
        public int ID { get; set; }
    }
    public class PostData
    {
        public int FINANCIAL_YEAR_NO { get; set; }
        public int DOC_RELATION_NO { get; set; }
        public decimal TRAVEL_PRICE_1 { get; set; }
        public string DOC_RELATION_DATE { get; set; }
        public int FROM_COUNTRY { get; set; }
        public int to_COUNTRY { get; set; }
        public int CLASS_OK { get; set; }
        public int ONE_WAY { get; set; }
        public string user { get; set; }
        public int ST_ID { get; set; }
        public string ST_NO { get; set; }
        public decimal ST_PRICE { get; set; }

    }

}