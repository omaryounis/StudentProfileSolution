using Dashboard_StudentProfile.Cls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Dashboard_StudentProfile.Helpers
{

    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }
    public class Permissions
    {
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Save { get; set; }
    }
    public class CustomAuthorizeHelper: ActionFilterAttribute
    {

        public int ScreenId { get; set; }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var CurrentUser= System.Web.HttpContext.Current.Session["UserId"] as StudentProfile.DAL.Models.DashBoard_Users; // filterContext.HttpContext.Session["UserId"] as StudentProfile.DAL.Models.DashBoard_Users;

            var session = System.Web.HttpContext.Current.Session;
            HttpSessionStateBase sessionBase = new HttpSessionStateWrapper(session);
            if (CurrentUser!=null && CurrentUser.ID>0)
            {


                // Check if he Change Password 

                if ((CurrentUser.LastChangeDatetime <= CurrentUser.CreatedDate))
                {
                    filterContext.Result = new RedirectResult("/Login/ChangePassword");

                }



                //User Authenticated but we need check for  authorization using UserId From Session And ScreenId PassedBy Screen Developer.

                if (ScreenId != 0)
                {
                    var HasAccessToRead = GetPermissions(ScreenId,CurrentUser.ID);


                    if (!HasAccessToRead.Read)
                    {
                        filterContext.Result = new RedirectResult("/Security/NotAuthorized");
                    }
                }

        


                base.OnActionExecuting(filterContext);
            }
            else
            {
                //User Not Authenticated yet.. 
                filterContext.Result = new RedirectResult("/Login/Login2"); //new RedirectResult("Not Authorized url");
            }
               
        }

        public Permissions GetPermissions(int screenId, int userId)
        {

            var perm = CheckPermissions.IsAuthorized(userId, screenId);

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


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {


        private void ValidateRequestHeader(HttpRequestBase request)
        {
            string cookieToken = String.Empty;
            string formToken = String.Empty;
            string tokenValue = request.Headers["RequestVerificationToken"];
            if (!String.IsNullOrEmpty(tokenValue))
            {
                string[] tokens = tokenValue.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            AntiForgery.Validate(cookieToken, formToken);
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {

            try
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    ValidateRequestHeader(filterContext.HttpContext.Request);
                }
                else
                {
                    AntiForgery.Validate();
                }
            }
            catch (HttpAntiForgeryException e)
            {
                throw new HttpAntiForgeryException("Requst Doesnt Have Correct Token");
            }
        }
    }

    public enum Theme { Red, White, BlackGlass, Clean }

    [Serializable]
    public class InvalidKeyException : ApplicationException
    {
        public InvalidKeyException() { }
        public InvalidKeyException(string message) : base(message) { }
        public InvalidKeyException(string message, Exception inner) : base(message, inner) { }
    }
    public class ReCaptchaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userIP = filterContext.RequestContext.HttpContext.Request.UserHostAddress;

            var privateKey = ConfigurationManager.AppSettings["ReCaptcha.PrivateKey"];

            if (string.IsNullOrWhiteSpace(privateKey))
                throw new InvalidKeyException("ReCaptcha.PrivateKey missing from appSettings");

            var postData = string.Format("&secret={0}&remoteip={1}&response={2}",
                                         privateKey,
                                         userIP,
                                         filterContext.RequestContext.HttpContext.Request.Form["g-recaptcha-response"]
                                        );

            var postDataAsBytes = Encoding.UTF8.GetBytes(postData);

            // Create web request
            var request = WebRequest.Create("https://www.google.com/recaptcha/api/siteverify");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataAsBytes.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(postDataAsBytes, 0, postDataAsBytes.Length);
            dataStream.Close();

            // Get the response.
            var response = request.GetResponse();

            using (dataStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(dataStream))
                {
                    var responseFromServer = reader.ReadToEnd();
                    var res = Newtonsoft.Json.JsonConvert.DeserializeObject<CaptchaResponse>(responseFromServer);
                    if (!res.success)
                        ((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Captcha words typed incorrectly");



                }
            }
        }
    }
    public class CaptchaResponse
    {
        public bool success { get; set; }
    }

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString GenerateCaptcha(this HtmlHelper helper, Theme theme, string callBack = null)
        {
            const string htmlInjectString = @"<div id=""recaptcha_div""></div>
<script type=""text/javascript"">
    Recaptcha.create(""{0}"", ""recaptcha_div"", {{ theme: ""{1}"" {2}}});
</script>";

            var publicKey = ConfigurationManager.AppSettings["ReCaptcha.PublicKey"];

            if (string.IsNullOrWhiteSpace(publicKey))
                throw new InvalidKeyException("ReCaptcha.PublicKey missing from appSettings");

            if (!string.IsNullOrWhiteSpace(callBack))
                callBack = string.Concat(", callback: ", callBack);

            var html = string.Format(htmlInjectString, publicKey, theme.ToString().ToLower(), callBack);
            return MvcHtmlString.Create(html);
        }
    }










}