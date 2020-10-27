
using StudentProfile.DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DevExpress.Data.ODataLinq.Helpers;
using System.Text;
using System.Security.Cryptography;
using StudentProfile.Components;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class SecurityController : Controller
    {
        #region Security

        private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();


          public class Permissions
    {
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Save { get; set; }
    }


        // GET: Security/Users
        public ActionResult Users()
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            //المستخدمين
           
            var permissions = GetPermissionsFn(13);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read)
            {
                return View();
            }

            return RedirectToAction("NotAuthorized");
        }

        [HttpPost]
        public JsonResult GetPermissions(int screenId)
        {

            var user = HttpContext.Session["UserId"] as DashBoard_Users;
           // var userId = 0;
            if (user!=null)
            {
             //   userId = int.Parse(HttpContext.Session["UserId"].ToString());

                if (user.ID == 0)
                {
                    //return RedirectToAction("Login", "Login");
                    return Json(0, JsonRequestBehavior.AllowGet);
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

            return Json(permissions, JsonRequestBehavior.AllowGet);
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
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId =13)]
     
        public ActionResult GetUsers()
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}
            //if (Request.Headers["Referer"] == null)
            //{
            //    return RedirectToAction("Users", "Security");
            //}
            var users = db.DashBoard_Users
                .Select(x => new
                {
                    x.ID,
                    x.Name,
                    x.Username,
                    x.Mobile,
                    x.IsAdmin,
                    //Password = null as object,
                    GroupName = x.DashBoard_UserGroups.Select(groups => groups.DashBoard_Groups.Name).FirstOrDefault(),
                    Group_ID = x.DashBoard_UserGroups.Select(groups => groups.DashBoard_Groups.ID).FirstOrDefault()
                    //GroupName=x.DashBoard_UserGroups.name
                });
            //RSACryptoServiceProvider rsaEncrypt = new RSACryptoServiceProvider();
            //var json = JsonConvert.SerializeObject(users);
            //ClsCrypto crypto = new ClsCrypto("Crypto");
            //string EncryptedString = crypto.Encrypt(json);

            return Json(users.ToList(),JsonRequestBehavior.AllowGet);
        }

        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 13)]
        public ActionResult GetNonAdminUsers()
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("Users", "Security");
            }
            var users = db.DashBoard_UserGroups.Where(x => x.DashBoard_Groups.IsAdminGroup == false).Select(x => new
            {
                x.DashBoard_Users.ID,
                x.DashBoard_Users.Username,
                x.DashBoard_Users.Password,
                x.IsActive,
                x.DashBoard_Users.Mobile,
                x.DashBoard_Users.IsAdmin
            });
            return Json(users, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [CustomValidateAntiForgeryToken()]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 13)]
        public JsonResult SaveUser(string name, string accName, string mobile, bool isAdmin, string password,
            int groupId)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            try
            {
                var username = accName.Trim();


                var isAccNameExist = db.DashBoard_Users.Any(x => x.Username == username);
                if (isAccNameExist)
                {
                    return Json("عفوا اسم المستخدم مستخدم من قبل", JsonRequestBehavior.AllowGet);
                }

                var user = new DashBoard_Users
                {
                    Name = name,
                    Username = username,
                    IsAdmin = isAdmin,
                    Mobile = mobile,
                    Password = Passwordhelper.HashText(password),
                    PasswordHash = Passwordhelper.HashText(password),
                    unencryptedUsername = username

                };

                db.DashBoard_Users.Add(user);
                db.SaveChanges();
                var userId = user.ID;
                var userGroup = new DashBoard_UserGroups
                {
                    User_ID = userId,
                    Group_ID = groupId
                };
                db.DashBoard_UserGroups.Add(userGroup);
                db.SaveChanges();

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("حدث خطأ أثناء الحفظ، حاول تصحيح الاخطاء والمحاولة مرة أخرى", JsonRequestBehavior.AllowGet);
            }
        }





        [HttpPost]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 13)]
        public ActionResult GetUserDetails(int id)
        {
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("Users", "Security");
            }
            return Json(db.DashBoard_Users.Find(id));
        }


        [HttpPost]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 13)]
        public ActionResult DeleteUser(int id)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            try
            {
                var user = db.DashBoard_Users.SingleOrDefault(x => x.ID == id);
                if (user == null)
                {
                    return Content("عفوا لا يوجد مستخدم بهذه البيانات");
                }

                if (user.Dashboard_SecretPermissions.Any())
                {
                    //حذف الصلاحيات السرية ان وجدت
                    db.Dashboard_SecretPermissions.RemoveRange(user.Dashboard_SecretPermissions);
                }

                if (user.DashBoard_UserGroups.Any())
                {
                    //حذف المجموعة للمستخدم ان وجدت
                    db.DashBoard_UserGroups.RemoveRange(user.DashBoard_UserGroups);
                }

                //اخيراً حذف المستخدم من جدول المستخدمين
                db.DashBoard_Users.Remove(user);
                db.SaveChanges();
                return Content("");
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException == null) return Content("");
                if (sqlException.Errors.Count <= 0) return Content("");
                switch (sqlException.Errors[0].Number)
                {
                    case 547: // Foreign Key violation
                        return Content("حدث خطأ أثناء الحذف");
                    default:
                        return Content("حدث خطأ أثناء الحذف");
                }
            }
        }


        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 13)]

        public virtual ActionResult IsValidUserName(string username)
        {
            if (string.IsNullOrEmpty(username)) return Json("", JsonRequestBehavior.AllowGet);
            var hasAnySpecialChars = username.Any(ch => !char.IsLetterOrDigit(ch));
            if (hasAnySpecialChars)
            {
                return Json("اسم المستخدم يجب الا يحتوى على رموز خاصة، فقط ارقام وحروف",
                    JsonRequestBehavior.AllowGet);
            }

            //var isAccNameExist = db.DashBoard_Users.Any(x => x.Username == username);
            //if (isAccNameExist)
            //{
            //    return Json("عفوا اسم المستخدم مستخدم من قبل", JsonRequestBehavior.AllowGet);
            //}

            return Json("", JsonRequestBehavior.AllowGet);
        }

      [CustomValidateAntiForgeryToken()]
        [StudentProfile.Components.CustomAuthorizeHelper(ScreenId = 13)]

        public ActionResult EditUser(int? id, string name, string accName, string mobile, bool isAdmin, string password,
            int groupId)
        {
            var username = accName.Trim();

            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            var user = db.DashBoard_Users.Find(id);
            if (user == null)
            {
                return Json("عفوا لا يوجد مستخدم بهذه البيانات", JsonRequestBehavior.AllowGet);
            }

            //بحيث لو غير اسم المستخدم لاسم جديد 
            if (user.Username != username)
            {
                var isAccNameExist = db.DashBoard_Users.Any(x => x.Username == username);
                if (isAccNameExist)
                {
                    return Json("عفوا اسم المستخدم مستخدم من قبل", JsonRequestBehavior.AllowGet);
                }
            }


            user.Name = name;
            user.Username = username;
            user.Mobile = mobile;
            user.IsAdmin = isAdmin;
            user.Password = Passwordhelper.HashText(password);
            user.PasswordHash = Passwordhelper.HashText(password);
            user.unencryptedUsername = username;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            var userGroup = db.DashBoard_UserGroups.FirstOrDefault(x => x.User_ID == id);
            if (userGroup != null)
            {
                userGroup.Group_ID = groupId;
                db.Entry(userGroup).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        // GET: Security/Groups
        //[CustomAuthorizeHelper(ScreenId = 14)]
        public ActionResult Groups()
        {
            var permissions = GetPermissionsFn(14);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read)
            {
                return View();
            }

            return RedirectToAction("NotAuthorized");
        }



        //[CustomAuthorizeHelper(ScreenId = 14)]
        public ActionResult GetGroups()
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("Groups", "Security");
            }
            var users = db.DashBoard_Groups.Select(x => new
            {
                x.Name,
                x.IsActive,
                x.ID,
                x.IsAdminGroup,
            });

            return Json(users, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetScreens()
        {
            var users = db.Screens.Where(x=>x.ParentId==null && x.IsActive==true).Select(x => new
            {
               Value= x.ScreenId.ToString(),
               Text= x.ScreenName
            });

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[CustomAuthorizeHelper(ScreenId = 14)]
        public ActionResult DeleteGroup(int id)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            try
            {
                var group = db.DashBoard_Groups.Find(id);
                if (group == null) return Content("عفوا لا يوجد مجموعة بهذه البيانات");
                db.DashBoard_Groups.Remove(group);
                db.SaveChanges();
                return Content("");
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException == null) return Content("");
                if (sqlException.Errors.Count <= 0) return Content("");
                switch (sqlException.Errors[0].Number)
                {
                    case 547: // Foreign Key violation
                        return Content("لايمكن حذف هذه المجموعة لاحتوائها على مستخدمين");
                    default:
                        return Content("");
                }
            }
        }

        [HttpPost]
        //[CustomAuthorizeHelper(ScreenId = 14)]
        public ActionResult EditGroup(int? id, string groupName, bool isAdminGroup, bool isActive)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            var group = db.DashBoard_Groups.Find(id);
            if (group == null)
            {
                return Content("عفوا لا يوجد مجموعة بهذه البيانات");
            }

            group.IsActive = isActive;
            group.IsAdminGroup = isAdminGroup;
            group.Name = groupName;

            db.Entry(group).State = EntityState.Modified;
            db.SaveChanges();
            return Content("");
        }
        //[CustomAuthorizeHelper(ScreenId = 14)]
        public ActionResult IsGroupExist(string groupName)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}
            var isGroupExist = db.DashBoard_Groups.Any(x => x.Name == groupName.Trim());
            return Json(isGroupExist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[CustomAuthorizeHelper(ScreenId = 14)]
        public string SaveGroup(string groupName, bool isAdminGroup, bool isActive)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            var isGroupExist = db.DashBoard_Groups.Any(x => x.Name == groupName);
            if (isGroupExist)
            {
                return "عفوا اسم المجموعة مستخدم من قبل";
            }

            var group = new DashBoard_Groups
            {
                Name = groupName,
                IsActive = isActive,
                IsAdminGroup = isAdminGroup,
            };

            db.DashBoard_Groups.Add(group);
            db.SaveChanges();
            return "";
        }


        public ActionResult GetUserPermissions(int id)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("Users", "Security");
            }
            //var userPermissions = db.Con_Company.Where(c => c.CON_ID == id).Select(x => x.ComID);
            var userPermissions = "";
            return Json(userPermissions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserGroup()
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            //شاشات المستخدمين
          
            var permissions = GetPermissionsFn(15);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;


            if (permissions.Read)
            {
                return View();
            }

            return RedirectToAction("NotAuthorized");
        }

        [HttpPost]
        //[CustomAuthorizeHelper(ScreenId = 15)]
        public ActionResult GetGroupUsers(int groupId)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("UserGroup", "Security");
            }
            var groupUsers = db.DashBoard_UserGroups.Where(x => x.Group_ID == groupId).Select(x => new
            {
                Name = x.DashBoard_Users.Username
            });
            return Json(groupUsers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[CustomAuthorizeHelper(ScreenId = 15)]

        public ActionResult GetUsersNotInSelectedGroup(int groupId)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("UserGroup", "Security");
            }
            var usersNotInSelectedGroup = (from p in db.DashBoard_Users
                                           where p.DashBoard_UserGroups.Any(x => x.Group_ID != groupId)
                                           select new { p.ID, p.Username }).ToList();
            return Json(usersNotInSelectedGroup, JsonRequestBehavior.AllowGet);
        }
        //[CustomAuthorizeHelper(ScreenId = 15)]

        public ActionResult SaveUserGroup(int groupId, int userId)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            //every user has only one group and every group has many users
            //first remvove user group
            var oldGroups = db.DashBoard_UserGroups.Where(x => x.User_ID == userId);
            foreach (var group in oldGroups)
            {
                db.DashBoard_UserGroups.Remove(group);
            }

            db.SaveChanges();

            //second assign new group
            var newGroup = new DashBoard_UserGroups
            {
                IsActive = true,
                User_ID = userId,
                Group_ID = groupId
            };
            db.DashBoard_UserGroups.Add(newGroup);
            db.SaveChanges();

            return Content("");
        }
        //////[CustomAuthorizeHelper(ScreenId = 12)]

        public ActionResult ScreensPermissions()
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            //صلاحيات الشاشات
            
            var permissions = GetPermissionsFn(12);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;

            if (permissions.Read)
            {
                return View();
            }

            return RedirectToAction("NotAuthorized");
        }

        //public ActionResult GetScreenActions()
        //{
        //    //if (Session["UserId"] == null)
        //    //{
        //    //    RedirectToAction("Login", "Login");
        //    //}
        //    if (Request.Headers["Referer"] == null)
        //    {
        //        return RedirectToAction("ScreensPermissions", "Security");
        //    }
        //    var screenActions = db.ScreenActions.OrderBy(x => x.Screens.Order).Select(x => new
        //    {
        //        x.ActionName,
        //        id = x.ScreenActionId,
        //        x.ScreenId,
        //        x.Screens.ScreenName,
        //        x.Screens.Order
        //    }).ToList();
        //    return Json(screenActions, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetScreenActions(string screenIds)
        {
            string[] screens = screenIds.Split(',');
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("ScreensPermissions", "Security");
            }
            var screenActions = db.ScreenActions.Where(x => (screens.Any(p => p == x.Screens.ParentId.ToString()))).OrderBy(x => x.Screens.Order).Select(x => new
            {

                x.ActionName,
                id=/*(int?)null*/x.ScreenActionId,
                x.ScreenId,
                x.Screens.ScreenName,
                x.Screens.Order
            }).ToList();
            return Json(screenActions, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetActions(int ScreenId)
        {
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("ScreensPermissions", "Security");
            }
            var screenActions = db.ScreenActions.Where(x=>x.ScreenId==ScreenId).OrderBy(x => x.Screens.Order).Select(x => new
            {
                ScreenName = x.ActionName,
                ScreenActionId = x.ScreenActionId,
                ScreenId = x.ScreenId,
                ParentId = x.ScreenId
            }).Distinct().ToList();

            return Json(screenActions, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        ////[CustomAuthorizeHelper(ScreenId = 12)]
        public ActionResult SaveScreensPermissions(int groupId,string screenIds, int[] screenActionIds)
        {
            var screens = screenIds.Split(',');
            
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            //first remove old permission (ScreenActionId)
            var DBTrans = db.Database.BeginTransaction();
            try
            {

                var oldPermissions = db.ScreenSctionsGroup.Where(x => x.GroupId == groupId
                && (screens.Any(p => p == x.ScreenActions.Screens.ParentId.ToString()))
                );

                db.ScreenSctionsGroup.RemoveRange(oldPermissions);

                //foreach (var permission in oldPermissions)
                //{
                //    db.ScreenSctionsGroup.Remove(permission);
                //}

               // db.SaveChanges();

                //then insert new ones
                foreach (var actionId in screenActionIds)
                {
                    var item = new ScreenSctionsGroup
                    {
                        GroupId = groupId,
                        ScreenActionId = actionId
                    };
                    db.ScreenSctionsGroup.Add(item);
                    db.SaveChanges();
                }

                db.SaveChanges();
                DBTrans.Commit();
                return Content("");
            }
            catch (Exception e)
            {
                DBTrans.Rollback();
                return Content("يوجد خطأ");
            }
        }

        ////[CustomAuthorizeHelper(ScreenId = 12)]

        //public ActionResult GetGroupScreenActionIds(int grpId)
        //{
        //    //if (Session["UserId"] == null)
        //    //{
        //    //    RedirectToAction("Login", "Login");
        //    //}
        //    if (Request.Headers["Referer"] == null)
        //    {
        //        return RedirectToAction("ScreensPermissions", "Security");
        //    }
        //    var screenActionIds = db.ScreenSctionsGroup.AsEnumerable().Where(x => x.GroupId == grpId)
        //        .Select(x => x.ScreenActionId).ToList();
        //    return Json(screenActionIds, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetGroupScreenActionIds(int grpId,string  screens)
        {
            var MainScreensArr = screens.Split(',');
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("ScreensPermissions", "Security");
            }


            var screenActionIds = (from s in db.ScreenActions.Where(x => (MainScreensArr.Any(p => p == x.Screens.ParentId.ToString())))
                                   join scg in db.ScreenSctionsGroup.Where(x => x.GroupId == grpId) on s.ScreenActionId
                                   equals scg.ScreenActionId into ps
                                   from scg in ps.DefaultIfEmpty()
                                   select new
                                   {
                                       id = s.ScreenActionId,
                                       ScreenName =s.Screens.ScreenName,
                                       ScreenId = s.ScreenId,
                                       Order = s.Screens.Order,
                                       ParentId = s.Screens.ParentId,
                                       ActionName = s.ActionName,
                                       IsExist =scg!=null ? true:false
                                   }).ToList();
            //.Select(x => new {
            //    id = x.GroupScreenId,
            //    ScreenActionId=x.ScreenActionId,
            //    ScreenName =x.ScreenActions.Screens!=null? x.ScreenActions.Screens.ScreenName:null,
            //    ScreenId = x.ScreenActions != null ? x.ScreenActions.Screens.ScreenId : 0,
            //    Order = x.ScreenActions != null ? x.ScreenActions.Screens.Order : 0,
            //    ParentId = x.ScreenActions != null ? x.ScreenActions.Screens.ParentId : 0,
            //    ActionName = x.ScreenActions != null ? x.ScreenActions.ActionName : null
            //}).ToList();
            return Json(screenActionIds, JsonRequestBehavior.AllowGet);
        }
        ////[CustomAuthorizeHelper(ScreenId = 12)]

        public string CheckUserGroupBeforeSave(int groupId, int userId)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            //التاكد اذا كان المستخدم مسند الى مجموعة ام لا
            var userGroupNameQ =
                (from p in db.DashBoard_UserGroups
                 where p.User_ID == userId
                 select new { p.DashBoard_Groups.Name, p.DashBoard_Users.Username, p.Group_ID, p.User_ID })
                .SingleOrDefault();


            if (userGroupNameQ == null)
            {
                //فى حالة ان المستخدم غير مسند الية من قبل اى مجموعة
                var groupName = db.DashBoard_Groups.Single(x => x.ID == groupId).Name;
                string userName = db.DashBoard_Users.Single(x => x.ID == userId).Username;
                return "المستخدم الجديد " + userName + " " + "لم يتم اسنادة مسبقا على اى مجموعة " + " " +
                       "وسوف يتم اسنادة الى مجموعة " + groupName;
            }

            //المستخدم مسند على نفس المجموعة
            if (groupId == userGroupNameQ.Group_ID)
            {
                return "عفوا نفس المستخدم مسجل من قبل على نفس المجموعة";
            }

            {
                //المستخدم سوف يتم تحويلة على مجموعة اخرى
                var groupName = db.DashBoard_Groups.Single(x => x.ID == groupId).Name;
                return "المستخدم " + " " + userGroupNameQ.Username + " " + "مسند من قبل الى مجموعة " + " " +
                       userGroupNameQ.Name + "و سوف يتم نقلة الى المجموعة " + " " + groupName;
            }
        }


        public ActionResult NotAuthorized()
        {
            return View();
        }

        #endregion


        #region الصلاحيات السرية فى سجل السلوك الخارجى
        ////[CustomAuthorizeHelper(ScreenId = 12)]

        public ActionResult SecretPermissions()
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}


            var Currentuser = HttpContext.Session["UserId"] as DashBoard_Users;
            //var userId = int.Parse(HttpContext.Session["UserId"].ToString());
            var user = db.DashBoard_Users.SingleOrDefault(x => x.ID == Currentuser.ID);

            var secretPermissons = db.Dashboard_SecretPermissions.SingleOrDefault(x => x.UserId == Currentuser.ID);
            ViewData["SecretPermisson"] = secretPermissons?.Secret;
            ViewData["TopSecretPermisson"] = secretPermissons?.TopSecret;

            var permissions = GetPermissionsFn(121);
            ViewBag.Create = permissions.Create;
            ViewBag.Read = permissions.Read;
            ViewBag.Update = permissions.Update;
            ViewBag.Delete = permissions.Delete;
            ViewBag.Save = permissions.Save;
            ViewBag.View = permissions.View;
            if (user?.IsAdmin == true && (permissions.View || permissions.Read))
            {
                return View();
            }
            

            return RedirectToAction("NotAuthorized");
        }

        [HttpGet]
        ////[CustomAuthorizeHelper(ScreenId = 12)]

        public ActionResult GetSecretUsers()
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}
            var Currentuser = HttpContext.Session["UserId"] as DashBoard_Users;
          //  var userId = int.Parse(HttpContext.Session["UserId"].ToString());
            //1. التاكد من ان المستخدم الحالى له صلاحية فى جدول الصلاحيات السرية
            var currentUserSecretPermissions = db.Dashboard_SecretPermissions.SingleOrDefault(x => x.UserId == Currentuser.ID);
            var users = new List<DashBoard_Users>();

            if (currentUserSecretPermissions != null)
            {
                // سرى فقط
                var secretPermissions =
                    db.Dashboard_SecretPermissions.Where(x =>
                        ((x.Secret == currentUserSecretPermissions.Secret &&
                          x.TopSecret == currentUserSecretPermissions.TopSecret) ||
                         (x.Secret == false && x.TopSecret == false)) &&
                        x.UserId != currentUserSecretPermissions.UserId).ToList()?.Select(x => x.UserId);

                var secretUsers =
                    db.DashBoard_Users.Where(x => secretPermissions.Any(u => u == x.ID)).ToList();

                //سرى جداً فقط
                //var topSecretPermissions =
                //    db.Dashboard_SecretPermissions.Where(x => x.Secret == false && x.TopSecret == true);

                //var topSecretUsers =
                //    db.DashBoard_Users.Where(x => x.Dashboard_SecretPermissions == topSecretPermissions).ToList();


                ////سرى & سرى جداً
                //var topSecretAndSecretPermissions =
                //    db.Dashboard_SecretPermissions.Where(x => x.Secret == true && x.TopSecret == true);

                //var topSecretAndSecretUsers =
                //    db.DashBoard_Users.Where(x => x.Dashboard_SecretPermissions == topSecretAndSecretPermissions)
                //        .ToList();

                //عادى
                var normalUsers = db.DashBoard_Users
                    .Where(user =>
                        !db.Dashboard_SecretPermissions.Any(secretPermission => secretPermission.UserId == user.ID))
                    .ToList();
                users.AddRange(normalUsers);
                users.AddRange(secretUsers);
                //if (currentUserSecretPermissions.TopSecret == true && currentUserSecretPermissions.Secret == true)
                //{
                //    //عادى + سرى + سرى جدا + سرى & سرى جدا
                //    users.AddRange(normalUsers);
                //    users.AddRange(secretUsers);
                //    users.AddRange(topSecretUsers);
                //    users.AddRange(topSecretAndSecretUsers);
                //}
                //else if (currentUserSecretPermissions.TopSecret == true && currentUserSecretPermissions.Secret == false)
                //{
                //    //عادى + سرى جداً
                //    users.AddRange(normalUsers);
                //    users.AddRange(topSecretUsers);
                //}
                //else if (currentUserSecretPermissions.TopSecret == false && currentUserSecretPermissions.Secret == true)
                //{
                //    //عادى + سرى
                //    users.AddRange(normalUsers);
                //    users.AddRange(secretUsers);
                //}

                var lst = users.Select(x => new { Value = x.ID, Text = x.Username });
                return Json(lst, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        [HttpPost]
        [CustomValidateAntiForgeryToken()]
        ////[CustomAuthorizeHelper(ScreenId = 12)]

        public ActionResult SaveSecretPermissions(int userId, bool isSecret, bool isTopSecret)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            try
            {
                var secretPermission = db.Dashboard_SecretPermissions.SingleOrDefault(x => x.UserId == userId);

                //المستخدم له صلاحيات من قبل، يتم تحديثها
                if (secretPermission != null)
                {
                    secretPermission.TopSecret = isTopSecret;
                    secretPermission.Secret = isSecret;
                    db.Entry(secretPermission).State = EntityState.Modified;
                }
                else
                {
                    //المستخدم ليس له صلاحيات من قبل، يتم اضافة صلاحيات له
                    db.Dashboard_SecretPermissions.Add(new Dashboard_SecretPermissions
                    {
                        UserId = userId,
                        Secret = isSecret,
                        TopSecret = isTopSecret
                    });
                }

                db.SaveChanges();
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json($"حدث خطأ أثناء الحفظ {e.Message}", JsonRequestBehavior.AllowGet);
            }
        }

        ////[CustomAuthorizeHelper(ScreenId = 12)]

        public ActionResult GetUserSecretPermissions(int userId)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}
            if (Request.Headers["Referer"] == null)
            {
                return RedirectToAction("ScreensPermissions", "Security");
            }
            var secretPermission = db.Dashboard_SecretPermissions.SingleOrDefault(x => x.UserId == userId);
            if (secretPermission != null)
            {
                return Json(new { isSecret = secretPermission.Secret, isTopSecret = secretPermission.TopSecret },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region المستخدمين الجدد
        ////[CustomAuthorizeHelper(ScreenId = 12)]
        [AllowAnonymous]
        public ActionResult NoPermissions(int? id)
        {
            //if (Session["UserId"] == null)
            //{
            //    RedirectToAction("Login", "Login");
            //}

            if (id != null && id > 0)
            {
                int? count = db.DashBoard_UserGroups.Where(x => x.User_ID == id).Count();
                if (count != null && count > 0)
                {
                    return RedirectToAction("Index", "Students");
                }
            }

            ViewBag.Message = "عفوا هذا المستخدم ليس لديه أي صلاحيات برجاء الرجوع لإدارة النظام";
            return View();
        }

        #endregion
    }
    public class ClsCrypto
    {
        private RijndaelManaged myRijndael = new RijndaelManaged();
        private int iterations;
        private byte[] salt;

        public ClsCrypto(string strPassword)
        {
            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 128;
            //myRijndael.IV = HexStringToByteArray("a5s8d2e9c1721ae0e84ad660c472y1f3");

            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            iterations = 1000;
            salt = System.Text.Encoding.UTF8.GetBytes("cryptography123example");
            myRijndael.Key = GenerateKey(strPassword);
        }

        public string Encrypt(string strPlainText)
        {
            byte[] strText = new System.Text.UTF8Encoding().GetBytes(strPlainText);
            ICryptoTransform transform = myRijndael.CreateEncryptor();
            byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);
            return Convert.ToBase64String(cipherText);
        }

        public string Decrypt(string encryptedText)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            ICryptoTransform transform = myRijndael.CreateDecryptor();
            byte[] cipherText = transform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return System.Text.Encoding.UTF8.GetString(cipherText);
        }

        public static byte[] HexStringToByteArray(string strHex)
        {
            //    dynamic r = new byte[strHex.Length / 2];
            //    for (int i = 0; i <= strHex.Length - 1; i += 2)
            //    {
            //        r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            //    }
            //    return r;
            return Enumerable.Range(0, strHex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(strHex.Substring(x, 2), 16))
                     .ToArray();
        }

        private byte[] GenerateKey(string strPassword)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), salt, iterations);
            return rfc2898.GetBytes(128 / 8);
        }
    }

}