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
    public partial class ExchangeOrderController : Controller
    {
        #region DataMembers

        private SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();
        private DashBoard_Users currentUser
        {
            get
            {
                if (HttpContext.Session["UserId"] != null)
                    return HttpContext.Session["UserId"] as DashBoard_Users;
                return null;
            }
        }
        #endregion
        #region Views

        public ActionResult DeliverToRep()
        {
            var permissions = GetPermissionsFn(89);
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
        public ActionResult StudentsCash()
        {
            var permissions = GetPermissionsFn(90);
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
        #endregion
        #region JsonActions
        public JsonResult GetChecks(int? deliveredTo)
        {
            return Json(db.Checks.Where(x => x.ExchangeOrdersChecks.Count() > 0 && (deliveredTo == null || deliveredTo == x.DeliveredTo)).Select(x => new { x.ID, x.CheckNumber }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUsers()
        {
            return Json(db.DashBoard_Users.Select(x => new { x.ID, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult RepUsersGrid()
        {
            return Json(db.DashBoard_Users.Where(x => x.DeliveredTo_Checks.Count() > 0).Select(x => new
            {
                ID = x.ID,
                UserName = x.Name,
                TotalAssignedValue = x.DeliveredTo_Checks.Sum(p => p.CheckValue)
                // AssignedChecks = x.DeliveredTo_Checks
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeliveredChecksByUserID(int userID)
        {
            return Json(db.Checks.Where(x => x.DeliveredTo == userID).ToList().Select(x => new
            {
                x.CheckNumber,
                x.CheckValue,
                DeliverDate = x.DeliverDate?.ToShortDateString(),
                AssignedFrom = x.DeliveredBy_User.Name,
                x.DeliveredTo
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCurrentChecksByUserID(int userID)
        {
            return Json(db.Checks.Where(x => x.DeliveredTo == userID).Select(x => x.ID).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCurrentUserChecks()
        {
            return Json(db.Checks.Where(x => x.DeliveredTo == currentUser.ID).Select(x =>new { x.ID ,x.CheckNumber}).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetExchangeOrdersByCheckID(int checkID)
        {
            return Json(db.ExchangeOrder.Where(x => x.ExchangeOrdersChecks.Any(p => p.CheckID == checkID))
                .Select(x => new { x.ID, x.OrderNumber }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPayrollsByExchangeOrders(List<int> exchangeOrderIds)
        {
            return Json(db.Payroll.Where(x => exchangeOrderIds.Any(p => p == x.ExchangeOrderID))
                .Select(x => new { x.ID, x.PayrollNumber }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStudentPayrollsByPayrollID(List<int> payrolls)
        {
            
            return Json(
                db.StudentPayroll.Where(x=>payrolls.Any(p => p == x.PayrollID)).GroupBy(x => new { x.Payroll, x.StudentID })
                .Select(x => new
                {
                    x.Key.StudentID,
                    StudentName = db.INTEGRATION_All_Students.Where(p => p.STUDENT_ID == x.Key.StudentID).FirstOrDefault().STUDENT_NAME,
                   PayrollNumber = x.Key.Payroll.IssueDate.Day + "-" + x.Key.Payroll.IssueDate.Month + "-" + x.Key.Payroll.IssueDate.Year + "-(" + x.Key.Payroll.PayrollNumber + ")",
                    Value = x.Sum(g => (g.Value * g.RewardItems.RewardItemTypes.RewardItemType)),
                    PayrollID= x.Key.Payroll.ID
                }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AssignCheckToUser(List<int> checkIds, int deliveredTo)
        {
            string result = "";
            try
            {
                db.Checks.Where(x => checkIds.Any(p => p == x.ID)).ToList().ForEach(x =>
                {
                    x.DeliveredTo = deliveredTo;
                    x.DeliverDate = DateTime.Now;
                    x.DeliveredBy = currentUser.ID;
                });

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = "حدث خطأ أثناء الحفظ";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PayRewardtoStudents(int checkID,List<KeyValuePair<int,int>> selectedStudents)
        {
            var notify = new notify();
            
            try
            {
                bool isAllstudents = true;
                var checkValue = db.Checks.Where(x => x.ID == checkID).SingleOrDefault().CheckValue;
                db.StudentPayroll.Where(x =>selectedStudents.Any(p => p.Key == x.PayrollID && p.Value == x.StudentID)).GroupBy(x=>x.StudentID).ToList().ForEach(x =>
                {
                    x.ToList().ForEach(g=>g.IsPaid=true);
                    var studentValue = (x.Sum(g => (g.Value * g.RewardItems.RewardItemTypes.RewardItemType)));
                    if (checkValue >= studentValue)
                        checkValue = studentValue > 0 ? checkValue - (x.Sum(g => (g.Value * g.RewardItems.RewardItemTypes.RewardItemType))) : checkValue;
                    else
                        isAllstudents = false;
                });
                
                db.SaveChanges();
                notify.Message = isAllstudents ? $" تم تسليم مكافات كل الطلاب المختارين بنجاح وتبقى قيمة {checkValue} من قيمة الشيك"
                    : "تم تسليم المكافات لبعض الطلاب المختارين حيث لم تكفي قيمة الشيك لكل الطلاب";
                notify.Type = isAllstudents ? "success" : "warning";
                notify.status = 200;
            }
            catch (Exception ex)
            {
                notify.Message= "حدث خطأ أثناء تسليم المكافأت للطلاب";
                notify.Type = "error";
                notify.status = 500;
            }
            return Json(notify, JsonRequestBehavior.AllowGet);
        }
        #endregion

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