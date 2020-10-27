using DevExpress.Utils.Extensions;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Newtonsoft.Json;
using StudentProfile.DAL.Models;
using StudentProfile.DAL.Models.VM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace StudentProfile.Web.Controllers
{
    public partial class StudentsController
    {
        #region مستندات الطالب

        public ActionResult EmpDocument()
        {
            GetEmpDocumentPermssionsViewBags();

            return View();
        }

        private void GetEmpDocumentPermssionsViewBags()
        {
            //مستندات الطالب
            var permissionsEmpDocumentData = (Permissions)GetPermissions(5).Data;
            //var permissionsEmpDocumentData = JsonConvert.DeserializeObject<Permissions>(jsonEmpDocumentData);
            ViewBag.EmpDocumentDataCreate = permissionsEmpDocumentData.Create;
            ViewBag.EmpDocumentDataRead = permissionsEmpDocumentData.Read;
            ViewBag.EmpDocumentDataUpdate = permissionsEmpDocumentData.Update;
            ViewBag.EmpDocumentDataDelete = permissionsEmpDocumentData.Delete;
            ViewBag.EmpDocumentDataSave = permissionsEmpDocumentData.Save;
            ViewBag.EmpDocumentDataDownloadAttachments = permissionsEmpDocumentData.DownloadAttachments;
            ViewBag.EmpDocumentDataPreviewAttachments = permissionsEmpDocumentData.PreviewAttachments;
        }

        #region FTP


        #endregion
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 5)]
        public ActionResult DownloadEmpDocumentImage(int empDocumentId)
        {


            var studentid = int.Parse(Session["StudentID"].ToString());
            var dashboardPath = Server.MapPath($"~/Content/UserFiles/{studentid}/المستندات/");
            var imageName = dbHR.EmpDocument.Find(empDocumentId)?.DocImagePath;
            var fileName = dashboardPath + imageName;
            var file = System.IO.File.ReadAllBytes(fileName);
            return File(file, "Image/jpeg", $"{imageName}");
        }

        [ValidateInput(false)]

        public ActionResult StudentDocumentsGridViewPartial(string idNumber)
        {
            GetEmpDocumentPermssionsViewBags();

            if (idNumber == null)
            {
                idNumber = Session["IdNumber"].ToString();
            }

            Session["idNumer"] = idNumber;
            var data = db.UniversityStudents.Where(x => x.National_ID == idNumber).FirstOrDefault()?.StudentDocuments;


            return PartialView("_StudentDocumentsGridViewPartial", data);
        }
        public static dynamic GetDocuments()
        {
            var dbHR = new HRMadinaEntities();
            return dbHR.Documents.Select(x => new { x.Name, x.ID }).ToList();
        }

        [ValidateInput(false)]

        public ActionResult EmpDocumentGridViewPartialAddNew(EmpDocument item)
        {
            GetEmpDocumentPermssionsViewBags();

            var idNumber = Session["idNumber"].ToString();
            ViewData["DocumentTypes"] = dbHR.Documents.Select(x => new { x.Name, x.ID }).ToList();

            var model = new List<EmpDocument>();
            var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == idNumber);
            if (emp == null)
            {
                return PartialView("_EmpDocumentGridViewPartial", model);
            }

            model = dbHR.EmpDocument.Where(x => x.EmpID == emp.ID).ToList();
            var imageBytes = EditorExtension.GetValue<byte[]>("UploadedDocImage");
            if (imageBytes != null)
            {
                try
                {
                    var ms = new MemoryStream(imageBytes);
                    var image = Image.FromStream(ms);
                    item.IsActive = true;
                    item.DocImagePath = string.Empty;
                    item.EmpID = emp.ID;
                    dbHR.EmpDocument.Add(item);
                    dbHR.SaveChanges();

                    var imagename = emp.ID + "_" + item.ID + "_" + "DocImage" + ".jpg";

                    var studentid = int.Parse(Session["StudentID"].ToString());

                    //var halfQualityImage = Files.CompressImage(image, 90);
                    //حفظ Dashboard
                    var dashboardPath = Server.MapPath($"~/Content/UserFiles/{studentid}/المستندات/");
                    var dashboardDirectoryInfo = new DirectoryInfo(dashboardPath);
                    if (dashboardDirectoryInfo.Exists == false)
                    {
                        Directory.CreateDirectory(dashboardPath);
                    }

                    //System.IO.File.WriteAllBytes(dashboardPath + imagename, halfQualityImage);
                    image.Save(dashboardPath + imagename, ImageFormat.Jpeg);
                    //حفظ HR

                    var hrPath = System.Configuration.ConfigurationManager.AppSettings["Hr.ImageUploadPath"];
                    var hrDirectoryInfo = new DirectoryInfo(hrPath);
                    if (hrDirectoryInfo.Exists == false)
                    {
                        Directory.CreateDirectory(hrPath);
                    }

                    image.Save(hrPath + imagename, ImageFormat.Jpeg);

                    item.DocImagePath = imagename;
                    dbHR.Entry(item).State = EntityState.Modified;
                    dbHR.SaveChanges();
                    model = dbHR.EmpDocument.Where(x => x.EmpID == emp.ID).ToList();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى" + e.Message;
                }
            }
            else
            {
                try
                {
                    item.DocImagePath = string.Empty;
                    item.IsActive = true;
                    item.EmpID = emp.ID;
                    dbHR.EmpDocument.Add(item);
                    dbHR.SaveChanges();
                    model = dbHR.EmpDocument.Where(x => x.EmpID == emp.ID).ToList();
                }
                catch (Exception)
                {
                    ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
                }
            }

            return PartialView("_EmpDocumentGridViewPartial", model.ToList());
        }

        #region FTP

        //FTP
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult EmpDocumentGridViewPartialAddNew(EmpDocument item)
        //{
        //    GetEmpDocumentPermssionsViewBags();

        //    var idNumber = Session["idNumber"].ToString();
        //    ViewData["DocumentTypes"] = dbHR.Documents.Select(x => new {x.Name, x.ID}).ToList();

        //    var model = new List<EmpDocument>();
        //    var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == idNumber);
        //    if (emp == null)
        //        return PartialView("_EmpDocumentGridViewPartial", model);

        //    model = dbHR.EmpDocument.Where(x => x.EmpID == emp.ID).ToList();
        //    var imageBytes = EditorExtension.GetValue<byte[]>("UploadedDocImage");
        //    if (imageBytes != null)
        //    {
        //        try
        //        {
        //            var ms = new MemoryStream(imageBytes);
        //            var image = Image.FromStream(ms);
        //            var format = image.RawFormat;
        //            //var ext = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == format.Guid)
        //            //    ?.FilenameExtension;
        //            //if (ext == null)
        //            //{
        //            //    ViewData["EditError"] = "من فضلك اختر مستند صحيح";
        //            //    return PartialView("_EmpDocumentGridViewPartial", model);
        //            //}

        //            //ext = ext.Substring(1, ext.Length - 1);
        //            //ext = ext.Replace("*", string.Empty);

        //            item.IsActive = true;
        //            item.DocImagePath = string.Empty;
        //            item.EmpID = emp.ID;
        //            dbHR.EmpDocument.Add(item);

        //            dbHR.SaveChanges();
        //            //var fullImagePath = @"C:/inetpub/wwwroot/HRMadina/images/HRImagesUploaded/" + imagename;
        //            //var fileStream = new FileStream(fullImagePath, FileMode.CreateNew, FileAccess.ReadWrite);
        //            //var halfQualityImage = Files.CompressImage(image, 90);
        //            //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
        //            //fileStream.Close();

        //            //var studentid = int.Parse(Session["StudentID"].ToString());
        //            const string ext = ".jpg";
        //            var datetime = DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss");
        //            var imagename = $"{emp.ID}_ {item.ID}_DocImage{ext}";

        //            //var filePathDirectory = $"UserFiles/{studentid}/المستندات/{imagename}";
        //            //var isSaved = SaveToFTP(halfQualityImage, filePathDirectory);
        //            //if (isSaved == false)
        //            //{
        //            //    ViewData["EditError"] = "حدث خطأ أثناء حفظ المرفقات برجاء المحاولة مرة أخرى";
        //            //}


        //            var halfQualityImage = Files.CompressImage(image, 90);
        //            var studentid = int.Parse(Session["StudentID"].ToString());
        //            var path = $"UserFiles/{studentid}/المستندات/{imagename}";
        //            var isSavedToFtp = SaveToFTP(halfQualityImage, path);
        //            if (!isSavedToFtp)
        //            {
        //                ViewData["EditError"] = "حدث خطأ أثناء رفع المرفقات، برجاء المحاولة مرة أخرى.";
        //            }

        //            //var destinationPath = Server.MapPath(filePathDirectory);
        //            //var currentFolder = new DirectoryInfo(destinationPath);

        //            //if (currentFolder.Exists == false)
        //            //{
        //            //    Directory.CreateDirectory(destinationPath);
        //            //}

        //            //var fullName = destinationPath + imagename;
        //            //fileStream = new FileStream(fullName, FileMode.CreateNew, FileAccess.ReadWrite);
        //            //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
        //            //fileStream.Close();

        //            item.DocImagePath = imagename;
        //            dbHR.Entry(item).State = EntityState.Modified;
        //            dbHR.SaveChanges();
        //            model = dbHR.EmpDocument.Where(x => x.EmpID == emp.ID).ToList();
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
        //        }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            item.DocImagePath = string.Empty;
        //            item.IsActive = true;
        //            item.EmpID = emp.ID;
        //            dbHR.EmpDocument.Add(item);
        //            dbHR.SaveChanges();
        //            model = dbHR.EmpDocument.Where(x => x.EmpID == emp.ID).ToList();
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
        //        }
        //    }

        //    return PartialView("_EmpDocumentGridViewPartial", model.ToList());
        //}

        #endregion

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EmpDocumentGridViewPartialUpdate(EmpDocument item)
        {
            GetEmpDocumentPermssionsViewBags();
            var idNumber = Session["idNumber"];
            ViewData["DocumentTypes"] = dbHR.Documents.Select(x => new { x.Name, x.ID }).ToList();

            var model = new List<EmpDocument>();

            if (idNumber == null)
            {
                ViewData["EditError"] = "من فضلك اختر طالب";
                return PartialView("_EmpDocumentGridViewPartial", model);
            }

            var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == (string)idNumber);
            if (emp == null)
            {
                return PartialView("_EmpDocumentGridViewPartial", model);
            }

            try
            {
                var modelItem = dbHR.EmpDocument.FirstOrDefault(it => it.ID == item.ID);
                if (modelItem != null)
                {
                    var imageBytes = EditorExtension.GetValue<byte[]>("UploadedDocImage");
                    if (imageBytes != null)
                    {
                        modelItem.DocImagePath = string.Empty;
                        dbHR.Entry(modelItem).State = EntityState.Modified;
                        dbHR.SaveChanges();

                        var ms = new MemoryStream(imageBytes);
                        var image = Image.FromStream(ms);

                        var studentid = int.Parse(Session["StudentID"].ToString());
                        //var halfQualityImage = Files.CompressImage(image, 90);
                        var imagename = emp.ID + "_" + item.ID + "_" + "DocImage" + ".jpg";

                        //حفظ Dashboard
                        var dashboardPath = Server.MapPath($"~/Content/UserFiles/{studentid}/المستندات/");
                        var dashboardDirectoryInfo = new DirectoryInfo(dashboardPath);
                        if (dashboardDirectoryInfo.Exists == false)
                        {
                            Directory.CreateDirectory(dashboardPath);
                        }

                        //System.IO.File.WriteAllBytes(dashboardPath + imagename, halfQualityImage);
                        image.Save(dashboardPath + imagename, ImageFormat.Jpeg);


                        //حفظ HR
                        var hrPath = System.Configuration.ConfigurationManager.AppSettings["Hr.ImageUploadPath"];
                        var hrDirectoryInfo = new DirectoryInfo(hrPath);
                        if (hrDirectoryInfo.Exists == false)
                        {
                            Directory.CreateDirectory(hrPath);
                        }

                        image.Save(hrPath + imagename, ImageFormat.Jpeg);

                        modelItem.ID = item.ID;
                        modelItem.EmpID = emp.ID;
                        modelItem.DocumentID = item.DocumentID;
                        modelItem.ExpDate = item.ExpDate;
                        modelItem.IdentityNum = item.IdentityNum;
                        modelItem.IssueDate = item.IssueDate;
                        modelItem.DocImagePath = imagename;
                        modelItem.IsActive = true;
                        dbHR.Entry(modelItem).State = EntityState.Modified;
                        dbHR.SaveChanges();
                    }
                    else
                    {
                        modelItem.DocImagePath = string.Empty;
                        modelItem.ID = item.ID;
                        modelItem.EmpID = emp.ID;
                        modelItem.DocumentID = item.DocumentID;
                        modelItem.ExpDate = item.ExpDate;
                        modelItem.IdentityNum = item.IdentityNum;
                        modelItem.IssueDate = item.IssueDate;
                        modelItem.IsActive = true;
                        dbHR.Entry(modelItem).State = EntityState.Modified;
                        dbHR.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى" + e.Message;
            }

            model = dbHR.EmpDocument.Where(x => x.EmpID == emp.ID).ToList();
            return PartialView("_EmpDocumentGridViewPartial", model.ToList());
        }

        #region FTP

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult EmpDocumentGridViewPartialUpdate(EmpDocument item)
        //{
        //    GetEmpDocumentPermssionsViewBags();

        //    var idNumber = Session["idNumber"];
        //    ViewData["DocumentTypes"] = dbHR.Documents.Select(x => new {x.Name, x.ID}).ToList();

        //    var model = new List<EmpDocument>();

        //    if (idNumber == null)
        //    {
        //        ViewData["EditError"] = "من فضلك اختر طالب";
        //        return PartialView("_EmpDocumentGridViewPartial", model);
        //    }

        //    var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == (string) idNumber);
        //    if (emp == null) return PartialView("_EmpDocumentGridViewPartial", model);
        //    try
        //    {
        //        var modelItem = dbHR.EmpDocument.FirstOrDefault(it => it.ID == item.ID);
        //        if (modelItem != null)
        //        {
        //            var imageBytes = EditorExtension.GetValue<byte[]>("UploadedDocImage");
        //            if (imageBytes != null)
        //            {
        //                //modelItem.DocImagePath = string.Empty;
        //                //dbHR.Entry(modelItem).State = EntityState.Modified;
        //                //dbHR.SaveChanges();
        //                var ms = new MemoryStream(imageBytes);
        //                var image = Image.FromStream(ms);
        //                //var format = image.RawFormat;//var ext = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == format.Guid)
        //                //    ?.FilenameExtension;
        //                //if (ext == null)
        //                //{
        //                //    ViewData["EditError"] = "من فضلك، اختر صورة بالامتداد الصحيح";
        //                //    return PartialView("_EmpDocumentGridViewPartial", model);
        //                //}

        //                //ext = ext.Substring(1, ext.Length - 1);
        //                //ext = ext.Replace("*", string.Empty);

        //                const string ext = ".jpg";
        //                var datetime = DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss");
        //                var imagename = $"{emp.ID}_ {item.ID}_DocImage{ext}";
        //                //var fullImagePath = @"C:/inetpub/wwwroot/HRMadina/images/HRImagesUploaded/" + imagename;
        //                //var fileStream = new FileStream(fullImagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
        //                //    FileShare.None);
        //                //var halfQualityImage = Files.CompressImage(image, 90);
        //                //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
        //                //fileStream.Close();


        //                //var studentid = int.Parse(Session["StudentID"].ToString());
        //                //var filePathDirectory = $"~/Content/UserFiles/{studentid}/المستندات/";
        //                //var destinationPath = Server.MapPath(filePathDirectory);
        //                //var currentFolder = new DirectoryInfo(destinationPath);

        //                //if (currentFolder.Exists == false)
        //                //{
        //                //    Directory.CreateDirectory(destinationPath);
        //                //}

        //                //var fullName = destinationPath + imagename;
        //                //fileStream = new FileStream(fullName, FileMode.CreateNew, FileAccess.ReadWrite);
        //                //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
        //                //fileStream.Close();
        //                var studentid = int.Parse(Session["StudentID"].ToString());

        //                //if (!string.IsNullOrEmpty(modelItem.DocImagePath))
        //                //{
        //                //    if (modelItem.DocImagePath != imagename)
        //                //    {
        //                //        //بحيث لو كان عنده صورة وحب يحدث الصورة يحذف الصورة القديمة
        //                //        var oldImage = $"UserFiles/{studentid}/المستندات/{modelItem.DocImagePath}";
        //                //        DeleteFromFTP(oldImage);
        //                //    }
        //                //}

        //                var halfQualityImage = Files.CompressImage(image, 90);
        //                var path = $"UserFiles/{studentid}/المستندات/{imagename}";
        //                var isSavedToFtp = SaveToFTP(halfQualityImage, path);
        //                if (!isSavedToFtp)
        //                {
        //                    ViewData["EditError"] = "حدث خطأ أثناء رفع المرفقات، برجاء المحاولة مرة أخرى.";
        //                }

        //                modelItem.ID = item.ID;
        //                modelItem.EmpID = emp.ID;
        //                modelItem.DocumentID = item.DocumentID;
        //                modelItem.ExpDate = item.ExpDate;
        //                modelItem.IdentityNum = item.IdentityNum;
        //                modelItem.IssueDate = item.IssueDate;
        //                modelItem.DocImagePath = imagename;
        //                modelItem.IsActive = true;
        //                dbHR.Entry(modelItem).State = EntityState.Modified;
        //                dbHR.SaveChanges();
        //            }
        //            else
        //            {
        //                modelItem.DocImagePath = string.Empty;
        //                modelItem.ID = item.ID;
        //                modelItem.EmpID = emp.ID;
        //                modelItem.DocumentID = item.DocumentID;
        //                modelItem.ExpDate = item.ExpDate;
        //                modelItem.IdentityNum = item.IdentityNum;
        //                modelItem.IssueDate = item.IssueDate;
        //                modelItem.IsActive = true;
        //                dbHR.Entry(modelItem).State = EntityState.Modified;
        //                dbHR.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
        //    }

        //    model = dbHR.EmpDocument.Where(x => x.EmpID == emp.ID).ToList();
        //    return PartialView("_EmpDocumentGridViewPartial", model.ToList());
        //}

        #endregion

        #region FTP

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult EmpDocumentGridViewPartialDelete(int id)
        //{
        //    try
        //    {
        //        var empDocument = dbHR.EmpDocument.FirstOrDefault(x => x.ID == id);

        //        if (empDocument != null)
        //        {
        //            var studentid = int.Parse(Session["StudentID"].ToString());
        //            //var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/المستندات/");

        //            ////حذف الملفات للمستندات
        //            //var filExists = System.IO.File.Exists(tempFolder + empDocument.DocImagePath);
        //            //if (filExists)
        //            //{
        //            //    System.IO.File.Delete(tempFolder + empDocument.DocImagePath);
        //            //}

        //            var path = $"UserFiles/{studentid}/المستندات/{empDocument.DocImagePath}";
        //            var isDeletedDeleteFromFtp = DeleteFromFTP(path);
        //            if (!isDeletedDeleteFromFtp)
        //            {
        //                return Json("حدث خطأ أثناء حذف المرفقات، برجاء المحاولة مرة أخرى",
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            dbHR.EmpDocument.Remove(empDocument);
        //            dbHR.SaveChanges();
        //        }
        //        else
        //        {
        //            return Json("لا يوجد مستند بهذه البيانات", JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Json("حدث خطأ اثناء الحذف", JsonRequestBehavior.AllowGet);
        //    }

        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        #endregion

        [HttpPost]
        [ValidateInput(false)]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 5)]
        public ActionResult EmpDocumentGridViewPartialDelete(int id)
        {
            try
            {
                var empDocument = dbHR.EmpDocument.FirstOrDefault(x => x.ID == id);

                if (empDocument != null)
                {
                    var studentid = int.Parse(Session["StudentID"].ToString());

                    //حذف Dashboard
                    var dashboardPath = Server.MapPath($"~/Content/UserFiles/{studentid}/المستندات/");
                    var dashboardExists = System.IO.File.Exists(dashboardPath + empDocument.DocImagePath);
                    if (dashboardExists)
                    {
                        System.IO.File.Delete(dashboardPath + empDocument.DocImagePath);
                    }

                    //حذف من الـ HR
                    var hrPath = System.Configuration.ConfigurationManager.AppSettings["Hr.DocUploadPath"];
                    var hrExists = System.IO.File.Exists(hrPath + empDocument.DocImagePath);
                    if (hrExists)
                    {
                        System.IO.File.Delete(hrPath + empDocument.DocImagePath);
                    }

                    dbHR.EmpDocument.Remove(empDocument);
                    dbHR.SaveChanges();
                }
                else
                {
                    return Json("لا يوجد مستند بهذه البيانات", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("حدث خطأ اثناء الحذف", JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region بيانات المرافقين

        public ActionResult Relatives()
        {
            GetRelativesPermissionsViewBags();

            return View();
        }

        #region FTP

        //[ValidateInput(false)]
        //public ActionResult DownloadRelativeImage(int relativeId)
        //{
        //    var relative = dbHR.Relatives.SingleOrDefault(x => x.ID == relativeId);
        //    if (relative == null)
        //    {
        //        return null;
        //    }

        //    var employee = dbHR.Employees.SingleOrDefault(x => x.ID == relative.EmpID);
        //    if (employee == null)
        //    {
        //        return null;
        //    }

        //    var path = $"/UserFiles/{employee.empFileNum}/المرافقين/{relative.PassImage}";
        //    var file = new StudentsController().DownloadFromFTP(path);
        //    return file != null ? File(file, "Image/jpeg", $"{relative.PassImage}") : null;
        //}

        #endregion

        [ValidateInput(false)]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 4)]
        public ActionResult DownloadRelativeImage(int relativeId)
        {
            var studentid = int.Parse(Session["StudentID"].ToString());
            var dashboardPath = Server.MapPath($"~/Content/UserFiles/{studentid}/المرافقين/");
            var imageName = dbHR.Relatives.Find(relativeId)?.PassImage;
            var fileName = dashboardPath + imageName;
            var file = System.IO.File.ReadAllBytes(fileName);
            return File(file, "Image/jpeg", $"{imageName}");
        }

        [ValidateInput(false)]
        public ActionResult RelativesGridViewPartial(string idNumber)
        {
            GetRelativesPermissionsViewBags();
            var relationships = new List<string> { "اب", "ام", "اخ", "اخت", "ابن", "ابنه", "زوجة", "زوج" };
            ViewData["Relationship"] = relationships;
            var model = new List<Relatives>();
            if (string.IsNullOrEmpty(idNumber))
            {
                idNumber = Session["idNumber"].ToString();
            }

            var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == idNumber);
            if (emp == null)
            {
                return PartialView("_RelativesGridViewPartial", model);
            }

            model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();
            return PartialView("_RelativesGridViewPartial", model);
        }

        private void GetRelativesPermissionsViewBags()
        {
            //بيانات المرافقين
            var jsonRelativesData = new JavaScriptSerializer().Serialize(GetPermissions(4).Data);
            var permissionsRelativesData = JsonConvert.DeserializeObject<Permissions>(jsonRelativesData);
            ViewBag.RelativesDataRead = permissionsRelativesData.Read;
            ViewBag.RelativesDataCreate = permissionsRelativesData.Create;
            ViewBag.RelativesDataUpdate = permissionsRelativesData.Update;
            ViewBag.RelativesDataDelete = permissionsRelativesData.Delete;
            ViewBag.RelativesDataSave = permissionsRelativesData.Save;
            ViewBag.RelativesDataDownloadAttachments = permissionsRelativesData.DownloadAttachments;
            ViewBag.RelativesDataPreviewAttachments = permissionsRelativesData.PreviewAttachments;
        }

        #region FTP

        //[HttpPost]
        ////[ValidateInput(false)]
        //public ActionResult RelativesGridViewPartialAddNew(Relatives relative)
        //{
        //    GetRelativesPermissionsViewBags();

        //    var relationships = new List<string> {"اب", "ام", "اخ", "اخت", "ابن", "ابنه", "زوجة", "زوج"};
        //    ViewData["Relationship"] = relationships;

        //    string idNumber = Session["idNumber"].ToString();
        //    var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == idNumber);
        //    if (emp == null) return PartialView("_RelativesGridViewPartial");
        //    var model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();
        //    {
        //        relative.EmpID = emp.ID;
        //        relative.IDNumber = idNumber;
        //        var imageBytes = EditorExtension.GetValue<byte[]>("UploadedPassImage");
        //        if (imageBytes != null)
        //        {
        //            try
        //            {
        //                relative.PassImage = string.Empty;
        //                dbHR.Relatives.Add(relative);
        //                dbHR.SaveChanges();
        //                var ms = new MemoryStream(imageBytes);
        //                var image = Image.FromStream(ms);
        //                //var format = image.RawFormat;
        //                //var halfQualityImage = Files.CompressImage(image, 90);
        //                //var ext = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == format.Guid)
        //                //    ?.FilenameExtension;
        //                //if (ext == null)
        //                //{
        //                //    ViewData["EditError"] = "من فضلك اختر صورة من الامتدادت المدعومة";
        //                //    return PartialView("_RelativesGridViewPartial", model);
        //                //}

        //                //ext = ext.Substring(1, ext.Length - 1);
        //                //ext = ext.Replace("*", string.Empty);
        //                // const string ext = ".jpg";
        //                //var datetime = DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss");
        //                //var imagename = $"{emp.ID}_ {relative.ID}_{relative.DocumentID}_{datetime}_DocImage{ext}";
        //                //var imagename = emp.ID + "_" + relative.ID + "_" + "Passport" + ext;
        //                const string ext = ".jpg";

        //                var datetime = DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss");
        //                var imagename = $"{emp.ID}_ {relative.ID}_Passport{ext}";


        //                var halfQualityImage = Files.CompressImage(image, 90);
        //                var studentid = int.Parse(Session["StudentID"].ToString());
        //                var path = $"UserFiles/{studentid}/المرافقين/{imagename}";
        //                var isSavedToFtp = SaveToFTP(halfQualityImage, path);
        //                if (!isSavedToFtp)
        //                {
        //                    ViewData["EditError"] = "حدث خطأ أثناء رفع المرفقات، برجاء المحاولة مرة أخرى.";
        //                }

        //                relative.PassImage = imagename;

        //                //var filePathDirectory = $"~/Content/UserFiles/{studentid}/المرافقين/";
        //                //var destinationPath = Server.MapPath(filePathDirectory);
        //                //var currentFolder = new DirectoryInfo(destinationPath);

        //                //if (currentFolder.Exists == false)
        //                //{
        //                //    Directory.CreateDirectory(destinationPath);
        //                //}

        //                //var fullName = destinationPath + imagename;
        //                //var fileStream = new FileStream(fullName, FileMode.CreateNew, FileAccess.ReadWrite);
        //                //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
        //                //fileStream.Close();

        //                //var fullImagePath = @"C:/inetpub/wwwroot/HRMadina/images/DocImages/" + imagename;
        //                //fileStream = new FileStream(fullImagePath, FileMode.CreateNew, FileAccess.ReadWrite);
        //                //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
        //                //fileStream.Close();

        //                dbHR.Entry(relative).State = EntityState.Modified;
        //                dbHR.SaveChanges();

        //                model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();
        //            }
        //            catch (Exception e)
        //            {
        //                ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
        //            }

        //            return PartialView("_RelativesGridViewPartial", model);
        //        }

        //        try
        //        {
        //            relative.PassImage = string.Empty;
        //            dbHR.Relatives.Add(relative);
        //            dbHR.SaveChanges();

        //            model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
        //        }

        //        return PartialView("_RelativesGridViewPartial", model);
        //    }
        //}

        #endregion


        [HttpPost]
        //[ValidateInput(false)]
        public ActionResult RelativesGridViewPartialAddNew(Relatives relative)
        {
            GetRelativesPermissionsViewBags();

            var relationships = new List<string> { "اب", "ام", "اخ", "اخت", "ابن", "ابنه", "زوجة", "زوج" };
            ViewData["Relationship"] = relationships;

            string idNumber = Session["idNumber"].ToString();
            var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == idNumber);
            if (emp == null)
            {
                return PartialView("_RelativesGridViewPartial");
            }

            var model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();
            {
                relative.EmpID = emp.ID;
                relative.IDNumber = idNumber;
                var imageBytes = EditorExtension.GetValue<byte[]>("UploadedPassImage");
                if (imageBytes != null)
                {
                    try
                    {
                        relative.PassImage = string.Empty;
                        dbHR.Relatives.Add(relative);
                        dbHR.SaveChanges();
                        var ms = new MemoryStream(imageBytes);
                        var image = Image.FromStream(ms);
                        //var halfQualityImage = Files.CompressImage(image, 90);
                        var imagename = emp.ID + "_" + relative.ID + "_" + "Passport" + ".jpg";
                        relative.PassImage = imagename;
                        var studentid = int.Parse(Session["StudentID"].ToString());
                        var dashboardPath = Server.MapPath($"~/Content/UserFiles/{studentid}/المرافقين/");
                        var dashboardDirectoryInfo = new DirectoryInfo(dashboardPath);
                        if (dashboardDirectoryInfo.Exists == false)
                        {
                            Directory.CreateDirectory(dashboardPath);
                        }

                        var fullName = dashboardPath + imagename;
                        //System.IO.File.WriteAllBytes(fullName, halfQualityImage);
                        image.Save(fullName, ImageFormat.Jpeg);


                        var hrPath = System.Configuration.ConfigurationManager.AppSettings["Hr.DocUploadPath"];
                        var hrDirectoryInfo = new DirectoryInfo(hrPath);
                        if (hrDirectoryInfo.Exists == false)
                        {
                            Directory.CreateDirectory(hrPath);
                        }

                        image.Save(fullName, ImageFormat.Jpeg);

                        dbHR.Entry(relative).State = EntityState.Modified;
                        dbHR.SaveChanges();
                        model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();
                    }
                    catch (Exception)
                    {
                        ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
                    }

                    return PartialView("_RelativesGridViewPartial", model);
                }

                try
                {
                    relative.PassImage = string.Empty;
                    dbHR.Relatives.Add(relative);
                    dbHR.SaveChanges();

                    model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();
                }
                catch (Exception)
                {
                    ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
                }

                return PartialView("_RelativesGridViewPartial", model);
            }
        }


        #region FTP

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult RelativesGridViewPartialUpdate(Relatives relative)
        //{
        //    GetRelativesPermissionsViewBags();


        //    var relationships = new List<string> {"اب", "ام", "اخ", "اخت", "ابن", "ابنه", "زوجة", "زوج"};
        //    ViewData["Relationship"] = relationships;
        //    var model = new List<Relatives>();
        //    var idNumber = Session["IdNumber"];
        //    var relativeModel = dbHR.Relatives.SingleOrDefault(x => x.ID == relative.ID);
        //    if (idNumber == null)
        //        return PartialView("_RelativesGridViewPartial", model);
        //    var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == (string) idNumber);
        //    if (emp == null)
        //        return PartialView("_RelativesGridViewPartial", model);
        //    model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();

        //    if (relative != null)
        //    {
        //        relative.EmpID = emp.ID;
        //        relative.IDNumber = idNumber.ToString();
        //        if (relativeModel != null)
        //        {
        //            relativeModel.EmpID = emp.ID;
        //            relative.IDNumber = idNumber.ToString();
        //            {
        //                var imageBytes = EditorExtension.GetValue<byte[]>("UploadedPassImage");
        //                if (imageBytes != null)
        //                {
        //                    try
        //                    {
        //                        relative.PassImage = string.Empty;
        //                        dbHR.Entry(relativeModel).State = EntityState.Modified;
        //                        dbHR.SaveChanges();

        //                        var ms = new MemoryStream(imageBytes);
        //                        var image = Image.FromStream(ms);
        //                        //var format = image.RawFormat;
        //                        //var ext = ImageCodecInfo.GetImageEncoders()
        //                        //    .FirstOrDefault(x => x.FormatID == format.Guid)
        //                        //    ?.FilenameExtension;
        //                        //if (ext == null)
        //                        //    return PartialView("_RelativesGridViewPartial", model);
        //                        //ext = ext.Substring(1, ext.Length - 1);
        //                        //ext = ext.Replace("*", string.Empty);
        //                        //var imagename = emp.ID + "_" + relative.ID + "_" + "Passport" + ext;


        //                        //var fullImagePath = @"C:/inetpub/wwwroot/HRMadina/images/DocImages/" + imagename;
        //                        //var fileStream = new FileStream(fullImagePath, FileMode.OpenOrCreate,
        //                        //    FileAccess.ReadWrite,
        //                        //    FileShare.None);
        //                        //var halfQualityImage = Files.CompressImage(image, 90);
        //                        //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
        //                        //fileStream.Close();

        //                        //var studentid = int.Parse(Session["StudentID"].ToString());
        //                        //var filePathDirectory = $"~/Content/UserFiles/{studentid}/المرافقين/";
        //                        //var destinationPath = Server.MapPath(filePathDirectory);
        //                        //var currentFolder = new DirectoryInfo(destinationPath);

        //                        //if (currentFolder.Exists == false)
        //                        //{
        //                        //    Directory.CreateDirectory(destinationPath);
        //                        //}

        //                        //var fullName = destinationPath + imagename;
        //                        //fileStream = new FileStream(fullName, FileMode.CreateNew, FileAccess.ReadWrite);
        //                        //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
        //                        //fileStream.Close();
        //                        var studentid = int.Parse(Session["StudentID"].ToString());


        //                        const string ext = ".jpg";

        //                        var datetime = DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss");
        //                        var imagename = $"{emp.ID}_ {relative.ID}_Passport{ext}";
        //                        if (!string.IsNullOrEmpty(relativeModel.PassImage))
        //                        {
        //                            if (relativeModel.PassImage != imagename)
        //                            {
        //                                //بحيث لو كان عنده صورة وحب يحدث الصورة يحذف الصورة القديمة
        //                                var oldImage = $"UserFiles/{studentid}/المرافقين/{relativeModel.PassImage}";
        //                                DeleteFromFTP(oldImage);
        //                            }
        //                        }


        //                        var halfQualityImage = Files.CompressImage(image, 90);
        //                        var path = $"UserFiles/{studentid}/المرافقين/{imagename}";
        //                        var isSavedToFtp = SaveToFTP(halfQualityImage, path);
        //                        if (!isSavedToFtp)
        //                        {
        //                            ViewData["EditError"] = "حدث خطأ أثناء رفع المرفقات، برجاء المحاولة مرة أخرى.";
        //                        }

        //                        relativeModel.ID = relative.ID;
        //                        relativeModel.EmpID = relative.EmpID;
        //                        relativeModel.IDNumber = relative.IDNumber;
        //                        relativeModel.Name = relative.Name;
        //                        relativeModel.PassExpDate = relative.PassExpDate;
        //                        relativeModel.PassportNumber = relative.PassportNumber;
        //                        relativeModel.PassImage = imagename;
        //                        dbHR.Entry(relativeModel).State = EntityState.Modified;
        //                        dbHR.SaveChanges();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
        //                    }
        //                }
        //                else
        //                {
        //                    try
        //                    {
        //                        var imagename = string.Empty;
        //                        relativeModel.ID = relative.ID;
        //                        relativeModel.EmpID = relative.EmpID;
        //                        relativeModel.IDNumber = relative.IDNumber;
        //                        relativeModel.Name = relative.Name;
        //                        relativeModel.PassExpDate = relative.PassExpDate;
        //                        relativeModel.PassportNumber = relative.PassportNumber;
        //                        relativeModel.PassImage = imagename;
        //                        dbHR.Entry(relativeModel).State = EntityState.Modified;
        //                        dbHR.SaveChanges();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();
        //    return PartialView("_RelativesGridViewPartial", model);
        //}

        #endregion


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RelativesGridViewPartialUpdate(Relatives relative)
        {
            GetRelativesPermissionsViewBags();

            var relationships = new List<string> { "اب", "ام", "اخ", "اخت", "ابن", "ابنه", "زوجة", "زوج" };
            ViewData["Relationship"] = relationships;
            var model = new List<Relatives>();
            var idNumber = Session["IdNumber"];
            var relativeModel = dbHR.Relatives.SingleOrDefault(x => x.ID == relative.ID);
            if (idNumber == null)
            {
                return PartialView("_RelativesGridViewPartial", model);
            }

            var emp = dbHR.Employees.FirstOrDefault(x => x.IDNumber == (string)idNumber);
            if (emp == null)
            {
                return PartialView("_RelativesGridViewPartial", model);
            }

            model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();

            if (relative != null)
            {
                relative.EmpID = emp.ID;
                relative.IDNumber = idNumber.ToString();
                if (relativeModel != null)
                {
                    relativeModel.EmpID = emp.ID;
                    relative.IDNumber = idNumber.ToString();
                    {
                        var imageBytes = EditorExtension.GetValue<byte[]>("UploadedPassImage");
                        if (imageBytes != null)
                        {
                            try
                            {
                                relative.PassImage = string.Empty;
                                dbHR.Entry(relativeModel).State = EntityState.Modified;
                                dbHR.SaveChanges();

                                var ms = new MemoryStream(imageBytes);
                                var image = Image.FromStream(ms);
                                var imagename = emp.ID + "_" + relative.ID + "_" + "Passport" + ".jpg";
                                //var halfQualityImage = Files.CompressImage(image, 90);

                                //حفظ فى Dashboard
                                var studentid = int.Parse(Session["StudentID"].ToString());
                                var dashboardPath = Server.MapPath($"~/Content/UserFiles/{studentid}/المرافقين/");
                                var dashboardDirectoryInfo = new DirectoryInfo(dashboardPath);
                                if (dashboardDirectoryInfo.Exists == false)
                                {
                                    Directory.CreateDirectory(dashboardPath);
                                }

                                image.Save(dashboardPath + imagename, ImageFormat.Jpeg);

                                //حفظ فى HR
                                var hrPath = System.Configuration.ConfigurationManager.AppSettings["Hr.DocUploadPath"];
                                var hrDirectoryInfo = new DirectoryInfo(hrPath);
                                if (hrDirectoryInfo.Exists == false)
                                {
                                    Directory.CreateDirectory(hrPath);
                                }

                                image.Save(hrPath + imagename, ImageFormat.Jpeg);
                                relativeModel.ID = relative.ID;
                                relativeModel.EmpID = relative.EmpID;
                                relativeModel.IDNumber = relative.IDNumber;
                                relativeModel.Name = relative.Name;
                                relativeModel.PassExpDate = relative.PassExpDate;
                                relativeModel.PassportNumber = relative.PassportNumber;
                                relativeModel.PassImage = imagename;
                                dbHR.Entry(relativeModel).State = EntityState.Modified;
                                dbHR.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
                            }
                        }
                        else
                        {
                            //لم يتم رفع اى مرفقات
                            try
                            {
                                var imagename = string.Empty;
                                relativeModel.ID = relative.ID;
                                relativeModel.EmpID = relative.EmpID;
                                relativeModel.IDNumber = relative.IDNumber;
                                relativeModel.Name = relative.Name;
                                relativeModel.PassExpDate = relative.PassExpDate;
                                relativeModel.PassportNumber = relative.PassportNumber;
                                relativeModel.PassImage = imagename;
                                dbHR.Entry(relativeModel).State = EntityState.Modified;
                                dbHR.SaveChanges();
                            }
                            catch (Exception)
                            {
                                ViewData["EditError"] = "حدث خطأ اثناء الحفظ حاول تصحيح الاخطاء والمحاولة مرة أخرى";
                            }
                        }
                    }
                }
            }

            model = dbHR.Relatives.Where(x => x.EmpID == emp.ID).ToList();
            return PartialView("_RelativesGridViewPartial", model);
        }


        #region FTP

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult RelativesGridViewPartialDelete(int id)
        //{
        //    GetRelativesPermissionsViewBags();

        //    try
        //    {
        //        var relatives = dbHR.Relatives.FirstOrDefault(x => x.ID == id);

        //        if (relatives != null)
        //        {
        //            var studentid = int.Parse(Session["StudentID"].ToString());

        //            //var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/المرافقين/");

        //            ////حذف الملفات للمرافقين
        //            //var filExists = System.IO.File.Exists(tempFolder + relatives.PassImage);
        //            //if (filExists)
        //            //{
        //            //    System.IO.File.Delete(tempFolder + relatives.PassImage);
        //            //}

        //            ////حذف من الـ HR

        //            //const string hrImagePath = @"C:/inetpub/wwwroot/HRMadina/images/DocImages/";
        //            //var hrExists = System.IO.File.Exists(hrImagePath + relatives.PassImage);
        //            //if (hrExists)
        //            //{
        //            //    System.IO.File.Delete(hrImagePath + relatives.PassImage);
        //            //}
        //            var imagename = relatives.PassImage;
        //            if (!string.IsNullOrEmpty(imagename))
        //            {
        //                var path = $"UserFiles/{studentid}/المرافقين/{imagename}";
        //                var isDeletedFromFtp = DeleteFromFTP(path);
        //                if (!isDeletedFromFtp)
        //                {
        //                    return Json("حدث خطأ أثناء حذف المرفقات", JsonRequestBehavior.AllowGet);
        //                }
        //            }

        //            dbHR.Relatives.Remove(relatives);
        //            dbHR.SaveChanges();
        //            return Json("", JsonRequestBehavior.AllowGet);
        //        }

        //        return Json("لا يوجد مرافق بهذه البيانات", JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json("حدث خطأ اثناء الحذف", JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RelativesGridViewPartialDelete(int id)
        {
            GetRelativesPermissionsViewBags();
            try
            {
                var relatives = dbHR.Relatives.FirstOrDefault(x => x.ID == id);

                if (relatives != null)
                {
                    var studentid = int.Parse(Session["StudentID"].ToString());

                    var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/المرافقين/");

                    //حذف الملفات للمرافقين
                    var filExists = System.IO.File.Exists(tempFolder + relatives.PassImage);
                    if (filExists)
                    {
                        System.IO.File.Delete(tempFolder + relatives.PassImage);
                    }

                    //حذف من  HR
                    var hrPath = System.Configuration.ConfigurationManager.AppSettings["Hr.DocUploadPath"];
                    var hrExists = System.IO.File.Exists(hrPath + relatives.PassImage);
                    if (hrExists)
                    {
                        System.IO.File.Delete(hrPath + relatives.PassImage);
                    }

                    dbHR.Relatives.Remove(relatives);
                    dbHR.SaveChanges();
                    return Json("", JsonRequestBehavior.AllowGet);
                }

                return Json("لا يوجد مرافق بهذه البيانات", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("حدث خطأ اثناء الحذف", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult PassImageColumnPhotoUpdate()
        {
            return BinaryImageEditExtension.GetCallbackResult();
        }

        #endregion


        #region الماليات

        public ActionResult Finances(string idNumber)
        {



            var permissionsFinancesData = GetPermissionsfn(11);
            ViewBag.FinancesDataCreate = permissionsFinancesData.Create;
            ViewBag.FinancesDataRead = permissionsFinancesData.Read;
            ViewBag.FinancesDataUpdate = permissionsFinancesData.Update;
            ViewBag.FinancesDataDelete = permissionsFinancesData.Delete;
            ViewBag.FinancesDataSave = permissionsFinancesData.Save;


            var studentId = db.UniversityStudents.Where(x => x.National_ID == idNumber).FirstOrDefault()?.Student_ID;
            var advancesLst = db.AdvancePaymentDetails.Where(x => x.AdvancePaymentMaster.Student_Id == studentId).Select(adv => new
            {
                StudentId= adv.AdvanceRequests.Student_Id,
                Name = adv.AdvanceRequests.AdvanceSettings.AdvanceSettingName,
                ApprovedValue = adv.NetValue,
                adv.AdvancePaymentMaster.InsertionDate,
                Paid = adv.AdvanceReceiveDetails.Count() > 0 ? adv.AdvanceReceiveDetails.Sum(p => p.NetValue) : (decimal)0.00,
                AdvanceType = adv.AdvanceRequests.AdvanceSettings.AdvanceType,
                Attachements = adv.AdvanceRequests.AdvanceRequestsAttachment.Select(x => x.Path)
            }).ToList();
            var lst = new List<AdvancesVM>();

            lst.AddRange(advancesLst.Select(item => new AdvancesVM
            {
                StudentId = item.StudentId,
                Name = item.Name,
                date = item.InsertionDate.ToString("dd/MM/yyyy") ?? string.Empty,
                AdvanceValue = item.ApprovedValue,
                Paid = item.Paid,
                AdvanceType = item.AdvanceType == "S" ? "إعانة" : "سلفة",
                reset = item.AdvanceType == "S" ? (decimal)0.00 : item.ApprovedValue - item.Paid,
                Attachements = item.Attachements.ToArray()
            }));

            ViewBag.Advances = lst;

            return PartialView();
        }

        [HttpPost]
        public ActionResult DownloadAdvancesArchive(int studentId, string advanceRequestName, string pathStr)
        {

            string archicePath = $"/Content/{ studentId}_{advanceRequestName}.zip";
            try
            {
                string[] pathArr = pathStr.Split(',');
                if (System.IO.File.Exists(Server.MapPath
                    ($"~{archicePath}")))
                {
                    System.IO.File.Delete(Server.MapPath
                        ($"~{archicePath}"));
                }
                int emptyCount=0;
                using (ZipArchive zipPth = ZipFile.Open(Server.MapPath
                    ($"~/Content/{ studentId}_{advanceRequestName}.zip"), ZipArchiveMode.Create))
                {


                    for (int i = 0; i < pathArr.Length; i++)
                    {
                        if (System.IO.File.Exists(Server.MapPath
                        (pathArr[i])))
                        {
                            zipPth.CreateEntryFromFile(Server.MapPath
                               (pathArr[i]), pathArr[i].Split('/').LastOrDefault());
                            continue;
                        }
                        emptyCount++;
                    }
                }

                if(emptyCount == pathArr.Count())
                    return Content("0");
                return Content(archicePath);
            }
            catch (Exception)
            {
                return Content("");
            }
        }
        #endregion

        #region المساهمات والمشاركات

        public ActionResult _StudentIssues(string studentid)
        {

            var permissionsEmpDocumentData = GetPermissionsfn(5);
            ViewBag.EmpDocumentDataCreate = permissionsEmpDocumentData.Create;
            ViewBag.EmpDocumentDataRead = permissionsEmpDocumentData.Read;
            ViewBag.EmpDocumentDataUpdate = permissionsEmpDocumentData.Update;
            ViewBag.EmpDocumentDataDelete = permissionsEmpDocumentData.Delete;
            ViewBag.EmpDocumentDataSave = permissionsEmpDocumentData.Save;


            if (studentid == null)
            {
                studentid = Session["StudentId"].ToString();
            }

            Session["StudentId"] = studentid;
            INTEGRATION_All_Students student =
                db.INTEGRATION_All_Students.Where(x => x.STUDENT_ID.ToString() == studentid).FirstOrDefault();
            var studentissues = db.StudentNotes.FirstOrDefault(x => x.StDetailId == student.STUDENT_ID);
            ViewData["IssuesTypes"] = db.Issues.Select(x => new { x.IssueDescription, x.Id }).ToList();
            return PartialView("_StudentIssues", student);
        }

        public void GetFiles()
        {
            string[] errors;

            List<UploadedFile> uploaded = (List<UploadedFile>)Session["NoteFiles"];
            if (uploaded != null && uploaded.Count() > 0)
            {
                List<UploadedFile> files =
                    UploadControlExtension.GetUploadedFiles("myFile",
                        null, out errors, (s, e) => { },
                        UploadControl_FilesUploadComplete).ToList();
                if (files != null && files.Count() > 0)
                {
                    uploaded.ToList().AddRange(files);
                    Session["NoteFiles"] = uploaded;
                }
            }

            else
            {
                List<UploadedFile> files =
                    UploadControlExtension.GetUploadedFiles("myFile", null
                        , out errors, (s, e) => { },
                        UploadControl_FilesUploadComplete).ToList();
                if (files != null)
                {
                    Session["NoteFiles"] = files;
                }
                else
                {
                    Session["NoteFiles"] = new List<UploadedFile>();
                }
            }
        }


        #region FTP

        //[HttpPost]
        //public ActionResult DownloadStudentArchive(int? noteid, int? stID)
        //{
        //    var studentNote = db.StudentNotes.SingleOrDefault(x => x.NoteId == noteid);
        //    if (studentNote == null)
        //    {
        //        return null;
        //    }

        //    var noteFiles = db.StudentFiles.Where(x => x.StudentNoteId == noteid).ToList();
        //    if (noteFiles.Count <= 0)
        //    {
        //        return null;
        //    }

        //    var tempfolder = Server.MapPath("~/Content/tempfiles/FTP/" + stID + "/الملاحظات/");

        //    var exists = Directory.Exists(tempfolder);
        //    if (exists)
        //    {
        //        //empty directory first
        //        var di = new DirectoryInfo(tempfolder);
        //        foreach (var file in di.GetFiles())
        //        {
        //            file.Delete();
        //        }

        //        foreach (var dir in di.GetDirectories())
        //        {
        //            dir.Delete(true);
        //        }
        //    }
        //    else
        //    {
        //        Directory.CreateDirectory(tempfolder);
        //    }

        //    foreach (var studentFilese in noteFiles)
        //    {
        //        if (!string.IsNullOrEmpty(studentFilese.FilePath))
        //        {
        //            var path = $"/UserFiles/{stID}/الملاحظات/{studentFilese.FilePath}";
        //            var fileFromFtp = new StudentsController().DownloadFromFTP(path);
        //            if (fileFromFtp != null)
        //            {
        //                System.IO.File.WriteAllBytes(tempfolder + studentFilese.FilePath, fileFromFtp);
        //            }
        //        }
        //    }

        //    var archive = ZipFile.Open(tempfolder + noteid + ".zip", ZipArchiveMode.Create);
        //    var d = new DirectoryInfo(tempfolder);
        //    foreach (var file in d.GetFiles("*.jpg"))
        //    {
        //        archive.CreateEntryFromFile(tempfolder + file.Name, file.Name);
        //    }

        //    archive.Dispose();
        //    return Content($"/Content/tempfiles/FTP/{stID}/الملاحظات/" + noteid + ".zip");
        //}

        #endregion

        [HttpPost]
        public ActionResult DownloadStudentArchive(int? noteid, int? stID)
        {
            var filesCol = GeNotetFiles(noteid).ToList();

            if (System.IO.File.Exists(Server.MapPath
                ("~/Content/" + noteid + ".zip")))
            {
                System.IO.File.Delete(Server.MapPath
                    ("~/Content/" + noteid + ".zip"));
            }

            ZipArchive zipPth = ZipFile.Open(Server.MapPath
                ("~/Content/" + noteid + ".zip"), ZipArchiveMode.Create);
            foreach (var file in filesCol)
            {
                zipPth.CreateEntryFromFile(Server.MapPath
                    ("~/Content/UserFiles/" + stID + "/الملاحظات/" + file.FileName), file.FileName);
            }

            zipPth.Dispose();
            return Content("/Content/" + noteid + ".zip");
        }

        [HttpPost]
        public ActionResult DownloadStudentNoteFile(int? noteid, int? fileId, int? stID)
        {
            var studentFile = db.StudentFiles.SingleOrDefault(x => x.FileId == fileId);
            if (studentFile == null)
            {
                return null;
            }

            var path = $"/UserFiles/{stID}/الملاحظات/{studentFile.FilePath}";
            var fileName = path + studentFile.FilePath;
            var file = DownloadFromFTP(fileName);
            return file != null ? File(file, "Image/jpeg", $"{studentFile.FilePath}") : null;
        }

        public List<FileInfos> GeNotetFiles(int? ID)
        {
            List<FileInfos> listFiles = new List<FileInfos>();
            StudentNotes model = db.StudentNotes.Where(x => x.NoteId == ID).SingleOrDefault();
            string fileSavePath =
                HostingEnvironment.MapPath("~/Content/UserFiles/" + model.StDetailId + "/الملاحظات/");
            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);

            var fileNames = db.StudentFiles.Where(x => x.StudentNoteId == model.NoteId).ToList();

            if (fileNames != null)
            {
                int i = 0;
                foreach (var name in fileNames)
                {
                    listFiles.Add(new FileInfos
                    {
                        FileId = i + 1,
                        FileName = fileNames[i].FilePath,
                        FilePath = dirInfo.FullName + @"\" + fileNames[i].FilePath
                    });
                    i = i + 1;
                }
            }

            return listFiles;
        }

        public void UploadControl_FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {
            UploadedFile[] files = ((MVCxUploadControl)sender).UploadedFiles;

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].IsValid && !string.IsNullOrWhiteSpace(files[i].FileName))
                {
                    string resultFilePath = "~/Content/tempfiles/" + files[i].FileName;
                    files[i].SaveAs(
                        System.Web.HttpContext.Current.Request
                            .MapPath(resultFilePath)); // Code Central Mode - Uncomment This Line

                    string file = string.Format("{0} ({1}KB)", files[i].FileName, files[i].ContentLength / 1024);
                    string url = ((IUrlResolutionService)sender).ResolveClientUrl(resultFilePath);

                    e.CallbackData += file + "|" + url + "|";
                }
            }
        }

        #endregion


        #region الملاحظات

        public ActionResult StudentNotes()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult StudentNotesGridViewPartial()
        {
            GetStudentNotesPermssionsViewBags();

            TempData["Issues"] = db.Issues.Select(issue => new { issue.Id, issue.IssueDescription }).ToList();
            var studentid = int.Parse(Session["StudentID"].ToString());

            var user = HttpContext.Session["UserId"] as DashBoard_Users;


            var secretPermissons = db.Dashboard_SecretPermissions.SingleOrDefault(x => x.UserId == user.ID);
            ViewData["SecretPermisson"] = secretPermissons?.Secret;
            ViewData["TopSecretPermisson"] = secretPermissons?.TopSecret;
            var data = new SelectListItem { Text = "النظام", Value = "1", Selected = true };
            var Punishments = new List<SelectListItem>();
            Punishments.Add(data);
            ViewBag.Punishments = Punishments;
            IEnumerable<StudentNotes> model = new List<StudentNotes>();
            if (secretPermissons != null)
            {
                // العادية فقط
                var nortmalNotes = db.StudentNotes.Where(x =>
                        x.StDetailId == studentid && x.IsSecret == false && x.IsTopSecret == false)
                    .Include(x => x.StudentFiles);

                if (secretPermissons.Secret == true && secretPermissons.TopSecret == true)
                {
                    //السرية جداً فقط
                    var topSecretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
                        .Include(x => x.StudentFiles);


                    //السرية فقط
                    var secretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
                        .Include(x => x.StudentFiles);

                    //السرية جداً & السرية
                    var topSecretAndSecretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == true)
                        .Include(x => x.StudentFiles);
                    //سرى فقط & سرى جدا فقط & سرى جدا & سرى
                    var notes = topSecretNotes.Union(secretNotes).Union(topSecretAndSecretNotes);

                    model = notes.Union(nortmalNotes).Distinct();
                }
                else if (secretPermissons.Secret == true && secretPermissons.TopSecret == false)
                {
                    //السرية فقط
                    var secretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
                        .Include(x => x.StudentFiles);

                    model = secretNotes.Union(nortmalNotes).Distinct();
                }
                else if (secretPermissons.Secret == false && secretPermissons.TopSecret == true)
                {
                    //السرية جداً فقط
                    var topSecretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
                        .Include(x => x.StudentFiles);

                    model = topSecretNotes.Union(nortmalNotes).Distinct();
                }
                else if (secretPermissons.Secret == false && secretPermissons.TopSecret == false)
                {
                    // العادية فقط
                    model = nortmalNotes;
                }
            }

            var userGroup = db.DashBoard_UserGroups.FirstOrDefault(x => x.User_ID == user.ID);
            //مستخدمين المجموعة ج، د لهم صلاحية فقط على الملاحظات اللى عملوها
            if (userGroup?.Group_ID == 7 || userGroup?.Group_ID == 8)
            {
                model = db.StudentNotes.Where(x => x.AddedBy == user.ID);
            }

            return PartialView("_StudentNotesGridViewPartial", model.ToList());
        }

        private void GetStudentNotesPermssionsViewBags()
        {
            //الملاحظات

            var permissionsNotesData = GetPermissionsfn(10);
            ViewBag.NotesDataRead = permissionsNotesData.Read;
            ViewBag.NotesDataCreate = permissionsNotesData.Create;
            ViewBag.NotesDataUpdate = permissionsNotesData.Update;
            ViewBag.NotesDataDelete = permissionsNotesData.Delete;
            ViewBag.NotesDataSave = permissionsNotesData.Save;
            ViewBag.NotesDataDownloadAttachments = permissionsNotesData.DownloadAttachments;
        }

        #region FTP

        //[HttpPost, ValidateInput(false)]
        //public ActionResult StudentNotesGridViewPartialAddNew(StudentNotes item)
        //{
        //    GetStudentNotesPermssionsViewBags();
        //    var studentid = int.Parse(Session["StudentID"].ToString());
        //    TempData["Issues"] = db.Issues.Select(issue => new {issue.Id, issue.IssueDescription}).ToList();
        //    var userId = int.Parse(HttpContext.Session["UserId"].ToString());
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var studentNotes = new StudentNotes
        //            {
        //                IsSecret = item.IsSecret ?? false,
        //                IsTopSecret = item.IsTopSecret ?? false,
        //                IssueId = item.IssueId,
        //                StDetailId = studentid,
        //                NoteDate = item.NoteDate,
        //                NoteDetails = item.NoteDetails,
        //                AddedBy = userId,
        //                //LastUpdatedBy = userId
        //            };
        //            db.StudentNotes.Add(studentNotes);
        //            db.SaveChanges();

        //            var filesNames = (List<string>) Session["studentNotesFileNames"];
        //            //تم اضافة ملفات للملاحظة وسيتم نقلها من الفولدر المؤقت الى فولد الطالب
        //            if (filesNames != null && filesNames.Any())
        //            {
        //                //var filePathDirectory = $"~/Content/UserFiles/{studentid}/الملاحظات/";
        //                //var destinationPath = Server.MapPath(filePathDirectory);
        //                //var currentFolder = new DirectoryInfo(destinationPath);

        //                //if (currentFolder.Exists == false)
        //                //{
        //                //    Directory.CreateDirectory(destinationPath);
        //                //}

        //                foreach (var filesName in filesNames)
        //                {
        //                    //يقرا الملف من المكان الموقت
        //                    var bytes = System.IO.File.ReadAllBytes(
        //                        Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + filesName));

        //                    //var filename =
        //                    //    $"{filesName.Split('.')[0]}_{DateTime.Now:dd-MM-yyyy-HH-mm}.{filesName.Split('.')[1]}";
        //                    //يحفظ ملف الملاحظات فى فولدر الطالب
        //                    //var fileStream = new FileStream(destinationPath + "\\" + filename, FileMode.Create,
        //                    //    FileAccess.ReadWrite);
        //                    //var ms = new MemoryStream(bytes);
        //                    //var image = Image.FromStream(ms);
        //                    //var halfQualityImage = Files.CompressImage(image, 90);
        //                    //fileStream.Write(halfQualityImage, 0, halfQualityImage.Length);
        //                    //fileStream.Close();

        //                    var ms = new MemoryStream(bytes);
        //                    var image = Image.FromStream(ms);
        //                    const string ext = ".jpg";
        //                    var imagename =
        //                        $"{studentNotes.NoteId}_{studentNotes.StDetailId}_{DateTime.Now:dd.MM.yyyy.h.mm.ss}{ext}";
        //                    var halfQualityImage = Files.CompressImage(image, 90);
        //                    var path = $"UserFiles/{studentid}/الملاحظات/{imagename}";
        //                    var isSavedToFtp = SaveToFTP(halfQualityImage, path);
        //                    if (!isSavedToFtp)
        //                    {
        //                        ViewData["EditError"] = "حدث خطأ أثناء رفع المرفقات، برجاء المحاولة مرة أخرى.";
        //                    }

        //                    db.StudentFiles.Add(new StudentFiles
        //                    {
        //                        FilePath = imagename,
        //                        StudentNoteId = studentNotes.NoteId
        //                    });

        //                    db.SaveChanges();
        //                }

        //                //cleanup process
        //                var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/");
        //                var tempDirectoryInfo = new DirectoryInfo(tempFolder);
        //                foreach (var fileInfo in tempDirectoryInfo.GetFiles())
        //                {
        //                    fileInfo.Delete();
        //                }

        //                Session["studentNotesFileNames"] = new List<string>();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //    {
        //        ViewData["EditError"] = "حدث خطأ أثناء الحفظ، حاول تصحيح الأخطاء والمحاولة مرة أخرى.";
        //    }

        //    var secretPermissons = db.Dashboard_SecretPermissions.SingleOrDefault(x => x.UserId == userId);
        //    ViewData["SecretPermisson"] = secretPermissons?.Secret;
        //    ViewData["TopSecretPermisson"] = secretPermissons?.TopSecret;
        //    IEnumerable<StudentNotes> model = new List<StudentNotes>();
        //    if (secretPermissons != null)
        //    {
        //        // العادية فقط
        //        var nortmalNotes = db.StudentNotes.Where(x =>
        //                x.StDetailId == studentid && x.IsSecret == false && x.IsTopSecret == false)
        //            .Include(x => x.StudentFiles);

        //        if (secretPermissons.Secret == true && secretPermissons.TopSecret == true)
        //        {
        //            //السرية جداً فقط
        //            var topSecretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
        //                .Include(x => x.StudentFiles);


        //            //السرية فقط
        //            var secretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
        //                .Include(x => x.StudentFiles);

        //            //السرية جداً & السرية
        //            var topSecretAndSecretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == true)
        //                .Include(x => x.StudentFiles);

        //            //سرى فقط & سرى جدا فقط & سرى جدا & سرى
        //            var notes = topSecretNotes.Union(secretNotes).Union(topSecretAndSecretNotes);

        //            model = notes.Union(nortmalNotes).Distinct();
        //        }
        //        else if (secretPermissons.Secret == true && secretPermissons.TopSecret == false)
        //        {
        //            //السرية فقط
        //            var secretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
        //                .Include(x => x.StudentFiles);

        //            model = secretNotes.Union(nortmalNotes).Distinct();
        //        }
        //        else if (secretPermissons.Secret == false && secretPermissons.TopSecret == true)
        //        {
        //            //السرية جداً فقط
        //            var topSecretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
        //                .Include(x => x.StudentFiles);

        //            model = topSecretNotes.Union(nortmalNotes).Distinct();
        //        }
        //        else if (secretPermissons.Secret == false && secretPermissons.TopSecret == false)
        //        {
        //            // العادية فقط
        //            model = nortmalNotes;
        //        }
        //    }

        //    var userGroup = db.DashBoard_UserGroups.FirstOrDefault(x => x.User_ID == userId);
        //    //مستخدمين المجموعة ج، د لهم صلاحية فقط على الملاحظات اللى عملوها
        //    if (userGroup?.Group_ID == 7 || userGroup?.Group_ID == 8)
        //    {
        //        model = db.StudentNotes.Where(x => x.AddedBy == userId);
        //    }

        //    return PartialView("_StudentNotesGridViewPartial", model.ToList());
        //}

        #endregion


        [HttpPost, ValidateInput(false)]
        public ActionResult StudentNotesGridViewPartialAddNew(StudentNotes item)
        {
            GetStudentNotesPermssionsViewBags();

            var studentid = int.Parse(Session["StudentID"].ToString());
            TempData["Issues"] = db.Issues.Select(issue => new { issue.Id, issue.IssueDescription }).ToList();
            var userId = int.Parse(HttpContext.Session["UserId"].ToString());

            if (ModelState.IsValid)
            {
                try
                {
                    var studentNotes = new StudentNotes
                    {
                        IsSecret = item.IsSecret ?? false,
                        IsTopSecret = item.IsTopSecret ?? false,
                        IssueId = item.IssueId,
                        StDetailId = studentid,
                        NoteDate = item.NoteDate,
                        NoteDetails = item.NoteDetails,
                        AddedBy = userId,
                        PunichedBy = item.PunichedBy,
                        Punishment = item.Punishment
                        //LastUpdatedBy = userId
                    };
                    db.StudentNotes.Add(studentNotes);
                    db.SaveChanges();

                    var filesNames = (List<string>)Session["studentNotesFileNames"];
                    //تم اضافة ملفات للملاحظة وسيتم نقلها من الفولدر المؤقت الى فولد الطالب
                    if (filesNames != null && filesNames.Any())
                    {
                        var filePathDirectory = $"~/Content/UserFiles/{studentid}/الملاحظات/";
                        var destinationPath = Server.MapPath(filePathDirectory);
                        var currentFolder = new DirectoryInfo(destinationPath);

                        if (currentFolder.Exists == false)
                        {
                            Directory.CreateDirectory(destinationPath);
                        }

                        foreach (var name in filesNames)
                        {
                            var studentFile = new StudentFiles
                            {
                                FilePath = "",
                                StudentNoteId = studentNotes.NoteId
                            };
                            db.StudentFiles.Add(studentFile);
                            db.SaveChanges();

                            var filename = $"{studentNotes.NoteId}_{studentFile.FileId}.{name.Split('.')[1]}";
                            //يقرا الملف من المكان الموقت
                            var bytes = System.IO.File.ReadAllBytes(
                                Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + name));

                            //يحفظ ملف الملاحظات فى فولدر الطالب
                            System.IO.File.WriteAllBytes(destinationPath + filename, bytes);
                            studentFile.FilePath = filename;
                            db.Entry(studentFile).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        //cleanup process
                        var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/");
                        var tempDirectoryInfo = new DirectoryInfo(tempFolder);
                        foreach (var fileInfo in tempDirectoryInfo.GetFiles())
                        {
                            fileInfo.Delete();
                        }

                        Session["studentNotesFileNames"] = new List<string>();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "حدث خطأ أثناء الحفظ، حاول تصحيح الأخطاء والمحاولة مرة أخرى.";
            }

            var secretPermissons = db.Dashboard_SecretPermissions.SingleOrDefault(x => x.UserId == userId);
            ViewData["SecretPermisson"] = secretPermissons?.Secret;
            ViewData["TopSecretPermisson"] = secretPermissons?.TopSecret;
            IEnumerable<StudentNotes> model = new List<StudentNotes>();
            if (secretPermissons != null)
            {
                // العادية فقط
                var nortmalNotes = db.StudentNotes.Where(x =>
                        x.StDetailId == studentid && x.IsSecret == false && x.IsTopSecret == false)
                    .Include(x => x.StudentFiles);

                if (secretPermissons.Secret == true && secretPermissons.TopSecret == true)
                {
                    //السرية جداً فقط
                    var topSecretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
                        .Include(x => x.StudentFiles);


                    //السرية فقط
                    var secretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
                        .Include(x => x.StudentFiles);

                    //السرية جداً & السرية
                    var topSecretAndSecretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == true)
                        .Include(x => x.StudentFiles);

                    //سرى فقط & سرى جدا فقط & سرى جدا & سرى
                    var notes = topSecretNotes.Union(secretNotes).Union(topSecretAndSecretNotes);

                    model = notes.Union(nortmalNotes).Distinct();
                }
                else if (secretPermissons.Secret == true && secretPermissons.TopSecret == false)
                {
                    //السرية فقط
                    var secretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
                        .Include(x => x.StudentFiles);

                    model = secretNotes.Union(nortmalNotes).Distinct();
                }
                else if (secretPermissons.Secret == false && secretPermissons.TopSecret == true)
                {
                    //السرية جداً فقط
                    var topSecretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
                        .Include(x => x.StudentFiles);

                    model = topSecretNotes.Union(nortmalNotes).Distinct();
                }
                else if (secretPermissons.Secret == false && secretPermissons.TopSecret == false)
                {
                    // العادية فقط
                    model = nortmalNotes;
                }
            }

            var userGroup = db.DashBoard_UserGroups.FirstOrDefault(x => x.User_ID == userId);
            //مستخدمين المجموعة ج، د لهم صلاحية فقط على الملاحظات اللى عملوها
            if (userGroup?.Group_ID == 7 || userGroup?.Group_ID == 8)
            {
                model = db.StudentNotes.Where(x => x.AddedBy == userId);
            }

            return PartialView("_StudentNotesGridViewPartial", model.ToList());
        }


        #region FTP

        //[HttpPost, ValidateInput(false)]
        //public ActionResult StudentNotesGridViewPartialUpdate(StudentNotes item)
        //{
        //    GetStudentNotesPermssionsViewBags();
        //    var studentid = int.Parse(Session["StudentID"].ToString());

        //    TempData["Issues"] = db.Issues.Select(issue => new {issue.Id, issue.IssueDescription}).ToList();

        //    item.StDetailId = studentid;

        //    var userId = int.Parse(HttpContext.Session["UserId"].ToString());
        //    var secretPermissons = db.Dashboard_SecretPermissions.SingleOrDefault(x => x.UserId == userId);
        //    ViewData["SecretPermisson"] = secretPermissons?.Secret;
        //    ViewData["TopSecretPermisson"] = secretPermissons?.TopSecret;
        //    IEnumerable<StudentNotes> model = new List<StudentNotes>();
        //    if (secretPermissons != null)
        //    {
        //        // العادية فقط
        //        var nortmalNotes = db.StudentNotes.Where(x =>
        //                x.StDetailId == studentid && x.IsSecret == false && x.IsTopSecret == false)
        //            .Include(x => x.StudentFiles);

        //        if (secretPermissons.Secret == true && secretPermissons.TopSecret == true)
        //        {
        //            //السرية جداً فقط
        //            var topSecretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
        //                .Include(x => x.StudentFiles);


        //            //السرية فقط
        //            var secretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
        //                .Include(x => x.StudentFiles);

        //            //السرية جداً & السرية
        //            var topSecretAndSecretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == true)
        //                .Include(x => x.StudentFiles);

        //            //سرى فقط & سرى جدا فقط & سرى جدا & سرى
        //            var notes = topSecretNotes.Union(secretNotes).Union(topSecretAndSecretNotes);

        //            model = notes.Union(nortmalNotes).Distinct();
        //        }
        //        else if (secretPermissons.Secret == true && secretPermissons.TopSecret == false)
        //        {
        //            //السرية فقط
        //            var secretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
        //                .Include(x => x.StudentFiles);

        //            model = secretNotes.Union(nortmalNotes).Distinct();
        //        }
        //        else if (secretPermissons.Secret == false && secretPermissons.TopSecret == true)
        //        {
        //            //السرية جداً فقط
        //            var topSecretNotes = db.StudentNotes.Where(x =>
        //                    x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
        //                .Include(x => x.StudentFiles);

        //            model = topSecretNotes.Union(nortmalNotes).Distinct();
        //        }
        //        else if (secretPermissons.Secret == false && secretPermissons.TopSecret == false)
        //        {
        //            // العادية فقط
        //            model = nortmalNotes;
        //        }
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var modelItem = db.StudentNotes.FirstOrDefault(it => it.NoteId == item.NoteId);
        //            if (modelItem != null)
        //            {
        //                modelItem.LastUpdatedBy = userId;
        //                UpdateModel(modelItem);
        //                db.SaveChanges();
        //            }

        //            var filesNames = (List<string>) Session["studentNotesFileNames"];
        //            //تم اضافة ملفات للملاحظة وسيتم نقلها من الفولدر المؤقت الى فولد الطالب
        //            if (filesNames != null && filesNames.Any())
        //            {
        //                if (modelItem?.StudentFiles != null)
        //                {
        //                    //فى حالة وجود ملفات سابقة للملاحظة يتم حذفها قبل اضافة الملفات الجديدة منعاً للتكرار
        //                    foreach (var studentFile in modelItem.StudentFiles)
        //                    {
        //                        filesNames.Remove(x => x.Equals(studentFile.FilePath));
        //                    }
        //                }
        //                //filesNames تحتوى على اسماء الملفات الجديدة فقط
        //                //var filePathDirectory = $"~/Content/UserFiles/{studentid}/الملاحظات/";
        //                //var destinationPath = Server.MapPath(filePathDirectory);
        //                //var currentFolder = new DirectoryInfo(destinationPath);
        //                //if (currentFolder.Exists == false)
        //                //{
        //                //    Directory.CreateDirectory(destinationPath);
        //                //}

        //                foreach (var filesName in filesNames)
        //                {
        //                    //يقرا الملف من المكان الموقت
        //                    var bytes = System.IO.File.ReadAllBytes(
        //                        Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + filesName));
        //                    var ms = new MemoryStream(bytes);
        //                    var image = Image.FromStream(ms);
        //                    const string ext = ".jpg";
        //                    if (modelItem != null)
        //                    {
        //                        var imagename =
        //                            $"{modelItem.NoteId}_{modelItem.StDetailId}_{DateTime.Now:dd.MM.yyyy.h.mm.ss}{ext}";
        //                        var halfQualityImage = Files.CompressImage(image, 90);
        //                        var path = $"UserFiles/{studentid}/الملاحظات/{imagename}";
        //                        var isSavedToFtp = SaveToFTP(halfQualityImage, path);
        //                        if (!isSavedToFtp)
        //                        {
        //                            ViewData["EditError"] = "حدث خطأ أثناء رفع المرفقات، برجاء المحاولة مرة أخرى.";
        //                        }

        //                        db.StudentFiles.Add(new StudentFiles
        //                        {
        //                            FilePath = imagename,
        //                            StudentNoteId = item.NoteId
        //                        });
        //                        db.SaveChanges();
        //                    }

        //                    //var filename =
        //                    //    $"{filesName.Split('.')[0]}_{DateTime.Now:dd.MM.yyyy.HH.mm.ss}.{filesName.Split('.')[1]}";
        //                    ////يحفظ ملف الملاحظات فى فولدر الطالب
        //                    //var fileStream = new FileStream(destinationPath + "\\" + filename, FileMode.Create,
        //                    //    FileAccess.ReadWrite);
        //                    ////var ms = new MemoryStream(bytes);
        //                    //var image = Image.FromStream(ms);
        //                    //var halfQualityImage = Files.CompressImage(image, 90);
        //                    //fileStream.Write(bytes, 0, bytes.Length);
        //                    //fileStream.Close();
        //                }

        //                //cleanup process
        //                var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/");
        //                var tempDirectoryInfo = new DirectoryInfo(tempFolder);
        //                foreach (var fileInfo in tempDirectoryInfo.GetFiles())
        //                {
        //                    fileInfo.Delete();
        //                }

        //                Session["studentNotesFileNames"] = new List<string>();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //        ViewData["EditError"] = "حدث خطأ أثناء الحفظ، حاول تصحيح الأخطاء والمحاولة مرة أخرى.";

        //    var userGroup = db.DashBoard_UserGroups.FirstOrDefault(x => x.User_ID == userId);
        //    //مستخدمين المجموعة ج، د لهم صلاحية فقط على الملاحظات اللى عملوها
        //    if (userGroup?.Group_ID == 7 || userGroup?.Group_ID == 8)
        //    {
        //        model = db.StudentNotes.Where(x => x.AddedBy == userId);
        //    }

        //    return PartialView("_StudentNotesGridViewPartial", model.ToList());
        //}

        #endregion


        [HttpPost, ValidateInput(false)]
        public ActionResult StudentNotesGridViewPartialUpdate(StudentNotes item)
        {
            GetStudentNotesPermssionsViewBags();

            var studentid = int.Parse(Session["StudentID"].ToString());

            TempData["Issues"] = db.Issues.Select(issue => new { issue.Id, issue.IssueDescription }).ToList();

            item.StDetailId = studentid;
            var userId = 0;
            if (HttpContext.Session.Count > 0)
            {
                var CurrentUser = System.Web.HttpContext.Current.Session["UserId"] as StudentProfile.DAL.Models.DashBoard_Users;

                userId = CurrentUser.ID;
                if (userId == 0)
                {

                }
            }

            var secretPermissons = db.Dashboard_SecretPermissions.SingleOrDefault(x => x.UserId == userId);
            ViewData["SecretPermisson"] = secretPermissons?.Secret;
            ViewData["TopSecretPermisson"] = secretPermissons?.TopSecret;
            IEnumerable<StudentNotes> model = new List<StudentNotes>();
            if (secretPermissons != null)
            {
                // العادية فقط
                var nortmalNotes = db.StudentNotes.Where(x =>
                        x.StDetailId == studentid && x.IsSecret == false && x.IsTopSecret == false)
                    .Include(x => x.StudentFiles);

                if (secretPermissons.Secret == true && secretPermissons.TopSecret == true)
                {
                    //السرية جداً فقط
                    var topSecretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
                        .Include(x => x.StudentFiles);


                    //السرية فقط
                    var secretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
                        .Include(x => x.StudentFiles);

                    //السرية جداً & السرية
                    var topSecretAndSecretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == true)
                        .Include(x => x.StudentFiles);

                    //سرى فقط & سرى جدا فقط & سرى جدا & سرى
                    var notes = topSecretNotes.Union(secretNotes).Union(topSecretAndSecretNotes);

                    model = notes.Union(nortmalNotes).Distinct();
                }
                else if (secretPermissons.Secret == true && secretPermissons.TopSecret == false)
                {
                    //السرية فقط
                    var secretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == false && x.IsSecret == true)
                        .Include(x => x.StudentFiles);

                    model = secretNotes.Union(nortmalNotes).Distinct();
                }
                else if (secretPermissons.Secret == false && secretPermissons.TopSecret == true)
                {
                    //السرية جداً فقط
                    var topSecretNotes = db.StudentNotes.Where(x =>
                            x.StDetailId == studentid && x.IsTopSecret == true && x.IsSecret == false)
                        .Include(x => x.StudentFiles);

                    model = topSecretNotes.Union(nortmalNotes).Distinct();
                }
                else if (secretPermissons.Secret == false && secretPermissons.TopSecret == false)
                {
                    // العادية فقط
                    model = nortmalNotes;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = db.StudentNotes.FirstOrDefault(it => it.NoteId == item.NoteId);
                    if (modelItem != null)
                    {
                        modelItem.LastUpdatedBy = userId;
                        UpdateModel(modelItem);
                        db.SaveChanges();
                    }


                    var filesNames = (List<string>)Session["studentNotesFileNames"];
                    //تم اضافة ملفات للملاحظة وسيتم نقلها من الفولدر المؤقت الى فولد الطالب
                    if (filesNames != null && filesNames.Any())
                    {
                        if (modelItem?.StudentFiles != null)
                        {
                            //فى حالة وجود ملفات سابقة للملاحظة يتم حذفها قبل اضافة الملفات الجديدة منعاً للتكرار
                            foreach (var studentFile in modelItem.StudentFiles)
                            {
                                filesNames.Remove(x => x.Equals(studentFile.FilePath));
                            }
                        }

                        //filesNames تحتوى على اسماء الملفات الجديدة فقط
                        var filePathDirectory = $"~/Content/UserFiles/{studentid}/الملاحظات/";
                        var destinationPath = Server.MapPath(filePathDirectory);
                        var currentFolder = new DirectoryInfo(destinationPath);

                        if (currentFolder.Exists == false)
                        {
                            Directory.CreateDirectory(destinationPath);
                        }

                        foreach (var filesName in filesNames)
                        {
                            //يقرا الملف من المكان الموقت
                            var bytes = System.IO.File.ReadAllBytes(
                                Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + filesName));
                            if (modelItem != null)
                            {
                                var studentFile = new StudentFiles
                                {
                                    FilePath = "",
                                    StudentNoteId = modelItem.NoteId
                                };
                                db.StudentFiles.Add(studentFile);
                                db.SaveChanges();

                                var filename = $"{modelItem.NoteId}_{studentFile.FileId}.{filesName.Split('.')[1]}";
                                //يحفظ ملف الملاحظات فى فولدر الطالب
                                System.IO.File.WriteAllBytes(destinationPath + filename, bytes);
                                studentFile.FilePath = filename;
                                db.Entry(studentFile).State = EntityState.Modified;
                            }

                            db.SaveChanges();
                        }

                        //cleanup process
                        var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/");
                        var tempDirectoryInfo = new DirectoryInfo(tempFolder);
                        foreach (var fileInfo in tempDirectoryInfo.GetFiles())
                        {
                            fileInfo.Delete();
                        }

                        Session["studentNotesFileNames"] = new List<string>();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "حدث خطأ أثناء الحفظ، حاول تصحيح الأخطاء والمحاولة مرة أخرى.";
            }

            var userGroup = db.DashBoard_UserGroups.FirstOrDefault(x => x.User_ID == userId);
            //مستخدمين المجموعة ج، د لهم صلاحية فقط على الملاحظات اللى عملوها
            if (userGroup?.Group_ID == 7 || userGroup?.Group_ID == 8)
            {
                model = db.StudentNotes.Where(x => x.AddedBy == userId);
            }

            return PartialView("_StudentNotesGridViewPartial", model.ToList());
        }

        #region FTP

        //[HttpPost, ValidateInput(false)]
        //public ActionResult StudentNotesGridViewPartialDelete(int NoteId)
        //{
        //    TempData["Issues"] = db.Issues.Select(issue => new {issue.Id, issue.IssueDescription}).ToList();

        //    if (NoteId >= 0)
        //    {
        //        try
        //        {
        //            var item = db.StudentNotes.FirstOrDefault(it => it.NoteId == NoteId);
        //            if (item != null)
        //            {
        //                var studentid = int.Parse(Session["StudentID"].ToString());

        //                //var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/الملاحظات/");

        //                //حذف الملفات للملاحظة
        //                foreach (var studentFile in item.StudentFiles)
        //                {
        //                    var imagename = studentFile.FilePath;
        //                    var path = $"UserFiles/{studentid}/الملاحظات/{imagename}";
        //                    var isDeletedFromFtp = DeleteFromFTP(path);
        //                    if (!isDeletedFromFtp)
        //                    {
        //                        return Json("حدث خطأ أثناء حذف المرفقات", JsonRequestBehavior.AllowGet);
        //                    }

        //                    //var filExists = System.IO.File.Exists(tempFolder + studentFile);
        //                    //if (filExists)
        //                    //{
        //                    //    System.IO.File.Delete(tempFolder + studentFile);
        //                    //}
        //                }

        //                db.StudentFiles.RemoveRange(item.StudentFiles);
        //                db.StudentNotes.Remove(item);
        //            }

        //            db.SaveChanges();
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //            return Json("حدث خطأ أثناء الحذف" + e.Message, JsonRequestBehavior.AllowGet);
        //        }
        //    }

        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult StudentNotesGridViewPartialDelete(int NoteId)
        {
            GetStudentNotesPermssionsViewBags();
            TempData["Issues"] = db.Issues.Select(issue => new { issue.Id, issue.IssueDescription }).ToList();

            if (NoteId >= 0)
            {
                try
                {
                    var item = db.StudentNotes.FirstOrDefault(it => it.NoteId == NoteId);
                    if (item != null)
                    {
                        var studentid = int.Parse(Session["StudentID"].ToString());

                        var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/الملاحظات/");

                        //حذف الملفات للملاحظة
                        foreach (var studentFile in item.StudentFiles)
                        {
                            var filExists = System.IO.File.Exists(tempFolder + studentFile.FilePath);
                            if (filExists)
                            {
                                System.IO.File.Delete(tempFolder + studentFile.FilePath);
                            }
                        }

                        db.StudentFiles.RemoveRange(item.StudentFiles);
                        db.StudentNotes.Remove(item);
                    }

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return Json("حدث خطأ أثناء الحذف" + e.Message, JsonRequestBehavior.AllowGet);
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void StudentNoteFilesUpload()
        {
            string[] errors;
            var listUploadedFilesNames = new List<string>();
            listUploadedFilesNames = (List<string>)Session["studentNotesFileNames"];
            if (listUploadedFilesNames != null && listUploadedFilesNames.Count > 0
            ) //يوجد ملفات من قبل ويتم الاضافة عليها او تحديثها
            {
                var uploadedFiles = UploadControlExtension.GetUploadedFiles("myFile",
                    null, out errors, (s, e) => { },
                    (sender, e) =>
                    {
                        UploadedFile[] files = ((MVCxUploadControl)sender).UploadedFiles;

                        foreach (var uploadedFile in files)
                        {
                            if (uploadedFile.IsValid && !string.IsNullOrWhiteSpace(uploadedFile.FileName))
                            {
                                var studentid = int.Parse(Session["StudentID"].ToString());

                                var filePathDirectory = $"~/Content/UserFiles/{studentid}/tempfiles/";
                                var destinationPath = Server.MapPath(filePathDirectory);
                                var currentFolder = new DirectoryInfo(destinationPath);

                                if (currentFolder.Exists == false)
                                {
                                    Directory.CreateDirectory(destinationPath);
                                }

                                string resultFilePath = destinationPath + uploadedFile.FileName;
                                uploadedFile.SaveAs(resultFilePath);

                                var file = $"{uploadedFile.FileName} ({uploadedFile.ContentLength / 1024}KB)";
                                var url =
                                    ((IUrlResolutionService)sender).ResolveClientUrl(
                                        filePathDirectory + uploadedFile.FileName);

                                e.CallbackData += file + "|" + url + "|";
                            }
                        }
                    }).ToList();
                if (uploadedFiles.Count > 0)
                {
                    foreach (var file in uploadedFiles)
                    {
                        listUploadedFilesNames.Add(file.FileName);
                    }

                    Session["studentNotesFileNames"] = listUploadedFilesNames;
                }
            }
            else //اضافة ملفات لاول مرة
            {
                var myfiles =
                    UploadControlExtension.GetUploadedFiles("myFile", null
                        , out errors, (s, e) => { },
                        (sender, e) =>
                        {
                            UploadedFile[] files = ((MVCxUploadControl)sender).UploadedFiles;

                            foreach (var t in files)
                            {
                                if (t.IsValid && !string.IsNullOrWhiteSpace(t.FileName))
                                {
                                    var studentid = int.Parse(Session["StudentID"].ToString());

                                    var filePathDirectory = $"~/Content/UserFiles/{studentid}/tempfiles/";
                                    var destinationPath = Server.MapPath(filePathDirectory);
                                    var currentFolder = new DirectoryInfo(destinationPath);
                                    if (currentFolder.Exists == false)
                                    {
                                        Directory.CreateDirectory(destinationPath);
                                    }

                                    var resultFilePath = destinationPath + t.FileName;
                                    t.SaveAs(resultFilePath);

                                    string file = $"{t.FileName} ({t.ContentLength / 1024} KB)";
                                    string url = ((IUrlResolutionService)sender).ResolveClientUrl(resultFilePath);

                                    e.CallbackData += file + "|" + url + "|";
                                }
                            }
                        }).ToList();

                listUploadedFilesNames = new List<string>();

                foreach (var uploadedFile in myfiles)
                {
                    listUploadedFilesNames.Add(uploadedFile.FileName);
                }

                Session["studentNotesFileNames"] = listUploadedFilesNames;
            }
        }

        public ActionResult ImageUpload()
        {
            UploadControlExtension.GetUploadedFiles("NoteFiles", null,
                (s, e) =>
                {
                    if (e.UploadedFile.IsValid)
                    {
                        try
                        {
                            var name = e.UploadedFile.FileName;
                            var studentid = int.Parse(Session["StudentID"].ToString());

                            var filePathDirectory = $"~/Content/tempfiles/";
                            var destinationPath = Server.MapPath(filePathDirectory);
                            if (!Directory.Exists(destinationPath))
                            {
                                Directory.CreateDirectory(destinationPath);
                            }


                            var generatedFileName = $"tempnotefile_{Path.GetExtension(e.UploadedFile.FileName)}";
                            var resultFilePath = destinationPath + generatedFileName;
                            e.UploadedFile.SaveAs(resultFilePath, true);
                            Session["studentNotesFileNames"] = e.UploadedFile.FileContent;

                            string file = $"{e.UploadedFile.FileName} ({e.UploadedFile.ContentLength / 1024}KB)";
                            string url = ((IUrlResolutionService)s).ResolveClientUrl(resultFilePath);

                            e.CallbackData += file + "|" + url + "|";
                            //e.CallbackData += "تم الحفظ بنجاح";
                            //e.ErrorText += "";

                            //IUrlResolutionService urlResolver = s as IUrlResolutionService;
                            //if (urlResolver != null)
                            //{
                            //    e.CallbackData = urlResolver.ResolveClientUrl(resultFilePath) + "?refresh=" +
                            //                     Guid.NewGuid();
                            //}
                        }
                        catch (Exception)
                        {
                            e.CallbackData += "";
                            //e.ErrorText += "عفواً حدث خطأ أثناء الحفظ برجاء تصحيح الأخطاء والمحاولة مرة أخرى";
                        }
                    }
                }
            );
            return null;
        }


        public void uploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                try
                {
                    var name = e.UploadedFile.FileName;
                    e.CallbackData = name;

                    var idNumber = int.Parse(Session["IdNumber"].ToString());

                    var filePathDirectory = $"~/Content/tempfiles/";
                    var destinationPath = Server.MapPath(filePathDirectory);
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }


                    var dt = DateTime.Now.ToString("dd-MMM-yyyy");
                    var generatedFileName = $"tempnotefile_{Path.GetExtension(e.UploadedFile.FileName)}";
                    var resultFilePath = destinationPath + generatedFileName;
                    e.UploadedFile.SaveAs(resultFilePath, true);
                    Session["NoteFiles"] = e.UploadedFile.FileContent;

                    string file = string.Format("{0} ({1}KB)", e.UploadedFile.FileName,
                        e.UploadedFile.ContentLength / 1024);
                    string url = ((IUrlResolutionService)sender).ResolveClientUrl(resultFilePath);

                    e.CallbackData += file + "|" + url + "|";
                    //e.CallbackData += "تم الحفظ بنجاح";
                    //e.ErrorText += "";

                    IUrlResolutionService urlResolver = sender as IUrlResolutionService;
                    if (urlResolver != null)
                    {
                        e.CallbackData = urlResolver.ResolveClientUrl(resultFilePath) + "?refresh=" + Guid.NewGuid();
                    }
                }
                catch (Exception)
                {
                    e.CallbackData += "";
                    //e.ErrorText += "عفواً حدث خطأ أثناء الحفظ برجاء تصحيح الأخطاء والمحاولة مرة أخرى";
                }
            }
        }

        [HttpPost]
        public ActionResult RemoveNotesFilesSession()
        {
            //remove files from temp folder
            var studentid = int.Parse(Session["StudentID"].ToString());
            var di = new DirectoryInfo(Server.MapPath($@"~\Content\UserFiles\{studentid}\tempfiles"));
            if (di.GetFiles().Length > 0)
            {
                foreach (var file in di.EnumerateFiles())
                {
                    file.Delete();
                }
            }


            Session.Remove("studentNotesFileNames");
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region حساب مواقع التواصل الاجتماعى

        [HttpPost]
        public ActionResult SaveFacebookAccount(string facebookAccountTextBox)
        {
            var studentid = int.Parse(Session["StudentID"].ToString());
            var idNumber = Session["IdNumber"].ToString();

            //التاكد من ان الحساب غير موجود من قبل
            var scoialMediaAccounts = db.ST_ScoialMediaAccounts
                .SingleOrDefault(x => x.StudentID == studentid && x.IdNumber == idNumber);
            if (scoialMediaAccounts != null)
            {
                scoialMediaAccounts.FacebookAccount = facebookAccountTextBox;
                db.Entry(scoialMediaAccounts).State = EntityState.Modified;
            }
            else
            {
                db.ST_ScoialMediaAccounts.Add(new ST_ScoialMediaAccounts
                {
                    FacebookAccount = facebookAccountTextBox,
                    IdNumber = idNumber,
                    StudentID = studentid
                });
            }

            db.SaveChanges();

            return RedirectToAction("_StBasicData", new { Id = studentid.ToString() });
        }

        [HttpPost]
        public ActionResult SaveTwitterAccount(string twitterAccountTextBox)
        {
            var studentid = int.Parse(Session["StudentID"].ToString());
            var idNumber = Session["IdNumber"].ToString();

            //التاكد من ان الحساب غير موجود من قبل
            var scoialMediaAccounts = db.ST_ScoialMediaAccounts
                .SingleOrDefault(x => x.StudentID == studentid && x.IdNumber == idNumber);

            if (scoialMediaAccounts != null)
            {
                scoialMediaAccounts.TwitterAccount = twitterAccountTextBox;
                db.Entry(scoialMediaAccounts).State = EntityState.Modified;
            }
            else
            {
                db.ST_ScoialMediaAccounts.Add(new ST_ScoialMediaAccounts
                {
                    TwitterAccount = twitterAccountTextBox,
                    IdNumber = idNumber,
                    StudentID = studentid
                });
            }

            db.SaveChanges();

            return RedirectToAction("_StBasicData", new { Id = studentid.ToString() });
        }

        #endregion

        #region المساهمات و المشاركات

        public ActionResult EmpCourses()
        {
            return PartialView();
        }

        public static List<SelectListItem> GetCoursesTypes()
        {
            using (var dbHr = new HRMadinaEntities())
            {
                return dbHr.CourseTypes.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.Name
                }).ToList();
            }
        }



        [ValidateInput(false)]
        public ActionResult EmpCoursesGridViewPartial(int studentId)
        {
            #region Old Code
            //var model = db.ActivityRequestsArchive.Where(x => x.Student_ID == studentId && x.ApprovedbyID != null).ToList()
            //                                      .Select(p => new ActivityRequestsVM
            //                                      {
            //                                          ID = p.ID,
            //                                          Degree = p.Degree,
            //                                          Duration = p.Duration,
            //                                          Location = p.Location,
            //                                          Type = p.Type,
            //                                          EndDate = p.EndDate.ToString("dd/MM/yyyy"),
            //                                          StartDate = p.StartDate.ToString("dd/MM/yyyy"),
            //                                          Name = p.Name,
            //                                          Ratio = Convert.ToInt32(p.Ratio) + " %"
            //                                      }).ToList();

            // GetEmpCousesPermissionsViewBags();
            //var model = new List<EmpCourses>();
            //if (idnumber == null)
            //{
            //    idnumber = Session["IdNumber"]?.ToString();
            //}

            //Employees emp = dbHR.Employees.Where(x => x.IDNumber == idnumber).FirstOrDefault();
            //if (emp != null)
            //{
            //    model = dbHR.EmpCourses.Where(x => x.EmployeeID == emp.ID).ToList();
            //}
            #endregion
            ViewBag.FACULTY_NO = Session["FACULTY_NO"];
            ViewBag.StudentName = Session["StudentName"];
            ViewBag.STUDENT_NAME_S = Session["STUDENT_NAME_S"];
            var db = new SchoolAccGam3aEntities();
            Session["_EmpCoursesGridViewPartial"]= db.Usp_GetAllApprovedActivities_StudentId(studentId).ToList();
            return PartialView("_EmpCoursesGridViewPartial", db.Usp_GetAllApprovedActivities_StudentId(studentId).ToList());
        }

        [HttpPost]
        public ActionResult RemoveEmpCoursesFilesSession()
        {
            //remove files from temp folder
            var studentid = int.Parse(Session["StudentID"].ToString());
            var di = new DirectoryInfo(Server.MapPath($@"~\Content\UserFiles\{studentid}\tempfiles"));
            foreach (var file in di.EnumerateFiles())
            {
                file.Delete();
            }

            Session.Remove("EmpCoursesFileNames");
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #region FTP

        //[HttpPost, ValidateInput(false)]
        //public ActionResult EmpCoursesGridViewPartialAddNew(EmpCourses item)
        //{
        //    GetEmpCousesPermissionsViewBags();

        //    string idnumber = Session["IdNumber"]?.ToString();
        //    var model = new List<EmpCourses>();
        //    Employees emp = dbHR.Employees.Where(x => x.IDNumber == idnumber).FirstOrDefault();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var studentid = int.Parse(Session["StudentID"].ToString());
        //            var fileName = (string) Session["EmpCoursesFileNames"];
        //            item.EmployeeID = emp?.ID;
        //            dbHR.EmpCourses.Add(item);
        //            dbHR.SaveChanges();
        //            //تم اضافة ملفات للمساهمة وسيتم نقلها من الفولدر المؤقت الى فولد الطالب
        //            if (!string.IsNullOrEmpty(fileName))
        //            {
        //                //var filePathDirectory = $"~/Content/UserFiles/{studentid}/المساهمات/";
        //                //var destinationPath = Server.MapPath(filePathDirectory);
        //                //var currentFolder = new DirectoryInfo(destinationPath);
        //                //if (currentFolder.Exists == false)
        //                //{
        //                //    Directory.CreateDirectory(destinationPath);
        //                //}

        //                ////يقرا الملف من المكان الموقت
        //                //var bytes = System.IO.File.ReadAllBytes(
        //                //    Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + fileName));

        //                //var filename =
        //                //    $"{fileName.Split('.')[0]}_{DateTime.Now:dd-MM-yyyy-HH-mm}.{fileName.Split('.')[1]}";

        //                //يحفظ ملف الملاحظات فى فولدر الطالب
        //                //using (var fileStream =
        //                //    new FileStream(destinationPath + "\\" + filename, FileMode.Create,
        //                //        FileAccess.ReadWrite))
        //                //{
        //                //    fileStream.Write(bytes, 0, bytes.Length);
        //                //    fileStream.Close();
        //                //}

        //                const string ext = ".jpg";
        //                var datetime = DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss");
        //                var imagename = $"{item.ID}_{item.CourseTypes_ID}.{fileName.Split('.')[1]}";

        //                var image = Image.FromFile(
        //                    Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + fileName));
        //                var halfQualityImage = Files.CompressImage(image, 90);
        //                image.Dispose();

        //                var path = $"UserFiles/{studentid}/المساهمات/{imagename}";

        //                var isSavedToFtp = SaveToFTP(halfQualityImage, path);
        //                if (!isSavedToFtp)
        //                {
        //                    ViewData["EditError"] = "حدث خطأ أثناء رفع المرفقات، برجاء المحاولة مرة أخرى.";
        //                }

        //                item.CourseImagePath = imagename;
        //                dbHR.Entry(item).State = EntityState.Modified;
        //                dbHR.SaveChanges();

        //                //cleanup process
        //                var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/");
        //                var tempDirectoryInfo = new DirectoryInfo(tempFolder);
        //                foreach (var fileInfo in tempDirectoryInfo.GetFiles())
        //                {
        //                    fileInfo.Delete();
        //                }

        //                Session["EmpCoursesFileNames"] = new List<string>();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = "حدث خطأ أثناء الحفظ " + e.Message;
        //        }
        //    }
        //    else
        //        ViewData["EditError"] = "من فضلك، حاول تصحيح الأخطاء";

        //    model = dbHR.EmpCourses.Where(x => x.EmployeeID == emp.ID).ToList();
        //    return PartialView("_EmpCoursesGridViewPartial", model.ToList());
        //}

        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult EmpCoursesGridViewPartialAddNew(EmpCourses item)
        {
            GetEmpCousesPermissionsViewBags();
            string idnumber = Session["IdNumber"]?.ToString();
            var model = new List<EmpCourses>();
            Employees emp = dbHR.Employees.Where(x => x.IDNumber == idnumber).FirstOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    var studentid = int.Parse(Session["StudentID"].ToString());
                    var fileName = (string)Session["EmpCoursesFileNames"];
                    //تم اضافة ملفات للمساهمة وسيتم نقلها من الفولدر المؤقت الى فولد الطالب
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var filePathDirectory = $"~/Content/UserFiles/{studentid}/المساهمات/";
                        var destinationPath = Server.MapPath(filePathDirectory);
                        var currentFolder = new DirectoryInfo(destinationPath);

                        if (currentFolder.Exists == false)
                        {
                            Directory.CreateDirectory(destinationPath);
                        }

                        //يقرا الملف من المكان الموقت
                        var bytes = System.IO.File.ReadAllBytes(
                            Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + fileName));

                        var filename =
                            $"{fileName.Split('.')[0]}_{DateTime.Now:dd-MM-yyyy-HH-mm}.{fileName.Split('.')[1]}";

                        //يحفظ ملف الملاحظات فى فولدر الطالب
                        using (var fileStream =
                            new FileStream(destinationPath + "\\" + filename, FileMode.Create,
                                FileAccess.ReadWrite))
                        {
                            fileStream.Write(bytes, 0, bytes.Length);
                            fileStream.Close();
                        }


                        item.CourseImagePath = filename;

                        //cleanup process
                        var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/");
                        var tempDirectoryInfo = new DirectoryInfo(tempFolder);
                        foreach (var fileInfo in tempDirectoryInfo.GetFiles())
                        {
                            fileInfo.Delete();
                        }

                        Session["EmpCoursesFileNames"] = new List<string>();
                    }

                    item.EmployeeID = emp?.ID;
                    dbHR.EmpCourses.Add(item);
                    dbHR.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = "حدث خطأ أثناء الحفظ " + e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "من فضلك، حاول تصحيح الأخطاء";
            }

            model = dbHR.EmpCourses.Where(x => x.EmployeeID == emp.ID).ToList();
            return PartialView("_EmpCoursesGridViewPartial", model.ToList());
        }

        #region FTP

        //[HttpPost, ValidateInput(false)]
        //public ActionResult EmpCoursesGridViewPartialUpdate(EmpCourses item)
        //{
        //    GetEmpCousesPermissionsViewBags();

        //    string idnumber = Session["IdNumber"]?.ToString();

        //    Employees emp = dbHR.Employees.Where(x => x.IDNumber == idnumber).FirstOrDefault();
        //    var model = dbHR.EmpCourses.Where(x => x.EmployeeID == item.EmployeeID);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var modelItem = dbHR.EmpCourses.FirstOrDefault(it => it.ID == item.ID);
        //            if (modelItem != null)
        //            {
        //                var fileName = (string) Session["EmpCoursesFileNames"];
        //                //تم اضافة ملفات للمساهمة وسيتم نقلها من الفولدر المؤقت الى فولد الطالب
        //                if (!string.IsNullOrEmpty(fileName))
        //                {
        //                    var studentid = int.Parse(Session["StudentID"].ToString());


        //                    //var filePathDirectory = $"~/Content/UserFiles/{studentid}/المساهمات/";
        //                    //var destinationPath = Server.MapPath(filePathDirectory);
        //                    //var currentFolder = new DirectoryInfo(destinationPath);

        //                    //if (currentFolder.Exists == false)
        //                    //{
        //                    //    Directory.CreateDirectory(destinationPath);
        //                    //}

        //                    if (fileName != modelItem.CourseImagePath)
        //                    {
        //                        //يعنى تم تحديث الملف
        //                        //يقرا الملف من المكان الموقت
        //                        //var bytes = System.IO.File.ReadAllBytes(
        //                        //    Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + fileName));


        //                        var imagename = $"{item.ID}_{item.CourseTypes_ID}.{fileName.Split('.')[1]}";

        //                        var image = Image.FromFile(
        //                            Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + fileName));
        //                        var halfQualityImage = Files.CompressImage(image, 90);
        //                        image.Dispose();

        //                        var path = $"UserFiles/{studentid}/المساهمات/{imagename}";

        //                        var isSavedToFtp = SaveToFTP(halfQualityImage, path);
        //                        if (!isSavedToFtp)
        //                        {
        //                            ViewData["EditError"] = "حدث خطأ أثناء رفع المرفقات، برجاء المحاولة مرة أخرى.";
        //                        }

        //                        //var filename =
        //                        //    $"{fileName.Split('.')[0]}_{DateTime.Now:dd-MM-yyyy-HH-mm}.{fileName.Split('.')[1]}";

        //                        //يحفظ ملف الملاحظات فى فولدر الطالب
        //                        //using (var fileStream =
        //                        //    new FileStream(destinationPath + "\\" + filename, FileMode.Create,
        //                        //        FileAccess.ReadWrite))
        //                        //{
        //                        //    fileStream.Write(bytes, 0, bytes.Length);
        //                        //    fileStream.Close();
        //                        //}


        //                        modelItem.CourseImagePath = imagename;
        //                        //cleanup process
        //                        var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/");
        //                        var tempDirectoryInfo = new DirectoryInfo(tempFolder);
        //                        foreach (var fileInfo in tempDirectoryInfo.GetFiles())
        //                        {
        //                            fileInfo.Delete();
        //                        }

        //                        Session["EmpCoursesFileNames"] = "";
        //                    }
        //                }

        //                modelItem.EmployeeID = emp.ID;

        //                this.UpdateModel(modelItem);
        //                dbHR.SaveChanges();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = "حدث خطأ أثناء الحفظ " + e.Message;
        //        }
        //    }
        //    else
        //        ViewData["EditError"] = "من فضلك، حاول تصحيح الأخطاء";

        //    model = dbHR.EmpCourses.Where(x => x.EmployeeID == emp.ID);
        //    return PartialView("_EmpCoursesGridViewPartial", model.ToList());
        //}

        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult EmpCoursesGridViewPartialUpdate(EmpCourses item)
        {
            GetEmpCousesPermissionsViewBags();

            string idnumber = Session["IdNumber"]?.ToString();

            Employees emp = dbHR.Employees.Where(x => x.IDNumber == idnumber).FirstOrDefault();
            var model = dbHR.EmpCourses.Where(x => x.EmployeeID == item.EmployeeID);

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = dbHR.EmpCourses.FirstOrDefault(it => it.ID == item.ID);
                    if (modelItem != null)
                    {
                        var fileName = (string)Session["EmpCoursesFileNames"];
                        //تم اضافة ملفات للمساهمة وسيتم نقلها من الفولدر المؤقت الى فولد الطالب
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            var studentid = int.Parse(Session["StudentID"].ToString());
                            var filePathDirectory = $"~/Content/UserFiles/{studentid}/المساهمات/";
                            var destinationPath = Server.MapPath(filePathDirectory);
                            var currentFolder = new DirectoryInfo(destinationPath);

                            if (currentFolder.Exists == false)
                            {
                                Directory.CreateDirectory(destinationPath);
                            }

                            if (fileName != modelItem.CourseImagePath)
                            {
                                //يعنى تم تحديث الملف
                                //يقرا الملف من المكان الموقت
                                var bytes = System.IO.File.ReadAllBytes(
                                    Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/" + fileName));

                                var filename =
                                    $"{fileName.Split('.')[0]}_{DateTime.Now:dd-MM-yyyy-HH-mm}.{fileName.Split('.')[1]}";

                                //يحفظ ملف الملاحظات فى فولدر الطالب
                                using (var fileStream =
                                    new FileStream(destinationPath + "\\" + filename, FileMode.Create,
                                        FileAccess.ReadWrite))
                                {
                                    fileStream.Write(bytes, 0, bytes.Length);
                                    fileStream.Close();
                                }


                                modelItem.CourseImagePath = filename; //cleanup process
                                var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/tempfiles/");
                                var tempDirectoryInfo = new DirectoryInfo(tempFolder);
                                foreach (var fileInfo in tempDirectoryInfo.GetFiles())
                                {
                                    fileInfo.Delete();
                                }

                                Session["EmpCoursesFileNames"] = "";
                            }
                        }

                        modelItem.EmployeeID = emp.ID;

                        UpdateModel(modelItem);
                        dbHR.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = "حدث خطأ أثناء الحفظ " + e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "من فضلك، حاول تصحيح الأخطاء";
            }

            model = dbHR.EmpCourses.Where(x => x.EmployeeID == emp.ID);
            return PartialView("_EmpCoursesGridViewPartial", model.ToList());
        }

        #region FTP

        //[HttpPost, ValidateInput(false)]
        //public ActionResult EmpCoursesGridViewPartialDelete(int id)
        //{
        //    GetEmpCousesPermissionsViewBags();

        //    var model = dbHR.EmpCourses;
        //    if (id >= 0)
        //    {
        //        try
        //        {
        //            var item = model.FirstOrDefault(it => it.ID == id);
        //            if (item != null)
        //            {
        //                var studentid = int.Parse(Session["StudentID"].ToString());

        //                //var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/المساهمات/");

        //                //حذف الملفات للملاحظة
        //                if (!string.IsNullOrEmpty(item.CourseImagePath))
        //                {
        //                    var imagename = item.CourseImagePath;
        //                    var path = $"UserFiles/{studentid}/المساهمات/{imagename}";
        //                    var isDeletedFromFtp = DeleteFromFTP(path);
        //                    if (!isDeletedFromFtp)
        //                    {
        //                        return Json("حدث خطأ أثناء حذف المرفقات", JsonRequestBehavior.AllowGet);
        //                    }

        //                    //var filExists = System.IO.File.Exists(tempFolder + item.CourseImagePath);
        //                    //if (filExists)
        //                    //{
        //                    //    System.IO.File.Delete(tempFolder + item.CourseImagePath);
        //                    //}
        //                }

        //                model.Remove(item);
        //            }

        //            dbHR.SaveChanges();
        //        }
        //        catch (Exception e)
        //        {
        //            Json(e.Message, JsonRequestBehavior.AllowGet);
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }

        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult EmpCoursesGridViewPartialDelete(int id)
        {
            //المساهمات و المشاركات

            var permissionsCourseData = GetPermissionsfn(6);
            ViewBag.EmpCoursesCreate = permissionsCourseData.Create;
            ViewBag.EmpCoursesUpdate = permissionsCourseData.Update;
            ViewBag.EmpCoursesDelete = permissionsCourseData.Delete;

            var model = dbHR.EmpCourses;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.ID == id);
                    if (item != null)
                    {
                        var studentid = int.Parse(Session["StudentID"].ToString());

                        var tempFolder = Server.MapPath($"~/Content/UserFiles/{studentid}/المساهمات/");

                        //حذف الملفات للملاحظة
                        if (item.CourseImagePath != null)
                        {
                            var filExists = System.IO.File.Exists(tempFolder + item.CourseImagePath);
                            if (filExists)
                            {
                                System.IO.File.Delete(tempFolder + item.CourseImagePath);
                            }
                        }

                        model.Remove(item);
                    }

                    dbHR.SaveChanges();
                }
                catch (Exception e)
                {
                    Json(e.Message, JsonRequestBehavior.AllowGet);
                    ViewData["EditError"] = e.Message;
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        private void GetEmpCousesPermissionsViewBags()
        {
            //المساهمات و المشاركات

            var permissionsCourseData = GetPermissionsfn(6);
            ViewBag.EmpCoursesCreate = permissionsCourseData.Create;
            ViewBag.EmpCoursesUpdate = permissionsCourseData.Update;
            ViewBag.EmpCoursesDelete = permissionsCourseData.Delete;
        }

        public void EmpCoursesFilesUpload()
        {
            string[] errors;
            var listUploadedFilesNames = "";
            if (Session["EmpCoursesFileNames"] != null)
            {
                listUploadedFilesNames = (string)Session["EmpCoursesFileNames"];
            }

            var uploadedFiles = UploadControlExtension.GetUploadedFiles("EmpCoursesAttachments",
                null, out errors, (s, e) => { },
                (sender, e) =>
                {
                    UploadedFile[] files = ((MVCxUploadControl)sender).UploadedFiles;

                    foreach (var f in files)
                    {
                        if (f.IsValid && !string.IsNullOrWhiteSpace(f.FileName))
                        {
                            var studentid = int.Parse(Session["StudentID"].ToString());

                            var filePathDirectory = $"~/Content/UserFiles/{studentid}/tempfiles/";
                            var destinationPath = Server.MapPath(filePathDirectory);
                            DirectoryInfo currentFolder = new DirectoryInfo(destinationPath);

                            if (currentFolder.Exists == false)
                            {
                                Directory.CreateDirectory(destinationPath);
                            }

                            string resultFilePath = destinationPath + f.FileName;
                            f.SaveAs(resultFilePath);

                            string file = $"{f.FileName} ({f.ContentLength / 1024} KB)";
                            string url =
                                ((IUrlResolutionService)sender).ResolveClientUrl(filePathDirectory + f.FileName);

                            e.CallbackData += file + "|" + url + "|";
                        }
                    }
                }).ToList();


            foreach (var file in uploadedFiles)
            {
                listUploadedFilesNames = file.FileName;
            }

            Session["EmpCoursesFileNames"] = listUploadedFilesNames;
        }

        #region FTP

        //[HttpPost]
        //public ActionResult DownloadEmpCoursesArchive(int empCourseId)
        //{
        //    var studentId = int.Parse(Session["StudentID"].ToString());

        //    var empCourse = dbHR.EmpCourses.SingleOrDefault(x => x.ID == empCourseId);
        //    if (empCourse == null)
        //    {
        //        return null;
        //    }

        //    var tempfolder = Server.MapPath("~/Content/tempfiles/FTP/" + studentId + "/المساهمات/");
        //    var exists = Directory.Exists(tempfolder);
        //    if (exists)
        //    {
        //        //empty directory first
        //        var directoryInfo = new DirectoryInfo(tempfolder);
        //        foreach (var file in directoryInfo.GetFiles())
        //        {
        //            file.Delete();
        //        }

        //        foreach (var dir in directoryInfo.GetDirectories())
        //        {
        //            dir.Delete(true);
        //        }
        //    }
        //    else
        //    {
        //        Directory.CreateDirectory(tempfolder);
        //    }

        //    if (!string.IsNullOrEmpty(empCourse.CourseImagePath))
        //    {
        //        var path = $"/UserFiles/{studentId}/المساهمات/{empCourse.CourseImagePath}";
        //        var fileFromFtp = new StudentsController().DownloadFromFTP(path);
        //        if (fileFromFtp != null)
        //        {
        //            System.IO.File.WriteAllBytes(tempfolder + empCourse.CourseImagePath, fileFromFtp);
        //        }
        //    }

        //    var archive = ZipFile.Open(tempfolder + empCourseId + ".zip", ZipArchiveMode.Create);
        //    var d = new DirectoryInfo(tempfolder);
        //    foreach (var file in d.GetFiles().Where(x => !x.Name.EndsWith(".zip")))
        //    {
        //        archive.CreateEntryFromFile(tempfolder + file.Name, file.Name);
        //    }

        //    archive.Dispose();
        //    return Content($"/Content/tempfiles/FTP/{studentId}/المساهمات/" + empCourseId + ".zip");


        //    //if (System.IO.File.Exists(Server.MapPath("~/Content/" + empCourseId + ".zip")))
        //    //{
        //    //    System.IO.File.Delete(Server.MapPath("~/Content/" + empCourseId + ".zip"));
        //    //}
        //    //var empCourse = dbHR.EmpCourses.Find(empCourseId);
        //    //if (empCourse == null) return Content("/Content/" + empCourseId + ".zip");
        //    //using (var zipPth = ZipFile.Open(Server.MapPath
        //    //    ("~/Content/" + empCourseId + ".zip"), ZipArchiveMode.Create))
        //    //{
        //    //    zipPth.CreateEntryFromFile(Server.MapPath
        //    //            ("~/Content/UserFiles/" + studentId + "/المساهمات/" + empCourse.CourseImagePath),
        //    //        empCourse.CourseImagePath);
        //    //    zipPth.Dispose();
        //    //}

        //    //return Content("/Content/" + empCourseId + ".zip");
        //}

        #endregion

        [HttpPost]
        public ActionResult DownloadEmpCoursesArchive(int empCourseId)
        {
            if (System.IO.File.Exists(Server.MapPath("~/Content/" + empCourseId + ".zip")))
            {
                System.IO.File.Delete(Server.MapPath("~/Content/" + empCourseId + ".zip"));
            }

            var studentId = int.Parse(Session["StudentID"].ToString());


            var empCourse = dbHR.EmpCourses.Find(empCourseId);
            if (empCourse == null)
            {
                return Content("/Content/" + empCourseId + ".zip");
            }

            using (var zipPth = ZipFile.Open(Server.MapPath
                ("~/Content/" + empCourseId + ".zip"), ZipArchiveMode.Create))
            {
                zipPth.CreateEntryFromFile(Server.MapPath
                        ("~/Content/UserFiles/" + studentId + "/المساهمات/" + empCourse.CourseImagePath),
                    empCourse.CourseImagePath);
                zipPth.Dispose();
            }

            return Content("/Content/" + empCourseId + ".zip");
        }

        #endregion

        #region المخالفات المرورية

        public ActionResult TraficViolations()
        {
            return View();
        }

        public ActionResult TraficViolationsGridViewPartial(string idNumber)
        {
            var studentid = int.Parse(Session["StudentID"].ToString());

            if (!string.IsNullOrEmpty(idNumber))
            {
                studentid = int.Parse(idNumber);
            }

            var student = db.INTEGRATION_All_Students.FirstOrDefault(x => x.STUDENT_ID == studentid);
            if (student != null && student.STATUS_CODE != 1)
            {
                //طالب غير منتظم
                var result = db.proc_GetStudentsViolationsForNonRegularStudents(studentid);
                return PartialView(result);
            }

            var model = db.proc_GetStudentsViolations(studentid).ToList();

            return PartialView(model);
        }

        #endregion

        #region CustomFields

        public ActionResult CustomFields(string idNumber)
        {

            var permissions = GetPermissionsfn(26);
            ViewBag.Create = permissions.Create;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;

            if (!permissions.View)
            {
                return RedirectToAction("NotAuthorized", "Security");
            }

            return View(idNumber);
        }

        public static IEnumerable<CustomFields> GetParnetCustomFields()
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                return db.CustomFields.Where(c => c.ParentId == null).ToList();
            }
        }

        public static IEnumerable<SelectListItem> GetChildCustomFiled(int? customFieldId)
        {
            using (var db = new SchoolAccGam3aEntities())
            {
                if (customFieldId != null)
                {
                    return db.CustomFields.Where(c => c.ParentId == customFieldId).Select(x =>
                        new SelectListItem { Value = x.CustomFieldId.ToString(), Text = x.Key }).ToList();
                }
                else
                {
                    return db.CustomFields.Where(c => c.ParentId != null).Select(x =>
                        new SelectListItem { Value = x.CustomFieldId.ToString(), Text = x.Key }).ToList();
                    ;
                }
            }
        }


        public ActionResult GetChildCustomFields(int? customFieldId, string textField)
        {
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                //p.TextField = textField;
                if (customFieldId != null)
                {
                    //p.BindList(OnItemsRequestedByFilterCondition, OnItemRequestedByValue);
                    p.BindList(GetChildCustomFiled(customFieldId));
                }
            });
        }


        [ValidateInput(false)]
        public ActionResult StudentsCustomFieldsGridViewPartial(string idNumber)
        {

            var permissions = GetPermissionsfn(26);
            ViewBag.Create = permissions.Create;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;


            var model = db.StudentsCustomFields.Where(x => x.NationalId == idNumber);
            return PartialView("_StudentsCustomFieldsGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult StudentsCustomFieldsGridViewPartialAddNew(StudentsCustomFields item)
        {

            var permissions = GetPermissionsfn(26);
            ViewBag.Create = permissions.Create;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;


            var studentId = decimal.Parse(Session["StudentID"].ToString());
            if (ModelState.IsValid)
            {
                try
                {
                    var student = db.INTEGRATION_All_Students.SingleOrDefault(x => x.STUDENT_ID == studentId);
                    if (student != null)
                    {
                        item.StudentId = studentId.ToString(CultureInfo.InvariantCulture);
                        item.NationalId = student.NATIONAL_ID;
                    }

                    db.StudentsCustomFields.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "من فضلك، حاول تصحيح الأخطاء.";
            }

            return PartialView("_StudentsCustomFieldsGridViewPartial",
                db.StudentsCustomFields.Where(x => x.StudentId == studentId.ToString()).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult StudentsCustomFieldsGridViewPartialUpdate(StudentsCustomFields item)
        {

            var permissions = GetPermissionsfn(26);
            ViewBag.Create = permissions.Create;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;

            var studentId = decimal.Parse(Session["StudentID"].ToString());
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem =
                        db.StudentsCustomFields.FirstOrDefault(it =>
                            it.StudentCustomFieldId == item.StudentCustomFieldId);
                    if (modelItem != null)
                    {
                        var student = db.INTEGRATION_All_Students.SingleOrDefault(x => x.STUDENT_ID == studentId);
                        if (student != null)
                        {
                            item.StudentId = studentId.ToString(CultureInfo.InvariantCulture);
                            item.NationalId = student.NATIONAL_ID;
                            UpdateModel(modelItem);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "من فضلك، حاول تصحيح الأخطاء.";
            }

            return PartialView("_StudentsCustomFieldsGridViewPartial",
                db.StudentsCustomFields.Where(x => x.StudentId == studentId.ToString()).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult StudentsCustomFieldsGridViewPartialDelete(int StudentCustomFieldId)
        {

            var permissions = GetPermissionsfn(26);
            ViewBag.Create = permissions.Create;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;


            var studentId = decimal.Parse(Session["StudentID"].ToString());
            if (StudentCustomFieldId >= 0)
            {
                try
                {
                    var item = db.StudentsCustomFields.FirstOrDefault(it =>
                        it.StudentCustomFieldId == StudentCustomFieldId);
                    if (item != null)
                    {
                        db.StudentsCustomFields.Remove(item);
                    }

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            return PartialView("_StudentsCustomFieldsGridViewPartial",
                db.StudentsCustomFields.Where(x => x.StudentId == studentId.ToString()).ToList());
        }

        #endregion

        #region دورات كل الطلاب

        public ActionResult AllStudentsCourses()
        {
            ViewBag.Faculties =
               db.usp_getFaculties().Select(x => new { Value = x.FACULTY_NO, Text = x.FACULTY_NAME })
                   .OrderBy(x => x.Value).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult AllStudentsCourses(EmpCourses model, List<int> StudentsListBox)
        {
            foreach (var item in StudentsListBox)
            {
                string fileName = (string)Session["AllStudentsCoursesFileNames"];
                string idNumber = dbHR.Employees.Where(x => x.ID == item).SingleOrDefault()?.IDNumber;
                if (idNumber != null)
                {
                    var studentID = db.INTEGRATION_All_Students.Where(x => x.NATIONAL_ID == idNumber
                    && x.STATUS_CODE == 1).SingleOrDefault()?.STUDENT_ID;
                    if (studentID != null)
                    {


                        if (ModelState.IsValid)
                        {
                            try
                            {

                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    var filePathDirectory = $"~/Content/UserFiles/{studentID}/المساهمات/";
                                    var destinationPath = Server.MapPath(filePathDirectory);
                                    var currentFolder = new DirectoryInfo(destinationPath);

                                    if (currentFolder.Exists == false)
                                    {
                                        Directory.CreateDirectory(destinationPath);
                                    }

                                    //يقرا الملف من المكان الموقت
                                    var bytes = System.IO.File.ReadAllBytes(
                                        Server.MapPath($"~/Content/UserFiles/tempfiles/" + fileName));

                                    var filename =
                                        $"{fileName.Split('.')[0]}_{DateTime.Now:dd-MM-yyyy-HH-mm}.{fileName.Split('.')[1]}";

                                    //يحفظ ملف الملاحظات فى فولدر الطالب
                                    using (var fileStream =
                                        new FileStream(destinationPath + "\\" + filename, FileMode.Create,
                                            FileAccess.ReadWrite))
                                    {
                                        fileStream.Write(bytes, 0, bytes.Length);
                                        fileStream.Close();
                                    }


                                    model.CourseImagePath = filename;

                                    //cleanup process
                                    var tempFolder = Server.MapPath($"~/Content/UserFiles/tempfiles/");
                                    var tempDirectoryInfo = new DirectoryInfo(tempFolder);
                                    foreach (var fileInfo in tempDirectoryInfo.GetFiles())
                                    {
                                        fileInfo.Delete();
                                    }

                                    Session["AllStudentsCoursesFileNames"] = "";
                                }

                                model.EmployeeID = item;
                                dbHR.EmpCourses.Add(model);
                                dbHR.SaveChanges();

                            }
                            catch (Exception e)
                            {
                                ViewData["EditError"] = "حدث خطأ أثناء الحفظ " + e.Message;
                                break;
                            }

                        }
                    }
                }
            }
            return PartialView();
        }
        public void AllStudentsCoursesFilesUpload()
        {
            string[] errors;
            var listUploadedFilesNames = "";
            if (Session["AllStudentsCoursesFileNames"] != null)
            {
                listUploadedFilesNames = (string)Session["AllStudentsCoursesFileNames"];
            }

            var uploadedFiles = UploadControlExtension.GetUploadedFiles("AllStudentsCoursesAttachments",
                null, out errors, (s, e) => { },
                (sender, e) =>
                {
                    UploadedFile[] files = ((MVCxUploadControl)sender).UploadedFiles;

                    foreach (var f in files)
                    {
                        if (f.IsValid && !string.IsNullOrWhiteSpace(f.FileName))
                        {

                            var filePathDirectory = $"~/Content/UserFiles/tempfiles/";
                            var destinationPath = Server.MapPath(filePathDirectory);
                            DirectoryInfo currentFolder = new DirectoryInfo(destinationPath);

                            if (currentFolder.Exists == false)
                            {
                                Directory.CreateDirectory(destinationPath);
                            }
                            string imageName = DateTime.Now.ToString("ddMMyyyyHHmmss") + f.FileName;
                            string resultFilePath = destinationPath + imageName;
                            listUploadedFilesNames = imageName;
                            f.SaveAs(resultFilePath);

                            string file = $"{imageName} ({f.ContentLength / 1024} KB)";
                            string url =
                                ((IUrlResolutionService)sender).ResolveClientUrl(filePathDirectory + imageName);

                            e.CallbackData += file + "|" + url + "|";
                        }
                    }
                }).ToList();




            Session["AllStudentsCoursesFileNames"] = listUploadedFilesNames;
        }
        public ActionResult _StudentsPerFacultyListBox(int? facid, string selectedStudents)
        {
            var students = dbHR.PR_GetStudentsByFacandMaj(facid, null, selectedStudents).ToList();
            return PartialView("_StudentsPerFacultyListBox", students);
        }

        public ActionResult _AllStudentsCourses()
        {
            //var idNumbers = db.INTEGRATION_All_Students.Where(x=>x.NATIONAL_ID != null && x.STATUS_CODE==1).Select(X => X.NATIONAL_ID).ToList();
            //var empList = dbHR.Employees.Where(x => idNumbers.Any(P=>P== x.IDNumber)).ToList();

            //if (empList != null)
            //{
            //    model = dbHR.EmpCourses.Where(x => empList.Any(p=>p.ID== x.EmployeeID)).ToList();
            //}
            var model = dbHR.PR_StudentsCourses_SelectAll().ToList();
            return PartialView("_AllStudentsCourses", model);
        }

        #endregion


        [ValidateInput(false)]
        public ActionResult CoursesGridViewPartial()
        {
            var model = dbHR.EmpCourses;
            return PartialView("_CoursesGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AllCoursesGridViewPartialAddNew(EmpCourses item)
        {
            var model = dbHR.EmpCourses;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    dbHR.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
            }

            return PartialView("_CoursesGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AllCoursesGridViewPartialUpdate(EmpCourses item)
        {
            var model = dbHR.EmpCourses;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.ID == item.ID);
                    if (modelItem != null)
                    {
                        UpdateModel(modelItem);
                        dbHR.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
            }

            return PartialView("_CoursesGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AllCoursesGridViewPartialDelete(int id)
        {
            var model = dbHR.EmpCourses;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.ID == id);
                    if (item != null)
                    {
                        model.Remove(item);
                    }

                    dbHR.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            return PartialView("_CoursesGridViewPartial", model.ToList());
        }
    }
}
public class ActivityRequestsVM
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Duration { get; set; }
    public string Location { get; set; }
    public string Type { get; set; }
    public string Degree { get; set; }
    public string Ratio { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
}