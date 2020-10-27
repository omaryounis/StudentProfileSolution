using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StudentProfile.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace StudentProfile.Web.Controllers
{
    public partial class BaseController : Controller
    {
        public BaseController()
        {

        }
        public Dictionary<string, string> permissionAttributes { get { return GetpermissionAttributes(); } }
        public int ScreenId { get; set; }

        SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();
        public StudentProfile.DAL.Models.DashBoard_Users CurrentUser
        {
            get
            {

                return System.Web.HttpContext.Current.Session["UserId"] as StudentProfile.DAL.Models.DashBoard_Users;

            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {


            if (CurrentUser != null && CurrentUser.ID > 0)
            {


                // Check if he Change Password 

                if ((CurrentUser.LastChangeDatetime <= CurrentUser.CreatedDate))
                {
                    filterContext.Result = new RedirectResult("/Login/ChangePassword");

                }



                //User Authenticated but we need check for  authorization using UserId From Session And ScreenId PassedBy Screen Developer.


                var request = Request;



                base.OnActionExecuting(filterContext);
            }
            else
            {
                //User Not Authenticated yet.. 
                filterContext.Result = new RedirectResult("/Login/Esol"); //new RedirectResult("Not Authorized url");
            }

        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            if (ScreenId != 0)
            {
                var authorizedPermissions = GetPermissions(ScreenId);


                if (!authorizedPermissions.Read)
                {
                    filterContext.Result = new RedirectResult("/Security/NotAuthorized");
                }
                base.OnActionExecuted(filterContext);
            }
            else
            {
                //User Not Authenticated yet.. 
                filterContext.Result = new RedirectResult("/Login/Esol"); //new RedirectResult("Not Authorized url");
            }
        }
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }


        protected AllPermissions GetPermissions(int screenId)
        {
            var perm = IsAuthorized(screenId, CurrentUser.ID);
            AllPermissions currentPermissions = new AllPermissions();
            ViewData.Clear();
            var list = permissionAttributes.Where(x => perm.Any(p => p == x.Key)).ToList();
            foreach (var item in list)
            {
                PropertyInfo propertyInfo = currentPermissions.GetType().GetProperty(item.Value);
                propertyInfo.SetValue(currentPermissions, Convert.ChangeType(true, propertyInfo.PropertyType));
                ViewData.Add(propertyInfo.Name, true);
            }
            return currentPermissions;

        }

        private List<string> IsAuthorized(int? _screenId, int? userId)
        {
            if (_screenId == null || _screenId == 0) _screenId = ScreenId;
            var groupPermission = new List<string>();
            if (userId < 0)
            {
                return groupPermission; //Empty list of permissions
            }
            var usergroup = db.DashBoard_UserGroups.SingleOrDefault(x => x.User_ID == userId);
            if (usergroup == null)
            {
                return groupPermission; //Empty list of permissions
            }
            var groupId = usergroup.Group_ID;
            groupPermission = db.ScreenSctionsGroup
                .Where(x => (x.GroupId == groupId & x.ScreenActions.ScreenId == ScreenId))
                .Select(x => x.ScreenActions.ActionName).ToList();
            return groupPermission;
        }
        private Dictionary<string, string> GetpermissionAttributes()
        {
            return new Dictionary<string, string>()
            {
                {"قراءة", "Read"},
                {"ارسال الكود", "SendCode"},
                {"استثناء", "Exception"},
                {"اضافة", "Create"},
                {"تعديل", "Update"},
                {"حذف", "Delete"},
                {"عرض", "View"},
                {"حفظ", "Save"},
                {"موافقة / رفض", "AcceptReject"},
                {"اضافة الرغبات", "AddRequest"},
                {"اضافة حقل مخصص رئيسي", "CreateParent"},
                {"اضافة حقل مخصص فرعي", "CreateChild"},
                {"تاكيد الكود", "ConfirmCode"},
                {"رفع الصورة", "UploadeImage"},
                {"تحديث", "Update"},
                {"تحميل المرفقات", "DownloadAttachments"},
                {"عرض صورة الطالب", "ShowStudentImage"},
                {"معاينة المرفقات", "PreviewAttachments"},
                {"قبول", "Accept"},
                {"مشاهدة", "Read"}
            };
        }



        //JsonSerializerSettings
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonDotNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

    }
    public class AllPermissions
    {
        public bool Read { get; set; }
        public bool SendCode { get; set; }
        public bool Exception { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Save { get; set; }
        public bool AcceptReject { get; set; }
        public bool AddRequest { get; set; }
        public bool CreateParent { get; set; }
        public bool CreateChild { get; set; }
        public bool ConfirmCode { get; set; }
        public bool UploadeImage { get; set; }
        public bool DownloadAttachments { get; set; }
        public bool ShowStudentImage { get; set; }
        public bool PreviewAttachments { get; set; }
        public bool Accept { get; set; }

    }


    public class JsonDotNetResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public override void ExecuteResult(ControllerContext context)
        {
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
            String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("GET request not allowed");
            }

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            MaxJsonLength= Int32.MaxValue;

            if (Data == null)
            {
                return;
            }

            response.Write(JsonConvert.SerializeObject(this.Data, Settings));
        }
    }
}