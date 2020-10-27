using System;
using System.Web.Mvc;


namespace Dashboard_StudentProfile.Helpers
{
    public class DashboardStudentExceptionHandler : HandleErrorAttribute
    {
        private bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        public override void OnException(ExceptionContext filterContext)
        {
            var errorNumber = DateTime.Now.Date.ToLongDateString();
            if (IsAjax(filterContext))
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                string script = "window.location.href ='/ErrorHandler/Index'";
                filterContext.Result = new JavaScriptResult() { Script = script };
            }
            else { filterContext.Result = new RedirectResult("/ErrorHandler/Index"); }

            //base.OnException(filterContext);
        }
    }
}