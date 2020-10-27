using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class IssuesController : Controller
    {
        // GET: Issues
        notify notify = new notify();
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();

        // GET: Issues
        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(65);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.AddCategory = permissions.AddCategory;

            if (permissions.Read)
            {
                return View();
            }

            return RedirectToAction("NoPermissions", "Security");
        }

        public ActionResult GetIssuesCategories()
        {
            var CurrentUser = Session["UserId"] as DashBoard_Users;
            var Issues = dbSC.IssuessCategoriesPermissions.AsEnumerable().Where(p => p.UserId == CurrentUser.ID)
                .Select(x => new SelectListItem { Text = x.IssuesCategories.Name, Value = x.IssuesCategories.Id.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(Issues, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveIssues(Issues model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model != null)
                    {
                        if (model.Id == 0 || model.Id == null)
                        {
                            dbSC.Issues.Add(model);
                            dbSC.SaveChanges();
                            return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var entry = dbSC.Issues.Find(model.Id);
                            entry.CategoryId = model.CategoryId;
                            entry.IssueDescription = model.IssueDescription;
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

        public ActionResult GetIssues()
        {
            var data = dbSC.Issues.Select(x => new
            {
                x.Id,
                x.CategoryId,
                CategoryName = x.IssuesCategories.Name,
                Name = x.IssueDescription
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssuesById(int Id)
        {
            if (Id > 0)
            {
                var data = dbSC.Issues.Find(Id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public ActionResult DeleteIssues(int Id)
        {
            if (Id > 0)
            {
                var row = dbSC.Issues.Find(Id);
                dbSC.Issues.Remove(row);
                try
                {
                    dbSC.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(notify = new notify() { Message = "لايمكن حذف القسم لارتباطة بعمليات اخري", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
                return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحذف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        public IssuesPermissions GetPermissionsFn(int screenId)
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

            var permissions = new IssuesPermissions();
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
              else if (permission == "اضافة تصنيف")
                {
                    permissions.AddCategory = true;
                }
            }

            return permissions;
        }

        /*************************** IssuesCategories ************************************/
        #region IssuesCategories
        //IssuesCategories
        public ActionResult SaveIssuesCategories(IssuesCategories model)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                if (model != null)
                {
                    var user = Session["UserId"] as DashBoard_Users;

                    if (model.Id == 0 || model.Id == null)
                    {
                        model.InsertedBy = user.ID;
                        model.InsertedDate = DateTime.Now;
                        // Add Issuess Categories Permissions
                        model.IssuessCategoriesPermissions.Add(new IssuessCategoriesPermissions
                        {
                            CategoryId = model.Id,
                            UserId = user.ID,
                            InsertionDate = DateTime.Now,
                            InsertedBy = user.ID
                        });

                        dbSC.IssuesCategories.Add(model);
                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var entry = dbSC.IssuesCategories.Find(model.Id);
                        entry.Name = model.Name;
                        entry.LastEditDate = DateTime.Now;
                        entry.LastEdittedBy = user.ID;
                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم التعديل بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
            }
            //}
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllIssuesCategories()
        {
            var data = dbSC.IssuesCategories.Select(x => new
            {
                x.Id,
                x.Name
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssuesCategoriesById(int Id)
        {
            if (Id > 0)
            {
                var data = dbSC.IssuesCategories.Find(Id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        [HttpDelete]
        public ActionResult DeleteIssuesCategories(int Id)
        {
            if (Id > 0)
            {
                var row = dbSC.IssuesCategories.Find(Id);
                dbSC.IssuesCategories.Remove(row);
                try
                {
                    dbSC.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(notify = new notify() { Message = "لايمكن حذف التصنيف لارتباطة بعمليات اخري", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
                return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحذف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }

    public class IssuesPermissions
    {
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Save { get; set; }
        public bool AddCategory { get; set; }

    }
}