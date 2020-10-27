
using StudentProfile.DAL.Models;
using StudentProfile.DAL.Models.VM;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
//using System.IO.Compression;
using Ionic.Zip;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using System.Text;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace StudentProfile.Web.Controllers
{

    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public partial class StudentsController
    {
        static HRMadinaEntities hrContext = new HRMadinaEntities();

        public static int EmpID
        {
            get
            {
                var idnumber = System.Web.HttpContext.Current.Session["IdNumber"];
                var employee = hrContext.Employees.Where(x => x.IDNumber == idnumber.ToString()).FirstOrDefault();
                return employee.ID;
            }
        }
       [NonAction]
        private List<EmpCourseVM> getCoursesByEmpID(int EmpID)
        {
            var courses = hrContext.EmpCourses.Where(x => x.EmployeeID == EmpID).ToList();
            return courses.Select(x => x.ToViewModel()).ToList();
        }
        [NonAction]
        private List<CourseTypes> getCourseTypes()
        {
            var types = hrContext.CourseTypes.ToList();
            return types;
        }

        // GET: StudentsPartial
        public ActionResult Course(string idNumber)
        {
            int? empID = 0;
            if (idNumber == null)
            {
                return View("Index", $"Students");
            }
            //المساهمات و المشاركات
            var jsonCourseData = new JavaScriptSerializer().Serialize(GetPermissions(6).Data);
            var permissionsCourseData = JsonConvert.DeserializeObject<Permissions>(jsonCourseData);
            ViewBag.CourseDataRead = permissionsCourseData.Read;
            System.Web.HttpContext.Current.Session["IdNumber"] = idNumber;
            var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == idNumber);
            if (emp != null)
            {
                empID = emp.ID;
            }

            ViewBag.courseLst = getCoursesByEmpID(empID.Value);
            ViewBag.courseTypLst = new SelectList(getCourseTypes(), "ID", "Name");

            // HtmlHelper.ClientValidationEnabled = false;
            return PartialView();
        }

        [HttpPost]
        public ActionResult MultiSelectionImageUpload()
        {
            HomeControllerUploadControlSettings.uploadFiles = new Dictionary<string, byte[]>();
            Session["UploadedFiles"] = null;
            UploadControlExtension.GetUploadedFiles("UploadedImages",
                HomeControllerUploadControlSettings.UploadValidationSettings,
                HomeControllerUploadControlSettings.FileUploadComplete);
            return RedirectToAction("UploadedFiles");
            //return RedirectToAction("UploadedFiles");
            //return PartialView("_UploadedFilesContainer");
        }

        [HttpPost]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId =6)]
        public ActionResult PostCourse(EmpCourseVM cours)
        {
            if (cours.ID == 0)
                ModelState.Remove("ID");
            if (!ModelState.IsValid)
            {
                foreach (var x in ModelState)
                {
                    var errors = x.Value.Errors.ToList();
                    foreach (var error in errors)
                        ModelState.AddModelError(x.Key, new Exception(error.ErrorMessage));
                }

                //ModelState.AddModelError(string.Empty, "من فضلك تاكد من صحة بياناتك");
            }
            else
            {
                var course = cours.ID.Equals(0) ? null : hrContext.EmpCourses.Find(cours.ID);
                var IsNew = course == null;
                if (IsNew)
                {
                    course = new EmpCourses();
                    hrContext.EmpCourses.Add(course);
                }

                course.CourseDays = cours.CourseDays;
                course.CourseName = cours.CourseName;
                course.CoursePlace = cours.CoursePlace;
                course.Degree = cours.Degree.ToString();
                course.EndDate = cours.StartDate;
                course.StartDate = cours.StartDate;
                course.EmployeeID = EmpID;
                course.CourseTypes_ID = cours.CourseTypes_ID;
                course.DegreePercentage = cours.DegreePercentage;

                var files = (Dictionary<string, byte[]>) Session["UploadedFiles"];
                var imgPath = "";
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        imgPath += file.Key + ",";
                    }

                    if (imgPath.Substring(imgPath.Length - 1) == ",")
                        imgPath = imgPath.Remove(imgPath.LastIndexOf(','), 1);
                    if (course.CourseImagePath != imgPath)
                    {
                        if (course.CourseImagePath != null && course.CourseImagePath != "")
                        {
                            DeleteFile(course.CourseImagePath);
                        }

                        course.CourseImagePath = "";
                        foreach (var file in files)
                        {
                            course.CourseImagePath += UploadImage(file) + ",";
                        }
                    }
                }
                else
                {
                    course.CourseImagePath = "";
                }

                if (course.CourseImagePath != null && course.CourseImagePath != "")
                    if (course.CourseImagePath.Substring(course.CourseImagePath.Length - 1) == ",")
                        course.CourseImagePath =
                            course.CourseImagePath.Remove(course.CourseImagePath.LastIndexOf(','), 1);

                var res = hrContext.SaveChanges();
                if (res >= 0)
                {
                    ModelState.Clear();
                    Session["UploadedFiles"] = null;
                }
            }

            ViewBag.courseLst = getCoursesByEmpID(EmpID);
            ViewBag.courseTypLst = new SelectList(getCourseTypes(), "ID", "Name");

            return PartialView("_StDetails", "");
        }

        [HttpGet]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 6)]
        public ActionResult Delete(int ID)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "من فضلك تاكد من صحة بياناتك");
            }

            var course = hrContext.EmpCourses.Where(x => x.ID == ID).FirstOrDefault();
            if (course != null)
            {
                hrContext.EmpCourses.Remove(course);
                var res = hrContext.SaveChanges();
                if (res > 0)
                {
                    if (course.CourseImagePath != null)
                    {
                        DeleteFile(course.CourseImagePath);
                    }
                }
            }

            ViewBag.courseLst = getCoursesByEmpID(EmpID);
            ViewBag.courseTypLst = new SelectList(getCourseTypes(), "ID", "Name");

            return PartialView("_StDetails","");
        }

        [HttpGet]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 6)]
        public ActionResult Edit(int? ID)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "من فضلك تاكد من صحة بياناتك");
            }

            Session["UploadedFiles"] = null;
            var course = hrContext.EmpCourses.Where(x => x.ID == ID).FirstOrDefault();
            Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>();
            if (course.CourseImagePath != null && course.CourseImagePath != "")
            {
                var names = course.CourseImagePath.Split(',');
                foreach (var name in names)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Content/Course/") + name))
                    {
                        var content = System.IO.File.ReadAllBytes(Server.MapPath("/Content/Course/") + name);
                        dic.Add(name, content);
                    }
                }

                Session["UploadedFiles"] = dic;
            }

            //Session["UploadedFiles"] = course.CourseImagePath;
            ViewBag.courseLst = getCoursesByEmpID(EmpID);
            ViewBag.courseTypLst = new SelectList(getCourseTypes(), "ID", "Name");

            return PartialView("Course", course.ToViewModel());
        }
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 6)]
        public ActionResult UploadedFiles()
        {
            return PartialView("_UploadedFilesContainer");
        }

        void DeleteFile(string Names)
        {
            if (Names != "")
            {
                var fileNames = Names.Split(',');
                foreach (var name in fileNames)
                {
                    string filePath =
                        System.Web.HttpContext.Current.Server.MapPath(
                            "~/Content/UserFiles/{studentid}/المساهمات/" + name);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
            }
        }

        static string UploadImage(KeyValuePair<string, byte[]> file)
        {
            string[] names = file.Key.Split('.');
            var fileName = names[0] + "_" + Guid.NewGuid().ToString() + "." + names[1];

            var studentid = int.Parse(System.Web.HttpContext.Current.Session["StudentID"].ToString());

            var path = $"~/Content/UserFiles/{studentid}/المساهمات/";
            var destinationPath = System.Web.HttpContext.Current.Server.MapPath(path);

            var currentFolder = new DirectoryInfo(destinationPath);

            if (currentFolder.Exists == false)
            {
                Directory.CreateDirectory(destinationPath);
            }

            string filePath = System.Web.HttpContext.Current.Server.MapPath(path + fileName);
            System.IO.File.WriteAllBytes(filePath, file.Value);
            return fileName;
        }

        [HttpPost]
        public ActionResult DownloadImage(int? ID)
        {
            //////int CurrentFileID = Convert.ToInt32(FileID);  
            var filesCol = GetFile(ID).ToList();
            if (System.IO.File.Exists(Server.MapPath
                ("~/Content/" + ID + ".zip")))
            {
                System.IO.File.Delete(Server.MapPath
                    ("~/Content/" + ID + ".zip"));
            }

            ZipArchive zipPth = System.IO.Compression.ZipFile.Open(Server.MapPath
                ("~/Content/" + ID + ".zip"), ZipArchiveMode.Create);
            foreach (var file in filesCol)
            {
                zipPth.CreateEntryFromFile(Server.MapPath
                    ("~/Content/Course/" + file.FileName), file.FileName);
            }

            zipPth.Dispose();
            return Content("/Content/" + ID + ".zip");
            //using (var memoryStream = new MemoryStream())
            //{
            //    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            //    {
            //        for (int i = 0; i < filesCol.Count; i++)
            //        {
            //            ziparchive.CreateEntryFromFile(filesCol[i].FilePath, filesCol[i].FileName);
            //        }
            //    }
            //    //var cd = new System.Net.Mime.ContentDisposition
            //    //{
            //    //    FileName = "Attachments.zip",
            //    //    Inline = false
            //    //};
            //    //var name = cd.ToString();
            //    //Response.AppendHeader("Content-Disposition", name);

            //    var encoding = System.Text.Encoding.UTF8;
            //    Response.Charset = encoding.WebName;
            //    Response.HeaderEncoding = encoding;

            //    Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", (Request.Browser.Browser == "IE") ? HttpUtility.UrlEncode("Attachments.zip", encoding) : "Attachments.zip"));


            //    return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
        }


        [HttpGet]
        public ActionResult Clear()
        {
            this.ModelState.Clear();
            Session["UploadedFiles"] = null;
            //ViewBag.courseLst = getCoursesByEmpID(EmpID);
            //ViewBag.courseTypLst = new SelectList(getCourseTypes(), "ID", "Name");
            return RedirectToAction("Course", new {IdNumber = System.Web.HttpContext.Current.Session["IdNumber"]});
        }

        public List<FileInfos> GetFile(int? ID)
        {
            List<FileInfos> listFiles = new List<FileInfos>();
            string fileSavePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Course");
            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);

            var course = hrContext.EmpCourses.Where(x => x.ID == ID).FirstOrDefault();
            string[] fileNames = null;
            if (course.CourseImagePath != null || course.CourseImagePath != "")
            {
                fileNames = course.CourseImagePath.Split(',');
            }

            if (fileNames != null)
            {
                int i = 0;
                foreach (var name in fileNames)
                {
                    listFiles.Add(new FileInfos()
                    {
                        FileId = i + 1,
                        FileName = name,
                        FilePath = dirInfo.FullName + @"\" + name
                    });
                    i = i + 1;
                }
            }

            return listFiles;
        }

        [HttpPost]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 6)]
        public ActionResult AddNewCourseType(FormCollection frm)
        {
            string courseTypName = frm["courseTypName"];
            if (courseTypName != null || courseTypName != "" || courseTypName != string.Empty)
            {
                int? IsExist = hrContext.CourseTypes.Where(x => x.Name == courseTypName).Count();
                if (IsExist == 0)
                {
                    CourseTypes model = new CourseTypes();
                    model.Name = courseTypName;
                    hrContext.CourseTypes.Add(model);
                    hrContext.SaveChanges();
                }
            }

            ViewBag.courseLst = getCoursesByEmpID(EmpID);
            ViewBag.courseTypLst = new SelectList(getCourseTypes(), "ID", "Name");
            return PartialView("Course");
        }
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 6)]
        public ActionResult DeleteCourseType(int? id)
        {
            if (id != null && id != 0)
            {
                bool? IsExist = hrContext.EmpCourses.Any(x => x.CourseTypes_ID == id);
                if (IsExist == false)
                {
                    CourseTypes model = hrContext.CourseTypes.Where(x => x.ID == id).FirstOrDefault();
                    hrContext.CourseTypes.Remove(model);
                    hrContext.SaveChanges();
                }
            }

            ViewBag.courseLst = getCoursesByEmpID(EmpID);
            ViewBag.courseTypLst = new SelectList(getCourseTypes(), "ID", "Name");
            return PartialView("Course");
        }
    }

    public class HomeControllerUploadControlSettings
    {
        public static Dictionary<string, byte[]> uploadFiles { get; set; }

        public static DevExpress.Web.UploadControlValidationSettings UploadValidationSettings =
            new DevExpress.Web.UploadControlValidationSettings()
            {
                AllowedFileExtensions = new string[]
                    {".jpg", ".jpeg", ".xls", ".jfif", ".pdf", ".doc", ".docx", ".xlsx", ".png", ".gif"},
                MaxFileSize = 4000000
            };

        public static void FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                if (!uploadFiles.ContainsKey(e.UploadedFile.FileName))
                    // Save uploaded file to some location
                    uploadFiles.Add(e.UploadedFile.FileName, e.UploadedFile.FileBytes);
                HttpContext.Current.Session["UploadedFiles"] = uploadFiles;
                //HttpContext.Current.Session["UploadedFiles"] += UploadImage(e.UploadedFile) + ",";
            }
        }
    }
}