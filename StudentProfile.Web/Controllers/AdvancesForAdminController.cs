using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    public partial class AdvancesForAdminController : Controller
    {
        private notify _notify;
        private readonly SchoolAccGam3aEntities _dbSc = new SchoolAccGam3aEntities();

        // GET: AdvancesForAdmin
        public ActionResult StudentAdvanceRequestsForAdmin()
        {
            return View();
        }

        public ActionResult GetStudents()
        {
            var students = _dbSc.INTEGRATION_All_Students.Where(x => x.STATUS_CODE == 1 && x.JOIN_DATE > new DateTime(2015, 01, 01))
                .Select(x => new SelectListItem
                {
                    Value = x.STUDENT_ID.ToString(),
                    Text = x.STUDENT_NAME
                }).ToList();

            return this.JsonMaxLength(students);
        }

        [HttpPost]
        public ActionResult SaveAdvanceRequestByAdmin(int advanceTypeId, decimal advanceValue, string advanceRequestNotes, int studentId)
        {
            if (ModelState.IsValid)
            {
                var user = Session["UserId"] as DashBoard_Users;

                try
                {
                    if (_dbSc.AdvanceRequests.Any(x => x.AdvanceSettings_Id == advanceTypeId && x.Student_Id == studentId && x.IsCanceled != true && x.ApprovedbyID == null && x.RefusedbyID == null))
                        return Json(_notify = new notify() { Message = "عفوا لقد تم إدخال هذا الطلب من قبل وجاري مراجعته", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                    if (advanceValue <= 0)
                        return Json(_notify = new notify() { Message = "لا يمكن أن يكون القيمة المطلوبة أقل من أو تساوي صفر", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);


                    var advanceRequests = new AdvanceRequests
                    {
                        AdvanceSettings_Id = advanceTypeId,
                        InsertionDate = DateTime.Now,
                        Student_Id = studentId,
                        RequestNotes = advanceRequestNotes,
                        RequestedValue = advanceValue,
                        User_Id = user.ID
                    };
                    _dbSc.AdvanceRequests.Add(advanceRequests);
                    _dbSc.SaveChanges();

                    var type = _dbSc.AdvanceSettings.SingleOrDefault(x => x.AdvanceSettingId == advanceTypeId)?.AdvanceType;

                    if (type == "S")
                    {
                        var file = (HttpPostedFileBase)Session["AdvanceRequestFile"];

                        // file
                        var rootFolder = Server.MapPath("~/Content/UserFiles/");
                        var studentFolder = Path.Combine(rootFolder, studentId.ToString());
                        var currentFolder = new DirectoryInfo(studentFolder);

                        if (currentFolder.Exists == false)
                        {
                            Directory.CreateDirectory(studentFolder);
                        }

                        var dSecurity = currentFolder.GetAccessControl();


                        dSecurity.AddAccessRule(new FileSystemAccessRule(
                            new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                            PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                        currentFolder.SetAccessControl(dSecurity);


                        var stAdvancesFolder = Path.Combine(rootFolder, studentId.ToString(), "Advances");

                        if (!new DirectoryInfo(stAdvancesFolder).Exists)
                        {
                            Directory.CreateDirectory(stAdvancesFolder);
                        }


                        var dSecurityForAdvances = new DirectoryInfo(stAdvancesFolder).GetAccessControl();

                        dSecurityForAdvances.AddAccessRule(new FileSystemAccessRule(
                            new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                            PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                        new DirectoryInfo(stAdvancesFolder).SetAccessControl(dSecurityForAdvances);


                        // Start From Here

                        string filename = String.Format("{0}_{1}.{2}", file.FileName.Split('.')[0],
                            DateTime.Now.ToString("dd-MM-yyyy-HH-mm"), file.FileName.Split('.')[1]);

                        string path = Path.Combine(stAdvancesFolder, filename);


                        Stream stream = file.InputStream;
                        byte[] bytes = ReadToEnd(stream);
                        FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

                        fileStream.Write(bytes, 0, bytes.Length);
                        fileStream.Close();


                        var advanceRequestsAttachment = new AdvanceRequestsAttachment
                        {
                            FileName = filename,
                            Path = path,
                            InsertDate = DateTime.Now,
                            UserId = user.ID,
                            AdvanceRequests_Id = advanceRequests.ID,

                        };
                        _dbSc.AdvanceRequestsAttachment.Add(advanceRequestsAttachment);
                        _dbSc.SaveChanges();

                        Session["AdvanceRequestFile"] = null;
                    } 

                }
                catch (Exception)
                {
                    return Json(_notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(_notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);

        }

        #region اضافة الإعانات من قبل الادمن
        [HttpGet]
        public ActionResult StudentSubsidyRequestsForAdmin()
        {
            return View();
        }

        #region FileUploader

        [HttpPost]
        public JsonResult UploadFiles()
        {
            Session["AdvanceRequestFile"] = null;
            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                Session["AdvanceRequestFile"] = file;
            }
            return Json(0);
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

        [HttpGet]
        public ActionResult DownloadAdvanceAttachment(int advanceRequestId)
        {
            var dbSch = new SchoolAccGam3aEntities();
            var FileVirtualPath = dbSch.AdvanceRequestsAttachment.Where(x => x.AdvanceRequests_Id == advanceRequestId).FirstOrDefault()?.Path;
            if (FileVirtualPath != null)
            {
                byte[] FileBytes = System.IO.File.ReadAllBytes(FileVirtualPath);
                return File(FileBytes, "application/pdf");

                //===============================================================================================
                // ** File As DownLoad ** ...
                // return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
                //===============================================================================================
            }
            return Content("");
        }

        #endregion




        #endregion

    }
}