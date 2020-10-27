using DevExpress.Web;
using DevExpress.Web.Mvc;
using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using StudentProfile.DAL.Models;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System.Data.Entity.Core.EntityClient;
using StudentProfile.DAL.Models.VM;
using StudentProfile.Reports;
using DevExpress.XtraPrinting;
using DevExpress.DataAccess.ConnectionParameters;
using StudentProfile.Components;
using StudentProfile.Web.Reports;
using StudentProfile.Web.VM;
using System.Threading;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace StudentProfile.Web.Controllers
{

    public class Permissions
    {
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Save { get; set; }
    }
   // [RequireHttps]
    //[StudentProfile.Components.CustomAuthorizeHelper]
    //[StudentProfile.Components.CustomAuthorizeHelper]
    public class ReportsController : Controller
    {
        notify notify = new notify();

        //private HRMadinaEntities db = new HRMadinaEntities();
        private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();
        #region EmpDocuments

        [HttpPost]
        public JsonResult GetPermissions(int screenId)
        {
            var userId = 0;
            var CurrentUser = HttpContext.Session["UserId"] as DashBoard_Users;
            if (CurrentUser != null)
            {
                userId = CurrentUser.ID;
                if (CurrentUser.ID == 0)
                {
                    //return RedirectToAction("Login", "Login");
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            var perm = CheckPermissions.IsAuthorized(userId, screenId);

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

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// https://github.com/ExcelDataReader/ExcelDataReader
        /// </summary>
        public ActionResult UploadResidentExcelFile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            //رفع ملفات المقيمين
            var json = new JavaScriptSerializer().Serialize(GetPermissions(16).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            if (permissions.Read)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotAuthorized", "Security");
            }
        }

        public ActionResult UploadExcelFiles()

        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string[] errors;

            UploadedFile[] files = UploadControlExtension.GetUploadedFiles("excelFileUploader",
                MyUploadControlValidationSettings.Settings, out errors, (s, e) => { },
                UploadControl_FilesUploadComplete);

            return null;
        }

        public void UploadControl_FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {
            var files = ((MVCxUploadControl)sender).UploadedFiles;

            foreach (var file in files)
            {
                try
                {

                    var newCulture = new CultureInfo("en-US")
                    {
                        DateTimeFormat = { ShortDatePattern = "dd/MM/yyyy", DateSeparator = "/" }
                    };
                    Thread.CurrentThread.CurrentCulture = newCulture;

                    using (var reader = ExcelReaderFactory.CreateReader(file.FileContent))
                    {
                        var result = reader.AsDataSet();
                        var dataTable = result.Tables[0];
                        var lst = new List<MyDataTable>();
                        for (var index = 1; index < dataTable.Rows.Count; index++)
                        {
                            var dataRow = dataTable.Rows[index];
                            //الهوية
                            lst.Add(new MyDataTable
                            {
                                IdNumber = (string)dataRow[0].ToString(),
                                DocumentId = 1,
                                IdentityNum = (string)dataRow[0].ToString(),
                                IssueDate = DateTime.ParseExact(dataRow[9].ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US")),
                                ExpDate = DateTime.ParseExact(dataRow[9].ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US")),
                                IsActive = true
                            });
                            //جواز السفر
                            lst.Add(new MyDataTable
                            {
                                IdNumber = (string)dataRow[0].ToString(),
                                DocumentId = 2,
                                IdentityNum = (string)dataRow[5].ToString(),
                                IssueDate = DateTime.ParseExact(dataRow[7].ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US")),
                                ExpDate = DateTime.ParseExact(dataRow[7].ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US")),
                                IsActive = true
                            });
                        }

                        var dt = lst.CopyToDataTable();
                        var connectionString =
                            ConfigurationManager.ConnectionStrings["HRMadinaEntities"].ConnectionString;
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
                                CommandText = "usp_UpdateDateFromExcel"
                            };
                            cmd.Parameters.AddWithValue("@ResidentItems", dt);
                            var count = cmd.ExecuteNonQuery();
                            sqlConnection.Close();
                            if (count > 0)
                            {
                                e.CallbackData += $"تم اضافة وتحديث عدد  {count} طالب";
                                e.ErrorText += "";
                            }
                            else
                            {
                                e.CallbackData += "";
                                e.ErrorText += "عفواً، برجاء اختيار ملف يحتوى على بيانات صحيحة";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    e.ErrorText += "حدث خطأ أثناء الرفع، برجاء تصحيح الأخطاء المحاولة مرة أخرى" + ex.Message;
                    e.CallbackData += "";
                }
            }
        }

        #endregion    }


        #region ResidentViolations

        public ActionResult UploadResidentViolationsExcelFiles()
        {
            string[] errors;

            UploadedFile[] files = UploadControlExtension.GetUploadedFiles("ResidentViolationsFiles",
                MyUploadControlValidationSettings.Settings, out errors, (s, e) => { },
                UploadControl_ResidentViolationsFilesUploadComplete);

            return null;
        }

        public void UploadControl_ResidentViolationsFilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {
            var files = ((MVCxUploadControl)sender).UploadedFiles;

            foreach (var file in files)
            {
                try
                {
                    using (var reader = ExcelReaderFactory.CreateReader(file.FileContent))
                    {
                        var result = reader.AsDataSet();
                        var dataTable = result.Tables[0];
                        var lst = new List<ResidentViolationsDataTable>();
                        for (var index = 1; index < dataTable.Rows.Count; index++)
                        {
                            var dataRow = dataTable.Rows[index];
                            lst.Add(new ResidentViolationsDataTable
                            {
                                IdNumber = (string)dataRow[0],
                                Amount = decimal.Parse(dataRow[2].ToString())
                            });
                        }

                        var CurrentUser = HttpContext.Session["UserId"] as DashBoard_Users;
                        var dt = lst.CopyToDataTable();
                        var connectionString = ConfigurationManager.ConnectionStrings["SchoolAccGam3aEntities"]
                            .ConnectionString;
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
                                CommandText = "usp_UpdateResidentViolationsFromExcel"
                            };
                            cmd.Parameters.AddWithValue("@ResidentViolationItems", dt);
                            cmd.Parameters.AddWithValue("@USERID", CurrentUser.ID);
                            var count = cmd.ExecuteNonQuery();
                            sqlConnection.Close();
                            if (count > 0)
                            {
                                e.CallbackData += $"تم اضافة وتحديث عدد  {count} مخالفة";
                                e.ErrorText += "";
                            }
                            else
                            {
                                e.CallbackData += "";
                                e.ErrorText += "عفواً، برجاء اختيار ملف يحتوى على بيانات صحيحة";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    e.ErrorText += "حدث خطأ أثناء الرفع، برجاء تصحيح الأخطاء المحاولة مرة أخرى" + ex.Message;
                    e.CallbackData += "";
                }
            }
        }
        public JsonResult DeleteOldResidentViolations()
        {
            int result = 0;
            try
            {
                var list = db.ResidentViolations.ToList();
                result = list.Count();
                db.ResidentViolations.RemoveRange(list);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                result = 0;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ResidentRelatives

        public ActionResult UploadResidentRelativesExcelFiles()

        {
            string[] errors;

            UploadedFile[] files = UploadControlExtension.GetUploadedFiles("ResidentRelativesFiles",
                MyUploadControlValidationSettings.Settings, out errors, (s, e) => { },
                UploadControl_ResidentRelativesFilesUploadComplete);

            return null;
        }

        public void UploadControl_ResidentRelativesFilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {
            var files = ((MVCxUploadControl)sender).UploadedFiles;

            foreach (var file in files)
            {
                try
                {
                    using (var reader = ExcelReaderFactory.CreateReader(file.FileContent))
                    {
                        var result = reader.AsDataSet();
                        var dataTable = result.Tables[0];
                        var lst = new List<ResidentRelativesDataTable>();
                        for (var index = 8; index < dataTable.Rows.Count; index++)
                        {
                            var dataRow = dataTable.Rows[index];
                            lst.Add(new ResidentRelativesDataTable
                            {
                                RelativeIdNumber = (string)dataRow[0],
                                Name = (string)dataRow[1],
                                Relationship = (string)dataRow[2],
                                IdNumber = (string)dataRow[3],
                                Nationality = (string)dataRow[4],
                                Gender = (string)dataRow[5],
                                //BirthDate = dataRow[6].ToString() == "" ? null :DateTime.Parse(dataRow[6].ToString()) ,
                                BirthDate = null,
                                Job = (string)dataRow[7],
                            });
                        }

                        var dt = lst.CopyToDataTable();
                        var connectionString =
                            ConfigurationManager.ConnectionStrings["HRMadinaEntities"].ConnectionString;

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
                                CommandText = "usp_UpdateResidentRelativesDateFromExcel"
                            };
                            cmd.Parameters.AddWithValue("@ResidentRelativesItems", dt);
                            var count = cmd.ExecuteNonQuery();
                            sqlConnection.Close();
                            if (count > 0)
                            {
                                e.CallbackData += $"تم اضافة وتحديث عدد  {count} قريب";
                                e.ErrorText += "";
                            }
                            else
                            {
                                e.CallbackData += "";
                                e.ErrorText += "عفواً، برجاء اختيار ملف يحتوى على بيانات صحيحة";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    e.ErrorText += "حدث خطأ أثناء الرفع، برجاء تصحيح الأخطاء المحاولة مرة أخرى" + ex.Message;
                    e.CallbackData += "";
                }
            }
        }



        #endregion


        #region ERVisaRecords

        public ActionResult UploadERVisaRecordsExcelFiles()
        {
            string[] errors;

            UploadControlExtension.GetUploadedFiles("ERVisaRecordsFiles",
                MyUploadControlValidationSettings.Settings, out errors, (s, e) => { },
                UploadControl_ERVisaRecordsFilesUploadComplete);

            return null;
        }


        #region date Transforms Hijri,Gero
        private static DateTime HijriToGregorian(string hijri)
        {
            var arCi = new CultureInfo("ar-SA");
            var enCi = new CultureInfo("ar-EG");
            var hijriDate = DateTime.ParseExact(hijri, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return DateTime.Parse(hijriDate.ToString("dd/MM/yyyy", arCi));
        }


        private DateTime GregorianToHijri(DateTime? greg)
        {
            var arCi = new CultureInfo("ar-SA");
            var enCi = new CultureInfo("en-EG");
            var strDate = greg == null ? null : ((DateTime)greg).ToString("dd/MM/yyyy");
            var hijriDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", enCi.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

            return DateTime.Parse(hijriDate.ToString("dd/MM/yyyy", arCi));
        }
        #endregion
        public void UploadControl_ERVisaRecordsFilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {
            var files = ((MVCxUploadControl)sender).UploadedFiles;
            foreach (var file in files)
            {
                try
                {
                    var newCulture = new CultureInfo("en-US")
                    {
                        DateTimeFormat = { ShortDatePattern = "dd/MM/yyyy", DateSeparator = "/" }
                    };
                    Thread.CurrentThread.CurrentCulture = newCulture;

                    using (var reader = ExcelReaderFactory.CreateReader(file.FileContent))
                    {
                        var result = reader.AsDataSet();
                        var dataTable = result.Tables[0];
                        var lst = new List<ReservedVisa>();
                        DateTime? nulledDate = null;
                        for (var index = 1; index < dataTable.Rows.Count; index++)
                        {
                            var dataRow = dataTable.Rows[index];
                            ReservedVisa reservedVisa = new ReservedVisa();

                            reservedVisa.IdNumber = dataRow[0].ToString();
                            reservedVisa.Residence_ExpDate = string.IsNullOrEmpty(dataRow[1].ToString()) ? nulledDate : Convert.ToDateTime(dataRow[1].ToString());
                            reservedVisa.Visa_InitDate = string.IsNullOrEmpty(dataRow[2].ToString()) ? nulledDate : DateTime.Parse(dataRow[2].ToString());
                            reservedVisa.Visa_ExpDate = string.IsNullOrEmpty(dataRow[3].ToString()) ? nulledDate : DateTime.Parse(dataRow[3].ToString());//DateTime.ParseExact(dataRow[3].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            reservedVisa.InKsa = dataRow[4].ToString() == "لا"?false:true;
                            reservedVisa.Status = dataRow[5].ToString();
                            reservedVisa.VisaDays = dataRow[6].ToString();
                            reservedVisa.VisaNumber = dataRow[7].ToString();
                            lst.Add(reservedVisa);
                            //reservedVisa.Student_Name = dataRow[1].ToString();
                            //reservedVisa.Passport_Number = dataRow[2].ToString();
                            //reservedVisa.Religion = dataRow[3].ToString();
                            //reservedVisa.Nationality = dataRow[4].ToString();
                            ////reservedVisa.Job = dataRow[5].ToString();
                            //reservedVisa.Visa_Type = dataRow[8].ToString();
                            

                            //var existed = db.ReservedVisa.Where(x => x.IdNumber == reservedVisa.IdNumber).FirstOrDefault();
                            //if (existed != null)
                            //{
                            //    existed.InKsa = reservedVisa.InKsa;
                            //    if (existed.Residence_ExpDate != reservedVisa.Residence_ExpDate)
                            //        existed.Residence_ExpDate = reservedVisa.Residence_ExpDate;
                            //    if (existed.Visa_ExpDate != reservedVisa.Visa_ExpDate)
                            //        existed.Visa_ExpDate = reservedVisa.Visa_ExpDate;

                            //}
                            //else
                            //{
                            //    lst.Add(reservedVisa);
                            //}

                        }

                        if (lst.Count > 0)
                        {
                            var fieldsToBeDeleted = db.ReservedVisa.ToList();
                            db.ReservedVisa.RemoveRange(fieldsToBeDeleted);
                            db.ReservedVisa.AddRange(lst);
                            db.SaveChanges();
                            e.CallbackData += $"تم اضافة وتحديث عدد  {lst.Count} تأشيرة";
                            e.ErrorText += "";
                        }
                        else
                        {
                            e.CallbackData += "";
                            e.ErrorText += "عفواً، برجاء اختيار ملف يحتوى على بيانات صحيحة";
                        }
                    }
                }

                catch (Exception ex)
                {
                    e.ErrorText += "حدث خطأ أثناء الرفع، برجاء تصحيح الأخطاء المحاولة مرة أخرى" + ex.Message;
                    e.CallbackData += "";
                }
            }
        }

        public JsonResult DeleteOldReservedVisa()
        {
            int result = 0;
            try
            {
                var list = db.ReservedVisa.ToList();
                result = list.Count();
                db.ReservedVisa.RemoveRange(list);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                result = 0;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region RunwayRecords

        public ActionResult UploadRunwayRecordsExcelFiles()
        {
            string[] errors;

            UploadControlExtension.GetUploadedFiles("RunwayRecordsFiles",
                MyUploadControlValidationSettings.Settings, out errors, (s, e) => { },
                UploadControl_RunwayRecordsFilesUploadComplete);

            return null;
        }

        public void UploadControl_RunwayRecordsFilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {
            var files = ((MVCxUploadControl)sender).UploadedFiles;
            foreach (var file in files)
            {
                try
                {
                    var newCulture = new CultureInfo("en-US")
                    {
                        DateTimeFormat = { ShortDatePattern = "dd/MM/yyyy", DateSeparator = "/" }
                    };
                    Thread.CurrentThread.CurrentCulture = newCulture;

                    using (var reader = ExcelReaderFactory.CreateReader(file.FileContent))
                    {
                        var result = reader.AsDataSet();
                        var dataTable = result.Tables[0];
                        var lst = new List<RunawayAliens>();
                        DateTime? nulledDate = null;
                        for (var index = 1; index < dataTable.Rows.Count; index++)
                        {
                            var dataRow = dataTable.Rows[index];
                            RunawayAliens runawayAliens = new RunawayAliens();
                            runawayAliens.IdNumber =    dataRow[0].ToString();
                            runawayAliens.student_name = dataRow[1].ToString();
                            runawayAliens.nationality = dataRow[2].ToString();
                            //runawayAliens.IssuanceDate = string.IsNullOrEmpty(dataRow[3].ToString()) ? nulledDate : DateTime.ParseExact(dataRow[3].ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US"));
                            runawayAliens.EntryDate = DateTime.Now;// string.IsNullOrEmpty(dataRow[4].ToString()) ? nulledDate : DateTime.ParseExact(dataRow[4].ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US"));
                            runawayAliens.InKsa = dataRow[6].ToString() == "نعم" ? true : false;
                            lst.Add(runawayAliens);
                            //var existed = db.RunawayAliens.Where(x => x.IdNumber == runawayAliens.IdNumber).FirstOrDefault();
                            //if (existed != null)
                            //{

                            //    if (existed.IssuanceDate != runawayAliens.IssuanceDate)
                            //        existed.IssuanceDate = runawayAliens.IssuanceDate;
                            //    if (existed.EntryDate != runawayAliens.EntryDate)
                            //        existed.EntryDate = runawayAliens.EntryDate;
                            //}
                            //else
                            //{
                            //    lst.Add(runawayAliens);
                            //}

                        }

                        if (lst.Count > 0)
                        {
                            var AllRunAwayAliens = db.RunawayAliens.ToList();
                            db.RunawayAliens.RemoveRange(AllRunAwayAliens);
                            db.RunawayAliens.AddRange(lst);
                            db.SaveChanges();
                            e.CallbackData += $"تم اضافة وتحديث عدد  {lst.Count} تأشيرة";
                            e.ErrorText += "";
                        }
                        else
                        {
                            e.CallbackData += "";
                            e.ErrorText += "عفواً، برجاء اختيار ملف يحتوى على بيانات صحيحة";
                        }
                    }
                }

                catch (Exception ex)
                {
                    e.ErrorText += "حدث خطأ أثناء الرفع، برجاء تصحيح الأخطاء المحاولة مرة أخرى" + ex.Message;
                    e.CallbackData += "";
                }
            }
        }

        public JsonResult DeleteOldRunWayRecords()
        {
            int result = 0;
            try
            {
                var list = db.RunawayAliens.ToList();
                result = list.Count();
                db.RunawayAliens.RemoveRange(list);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                result = 0;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region الحقول المخصصة
        public ActionResult UploadCustomFiles(int? customFileID)
        {
            string[] errors;
            if (customFileID != null)
            {
                UploadControlExtension.GetUploadedFiles("ERcustomFiles",
                    MyUploadControlValidationSettings.Settings, out errors, (s, e) => { },
                    UploadControl_ERcustomFilesUploadComplete);
            }

            return null;
        }
        public void UploadControl_ERcustomFilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {
            var customField = HttpContext.Request.Params["customFileID_VI"];

            if (!string.IsNullOrEmpty(customField))
            {
                int CustomFieldID = int.Parse(customField);
                var files = ((MVCxUploadControl)sender).UploadedFiles;
                foreach (var file in files)
                {
                    try
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(file.FileContent))
                        {
                            var result = reader.AsDataSet();
                            var dataTable = result.Tables[0];
                            var lst = new List<StudentsCustomFields>();


                            for (var index = 1; index < dataTable.Rows.Count; index++)
                            {
                                var dataRow = dataTable.Rows[index];
                                var idNumber = dataRow[0].ToString();
                                var currentStudentRow = db.INTEGRATION_All_Students.Where(x => string.IsNullOrEmpty(idNumber) ||
                                    x.NATIONAL_ID == idNumber).OrderByDescending(x => x.JOIN_DATE).FirstOrDefault();
                                var stCustomField = new StudentsCustomFields
                                {
                                    NationalId = idNumber,
                                    CustomFieldId = CustomFieldID,
                                    Note = dataRow[1] != null ? dataRow[1].ToString() : "",
                                    StudentId = currentStudentRow == null ? "" : currentStudentRow.STUDENT_ID.ToString(),
                                };
                                var existed = db.StudentsCustomFields.Where(x => x.NationalId == stCustomField.NationalId && x.CustomFieldId == stCustomField.CustomFieldId).FirstOrDefault();
                                if (existed == null)
                                {
                                    lst.Add(stCustomField);
                                }
                            }

                            if (lst.Count > 0)
                            {
                                db.StudentsCustomFields.AddRange(lst);
                                db.SaveChanges();
                                e.CallbackData += $"تم اسناد الحقل المخصص لعدد {lst.Count}";
                                e.ErrorText += "";
                            }
                            else
                            {
                                e.CallbackData += "";
                                e.ErrorText = "عفوا برجاء اختيار ملف يحتوى على بيانات جديدة وصالحة";
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        e.ErrorText += "حدث خطأ أثناء الرفع، برجاء تصحيح الأخطاء المحاولة مرة أخرى";
                        e.CallbackData += "";
                    }
                }
            }
        }

        public JsonResult DeleteOldCustomFields(int? Id)
        {

            int result = 0;
            if (Id != null)
            {

                var fieldsToBeDeleted = db.StudentsCustomFields.Where(x => x.CustomFieldId == Id).ToList();
                result = fieldsToBeDeleted.Count();
                db.StudentsCustomFields.RemoveRange(fieldsToBeDeleted);
                db.SaveChanges();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ValidResidentsUpload
        public ActionResult UploadResidencyFiles()
        {
            string[] errors;

            UploadControlExtension.GetUploadedFiles("ErResidencyFiles",
                MyUploadControlValidationSettings.Settings, out errors, (s, e) => { },
                UploadControl_ERResidencyFilesUploadComplete);


            return null;
        }

        public void UploadControl_ERResidencyFilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {

            var DBTrans = db.Database.BeginTransaction();
            string x = "";
            string y = "";
            string z = "";
            string n = "";
            var files = ((MVCxUploadControl)sender).UploadedFiles;
            foreach (var file in files)
            {
                try
                {
                    using (var reader = ExcelReaderFactory.CreateReader(file.FileContent))
                    {
                        var result = reader.AsDataSet();
                        var dataTable = result.Tables[0];
                        var lst = new List<UniversityStudents>();
                        bool isUpdated = false;
                        int counter = 0;
                        string ErrorMessage = null;
                        string ErrorMessage2 = null;
                        bool isBreakError = false;
                        var CurrentUser = HttpContext.Session["UserId"] as DashBoard_Users;
                        Usp_CalcResidance_Result results = new Usp_CalcResidance_Result();
                        for (var index = 1; index < dataTable.Rows.Count; index++)
                        {
                           
                            var dataRow = dataTable.Rows[index];
                           
                            if (dataRow.IsNull(0)|| dataRow.IsNull(1) || dataRow.IsNull(2) || dataRow.IsNull(3))
                            {
                                var newIndex = index + 1;
                                if (ErrorMessage2 == null)
                                {

                                    ErrorMessage2 =  "يوجد بيانات غير كاملة في الصف رقم"+ "(" + newIndex + ")" ;
                                    isBreakError = true;
                                }
                                else
                                {
                                    ErrorMessage2 =   ErrorMessage2 + ","+ "(" + newIndex + ")";
                                    isBreakError = true;
                                }
                                continue;
                            }
                          
                            var IdNumberStr = dataRow[0].ToString();
                            var ResidenceExpDateG = Convert.ToDateTime(dataRow[3].ToString().Trim(),CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            var PassportNumber = dataRow[1].ToString();
                            var PassportExpDateG = Convert.ToDateTime(dataRow[2].ToString().Trim(), CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            x = ResidenceExpDateG;
                            y = PassportExpDateG;
                            z = IdNumberStr;
                            n = PassportNumber;
                            results = db.Usp_CalcResidance(IdNumberStr, PassportNumber,CurrentUser.ID, PassportExpDateG, ResidenceExpDateG).FirstOrDefault();
                            if (results.isUpdated == true)
                            {
                                isUpdated = true;
                                counter++;
                            }
                            if (!String.IsNullOrEmpty(results.ErrorMessage))
                            {
                                if (ErrorMessage == null)
                                {
                                    ErrorMessage = results.ErrorMessage;
                                }
                                else
                                {
                                    ErrorMessage = ErrorMessage + "," + results.ErrorMessage;
                                }
                            }
                             
                        }
                        if (!String.IsNullOrEmpty(ErrorMessage) && isUpdated == true)
                        {
                            if (!isBreakError) {

                                e.CallbackData += $"";
                                e.ErrorText = "لا توجد بيانات طلاب لأرقام الهوية هذه" + " " + ErrorMessage + " " + "ولكن تم تحديث عدد" + " " + counter + " " + "بنجاح";//" ولكن " + counter+ " "+"تم تحديث لبقية بيانات الطلاب بنجاح لعدد";

                            }
                            else
                            {
                                e.CallbackData += $"";
                                e.ErrorText = "لا توجد بيانات طلاب لأرقام الهوية هذه" + " " + ErrorMessage + " " + "ولكن تم تحديث عدد" + " " + counter + " " + "بنجاح" + " "+ "كما " + ErrorMessage2;//" ولكن " + counter+ " "+"تم تحديث لبقية بيانات الطلاب بنجاح لعدد";

                            }
                        }
                        else  if (String.IsNullOrEmpty(ErrorMessage) && isUpdated == true)
                        {
                            if (!isBreakError)
                            {

                                e.CallbackData += $"تم تحديث بيانات الطلاب بنجاح" + " " + " لعدد " + " " + counter;
                                e.ErrorText += "";

                            }
                            else
                            {

                                e.CallbackData += $"تم تحديث بيانات الطلاب بنجاح" + " " + " لعدد " + " " + counter +" "+ "كما  " + ErrorMessage2;
                                e.ErrorText += "";

                                //;
                            }
                        }
                        else if (!String.IsNullOrEmpty(ErrorMessage) && isUpdated != true )
                        {
                            if (!isBreakError)
                            {
                                e.CallbackData += $"";
                                e.ErrorText = ErrorMessage + " " + "لا توجد بيانات لهذه الأرقام";
                            }
                            else
                            {
                                e.CallbackData += $"";
                                e.ErrorText = ErrorMessage + " " + "لا توجد بيانات لهذه الأرقام"+" "+"كما "+ErrorMessage2;
                            }
                        }
                        else if (String.IsNullOrEmpty(ErrorMessage) && isUpdated != true)

                        {
                            if (!isBreakError)
                            {
                                e.CallbackData += $"تم تحديث بيانات الطلاب مسبقاً ولا يوجد تحديثات";
                                e.ErrorText = "";
                            }
                            else
                            {
                                e.CallbackData += $"تم تحديث بيانات الطلاب مسبقاً ولا يوجد تحديثات" + " " + "كما " + ErrorMessage2;
                                e.ErrorText = "";
                            }

                        }
                        DBTrans.Commit();
                    }
                }
                
                catch (Exception ex)
                {
                    e.ErrorText += "حدث خطأ أثناء الرفع، برجاء تصحيح الأخطاء المحاولة مرة أخرى";
                    e.CallbackData += "";
                    DBTrans.Rollback();
                }

            }
        }
        public JsonResult DeleteOldValidResidents()
        {

            int result = 0;
            try
            {

                var fieldsToBeDeleted = db.AllResidents.ToList();
                result = fieldsToBeDeleted.Count();
                db.AllResidents.RemoveRange(fieldsToBeDeleted);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = 0;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region FinalExitUpload
        public ActionResult UploadFinalExitFiles()
        {
            string[] errors;

            UploadControlExtension.GetUploadedFiles("ErFinalExitFiles",
                MyUploadControlValidationSettings.Settings, out errors, (s, e) => { },
                UploadControl_ERFinalExitFilesUploadComplete);


            return null;
        }

        public void UploadControl_ERFinalExitFilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {

            var files = ((MVCxUploadControl)sender).UploadedFiles;
            foreach (var file in files)
            {
                try
                {
                    using (var reader = ExcelReaderFactory.CreateReader(file.FileContent))
                    {
                        var result = reader.AsDataSet();
                        var dataTable = result.Tables[0];
                        var lst = new List<FinalExit>();


                        for (var index = 1; index < dataTable.Rows.Count; index++)
                        {
                            var dataRow = dataTable.Rows[index];
                            var idNumber = dataRow[0].ToString();

                            var resident = new FinalExit
                            {
                                IDNumber = idNumber.Trim(),
                                Name = dataRow[1].ToString().Trim(),

                                Nationality = dataRow[3].ToString().Trim(),
                                VisaExpDateG = dataRow[8].ToString().Trim(),

                                Status = dataRow[9]?.ToString().Trim()
                            };
                            lst.Add(resident);
                        }

                        if (lst.Count > 0)
                        {
                            var fieldsToBeDeleted = db.FinalExit.ToList();
                            db.FinalExit.RemoveRange(fieldsToBeDeleted);
                            db.FinalExit.AddRange(lst);
                            db.SaveChanges();
                            e.CallbackData += $"تم اسناد لعدد {lst.Count}";
                            e.ErrorText += "";
                        }
                        else
                        {
                            e.CallbackData += "";
                            e.ErrorText = "عفوا برجاء اختيار ملف يحتوى على بيانات جديدة وصالحة";
                        }

                    }
                }
                catch (Exception ex)
                {
                    e.ErrorText += "حدث خطأ أثناء الرفع، برجاء تصحيح الأخطاء المحاولة مرة أخرى";
                    e.CallbackData += "";
                }
            }
        }
        public JsonResult DeleteOldFinalExit()
        {

            int result = 0;
            try
            {

                var fieldsToBeDeleted = db.FinalExit.ToList();
                result = fieldsToBeDeleted.Count();
                db.FinalExit.RemoveRange(fieldsToBeDeleted);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = 0;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Report Designer

        public ActionResult ReportDesigner()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        #endregion

        #region نتائج الطلاب

        public ActionResult StudentsResults()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }


        public static List<SelectListItem> GetFaculties()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                return db.usp_getFaculties()
                    .Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).ToList();
            }
        }

        public static List<SelectListItem> GetNationalities()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                return db.usp_getNationalities()
                    .Select(x => new SelectListItem { Text = x.NATIONALITY_DESC, Value = x.NATIONALITY_CODE.ToString() })
                    .ToList();
            }
        }

        //المستوى - المرحلة العلمية
        public static List<SelectListItem> GetLevels()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                return db.usp_getLevels()
                    .Select(x => new SelectListItem { Text = x.LEVEL_DESC, Value = x.LEVEL_CODE.ToString() }).ToList();
            }
        }

        //الدرجة العلمية
        public static List<SelectListItem> GetDegres()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                return db.usp_getDegrees()
                    .Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).ToList();
            }
        }

        //الحالة
        public static List<SelectListItem> GetStatus()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                return db.usp_getStatus()
                    .Select(x => new SelectListItem { Text = x.STATUS_DESC, Value = x.STATUS_CODE.ToString() }).ToList();
            }
        }
        public static List<SelectListItem> GetCustomFiles()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                return db.CustomFields.Where(x => x.ParentId != null)
                    .Select(x => new SelectListItem { Text = x.Key, Value = x.CustomFieldId.ToString() }).ToList();
            }
        }

        //نوع الدراسة / صباحي او مسائي
        public static List<SelectListItem> GetStudyTypes()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                return db.usp_getStudyTypes()
                    .Select(x => new SelectListItem { Text = x.STUDY_DESC, Value = x.STUDY_CODE.ToString() }).ToList();
            }
        }

        //التخصصات
        public static List<SelectListItem> GetMajors()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                return db.usp_getMajors()
                    .Select(x => new SelectListItem { Text = x.MAJOR_NAME, Value = x.MAJOR_NO.ToString() }).ToList();
            }
        }
        //public static IEnumerable<Person> GetPersonsRange(ListEditItemsRequestedByFilterConditionEventArgs args) {
        //    var skip = args.BeginIndex;
        //    var take = args.EndIndex - args.BeginIndex + 1;
        //    var query = (from person in DB.Persons
        //            where (person.FirstName + " " + person.LastName + " " + person.Phone).StartsWith(args.Filter)
        //            orderby person.LastName
        //            select person
        //        ).Skip(skip).Take(take);
        //    return query.ToList();
        //}

        public static SelectListItem GetStudentById(ListEditItemRequestedByValueEventArgs args)
        {
            int id;
            if (args.Value == null || !int.TryParse(args.Value.ToString(), out id))
            {
                return null;
            }

            using (var db = new SchoolAccGam3aEntities())
            {
                var query = db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == id)
                    .Select(x => new SelectListItem { Value = x.STUDENT_ID.ToString(), Text = x.STUDENT_NAME }).Take(1)
                    .SingleOrDefault();
                return query;
            }
        }

        public static List<SelectListItem> GetStudentsName(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;

            using (var db = new SchoolAccGam3aEntities())
            {
                var query = db.INTEGRATION_All_Students.Where(x => x.STUDENT_NAME.StartsWith(args.Filter))
                    .OrderBy(x => x.STUDENT_NAME)
                    .Select(x => new SelectListItem { Value = x.STUDENT_ID.ToString(), Text = x.STUDENT_NAME })
                    .Skip(skip).Take(take).ToList();
                return query;
            }
        }


        public ActionResult StudentsResultsDocumentViewerPartial(int? faculty, int? nationality, int? degree,
            int? level, int? statusType, int? studyType, string studentName)
        {
            var report = new ResultsReport { RequestParameters = false };
            report.Parameters.Add(new Parameter
            {
                Name = "FacultyNo",
                Type = typeof(int?),
                Value = faculty
            });

            //report.Parameters["FacultyNo"].Type = typeof(int?);
            //report.Parameters["FacultyNo"].Visible = false;
            //report.Parameters["FacultyNo"].Value = faculty;

            return PartialView("_StudentsResultsDocumentViewerPartial", report);
        }

        #endregion


        #region طلاب الكليات

        //بيان بالطلاب المسافرين
        public ActionResult TravelStudents()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var permissions = GetPermissionsFn(123);
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

        public ActionResult TravelStudentReportDesignerPartial(int?[] nationality, int? studentId,string TripNumber,string AgentRefNumber)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var report = new StudentsTravel();
            List<RPTStudentTravelOrderVM> data = new List<RPTStudentTravelOrderVM>();
            if (nationality!=null)
            {
                data = db.StudentsTravelOrder.Where(a =>
                ( nationality.Any(p => p == db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == a.StudentID).NATIONALITY_CODE)) &&
                (studentId == null || a.StudentID == studentId) &&
                (TripNumber == null || a.TravelOrders.TripNumber == TripNumber) &&
                (AgentRefNumber == null || a.TravelOrders.AgentRefNumber == AgentRefNumber)

                ).Select(x => new RPTStudentTravelOrderVM
                {
                    FROM_DATE = x.FromDate,
                    END_DATE = x.ToDate,
                    TRIP_PATH = x.TravelOrders.TripPath,
                    STUDENT_ID = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).STUDENT_ID,
                    STUDENT_NAME = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).STUDENT_NAME,
                    STUDENT_NAME_S = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).STUDENT_NAME,
                    FACULTY_NAME = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).FACULTY_NAME,
                    LEVEL_DESC = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).LEVEL_DESC,
                    NATIONALITY_CODE = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).NATIONALITY_CODE,
                    NATIONALITY_DESC = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).NATIONALITY_DESC,
                    TripNumber = x.TravelOrders.TripNumber,
                    AgentRefNumber = x.TravelOrders.AgentRefNumber,
                    TicketNumber = x.TicketNumber,
                    RequestDate = x.StTravelRequest.InsertDate,
                    TicketNumberDate = x.StudentsTicketsDetails.Where(p => p.TicketNumber == x.TicketNumber).FirstOrDefault() != null ? x.StudentsTicketsDetails.Where(p => p.TicketNumber == x.TicketNumber).FirstOrDefault().InsertDate : null

                }).ToList();
            }
            else
            {
                 data = db.StudentsTravelOrder.Where(a =>
                (studentId == null || a.StudentID == studentId) &&
                (TripNumber == null || a.TravelOrders.TripNumber == TripNumber) &&
                (AgentRefNumber == null || a.TravelOrders.AgentRefNumber == AgentRefNumber)

                ).Select(x => new RPTStudentTravelOrderVM
                {
                    FROM_DATE = x.FromDate,
                    END_DATE = x.ToDate,
                    TRIP_PATH = x.TravelOrders.TripPath,
                    STUDENT_ID = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).STUDENT_ID,
                    STUDENT_NAME = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).STUDENT_NAME,
                    STUDENT_NAME_S = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).STUDENT_NAME,
                    FACULTY_NAME = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).FACULTY_NAME,
                    LEVEL_DESC = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).LEVEL_DESC,
                    NATIONALITY_CODE = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).NATIONALITY_CODE,
                    NATIONALITY_DESC = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).NATIONALITY_DESC,
                    TripNumber = x.TravelOrders.TripNumber,
                    AgentRefNumber = x.TravelOrders.AgentRefNumber,
                    TicketNumber = x.TicketNumber,
                    RequestDate = x.StTravelRequest.InsertDate,
                    TicketNumberDate = x.StudentsTicketsDetails.Where(p => p.TicketNumber == x.TicketNumber).FirstOrDefault() != null ? x.StudentsTicketsDetails.Where(p => p.TicketNumber == x.TicketNumber).FirstOrDefault().InsertDate : null

                }).ToList();
            }

            Session["TravelStudentRepor"] = data;

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = data;
            };

            return PartialView("_TravelStudentReportDesignerPartial", report);
        }

        public ActionResult TravelStudentReportCallback(int?[] nationality, int? studentId, string TripNumber, string AgentRefNumber)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var report = new StudentsTravel();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["TravelStudentRepor"];
            };

            return PartialView("_TravelStudentReportDesignerPartial", report);
        }

        public ActionResult TravelStudentReportExport(int?[] nationality, int? studentId, string TripNumber, string AgentRefNumber)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var report = new StudentsTravel();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["TravelStudentRepor"];
            };
            return ReportViewerExtension.ExportTo(report);
        }

        //public ActionResult TravelStudentReportDesignerPartial(int?[] nationalitiesCheckListBox, int? studentId)
        //{
        //    if (Session["UserId"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    //db.StTravelRequest//.Where(x => (nationalitiesCheckListBox.Count() == 0 || nationalitiesCheckListBox.Any(p => p.Value == x.CountryID)) && (studentId == null || x.StudentID == studentId))
        //    //.Select(x => new RptGetAllTravelStudentVM
        //    //{
        //    //    Departing = x.Departing,
        //    //    Returning = x.Returning.Value,
        //    //    StudentID = x.StudentID,
        //    //    NamePerIdentity_Ar = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).NamePerIdentity_Ar,
        //    //    NamePerIdentity_En = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).NamePerIdentity_En,
        //    //    Country = x.Country.CountryNameAr,
        //    //    Level = db.INTEGRATION_All_Students.FirstOrDefault(o => o.STUDENT_ID == x.StudentID).LEVEL_DESC,
        //    //    Faculty = db.INTEGRATION_All_Students.FirstOrDefault(o => o.STUDENT_ID == x.StudentID).FACULTY_NAME,
        //    //}).ToList();

        //    var report = new StudentsTravel();

        //    string Nationalities = "";
        //    if (nationalitiesCheckListBox != null)
        //    {
        //        foreach (var item in nationalitiesCheckListBox)
        //            Nationalities += item + ",";
        //    }
        //    else
        //        Nationalities = null;

        //    var RptData = db.RPT_TravelStudentReport(Nationalities, studentId)
        //        .Select(x => new
        //        {

        //            STUDENT_ID = db.INTEGRATION_All_Students.Where(a=>a.STUDENT_ID==) x.STUDENT_ID,
        //            STUDENT_NAME = x.STUDENT_NAME,
        //            STUDENT_NAME_S = x.STUDENT_NAME_S,

        //            FROM_DATE = x.FROM_DATE,
        //            END_DATE = x.END_DATE,
        //            LAST_UPDATE_DATE = x.LAST_UPDATE_DATE,

        //            FACULTY_NAME = x.FACULTY_NAME,
        //            LEVEL_DESC = x.LEVEL_DESC,
        //            NATIONALITY_CODE = x.NATIONALITY_CODE,
        //            NATIONALITY_DESC = x.NATIONALITY_DESC,
        //            REQUEST_SEQ = x.REQUEST_SEQ,
        //            TRAVEL_LINE_DESC = x.TRAVEL_LINE_DESC,
        //            TRAVEL_LINE_NO = x.TRAVEL_LINE_NO
        //        }).ToList();

        //    var TravelOrderStudent = db.INTEGRATION_All_Students.fi.Where(a => a.STUDENT_ID == studentId);

        //    var data = db.StudentsTravelOrder.Where(a=>a.StudentID==studentId).Select(x=> new {
        //        STUDENT_ID = TravelOrderStudent.st,
        //        STUDENT_NAME = x.STUDENT_NAME,
        //        STUDENT_NAME_S = x.STUDENT_NAME_S,
        //        x.FromDate,
        //        x.ToDate,
        //        x.TravelOrders.TripPath,
        //        FACULTY_NAME = db.INTEGRATION_All_Students.FirstOrDefault(c=>c.STUDENT_ID == x.StudentID).FACULTY_NAME,
        //        LEVEL_DESC = db.INTEGRATION_All_Students.FirstOrDefault(c=>c.STUDENT_ID == x.StudentID).LEVEL_DESC,

        //    })


        //    report.DataSourceDemanded += (s, e) =>
        //    {
        //        ((XtraReport)s).DataSource = RptData;
        //    };

        //    return PartialView("_TravelStudentReportDesignerPartial", report);
        //}

        //DocumentViewer
        public ActionResult TravelStudentsDocumentViewerPartial(int?[] nationalitiesCheckListBox, int? studentId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            //var report = new StudentsTravel { xrTableOfContents1 = { Visible = false } };
            //report.DataSourceDemanded += (s, e) =>
            //{
            //    ((XtraReport)s).DataSource = db.StTravelRequest.Where(x => (nationalitiesCheckListBox.Count() == 0 || nationalitiesCheckListBox.Any(p => p.Value == x.CountryID)) && (studentId == null || x.StudentID == studentId)).ToList()
            //    .Select(x => new RptGetAllTravelStudentVM
            //    {
            //        Departing = x.Departing,
            //        Returning = x.Returning.Value,
            //        StudentID = x.StudentID,
            //        NamePerIdentity_Ar = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).NamePerIdentity_Ar,
            //        NamePerIdentity_En = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).NamePerIdentity_En,
            //        Country = x.Country.CountryNameAr,
            //        Level = db.INTEGRATION_All_Students.FirstOrDefault(o => o.STUDENT_ID == x.StudentID).LEVEL_DESC,
            //        Faculty = db.INTEGRATION_All_Students.FirstOrDefault(o => o.STUDENT_ID == x.StudentID).FACULTY_NAME,
            //    }).ToList();

            //};
            var report = new StudentsTravel();

            string Nationalities = "";
            foreach (var item in nationalitiesCheckListBox)
                Nationalities += item + ",";

            var RptData = db.RPT_TravelStudentReport(Nationalities, studentId).ToList();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = RptData;
            };


            return PartialView("_TravelStudentsDocumentViewerPartial", report);
        }

        public ActionResult TravelStudentsDocumentViewerPartialExport(int?[] nationalitiesCheckListBox, int? studentId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
             
            var report = new StudentsTravel();

            string Nationalities = "";
            foreach (var item in nationalitiesCheckListBox)
                Nationalities += item + ",";

            var RptData = db.RPT_TravelStudentReport(Nationalities, studentId).ToList();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = RptData;
            };

            return DocumentViewerExtension.ExportTo(report, Request);
        }


        //بيان تفصيلى
        public ActionResult TravelStudentDetails()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var permissions = GetPermissionsFn(124);
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

        public ActionResult TravelStudentDetailsReportDesignerPartial(int?[] nationalitiesCheckListBox, int? studentId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string Nationalities = nationalitiesCheckListBox.Count()!=0? string.Join(",", nationalitiesCheckListBox):null;
            var report = new InstituteTravelStudentsDetailsReport();

            var RptData =/* db.RPT_TravelStudentDetailsReport(Nationalities, studentId,null)
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
                           }).ToList();*/
                 db.StudentsTravelOrder
            .Where(a =>
                     (nationalitiesCheckListBox.Count() == 0 || nationalitiesCheckListBox.Any(p => p == db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == a.StudentID).NATIONALITY_CODE)) &&
                     (studentId == null || a.StudentID == studentId)
             )
                 .Select(x => new
                 {
                     FROM_DATE = x.FromDate,
                     END_DATE = x.ToDate,
                     TripNumber = x.TravelOrders.TripNumber,
                     TRAVEL_LINE_DESC = x.TravelOrders.TripPath,
                     STUDENT_ID = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).STUDENT_ID,
                     STUDENT_NAME = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).STUDENT_NAME,
                     FACULTY_NO = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).FACULTY_NO,
                     FACULTY_NAME = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).FACULTY_NAME,
                     NATIONALITY_CODE = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).NATIONALITY_CODE,
                     NATIONALITY_DESC = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).NATIONALITY_DESC,
                     DEGREE_CODE = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).DEGREE_CODE,
                     DEGREE_DESC = db.INTEGRATION_All_Students.FirstOrDefault(a => a.STUDENT_ID == x.StudentID).DEGREE_DESC,
                     MOBILE_NO = db.UniversityStudents.FirstOrDefault(a => a.Student_ID == x.StudentID).MobileNumber,
                     EMAIL = db.UniversityStudents.FirstOrDefault(a => a.Student_ID == x.StudentID).UniversityEmail,
                     TRAVEL_LINE_NO = x.StTravelRequest.TransportationTracking.Tracking,


                 }).ToList();

            Session["TravelStudentDetailsReport"] = RptData;
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = RptData;
            };

            return PartialView("_TravelStudentDetailsReportDesignerPartial", report);
        }


        public ActionResult TravelStudentDetailsReportCallBack()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new InstituteTravelStudentsDetailsReport();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["TravelStudentDetailsReport"];
            };

            return PartialView("_TravelStudentDetailsReportDesignerPartial", report);
        }



        public ActionResult TravelStudentDetailsReportExport(int?[] nationalitiesCheckListBox, int? studentId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new InstituteTravelStudentsDetailsReport();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["TravelStudentDetailsReport"];
            };


            return DocumentViewerExtension.ExportTo(report, Request);
        }

        #endregion

        #region طلاب المعاهد

        public ActionResult InstituteTravelStudents()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        public ActionResult InstituteTravelStudentsReportDesignerPartial(int?[] nationalitiesCheckListBox,
            int? studentId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new InstituteTravelStudentsRPT { xrTableOfContents1 = { Visible = false } };

            var ds = report.DataSource as SqlDataSource;
            var query = ds?.Queries[0] as CustomSqlQuery;

            if (nationalitiesCheckListBox == null && studentId == null)
            {
                return PartialView("_InstituteTravelStudentsReportDesignerPartial", report);
            }

            if (nationalitiesCheckListBox == null || nationalitiesCheckListBox.Length >= 2)
            {
                report.xrTableOfContents1.Visible = true;
            }

            if (nationalitiesCheckListBox != null && nationalitiesCheckListBox.Length >= 1)
            {
                var builder = new StringBuilder();
                builder.Append('(');
                for (var i = 0; i < nationalitiesCheckListBox.Length; i++)
                {
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    builder.Append(nationalitiesCheckListBox[i]);
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    if (i != nationalitiesCheckListBox.Length - 1)
                    {
                        builder.Append(',');
                    }
                }

                builder.Append(')');
                if (query != null)
                {
                    query.Sql += $" AND [ESOL_STUDENT_TRAVE_INST].[nationality_curr] IN {builder.ToString()}";
                }
            }

            if (studentId != null)
            {
                if (query != null)
                {
                    query.Sql += $" AND [ESOL_STUDENT_TRAVE_INST].[stud_code] = {studentId}";
                }
            }

            ds?.RebuildResultSchema();

            return PartialView("_InstituteTravelStudentsReportDesignerPartial", report);
        }

        public ActionResult InstituteTravelStudentsDetails()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        public ActionResult InstituteTravelStudentsDetailsReportDesignerPartial(int?[] nationalitiesCheckListBox,
            int? studentId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new InstituteTravelStudentsDetailsReport();
            var ds = report.DataSource as SqlDataSource;
            var query = ds?.Queries[0] as CustomSqlQuery;

            if (nationalitiesCheckListBox == null && studentId == null)
            {
                return PartialView("_InstituteTravelStudentsDetailsReportDesignerPartial", report);
            }

            if (nationalitiesCheckListBox != null && nationalitiesCheckListBox.Length >= 1)
            {
                var builder = new StringBuilder();
                builder.Append('(');
                for (var i = 0; i < nationalitiesCheckListBox.Length; i++)
                {
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    builder.Append(nationalitiesCheckListBox[i]);
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    if (i != nationalitiesCheckListBox.Length - 1)
                    {
                        builder.Append(',');
                    }
                }

                builder.Append(')');
                if (query != null)
                {
                    query.Sql += $" AND ESOL_STUDENT_TRAVE_INST].[nationality_curr] IN {builder.ToString()}";
                }
            }

            if (studentId != null)
            {
                if (query != null)
                {
                    query.Sql += $" AND [ESOL_STUDENT_TRAVE_INST].[stud_code] = {studentId}";
                }
            }


            ds?.RebuildResultSchema();

            return PartialView("_InstituteTravelStudentsDetailsReportDesignerPartial", report);
        }

        #endregion


        #region الطلاب المسلمين

        //الطلاب المسلمين
        public ActionResult CountryPopulation()
        {
           
            var permissions = GetPermissionsFn(118);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;
            if (permissions.View|| permissions.Read)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }

        public ActionResult CountryPopulationReportDesignerPartial(int?[] nationalitiesCheckListBox, int? statusType)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new CountryPopulation();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = db.PR_GetAllCountriesPopulation().Where(x => (nationalitiesCheckListBox.Count() == 0 || nationalitiesCheckListBox.Any(p => p.Value == x.CountryCode)) && (statusType == null || x.STATUS_CODE == statusType)).ToList();
            };
            //var ds = report.DataSource as SqlDataSource;
            //var query = ds?.Queries[0] as CustomSqlQuery;
            //if (nationalitiesCheckListBox != null && nationalitiesCheckListBox.Length >= 1)
            //{
            //    var builder = new StringBuilder();
            //    builder.Append('(');
            //    for (var i = 0; i < nationalitiesCheckListBox.Length; i++)
            //    {
            //        //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
            //        builder.Append(nationalitiesCheckListBox[i]);
            //        //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
            //        if (i != nationalitiesCheckListBox.Length - 1)
            //        {
            //            builder.Append(',');
            //        }
            //    }

            //    builder.Append(')');
            //    if (query != null)
            //    {
            //        query.Sql += $" HAVING cp.CountryCode IN {builder.ToString()}";
            //    }
            //}

            //if (statusType != null)
            //{
            //    if (query != null)
            //    {
            //        if (nationalitiesCheckListBox != null && nationalitiesCheckListBox.Length == 0)
            //        {
            //            query.Sql += $" HAVING vin.STATUS_CODE = {statusType}";
            //        }
            //        else
            //        {
            //            query.Sql += $"AND vin.STATUS_CODE = {statusType}";
            //        }
            //    }
            //}


            return PartialView("_CountryPopulationReportDesignerPartial", report);
        }

        //DocumentViewer
        public ActionResult CountryPopulationDocumentViewerPartial(int?[] nationalitiesCheckListBox, int? statusType)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new CountryPopulation();
            var ds = report.DataSource as SqlDataSource;
            var query = ds?.Queries[0] as CustomSqlQuery;
            if (nationalitiesCheckListBox != null && nationalitiesCheckListBox.Length >= 1)
            {
                var builder = new StringBuilder();
                builder.Append('(');
                for (var i = 0; i < nationalitiesCheckListBox.Length; i++)
                {
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    builder.Append(nationalitiesCheckListBox[i]);
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    if (i != nationalitiesCheckListBox.Length - 1)
                    {
                        builder.Append(',');
                    }
                }

                builder.Append(')');
                if (query != null)
                {
                    query.Sql += $" HAVING cp.CountryCode IN {builder.ToString()}";
                }
            }


            if (statusType != null)
            {
                if (query != null)
                {
                    if (nationalitiesCheckListBox == null)
                    {
                        query.Sql += $" HAVING vin.STATUS_CODE = {statusType}";
                    }
                    else
                    {
                        query.Sql += $"AND vin.STATUS_CODE = {statusType}";
                    }
                }
            }

            ds?.RebuildResultSchema();

            return PartialView("_CountryPopulationDocumentViewerPartial", report);
        }

        public ActionResult CountryPopulationDocumentViewerPartialExport(int?[] nationalitiesCheckListBox,
            int? statusType)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new CountryPopulation();
            var ds = report.DataSource as SqlDataSource;
            var query = ds?.Queries[0] as CustomSqlQuery;
            if (nationalitiesCheckListBox != null && nationalitiesCheckListBox.Length >= 1)
            {
                var builder = new StringBuilder();
                builder.Append('(');
                for (var i = 0; i < nationalitiesCheckListBox.Length; i++)
                {
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    builder.Append(nationalitiesCheckListBox[i]);
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    if (i != nationalitiesCheckListBox.Length - 1)
                    {
                        builder.Append(',');
                    }
                }

                builder.Append(')');
                if (query != null)
                {
                    query.Sql += $" HAVING cp.CountryCode IN {builder.ToString()}";
                }
            }

            if (query != null)
            {
                if (nationalitiesCheckListBox == null)
                {
                    query.Sql += $" HAVING vin.STATUS_CODE = {statusType}";
                }
                else
                {
                    query.Sql += $"AND vin.STATUS_CODE = {statusType}";
                }
            }

            ds?.RebuildResultSchema();
            return DocumentViewerExtension.ExportTo(report, Request);
        }

        #endregion


        #region السلف

        public ActionResult Advances()
        {
            //تقرير مفصل بسلف الطلاب
            var json = new JavaScriptSerializer().Serialize(GetPermissions(32).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            if (permissions.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            return View();
        }

        public ActionResult AdvancesAggregate()
        {
            //تقرير مجمع بسلف الطلاب
            var json = new JavaScriptSerializer().Serialize(GetPermissions(33).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            if (permissions.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }
            return View();
        }


        public ActionResult SubsidiesDetailed()
        {
            //تقرير مفصل باعانات الطلاب
            var json = new JavaScriptSerializer().Serialize(GetPermissions(34).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            if (permissions.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            TempData["SubsidTypes"] = db.AdvanceSettings.Where(x => x.AdvanceType == "S").Select(x =>
                new SelectListItem
                {
                    Text = x.AdvanceSettingName.ToString(),
                    Value = x.AdvanceSettingId.ToString()
                }).ToList();

            return View();
        }

        public ActionResult AdvancesReportPartial(int?[] nationalitiesCheckListBox, int? remainderGreaterThan)
        {
            //تقرير مفصل بسلف الطلاب
            var json = new JavaScriptSerializer().Serialize(GetPermissions(32).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            if (permissions.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new AdvancesRPT();
            var ds = report.DataSource as SqlDataSource;
            var query = ds?.Queries[0] as CustomSqlQuery;
            if (nationalitiesCheckListBox != null && nationalitiesCheckListBox.Length >= 1)
            {
                var builder = new StringBuilder();
                builder.Append('(');
                for (var i = 0; i < nationalitiesCheckListBox.Length; i++)
                {
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    builder.Append(nationalitiesCheckListBox[i]);
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    if (i != nationalitiesCheckListBox.Length - 1)
                    {
                        builder.Append(',');
                    }
                }

                builder.Append(')');
                if (query != null)
                {
                    query.Sql += $" WHERE Employees.NationaltyID IN {builder.ToString()}";
                }
            }

            if (remainderGreaterThan != null)
            {
                if (query != null)
                {
                    query.Sql += $"AND Advances.AdvanceValue-AdvancesPremiums.NetValue>{remainderGreaterThan}";
                }
            }

            ds?.RebuildResultSchema();
            return PartialView("_AdvancesReportPartial", report);
        }

        public ActionResult SubsidiesDetailedReportPartial(DateTime? fromdate, DateTime? toDate, int? subsidyType)
        {
            //تقرير مفصل باعانات الطلاب
            var json = new JavaScriptSerializer().Serialize(GetPermissions(34).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            if (permissions.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}

            var report = new SubsidiesDetailedRPT();
            var ds = report.DataSource as SqlDataSource;
            var query = ds?.Queries[0] as CustomSqlQuery;
            //if (nationalitiesCheckListBox != null && nationalitiesCheckListBox.Length >= 1)
            //{
            //    var builder = new StringBuilder();
            //    builder.Append('(');
            //    for (var i = 0; i < nationalitiesCheckListBox.Length; i++)
            //    {
            //        //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
            //        builder.Append(nationalitiesCheckListBox[i]);
            //        //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
            //        if (i != nationalitiesCheckListBox.Length - 1)
            //        {
            //            builder.Append(',');
            //        }
            //    }

            //    builder.Append(')');
            //    if (query != null)
            //    {
            //        query.Sql += $" WHERE Employees.NationaltyID IN {builder.ToString()}";
            //    }
            //}

            if (subsidyType != null)
            {
                if (query != null)
                {
                    query.Sql += $"AND AdvanceRequest.AdvanceSettingId = {subsidyType}";
                }
            }

            if (fromdate != null)
            {
                if (query != null)
                {
                    query.Sql += $"AND AdvanceRequest.[Date] > '{fromdate.Value:MM.dd.yyyy}'";
                }
            }

            if (toDate != null)
            {
                if (query != null)
                {
                    query.Sql += $"AND AdvanceRequest.[Date] < '{toDate.Value:MM.dd.yyyy}'";
                }
            }

            ds?.RebuildResultSchema();
            return PartialView("_SubsidiesDetailedReportPartial", report);
        }

        public ActionResult AdvancesAggregateReportPartial(int?[] nationalitiesCheckListBox, int? remainderGreaterThan)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new AdvancesAggregateRPT();
            var ds = report.DataSource as SqlDataSource;
            var query = ds?.Queries[0] as CustomSqlQuery;
            if (nationalitiesCheckListBox != null && nationalitiesCheckListBox.Length >= 1)
            {
                var builder = new StringBuilder();
                builder.Append('(');
                for (var i = 0; i < nationalitiesCheckListBox.Length; i++)
                {
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    builder.Append(nationalitiesCheckListBox[i]);
                    //builder.Append('\''); // Uncomment this line when parsing a string parameter value.
                    if (i != nationalitiesCheckListBox.Length - 1)
                    {
                        builder.Append(',');
                    }
                }

                builder.Append(')');
                if (query != null)
                {
                    query.Sql += $" HAVING Employees.NationaltyID IN {builder.ToString()}";
                }
            }

            if (remainderGreaterThan != null)
            {
                if (query != null)
                {
                    query.Sql +=
                        $"AND (SUM(Advances.AdvanceValue)-SUM(AdvancesPremiums.NetValue))>{remainderGreaterThan}";
                }
            }

            ds?.RebuildResultSchema();
            return PartialView("_AdvancesAggregateReportPartial", report);
        }

        #endregion

        #region تقارير الاقامات الصالحةو الخروج النهائي
        public ActionResult AvailableTerminated()
        {
            var permissions = GetPermissionsFn(119);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;
            if (permissions.View || permissions.Read)
            {
                var SysTypeList = new List<SelectListItem>();
                SysTypeList.Add(new SelectListItem { Value = "1", Text = "إقامة صالحة و غير موجودين" });
                SysTypeList.Add(new SelectListItem { Value = "0", Text = "خروج نهائي مع إقامة صالحة" });
                SysTypeList.Add(new SelectListItem { Value = "2", Text = "خرج ولم يعد" });
                SysTypeList.Add(new SelectListItem { Value = "3", Text = "منتظمين بدون إقامة صالحة" });

                ViewBag.Types = SysTypeList;
                ViewBag.Nationalities = db.Nationality.Select(x =>
                new SelectListItem { Value = x.ID.ToString(), Text = x.NationalityName.Trim() }).Distinct().ToList();
                return View(); 
            }
            return RedirectToAction("NotAuthorized", "Security");
            
        }
        public ActionResult _AvailableTerminated(string nationality, int? degree, int? level, int? StatusType, int? restDays, string type,bool? InKsa,string IsNew=null)
        {
            db.Database.CommandTimeout = 0;
            if (string.IsNullOrEmpty(nationality))
                nationality = null;
            if (type == "0")
            {
                var Obj = db.GetResidentsFinalExit(nationality, StatusType, degree, level, restDays, type).ToList();
                return PartialView("_GetResidentsFinalExit", Obj);
            }
            if (type == "4")
            {
                var Obj = db.CheckResidanceValidation(nationality, StatusType, degree, level, restDays, type).ToList();
                return PartialView("_CheckResidanceValidation", Obj);
            }
            if (type == "5")
            {
                var Obj = db.CheckPassportValidation(nationality, StatusType, degree, level, restDays, type).ToList();
                return PartialView("_CheckPassportValidation", Obj);
            }
            if (type == "6")
            {
                var Obj = db.CheckResidanceViolations(nationality, StatusType, degree, level, restDays, type).ToList();
                return PartialView("_ResidentViolations", Obj);
            }
            if (IsNew == "")
                IsNew = null;
            if (type == "8")
            {
                var Obj = db.GetResidentsRunAway(nationality, InKsa).ToList();
                return PartialView("_ResidentsRunAway", Obj);
            }
            else
            {
                var model = db.GetResidentsValidOrNot(nationality, StatusType, degree, level, restDays, type, InKsa, IsNew).ToList();
                return PartialView("_AvailableTerminated", model);
            }
        }
        #endregion

        #region تقرير الحقول المخصصة
        public ActionResult CustomFieldsReport()
        {
            var permissions = GetPermissionsFn(120);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;
            if (permissions.View || permissions.Read)
            {
                return View();
            }
            return RedirectToAction("NotAuthorized", "Security");
        }
        public ActionResult _CustomFieldsReport(int[] fieldType, decimal? StudentNum, string IdentityNum)
        {
            string customFields = null;
            if (fieldType != null && fieldType.Count() > 0)
            {
                customFields = string.Join(",", fieldType);
            }
            var model = db.GetStudentsByCustomFields(customFields, StudentNum, IdentityNum).ToList();
            return PartialView("_CustomFieldsReport", model);
        }
        public ActionResult DeleteCustomFields(int[] toBeDeleted, int[] fieldType, decimal? StudentNum, string IdentityNum)
        {
            var customField = db.StudentsCustomFields.Where(x => toBeDeleted.Any(p => p == x.StudentCustomFieldId)).ToList();
            if (customField != null && customField.Count() > 0)
            {
                db.StudentsCustomFields.RemoveRange(customField);
                db.SaveChanges();
            }
            string customFields = null;
            if (fieldType != null && fieldType.Count() > 0)
            {
                customFields = string.Join(",", fieldType);
            }
            var model = db.GetStudentsByCustomFields(customFields, StudentNum, IdentityNum).ToList();
            return PartialView("_CustomFieldsReport", model);
        }
        #endregion


        #region Advertisement Reports

        public ActionResult AdvertisementStudentsReport()
        {
            var json = new JavaScriptSerializer().Serialize(GetPermissions(45).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            ViewBag.View = permissions.View;

            if (permissions.View)
            {
                return View();
            }
            return RedirectToAction("NoPermissions", "Security");
        }

        //For DDl
        public ActionResult GetAllAdvertisement()
        {
            var Advertisements = db.TravelAdvertisement.Where(x => x.IsActive == true).Select(x => new SelectListItem { Text = x.AdName, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(Advertisements, JsonRequestBehavior.AllowGet);
        }

        //For Grid
        public ActionResult GetAllStudentsByAdvertisement(int id)
        {
            if (id > 0)
            {
                var Students = db.StTravelRequest.Where(x => x.AdvertisementID == id).Select(x => new
                {
                    StudentID = x.StudentID,
                    STATUS_DESC = db.INTEGRATION_All_Students.FirstOrDefault(o => o.STUDENT_ID == x.StudentID).STATUS_DESC,
                    InsertDate=x.InsertDate.Hour +":"+x.InsertDate.Minute+" "+ x.InsertDate.Day+"/" + x.InsertDate.Month+"/"+ x.InsertDate.Year,
                    IdentityName = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).NamePerIdentity_Ar,
                    PassportName = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).NamePerPassport_Ar,
                    National_ID = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).National_ID,
                    Nationality = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).Nationality.NationalityName,
                    MobileNumber = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).MobileNumber,
                    UniversityEmail = db.UniversityStudents.FirstOrDefault(o => o.Student_ID == x.StudentID).UniversityEmail,
                    TicketNumber = x.StudentsTravelOrder.FirstOrDefault(o => o.StudentID == x.StudentID).TicketNumber,
                    Tracking=  x.TransportationTracking.Tracking,
                    REMAININGCREDITHOURSCOUNT = db.INTEGRATION_All_Students.FirstOrDefault(o => o.STUDENT_ID == x.StudentID).REMAININGCREDITHOURSCOUNT,

                }).ToList();
                return this.JsonMaxLength(Students, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(notify = new notify() { Message = "خطأ اثناء العرض", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region تقرير مخالفات الطالب السلوكية
        public ActionResult StudentNotesRPT()
        {
            ViewBag.Issues = db.Issues.Select(x => new { x.Id, x.IssueDescription }).ToList();
            var permissions = GetPermissionsFn(76);
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
        public ActionResult _StudentNotesRPT(List<int> issuesCheckListBox)
        {
            var report = new StudentNotesRpt();

            string Issues = null;
            if (issuesCheckListBox != null)
            {
                foreach (var item in issuesCheckListBox)
                    Issues += item + ",";
            }
            else
                Issues = null;

            var RptData = db.Usp_GetStudentsNotes(Issues).ToList();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = RptData;
            };

            return PartialView("_StudentNotesRPT", report);
        }
        #endregion

        #region تقارير السلف

        //تقرير تحصيل السلف
        public ActionResult AdvancesRecievingReportIndex()
        {
            ViewBag.AdvancesName = db.AdvanceSettings.Where(a => a.AdvanceType == "A").Select(x => new SelectListItem { Text = x.AdvanceSettingName, Value = x.AdvanceSettingId.ToString() }).Where(x => x.Text != null).ToList();
            ViewBag.DocNumbers = db.AdvanceReceiveMaster.Select(x => new SelectListItem { Text = x.DocNumber, Value = x.DocNumber.ToString() }).Where(x => x.Text != null).Distinct().OrderByDescending(x => x.Text).ToList();
            var permissions = GetPermissionsFn(125);
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

        public ActionResult AdvancesRecievingReport(DateTime? DateFrom, DateTime? DateTo, int? AdvancesNameComboBox, int? txtDocNumber, int? ReciveMethodComboBox)
        {
            var data = db.Usp_GetAdvancesRecieving(DateFrom, DateTo, AdvancesNameComboBox, txtDocNumber, ReciveMethodComboBox).Select(a => new
            {
                STUDENT_NAME = a.STUDENT_NAME,
                AdvanceTypeId = a.AdvanceTypeId,
                AdvanceSettingName = a.AdvanceSettingName,
                PayDate = a.PayDate,
                RecieveDate = a.RecieveDate,
                DocNumber = a.DocNumber,
                Student_ID = a.Student_ID,
                NATIONAL_ID = a.NATIONAL_ID,
                TotalAdvanceValue = a.TotalAdvanceValue,
                ReturnedValue = a.ReturnedValue,
                RemainingValue = a.RemainingValue,
                ReturnMethod = a.ReturnMethod,
                PayRollNumber = a.PayRollNumber
            }).ToList();


            Session["AdvancesRecieving"] = data;

            GetAdvancesRecievingRPT report = new GetAdvancesRecievingRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = data;
            };

            return PartialView("_AdvancesRecievingReportPartial", report);
        }

        public ActionResult AdvancesRecievingReportExport(DateTime? DateFrom, DateTime? DateTo, int? AdvancesNameComboBox, int? txtDocNumber, int? ReciveMethodComboBox)
        {
            GetAdvancesRecievingRPT report = new GetAdvancesRecievingRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["AdvancesRecieving"];
            };
            return ReportViewerExtension.ExportTo(report);
        }

        public ActionResult StudentSkillRecordRpt(DateTime? DateFrom, DateTime? DateTo, int? AdvancesNameComboBox, int? txtDocNumber, int? ReciveMethodComboBox)
        {
            GetAdvancesRecievingRPT report = new GetAdvancesRecievingRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                var a = Session["_EmpCoursesGridViewPartial"];
                ((XtraReport)s).DataSource = Session["_EmpCoursesGridViewPartial"];
            };

            return PartialView("_EmpCoursesGridViewPartial", report);
        }
        public ActionResult StudentSkillRecordRptExport(DateTime? DateFrom, DateTime? DateTo, int? AdvancesNameComboBox, int? txtDocNumber, int? ReciveMethodComboBox)
        {
           StudentSkillRecordRpt report = new StudentSkillRecordRpt();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["_EmpCoursesGridViewPartial"];
                ((XtraReport)s).Parameters["StudentID"].Value = Session["FACULTY_NO"];
                ((XtraReport)s).Parameters["StudentName"].Value = Session["StudentName"];
            };
            return ReportViewerExtension.ExportTo(report);
        }

        //تقرير بحالة الشيكات
        public ActionResult CheckDataReportIndex()
        {
            ViewBag.BenefName = db.DashBoard_Users.Where(m=>m.PayrollChecks.Count > 0).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).Distinct().OrderByDescending(x => x.Text).ToList();
            var payrolls = db.Payroll.Where(m=>m.PayrollChecks.Count > 0).ToList().Select(x => new SelectListItem { Text = x.IssueDate.ToString("dd-MM-yyyy") + "-" + "(" + x.PayrollNumber + ")", Value = x.ID.ToString() }).Distinct().OrderByDescending(x => x.Text).ToList();
            ViewBag.Payroll = payrolls.Distinct();
            var permissions = GetPermissionsFn(111);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            ViewBag.View = permissions.View;
            if (permissions.Read|| permissions.View)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult CheckDataReport(DateTime? DateFrom, DateTime? DateTo, int? PayrollID, int? UserID,string CheckNumber)
        {
            var data = db.Usp_GetCheckDataReport(PayrollID, UserID, CheckNumber, DateFrom, DateTo).ToList().Select(a => new ChecksDataVM
            {
                CheckNumber = a.CheckNumber,
                BenefName = a.name,
                ExportDate = a.ExportDate!=null? a.ExportDate.Value.ToString("dd/MM/yyyy"):"",
                CheckValue = a.CheckValue,
                IsActive = a.IsActive ==true?"نشط":"ملغي",
                IsReceived = a.IsReceived==true?"تم الإستلام":"غير مستلم",
                ReceiveDate = a.ReceiveDate!=null? a.ReceiveDate.Value.ToString("dd/MM/yyyy"):"",
                PayrollNumber = a.PayrollNumber,
                Description = a.Description,
                FileName= a.FileName,
                FilePath= a.FilePath,
                SandFileName= a.SandFileName,
                SandFilePath=a.SandFilePath
            }).ToList();


            Session["CheckDataReport"] = data;

            CheckDataRPT report = new CheckDataRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = data;
            };

            return PartialView("_CheckDataReportPartial", report);
        }

        public ActionResult CheckDataReportExport(DateTime? DateFrom, DateTime? DateTo, int? PayrollID, int? UserID, string CheckNumber)
        {
            CheckDataRPT report = new CheckDataRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["CheckDataReport"];
            };
            return ReportViewerExtension.ExportTo(report);
        }

        public PartialViewResult StudentsComboBoxPartial(string Text)
        {
            var students = db.INTEGRATION_All_Students.Where(x => x.STATUS_CODE == 1 && x.STUDENT_NAME.Contains(Text)
                            || x.STUDENT_ID.ToString() == Text || x.NATIONAL_ID == Text)
                                    .Select(x => new SelectListItem
                                    {
                                        Value = x.STUDENT_ID.ToString(),
                                        Text = x.STUDENT_NAME
                                    }).ToList();
            return PartialView("_StudentsComboBoxPartial", students);
        }

        //تقرير بإستلام المكافأت
        public ActionResult AdvanceDataReportIndex()
        {
            ViewBag.BenefName = db.DashBoard_Users.Where(m => m.StudentPayroll.Count > 0).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).Distinct().OrderByDescending(x => x.Text).ToList();
            var payrolls = db.Payroll.Where(m => m.StudentPayroll.Count > 0).ToList().Select(x => new SelectListItem { Text = x.IssueDate.ToString("dd-MM-yyyy") + "-" + "(" + x.PayrollNumber + ")", Value = x.ID.ToString() }).Distinct().OrderByDescending(x => x.Text).ToList();
            ViewBag.Payroll = payrolls.Distinct();
            //ViewBag.Student = (from s in db.StudentPayroll
            //                   join i in db.INTEGRATION_All_Students
            //                   on s.StudentID equals i.STUDENT_ID
            //                   select new SelectListItem
            //                   {
            //                       Text = i.STUDENT_NAME,
            //                       Value = i.STUDENT_ID.ToString()
            //                   }).Distinct().OrderByDescending(x => x.Text).ToList();
            //ViewBag.StdID = (from s in db.StudentPayroll
            //                   join i in db.INTEGRATION_All_Students
            //                   on s.StudentID equals i.STUDENT_ID
            //                   select new SelectListItem
            //                   {
            //                       Text = i.STUDENT_ID.ToString(),
            //                       Value = i.STUDENT_ID.ToString()
            //                   }).Distinct().OrderByDescending(x => x.Text).ToList();
            //ViewBag.NationalID = (from s in db.StudentPayroll
            //                 join i in db.INTEGRATION_All_Students
            //                 on s.StudentID equals i.STUDENT_ID
            //                 select new SelectListItem
            //                 {
            //                     Text = i.NATIONAL_ID.ToString(),
            //                     Value = i.STUDENT_ID.ToString()
            //                 }).Distinct().OrderByDescending(x => x.Text).ToList();
            ViewBag.Faculties = db.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Degrees = db.usp_getDegrees().Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList();

            var permissions = GetPermissionsFn(112);
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

          return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult AdvanceDataReport(decimal? [] facultiesCheckListBox, decimal?[] degreeCheckListBox,
            string StudentsComboBox,
            int? PayrollID, int? UserID)
        {
            string faculties = facultiesCheckListBox != null ? facultiesCheckListBox.Length > 0 ? string.Join(",", facultiesCheckListBox) : null : null;
            string degrees = degreeCheckListBox != null ? degreeCheckListBox.Length > 0 ? string.Join(",", degreeCheckListBox) : null : null;
            var data = db.Usp_GetStudentRwards(faculties, degrees, StudentsComboBox,PayrollID, UserID).ToList().Select(a => new StudentRewardsVM
            {
                student_id = a.student_id,
                PayrollNumber = a.PayrollNumber,
                STUDENT_NAME = a.STUDENT_NAME,
                DeliveryDate = a.DeliveryDate,
                IsPaid = a.IsPaid == true ? "نعم" : "لا",
                UserName = a.UserName,
                FACULTY_NAME = a.FACULTY_NAME,
                DEGREE_DESC = a.DEGREE_DESC,
                PayrollMoneyValue=a.PayrollMoneyValue,
                DeleviryPath = a.DeleviryPath,
                DelevirFileName=a.DelevirFileName

            }).ToList();


            Session["AdvanceDataReport"] = data;

            AdvanceDataRPT report = new AdvanceDataRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = data;
            };

            return PartialView("_AdvanceDataReportPartial", report);
        }

        public ActionResult AdvanceDataReportExport(decimal?[] facultiesCheckListBox, decimal?[] degreeCheckListBox, string StudentsComboBox,
            int? PayrollID, int? UserID)
        {
            AdvanceDataRPT report = new AdvanceDataRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["AdvanceDataReport"];
            };
            return ReportViewerExtension.ExportTo(report);
        }


        //معاينة الشيك قبل الطباعه
        public ActionResult CheckReportIndex(int CheckID)
        {
            Session["CheckID"] = CheckID;
            return View();
        }

        public ActionResult CheckReport(int CheckID)
        {
            var data = db.PayrollChecks.Include("DashBoard_Users").Where(m => m.ID == CheckID).ToList().Select(a => new
            {
                Name = a.DashBoard_Users.Name,
                CheckNumber = a.CheckNumber,
                CheckValue = a.CheckValue,
                Description = a.Description,
                Tafkeet = db.Tafkeet_Sp(Convert.ToDecimal(a.CheckValue)).SingleOrDefault()?.ToString(),
                ExportDate = a.ExportDate
            }).FirstOrDefault();
            Session["Check"] = data;

            CheckRPT report = new CheckRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = data;
            };

            return PartialView("_CheckReportPartial", report);
        }

        public ActionResult CheckReportExport(int CheckID)
        {
            CheckRPT report = new CheckRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["Check"];
            };
            return ReportViewerExtension.ExportTo(report);
        }
        //طباعة كل الشيكات للمسير
        public ActionResult AllChecksReportIndex(int payrollID)
        {
            Session["PayrollIDParam"] = payrollID;

            return View();
        }

        public ActionResult AllChecksReport(int payrollID)
        {
            var data = db.Usp_GetPayRollChecks(payrollID).Select(a => new Usp_GetPayRollChecks_Result
            {
                name = a.name,
                CheckNumber = a.CheckNumber,
                CheckValue = a.CheckValue,
                Description = a.Description,
                Tafkeet = db.Tafkeet_Sp(a.CheckValue).SingleOrDefault()?.ToString(),
                ExportDate = a.ExportDate
            }).ToList();
            Session["ChecksData"] = data;
            AllChecksRPT report = new AllChecksRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((AllChecksRPT)s).DataSource = data;
            };
            var rows = db.PayrollChecks.Where(m => m.PayrollID == payrollID && m.IsActive == true && m.IsReceived != true && m.FilePath != null).ToList();
            foreach (var item in rows)
            {
                var row = db.PayrollChecks.Find(item.ID);
                row.IsPrinted = true;
                db.Entry(row).State = EntityState.Modified;
            }
            db.SaveChanges();
            return PartialView("_AllChecksReportPartial", report);
        }

        public ActionResult AllChecksReportExport(int payrollID)
        {
            AllChecksRPT report = new AllChecksRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["ChecksData"];
            };
            return ReportViewerExtension.ExportTo(report);
        }


        //خطاب تسليم المالية شيكات مأموري الصرف
        public ActionResult SandCheckReportIndex(int payrollID)
        {
            Session["PayrollIDParam"] = payrollID;

            return View();
        }

        public ActionResult SandCheckReport(int payrollID)
        {
            var Payroll = db.Payroll.Find(payrollID);
            int PayrollNumber = Payroll.PayrollNumber;
            string PayNumber = Payroll.PayNo;
            string DafPayNumber = Payroll.DafPayNo + "-" + Payroll.DafPayNo2;
            var data=(
               db.Usp_GetPayRollsMoneyAfterIsBeingMonetary_Details(payrollID).ToList()
                .Select(x => new Usp_GetPayRollsMoneyAfterIsBeingMonetary_Details_Result
                {
                    PayrollNumber=x.PayrollNumber,
                    PayNumber=x.PayNumber,
                    DafPayNumber = x.DafPayNumber,
                    Name = x.Name,
                    TotalChecksValues = x.TotalChecksValues,
                    NoOfChecks = x.NoOfChecks
                }).ToList());
            Session["SandCheckData"] = data;
            SandCheckRPT report = new SandCheckRPT();

            report.DataSourceDemanded += (s, e) =>
            {
                ((SandCheckRPT)s).DataSource = data;
            };  
            return PartialView("_SandCheckReportPartial", report);
        }

        public ActionResult SandCheckReportExport(int payrollID)
        {
            var Payroll = db.Payroll.Find(payrollID);
            int PayrollNumber = Payroll.PayrollNumber;
            string PayNumber = Payroll.PayNo;
            SandCheckRPT report = new SandCheckRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["SandCheckData"];
            };
            return ReportViewerExtension.ExportTo(report);
        }


        //طباعة سندات  إستلام الشيكات
        public ActionResult SandEstlamCheckReportIndex(int payrollID)
        {
            Session["PayrollIDParam"] = payrollID;

            return View();
        }

        public ActionResult SandEstlamCheckReport(int payrollID)
        {
            var data = (
             db.Usp_GetCheckSandEstlamDetails(payrollID).ToList()
              .Select(x => new Usp_GetCheckSandEstlamDetails_Result
              {
                  id=x.id,
                  faculty_name = x.faculty_name,
                  DafPayNumber = x.DafPayNumber,
                  CheckNumber = x.CheckNumber,
                  ExportDate = x.ExportDate,
                  ReceiveDate = x.ReceiveDate,
                  UserName = x.UserName,
                  FilePath=x.FilePath,
                  PayrollNumber = x.PayrollNumber
              }).ToList());
            var ids = data.GroupBy(m => m.id).Select(p=>p.Key);
            List<Usp_GetCheckSandEstlamDetails_Result> list = new List<Usp_GetCheckSandEstlamDetails_Result>();
            foreach(var item in ids)
            {
                var recod = data.Where(m => m.id == item).ToList();
                var count = recod.Select(i => i.faculty_name).Count();
                Usp_GetCheckSandEstlamDetails_Result obj = new Usp_GetCheckSandEstlamDetails_Result();
                if (count > 1)
                {
                    var fac = "";
                    for (int i = 0; i < count; i++)
                    {
                        if (fac == "")
                        {

                            fac =  recod[i].faculty_name;
                        }
                        else
                        {

                            fac = fac + "-" + recod[i].faculty_name;
                        }
                    }
                    obj.faculty_name = fac;
                 
                }
                else
                {
                    obj.faculty_name = recod[0].faculty_name;

                }
                obj.DafPayNumber = recod[0].DafPayNumber;
                obj.CheckNumber = recod[0].CheckNumber;
                obj.ExportDate = recod[0].ExportDate;
                obj.ReceiveDate = recod[0].ReceiveDate;
                obj.UserName = recod[0].UserName;
                obj.FilePath = recod[0].FilePath;
                obj.PayrollNumber = recod[0].PayrollNumber;
                list.Add(obj);
            }
            Session["SandEstlamCheckData"] = list;
            SandEstlamCheckRPT report = new SandEstlamCheckRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((SandEstlamCheckRPT)s).DataSource = list;
            };
            return PartialView("_SandEstlamCheckReportPartial", report);
        }

        public ActionResult SandEstlamCheckReportExport(int payrollID)
        {
            SandEstlamCheckRPT report = new SandEstlamCheckRPT();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["SandEstlamCheckData"];
            };
            return ReportViewerExtension.ExportTo(report);
        }
        //كشف حساب البنود المصروفه
        public ActionResult GetAdvancesPaymentByUsersReportIndex()
        {
            ViewBag.DocNumbers = db.AdvanceReceiveMaster.Select(x => new SelectListItem { Text = x.DocNumber, Value = x.DocNumber.ToString() }).Where(x => x.Text != null).Distinct().OrderByDescending(x => x.Text).ToList();
            ViewBag.Users = db.AdvancePaymentMaster.Select(x => new SelectListItem { Text = x.DashBoard_Users.Name, Value = x.DashBoard_Users.ID.ToString() }).Distinct().ToList();
            var permissions = GetPermissionsFn(126);
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

        public ActionResult AdvancesPaymentByUsersCallBack()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var report = new AdvancesPaymentByUsersReport();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["AdvancesPaymentByUsers"];
            };

            return PartialView("_AdvancesPaymentByUsersPartial", report);
        }


        public ActionResult AdvancesPaymentByUsers(DateTime? DateFrom, DateTime? DateTo, int? txtDocNumber, string TypeComboBox, int[] UsersCheckListBox)
        {
            if (UsersCheckListBox == null)
                UsersCheckListBox = new int[] { };

            string UsersIds = UsersCheckListBox.Length != 0 ? string.Join(",", UsersCheckListBox) : null;

            decimal lastPayment = 0;
            if (DateFrom != null)
                lastPayment = db.AdvancePaymentDetails.AsEnumerable().Where(x => x.AdvancePaymentMaster.InsertionDate < DateFrom.Value).Sum(x => x.NetValue);

            var data = db.Usp_GetAdvancesPaymentByUsers(DateFrom, DateTo, txtDocNumber, TypeComboBox, UsersIds).Select(x => new
            {
                ResponsibleName = x.ResponsibleName,
                STUDENT_NAME = x.STUDENT_NAME,
                AdvanceTypeId = x.AdvanceTypeId,
                AdvanceSettingName = x.AdvanceSettingName,
                PaymentDate = x.PaymentDate,
                DocNumber = x.DocNumber,
                Student_ID = x.Student_ID,
                NATIONAL_ID = x.NATIONAL_ID,
                TotalAdvanceValue = x.TotalAdvanceValue,
                MonthlyValue = x.MonthlyValue
            }).ToList();


            Session["AdvancesPaymentByUsers"] = data;

            AdvancesPaymentByUsersReport report = new AdvancesPaymentByUsersReport();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = data;
                if (DateFrom != null)
                    ((XtraReport)s).Parameters["lastPayment"].Value = $"إجمالي ما تم صرفه قبل تاريخ {DateFrom.Value.ToShortDateString()} بقيمة  {lastPayment.ToString("N2")}";
                else
                    ((XtraReport)s).Parameters["lastPayment"].Value = "إجمالي ما تم صرفه بقيمة 0.00";
            };

            return PartialView("_AdvancesPaymentByUsersPartial", report);
        }

        public ActionResult AdvancesPaymentByUsersExport(DateTime? DateFrom, DateTime? DateTo, int? txtDocNumber, string TypeComboBox, string UsersCheckListBox)
        {
            AdvancesPaymentByUsersReport report = new AdvancesPaymentByUsersReport();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["AdvancesPaymentByUsers"];
            };

            return ReportViewerExtension.ExportTo(report);
        }

        // السلف التفصيلي
        public ActionResult GetAdvancesRecievingByStudentIndex_()
        {
            var students = db.AdvancePaymentMaster
               .Select(x => new SelectListItem { Text = db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).STUDENT_NAME, Value = x.Student_Id.ToString() }).Distinct().ToList();
            ViewBag.students = students;

            var advanceTypes = db.AdvanceSettings
                .Select(a => new SelectListItem { Text = a.AdvanceSettingName, Value = a.AdvanceSettingId.ToString() }).Distinct().ToList();

            ViewBag.advanceTypes = advanceTypes;
            var permissions = GetPermissionsFn(127);
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

        public ActionResult AdvancesRecievingByStudents_(int[] StudentIDs, int[] AdvanceTypeIDs, DateTime? AdvanceStartDate, DateTime? AdvanceEndDate,
            DateTime? ReturnStartDate, DateTime? ReturnEndDate)
        {
            string StudentIds = StudentIDs!=null? StudentIDs.Length != 0 ? string.Join(",", StudentIDs) : null : null;
            string AdvanceTypeIds = AdvanceTypeIDs!=null? AdvanceTypeIDs.Length != 0 ? string.Join(",", AdvanceTypeIDs) : null : null;


            var data = db.Usp_GetAdvancesRecievingByStudents(AdvanceStartDate, AdvanceEndDate, ReturnStartDate, ReturnStartDate, StudentIds, AdvanceTypeIds).Select(x => new
            {
                STUDENT_NAME = x.STUDENT_NAME,
                Student_ID = x.Student_ID,
                NATIONAL_ID = x.NATIONAL_ID,
                AdvanceTypeId = x.AdvanceTypeId,
                AdvanceSettingName = x.AdvanceSettingName,
                PaymentDate = x.PaymentDate,// != null ? x.PaymentDate.Value.Day + "/" + x.PaymentDate.Value.Month + "/" + x.PaymentDate.Value.Year : null,
                ReturningDate = x.ReturningDate,// != null ? x.ReturningDate.Value.Day + "/" + x.ReturningDate.Value.Month + "/" + x.ReturningDate.Value.Year : null,
                DocNumber = x.DocNumber,
                TotalAdvanceValue = x.TotalAdvanceValue,
                MonthlyValue = x.MonthlyValue,
                ReturnedValue = x.ReturnedValue,
                ReturnMethod = x.ReturnMethod,
                PayRollNumber = x.PayRollNumber,
                PayDetailsId = x.PayDetailsId,
                Number = x.Number,
                RemainingValue = x.RemainingValue

            }).ToList();

            // store report data
            Session["AdvancesRecievingByStudents_"] = data;

            AdvancesRecievingByStudentsReport report = new AdvancesRecievingByStudentsReport();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = data;
            };

            return PartialView("AdvancesRecievingByStudents_Partial", report);
        }

        public ActionResult AdvancesRecievingByStudentsExport(string StudentIDs, DateTime? AdvanceStartDate, DateTime? AdvanceEndDate,
            DateTime? ReturnStartDate, DateTime? ReturnEndDate
            )
        {
            AdvancesRecievingByStudentsReport report = new AdvancesRecievingByStudentsReport();
            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = Session["AdvancesRecievingByStudents_"];
            };

            return ReportViewerExtension.ExportTo(report);
        }

        #region السلف التفصيلي Grid
        public ActionResult GetAdvancesRecievingByStudentsReportIndex()
        {
            return View();
        }

        public ActionResult GetStudentsForAdvances()
        {
            var students = db.AdvancePaymentMaster
                .Select(x => new SelectListItem { Text = db.INTEGRATION_All_Students.FirstOrDefault(c => c.STUDENT_ID == x.Student_Id).STUDENT_NAME, Value = x.Student_Id.ToString() }).Distinct().ToList();
            return this.JsonMaxLength(students);
        }

        public ActionResult AdvancesRecievingByStudents(AdvancesRecievingByStudentsVM model)
        {
            var data = db.Usp_GetAdvancesRecievingByStudents(model.AdvanceStartDate, model.AdvanceEndDate, model.ReturnStartDate, model.ReturnStartDate, model.StudentIDs, model.AdvanceTypeIDs).Select(x => new
            {
                STUDENT_NAME = x.STUDENT_NAME,
                Student_ID = x.Student_ID,
                NATIONAL_ID = x.NATIONAL_ID,
                AdvanceTypeId = x.AdvanceTypeId,
                AdvanceSettingName = x.AdvanceSettingName,
                PaymentDate = GregorianToHijri(x.PaymentDate),// != null ? x.PaymentDate.Value.Day + "/" + x.PaymentDate.Value.Month + "/" + x.PaymentDate.Value.Year : null,
                ReturningDate = GregorianToHijri(x.ReturningDate),// != null ? x.ReturningDate.Value.Day + "/" + x.ReturningDate.Value.Month + "/" + x.ReturningDate.Value.Year : null,
                DocNumber = x.DocNumber,
                TotalAdvanceValue = x.TotalAdvanceValue,
                MonthlyValue = x.MonthlyValue,
                ReturnedValue = x.ReturnedValue,
                ReturnMethod = x.ReturnMethod,
                PayRollNumber = x.PayRollNumber,
                PayDetailsId = x.PayDetailsId,
                Number = x.Number,
                RemainingValue = x.RemainingValue

            }).ToList();

            return this.JsonMaxLength(data);
        }

        #endregion

        #endregion



        public ActionResult DReports()
        {

            string connectionString = ConfigurationManager.AppSettings["DynamicReports.ConnectionString"];
            CustomStringConnectionParameters connectionParameters = new CustomStringConnectionParameters(connectionString);


            DevExpress.DataAccess.Sql.SqlDataSource ds = new DevExpress.DataAccess.Sql.SqlDataSource(connectionParameters);
            ds.Name = "StudentData";

            ds.Queries.Add(new CustomSqlQuery("StudentsData", "SELECT * FROM [dbo].VwStudentData"));
            ds.RebuildResultSchema();



            return View(new ReportDesignerModel() { DataSrc = ds });
        }

        public SqlDataSource LoadFromCache()
        {

            string cacheKey = "DreportDs";
            if (HttpContext.Cache[cacheKey] != null)
            {
                return (SqlDataSource)(HttpContext.Cache[cacheKey]);
            }
            else
            {

                string connectionString = System.Configuration.ConfigurationManager.AppSettings["DynamicReports.ConnectionString"];
                CustomStringConnectionParameters connectionParameters = new CustomStringConnectionParameters(connectionString);


                DevExpress.DataAccess.Sql.SqlDataSource ds = new DevExpress.DataAccess.Sql.SqlDataSource(connectionParameters);
                ds.Name = "StudentData";

                ds.Queries.Add(new CustomSqlQuery("StudentsData", "SELECT * FROM [dbo].VwStudentData"));
                ds.RebuildResultSchema();
                SqlDataSource result = ds;

                HttpContext.Cache[cacheKey] = result;
                return result;
            }
        }
        public ActionResult ShowMeReport()
        {
            var report = new SubsidiesDetailedRPT();
            var ds = report.DataSource as SqlDataSource;
            var query = ds?.Queries[0] as CustomSqlQuery;
            ds?.RebuildResultSchema();
            return View();
        }

        public ActionResult StudentsWithoutBankNumberIndex()
        {
            ViewBag.Faculties = db.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Degrees = db.usp_getDegrees().Select(x => new SelectListItem { Text = x.DEGREE_DESC, Value = x.DEGREE_CODE.ToString() }).Where(x => x.Text != null).ToList();

            ViewBag.Stutes = db.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.STATUS_DESC, Value = x.STATUS_CODE.ToString() }).Where(x => x.Text != null).Distinct().ToList();

            ViewBag.Levels = db.INTEGRATION_All_Students.Select(x => new SelectListItem { Text = x.LEVEL_DESC, Value = x.LEVEL_CODE.ToString() }).Where(x => x.Text != null).OrderBy(x => x.Value).Distinct().ToList();

            List<int> Years = new List<int>();
            DateTime startYear = DateTime.Now;
            for (int i = DateTime.Now.Year; i >= 2000; i--)
                Years.Add(i);

            ViewBag.Years = Years;

            var permissions = GetPermissionsFn(56);
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

        public List<Get_studentswithoutAccount_NO_Custom_Result> StudentsWithoutBankNumber(string facultiesIdsString, string degreesIdsString,
          string statusesIdsString, string levelsIdsString)
        {
            var studentsWithoutBankNumber = new List<Get_studentswithoutAccount_NO_Custom_Result>();

            studentsWithoutBankNumber = db.Get_studentswithoutAccount_NO_Custom(facultiesIdsString, degreesIdsString, statusesIdsString, levelsIdsString).Distinct().ToList();

            return studentsWithoutBankNumber;
        }

        public ActionResult StudentsWithoutBankNumberRPT(decimal[] facultiesCheckListBox, decimal[] degreeCheckListBox,
            decimal[] stutesCheckListBox, decimal[] levelsCheckListBox)
        {

            var facultiesIdsString = string.Join(",", facultiesCheckListBox);
            var degreesIdsString = string.Join(",", degreeCheckListBox);
            var statusesIdsString = string.Join(",", stutesCheckListBox);
            var levelsIdsString = string.Join(",", levelsCheckListBox);

            var reportSource = StudentsWithoutBankNumber(facultiesIdsString, degreesIdsString, statusesIdsString, levelsIdsString);

            var report = new StudentsWithoutIdNumbers();

            report.DataSourceDemanded += (s, e) =>
            {
                ((XtraReport)s).DataSource = reportSource;
            };
            return PartialView("_StudentswithoutidNumberspartial", report);
        }

        //public ActionResult StudentsWithoutBankNumberRPTExport()
        //{
        //    var reportSource = StudentsWithoutBankNumber();

        //    var report = new StudentsWithoutIdNumbers();

        //    report.DataSourceDemanded += (s, e) =>
        //    {
        //        ((XtraReport)s).DataSource = reportSource;
        //    };
        //    return DocumentViewerExtension.ExportTo(report, Request);
        //}
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

    public class StudentsWithoutBankNumberReportVM
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AccountNumber { get; set; }
        public string IdNumber { get; set; }
        public string FacultyName { get; set; }
        public string DegreeLevel { get; set; }
        public string AcadimicStatus { get; set; }
        public string State { get; set; }

    }

    public class ReportDesignerModel
    {
        public SqlDataSource DataSrc { get; set; }
    }

}

public class MyUploadControlValidationSettings
{
    public static UploadControlValidationSettings Settings = new UploadControlValidationSettings()
    {
        AllowedFileExtensions = new[] { ".xls", ".xlsx" },
        MaxFileSize = 4194304
    };
}


