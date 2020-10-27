using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public partial class HousingController : Controller
    {
        private EsolERPEntities dbACC = new EsolERPEntities();



        [HttpPost]
        public string addviolation(int userid, List<int> categoryids)
        {
            if (ModelState.IsValid) {
                //getting permissions for user from DB
                var categoriesPermissionsfromtable = dbSC.IssuessCategoriesPermissions.
                 Where(x => x.UserId == userid).ToList();

                //checking if both incoming and stored permissions are empty then we indicate invalid process

                if (categoriesPermissionsfromtable.Count == 0 && 
                    (categoryids == null || categoryids.Count == 0))
                {
                    return "0"; //indicate invalid process
                }

                //checking if stored permissions aren't empty and incoming are then we remove all existing permissions

                if (categoriesPermissionsfromtable.Count > 0 && 
                    (categoryids == null || categoryids.Count == 0))
                {
                    foreach (var permission in categoriesPermissionsfromtable)
                    {
                        dbSC.IssuessCategoriesPermissions.Remove(permission);
                        dbSC.SaveChanges();
                    }
                    return "1"; //indicates all permissions are deleted
                }

                //checking if stored permissions are empty and incoming aren't then we add all incoming permissions

                if (categoriesPermissionsfromtable.Count == 0 && categoryids.Count > 0)
                {
                    var user = ((DashBoard_Users)HttpContext.Session["userid"]);
                    IssuessCategoriesPermissions permission;
                    foreach (var category in categoryids)
                    {
                        permission = new IssuessCategoriesPermissions
                        {
                            UserId = userid,
                            CategoryId = category,
                            InsertedBy = user.ID,
                            InsertionDate = DateTime.Now
                        };
                        dbSC.IssuessCategoriesPermissions.Add(permission);
                        dbSC.SaveChanges();
                    }
                    return "2"; //indicates all permissions was added
                }

                //checking if both incoming and stored permissions aren't empty then we remove existing and add incoming permissions

                if (categoriesPermissionsfromtable.Count > 0 && categoryids.Count > 0)
                {
                    foreach (var PermissionInTable in categoriesPermissionsfromtable)
                    {
                        dbSC.IssuessCategoriesPermissions.Remove(PermissionInTable);
                    }

                    var user = ((DashBoard_Users)HttpContext.Session["userid"]);
                    IssuessCategoriesPermissions permissionToAdd;
                    foreach (var category in categoryids)
                    {
                        permissionToAdd = new IssuessCategoriesPermissions
                        {
                            UserId = userid,
                            CategoryId = category,
                            InsertedBy = user.ID,
                            InsertionDate = DateTime.Now
                        };
                        dbSC.IssuessCategoriesPermissions.Add(permissionToAdd);
                        dbSC.SaveChanges();
                    }
                    return "3"; //indicates edited permissions added or removed 
                }
            }
            return ""; // indicates error

        }



        //this function used to get root locations in AssetLocationsWithLevelsSelect_Vw
        [HttpGet]
        public ActionResult GetHousingLocations()
        {
            var locations = dbACC.AssetLocationsWithLevelsSelect_Vw.Where(x => x.ParentID == null).
                Select(x => new
                {
                    LocationId = x.ID, //.ToString(),
                    LocationName = x.LocationName
                }).ToList();
            return Json(locations, JsonRequestBehavior.AllowGet);
        }


        //get rooms' id and name to fill list of rooms
        [HttpGet]
        public ActionResult GetHousingRooms(int floorId)
        {
            var rooms = dbACC.AssetLocationsWithLevelsSelect_Vw.Where(x => x.ParentID == floorId).
                Select(x => new
                {
                    id = x.ID, //.ToString(),
                    text = x.LocationName
                }).ToList();
            return Json(rooms, JsonRequestBehavior.AllowGet);
        }


        // used to get buildings or floors to fill buildings and floors dropdown
        [HttpGet]
        public ActionResult GetHousingBuildings(int parentId) {


            var buildings = dbACC.AssetLocationsWithLevelsSelect_Vw.
                Where(x => x.ParentID == parentId).
                Select(x => new
                {
                    BuildingId = x.ID,
                    BuildingName = x.LocationName
                }).ToList();

            return Json(buildings, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult GetAllUsersCategoriesPermissions() {
            var usersCategories = dbSC.IssuessCategoriesPermissions.
                Select(x => new {
                    UserId = x.UserId,
                    Username = x.DashBoard_Users.Name,
                    CategoryId = x.CategoryId,
                    CategoryName = x.IssuesCategories.Name
                }).ToList();
            return Json(usersCategories, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetSupervisors()
        {
            /*this query gets all supervisors with thier rooms, floors, buildings and places names*/

            var supervisors = (from housingSupervisorsRooms in dbSC.HousingSupervisorsRooms
                               join housingsupervisors in dbSC.HousingSupervisors on housingSupervisorsRooms.SupervisorId equals housingsupervisors.UserId
                               select new housingSupervisorsRoomVm
                               {
                                   UserId = housingsupervisors.UserId,
                                   Username = housingsupervisors.DashBoard_Users1.Name,
                                   FileNumber = housingsupervisors.FileNumber,
                                   TelephoneNumber = housingsupervisors.TelphoneNumber,
                                   IsPresident = housingsupervisors.IsPresident,
                                   Notes = housingsupervisors.Notes,
                                   LocationId = housingSupervisorsRooms.LocationId
                               }).ToList().Select(r => new housingSupervisorsRoomVm
                               {
                                   UserId = r.UserId,
                                   Username = r.Username,
                                   FileNumber = r.FileNumber,
                                   TelephoneNumber = r.TelephoneNumber,
                                   IsPresident = r.IsPresident,
                                   Notes = r.Notes,
                                   LocationId = r.LocationId,
                                   Rooms = dbACC.AssetLocationsWithLevelsSelect_Vw.
                                   Where(l => l.ID == r.LocationId).Select(x => new RoomWithIdVm
                                   {
                                       RoomId = r.LocationId,

                                       RoomName = x.LocationName,

                                       HousingFloorName = dbACC.AssetLocationsWithLevelsSelect_Vw.
                                       FirstOrDefault(f => f.ID == x.ParentID).LocationName,

                                       HousingBuildingName = dbACC.AssetLocationsWithLevelsSelect_Vw.
                                       FirstOrDefault(b => b.ID == dbACC.AssetLocationsWithLevelsSelect_Vw.
                                       FirstOrDefault(f => f.ID == x.ParentID).ParentID).LocationName,

                                       HousingPlaceName = dbACC.AssetLocationsWithLevelsSelect_Vw.
                                       FirstOrDefault(p => p.ID == (dbACC.AssetLocationsWithLevelsSelect_Vw.
                                       FirstOrDefault(b => b.ID == dbACC.AssetLocationsWithLevelsSelect_Vw.
                                       FirstOrDefault(f => f.ID == x.ParentID).ParentID)).ParentID).LocationName
                                   }).ToList()
                               }).ToList();


            List<housingSupervisorsRoomVm> listToReturn = new List<housingSupervisorsRoomVm>();

            //foreach to fill the listToReturn with supervisors and each supervisor has a list of rooms details

            foreach (var supervisor in supervisors)
            {
                if (!listToReturn.Exists(x => x.UserId == supervisor.UserId))
                {
                    listToReturn.Add(supervisor);
                }
                else
                {
                    var old = listToReturn.FirstOrDefault(x => x.UserId == supervisor.UserId);
                    supervisor.Rooms.AddRange(old.Rooms);
                    listToReturn.Remove(old);
                    listToReturn.Add(supervisor);
                }

            }

            return Json(listToReturn, JsonRequestBehavior.AllowGet);
        }



        class housingSupervisorsRoomVm
        {
            public int UserId { get; set; }
            public string Username { get; set; }

            public string FileNumber { get; set; }
            public string TelephoneNumber { get; set; }
            public bool IsPresident { get; set; }
            public string Notes { get; set; }
            public int LocationId { get; set; }
            public List<RoomWithIdVm> Rooms { get; set; }
    }


        class RoomWithIdVm
        {
           public int RoomId { get; set; }
           public string RoomName { get; set; }
           public string HousingPlaceName { get; set; }
           public string HousingBuildingName { get; set; }
           public string HousingFloorName { get; set; }
        }


        [HttpPost]
        public string SaveSupervisor(int UserId, string FileNumber, string
               TelephoneNumber, bool IsPresident, string Notes, int[] LocationIds)
        {
            if (ModelState.IsValid)
            {

                if (dbSC.HousingSupervisors.Any(s => s.UserId == UserId))
                {
                    return "عفوا المستخدم مسجل من قبل";
                }

                int numIscorrect;

                if (!int.TryParse(FileNumber, out numIscorrect))
                {
                    return "الرقم الوظيفي غير صحيح";
                }

                if (!int.TryParse(TelephoneNumber, out numIscorrect))
                {
                    return "ادخل رقم هاتف مكون من ارقام فقط";
                }

                var user = ((DashBoard_Users)HttpContext.Session["UserId"]);
                HousingSupervisors supervisor = new HousingSupervisors
                {
                    UserId = UserId,
                    FileNumber = FileNumber,
                    TelphoneNumber = TelephoneNumber,
                    IsPresident = IsPresident,
                    Notes = Notes,
                    InsertedDate = DateTime.Now,
                    InsertedBy = user.ID
                };



                dbSC.HousingSupervisors.Add(supervisor);
                dbSC.SaveChanges();

                HousingSupervisorsRooms housingSupervisorsRooms;
                foreach (var id in LocationIds)
                {
                    housingSupervisorsRooms = new HousingSupervisorsRooms
                    {
                        SupervisorId = UserId,
                        LocationId = id,
                        BackUpDate = DateTime.Now
                    };

                    dbSC.HousingSupervisorsRooms.Add(housingSupervisorsRooms);
                    dbSC.SaveChanges();
                }

                return "";
            }
            return "عفوا حدث خطأ داخلي";
        }



        //get issues categories to fill categories list
        [HttpGet]
        public ActionResult GetCategories()
        {
            var categories = dbSC.IssuesCategories.Select(x => new
            {
                id = x.Id.ToString(),
                text = x.Name
            }).ToList();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Violation()
        {
            var permissions = GetPermissionsFn(62);
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


        [HttpGet]
        public ActionResult GetCategoriesPermissionsForUser(int userId)
        {
            var categoriesPermissionsForUser = dbSC.IssuessCategoriesPermissions.
                Where(x => x.UserId == userId).
                Select(x => new
                    {
                        UserId = x.UserId,
                        CategoryId = x.CategoryId,
                        Username = x.DashBoard_Users.Name,
                        CategoryName = x.IssuesCategories.Name
                    }).ToList();

            return Json(categoriesPermissionsForUser, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public string DeleteCategoriesPermissionForUser(int userId, int categoryId)
        {
            if (ModelState.IsValid)
            {
                var permission = dbSC.IssuessCategoriesPermissions.
                    FirstOrDefault(x => x.UserId == userId && x.CategoryId == categoryId);
                dbSC.IssuessCategoriesPermissions.Remove(permission);
                dbSC.SaveChanges();
                return "تم حذف الصلاحيه";
            }
            return "";
        }
    }
}
