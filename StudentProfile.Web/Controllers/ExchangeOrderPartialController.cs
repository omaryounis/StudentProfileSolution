using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    public partial class ExchangeOrderController : Controller
    {
        private notify _notify;
        private readonly SchoolAccGam3aEntities _dbSc = new SchoolAccGam3aEntities();
        // GET: ExchangeOrder
        public ActionResult CashCheck()
        {
            var permissions = GetPermissionsFn(88);
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

        public ActionResult GetExchangeOrder()
        {
            var exchangeOrders = _dbSc.ExchangeOrder.Where(x => x.IsActive == true && x.IsTotallyInChecks != true).Select(x => new SelectListItem { Text = x.OrderNumber, Value = x.ID.ToString() }).ToList();
            return this.JsonMaxLength(exchangeOrders);
        }

        public ActionResult GetExchangeOrderIsActive(int id)
        {
            var exchangeOrders = _dbSc.ExchangeOrder.Where(x => x.IsActive == true && x.IsTotallyInChecks != true || x.ID == id).Select(x => new SelectListItem { Text = x.OrderNumber, Value = x.ID.ToString() }).ToList();
            return this.JsonMaxLength(exchangeOrders);
        }

        public ActionResult GetExchangeOrderValueById(int id)
        {
            var outCheckValue = _dbSc.ExchangeOrdersChecks.Where(x => x.ExchangeOrderID == id).ToList().Sum(x => x.Value);

            var exchangeOrders = _dbSc.ExchangeOrder.Select(x => new { x.ID, TotalValue = (x.TotalValue) - outCheckValue }).SingleOrDefault(x => x.ID == id);
            if (exchangeOrders != null)
                return Json(exchangeOrders, JsonRequestBehavior.AllowGet);

            return Json("");

        }

        public ActionResult GetExchangeOrderValueByIdForEdit(int id ,int CurrentOrderId)
        {
            var outCheckValue = _dbSc.ExchangeOrdersChecks.Where(x => x.ExchangeOrderID == id && x.ID != CurrentOrderId).ToList().Sum(x => x.Value);

            var exchangeOrders = _dbSc.ExchangeOrder.Select(x => new { x.ID, TotalValue = (x.TotalValue) - outCheckValue }).SingleOrDefault(x => x.ID == id);
            if (exchangeOrders != null)
                return Json(exchangeOrders, JsonRequestBehavior.AllowGet);

            return Json("");

        }

        public ActionResult SaveSaveCheks(SaveChekVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (model != null)
                    {
                        var user = Session["UserId"] as DashBoard_Users;

                        var sumCheckValue = model.ExchangeOrdersChecks.ToList().Sum(x => x.Value);
                        if (sumCheckValue != model.Checks.CheckValue)
                            return Json(_notify = new notify() { Message = "قيمة اوامر الصرف لا تساوي قيمة الشيك", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                        if (model.Checks.ID == 0 || model.Checks.ID == null)
                        {
                            var checkFound = _dbSc.Checks.Select(x => x.CheckNumber).FirstOrDefault(x => x == model.Checks.CheckNumber);
                            if (checkFound != null)
                                return Json(_notify = new notify() { Message = "رقم الشيك مضاف مسبقا", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);

                            using (TransactionScope tran = new TransactionScope())
                            {
                                model.Checks.InsertedBy = user.ID;
                                model.Checks.InsertDate = DateTime.Now;
                                _dbSc.Checks.Add(model.Checks);
                                _dbSc.SaveChanges();


                                foreach (var item in model.ExchangeOrdersChecks)
                                {
                                    //ExchangeOrder IsTotallyInChecks
                                    var currentExchangeOrder = _dbSc.ExchangeOrder.Select(x => new { x.ID, x.TotalValue }).SingleOrDefault(x => x.ID == item.ExchangeOrderID);
                                    var prevValue = _dbSc.ExchangeOrdersChecks.Where(x => x.ExchangeOrderID == item.ExchangeOrderID).ToList().Sum(x => x.Value);
                                    var nextValue = model.ExchangeOrdersChecks.Where(x => x.ExchangeOrderID == item.ExchangeOrderID).ToList().Sum(x => x.Value);
                                    if ((nextValue + prevValue) == currentExchangeOrder.TotalValue)
                                        _dbSc.ExchangeOrder.Find(currentExchangeOrder.ID).IsTotallyInChecks = true;

                                    item.CheckID = model.Checks.ID;
                                    item.InsertDate = DateTime.Now;
                                    _dbSc.ExchangeOrdersChecks.Add(item);
                                }
                                _dbSc.SaveChanges();
                                tran.Complete();
                                return Json(_notify = new notify() { Message = "تم الحفظ بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var row = _dbSc.Checks.SingleOrDefault(x => x.ID == model.Checks.ID);
                            if (row != null)
                            {
                                using (TransactionScope tran = new TransactionScope())
                                {
                                    //remove Exchange Orders Checks Detalie
                                    _dbSc.ExchangeOrdersChecks.RemoveRange(_dbSc.ExchangeOrdersChecks.Where(x => x.CheckID == model.Checks.ID).ToList());
                                    db.SaveChanges();

                                    //Edit
                                    row.InsertedBy = user.ID;
                                    row.CheckNumber = model.Checks.CheckNumber;
                                    row.CheckValue = model.Checks.CheckValue;
                                    _dbSc.SaveChanges();

                                    foreach (var item in model.ExchangeOrdersChecks)
                                    {
                                        //ExchangeOrder IsTotallyInChecks
                                        var currentExchangeOrder = _dbSc.ExchangeOrder.Select(x => new { x.ID, x.TotalValue }).SingleOrDefault(x => x.ID == item.ExchangeOrderID);
                                        _dbSc.ExchangeOrder.Find(currentExchangeOrder.ID).IsTotallyInChecks = false;

                                        var prevValue = _dbSc.ExchangeOrdersChecks.Where(x => x.ExchangeOrderID == item.ExchangeOrderID).ToList().Sum(x => x.Value);
                                        var nextValue = model.ExchangeOrdersChecks.Where(x => x.ExchangeOrderID == item.ExchangeOrderID).ToList().Sum(x => x.Value);
                                        if ((nextValue + prevValue) == currentExchangeOrder.TotalValue)
                                            _dbSc.ExchangeOrder.Find(currentExchangeOrder.ID).IsTotallyInChecks = true;

                                        item.CheckID = model.Checks.ID;
                                        item.InsertDate = DateTime.Now;
                                        _dbSc.ExchangeOrdersChecks.Add(item);
                                    }
                                    _dbSc.SaveChanges();
                                    tran.Complete();
                                }
                            }
                            return Json(_notify = new notify() { Message = "تم التعديل بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                catch (Exception ex)
                {

                    return Json(_notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(_notify = new notify() { Message = "خطأ اثناء الحفظ", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCashChecks()
        {
            var Checks = _dbSc.Checks.Select(x => new
            {
                x.ID,
                x.CheckValue,
                x.CheckNumber,
                FullInsertDate = x.InsertDate.Month + "/" + x.InsertDate.Day + "/" + x.InsertDate.Year,
                InsertDate = x.InsertDate.Day + "/" + x.InsertDate.Month + "/" + x.InsertDate.Year,
                IssueDate = x.IssueDate.Day + "/" + x.IssueDate.Month + "/" + x.IssueDate.Year,
                InsertedBy = x.InsertedBy_Users.Name,
                DeliveredBy = x.DeliveredBy_User.Name,
                DeliverdTo = x.DeliverdTo_User.Name,
                DeliverDate = x.DeliverDate != null ? x.DeliverDate.Value.Day + "/" + x.DeliverDate.Value.Month + "/" + x.DeliverDate.Value.Year : null
            }).ToList();
            return this.JsonMaxLength(Checks);
        }

        public ActionResult GetExchangeOrdersChecksByCheckId(int id)
        {
            var ExchangeOrdersChecks = _dbSc.ExchangeOrdersChecks.Where(x => x.CheckID == id).Select(x => new
            {
                x.ID,
                x.CheckID,
                OrderNumber = x.ExchangeOrder.OrderNumber,
                InsertDate = x.InsertDate != null ? x.InsertDate.Value.Day + "/" + x.InsertDate.Value.Month + "/" + x.InsertDate.Value.Year : null,
                x.Value,
                x.ExchangeOrderID,
            }).ToList();
            return this.JsonMaxLength(ExchangeOrdersChecks);
        }

        public ActionResult DeleteCheck(int id)
        {
            if (id > 0)
            {
                var row = _dbSc.Checks.Include(x=>x.ExchangeOrdersChecks).SingleOrDefault(x => x.ID == id);
                if (row != null)
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        foreach (var item in _dbSc.ExchangeOrdersChecks.Where(x => x.CheckID == id).ToList())
                            item.ExchangeOrder.IsTotallyInChecks = false;

                        _dbSc.ExchangeOrdersChecks.RemoveRange(_dbSc.ExchangeOrdersChecks.Where(x => x.CheckID == id).ToList());
                  

                        db.SaveChanges();
                        _dbSc.Checks.Remove(row);
                        try
                        {
                            _dbSc.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(_notify = new notify() { Message = "لايمكن حذف الشيك لارتباطة بعمليات اخري", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
                        }
                        tran.Complete();
                    }
                }
                return Json(_notify = new notify() { Message = "تم الحذف بنجاح", Type = "success", status = 200 }, JsonRequestBehavior.AllowGet);
            }
            return Json(_notify = new notify() { Message = "خطأ اثناء الحذف", Type = "error", status = 500 }, JsonRequestBehavior.AllowGet);
        }


    }


    public class SaveChekVM
    {
        public Checks Checks { get; set; }
        public IList<ExchangeOrdersChecks> ExchangeOrdersChecks { get; set; }
    }
}