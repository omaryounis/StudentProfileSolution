using StudentProfile.Components;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class PayRollSettingController : Controller
    {
        SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();
        // GET: PayRollSetting
        public ActionResult Index()
        {
            var permissions = GetPermissionsFn(91);
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
        public JsonResult GetAllPayRollPhases()
        {

            var result = db.PayrollPhases.Select(x => new { x.ID, x.PhaseName, x.PhaseOrder, x.IsActive,x.IsFinancialApprove ,x.IssuingExchangeOrder,x.IssuingPaymentOrder}).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult addNewPayRollPhase(PayrollPhases payrollPhase)
        {
            var IsFinancialApproveCheck = db.PayrollPhases.ToList().Where(x => x.IsFinancialApprove == true).SingleOrDefault();
            if (IsFinancialApproveCheck != null && payrollPhase.IsFinancialApprove==true && payrollPhase.ID != IsFinancialApproveCheck.ID)
                return Content("يوجد اعتماد مالي مسجل من قبل");
            if (payrollPhase != null)
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(payrollPhase.PhaseName))
                        return Content("اسم المرحله مطلوب");

                    if (payrollPhase.ID == 0)
                    {
                        if (db.PayrollPhases.Any(x => x.PhaseName == payrollPhase.PhaseName))
                            return Content("هذا الاسم مسجل من قبل");

                        payrollPhase.IsActive = true;
                        db.PayrollPhases.Add(payrollPhase);
                        db.SaveChanges();
                    }
                    else
                    {
                        var checkPhaseOrde = db.PayrollPhases.FirstOrDefault(x => x.PhaseOrder == payrollPhase.PhaseOrder);
                        if(checkPhaseOrde == null)
                            return Content("الترتيب غير موجود");

                        int oldOrder = 0;
                        var row = db.PayrollPhases.Find(payrollPhase.ID);
                        oldOrder = row.PhaseOrder;
                        row.PhaseName = payrollPhase.PhaseName;
                        row.PhaseOrder = payrollPhase.PhaseOrder;
                        row.IsFinancialApprove = payrollPhase.IsFinancialApprove;
                        row.IssuingExchangeOrder = payrollPhase.IssuingExchangeOrder;
                        row.IssuingPaymentOrder = payrollPhase.IssuingPaymentOrder;
                        var changePhaseOrde = db.PayrollPhases.FirstOrDefault(x => x.PhaseOrder == payrollPhase.PhaseOrder);
                        changePhaseOrde.PhaseOrder = oldOrder;

                        db.SaveChanges();
                    }

                }
            }

            return Content("");
        }
        [HttpGet]
        public JsonResult GetAllUsers()
        {
            var result = db.DashBoard_Users.Select(x => new { x.ID, x.Name }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        

        [HttpGet]
        public JsonResult GetPayRollPhasesUsers()
        {
            var result = db.PayrollPhasesUsers.Select(x => new { x.ID, x.PayrollPhases.PhaseName, x.PayrollPhases.PhaseOrder, x.IsActive, x.DashBoard_Users.Name }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPayrollPhasesId(int id)
        {
            var result = db.PayrollPhases.Select(x => new { x.ID, x.PhaseName, x.PhaseOrder, x.IsActive,x.IsFinancialApprove ,x.IssuingExchangeOrder,x.IssuingPaymentOrder}).FirstOrDefault(x => x.ID == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public int? GetmaxOrder()
        {
            int? maxOrder = db.PayrollPhases.Count() > 0 ? db.PayrollPhases.Max(x => x.PhaseOrder) : 0;
            return maxOrder + 1;
        }

        [HttpPost]
        public ActionResult addPayRollPhasesUsers(PayrollPhasesUsers payrollPhasesUsers)
        {


            if (payrollPhasesUsers.PayrollPhaseID == 0)
                return Content("يرجى اختيار مرحله");
            if (payrollPhasesUsers.UserID == 0)
                return Content("يرجى اختيار مسؤول");

            if (db.PayrollPhasesUsers.Any(x => x.PayrollPhaseID == payrollPhasesUsers.PayrollPhaseID && x.UserID == payrollPhasesUsers.UserID))
                return Content(" هذا الاسم مسجل من قبل على هذه المرحله");
            else
            {
                db.PayrollPhasesUsers.Add(payrollPhasesUsers);
                db.SaveChanges();

            }
            return Content("");
        }
        [HttpPost]
        public ActionResult IsActiveEditing(int PayrollPhasesUsersId, bool status)
        {
            var PayrollPhasesUsersInDB = db.PayrollPhasesUsers.Where(x => x.ID == PayrollPhasesUsersId).FirstOrDefault();
            if (PayrollPhasesUsersInDB != null)
            {
                if (PayrollPhasesUsersInDB.IsActive == status)
                {
                    return Content("");
                }
                PayrollPhasesUsersInDB.IsActive = status;
                db.Entry(PayrollPhasesUsersInDB).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Content("");
            }
            return Content("لا يمكن تعديل حالة المسؤول نظرا لإرتباطه بسجلات أخري");

        }
        [HttpPost]
        public ActionResult UpdateRow(PayrollPhasesUsers payrollPhasesUsers)
        {

            var query =
                    from payroll in db.PayrollPhasesUsers
                    where payroll.ID == payrollPhasesUsers.ID
                    select payroll;

            if (db.PayrollPhasesUsers.Any(x => x.PayrollPhaseID == payrollPhasesUsers.PayrollPhaseID && x.UserID == payrollPhasesUsers.UserID))
                return Content(" هذا الاسم مسجل من قبل على هذه المرحله");

            else
            {
                foreach (PayrollPhasesUsers payroll in query)
                {
                    payroll.PayrollPhaseID = payrollPhasesUsers.PayrollPhaseID;
                    payroll.UserID = payrollPhasesUsers.UserID;
                }
                db.SaveChanges();
            }


            return Content("");
        }
        [HttpPost]
        public JsonResult GetPhaseName(PayrollPhasesUsers payrollPhasesUsers)
        {
            var result = db.PayrollPhasesUsers.Where(x => x.ID == payrollPhasesUsers.ID).SingleOrDefault().PayrollPhaseID;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetUserName(PayrollPhasesUsers payrollPhasesUsers)
        {
            var result = db.PayrollPhasesUsers.Where(x => x.ID == payrollPhasesUsers.ID).SingleOrDefault().UserID;
            return Json(result, JsonRequestBehavior.AllowGet);
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