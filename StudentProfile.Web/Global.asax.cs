using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DevExtremeMvcApp1;
using System;
using DevExpress.XtraReports.Web.Extensions;
using System.Net;
using System.IO;
using System.Web.WebPages;
using System.Web.SessionState;
using System.Text;
using System.Globalization;
using System.Threading;
using StudentProfile.Components.Helpers;
using StudentProfile.Components.Security;

namespace StudentProfile
{
    public static class ControllerExtensions
    {
        public static JsonResult JsonMaxLength(this Controller controller, object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }

    public class MvcApplication : HttpApplication
    {
        public int QueryId = 0;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }


        void Application_Error(object sender, EventArgs e)
        {
            HttpServerUtility server = HttpContext.Current.Server;
            Exception exception = server.GetLastError();
            if (exception != null)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);

                if (exception.InnerException != null)
                    System.Diagnostics.Debug.WriteLine(exception.InnerException.Message);
            }
        }

        protected void Application_Start()
        {
            //EncryptDecrypt.ProtectConfig();
            DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Disabled;
            DevExpress.XtraReports.Web.ReportDesigner.Native.ReportDesignerBootstrapper.SessionState = SessionStateBehavior.Required;
            DevExpress.XtraReports.Web.QueryBuilder.Native.QueryBuilderBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Required;
            //Disabledts.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState =
            //    System.Web.SessionState.SessionStateBehavior.Disabled;
            //Disabledts.Web.QueryBuilder.Native.QueryBuilderBootstrapper.SessionState =
            //    System.Web.SessionState.SessionStateBehavior.Disabled;
            //Disabledts.Web.ReportDesigner.Native.ReportDesignerBootstrapper.SessionState =
            //    System.Web.SessionState.SessionStateBehavior.Disabled;
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);


            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles);

            // Uncomment to use pre-17.2 routing for .Mvc() and .WebApi() data sources
            // DevExtreme.AspNet.Mvc.Compatibility.DataSource.UseLegacyRouting = true;
            // Uncomment to use pre-17.2 behavior for the "required" validation check
            // DevExtreme.AspNet.Mvc.Compatibility.Validation.IgnoreRequiredForBoolean = false;

            DevExpress.Web.Mvc.MVCxReportDesigner.StaticInitialize();
            DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(
                new ReportStorageWebExtension());
            DevExpress.Utils.UrlAccessSecurityLevelSetting.SecurityLevel =
                DevExpress.Utils.UrlAccessSecurityLevel.FilesFromBaseDirectory;
            //ValueProviderFactories.Factories.Insert(1, new CryptoValueProviderFactory());
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            string lcReqPath = Request.Path.ToLower();

            if (lcReqPath != "/login/login" && lcReqPath != "/login/Esol" && !lcReqPath.StartsWith("/api") && lcReqPath != "/security/notauthorized"&& lcReqPath != "/errorhandler" &&
                !lcReqPath.StartsWith("/help") && lcReqPath != "/"&& lcReqPath != "/aspxuploadprogresshandlerpage.ashx")
            {
                try
                {
                    // Session is not stable in AcquireRequestState - Use Current.Session instead.
                    System.Web.SessionState.HttpSessionState curSession = HttpContext.Current.Session;

                    // If we do not have a OK Logon (remember Session["UserId"] = null; on logout, and set to true on logon.)
                    //  and we are not already on loginpage, redirect.

                    // note: on missing pages curSession is null, Test this without 'curSession == null || ' and catch exception.

                    if (curSession == null || curSession["UserId"] == null || curSession["UserId"].ToString() == "0")
                    {
                        // Redirect nicely
                        //Context.Server.ClearError();
                        //Context.Response.AddHeader("Location", "/Login/Login");
                        //Context.Response.TrySkipIisCustomErrors = true;
                        //Context.Response.StatusCode = (int)System.Net.HttpStatusCode.Redirect;
                        //// End now end the current request so we dont leak.
                        //Context.Response.Output.Close();
                        //QueryId = 0;
                        ////Context.Response.End();
                        return;
                    }
                    else if (curSession["UserId"] != null)
                    {
                        //if (Context.Request.Headers["Referer"] == null)
                        //{
                        //    Context.Server.ClearError();
                        //    Context.Response.AddHeader("Location", "/Security/NotAuthorized");
                        //    Context.Response.TrySkipIisCustomErrors = true;
                        //    Context.Response.StatusCode = (int)System.Net.HttpStatusCode.Redirect;
                        //    // End now end the current request so we dont leak.
                        //    Context.Response.Output.Close();
                        //    QueryId = 0;
                        //}

                        //QueryId = int.Parse(curSession["UserId"].ToString());
                    }
                }

                catch (Exception ex)
                {
                    // todo: handle exceptions nicely!
                }
            }
            //if (HttpContext.Current.Session["UserId"] != null)
            //{
            //    QueryId = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            //}
            //else
            //{
            //    Session["UserId"] = 0;

            //    QueryId = 0;
            //    ActionExecutingContext filterContext = new ActionExecutingContext();
            //    filterContext.Result =
            //      new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Login" }));
            //    base.OnActionExecuting(filterContext)
            //}
        }

        //X-Frame-Options Header Not Set
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // HttpContext.Current.Response.AddHeader("x-frame-options", "DENY");
        }

        void Session_Start(Object sender, EventArgs e)
        {
            Session.Timeout = 180;
            //var context = new HttpContextWrapper(Context);
            //HttpContext.Current.Session["HeaderCompanyID"] = 0;
            //if (QueryId > 0)
            //{
            //    HttpContext.Current.Session["UserId"] = QueryId.ToString();


            //    //int UserIddd = int.Parse(QueryId.ToString());


            //    EsolErpEntities ctx = new EsolErpEntities();
            //    HttpContext.Current.Session["HeaderCompanyID"] = ctx.CON_USR.Where(x => x.ComID == QueryId).Select(x => x.ComID).FirstOrDefault();
            //}
        }

        protected void Session_End(Object sender, EventArgs e)
        {
            //Session.Clear();
            //Session.RemoveAll();
            //Session.Abandon();
        }

        void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);
            //
            HttpContext contextSession = HttpContext.Current;
            string currentpath = HttpContext.Current.Request.Path.ToLower();
            //if (!currentpath.StartsWith("/api"))
            //    if (!currentpath.EndsWith(".axd"))
            //    {
            //        bool x = context.Request.IsAjaxRequest();
            //        if (QueryId == 0 && context.Request.IsAjaxRequest())
            //        {
            //            Context.Response.Clear();
            //            Context.Response.StatusCode = 401;
            //        }


            //    }



        }


    
    }
}