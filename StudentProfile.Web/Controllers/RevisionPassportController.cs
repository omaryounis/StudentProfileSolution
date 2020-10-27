
using StudentProfile.DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using StudentProfile.Components;
using System.Data.Entity;
using System.Web.Hosting;

namespace StudentProfile.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class RevisionPassportController : Controller
    {
       
        notify notify = new notify();
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();
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
        // GET: RevisionPassport
        #region RevisionPassport
        public ActionResult Index()
        {
            var json = new JavaScriptSerializer().Serialize(GetPermissions(44).Data);
            var permissions = JsonConvert.DeserializeObject<Permissions>(json);
            ViewBag.View = permissions.View;
            ViewBag.Read = permissions.Read;
            ViewBag.Accept = permissions.Accept;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Refuse = permissions.Refuse;

            if (permissions.View)
            {
                return View();
            }
            return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult GetStudentsDocuments(bool isApproved)
        { 

            if (!isApproved)
            {
                var Query = dbSC.usp_get_new_students_docs().Select(x => new 
                {
                    UniversityStudent_ID = x.ID,
                    NamePerIdentity_Ar = x.NamePerIdentity_Ar,
                    Student_ID = x.Student_ID,
                    National_ID = x.National_ID,
                    MobileNumber = x.MobileNumber,
                    Nationality = x.NationalityName,
                    UnderRevesion = true,
                    UniversityEmail = x.UniversityEmail,
                    InsertDate = x.InsertDate!=null? x.InsertDate.Value.ToString("dd/MM/yyyy"):""
                    //dbSC.StudentDocuments.FirstOrDefault(c => c.UniversityStudent_ID == x.ID).InsertDate != null ?
                    //dbSC.StudentDocuments.FirstOrDefault(c => c.UniversityStudent_ID == x.ID).InsertDate.Value.Day + "/" +
                    //dbSC.StudentDocuments.FirstOrDefault(c => c.UniversityStudent_ID == x.ID).InsertDate.Value.Month + "/" +
                    //dbSC.StudentDocuments.FirstOrDefault(c => c.UniversityStudent_ID == x.ID).InsertDate.Value.Year : null
                    //NamePerIdentity_Ar = dbSC.UniversityStudents.FirstOrDefault(c => c.ID == x.Value).NamePerIdentity_Ar,
                    //Student_ID = dbSC.UniversityStudents.FirstOrDefault(c => c.ID == x.Value).Student_ID,
                    //National_ID = dbSC.UniversityStudents.FirstOrDefault(c => c.ID == x.Value).National_ID,
                    //MobileNumber = dbSC.UniversityStudents.FirstOrDefault(c => c.ID == x.Value).MobileNumber,
                    //Nationality = dbSC.UniversityStudents.FirstOrDefault(c => c.ID == x.Value).Nationality.NationalityName,
                    //UnderRevesion = true,
                    //UniversityEmail = dbSC.UniversityStudents.FirstOrDefault(c => c.ID == x.Value).UniversityEmail,
                    //InsertDate = dbSC.StudentDocuments.FirstOrDefault(c => c.UniversityStudent_ID == x.Value).InsertDate != null ?
                    //dbSC.StudentDocuments.FirstOrDefault(c => c.UniversityStudent_ID == x.Value).InsertDate.Value.Day + "/" +
                    //dbSC.StudentDocuments.FirstOrDefault(c => c.UniversityStudent_ID == x.Value).InsertDate.Value.Month + "/" +
                    //dbSC.StudentDocuments.FirstOrDefault(c => c.UniversityStudent_ID == x.Value).InsertDate.Value.Year : null
                }).ToList(); ;
                return this.JsonMaxLength(Query);

            }
            else
            {
                var Query = dbSC.usp_get_active_students_docs().Select(x=>new
                {
                    UniversityStudent_ID=x.ID,
                    NamePerIdentity_Ar = x.NamePerIdentity_Ar,
                    Student_ID =x.Student_ID,
                    National_ID =x.National_ID,
                    MobileNumber = x.MobileNumber,
                    Nationality = x.NationalityName,
                    UnderRevesion = false,
                    UniversityEmail =x.UniversityEmail,

                    InsertDate = x.InsertDate!=null? x.InsertDate.Value.ToString("dd/MM/yyyy"):""
                }).ToList(); ;
                return this.JsonMaxLength(Query);
            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetRevisionPassportByUser(int id, bool IsApproved)
        {
            var stData = dbSC.StudentDocuments.Where(x => x.UniversityStudent_ID == id &&((IsApproved==false &&(x.ApprovedDate==null && x.RefusedDate==null))
           || (IsApproved == true && x.ApprovedDate != null ))
            ).OrderByDescending(m=>m.ExpiryDate).ToList()
                .Select(x => new
                {
                    UniversityStudent_ID = x.UniversityStudents.Student_ID,
                    StudentDocumentID = x.StudentDocumentID,
                    Document_ID = x.Document_ID,
                    IdentityNumber = x.IdentityNumber,
                    IsActive = x.IsActive,
                    IsExpired=x.IsExpired,
                    ExpiryDate = x.ExpiryDate == null ? null : x.ExpiryDate.Value.ToString("dd/MM/yyyy"),
                    InsertDate = x.InsertDate == null ? null : x.InsertDate.Value.ToString("dd/MM/yyyy"),
                    ApprovedDate = x.ApprovedDate == null ? null : x.ApprovedDate.Value.ToString("dd/MM/yyyy"),
                    IsTransfer=x.IsTransfer,
                    RefusedDate = x.RefusedDate == null ? null : x.RefusedDate.Value.ToString("dd/MM/yyyy"),
                    RefusedNotes=x.RefusedNotes,
                    UserName = x.UserId != null ? x.DashBoard_Users.Name : "",
                    DocumentImage = $"{x.DocumentImage}",
                }).ToList();
            if (stData != null)
            {
                return Json(stData, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PreviewStudentDocument(int id)
        {
            string FileVirtualPath = null;
            var dbSch = new SchoolAccGam3aEntities();
            var doc = dbSch.StudentDocuments.Where(x => x.StudentDocumentID == id).SingleOrDefault();
            if (CurrentUser.IsStudent)
            {
                int studentID = Convert.ToInt32(CurrentUser.Username);
                if (doc.UniversityStudents.Student_ID == studentID)
                    FileVirtualPath = $"~/Content/UserFiles/{doc.UniversityStudents.Student_ID}/المستندات/{doc.DocumentImage}";
            }
            else
            {
                var json = new JavaScriptSerializer().Serialize(GetPermissions(44).Data);
                var readPermission = JsonConvert.DeserializeObject<Permissions>(json).Read;

                if (readPermission)
                {
                    FileVirtualPath = $"~/Content/UserFiles/{doc.UniversityStudents.Student_ID}/المستندات/{doc.DocumentImage}";
                }
            }
            if (FileVirtualPath != null)
            {
                if (!System.IO.File.Exists(HostingEnvironment.MapPath(FileVirtualPath)))
                {
                    return Content("");
                }

                byte[] FileBytes = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(FileVirtualPath));

                string base64String = Convert.ToBase64String(FileBytes, 0, FileBytes.Length);
                return Content(base64String);

                //===============================================================================================
                // ** File As DownLoad ** ...
                // return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
                //===============================================================================================
            }
            return Content("");
        }

        public ActionResult RemoveDocument(int id)
        {
            if (id > 0)
            {
                StudentDocumentsLog studentDocumentsLog = new StudentDocumentsLog();
                var StudentDocument = dbSC.StudentDocuments.Where(x => x.StudentDocumentID == id).SingleOrDefault();
              
                if (StudentDocument != null)
                {
                    var CurrentUser = Session["UserId"] as DashBoard_Users;
                    studentDocumentsLog.Document_ID = StudentDocument.Document_ID;
                    studentDocumentsLog.UniversityStudent_ID = StudentDocument.UniversityStudent_ID;
                    studentDocumentsLog.IdentityNumber = StudentDocument.IdentityNumber;
                    studentDocumentsLog.IssueDate = StudentDocument.IssueDate;
                    studentDocumentsLog.ExpiryDate = StudentDocument.ExpiryDate;
                    studentDocumentsLog.DocumentImage = StudentDocument.DocumentImage;
                    studentDocumentsLog.IsActive = StudentDocument.IsActive;
                    studentDocumentsLog.UserId = StudentDocument.UserId;
                    studentDocumentsLog.InsertDate = StudentDocument.InsertDate;
                    studentDocumentsLog.ApprovedDate = StudentDocument.ApprovedDate;
                    studentDocumentsLog.IsNew = StudentDocument.IsNew;
                    studentDocumentsLog.RefusedNotes = StudentDocument.RefusedNotes;
                    studentDocumentsLog.RefusedDate = StudentDocument.RefusedDate;
                    studentDocumentsLog.IsTransfer = StudentDocument.IsTransfer;
                    studentDocumentsLog.IsExpired = StudentDocument.IsExpired;
                    studentDocumentsLog.DeletedBy = CurrentUser.ID;
                    studentDocumentsLog.DeletedDate = DateTime.Now;
                    dbSC.StudentDocumentsLog.Add(studentDocumentsLog);
                    dbSC.StudentDocuments.Remove(StudentDocument);
                }
                dbSC.SaveChanges();
                return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(notify = new notify() { Message = "خطأ اثناء الحذف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ActiveDocument(int id)
        {
            var CurrentUser = Session["UserId"] as DashBoard_Users;

            var Document = dbSC.StudentDocuments.Find(id);
            if (Document != null)
            {
                var AllStDocs = dbSC.StudentDocuments.Where(x => x.UniversityStudent_ID == Document.UniversityStudent_ID && x.Document_ID == Document.Document_ID).ToList();
                foreach (var item in AllStDocs)
                Document.IsActive = true;
                Document.IsNew = false;
                Document.ApprovedDate = DateTime.Now;
                Document.UserId = CurrentUser.ID;
                dbSC.Entry(Document).State = EntityState.Modified;
                dbSC.SaveChanges();
                return Json(notify = new notify() { Message = "تم الاعتماد بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "success", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApprovedDocuments(ApprovedDocumentsVM model)
        {

            var CurrentUser = Session["UserId"] as DashBoard_Users;

            var UniversityStudentIds = model.UniversityStudentIds.Split(',');
            foreach (var item in UniversityStudentIds)
            {
                int universityStudent_Id = int.Parse(item);
                dbSC.StudentDocuments.Where(x => x.UniversityStudent_ID == universityStudent_Id).ToList().ForEach(x => { x.IsActive = false; x.IsNew = false; });

                var Identity = dbSC.StudentDocuments.Where(x => x.UniversityStudent_ID == universityStudent_Id && x.Document_ID == 1).OrderByDescending(x => x.ExpiryDate).FirstOrDefault();
                if (Identity != null)
                {
                    Identity.IsActive = true;
                    Identity.UserId = CurrentUser.ID;
                    Identity.ApprovedDate = DateTime.Now;
                }
                var Passport = dbSC.StudentDocuments.Where(x => x.UniversityStudent_ID == universityStudent_Id && x.Document_ID == 2).OrderByDescending(x => x.ExpiryDate).FirstOrDefault();
                if (Passport != null)
                {
                    Passport.IsActive = true;
                    Passport.UserId = CurrentUser.ID;
                    Passport.ApprovedDate = DateTime.Now;
                }
                var Visa = dbSC.StudentDocuments.Where(x => x.UniversityStudent_ID == universityStudent_Id && x.Document_ID == 3).OrderByDescending(x => x.ExpiryDate).FirstOrDefault();
                if (Visa != null)
                {
                    Visa.IsActive = true;
                    Visa.UserId = CurrentUser.ID;
                    Visa.ApprovedDate = DateTime.Now;
                }
            }
            try
            {
                dbSC.SaveChanges();
                return Json(notify = new notify() { Message = "تم الموافقة بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "success", status = 500 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RefusedDocuments(RefusedDocumentsVM model)
        {

            var CurrentUser = Session["UserId"] as DashBoard_Users;


            var studentDocument = dbSC.StudentDocuments.Where(x => x.StudentDocumentID == model.studentDocumentId).SingleOrDefault();

                if (studentDocument != null)
                {
                    studentDocument.IsActive = false;
                    studentDocument.IsNew = false;
                    studentDocument.UserId = CurrentUser.ID;
                    studentDocument.RefusedDate= DateTime.Now;
                    studentDocument.RefusedNotes = model.refusedNotes;
                }
               
            try
            {
                dbSC.Entry(studentDocument).State = EntityState.Modified;
                dbSC.SaveChanges();
                return Json(notify = new notify() { Message = "تم الرفض بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "success", status = 500 }, JsonRequestBehavior.AllowGet);
            }
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
            int userId = user.ID;

            var perm = CheckPermissions.IsAuthorized(userId, screenId);

            var permissions = new Permissions();
            foreach (var permission in perm)
            {

                if (permission == "قبول")
                {
                    permissions.Accept = true;
                }
                else if (permission == "مشاهدة")
                {
                    permissions.Read = true;
                }
                else if (permission == "رفض")
                {
                    permissions.Refuse = true;
                }
                else if (permission == "عرض")
                {
                    permissions.View = true;
                }
                else if (permission == "حذف")
                {
                    permissions.Delete = true;
                }
            }

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }

        private class Permissions
        {
            public bool Accept { get; set; }
            public bool Refuse { get; set; }
            public bool Delete { get; set; }
            public bool Read { get; set; }
            public bool View { get; set; }
        }
        #endregion
    }

    public class studentDataVM
    {
        public decimal STUDENT_ID { get; set; }
        public string STUDENT_NAME { get; set; }
        public string NATIONAL_ID { get; set; }
        public string PASSPORTIMG { get; set; }
    }
    

    public class ApprovedDocumentsVM
    {
        public string UniversityStudentIds { get; set; }
    }
    public class RefusedDocumentsVM
    {
        public int studentDocumentId { get; set; }
        public string refusedNotes { get; set; }
    }


}