using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class ViolationsDecisionsController : Controller
    {
        notify notify = new notify();
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();

        // GET: ViolationsDecisions
        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(60);
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

        public ActionResult SaveViolationsDecisions(ViolationsDecisions model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model != null)
                    {
                        var user = Session["UserId"] as DashBoard_Users;

                        if (model.Id == 0 || model.Id == null)
                        {


                            model.InsertedBy = user.ID;
                            model.InsertDate = DateTime.Now;

                            dbSC.ViolationsDecisions.Add(model);
                            dbSC.SaveChanges();
                            return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var entry = dbSC.ViolationsDecisions.Find(model.Id);
                            entry.Name = model.Name;
                            entry.Notes = model.Notes;
                            entry.LastEditDate = DateTime.Now;
                            entry.LastEdittedBy = user.ID;
                            dbSC.SaveChanges();
                            return Json(notify = new notify() { Message = "تم التعديل بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                catch (Exception)
                {

                    return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetViolationsDecisions()
        {
            var data = dbSC.ViolationsDecisions.Select(x => new
            {
                x.Id,
                x.Name,
                x.Notes
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetViolationsDecisionsById(int ID)
        {
            if (ID > 0)
            {
                var data = dbSC.ViolationsDecisions.Find(ID);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        [HttpDelete]
        public ActionResult DeleteViolationsDecisions(int ID)
        {
            if (ID > 0)
            {
                var row = dbSC.ViolationsDecisions.Find(ID);
                dbSC.ViolationsDecisions.Remove(row);
                try
                {
                    dbSC.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(notify = new notify() { Message = "لايمكن حذف القرار لارتباطة بعمليات اخري", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
                return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحذف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        // "file" is the value of the FileUploader's "name" option
        public void Upload(HttpPostedFileBase fileSent)
        {
            if (Session["AddViolationFiles"] == null)
                Session["AddViolationFiles"] = new List<HttpPostedFileBase>();

            var files = (List<HttpPostedFileBase>)Session["AddViolationFiles"];

            files.Add(fileSent);
            Session["AddViolationFiles"] = files;
        }
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        public string GetIssueNumberString(int issueId)
        {
            dbSC.Configuration.LazyLoadingEnabled = false;

            var categoryId = dbSC.Issues.Single(x => x.Id == issueId).CategoryId;

            var lastIssueNumber = dbSC.ViolationOfStudents.Where(x => x.IssueId == issueId).OrderByDescending(x => x.Id).Take(1)
                .Select(x => x.ViolationNumber)
                .ToList()
                .FirstOrDefault();


            var violationNumber = 0;
            if (!string.IsNullOrWhiteSpace(lastIssueNumber))
            {
                violationNumber = int.Parse(lastIssueNumber.Split('-')[2]);
            }

            violationNumber += 1;
            return $"{categoryId}-{issueId}-{violationNumber}";
        }


        public ActionResult SaveViolation(int studentId, int issueId, DateTime violationDate, string violationDescription)
        {
            var violationOfStudent = new ViolationOfStudents
            {
                StudentId = studentId,
                IssueId = issueId,
                ViolationDate = violationDate,
                ViolationDescription = violationDescription,
                InsertDate = DateTime.Now,
                ViolationNumber = GetIssueNumberString(issueId),
                SupervisorId = ((DashBoard_Users)HttpContext.Session["UserId"]).ID,
                IsForword = false
            };
            dbSC.ViolationOfStudents.Add(violationOfStudent);
            dbSC.SaveChanges();



            var files = (List<HttpPostedFileBase>)Session["AddViolationFiles"];

            if (files != null)
            {
                if (files.Count > 0)
                {

                    var rootFolder = Server.MapPath("~/Content/UserFiles/");
                    var studentFolder = Path.Combine(rootFolder, studentId.ToString());
                    var currentFolder = new DirectoryInfo(studentFolder);

                    if (currentFolder.Exists == false)
                        Directory.CreateDirectory(studentFolder);


                    var dSecurity = currentFolder.GetAccessControl();


                    dSecurity.AddAccessRule(new FileSystemAccessRule(
                        new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                    currentFolder.SetAccessControl(dSecurity);


                    var studentViolationFolder = Path.Combine(rootFolder, studentId.ToString(), "مخالفات");

                    if (!new DirectoryInfo(studentViolationFolder).Exists)
                    {
                        Directory.CreateDirectory(studentViolationFolder);

                    }


                    var dSecurityForViolation = new DirectoryInfo(studentViolationFolder).GetAccessControl();

                    dSecurityForViolation.AddAccessRule(new FileSystemAccessRule(
                        new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                        PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    new DirectoryInfo(studentViolationFolder).SetAccessControl(dSecurityForViolation);





                    foreach (var file in files)
                    {

                        string filename = String.Format("{0}_{1}.{2}", file.FileName.Split('.')[0],
                            DateTime.Now.ToString("dd-MM-yyyy-HH-mm"), file.FileName.Split('.')[1]);

                        string path = Path.Combine(studentViolationFolder, filename);


                        Stream stream = file.InputStream;
                        byte[] bytes = ReadToEnd(stream);
                        FileStream fileStream = new FileStream(path, FileMode.Create,
                            FileAccess.ReadWrite);

                        fileStream.Write(bytes, 0, bytes.Length);
                        fileStream.Close();


                        var violationOfStudentsAttachment = new ViolationOfStudentsAttachment
                        {
                            FileName = filename,
                            Path = path,
                            InsertDate = DateTime.Now,
                            UserId = (Session["UserId"] as DashBoard_Users).ID,
                            ViolationOfStudentsId = violationOfStudent.Id
                        };
                        dbSC.ViolationOfStudentsAttachment.Add(violationOfStudentsAttachment);
                        dbSC.SaveChanges();
                    }
                }
            }


            Session["AddViolationFiles"] = null;
            return Content("");
        }


        public ActionResult ViolationDecisionTaking()
        {
            var permissions = GetPermissionsFn(64);
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


        public JsonResult GetHangedViolationsForDecisionTaking()
        {
            //Where(x => x.IsRefused == false && x.IsForword == false)
            var query = dbSC.ViolationOfStudents.Select(o => new
            {
                o.Id,
                o.ViolationDate,
                o.InsertDate,
                o.ViolationDescription,
                o.ViolationNumber,
                o.Issues.IssueDescription,
                IssuesCategory = o.Issues.IssuesCategories.Name,
                studentName = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => (int?)x.STUDENT_ID == o.StudentId).STUDENT_NAME,
                studentId = (decimal?)dbSC.INTEGRATION_All_Students.FirstOrDefault(x => (int?)x.STUDENT_ID == o.StudentId).STUDENT_ID,
                nationalId = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => (int?)x.STUDENT_ID == o.StudentId).NATIONAL_ID,
                ViolationCount = dbSC.ViolationOfStudents.Count(c => c.StudentId == o.StudentId && c.IssueId == o.IssueId),
                o.ViolationOfStudentsAttachment.FirstOrDefault(s => s.ViolationOfStudentsId == o.Id).FileName,
                ForwordUser = o.ViolationForwords.FirstOrDefault().DashBoard_Users1.Name,
                o.DecisionId,
                Decision = o.DecisionId == null ? "لم يتم اتخاذ قرار" : "تم اتخاذ قرار",
                o.FilePath
            }).AsEnumerable()
              .Select(o => new
              {
                  o.Id,
                  o.studentId,
                  o.nationalId,
                  ViolationDate = o.ViolationDate.ToString("dd/MM/yyyy"),
                  InsertDate = o.InsertDate.ToString("dd/MM/yyyy"),
                  o.ViolationDescription,
                  o.ViolationNumber,
                  o.IssueDescription,
                  o.IssuesCategory,
                  o.studentName,
                  o.ViolationCount,
                  ViolationAttachment = o.FileName != null && o.studentId != null ? $"../Content/UserFiles/{o.studentId } مخالفات/{o.FileName}" : null,
                  ForwordFile = o.FilePath != null ? $"../Content/UserFiles/قرارات المخالفات الموجة/{o.FilePath}" : null,
                  o.ForwordUser,
                  o.DecisionId,
                  o.Decision
              }).ToArray();


            return Json(query, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult RefuseViolation(int id, string recommendations)
        {
            var violation = dbSC.ViolationOfStudents.Single(x => x.Id == id);

            if (violation.IsAccepted || violation.IsRefused)
                return Content("عفوا تم اتخاذ قرار مسبقا فى هذه المخالفة من فضلك قم باعادة تحميل الصفحة");



            violation.IsRefused = true;
            violation.Recommendations = recommendations;
            violation.DecisionDate = DateTime.Now;
            violation.DecisionTakerId = ((DashBoard_Users)HttpContext.Session["UserId"]).ID;

            dbSC.SaveChanges();

            return Content("");
        }


        public JsonResult GetDecisionList()
        {
            var violationDecisions = dbSC.ViolationsDecisions.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            return Json(violationDecisions, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetViolationsForDecisionByUser()
        {
            var user = Session["UserId"] as DashBoard_Users;
            var query = dbSC.ViolationForwords.Where(x => x.ViolationOfStudents.IsRefused == false && x.ViolationOfStudents.IsForword == true && x.UserId == user.ID).Select(o => new
            {
                Id = o.ViolationOfStudents.Id,
                IsAccepted = o.ViolationOfStudents.IsAccepted,
                ViolationDate = o.ViolationOfStudents.ViolationDate,
                InsertDate = o.ViolationOfStudents.InsertDate,
                o.ViolationOfStudents.ViolationDescription,
                o.ViolationOfStudents.ViolationNumber,
                o.ViolationOfStudents.Issues.IssueDescription,
                IssuesCategory = o.ViolationOfStudents.Issues.IssuesCategories.Name,
                studentName = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => (int?)x.STUDENT_ID == o.ViolationOfStudents.StudentId).STUDENT_NAME,
                studentId = (decimal?)dbSC.INTEGRATION_All_Students.FirstOrDefault(x => (int?)x.STUDENT_ID == o.ViolationOfStudents.StudentId).STUDENT_ID,
                nationalId = dbSC.INTEGRATION_All_Students.FirstOrDefault(x => (int?)x.STUDENT_ID == o.ViolationOfStudents.StudentId).NATIONAL_ID,
                ViolationCount = dbSC.ViolationOfStudents.Where(c => c.StudentId == o.ViolationOfStudents.StudentId && c.IssueId == o.ViolationOfStudents.IssueId).Count(),
                FileName = o.ViolationOfStudents.ViolationOfStudentsAttachment.FirstOrDefault(s => s.ViolationOfStudentsId == o.Id).FileName
            }).ToList().Select(o => new
            {
                o.Id,
                o.IsAccepted,
                o.studentId,
                o.nationalId,
                ViolationDate = o.ViolationDate.ToString("dd/MM/yyyy HH:mm:ss"),
                InsertDate = o.InsertDate.ToString("dd/MM/yyyy HH:mm:ss"),
                o.ViolationDescription,
                o.ViolationNumber,
                o.IssueDescription,
                o.IssuesCategory,
                o.studentName,
                o.ViolationCount,
                ViolationAttachment = o.FileName != null ? $"../Content/UserFiles/{o.studentId}/مخالفات/{o.FileName}" : null,
            }).ToList();


            return Json(query, JsonRequestBehavior.AllowGet);
        }



        public void UploadViolationForwordsFiles()
        {
            if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/قرارات المخالفات الموجة")))
                Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/قرارات المخالفات الموجة"));

            Session["UploadFile"] = Request.Files[0];

        }


        [HttpPost]
        public ActionResult ApproveViolation(int violationOfStudentId, int decisionId, string recommendations)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                var violationOfStuden = dbSC.ViolationOfStudents.Single(x => x.Id == violationOfStudentId);
                if (violationOfStuden.IsAccepted || violationOfStuden.IsRefused)
                    return Content(@"عفوا لقد تم اتخاذ قرار مسبقا فى فى هذه المخالفة من فضلك قم باعادة تحميل الصفحة");


                violationOfStuden.IsAccepted = true;
                violationOfStuden.Recommendations = recommendations;
                violationOfStuden.DecisionTakerId = ((DashBoard_Users)HttpContext.Session["UserId"]).ID;
                violationOfStuden.DecisionDate = DateTime.Now;
                violationOfStuden.DecisionId = decisionId;

                //ملف قرار المخالفة الموجهةت 
                if (Session["UploadFile"] != null)
                {
                    var UploadFile = (HttpPostedFileBase)Session["UploadFile"];
                    UploadFile.SaveAs(Server.MapPath($"~/Content/UserFiles/قرارات المخالفات الموجة/[{violationOfStudentId}]_{Guid.NewGuid().ToString()}_{UploadFile.FileName}"));
                    violationOfStuden.FilePath = $"[{violationOfStudentId}]_{Guid.NewGuid().ToString()}_{UploadFile.FileName}";
                }
                dbSC.SaveChanges();



                var studentNotes = new StudentNotes
                {
                    StDetailId = violationOfStuden.StudentId,
                    NoteDetails = violationOfStuden.ViolationDescription,
                    IssueId = violationOfStuden.IssueId,
                    NoteDate = violationOfStuden.ViolationDate,
                    Punishment = dbSC.ViolationsDecisions.Single(x => x.Id == decisionId).Name,
                    AddedBy = violationOfStuden.SupervisorId,
                    LastUpdatedBy = ((DashBoard_Users)HttpContext.Session["UserId"]).ID,
                    PunichedBy = ((DashBoard_Users)HttpContext.Session["UserId"]).ID,
                    IsTopSecret = false,
                    IsSecret = false
                };

                dbSC.StudentNotes.Add(studentNotes);
                dbSC.SaveChanges();


                scope.Complete();
            }

            return Content("");

        }

        /*************************************** Forword Violation ***************************************/
        public ActionResult GetUsersToForword()
        {
            var users = dbSC.DashBoard_Users.Where(x => x.DashBoard_UserGroups.FirstOrDefault().Group_ID == 16).Select(x => new
            {
                ID = x.ID,
                Name = x.Name,
                countViolation = dbSC.ViolationForwords.Where(c => c.UserId == x.ID && c.ViolationOfStudents.DecisionId == null).Count()
            }).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserForForwordByViolationId(int Id)
        {
            var Violation = dbSC.ViolationForwords.FirstOrDefault(x => x.ViolationId == Id);
            if (Violation != null)
                return Json(Violation.UserId, JsonRequestBehavior.AllowGet);

            return Json(0, JsonRequestBehavior.AllowGet);
        }

        //Save Forword Violation To User
        public ActionResult ForwordViolationToUser(ViolationForwords model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    var user = Session["UserId"] as DashBoard_Users;

                    if (model.Id == 0 || model.Id == null)
                    {
                        var Violation = dbSC.ViolationForwords.FirstOrDefault(x => x.ViolationId == model.ViolationId);
                        if (Violation != null)
                        {
                            Violation.UserId = model.UserId;
                            Violation.InsertedBy = user.ID;
                            Violation.InsertedDate = DateTime.Now;
                        }
                        else
                        {
                            model.InsertedBy = user.ID;
                            model.InsertedDate = DateTime.Now;
                            dbSC.ViolationForwords.Add(model);
                            dbSC.ViolationOfStudents.Find(model.ViolationId).IsForword = true;
                        }
                        try
                        {
                            dbSC.SaveChanges();
                        }
                        catch (Exception)
                        {
                            return Json(notify = new notify() { Message = "خطأ اثناء التوجية", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            return Json(notify = new notify() { Message = "تم التوجية بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }


        /*************************************** User Violation Decision ***************************************/
        public ActionResult ViolationDecisionByUser()
        {
            var permissions = GetPermissionsFn(75);
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



    }
}