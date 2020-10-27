using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Transactions;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using DevExpress.Office.Utils;
using StudentProfile.Components;

namespace StudentProfile.Web.Controllers
{

    public partial class HousingController : Controller
    {
        private notify notify = new notify();
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();


        #region HousingFurniture
        // GET: Housing Furniture
        public ActionResult HousingFurniture()
        {
            var permissions = GetPermissionsFn(59);
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

        public JsonResult GetHousingFurnitureCategories()
        {
            var data = dbSC.HousingFurnitureCategories
                                        .Select(x => new SelectListItem
                                        {
                                            Text = x.Name,
                                            Value = x.Id.ToString()
                                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveHousingFurnitures(HousingFurnitures model)
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
                            model.UserId = user.ID;
                            model.InsertDate = DateTime.Now;

                            dbSC.HousingFurnitures.Add(model);
                            dbSC.SaveChanges();
                            return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var entry = dbSC.HousingFurnitures.Find(model.Id);
                            entry.Name = model.Name;
                            entry.Notes = model.Notes;
                            entry.CategoryId = model.CategoryId;
                            entry.LastEdittingDate = DateTime.Now;
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
            //}
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHousingFurnitures()
        {
            var data = dbSC.HousingFurnitures.Select(x => new
            {
                x.Id,
                x.Name,
                x.Notes,
                x.CategoryId,
                FurnitureCategories = x.HousingFurnitureCategories.Name
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHousingFurnituresById(int ID)
        {
            if (ID > 0)
            {
                var data = dbSC.HousingFurnitures.Find(ID);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteHousingFurnitures(int ID)
        {
            if (ID > 0)
            {
                var row = dbSC.HousingFurnitures.Find(ID);
                dbSC.HousingFurnitures.Remove(row);
                try
                {
                    dbSC.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(notify = new notify() { Message = "لايمكن حذف الاثاث", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
                return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحذف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult GetUsers()
        {
            var users = dbSC.DashBoard_Users.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            }).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }


        #region HousingFurnitureCategories
        //HousingFurnitureCategories
        public ActionResult SaveHousingFurnitureCategories(HousingFurnitureCategories model)
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
                        model.UserId = user.ID;
                        model.InsertDate = DateTime.Now;

                        dbSC.HousingFurnitureCategories.Add(model);
                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var entry = dbSC.HousingFurnitureCategories.Find(model.Id);
                        entry.Name = model.Name;
                        entry.Notes = model.Notes;
                        entry.Prefix = model.Prefix;
                        entry.LastEdittingDate = DateTime.Now;
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

        public ActionResult GetAllHousingFurnitureCategories()
        {
            var data = dbSC.HousingFurnitureCategories.Select(x => new
            {
                x.Id,
                x.Name,
                x.Notes,
                x.Prefix,
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHousingFurnitureCategoriesById(int ID)
        {
            if (ID > 0)
            {
                var data = dbSC.HousingFurnitureCategories.Find(ID);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteHousingFurnitureCategories(int ID)
        {
            if (ID > 0)
            {
                var row = dbSC.HousingFurnitureCategories.Find(ID);
                dbSC.HousingFurnitureCategories.Remove(row);
                try
                {
                    dbSC.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(notify = new notify() { Message = "لايمكن حذف التصنيف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
                return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحذف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        #endregion



        #region ClearanceOfSudents

        public ActionResult CheckStudentHousingFurnitures(int id)
        {
            var checkHousingFurnituresStudentFound = dbSC.HousingFurnituresOfStudent.Where(x => x.HousingOfSudents.StudentId == id).FirstOrDefault();
            if (checkHousingFurnituresStudentFound == null)
                return Json(notify = new notify() { Message = "عفوا لايوجد عهد للطالب", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

            var checkHousingFurnitures = dbSC.HousingFurnituresOfStudent.Where(x => x.HousingOfSudents.StudentId == id && x.ReturnDate == null).FirstOrDefault();
            if (checkHousingFurnitures != null)
                return Json(notify = new notify() { Message = "يوجد عهد لم يقم الطالب بتسليمها", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

            //var checkClearanceOfSudents = dbSC.ClearanceOfSudents.Where(x => x.StudentId == id).FirstOrDefault();
            var checkClearanceOfSudents = dbSC.HousingOfSudents.FirstOrDefault(x => x.StudentId == id && x.LeaveDate !=null);
            if (checkClearanceOfSudents != null)
                return Json(notify = new notify() { Message = "تم اخلاء طرف الطالب مسبقا", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

            return Json(notify = new notify() { Message = "", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SaveClearanceOfSudents(ClearanceOfSudents model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model != null)
                    {
                        var user = Session["UserId"] as DashBoard_Users;

                        if (model.ID == 0 || model.ID == null)
                        {
                            model.UserId = user.ID;
                            model.Date = DateTime.Now;
                            dbSC.ClearanceOfSudents.Add(model);

                            // update housing of student data
                            var std=dbSC.HousingOfSudents.FirstOrDefault(a => a.StudentId == model.StudentId);
                            std.LeaveDate = model.Date;
                            std.SupervisorIdWhoMakeLeave = user.ID;
                            std.NotesOfLeave = model.Note;

                            dbSC.SaveChanges();
                            return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
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


        #endregion 



        public ActionResult SupervisorsDefinition()
        {
            var permissions = GetPermissionsFn(61);
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

        public ActionResult AddViolation()
        {
            Session["AddViolationFiles"] = null;
            var permissions = GetPermissionsFn(63);
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


        public ActionResult GetStudentForHousingViolations()
        {
            var students = dbSC.SP_GetAllStudent_Names_Customize()
                .Select(x => new 
                {
                    Value = x.STUDENT_ID.ToString(),
                    Text = x.STUDENT_NAME,
                    NationalID = x.NATIONAL_ID
                }).ToList();

            return this.JsonMaxLength(students);
        }


        public JsonResult GetIssuesCategories()
        {
            var user = Session["UserId"] as DashBoard_Users;
            dbSC.Configuration.LazyLoadingEnabled = false;
            var issuesCategories = dbSC.IssuesCategories.Where(x => x.IssuessCategoriesPermissions.Any(r => r.UserId == user.ID))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            return Json(issuesCategories, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIssuesByCategoryId(int categoryId)
        {
            dbSC.Configuration.LazyLoadingEnabled = false;


            var issues = dbSC.Issues.Where(x => x.CategoryId == categoryId)
                .Select(x => new SelectListItem
                {
                    Text = x.IssueDescription,
                    Value = x.Id.ToString()
                }).ToList();


            return Json(issues, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetIssueNumber(int issueId)
        {
            return Json(GetIssueNumberString(issueId), JsonRequestBehavior.AllowGet);
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