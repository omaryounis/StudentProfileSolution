
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using Newtonsoft.Json;
using StudentProfile.Components;
using StudentProfile.Components.Helpers;
using StudentProfile.DAL.Models;
using StudentProfile.DAL.Models.DataAnnotations;
using StudentProfile.DAL.Models.VM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace StudentProfile.Web.Controllers
{
    public partial class StudentsController : Controller
    {
        private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();

        private EsolERPEntities dbAcc = new EsolERPEntities();

        private HRMadinaEntities dbHR = new HRMadinaEntities();

        public class Permissions
        {
            public bool Create { get; set; }
            public bool Read { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }
            public bool View { get; set; }
            public bool Save { get; set; }
            public bool ShowStudentImage { get; set; }
            public bool DownloadAttachments { get; set; }
            public bool PreviewAttachments { get; set; }
        }

        public ActionResult AllData()
        {
            if (Session["StudentLogin"].ToString() == "1")
            {
                var model = db.INTEGRATION_All_Students.ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("NoPermissions", "Security");
            }
        }
        // GET: Students
        [CryptoValueProvider]
        public ActionResult Index(int? id)

        {
            Session["id"] = id.ToString();
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                if (id > 0)
                {
                    var model = db.INTEGRATION_All_Students.Where(x => x.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        ViewBag.StudentID = model.STUDENT_ID;

                    }
                }
                else
                {
                    ViewBag.StudentID = 0;
                }

                //OracleDataReader rdrGetTotals = cmdGetTotals.ExecuteReader();

                //con.Close();


                var json = new JavaScriptSerializer().Serialize(GetPermissions(1).Data);
                var permissions = JsonConvert.DeserializeObject<SecurityController.Permissions>(json);
                ViewBag.Create = permissions.Create;
                ViewBag.Read = permissions.Read;
                ViewBag.Update = permissions.Update;
                ViewBag.Delete = permissions.Delete;
                ViewBag.Save = permissions.Save;
                if (permissions.View)
                {
                    return View();
                }

                return RedirectToAction("NotAuthorized", "Security");

                //return View();
            }
        }

        [HttpPost]
        [CryptoValueProvider]
        public ActionResult Index(int? id, FormCollection frm)
        {
            if (Session["StudentLogin"].ToString() == "1")
            {
                int? studentid = int.Parse(frm["StudentId_VI"]);
                string StudentName = "";

                string idnumber = "";
                ViewBag.IdNumber = "";

                if (studentid > 0)
                {
                    idnumber = db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == studentid).FirstOrDefault()
                        .NATIONAL_ID;
                    StudentName = db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == studentid).FirstOrDefault()
                        .STUDENT_NAME;
                }

                ViewBag.StudentID = studentid;
                ViewBag.IdNumber = idnumber;
                ViewBag.StudentName = StudentName;

                return View();
            }
            else
            {
                return RedirectToAction("NoPermissions", "Security");
            }
        }

        public static List<SelectListItem> GetIssuess()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                var issues = db.Issues.Select(x => new SelectListItem
                {
                    Text = x.IssueDescription,
                    Value = x.Id.ToString()
                }).ToList();
                return issues;
            }

        }
        public ActionResult _StudentsSearch(string firstName, string txt, string lastName)
        {
            if (Session["StudentLogin"].ToString() == "1")
            {
                db.Database.CommandTimeout = 0;
                List<SelectListItem> selectedStudents = new List<SelectListItem>();
                selectedStudents = db.PR_GetStudentByName(firstName, txt, lastName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.student_id.Value.ToString(),
                        Text = x.student_name
                    }).ToList();
                return PartialView(selectedStudents);
            }
            else
            {
                return RedirectToAction("NoPermissions", "Security");
            }
        }

        public ActionResult _AdvancedSearchStudentName(string txt)
        {
            if (Session["StudentLogin"].ToString() == "1")
            {
                db.Database.CommandTimeout = 0;
                List<SelectListItem> selectedStudents = new List<SelectListItem>();
                selectedStudents = db.PR_GetStudentByName("", txt, "")
                    .Select(x => new SelectListItem
                    {
                        Value = x.student_id.Value.ToString(),
                        Text = x.student_name
                    }).ToList();
                return PartialView(selectedStudents);
            }
            else
            {
                return RedirectToAction("NoPermissions", "Security");
            }
        }
        //public ActionResult SubmitSearch(FormCollection St)
        //{
        //    string stID = St["StudentId_VI"];
        //    string StudentName = St["StudentId"];

        //    string studentIdNumber = St["StudentIdNumber"];
        //    string studentIdentityNumber = St["StudentIdentityNumber"];
        //    string studentPhoneNumber = St["StudentPhoneNumber"];
        //    var model = new INTEGRATION_All_Students();

        //    //remove default devexpress null values
        //    if (StudentName == "ادخل اسم الطالب")
        //    {
        //        StudentName = string.Empty;
        //    }
        //    else if (!string.IsNullOrEmpty(StudentName))
        //    {
        //        //بحث بالاسم
        //        decimal? stid = db.PR_GetStudentByName("", StudentName, "").FirstOrDefault().student_id;
        //        model = db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == stid).FirstOrDefault();
        //    }

        //    if (studentIdNumber == "ادخل الرقم الجامعى")
        //    {
        //        studentIdNumber = string.Empty;
        //    }
        //    else if (studentIdNumber != null)
        //    {
        //        decimal idnumber = decimal.Parse(studentIdNumber);
        //        //بحث بالرقم الجامعى
        //        model = db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == idnumber).ToList()
        //            .OrderBy(x => x.STUDENT_ID).LastOrDefault();
        //    }

        //    if (studentPhoneNumber == "ادخل الجوال")
        //    {
        //        studentPhoneNumber = string.Empty;
        //    }
        //    else if (!string.IsNullOrEmpty(studentPhoneNumber))
        //    {
        //        //بحث برقم الجوال
        //        model = db.INTEGRATION_All_Students.Where(x =>
        //                x.MOBILE_NO == studentPhoneNumber || x.MOBILE_PHONE == studentPhoneNumber)
        //            .OrderBy(x => x.STUDENT_ID).ToList()
        //            .LastOrDefault();
        //    }

        //    if (studentIdentityNumber == "ادخل رقم الهوية")
        //    {
        //        studentIdentityNumber = string.Empty;
        //    }
        //    else if (!string.IsNullOrEmpty(studentIdentityNumber))
        //    {
        //        //بحث برقم الهوية
        //        model = db.INTEGRATION_All_Students.Where(x => x.NATIONAL_ID == studentIdentityNumber).ToList()
        //            .OrderBy(x => x.STUDENT_ID).LastOrDefault();
        //    }
        //    return RedirectToAction("_StDetails", new { Studentid = model.STUDENT_ID });
        //}

        public ActionResult _StDetails(int? Studentid, FormCollection St)
        {
            var model = db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == Studentid).FirstOrDefault();
            var userId = 0;
            if (HttpContext.Session.Count > 0)
            {
                //userId = int.Parse(HttpContext.Session["UserId"].ToString());

                //if (userId == 0)
                //{
                //    //return RedirectToAction("Login", "Login");
                //    return PartialView("_StDetails");
                //}
            }
            var allPermissions = CheckPermissions.IsAuthorizedTab(userId);
            //var jsonBasicData = new JavaScriptSerializer().Serialize(GetPermissions(1).Data);
            //var jsonAcademicHistory = new JavaScriptSerializer().Serialize(GetPermissions(2).Data);
            //var jsonResidenceData = new JavaScriptSerializer().Serialize(GetPermissions(3).Data);
            //var jsonRelatives = new JavaScriptSerializer().Serialize(GetPermissions(4).Data);
            //var jsonEmpDocument = new JavaScriptSerializer().Serialize(GetPermissions(5).Data);
            //var jsonEmpCourses = new JavaScriptSerializer().Serialize(GetPermissions(6).Data);
            //var jsonViolationData = new JavaScriptSerializer().Serialize(GetPermissions(7).Data);
            //var jsonStudentNotes = new JavaScriptSerializer().Serialize(GetPermissions(10).Data);
            //var jsonClinicData = new JavaScriptSerializer().Serialize(GetPermissions(8).Data);
            //var jsonFileManager = new JavaScriptSerializer().Serialize(GetPermissions(9).Data);
            //var jsonFinances = new JavaScriptSerializer().Serialize(GetPermissions(11).Data);
            //var jsonTraficViolations = new JavaScriptSerializer().Serialize(GetPermissions(18).Data);

            //var basicData = GetTabsPermissions(allPermissions.Where(x=>x.ScreenID==1).Select(x=>  x.ScreenActionName).ToList());
            //var academicHistory = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 2).Select(x => x.ScreenActionName).ToList());
            //var residenceData = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 3).Select(x => x.ScreenActionName).ToList()); ;
            //var relatives = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 4).Select(x => x.ScreenActionName).ToList()); ;
            //var empDocument = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 5).Select(x => x.ScreenActionName).ToList()); ;
            //var empCourses = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 6).Select(x => x.ScreenActionName).ToList()); ;
            //var violationData = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 7).Select(x => x.ScreenActionName).ToList()); ;
            //var studentNotes = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 10).Select(x => x.ScreenActionName).ToList()); ;
            //var clinicData = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 8).Select(x => x.ScreenActionName).ToList()); ;
            //var FileManager = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 9).Select(x => x.ScreenActionName).ToList()); ;
            //var finances = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 11).Select(x => x.ScreenActionName).ToList()); ;
            //var traficViolations = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 18).Select(x => x.ScreenActionName).ToList()); ;
            //var customFieldsPermissions = GetTabsPermissions(allPermissions.Where(x => x.ScreenID == 26).Select(x => x.ScreenActionName).ToList()); ;
            //var customFieldsPermissions = JsonConvert.DeserializeObject<Permissions>(jsonCustomFieldsPermissions);


            ViewBag.basicDataRead = allPermissions.Where(x => x == 1) != null ? true : false;
            ViewBag.academicHistoryRead = allPermissions.Where(x => x == 2) != null ? true : false;
            ViewBag.residenceDataRead = allPermissions.Where(x => x == 3) != null ? true : false;
            ViewBag.relativesRead = allPermissions.Where(x => x == 4) != null ? true : false;
            ViewBag.empDocumentRead = allPermissions.Where(x => x == 5) != null ? true : false;
            ViewBag.empCoursesRead = allPermissions.Where(x => x == 6) != null ? true : false;
            ViewBag.violationDataRead = allPermissions.Where(x => x == 7) != null ? true : false;
            ViewBag.studentNotesRead = allPermissions.Where(x => x == 10) != null ? true : false;
            ViewBag.clinicDataRead = allPermissions.Where(x => x == 8) != null ? true : false;
            ViewBag.FileManagerRead = allPermissions.Where(x => x == 9) != null ? true : false;
            ViewBag.financesRead = allPermissions.Where(x => x == 11) != null ? true : false;
            ViewBag.traficViolationsRead = allPermissions.Where(x => x == 18) != null ? true : false;
            ViewBag.ViewCustomFields = allPermissions.Where(x => x == 26) != null ? true : false;
            ViewBag.StudentID = 0;

            ViewBag.FisrtName = "";

            string imgpath = Server.MapPath("~/assets/images/user.png");
            //Image defaultImg = Image.FromFile(imgpath);
            ViewBag.Image = imgpath;
            ViewBag.NationalID = "";
            ViewBag.ViolationsNumber = 0;
            ViewBag.NotesNumber = 0;
            var alerts = db.PR_GetStudentAlertsCount(model.STUDENT_ID, model.NATIONAL_ID).ToList();
            //ViewBag.ViolationsNumber = alerts.Where(x => x.tableName == "VIOLATIONSINGLE").SingleOrDefault().ItemsCount;
            ViewBag.NotesNumber = alerts.Where(x => x.tableName == "StudentNotes").SingleOrDefault().ItemsCount;
            ViewBag.TraficViolationsNumber = alerts.Where(x => x.tableName == "ResidentViolations").SingleOrDefault().ItemsCount;
            ViewBag.VisaAlertsNumber = alerts.Where(x => x.tableName == "RunawayAliens").SingleOrDefault().ItemsCount;
            ViewBag.RunawayAlertsNumber = alerts.Where(x => x.tableName == "ReservedVisa").SingleOrDefault().ItemsCount;
            ViewBag.DocumentsExpirationNumber = alerts.Where(x => x.tableName == "EmpDocument").SingleOrDefault().ItemsCount;
            ViewBag.CustomFieldNumber = alerts.Where(x => x.tableName == "CustomFields").SingleOrDefault().ItemsCount;
            //للحصول على عدد المستندات اللى هتنتهى
            //var emp = dbHR.Employees.Where(x => x.IDNumber == model.NATIONAL_ID).FirstOrDefault();
            //if (emp != null)
            //{
            //    var documents = dbHR.EmpDocument.Where(x => x.EmpID == emp.ID);
            //    var expiredDocuments = 0;

            //    foreach (var document in documents)
            //    {
            //        var alertDays = dbHR.Documents.SingleOrDefault(x => x.ID == document.DocumentID)?.AlertDays;
            //        var totalDaysDiffernce = (document.ExpDate - DateTime.Now)?.TotalDays;
            //        if (alertDays >= totalDaysDiffernce)
            //        {
            //            expiredDocuments++;
            //        }
            //    }


            //}


            Session["IdNumber"] = model?.NATIONAL_ID;
            Session["StudentId"] = model?.STUDENT_ID;
            Session["StudentName"] = model?.STUDENT_NAME;
           Session["STUDENT_NAME_S"] = model?.STUDENT_NAME_S;
            Session["FACULTY_NO"] = model.FACULTY_NO;
            ViewBag.NationalID = model?.NATIONAL_ID;
            return PartialView("_StDetails", model?.NATIONAL_ID);
        }
        [HttpPost]
        public ActionResult DeleteDocumentByAdmin( int DocumentId,int StudentId)
        {
            ViewBag.StudentID = StudentId;
            if (DocumentId > 0)
            {
                var document = db.StudentDocuments.Where(x => x.StudentDocumentID == DocumentId).SingleOrDefault();

                if (document != null)
                {

                    db.StudentDocuments.Remove(document);
                    if (Directory.Exists(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/")))
                    {
                        if (Directory.Exists(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/المستندات/")))
                        {
                            if (System.IO.File.Exists(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/المستندات/{document.DocumentImage}")))
                            {
                                System.IO.File.Delete(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/المستندات/{document.DocumentImage}"));
                            }
                        }

                    }
                }
                ViewBag.idNumber =db.UniversityStudents.Where(u=>u.ID == document.UniversityStudent_ID).FirstOrDefault().National_ID;
               
                db.SaveChanges();
                //return PartialView("_StDetails", ViewBag.idNumber);
                return Json(new { delete=true, JsonRequestBehavior.AllowGet });
            }
            else
            {
                return Json(new { delete = false, JsonRequestBehavior.AllowGet });
                //return RedirectToAction("_StDetails", new { Studentid = StudentId, FormCollection = new FormCollection() });
            }
        }

        [ValidateInput(false)]
        public ActionResult _FileManager(int? stID)
        {
            if (stID == null)
            {
                if (TempData["StudentID"] != null)
                {
                    stID = int.Parse(TempData["StudentID"].ToString());
                }
            }

            ViewBag.StudentID = stID.ToString();
            string rootFolder = Server.MapPath("~/Content/UserFiles/");
            string studentFolder = System.IO.Path.Combine(rootFolder, stID.ToString());
            DirectoryInfo currentFolder = new DirectoryInfo(studentFolder);
            if (currentFolder.Exists == false)
            {
                studentFolder = Path.Combine(rootFolder, "Empty");
            }

            return PartialView("_FileManager", studentFolder);
        }

        // [ValidateInput(false)]
        public FileStreamResult _FilesDownload(int? stID)
        {
            if (stID == null)
            {
                if (TempData["StudentID"] != null)
                {
                    stID = int.Parse(TempData["StudentID"].ToString());
                }
            }

            var settings = new DevExpress.Web.Mvc.FileManagerSettings() { Name = "fileManager" };
            settings.SettingsEditing.AllowDownload = true;
            string path = Server.MapPath("~/Content/UserFiles/" + stID);
            return FileManagerExtension.DownloadFiles(settings, path);
            //return FileManagerExtension.DownloadFiles(FileManagerDemoHelper.CreateMultipleFilesSelectionDownloadSettings(), (string)FileManagerDemoHelper.RootFolder);
        }

        [HttpPost]
        public ActionResult SaveNote(int? studentid, FormCollection frm)
        {
            if (studentid > 0)
            {
                var jsonNotesData = new JavaScriptSerializer().Serialize(GetPermissions(10).Data);
                var permissionsNotesData = JsonConvert.DeserializeObject<Permissions>(jsonNotesData);
                ViewBag.NotesDataCreate = permissionsNotesData.Create;
                ViewBag.NotesDataRead = permissionsNotesData.Read;
                ViewBag.NotesDataUpdate = permissionsNotesData.Update;
                ViewBag.NotesDataDelete = permissionsNotesData.Delete;
                ViewBag.NotesDataSave = permissionsNotesData.Save;


                bool IsSecret = false;
                string IssueValue = frm["IssuesComboBox_VI"];
                string note = frm["Note"];
                UploadedFile[] files = new List<UploadedFile>().ToArray();
                int? issueID = null;
                DateTime noteDate = DateTime.Now;
                string NoteDateTxt = frm["NoteDate"];
                string secret = frm["Secret"];
                if (secret == "on")
                {
                    IsSecret = true;
                }

                if (IssueValue != null)
                {
                    issueID = int.Parse(IssueValue);
                }

                if (noteDate != null)
                {
                    noteDate = Convert.ToDateTime(NoteDateTxt);
                }

                StudentNotes model = new StudentNotes();
                model.NoteDetails = note;
                //model.Evaluation = byte.Parse(evaluation);
                model.StDetailId = studentid.Value;
                model.IsSecret = IsSecret;
                model.NoteDate = noteDate;
                model.IssueId = issueID;
                db.StudentNotes.Add(model);
                db.SaveChanges();


                var fileList = Session["NoteFiles"];
                if (fileList != null)
                {
                    files = ((List<UploadedFile>)fileList).ToArray();
                }

                db.Database.CommandTimeout = 0;

                if (files != null)
                {
                    if (files.Count() > 0)
                    {
                        string rootFolder = Server.MapPath("~/Content/UserFiles/");
                        string studentFolder = System.IO.Path.Combine(rootFolder, studentid.ToString());
                        DirectoryInfo currentFolder = new DirectoryInfo(studentFolder);

                        if (currentFolder.Exists == false)
                        {
                            System.IO.Directory.CreateDirectory(studentFolder);
                        }

                        DirectorySecurity dSecurity = currentFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(
                            new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                            PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        currentFolder.SetAccessControl(dSecurity);
                        for (int i = 0; i < files.Count(); i++)
                        {
                            //var Code = file[i].FileCode.ToString().Split(',')[1].Trim();
                            Byte[] bytes =
                                System.IO.File.ReadAllBytes(Server.MapPath("~/Content/tempfiles/" + files[i].FileName));


                            string filename = String.Format("{0}_{1}.{2}", files[i].FileName.Split('.')[0],
                                DateTime.Now.ToString("dd-MM-yyyy-HH-mm"), files[i].FileName.Split('.')[1]);

                            FileStream fileStream = new FileStream(studentFolder + "\\" + filename, FileMode.Create,
                                FileAccess.ReadWrite);
                            try
                            {
                                var ms = new MemoryStream(bytes);
                                var image = Image.FromStream(ms);
                                var format = image.RawFormat;
                                var halfQualityImage = Files.CompressImage(image, 90);
                                fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
                                //fileStream.Write(bytes, 0, bytes.Length);
                                fileStream.Close();
                                StudentFiles file = new StudentFiles();
                                file.FilePath = filename;
                                file.StudentNoteId = model.NoteId;

                                db.StudentFiles.Add(file);

                                string root = Server.MapPath("~/Content/");
                                string tempFolder = System.IO.Path.Combine(rootFolder, "tempfiles");
                                DirectoryInfo Temp = new DirectoryInfo(tempFolder);
                                foreach (FileInfo temp in Temp.GetFiles())
                                {
                                    temp.Delete();
                                }
                            }
                            catch (Exception ex)
                            {
                            }

                            //System.IO.File.WriteAllBytes("../Content/img/TaskImages/", bytes);
                        }
                    }

                    //string evaluation = frm["evaluation_VI"];
                }

                try
                {
                    db.SaveChanges();
                    Session["NoteFiles"] = new List<UploadedFile>();
                }
                catch (Exception)
                {
                }
            }

            //return View("_StDetails");
            //return View("_StDetails", new {Studentid = studentid} );
            return RedirectToAction("_StDetails", new { studentid });
        }

        public ActionResult _AcademicHistory(string studentid)
        {
            db.Database.CommandTimeout = 0;
            //السجل الأكاديمي
            var jsonAcademicData = new JavaScriptSerializer().Serialize(GetPermissions(2).Data);
            var permissionsAcademicData = JsonConvert.DeserializeObject<Permissions>(jsonAcademicData);
            ViewBag.AcademicDataRead = permissionsAcademicData.Read;
            //if (studentid == null)
            //{
            //    studentid = Session["StudentId"]?.ToString();
            //}
            decimal? stid = decimal.Parse(studentid);
            INTEGRATION_All_Students currentRecord =
                db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == stid).FirstOrDefault();
            List<INTEGRATION_All_Students> studentHistory = db.INTEGRATION_All_Students.Where(x =>
                (x.NATIONAL_ID != null && x.NATIONAL_ID == currentRecord.NATIONAL_ID) ||
                (x.STUDENT_NAME == currentRecord.STUDENT_NAME && x.NATIONALITY_CODE == currentRecord.NATIONALITY_CODE)).OrderBy(x => x.STUDENT_ID).ToList();
            return PartialView(studentHistory);
        }

        [HttpPost]
        public ActionResult SaveAttachments(int? studentID)
        {
            DevExpress.Web.UploadedFile[] files =
                UploadControlExtension.GetUploadedFiles("myFile", FilesHelper.GetValidatingSettings());
            if (files != null)
            {
                if (files.Count() > 0)
                {
                    string rootFolder = Server.MapPath("~/Content/UserFiles/");
                    string studentFolder = System.IO.Path.Combine(rootFolder, studentID.ToString());
                    DirectoryInfo currentFolder = new DirectoryInfo(studentFolder);

                    if (currentFolder.Exists == false)
                    {
                        Directory.CreateDirectory(studentFolder);
                    }

                    DirectorySecurity dSecurity = currentFolder.GetAccessControl();


                    dSecurity.AddAccessRule(new FileSystemAccessRule(
                        new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    currentFolder.SetAccessControl(dSecurity);



                    for (int i = 0; i < files.Count(); i++)
                    {
                        //var Code = file[i].FileCode.ToString().Split(',')[1].Trim();
                        Byte[] bytes = files[i].FileBytes;

                        string filename = String.Format("{0}_{1}.{2}", files[i].FileName.Split('.')[0],
                            DateTime.Now.ToString("dd-MM-yyyy-HH-mm"), files[i].FileName.Split('.')[1]);

                        FileStream fileStream = new FileStream(studentFolder + "\\" + filename, FileMode.Create,
                            FileAccess.ReadWrite);
                        try
                        {
                            var ms = new MemoryStream(bytes);
                            var image = Image.FromStream(ms);
                            var format = image.RawFormat;
                            var halfQualityImage = Files.CompressImage(image, 90);
                            fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
                            //fileStream.Write(bytes, 0, bytes.Length);
                            fileStream.Close();
                            StudentFiles model = new StudentFiles();
                            model.FilePath = filename;
                            model.StudentNoteId = studentID.Value;
                            db.StudentFiles.Add(model);
                        }
                        catch (Exception ex)
                        {
                        }

                        //System.IO.File.WriteAllBytes("../Content/img/TaskImages/", bytes);
                    }

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                    }
                }
            }


            return RedirectToAction("_FileManager", new { stID = studentID });
        }

        public ActionResult _StBasicData(string Id)
        {
            //البيانات الأساسية
            var jsonBasicData = new JavaScriptSerializer().Serialize(GetPermissions(1).Data);
            var permissionsBasicData = JsonConvert.DeserializeObject<Permissions>(jsonBasicData);
            ViewBag.BasicDataRead = permissionsBasicData.Read;
            ViewBag.ShowStudentImage = permissionsBasicData.ShowStudentImage;

            var jsonCustomFieldsPermissions = new JavaScriptSerializer().Serialize(GetPermissions(26).Data);
            var customFieldsPermissions = JsonConvert.DeserializeObject<Permissions>(jsonCustomFieldsPermissions);
            ViewBag.ViewCustomFields = customFieldsPermissions.Read;
            int studentid = 0;
            if (int.TryParse(Id, out studentid))
            {
                studentid = int.Parse(Id);
            }
            db.Database.CommandTimeout = 0;

            GetStBasicDataByStudentID_Result model = db.GetStBasicDataByStudentID(studentid).FirstOrDefault();


            var StudyHistory = db.INTEGRATION_All_Students.Where(x => x.NATIONAL_ID == model.NATIONAL_ID).OrderBy(x => x.JOIN_DATE).ToList().Select(x =>
                new studentHistory
                {
                    JoinDate = x.JOIN_DATE.ToString("dd/MM/yyyy"),
                    Degree = x.DEGREE_DESC,
                    Name = x.STUDENT_NAME,
                    Status_Code = x.STATUS_CODE,
                    Student_ID = x.STUDENT_ID,
                    Status_Desc = x.STATUS_DESC
                }
            ).ToList();
            foreach (var item in StudyHistory)
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(
        "https://api.iu.edu.sa/api/ESOL_STUD_ACADEMIC/" + item.Student_ID);
                req.Headers.Add("authorization", "Bearer 0B07253E-4572-4F28-BA9E-59E6E77157BB");
                try
                {
                    var response = req.GetResponse();
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var streamString = reader.ReadToEnd();
                        if (streamString != null || streamString != "null")
                        {
                            var data = JsonConvert.DeserializeObject<ACADEMICAPI>(streamString);
                            item.GraduateDate = data?.GRADUATE_DATE?.ToString("dd/MM/yyyy");
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            ViewData["StudyHistory"] = StudyHistory;

            ViewData["REMAIN_HOURS"] = "";
            ViewData["MIN_SEMESTER"] = "";
            ViewData["PASSED_SEMESTER"] = "";
            ViewData["GRADUATE_DATE"] = "";
            ViewData["IS_SCHOLORSHIP"] = "";
            ViewData["SPONSOR_TYPE"] = "";
            HttpWebRequest academicRequest = (HttpWebRequest)HttpWebRequest.Create(
                 "https://api.iu.edu.sa/api/ESOL_STUD_ACADEMIC/" + model.STUDENT_ID);
            academicRequest.Headers.Add("authorization", "Bearer 0B07253E-4572-4F28-BA9E-59E6E77157BB");

            try
            {
                var response = academicRequest.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var streamString = reader.ReadToEnd();
                    if (streamString != null || streamString != "null")
                    {
                        var data = JsonConvert.DeserializeObject<ACADEMICAPI>(streamString);
                        ViewData["REMAIN_HOURS"] = data?.REMAIN_HOURS;
                        ViewData["MIN_SEMESTER"] = data?.MIN_SEMESTER;
                        ViewData["PASSED_SEMESTER"] = data?.PASSED_SEMESTER;
                        ViewData["GRADUATE_DATE"] = data?.GRADUATE_DATE;
                        ViewData["IS_SCHOLORSHIP"] = data?.IS_SCHOLORSHIP == true ? "نعم" : "لا";
                        ViewData["SPONSOR_TYPE"] = data?.SPONSOR_TYPE;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                "https://cas.iu.edu.sa/cas/photos/" + model.STUDENT_ID + ".jpg");
            request.Method = "HEAD";

            bool exists;
            try
            {
                request.GetResponse();
                exists = true;
                model.Student_Image = "https://cas.iu.edu.sa/cas/photos/" + model.STUDENT_ID + ".jpg";
            }
            catch (Exception ex)
            {
                exists = false;
                model.Student_Image = "/assets/images/user.png";
            }


            return PartialView("_StBasicData", model);
        }
        public ActionResult _EmergencyRelative(int? id, string StudentIDNumber, decimal studentID)
        {
            var model = new EmergencyRelatives();
            model.StudentIDNumber = StudentIDNumber;
            ViewData["stID"] = studentID;
            if (id > 0)
                model = db.EmergencyRelatives.Where(x => x.ID == id).SingleOrDefault();

            return PartialView("_EmergencyRelative", model);
        }
        public ActionResult SaveEmergencyRelative(string StudentID, EmergencyRelatives model)
        {
            var emergency = new EmergencyRelatives();
            if (!string.IsNullOrEmpty(model.StudentIDNumber))
            {
                if (model.ID > 0)
                {
                    emergency = db.EmergencyRelatives.Where(x => x.ID == model.ID).SingleOrDefault();
                    emergency.Name = model.Name;
                    emergency.PhoneNumber = model.PhoneNumber;
                    emergency.Relation = model.Relation;
                    emergency.Email = model.Email;

                }
                else
                {
                    emergency.Name = model.Name;
                    emergency.PhoneNumber = model.PhoneNumber;
                    emergency.Relation = model.Relation;
                    emergency.Email = model.Email;
                    emergency.StudentIDNumber = model.StudentIDNumber;
                    //emergency.StudentID = model.StudentID;
                    db.EmergencyRelatives.Add(emergency);
                }
                db.SaveChanges();
            }
            return RedirectToAction("_StBasicData", new { Id = StudentID });
        }
        public ActionResult DetailedAcademicRecords(string stid, int? semesterid)
        {
            if (stid != null && stid != "")
            {
                decimal studentid = decimal.Parse(stid);
                var model = db.proc_GetStudentGrades(studentid, semesterid).ToList();
                return View(model);
            }

            return View();
        }

        public ActionResult _AcademicData(int? studentid)
        {
            //السجل الأكاديمي
            var jsonAcademicData = new JavaScriptSerializer().Serialize(GetPermissions(2).Data);
            var permissionsAcademicData = JsonConvert.DeserializeObject<Permissions>(jsonAcademicData);
            ViewBag.AcademicDataRead = permissionsAcademicData.Read;

            //if (studentid == null)
            //{
            //    studentid = int.Parse(Session["StudentId"].ToString());
            //}

            List<ESOL_ACADEMIC_RECORDS> academicData = db.ESOL_ACADEMIC_RECORDS
                .Where(x => x.student_id == studentid)
                .OrderBy(x => x.semester).ToList();
            return PartialView(academicData);
        }

        public ActionResult _PageControl()
        {
            return PartialView("_StDetails", "");
        }

        public ActionResult _ClinicData(string nationalid)
        {
            //السجل الصحى
            var jsonClinicData = new JavaScriptSerializer().Serialize(GetPermissions(8).Data);
            var permissionsClinicData = JsonConvert.DeserializeObject<Permissions>(jsonClinicData);
            ViewBag.ClinicDataRead = permissionsClinicData.Read;
            DataTable dt = new DataTable();
            long NationalIDInt;
            if (nationalid != null && nationalid.Length > 0)
            {
                long.TryParse(nationalid, out NationalIDInt);
                dt = db.usp_GetClinicalHistory(NationalIDInt).ToList().CopyToDataTable();
            }

            return PartialView("_ClinicData", dt);
        }

        public ActionResult _ResidenceData(string studentid)
        {
            List<HousingDetailsVM> HousingDetailsVMList = new List<HousingDetailsVM>();
            int std = int.Parse(studentid);
            //بيانات السكن
            var jsonHousingData = new JavaScriptSerializer().Serialize(GetPermissions(3).Data);
            var permissionsHousingData = JsonConvert.DeserializeObject<Permissions>(jsonHousingData);
            ViewBag.HousingDataRead = permissionsHousingData.Read;
            var AccContext = new EsolERPEntities();
            var code = db.NextHousingNo_Sp(std, 0).FirstOrDefault();
            var StudentHousingInDB = db.HousingOfSudents.FirstOrDefault(x => x.StudentId == std && x.LeaveDate == null);
            if (StudentHousingInDB != null) {
                var LocationDetailsInDB = AccContext.VW_UnitParents.Where(p => p.SiteId == StudentHousingInDB.HousingLocationFurniture.LocationId).ToList();
                foreach (var item in LocationDetailsInDB) {

                    var data = new HousingDetailsVM
                    {
                        CompanyName = item.CompanyName,
                        DepartementName = item.DepartementName,
                        FloorName = item.FloorName,
                        SiteName = item.SiteName,
                        HousingNumber = StudentHousingInDB.Id,
                        OperationNo=StudentHousingInDB.Opt_No + "-" + code,
                        InsertionDate=StudentHousingInDB.InsertionDate.ToString("dd/MM/yyyy"),
                        HousingDate = StudentHousingInDB.HousingDate.ToString("dd/MM/yyyy"),
                        BedLocationId = StudentHousingInDB.HousingLocationFurniture.Id,
                        BedLocationCode = StudentHousingInDB.HousingLocationFurniture.Barcode,
                        LastUpdated = StudentHousingInDB.LastEdittededBy == null ? db.DashBoard_Users.FirstOrDefault(c => c.ID == StudentHousingInDB.userId).Name :
                        db.DashBoard_Users.FirstOrDefault(c => c.ID == StudentHousingInDB.LastEdittededBy).Name,
                        SiteId = StudentHousingInDB.LocationId,
                        IsFamilial = item.IsFamilial == true?"عائلي":"فردي",
                        NotesOfHosing = StudentHousingInDB.NotesOfHosing

                    };

                     


                    HousingDetailsVMList.Add(data);
                }
                return PartialView("_ResidenceData", HousingDetailsVMList);
            }
             return PartialView("_ResidenceData", HousingDetailsVMList);
        }

        public ActionResult _ViolationData(string studentid)
        {
            //السجل السلوكى
            var jsonViolationListData = new JavaScriptSerializer().Serialize(GetPermissions(7).Data);
            var permissionsViolationListData = JsonConvert.DeserializeObject<Permissions>(jsonViolationListData);
            ViewBag.ViolationListDataRead = permissionsViolationListData.Read;
            List<ViolationOfStudents> VioLationList = db.ViolationOfStudents
                .Where(x =>x.IsAccepted==true && x.StudentId.ToString() == studentid).ToList();
            return PartialView("_ViolationData", VioLationList);
        }

        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        [ValidateInput(false)]
        public ActionResult FileManagerPartial(int? stID)
        {
            if (stID == null)
            {
                if (Session["StudentID"] != null)
                {
                    stID = int.Parse(Session["StudentID"].ToString());
                }
            }

            //المرفقات
            var jsonAttachmentsData = new JavaScriptSerializer().Serialize(GetPermissions(9).Data);
            var permissionsAttachmentsData = JsonConvert.DeserializeObject<Permissions>(jsonAttachmentsData);
            ViewBag.AttachmentsDataRead = permissionsAttachmentsData.Read;
            ViewBag.AttachmentsDataCreate = permissionsAttachmentsData.Create;
            ViewBag.StudentID = stID.ToString();

            string studentFolder = Server.MapPath("~/Content/UserFiles/" + stID);
            DirectoryInfo currentFolder = new DirectoryInfo(studentFolder);
            if (currentFolder.Exists == false)
            {
                studentFolder = Server.MapPath("~/Content/UserFiles/Empty");
            }

            return PartialView("_FileManagerPartial", studentFolder);
        }


        public FileStreamResult FileManagerPartialDownload(int? stID)
        {
            if (stID == null)
            {
                if (TempData["StudentID"] != null)
                {
                    stID = int.Parse(TempData["StudentID"].ToString());
                }
            }

            var settings = new DevExpress.Web.Mvc.FileManagerSettings() { Name = "FileManager" };
            settings.SettingsEditing.AllowDownload = true;
            string path = Server.MapPath("~/Content/UserFiles/" + stID);
            return FileManagerExtension.DownloadFiles(settings, path);
        }

        public Permissions GetTabsPermissions(List<string> TabPermissions)
        {



            var perm = TabPermissions;

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
                else if (permission == "عرض صورة الطالب")
                {
                    permissions.ShowStudentImage = true;
                }
                else if (permission == "تحميل المرفقات")
                {
                    permissions.DownloadAttachments = true;
                }
                else if (permission == "معاينة المرفقات")
                {
                    permissions.PreviewAttachments = true;
                }
            }

            return permissions;
        }
        [HttpPost]
        public JsonResult GetPermissions(int screenId)
        {
            var userId = 0;
            if (HttpContext.Session.Count > 0)
            {
                var CurrentUser = System.Web.HttpContext.Current.Session["UserId"] as StudentProfile.DAL.Models.DashBoard_Users;

                userId = CurrentUser.ID;
                if (userId == 0)
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
                else if (permission == "عرض صورة الطالب")
                {
                    permissions.ShowStudentImage = true;
                }
                else if (permission == "تحميل المرفقات")
                {
                    permissions.DownloadAttachments = true;
                }
                else if (permission == "معاينة المرفقات")
                {
                    permissions.PreviewAttachments = true;
                }
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _StudentNotes()
        {
            var jsonNotesData = new JavaScriptSerializer().Serialize(GetPermissions(10).Data);
            var permissionsNotesData = JsonConvert.DeserializeObject<Permissions>(jsonNotesData);
            ViewBag.NotesDataCreate = permissionsNotesData.Create;
            ViewBag.NotesDataRead = permissionsNotesData.Read;
            ViewBag.NotesDataUpdate = permissionsNotesData.Update;
            ViewBag.NotesDataDelete = permissionsNotesData.Delete;
            ViewBag.NotesDataSave = permissionsNotesData.Save;

            List<SelectListItem> Issues = db.Issues.ToList()
                .Select(x => new SelectListItem { Text = x.IssueDescription, Value = x.Id.ToString() }).ToList();
            ViewBag.Issues = Issues;

            Session["NoteFiles"] = new List<DevExpress.Web.UploadedFile>();
            ViewData["Evaluation"] = new Dictionary<int, string> { { 0, "سلبى" }, { 1, "ايجابى" } };
            return PartialView("_StudentNotes");
        }

        [HttpPost]
        public ActionResult AddNewIssue(FormCollection frm)
        {
            var jsonNotesData = new JavaScriptSerializer().Serialize(GetPermissions(10).Data);
            var permissionsNotesData = JsonConvert.DeserializeObject<Permissions>(jsonNotesData);
            ViewBag.NotesDataCreate = permissionsNotesData.Create;
            ViewBag.NotesDataRead = permissionsNotesData.Read;
            ViewBag.NotesDataUpdate = permissionsNotesData.Update;
            ViewBag.NotesDataDelete = permissionsNotesData.Delete;
            ViewBag.NotesDataSave = permissionsNotesData.Save;

            string issueDescription = frm["IssueDescription"];
            if (issueDescription != null || issueDescription != "" || issueDescription != string.Empty)
            {
                int? IsExist = db.Issues.Where(x => x.IssueDescription == issueDescription).Count();
                if (IsExist == 0)
                {
                    Issues model = new Issues();
                    model.IssueDescription = issueDescription;
                    db.Issues.Add(model);
                    db.SaveChanges();
                }
            }

            List<SelectListItem> Issues = db.Issues.ToList()
                .Select(x => new SelectListItem { Text = x.IssueDescription, Value = x.Id.ToString() }).ToList();
            ViewBag.Issues = Issues;
            return PartialView("_StudentNotes");
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
        #region Advanced Search

        public ActionResult AdvancedSearch()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var permissions = GetPermissionsFn(37);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;
            if (permissions.Read == false)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }
            var SysTypeList = new List<SelectListItem>();
            SysTypeList.Add(new SelectListItem { Value = "إقامة صالحة", Text = "إقامة صالحة" });
            SysTypeList.Add(new SelectListItem { Value = "هروب", Text = "هروب" });
            SysTypeList.Add(new SelectListItem { Value = "مستجد", Text = "مستجدين" });

            ViewBag.SysType = SysTypeList.ToList();
            ViewBag.Nationalities = db.usp_getNationalities()
                .Select(x => new { Value = x.NATIONALITY_CODE, Text = x.NATIONALITY_DESC }).OrderBy(x => x.Value);
            ViewBag.Degrees = db.usp_getDegrees().Select(x => new { Value = x.DEGREE_CODE, Text = x.DEGREE_DESC })
                .OrderBy(x => x.Value).ToList();
            ViewBag.Faculties =
                db.usp_getFaculties().Select(x => new { Value = x.FACULTY_NO, Text = x.FACULTY_NAME })
                    .OrderBy(x => x.Value).ToList();
            ViewBag.Status = db.usp_getStatus().Select(x => new { Value = x.STATUS_CODE, Text = x.STATUS_DESC })
                .OrderBy(x => x.Value).ToList();
            ViewBag.Levels = db.usp_getLevels().Select(x => new { Value = x.LEVEL_CODE, Text = x.LEVEL_DESC })
                .OrderBy(x => x.Value).ToList();
            ViewBag.StudyTypes =
                db.usp_getStudyTypes().Select(x => new { Value = x.STUDY_CODE, Text = x.STUDY_DESC })
                    .OrderBy(x => x.Value).ToList();
            ViewBag.FieldTypes =
                db.CustomFields.Where(x => x.ParentId != null).Where(x => x.StudentsCustomFields.Count() > 0)
                    .ToList().Select(x => new { Value = x.CustomFieldId, Text = x.Key })
                    .OrderBy(x => x.Value).ToList();


            return View();
        }


        //[ValidateInput(false)]
        //public ActionResult AdvancedSearchGridViewPartial(decimal? nationality, decimal? degree, decimal? faculty,
        //    decimal? level, decimal? StatusType, decimal? studyType, decimal? fieldtype)
        //{
        //    List<decimal> studentids = db.StudentsCustomFields.Where(x => x.CustomFieldId == fieldtype).ToList()?.Select(x => decimal.Parse(x.StudentId)).ToList();
        //    var model = db.INTEGRATION_All_Students.Where(x =>
        //        (nationality == null || x.NATIONALITY_CODE == nationality) &&
        //        (degree == null || x.DEGREE_CODE == degree) &&
        //        (faculty == null || x.FACULTY_NO == faculty) &&
        //        (level == null || x.LEVEL_CODE == level) && (StatusType == null || x.STATUS_CODE == StatusType) &&
        //        (studyType == null || x.STUDY_CODE == studyType) && studentids.Any(p=>p == x.STUDENT_ID)).ToList();
        //    return PartialView("_AdvancedSearchGridViewPartial", model);
        //}


        [ValidateInput(false)]
        public ActionResult AdvancedSearchGridViewPartial(string fName, string inName, string lName, string IdentityNum, decimal? StudentNum, string PhoneNum, int[] nationality, int[] degree, decimal? faculty,
            decimal? level, int[] StatusType, decimal? StudyType, int[] fieldtype, string sysType, int[] NotInfieldtype)
        {
            bool validButNotExist = false;
            string statustype = null;
            string fieldType = null;
            string notfieldType = null;
            string nationalities = null;
            string degrees = null;
            if (nationality != null && nationality.Count() > 0)
                nationalities = string.Join(",", nationality);
            if (degree != null && degree.Count() > 0)
                degrees = string.Join(",", degree);
            if (NotInfieldtype != null && NotInfieldtype.Count() > 0)
                notfieldType = string.Join(",", NotInfieldtype);
            if (fieldtype != null && fieldtype.Count() > 0)
                fieldType = string.Join(",", fieldtype);
            if (StatusType != null && StatusType.Count() > 0)
                statustype = string.Join(",", StatusType);
            if (!string.IsNullOrEmpty(sysType))
                validButNotExist = true;
            db.Database.CommandTimeout = 0;
            var json = new JavaScriptSerializer().Serialize(GetPermissions(37).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            if (permissions.Read == false)
            {
                RedirectToAction("NotAuthorized", "Security");
            }
            var model = db.PR_GetAdvancedSearchResults(fName, inName, lName, IdentityNum, StudentNum, PhoneNum, fieldType, nationalities, degrees, faculty, level, statustype, StudyType, validButNotExist, sysType, notfieldType).ToList();

            return PartialView("_AdvancedSearchGridViewPartial", model);
        }


        public ActionResult ExportTo()
        {
            var model = Session["TypedListModel"];
            GridViewSettings settings = new GridViewSettings
            {
                Name = "gvTypedListDataBinding",
                CallbackRouteValues = new { Controller = "Home", Action = "TypedListDataBindingPartial" },
                KeyFieldName = "ID"
            };


            settings.Columns.Add("ID");
            settings.Columns.Add("Text");
            settings.Columns.Add("Quantity");
            settings.Columns.Add("Price");
            PrintableComponentLinkBase pcl = new PrintableComponentLinkBase(new PrintingSystemBase())
            {
                Component = GridViewExtension.CreatePrintableObject(settings, model)
            };
            pcl.CreateDocument();
            pcl.PrintingSystemBase.Document.AutoFitToPagesWidth = 1;

            using (MemoryStream stream = new MemoryStream())
            {
                pcl.ExportToPdf(stream);

                Response.Clear();
                Response.Buffer = false;
                Response.AppendHeader("Content-Type", "application/pdf");
                Response.AppendHeader("Content-Transfer-Encoding", "binary");
                Response.AppendHeader("Content-Disposition", "attachment; filename=document.pdf");
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }

            return RedirectToAction("Index");
        }

        public bool SaveToFTP(byte[] buffer, string targetFile)
        {
            bool saved = false;
            try
            {
                string ftpServerIP = "10.0.0.10";
                string ftpusername = "ftp_StudentFiles";
                string ftppassword = "1803@ftp_Stud#tFil!";
                Uri FullPath = new Uri("ftp://" + ftpServerIP + "//" + targetFile);
                Uri path = new Uri("ftp://" + ftpServerIP + "//");
                // + targetFile;


                string[] Folders = FullPath.LocalPath.Split('/').ToArray();
                Folders = Folders.Where(x => x != "").ToArray();
                for (int i = 0; i < Folders.Length - 1; i++)
                {
                    path = new Uri(path.AbsoluteUri + Folders[i] + "/");

                    try
                    {
                        FtpWebRequest ftpFoldersReq = (FtpWebRequest)WebRequest.Create(path);
                        ftpFoldersReq.Credentials = new NetworkCredential(ftpusername, ftppassword);
                        ftpFoldersReq.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                        FtpWebResponse usersFolderReq = (FtpWebResponse)ftpFoldersReq.GetResponse();
                    }
                    catch (WebException ex)
                    {
                        FtpWebRequest ftpFoldersReq = (FtpWebRequest)WebRequest.Create(path);
                        ftpFoldersReq.Credentials = new NetworkCredential(ftpusername, ftppassword);
                        ftpFoldersReq.Method = WebRequestMethods.Ftp.MakeDirectory;
                        FtpWebResponse usersFolderReq = (FtpWebResponse)ftpFoldersReq.GetResponse();
                    }
                }

                path = new Uri("ftp://" + ftpServerIP + "//" + targetFile);
                FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(path);
                ftpReq.Credentials = new NetworkCredential(ftpusername, ftppassword);
                ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                ftpReq.UseBinary = true;
                FtpWebResponse response = (FtpWebResponse)ftpReq.GetResponse();

                byte[] b = buffer;

                ftpReq.ContentLength = b.Length;
                using (Stream s = ftpReq.GetRequestStream())
                {
                    s.Write(b, 0, b.Length);
                }

                FtpWebResponse ftpResp = (FtpWebResponse)ftpReq.GetResponse();

                if (ftpResp != null)
                {
                    saved = true;
                }
            }
            catch (WebException ex)
            {
            }

            return saved;
        }

        public bool DeleteFromFTP(string targetFile)
        {
            bool saved = false;
            try
            {
                string ftpServerIP = "10.0.0.10";
                string ftpusername = "ftp_StudentFiles";
                string ftppassword = "1803@ftp_Stud#tFil!";
                Uri FullPath = new Uri("ftp://" + ftpServerIP + "//" + targetFile);

                FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(FullPath);
                ftpReq.Credentials = new NetworkCredential(ftpusername, ftppassword);
                ftpReq.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpReq.UseBinary = true;
                FtpWebResponse response = (FtpWebResponse)ftpReq.GetResponse();
                saved = true;
            }
            catch (WebException ex)
            {
            }

            return saved;
        }

        public byte[] DownloadFromFTP(string imagepath)
        {
            byte[] newFileData = null;
            try
            {
                string ftpServerIP = "10.0.0.10";
                string ftpusername = "ftp_StudentFiles";
                string ftppassword = "1803@ftp_Stud#tFil!";
                Uri FullPath = new Uri("ftp://" + ftpServerIP + "//" + imagepath);
                WebClient ftpReq = new WebClient();

                ftpReq.Credentials = new NetworkCredential(ftpusername, ftppassword);
                newFileData = ftpReq.DownloadData(FullPath);
                return newFileData;
            }
            catch (WebException ex)
            {
            }

            return newFileData;
        }

        public List<string> changeNames()
        {
            string ftpServerIP = "10.0.0.10";
            string ftpusername = "ftp_StudentFiles";
            string ftppassword = "1803@ftp_Stud#tFil!";
            Uri FullPath = new Uri("ftp://" + ftpServerIP + "/UserFiles/");

            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(FullPath);
            ftpRequest.Credentials = new NetworkCredential(ftpusername, ftppassword);
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());

            List<string> directories = new List<string>();

            string line = streamReader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                Uri path = new Uri(FullPath.ToString() + line + "/المستندات/");
                try
                {
                    FtpWebRequest ftpFoldersReq = (FtpWebRequest)WebRequest.Create(path);
                    ftpFoldersReq.Credentials = new NetworkCredential(ftpusername, ftppassword);
                    ftpFoldersReq.Method = WebRequestMethods.Ftp.ListDirectory;
                    FtpWebResponse usersFolderReq = (FtpWebResponse)ftpFoldersReq.GetResponse();
                    StreamReader stream = new StreamReader(usersFolderReq.GetResponseStream());
                    string filename = stream.ReadLine();
                    int index = filename.IndexOf(".");
                    string newfilename = filename;
                    if (index > 0)
                    {
                        newfilename = filename.Substring(0, index);
                    }

                    try
                    {
                        path = new Uri(path.ToString() + filename);
                        FtpWebRequest ftpFileReq = (FtpWebRequest)WebRequest.Create(path);
                        ftpFileReq.Credentials = new NetworkCredential(ftpusername, ftppassword);
                        ftpFileReq.Method = WebRequestMethods.Ftp.Rename;
                        ftpFileReq.RenameTo = newfilename + ".jpg";
                        FtpWebResponse FilesReq = (FtpWebResponse)ftpFileReq.GetResponse();
                    }
                    catch (Exception ex)
                    {
                        directories.Add(filename);
                    }
                }
                catch (WebException ex)
                {
                    directories.Add(line);
                }

                line = streamReader.ReadLine();
            }

            streamReader.Close();
            return directories;
        }

        public List<string> changeLocalFiles()
        {
            DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Content/UserFiles/")); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles();
            var directories = d.GetDirectories();
            List<string> filesInParent = directories.Select(x => x.Name).ToList();
            foreach (var item in filesInParent)
            {
                DirectoryInfo c = new DirectoryInfo(Server.MapPath("~/Content/UserFiles/" + item + "/المستندات/"));
                if (System.IO.Directory.Exists(Server.MapPath("~/Content/UserFiles/" + item + "/المستندات/")))
                {
                    var files = c.GetFiles();

                    foreach (var file in files)
                    {
                        string oldpath = Server.MapPath("~/Content/UserFiles/" + item + "/المستندات/" + file.Name);
                        string newpath = "";
                        if (oldpath.ToUpper().EndsWith(".JPG;.JPEG;.JPE;.JFIF"))
                        {
                            newpath = oldpath.Replace(".JPG;.JPEG;.JPE;.JFIF", ".jpg");
                            if (System.IO.File.Exists(oldpath))
                            {
                                System.IO.File.Move(oldpath, newpath);
                            }
                        }

                        if (oldpath.ToLower().EndsWith(".png.png"))
                        {
                            newpath = oldpath.Replace(".png.png", ".jpg");
                            if (System.IO.File.Exists(oldpath))
                            {
                                System.IO.File.Move(oldpath, newpath);
                            }
                        }
                    }
                }
            }

            return filesInParent;
        }

        public ActionResult _CustomFieldsSearch()
        {
            var model = ReportsController.GetCustomFiles().ToList();
            return PartialView("_CustomFieldsSearch", model);
        }
        public ActionResult _NotInCustomFieldsSearch()
        {
            var model = ReportsController.GetCustomFiles().ToList();
            return PartialView("_NotInCustomFieldsSearch", model);
        }
        public ActionResult _NationalitiesSearch()
        {
            var model = ReportsController.GetNationalities().ToList();
            return PartialView("_NationalitiesSearch", model);
        }

        public List<string> changeLocalHRFiles()
        {
            List<string> filesInParent = new List<string>();
            if (System.IO.Directory.Exists(Server.MapPath("~/Content/HRImagesUploaded/")))
            {
                DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Content/HRImagesUploaded/"));

                FileInfo[] Files = d.GetFiles();

                foreach (var item in Files)
                {
                    string oldpath = Server.MapPath("~/Content/HRImagesUploaded/" + item.Name);
                    string newpath = "";
                    if (oldpath.ToUpper().EndsWith(".JPG;.JPEG;.JPE;.JFIF"))
                    {
                        newpath = oldpath.Replace(".JPG;.JPEG;.JPE;.JFIF", ".jpg");
                        if (System.IO.File.Exists(oldpath))
                        {
                            System.IO.File.Move(oldpath, newpath);
                        }
                    }

                    if (oldpath.ToLower().EndsWith(".png.png"))
                    {
                        newpath = oldpath.Replace(".png.png", ".jpg");
                        if (System.IO.File.Exists(oldpath))
                        {
                            System.IO.File.Move(oldpath, newpath);
                        }
                    }
                }
            }

            return filesInParent;
        }

        #endregion
        #region UpdateStudentData
        public ActionResult StudentPhoto()
        {
            Session["PersonalImage"] = BinaryImageEditExtension.GetValue<byte[]>("PersonalImage");
            return BinaryImageEditExtension.GetCallbackResult();
        }
        [CryptoValueProvider]
        public ActionResult UpdateData(int id)
        {
            if (Session["UserId"] != null)
            {
                var user = Session["UserId"] as DashBoard_Users;
                if (user.IsStudent)
                {
                    Session["updateEmailCode"] = "";
                    Session["updateSMSCode"] = "";
                    Session["DataIsDone"] = false;

                    var studentPath = Server.MapPath($"~/Content/UserFiles/{id}");
                    var docsPath = Server.MapPath($"~/Content/UserFiles/{id}/المستندات");


                    if (!Directory.Exists(studentPath))
                    {
                        Directory.CreateDirectory(studentPath);
                    }
                    if (!Directory.Exists(docsPath))
                    {
                        Directory.CreateDirectory(docsPath);
                    }

                    return View(int.Parse(user.Username));
                }
                else
                {
                    return RedirectToAction("NoPermissions", "Security");
                }
            }
            return RedirectToAction("Login", "Login");
        }
        public ActionResult _CurrentData(int id)
        {
            var model = new UniversityStudents();
            ViewBag.ImageBytes = new byte[0];
            ViewBag.isInialized = true;
            Session["DataIsDone"] = false;
            ViewBag.Nationalities = db.usp_getNationalities()
               .Select(x => new { Value = x.NATIONALITY_CODE, Text = x.NATIONALITY_DESC }).OrderBy(x => x.Value).ToList();
            ViewBag.Countries = db.Country.Distinct().Select(x => new ListEditItem { Text = x.CountryNameAr, Value = x.ID.ToString() }).ToList();
            if (Session["UserId"] != null)
            {
                var nationalID = db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == id).FirstOrDefault()?.NATIONAL_ID;
                model = db.UniversityStudents.Where(x => x.Student_ID == id || (nationalID != null && x.National_ID == nationalID)).SingleOrDefault();
                if (model != null)
                {
                    model.Student_ID = id;
                    #region Old Code
                    //if ((model.LastEmailVerificationDate != null && model.LastPhoneVerificationDate != null) || Session["StudentLogin"].ToString() != "1")
                    //{
                    //    ViewBag.isInialized = false;
                    //}
                    #endregion

                    if (string.IsNullOrEmpty(model.ImageName))
                    {
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                        "https://cas.iu.edu.sa/cas/photos/" + model.Student_ID + ".jpg");
                        request.Method = "HEAD";

                        bool exists;
                        try
                        {
                            var response = request.GetResponse();
                            Stream objStream = response.GetResponseStream();
                            BinaryReader breader = new BinaryReader(objStream);
                            byte[] buffer = breader.ReadBytes((int)response.ContentLength);
                            ViewBag.ImageBytes = buffer;
                            exists = true;

                        }
                        catch (Exception ex)
                        {
                            exists = false;

                        }
                    }
                    else
                    {
                        string imagePath = Server.MapPath($"~/Content/UserFiles/{model.Student_ID}/{model.ImageName}");
                        if (System.IO.File.Exists(imagePath))
                        {
                            var x = System.IO.File.ReadAllBytes(imagePath);
                            ViewBag.ImageBytes = x.ToArray();
                        }
                    }
                }
                else
                {
                    model = new UniversityStudents();
                       DateTime birthDate;
                    DateTime? invalidDate = null;
                    int? nulledInt = null;
                    var oldRecord = db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID == id).SingleOrDefault();
                    if (oldRecord != null)
                    {
                        model.Student_ID = int.Parse(oldRecord.STUDENT_ID.ToString());
                        model.National_ID = oldRecord.NATIONAL_ID;
                        model.BirthDate = DateTime.TryParse(oldRecord.BIRTH_DATE, out birthDate) ? birthDate :
                           invalidDate;
                        model.NamePerIdentity_Ar = oldRecord.STUDENT_NAME;
                        model.NamePerPassport_Ar = oldRecord.STUDENT_NAME;
                        model.NamePerIdentity_En = oldRecord.STUDENT_NAME_S;
                        model.NamePerPassport_En = oldRecord.STUDENT_NAME_S;
                        model.PersonalEmail = oldRecord.EMAIL;
                        model.Nationality_ID = oldRecord.NATIONALITY_CODE != null ? int.Parse(oldRecord.NATIONALITY_CODE.ToString()) : nulledInt;

                        model.MobileNumber = string.IsNullOrEmpty(oldRecord.MOBILE_PHONE) ? oldRecord.MOBILE_NO : oldRecord.MOBILE_PHONE;
                        //model.OriginalCity = oldRecord.BIRTH_CITY_TEXT;
                        //model.OriginalCountry = oldRecord.na
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                        "https://cas.iu.edu.sa/cas/photos/" + model.Student_ID + ".jpg");
                        request.Method = "HEAD";

                        bool exists;
                        try
                        {
                            var response = request.GetResponse();
                            Stream objStream = response.GetResponseStream();
                            BinaryReader breader = new BinaryReader(objStream);
                            byte[] buffer = breader.ReadBytes((int)response.ContentLength);
                            ViewBag.ImageBytes = buffer;
                            exists = true;
                            model.ImageName = model.Student_ID + ".jpg";

                        }
                        catch (Exception ex)
                        {
                            exists = false;

                        }
                    }
                }
            }
            return PartialView("_CurrentData", model);
        }
        public ActionResult SaveStudentData(UniversityStudents model)
        {
            var notify = new notify();
            if (model != null)
            {
                try
                {
                    var existed = new UniversityStudents();
                    var files = BinaryImageEditExtension.GetValue<byte[]>("PersonalImage");
                    if (files != null)
                    {
                        if (files.Count() > 0)
                        {
                            string rootFolder = Server.MapPath("~/Content/UserFiles/");
                            string filename = String.Format($"{model.Student_ID}_image.jpg");
                            string studentFolder = System.IO.Path.Combine(rootFolder, model.Student_ID.ToString());

                            if (uploadImage(files, studentFolder, filename))
                            {
                                model.ImageName = filename;
                            }
                        }
                    }
                    else
                        return Json(notify = new notify() { Message = "برجاء اضافة الصورة الشخصية", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                    #region Old Code
                    //var checkUniversityStudents = db.UniversityStudents.FirstOrDefault(x => x.Student_ID == model.Student_ID);
                    //if (checkUniversityStudents != null)
                    //    return Json(notify = new notify() { Message = "تم حفظ بيانات الطالب مسبقا", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                    #endregion
                    var checkUniversityStudents = db.UniversityStudents.FirstOrDefault(x => x.National_ID == model.National_ID);
                    if (checkUniversityStudents != null)
                    {
                        existed = db.UniversityStudents.Where(x => x.ID == model.ID || x.National_ID == model.National_ID).SingleOrDefault();

                        if (existed != null)
                        {
                            existed.Student_ID = model.Student_ID;
                            existed.BirthDate = model.BirthDate;
                            existed.ImageName = model.ImageName;
                            existed.MobileNumber = model.MobileNumber;
                            existed.NamePerIdentity_Ar = model.NamePerIdentity_Ar;
                            existed.NamePerIdentity_En = model.NamePerIdentity_En;
                            existed.NamePerPassport_Ar = model.NamePerPassport_Ar;
                            existed.NamePerPassport_En = model.NamePerPassport_En;
                            existed.OriginalCity = model.OriginalCity;
                            existed.OriginalCountry = model.OriginalCountry;
                            existed.PersonalEmail = model.PersonalEmail;
                            existed.UniversityEmail = model.UniversityEmail;
                            existed.Nationality_ID = model.Nationality_ID;
                            existed.Country_ID = model.Country_ID;
                            existed.LastPhoneVerificationDate = DateTime.Now;
                            existed.LastEmailVerificationDate = DateTime.Now;
                            db.Entry(existed).State = EntityState.Modified;
                        }
                    }
                    else if (model.ID == 0)
                    {
                        model.National_ID = model.National_ID == null ? "" : model.National_ID;
                        model.LastPhoneVerificationDate = DateTime.Now;
                        model.LastEmailVerificationDate = DateTime.Now;
                        model.InsertDate = DateTime.Now;
                        db.UniversityStudents.Add(model);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(notify = new notify() { Message = "حدث خطأ أثناء حفظ البيانات برجاء مراجعتها والمتابعة مرة أخرى", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(notify = new notify() { Message = "تم الحفظ بنجاح برجاء استكمال بيانات الهوية", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckExpiration(int id)
        {
            bool ShowBtn = false;
            if (Session["UserId"] != null)
            {
                var user = Session["UserId"] as DashBoard_Users;
                if (user.IsStudent)
                {
                    var UniversityStudent = db.UniversityStudents.FirstOrDefault(x => x.Student_ID == id);
                    if (db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent.ID && (x.IsTransfer == true && x.IsExpired != true)).Count() == 2)
                    {
                        ShowBtn = false;
                    }
                    else if (db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent.ID && (x.IsTransfer != true)).Count() > 0)
                    {
                        ShowBtn = true;
                    }
                    else
                    {
                        ShowBtn = true;
                    }
                    if (UniversityStudent != null)
                    {
                        //get new docs and not send yet or  get approved docs to check on expiration
                        var List = db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent.ID
                        && ((x.ApprovedDate != null && x.IsNew == false && x.IsTransfer == true && x.IsExpired != true)
                            || (x.ApprovedDate == null && x.RefusedDate == null && x.IsNew == true && x.IsTransfer != true && x.IsExpired != true)
                           )).Select(p => new { p.StudentDocumentID, p.ExpiryDate, p.Document_ID }).ToList();

                        int configval = int.Parse(db.config.Where(m => m.Kay == "PassportExpirePeriod").Select(p => p.Value).FirstOrDefault());
                        foreach (var item in List)
                        {
                            if ((Math.Abs((DateTime.Now - item.ExpiryDate).Value.Days) <= configval || DateTime.Now >= item.ExpiryDate.Value) && item.Document_ID == 2)
                            {
                                StudentDocuments studentDocuments = db.StudentDocuments.Find(item.StudentDocumentID);
                                studentDocuments.IsExpired = true;
                                db.Entry(studentDocuments).State = EntityState.Modified;
                                db.SaveChanges();
                                return Json(new { isValid = false, msg = " مستند جواز السفر منتهي الصلاحية يرجي تحديث البيانات", ShowBtn = ShowBtn }, JsonRequestBehavior.AllowGet); ;
                            }
                            if (item.Document_ID == 1)
                            {
                                if (DateTime.Now >= item.ExpiryDate.Value)
                                {
                                    StudentDocuments studentDocuments = db.StudentDocuments.Find(item.StudentDocumentID);
                                    studentDocuments.IsExpired = true;
                                    db.Entry(studentDocuments).State = EntityState.Modified;
                                    db.SaveChanges();
                                    return Json(new { isValid = false, msg = " مستند رقم الهوية منتهي الصلاحية يرجي تحديث البيانات", ShowBtn = ShowBtn }, JsonRequestBehavior.AllowGet); ;
                                }
                            }
                        }
                        return Json(new { isValid = true, msg = "", ShowBtn = ShowBtn }, JsonRequestBehavior.AllowGet); ;
                    }
                }
            }
            return Json(new { isValid = true, msg = "", ShowBtn = ShowBtn }, JsonRequestBehavior.AllowGet); ;
        }

        public ActionResult _StudentDocuments(int id)
        {

            bool isValid = false;
            if (Session["UserId"] != null)
            {
                var user = Session["UserId"] as DashBoard_Users;
                if (user.IsStudent)
                {
                    var UniversityStudent = db.UniversityStudents.FirstOrDefault(x => x.Student_ID == id);
                    if (UniversityStudent != null)
                    {

                        ViewBag.StudentID = UniversityStudent.Student_ID;
                        var model = db.StudentDocuments.Select(c => new 
                        {
                            ID = c.StudentDocumentID,
                            c.UniversityStudent_ID,
                            c.UniversityStudents.Student_ID,
                            c.Document_ID,
                            Document = c.Document_ID == 1 ? "الهوية" : c.Document_ID == 2 ? "جواز السفر" : c.Document_ID == 3 ? "تاشيرة الدخول" : "",
                            c.IdentityNumber,
                            InsertDate = c.InsertDate != null ? c.InsertDate.Value.Day + "/" + c.InsertDate.Value.Month + "/" + c.InsertDate.Value.Year + " - " + c.InsertDate.Value.Hour + ":" + c.InsertDate.Value.Minute : null,
                            ExpiryDate = c.ExpiryDate != null ? c.ExpiryDate.Value.Day + "/" + c.ExpiryDate.Value.Month + "/" + c.ExpiryDate.Value.Year : null,
                            ApprovedDate = c.ApprovedDate != null ? c.ApprovedDate.Value.Day + "/" + c.ApprovedDate.Value.Month + "/" + c.ApprovedDate.Value.Year : null,
                            c.RefusedNotes,
                            isAproved = c.ApprovedDate != null && c.IsNew == false && c.IsExpired != true && c.IsTransfer == true ? "معتمدة" : (c.RefusedDate != null || c.RefusedNotes != null) && c.IsTransfer == true && c.IsNew == false && c.IsExpired != true ? "مرفوضة" : c.IsTransfer == true && c.IsNew == true && c.IsExpired != true ? "تحت المراجعة" : c.IsTransfer == null && c.ApprovedDate == null && c.RefusedDate == null && c.IsExpired != true ? "يرجي إرسال البيانات لإعتمادها" : c.IsExpired == true ? "منتهية الصلاحية" : "",
                            c.IsActive,
                            c.IsTransfer,
                            c.IsExpired
                        }).Where(x => x.UniversityStudent_ID == UniversityStudent.ID).ToList();
                        if (db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent.ID && x.IsTransfer != true).Count() > 0)
                        {
                            Session["DataIsDone"] = true;
                        }
                        else
                        {
                            Session["DataIsDone"] = false;
                        }
                        return PartialView(model);
                    }
                }
            }
            return PartialView();
        }

        public JsonResult StMailVerificationCode(string mailTo, string name)
        {
            var customMailMessage = new CustomMailMessage();
            var verificationCode = new VerificationCode();
            var notify = new notify();
            string GenerateCode = verificationCode.Generate(6);
            try
            {
                customMailMessage.SendMailConfirmationCode(mailTo, name, GenerateCode);
                Session["updateEmailCode"] = GenerateCode;
            }
            catch (Exception ex)
            {

                return Json(notify = new notify() { Message = "خطأ اثناء ارسال كود البريد الالكتروني", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "تم ارسال كود البريد الالكتروني بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StSMSVerificationCode(string mobileNumber)
        {
            var sendMessage = new SendMessage();
            var verificationCode = new VerificationCode();
            var notify = new notify();
            string GenerateCode = verificationCode.Generate(6);
            try
            {
                sendMessage._SendMessage("كود التفعيل الخاص بالطالب : " + GenerateCode, mobileNumber?.Trim());
                Session["updateSMSCode"] = GenerateCode;
            }
            catch (Exception ex)
            {
                return Json(notify = new notify() { Message = "خطأ اثناء ارسال كود الجوال", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }

            return Json(notify = new notify() { Message = "تم ارسال كود الجوال بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ConfirmCodes(string smsCode, string emailCode)
        {
            int result = 0;
            if (Session["UserId"] != null)
            {
                var user = Session["UserId"] as DashBoard_Users;

                if (Session["updateEmailCode"] != null && Session["updateSMSCode"] != null)
                {
                    if (Session["updateEmailCode"].ToString() == emailCode && Session["updateSMSCode"].ToString() == smsCode)
                    {
                        var studentid = user.ID;
                        var student = db.UniversityStudents.Where(x => x.Student_ID == studentid).SingleOrDefault();

                        result = 1;
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddStudentDocument(StudentDocuments model, int stID)
        {
            int studentRowid = 0;
            int id;
            if (Session["UserId"] != null)
            {
                id = stID;
                var student = db.UniversityStudents.SingleOrDefault(x => x.Student_ID == id);

                if (student == null) return null;
                studentRowid = student.ID;
                if (model.Document_ID == 1)
                {
                    student.National_ID = model.IdentityNumber;
                }
            }
            else
            {
                return null;
            }
            var files = BinaryImageEditExtension.GetValue<byte[]>("UploadedDocImage");
            model.UniversityStudent_ID = studentRowid;
            if (files != null)
            {
                if (files.Count() > 0)
                {
                    string rootFolder = Server.MapPath("~/Content/UserFiles/");
                    string filename = String.Format($"{model.UniversityStudent_ID}_doc_{model.Document_ID}_{DateTime.Now.ToString("ddMMyyyyhhmm")}.jpg");
                    string studentFolder = System.IO.Path.Combine(rootFolder, id.ToString() + "/المستندات/");

                    if (uploadImage(files, studentFolder, filename))
                    {
                        model.DocumentImage = filename;
                    }
                }
            }
            if (model.StudentDocumentID > 0)
            {
                var existed = db.StudentDocuments.Where(x => x.StudentDocumentID == model.StudentDocumentID).SingleOrDefault();
                existed.IdentityNumber = model.IdentityNumber;
                //existed.UniversityStudent_ID = studentRowid;
                existed.Document_ID = model.Document_ID;
                existed.ExpiryDate = model.ExpiryDate;
                existed.IssueDate = model.IssueDate;
                existed.IsActive = model.IsActive;
                existed.DocumentImage = model.DocumentImage;
            }
            else
            {
                db.StudentDocuments.Add(model);
            }
            db.SaveChanges();
            var list = db.StudentDocuments.Where(x => x.UniversityStudent_ID == model.UniversityStudent_ID).ToList();
            return PartialView("_IdentityDocuments", list);
        }

        //Save _Identity Data
        #region Save Identity Data
        public ActionResult _IdentityDocuments(int id)
        {
            if (Session["UserId"] != null)
            {
                ViewBag.PassportImageName = null;
                ViewBag.VisaImageName = null;
                
                int UniversityStudent_ID = db.UniversityStudents.FirstOrDefault(x => x.Student_ID == id).ID;
                if (id > 0 && UniversityStudent_ID > 0)
                {
                    var StDocIdentity = db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent_ID && x.Document_ID == 1)?.Select(x => new
                    {
                        IdentityID = x.StudentDocumentID,
                        IdentityNumber = x.IdentityNumber,
                        IdentityNo = x.IdentityNumber,
                        IdentityExpiryDate = x.ExpiryDate,
                        IdentityImageName = "~/Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
                    }).FirstOrDefault();
                    if (StDocIdentity != null)
                    {
                        if (System.IO.File.Exists(Server.MapPath(StDocIdentity.IdentityImageName)))
                        {
                            var x = System.IO.File.ReadAllBytes(Server.MapPath(StDocIdentity.IdentityImageName));
                            ViewBag.IdentityImageName = x.ToArray();
                        }
                    }

                    var StDocPassport = db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent_ID && x.Document_ID == 2)?.Select(x => new
                    {
                        PasportID = x.StudentDocumentID,
                        PassportNumber = x.IdentityNumber,
                        PassportExpiryDate = x.ExpiryDate,
                        PassportImageName = "~/Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
                    }).FirstOrDefault();
                    if (StDocPassport != null)
                    {
                        if (System.IO.File.Exists(Server.MapPath(StDocPassport.PassportImageName)))
                        {
                            var x = System.IO.File.ReadAllBytes(Server.MapPath(StDocPassport.PassportImageName));
                            ViewBag.PassportImageName = x.ToArray();
                        }

                    }


                    //var StDocVisa = db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent_ID && x.Document_ID == 3&& x.IsActive == true)?.Select(x => new
                    //{
                    //    VisaID = x.StudentDocumentID,
                    //    VisaNumber = x.IdentityNumber,
                    //    VisaExpiryDate = x.ExpiryDate,
                    //    VisaImageName = "~/Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
                    //}).FirstOrDefault();
                    //if (StDocVisa != null)
                    //{
                    //    if (System.IO.File.Exists(Server.MapPath(StDocVisa.VisaImageName)))
                    //    {
                    //        var x = System.IO.File.ReadAllBytes(Server.MapPath(StDocVisa.VisaImageName));
                    //        ViewBag.VisaImageName = x.ToArray();
                    //    }
                    //}


                    var studentDocuments = new StudentDocumentsVM
                    {
                        UniversityStudentID = UniversityStudent_ID,
                        Student_ID = id,
                        IdentityID = StDocIdentity == null ? 0 : StDocIdentity.IdentityID,
                        IdentityNumber = StDocIdentity == null ? null : StDocIdentity.IdentityNumber,
                        IdentityNo = StDocIdentity == null ? null : StDocIdentity.IdentityNumber,
                        IdentityExpiryDate = StDocIdentity == null ? null : StDocIdentity.IdentityExpiryDate,
                        IdentityImageName = StDocIdentity == null ? null : StDocIdentity.IdentityImageName,
                        PassportID = StDocPassport == null ? 0 : StDocPassport.PasportID,
                        PassportNumber = StDocPassport == null ? null : StDocPassport.PassportNumber,
                        PassportNo = StDocPassport == null ? null : StDocPassport.PassportNumber,
                        PassportExpiryDate = StDocPassport == null ? null : StDocPassport.PassportExpiryDate,
                        PassportImageName = StDocPassport == null ? null : StDocPassport.PassportImageName,
                        //VisaID = StDocVisa == null ? 0 : StDocVisa.VisaID,
                        //VisaNumber = StDocVisa == null ? null : StDocVisa.VisaNumber,
                        //VisaExpiryDate = StDocVisa == null ? null : StDocVisa.VisaExpiryDate,
                        //VisaImageName = StDocVisa == null ? null : StDocVisa.VisaImageName,
                    };
                    return PartialView("_IdentityDocuments", studentDocuments);
                }
            }
            return PartialView("_IdentityDocuments");
        }
        public ActionResult _IdentityDocumentForDelete(int id, int docId)
        {
            if (id > 0)
            {
                var document = db.StudentDocuments.Where(x => x.StudentDocumentID == docId).SingleOrDefault();

                if (document != null)
                {

                    db.StudentDocuments.Remove(document);
                    if (Directory.Exists(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/")))
                    {
                        if (Directory.Exists(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/المستندات/")))
                        {
                            if (System.IO.File.Exists(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/المستندات/{document.DocumentImage}")))
                            {
                                System.IO.File.Delete(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/المستندات/{document.DocumentImage}"));
                            }
                        }

                    }
                }
                db.SaveChanges();
                return RedirectToAction("_StudentDocuments", new { id = id });
            }
            else
            {
                return RedirectToAction("_StudentDocuments", new { id = id });
            }
        }
        public ActionResult _IdentityDocumentForEdie(int id, int docId)
        {
            if (Session["UserId"] != null)
            {
                ViewBag.IdentityImageName = null;
                ViewBag.PassportImageName = null;
                StudentDocumentsVM StudentDocuments = new StudentDocumentsVM();

                int UniversityStudent_ID = db.UniversityStudents.FirstOrDefault(x => x.Student_ID == id).ID;
                if (id > 0 && UniversityStudent_ID > 0)
                {
                    StudentDocuments.UniversityStudentID = UniversityStudent_ID;
                    var document = db.StudentDocuments.FirstOrDefault(x => x.StudentDocumentID == docId);
                    StudentDocuments.StudentDocumentID = document.StudentDocumentID;
                    if (document.Document_ID == 1)
                    {
                        var StDocIdentity = db.StudentDocuments.Select(x => new StudentDocumentsVM
                        {
                            Document_ID = x.Document_ID,
                            IdentityID = x.StudentDocumentID,
                            IdentityNumber = x.IdentityNumber,
                            IdentityNo = x.IdentityNumber,
                            IdentityExpiryDate = x.ExpiryDate,
                            IdentityImageName = "~/Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
                        }).FirstOrDefault(x => x.IdentityID == docId);

                        if (StDocIdentity != null)
                        {
                            if (System.IO.File.Exists(Server.MapPath(StDocIdentity.IdentityImageName)))
                            {
                                var x = System.IO.File.ReadAllBytes(Server.MapPath(StDocIdentity.IdentityImageName));
                                ViewBag.IdentityImageName = x.ToArray();
                            }
                        }
                        StudentDocuments.IdentityID = StDocIdentity == null ? 0 : StDocIdentity.IdentityID;
                        StudentDocuments.IdentityNumber = StDocIdentity == null ? null : StDocIdentity.IdentityNumber;
                        StudentDocuments.IdentityExpiryDate = StDocIdentity == null ? null : StDocIdentity.IdentityExpiryDate;
                        StudentDocuments.IdentityImageName = StDocIdentity == null ? null : StDocIdentity.IdentityImageName;
                    }
                    else
                    {
                        var StDocIdentity = db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent_ID && x.Document_ID == 1 && x.IsActive == true)?.Select(x => new StudentDocumentsVM
                        {
                            Document_ID = x.Document_ID,
                            IdentityID = x.StudentDocumentID,
                            IdentityNumber = x.IdentityNumber,
                            IdentityNo = x.IdentityNumber,
                            IdentityExpiryDate = x.ExpiryDate,
                            IdentityImageName = "~/Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
                        }).FirstOrDefault();
                        if (StDocIdentity != null)
                        {
                            if (System.IO.File.Exists(Server.MapPath(StDocIdentity.IdentityImageName)))
                            {
                                var x = System.IO.File.ReadAllBytes(Server.MapPath(StDocIdentity.IdentityImageName));
                                ViewBag.IdentityImageName = x.ToArray();
                            }
                        }
                        StudentDocuments.IdentityID = StDocIdentity == null ? 0 : StDocIdentity.IdentityID;
                        StudentDocuments.IdentityNumber = StDocIdentity == null ? null : StDocIdentity.IdentityNumber;
                        StudentDocuments.IdentityExpiryDate = StDocIdentity == null ? null : StDocIdentity.IdentityExpiryDate;
                        StudentDocuments.IdentityImageName = StDocIdentity == null ? null : StDocIdentity.IdentityImageName;
                    }

                    if (document.Document_ID == 2)
                    {
                        var StDocPassport = db.StudentDocuments.Select(x => new StudentDocumentsVM
                        {
                            PassportID = x.StudentDocumentID,
                            PassportNumber = x.IdentityNumber,
                            PassportExpiryDate = x.ExpiryDate,
                            PassportImageName = "~/Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
                        }).FirstOrDefault(x => x.PassportID == docId);
                        if (StDocPassport != null)
                        {
                            if (System.IO.File.Exists(Server.MapPath(StDocPassport.PassportImageName)))
                            {
                                var x = System.IO.File.ReadAllBytes(Server.MapPath(StDocPassport.PassportImageName));
                                ViewBag.PassportImageName = x.ToArray();
                            }

                        }
                        StudentDocuments.PassportID = StDocPassport == null ? 0 : StDocPassport.PassportID;
                        StudentDocuments.PassportNumber = StDocPassport == null ? null : StDocPassport.PassportNumber;
                        StudentDocuments.PassportExpiryDate = StDocPassport == null ? null : StDocPassport.PassportExpiryDate;
                        StudentDocuments.PassportImageName = StDocPassport == null ? null : StDocPassport.PassportImageName;
                    }
                    else
                    {
                        var StDocPassport = db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent_ID && x.Document_ID == 2 && x.IsActive == true)?.Select(x => new StudentDocumentsVM
                        {
                            Document_ID = x.Document_ID,
                            PassportID = x.StudentDocumentID,
                            PassportNumber = x.IdentityNumber,
                            PassportExpiryDate = x.ExpiryDate,
                            PassportImageName = "~/Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
                        }).FirstOrDefault();
                        if (StDocPassport != null)
                        {
                            if (System.IO.File.Exists(Server.MapPath(StDocPassport.PassportImageName)))
                            {
                                var x = System.IO.File.ReadAllBytes(Server.MapPath(StDocPassport.PassportImageName));
                                ViewBag.PassportImageName = x.ToArray();
                            }
                        }
                        StudentDocuments.PassportID = StDocPassport == null ? 0 : StDocPassport.PassportID;
                        StudentDocuments.PassportNumber = StDocPassport == null ? null : StDocPassport.PassportNumber;
                        StudentDocuments.PassportExpiryDate = StDocPassport == null ? null : StDocPassport.PassportExpiryDate;
                        StudentDocuments.PassportImageName = StDocPassport == null ? null : StDocPassport.PassportImageName;
                    }

                    return PartialView("_IdentityDocuments", StudentDocuments);
                }
            }
            return PartialView("_IdentityDocuments");
        }

        //public ActionResult GetDocumentsImage()
        //{
        //    if (Session["UserId"] != null && Session["UserId"].ToString() != null)
        //    {
        //        int stID = int.Parse(Session["UserId"].ToString());
        //        int UniversityStudent_ID = db.UniversityStudents.FirstOrDefault(x => x.Student_ID == stID).ID;
        //        if (stID > 0 && UniversityStudent_ID > 0)
        //        {
        //            string ImgIdentity = "";
        //            string ImgPassport = "";
        //            string ImgVisa = "";

        //            var StDocIdentity = db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent_ID  && x.Document_ID == 1).Select(x => new
        //            {
        //                ImagePath = "../Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
        //            }).FirstOrDefault();
        //            if (StDocIdentity != null)
        //            {
        //                byte[] binaryImage = (new WebClient()).DownloadData(Server.MapPath(StDocIdentity.ImagePath));
        //                ImgIdentity = string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(binaryImage));
        //            }

        //            var StDocPassport = db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent_ID && x.Document_ID == 2).Select(x => new
        //            {
        //                ImagePath = "../Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
        //            }).FirstOrDefault();
        //            if (StDocPassport != null)
        //            {
        //                byte[] binaryImage = (new WebClient()).DownloadData(Server.MapPath(StDocPassport.ImagePath));
        //                ImgPassport = string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(binaryImage));
        //            }

        //            var StDocVisa = db.StudentDocuments.Where(x => x.UniversityStudent_ID == UniversityStudent_ID  && x.Document_ID == 3).Select(x => new
        //            {
        //                ImagePath = "../Content/UserFiles/" + x.UniversityStudents.Student_ID + "/المستندات/" + x.DocumentImage
        //            }).FirstOrDefault();
        //            if (StDocVisa != null)
        //            {
        //                byte[] binaryImage = (new WebClient()).DownloadData(Server.MapPath(StDocVisa.ImagePath));
        //                ImgVisa = string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(binaryImage));
        //            }

        //            var data = new
        //            {
        //                ImgIdentity,
        //                ImgPassport,
        //                ImgVisa
        //            };
        //            return Json(data, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //            return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //} 
        public ActionResult CheckIdentityNumber(string IdentityNumber, int StudentDocumentID, int StudentID, string IdentityImageName, string IdentityExpiryDate)
        {
            if (Session["UserId"] != null)
            {
                if (String.IsNullOrEmpty(IdentityNumber))
                {
                    return Json(new { msg = "رقم الهوية مطلوب", isValid = false }
                             , JsonRequestBehavior.AllowGet);
                }
                if (String.IsNullOrEmpty(IdentityExpiryDate))
                {
                    return Json(new { msg = "تاريخ إنتهاء الهوية مطلوب", isValid = false }
                             , JsonRequestBehavior.AllowGet);
                }

                if (String.IsNullOrEmpty(IdentityImageName))
                {
                    return Json(new { msg = " يرجي رفع صور للوثائق أولا", isValid = false }
                              , JsonRequestBehavior.AllowGet);
                }
                if (db.StudentDocuments.FirstOrDefault(x => x.StudentDocumentID == StudentDocumentID) != null)
                {
                    int UniversityStudent_ID = db.StudentDocuments.FirstOrDefault(x => x.StudentDocumentID == StudentDocumentID).UniversityStudent_ID;
                    var IdentityReapeated = db.StudentDocuments.Where(m => m.IdentityNumber == IdentityNumber && m.UniversityStudent_ID != UniversityStudent_ID).Count();
                    var RepeatedIdentity = db.StudentDocuments.Where(m => m.IdentityNumber == IdentityNumber && m.UniversityStudent_ID == UniversityStudent_ID
                      && m.StudentDocumentID != StudentDocumentID && m.IsExpired != true && (m.IsTransfer == null || (m.IsTransfer == true && m.ApprovedDate == null && m.RefusedDate == null))).Count();
                    if (IdentityReapeated > 0 || RepeatedIdentity > 0)
                    {
                        return Json(new { msg = "  رقم الهوية موجود مسبقاً", isValid = false }
                          , JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { msg = "", isValid = true }
                        , JsonRequestBehavior.AllowGet);
                    }
                }
                else if (db.StudentDocuments.Where(x => x.IdentityNumber == IdentityNumber && x.StudentDocumentID != StudentDocumentID && x.IsExpired != true).Count() > 0)
                {
                    return Json(new { msg = "  رقم الهوية موجود مسبقاً", isValid = false }
                         , JsonRequestBehavior.AllowGet);
                }
                else if (db.StudentDocuments.Where(x => x.IsExpired != true && x.UniversityStudents.Student_ID == StudentID && x.Document_ID == 1 && StudentDocumentID == 0).Count() > 0)
                {
                    return Json(new
                    {
                        msg = "  رقم الهوية موجود مسبقاً",
                        isValid = false
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { msg = "", isValid = true }
                    , JsonRequestBehavior.AllowGet);
                }

            }
            return Json(null
                        , JsonRequestBehavior.AllowGet);
        }



        public ActionResult CheckPassportNumber(string PassportNumber, int StudentDocumentID, int StudentID, string PassportImageName, string PassportExpiryDate)
        {
            if (Session["UserId"] != null)
            {
                if (String.IsNullOrEmpty(PassportExpiryDate))
                {
                    return Json(new { msg = "تاريخ إنتهاء جواز السفر مطلوب", isValid = false }
                             , JsonRequestBehavior.AllowGet);
                }
                if (String.IsNullOrEmpty(PassportNumber))
                {
                    return Json(new { msg = "رقم جواز السفر مطلوب", isValid = false }
                             , JsonRequestBehavior.AllowGet);
                }
                if (String.IsNullOrEmpty(PassportNumber))
                {
                    return Json(new { msg = "رقم جواز السفر مطلوب", isValid = false }
                             , JsonRequestBehavior.AllowGet);
                }
                if (String.IsNullOrEmpty(PassportImageName))
                {
                    return Json(new { msg = " يرجي رفع صور للوثائق أولا", isValid = false }
                             , JsonRequestBehavior.AllowGet);
                }
                if (db.StudentDocuments.FirstOrDefault(x => x.StudentDocumentID == StudentDocumentID) != null)
                {
                    int UniversityStudent_ID = db.StudentDocuments.FirstOrDefault(x => x.StudentDocumentID == StudentDocumentID).UniversityStudent_ID;
                    var IdentityReapeated = db.StudentDocuments.Where(m => m.IdentityNumber == PassportNumber && m.UniversityStudent_ID != UniversityStudent_ID).Count();
                    var RepeatedIdentity = db.StudentDocuments.Where(m => m.IdentityNumber == PassportNumber && m.UniversityStudent_ID == UniversityStudent_ID
                   && m.StudentDocumentID != StudentDocumentID && m.IsExpired != true && (m.IsTransfer == null || (m.IsTransfer == true && m.ApprovedDate == null && m.RefusedDate == null))).Count();

                    if (IdentityReapeated > 0)
                    {
                        return Json(new { msg = "   رقم جواز السفر موجود مسبقاً", isValid = false }
                          , JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { msg = "", isValid = true }
                        , JsonRequestBehavior.AllowGet);
                    }
                }
                else if (db.StudentDocuments.Where(x => x.IdentityNumber == PassportNumber && x.StudentDocumentID != StudentDocumentID && x.IsExpired != true).Count() > 0)
                {
                    return Json(new
                    {
                        msg = "  رقم جواز السفر موجود مسبقاً",
                        isValid = false
                    }
                         ,
                        JsonRequestBehavior.AllowGet);
                }
                else if (db.StudentDocuments.Where(x => x.IsExpired != true && x.Document_ID == 2 && x.UniversityStudents.Student_ID == StudentID && StudentDocumentID == 0).Count() > 0


                   )
                {
                    return Json(new
                    {
                        msg = "  رقم جواز السفر موجود مسبقاً",
                        isValid = false
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { msg = "", isValid = true }
                    , JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }


        public ActionResult CheckData(string IDS)
        {
            if (Session["UserId"] != null)
            {
                if (!String.IsNullOrEmpty(IDS))
                {
                    var arr = IDS.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {

                        int doc = int.Parse(arr[i]);
                        var IdentityRow = db.StudentDocuments.Where(m => m.StudentDocumentID == doc && m.IsTransfer != true).FirstOrDefault();
                        string IdentityDocument = null;
                        if (IdentityRow != null)
                        {
                            IdentityDocument = IdentityRow?.DocumentImage;

                            if (!String.IsNullOrEmpty(IdentityDocument))
                            {
                                return Json(new { msg = "", isValid = true }
                                    , JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { msg = " يرجي رفع صور للوثائق أولا", isValid = false }
                                    , JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                }
            }
            return Json(new { msg = "يرجي إختيار الوثائق للإعتماد", isValid = false }
                             , JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SendData(string IDS)
        {
            if (Session["UserId"] != null)
            {

                if (!String.IsNullOrEmpty(IDS))
                {

                    var arr = IDS.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        int doc = int.Parse(arr[i]);
                        var Document = db.StudentDocuments.Where(m => m.StudentDocumentID == doc && m.IsTransfer != true).FirstOrDefault();
                        if (Document.DocumentImage != null)
                        {
                            Document.IsTransfer = true;
                            db.Entry(Document).State = EntityState.Modified;
                            db.SaveChanges();

                            return Json(true
                               , JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(false
                             , JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json("invaliddata", JsonRequestBehavior.AllowGet);

                }

            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }
        public ActionResult SaveIdentityData(StudentDocumentsIdentityVM model)
        {
            if (Session["UserId"] != null)
            {
                var user = Session["UserId"] as DashBoard_Users;
                int stID = db.UniversityStudents.Where(x => x.ID == model.IdentityUniversityStudent_ID).SingleOrDefault().Student_ID;
                StudentDocuments doc = null;
                string IdentityImageName = "";
                if (model != null)
                {
                    if (model.IdentityID == 0)
                    {
                        if (model.StudentDocumentID == 0 || model.StudentDocumentID == null)
                        {
                            //Doc Image
                            var files = BinaryImageEditExtension.GetValue<byte[]>("IdentityImageName");
                            if (files != null)
                            {
                                if (files.Count() > 0)
                                {
                                    string rootFolder = Server.MapPath("~/Content/UserFiles/");
                                    string filename = String.Format($"{model.IdentityUniversityStudent_ID}_doc_{model.Document_ID}_{DateTime.Now.ToString("ddMMyyyyhhmm")}.jpg");
                                    string studentFolder = System.IO.Path.Combine(rootFolder, stID.ToString() + "/المستندات/");

                                    if (uploadImage(files, studentFolder, filename))
                                    {
                                        IdentityImageName = filename;
                                    }
                                }
                            }
                            doc = new StudentDocuments()
                            {
                                Document_ID = 1,
                                UniversityStudent_ID = model.IdentityUniversityStudent_ID,
                                ExpiryDate = model.IdentityExpiryDate,
                                IdentityNumber = model.IdentityNumber,
                                IssueDate = DateTime.Now,
                                IsActive = false,
                                IsNew = true,
                                IsExpired = false,
                                InsertDate = DateTime.Now,
                                DocumentImage = IdentityImageName
                            };
                            db.StudentDocuments.Add(doc);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        doc = db.StudentDocuments.Find(model.IdentityID);
                        if (doc != null)
                        {
                            var files = BinaryImageEditExtension.GetValue<byte[]>("IdentityImageName");
                            if (files != null)
                            {
                                if (files.Count() > 0)
                                {
                                    string rootFolder = Server.MapPath("~/Content/UserFiles/");
                                    string filename = String.Format($"{model.IdentityUniversityStudent_ID}_doc_{doc.Document_ID}_{DateTime.Now.ToString("ddMMyyyyhhmm")}.jpg");
                                    string studentFolder = System.IO.Path.Combine(rootFolder, stID.ToString() + "/المستندات/");

                                    if (uploadImage(files, studentFolder, filename))
                                    {
                                        IdentityImageName = filename;
                                    }
                                }
                            }


                            doc.ExpiryDate = model.IdentityExpiryDate;
                            doc.IdentityNumber = model.IdentityNumber;
                            doc.IsActive = false;
                            doc.DocumentImage = IdentityImageName;
                            doc.RefusedDate = null;
                            db.SaveChanges();
                        }
                    }
                    Session["IdentityID"] = doc.StudentDocumentID;
                }
                return PartialView("_IdentityDocuments");
            }
            return PartialView("_IdentityDocuments");
        }
        public ActionResult SavePasportData(StudentDocumentsPasportVM model)
        {
            if (Session["UserId"] != null)
            {
                var user = Session["UserId"] as DashBoard_Users;
                int stID = db.UniversityStudents.Where(x => x.ID == model.PassportUniversityStudent_ID).SingleOrDefault().Student_ID;

                StudentDocuments doc = null;
                string IdentityImageName = "";
                if (model != null)
                {
                    if (model.PassportID == 0)
                    {
                        if (model.StudentDocumentID == 0 || model.StudentDocumentID == null)
                        {
                            //Doc Image
                            var files = BinaryImageEditExtension.GetValue<byte[]>("PassportImageName");
                            if (files != null)
                            {
                                if (files.Count() > 0)
                                {
                                    string rootFolder = Server.MapPath("~/Content/UserFiles/");
                                    string filename = String.Format($"{model.PassportUniversityStudent_ID}_doc_{model.Document_ID}_{DateTime.Now.ToString("ddMMyyyyhhmm")}.jpg");
                                    string studentFolder = System.IO.Path.Combine(rootFolder, stID.ToString() + "/المستندات/");

                                    if (uploadImage(files, studentFolder, filename))
                                    {
                                        IdentityImageName = filename;
                                    }
                                }
                            }
                            doc = new StudentDocuments()
                            {
                                Document_ID = 2,
                                UniversityStudent_ID = model.PassportUniversityStudent_ID,
                                ExpiryDate = model.PassportExpiryDate,
                                IdentityNumber = model.PassportNumber,
                                IssueDate = DateTime.Now,
                                ApprovedDate = null,
                                IsActive = false,
                                IsNew = true,
                                IsExpired = false,
                                InsertDate = DateTime.Now,
                                DocumentImage = IdentityImageName
                            };

                            db.StudentDocuments.Add(doc);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        doc = db.StudentDocuments.Find(model.PassportID);
                        if (doc != null)
                        {
                            var files = BinaryImageEditExtension.GetValue<byte[]>("PassportImageName");
                            if (files != null)
                            {
                                if (files.Count() > 0)
                                {
                                    string rootFolder = Server.MapPath("~/Content/UserFiles/");
                                    string filename = String.Format($"{model.PassportUniversityStudent_ID}_doc_{doc.Document_ID}_{DateTime.Now.ToString("ddMMyyyyhhmm")}.jpg");
                                    string studentFolder = System.IO.Path.Combine(rootFolder, stID.ToString() + "/المستندات/");

                                    if (uploadImage(files, studentFolder, filename))
                                    {
                                        IdentityImageName = filename;
                                    }
                                }
                            }

                            doc.ExpiryDate = model.PassportExpiryDate;
                            doc.IdentityNumber = model.PassportNumber;
                            doc.IsActive = false;
                            doc.DocumentImage = IdentityImageName;
                            doc.RefusedDate = null;
                            db.SaveChanges();
                        }
                    }
                    Session["PassportID"] = doc.StudentDocumentID;
                    return PartialView("_IdentityDocuments");
                }
            }
            return PartialView("_IdentityDocuments");
        }

        public ActionResult SaveVisaData(StudentDocumentsVisaVM model)
        {
            if (Session["UserId"] != null)
            {
                var user = Session["UserId"] as DashBoard_Users;
                int stID = db.UniversityStudents.Where(x => x.ID == model.VisaUniversityStudent_ID).SingleOrDefault().Student_ID;
                string IdentityImageName = "";
                if (model != null)
                {
                    if (model.VisaID == 0)
                    {
                        if (model.StudentDocumentID == 0 || model.StudentDocumentID == null)
                        {
                            //Doc Image
                            var files = BinaryImageEditExtension.GetValue<byte[]>("VisaImageName");
                            if (files != null)
                            {
                                if (files.Count() > 0)
                                {
                                    string rootFolder = Server.MapPath("~/Content/UserFiles/");
                                    string filename = String.Format($"{model.VisaUniversityStudent_ID}_doc_{model.Document_ID}_{DateTime.Now.ToString("ddMMyyyyhhmm")}.jpg");
                                    string studentFolder = System.IO.Path.Combine(rootFolder, stID.ToString() + "/المستندات/");

                                    if (uploadImage(files, studentFolder, filename))
                                    {
                                        IdentityImageName = filename;
                                    }
                                }
                            }

                            StudentDocuments doc = new StudentDocuments()
                            {
                                Document_ID = 3,
                                UniversityStudent_ID = model.VisaUniversityStudent_ID,
                                ExpiryDate = model.VisaExpiryDate,
                                IdentityNumber = model.VisaNumber,
                                IssueDate = DateTime.Now,
                                ApprovedDate = null,
                                IsActive = false,
                                IsNew = false,
                                InsertDate = DateTime.Now,
                                DocumentImage = IdentityImageName
                            };

                            db.StudentDocuments.Add(doc);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var docID = db.StudentDocuments.Find(model.VisaID);
                        if (docID != null)
                        {
                            docID.ExpiryDate = model.VisaExpiryDate;
                            docID.IdentityNumber = model.VisaNumber;
                            //Doc Image
                            var files = BinaryImageEditExtension.GetValue<byte[]>("VisaImageName");
                            if (files != null)
                            {
                                if (files.Count() > 0)
                                {
                                    string rootFolder = Server.MapPath("~/Content/UserFiles/");
                                    string filename = String.Format($"{model.VisaUniversityStudent_ID}_doc_{docID.Document_ID}_{DateTime.Now.ToString("ddMMyyyyhhmm")}.jpg");
                                    string studentFolder = System.IO.Path.Combine(rootFolder, stID.ToString() + "/المستندات/");

                                    if (uploadImage(files, studentFolder, filename))
                                    {
                                        docID.DocumentImage = filename;
                                    }
                                }
                            }
                            db.SaveChanges();
                        }
                        return PartialView("_IdentityDocuments");
                    }
                }
            }
            return PartialView("_IdentityDocuments");
        }

        #endregion

        public ActionResult DeleteStudentDocument(int? id)
        {
            int result = 0;
            if (id > 0)
            {
                var document = db.StudentDocuments.Where(x => x.StudentDocumentID == id).FirstOrDefault();
                if (document != null)
                {
                    try
                    {
                        db.StudentDocuments.Remove(document);
                        if (Directory.Exists(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/")))
                        {
                            if (Directory.Exists(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/المستندات/")))
                            {
                                if (System.IO.File.Exists(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/المستندات/{document.DocumentImage}")))
                                {
                                    System.IO.File.Delete(Server.MapPath($"~/UserFiles/{document.UniversityStudent_ID}/المستندات/{document.DocumentImage}"));
                                }
                            }

                        }
                        db.SaveChanges();
                        result = 1;
                    }
                    catch (Exception ex)
                    {


                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public static dynamic GetStDocuments()
        {
            var db = new SchoolAccGam3aEntities();
            return db.DocumentTypes.Select(x => new { x.DocumentID, x.DocumentName }).ToList();
        }
        #endregion
        #region imagesUpload
        public bool uploadImage(byte[] files, string rootPath, string filename)
        {
            DirectoryInfo currentFolder = new DirectoryInfo(rootPath);

            if (currentFolder.Exists == false)
            {
                System.IO.Directory.CreateDirectory(rootPath);
            }
            if (System.IO.File.Exists(rootPath + "/" + filename))
            {
                System.IO.File.Delete(rootPath + "/" + filename);
            }
            DirectorySecurity dSecurity = currentFolder.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            currentFolder.SetAccessControl(dSecurity);
            //var Code = file[i].FileCode.ToString().Split(',')[1].Trim();
            Byte[] bytes = files;




            FileStream fileStream = new FileStream(rootPath + "\\" + filename, FileMode.Create,
                FileAccess.ReadWrite);
            try
            {
                var ms = new MemoryStream(bytes);
                var image = Image.FromStream(ms);
                var format = image.RawFormat;
                //var halfQualityImage = Files.CompressImage(image, 90);
                //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }
        #endregion
        public Permissions GetPermissionsfn(int screenId)
        {

            var user = HttpContext.Session["UserId"] as DashBoard_Users;

            if (user != null)
            {


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

    public class studentSelection
    {
        public long STUDENT_ID { get; set; }
        public string STUDENT_NAME { get; set; }
    }
    public class studentHistory
    {
        public decimal Student_ID { get; set; }
        public string GraduateDate { get; set; }
        public string JoinDate { get; set; }
        public string Degree { get; set; }
        public string Name { get; set; }
        public decimal? Status_Code { get; set; }
        public string Status_Desc { get; set; }
    }

    //Student Documents VM
    public class StudentDocumentsIdentityVM
    {
        public int StudentDocumentID { get; set; }
        public int Document_ID { get; set; }
        public int IdentityID { get; set; }
        public int IdentityUniversityStudent_ID { get; set; }
        public string IdentityNumber { get; set; }
        public DateTime? IdentityExpiryDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public string IdentityImageName { get; set; }
        public bool? IsActive { get; set; } = false;
    }

    public class StudentDocumentsPasportVM
    {
        public int StudentDocumentID { get; set; }
        public int Document_ID { get; set; }
        public int PassportID { get; set; }
        public int PassportUniversityStudent_ID { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public string PassportImageName { get; set; }
        public bool? IsActive { get; set; } = false;
    }

    public class StudentDocumentsVisaVM
    {
        public int StudentDocumentID { get; set; }
        public int Document_ID { get; set; }
        public int VisaID { get; set; }
        public int VisaUniversityStudent_ID { get; set; }
        public string VisaNumber { get; set; }
        public DateTime? VisaExpiryDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public string VisaImageName { get; set; }
        public bool? IsActive { get; set; } = false;
    }


}