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
    public class AcademicCommitteeUsersController : Controller
    {
        notify notify = new notify();
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();


        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(69);
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

        public ActionResult GetFaculties()
        {
            var Faculties = dbSC.usp_getFaculties().Select(x => new SelectListItem { Text = x.FACULTY_NAME, Value = x.FACULTY_NO.ToString() }).ToList();
            return Json(Faculties, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetDegrees(string FacultionIds)
        {
            if (FacultionIds == null)
            {
                return this.JsonMaxLength(dbSC.usp_getDegrees()
                                               .Select(selector: x => new SelectListItem
                                               {
                                                   Text = x.DEGREE_DESC,
                                                   Value = x.DEGREE_CODE.ToString()
                                               })
                                              .OrderBy(x => x.Text).ToList());
            }
            else
            {

                return this.JsonMaxLength(dbSC.usp_getDegreesByFACULTYID(FacultionIds)
                                               .Select(selector: x => new SelectListItem
                                               {
                                                   Text = x.DEGREE_DESC,
                                                   Value = x.DEGREE_CODE.ToString()
                                               })
                                              .OrderBy(x => x.Text).ToList());
            }
          
        }
        public ActionResult SaveAcademicCommitteeUsers(AcademicCommitteeUsersVm model)
        {
            if (ModelState.IsValid)
            {
                    if (model.ID == 0 || model.ID == null)
                    {

                     var faculities = string.IsNullOrEmpty(model.FacultyIds) ? new string[0] : model.FacultyIds.Split(',');
                     var degrees = string.IsNullOrEmpty(model.DegreeIds) ? new string[0] : model.DegreeIds.Split(',');
                    if (faculities.Count() == 0)
                    {
                        return Json(notify = new notify() { Message = "يجب إختيار الكليات", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                    }
                    if (degrees.Count() == 0)
                    {
                        return Json(notify = new notify() { Message ="يجب إختيار الدرجات العلمية", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                    } 
                    foreach (var faculty in faculities)
                     {
                        var user = dbSC.AcademicCommitteeUsers.AsEnumerable().FirstOrDefault(x => x.UserID == model.UserID && x.FacultyID == int.Parse(faculty));
                        //if (user != null)
                        //{
                        //    return Json(notify = new notify() { Message = $"المسؤول مضاف مسبقا لكلية {dbSC.usp_getFaculties().AsEnumerable().FirstOrDefault(x => x.FACULTY_NO == int.Parse(faculty)).FACULTY_NAME}", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                        //}
                        int FacultyNo = int.Parse(faculty);
                        var DEGREE_CODEs = dbSC.INTEGRATION_All_Students.Where(m => m.FACULTY_NO == FacultyNo).Select(p=>p.DEGREE_CODE).Distinct().ToList() ;
                        //List<int> DEGREE_CODEs= DEGREE_CODEsNullable.Select<decimal, int>(i => i).ToList(); ;
                        List<decimal> degreesList = degrees.Select(decimal.Parse).ToList();
                        List<decimal> DegreesWillBeAdded = DEGREE_CODEs.Intersect(degreesList).ToList();
                        for(int i=0;i< DegreesWillBeAdded.Count; i++)
                        {
                            int dgrid = Convert.ToInt32(DegreesWillBeAdded[i]);
                          
                            dbSC.AcademicCommitteeUsers.Add(new AcademicCommitteeUsers()
                            {
                                UserID = model.UserID,
                                FacultyID = FacultyNo,
                                DegreeID =Convert.ToInt32(DegreesWillBeAdded[i]),
                                IsActive = true
                            });
                            dbSC.SaveChanges();

                        }
                    }

                    return Json(notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);

                    //foreach (var item in faculities)
                    //{
                    //    var user = dbSC.AcademicCommitteeUsers.AsEnumerable().FirstOrDefault(x => x.UserID == model.UserID && x.FacultyID == int.Parse(item));
                    //    if (user != null)
                    //        return Json(notify = new notify() { Message = $"المسؤول مضاف مسبقا لكلية {dbSC.usp_getFaculties().AsEnumerable().FirstOrDefault(x=>x.FACULTY_NO == int.Parse(item)).FACULTY_NAME}", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                    //    dbSC.AcademicCommitteeUsers.Add(new AcademicCommitteeUsers()
                    //    {
                    //        UserID = model.UserID,
                    //        FacultyID = int.Parse(item),
                    //        IsActive = true,
                    //    });
                    //}
                }
            }

            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAcademicCommitteeUsers()
        {
            var list = dbSC.AcademicCommitteeUsers.ToList();
            var AcademicCommitteeUsers = list.Select(x => new
            {
                x.ID,
                x.UserID,
                UserName = x.DashBoard_Users!=null?x.DashBoard_Users.Name:"",
                x.FacultyID, 
                Faculty = dbSC.usp_getFaculties().FirstOrDefault(c=>c.FACULTY_NO == x.FacultyID)!=null? dbSC.usp_getFaculties().FirstOrDefault(c => c.FACULTY_NO == x.FacultyID).FACULTY_NAME:"",
                Degree=dbSC.usp_getDegrees().Where(c=>c.DEGREE_CODE ==x.DegreeID)!=null? dbSC.usp_getDegrees().Where(c => c.DEGREE_CODE == x.DegreeID).Select(m=>m.DEGREE_DESC):null,
                x.IsActive
            }).ToList();
            return Json(AcademicCommitteeUsers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActiveAcademicCommitteeUsers(int id)
        {
            if (ModelState.IsValid)
            {
                var AcademicUserId = dbSC.AcademicCommitteeUsers.Find(id);
                if (AcademicUserId != null)
                {
                    if (AcademicUserId.IsActive == true)
                    {
                        AcademicUserId.IsActive = false;

                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم الايقاف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    if (AcademicUserId.IsActive == false)
                    {
                        AcademicUserId.IsActive = true;

                        dbSC.SaveChanges();
                        return Json(notify = new notify() { Message = "تم التنشيط بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 200 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAcademicCommitteeUsers(int id)
        {
            
                var AcademicUserId = dbSC.AcademicCommitteeUsers.Find(id);
                if (AcademicUserId != null)
                {
                    
                    dbSC.AcademicCommitteeUsers.Remove(AcademicUserId);
                    dbSC.SaveChanges();
                    return Json(notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                }

              return Json(notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
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

    public partial class AcademicCommitteeUsersVm
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string FacultyIds { get; set; }
        public bool? IsActive { get; set; }
        public string DegreeIds { get; set; }
    }

}
