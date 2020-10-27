
using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class TravelOrderPhasesController : Controller
    {
        notify notify = new notify();
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();

        [HttpPost]
        ///UploadImage
        public void UploadSignature(int id)
        {
            var currentUser = id;
            if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}")))
                Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}"));

            if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}/المستندات")))
                Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}/المستندات"));

            Session["SignatureFile"] = Request.Files[0];
            //file.SaveAs(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}/المستندات/[{int.Parse(currentUser.ToString())}]Signature.jpg"));
            //Session["Signature"] = file.FileName;
        }
        public void UploadStampOne(int id)
        {
            var currentUser = id;
            if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}")))
                Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}"));

            if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}/المستندات")))
                Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}/المستندات"));

            Session["StampOneFile"] = Request.Files[0];
            //file.SaveAs(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}/المستندات/[{int.Parse(currentUser.ToString())}]StampOne.jpg"));
            //Session["StampOne"] = file.FileName;
        }
        public void UploadStampTow(int id)
        {
            var currentUser = id;
            if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}")))
                Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}"));

            if (!Directory.Exists(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}/المستندات")))
                Directory.CreateDirectory(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}/المستندات"));
            Session["StampTowFile"] = Request.Files[0];
            //file.SaveAs(Server.MapPath($"~/Content/UserFiles/{int.Parse(currentUser.ToString())}/المستندات/[{int.Parse(currentUser.ToString())}]StampTow.jpg"));
            //Session["StampTow"] = file.FileName;
        }


        // GET: TravelOrderPhases
        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(46);
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

        public ActionResult GetUsers()
        {
            var users = dbSC.DashBoard_Users.Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPhases()
        {
            var phases = dbSC.TravelOrderPhase.Select(x => new SelectListItem { Text = x.PhaseName, Value = x.ID.ToString() }).OrderBy(x => x.Value).ToList();
            return Json(phases, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SavePhasesUsers(PhasesUsers model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    HttpPostedFileBase SignatureFile = null;
                    HttpPostedFileBase StampOneFile = null;
                    HttpPostedFileBase StampTowFile = null;
                    if (Session["SignatureFile"] != null)
                    {
                        string GuidKey = Guid.NewGuid().ToString();

                        SignatureFile = (HttpPostedFileBase)Session["SignatureFile"];
                        SignatureFile.SaveAs(Server.MapPath($"~/Content/UserFiles/{int.Parse(model.UserID.ToString())}/المستندات/{GuidKey}[{int.Parse(model.UserID.ToString())}]Signature.jpg"));

                        model.Signature = $"{GuidKey}[{model.UserID}]Signature.jpg";
                    }

                    if (Session["StampOneFile"] != null)
                    {
                        string GuidKey = Guid.NewGuid().ToString();

                        StampOneFile = (HttpPostedFileBase)Session["StampOneFile"];
                        StampOneFile.SaveAs(Server.MapPath($"~/Content/UserFiles/{int.Parse(model.UserID.ToString())}/المستندات/{GuidKey}[{int.Parse(model.UserID.ToString())}]StampOne.jpg"));

                        model.Stamp1 = $"{GuidKey}[{model.UserID}]StampOne.jpg";
                    }

                    if (Session["StampTowFile"] != null)
                    {
                        string GuidKey = Guid.NewGuid().ToString();

                        StampTowFile = (HttpPostedFileBase)Session["StampTowFile"];
                        StampTowFile.SaveAs(Server.MapPath($"~/Content/UserFiles/{int.Parse(model.UserID.ToString())}/المستندات/{GuidKey}[{int.Parse(model.UserID.ToString())}]StampTow.jpg"));

                        model.Stamp2 = $"{GuidKey}[{model.UserID}]StampTow.jpg";
                    }

                    if (model.ID == 0 || model.ID == null)
                    {
                        var user = dbSC.PhasesUsers.FirstOrDefault(x => x.UserID == model.UserID && x.PhaseID == model.PhaseID);
                        if (user != null)
                            return Json(notify = new notify() { Message = "المسؤول مضاف مسبقا لهذة المرحلة", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                        model.IsActive = true;
                        dbSC.PhasesUsers.Add(model);
                        dbSC.SaveChanges();

                        Session["SignatureFile"] = null;
                        Session["StampOneFile"] = null;
                        Session["StampTowFile"] = null;
                        return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var PhasesUsersID = dbSC.PhasesUsers.Find(model.ID);
                        PhasesUsersID.UserID = model.UserID;
                        PhasesUsersID.PhaseID = model.PhaseID;
                        PhasesUsersID.Signature = model.Signature != null ? model.Signature : PhasesUsersID.Signature;
                        PhasesUsersID.Stamp1 = model.Stamp1 != null ? model.Stamp1 : PhasesUsersID.Stamp1;
                        PhasesUsersID.Stamp2 = model.Stamp2 != null ? model.Stamp2 : PhasesUsersID.Stamp2;
                        dbSC.SaveChanges();

                        Session["SignatureFile"] = null;
                        Session["StampOneFile"] = null;
                        Session["StampTowFile"] = null;
                        return Json(notify = new notify() { Message = "تم التعديل بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        //List Phases Users
        public ActionResult GetPhasesUsers()
        {
            var PhasesUsers = dbSC.PhasesUsers.Select(x => new
            {
                ID = x.ID,
                UserName = x.DashBoard_Users.Name,
                PhaseName = x.TravelOrderPhase.PhaseName,
                UserID = x.UserID,
                PhaseID = x.PhaseID,
                Signature = x.Signature,
                StampOne = x.Stamp1,
                StampTow = x.Stamp2,
                x.IsActive
            }).ToList();
            return Json(PhasesUsers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActivePhasesUsers(int id)
        {
            if (ModelState.IsValid)
            {
                var PhasesUserId = dbSC.PhasesUsers.Find(id);
                if (PhasesUserId != null)
                {
                    if (PhasesUserId.IsActive == true)
                    {
                        PhasesUserId.IsActive = false;

                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم الايقاف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    if (PhasesUserId.IsActive == false)
                    {
                        PhasesUserId.IsActive = true;

                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم التنشيط بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletePhasesUsers(int id)
        {
            try
            {
                var PhasesUserId = dbSC.PhasesUsers.Find(id);
                if (PhasesUserId != null)
                {
                    dbSC.PhasesUsers.Remove(PhasesUserId);
                    dbSC.SaveChanges();
                    return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(notify = new notify() { Message = "لايمكن حذف المستخدم . مرتبط بعمليات اخري", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
            }

            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
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