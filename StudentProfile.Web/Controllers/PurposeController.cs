
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Controllers
{


    [RequireHttps]
    //[Dashboard_StudentProfile.Helpers.CustomAuthorizeHelper]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public class PurposeController : Controller
    {

        notify notify = new notify();
        private SchoolAccGam3aEntities dbSC = new SchoolAccGam3aEntities();
        // GET: Purpose
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetPurpose()
        {
            var purpose = dbSC.config.Where(x => x.Kay == "purpose").Select(x => new SelectListItem { Text = x.title, Value = x.ID.ToString() }).OrderBy(x => x.Text).ToList();
            return Json(purpose, JsonRequestBehavior.AllowGet);
        }

    }
}