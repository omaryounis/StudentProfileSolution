using StudentProfile.DAL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using StudentProfile.Components;
namespace StudentProfile.Controllers
{
    [RequireHttps]
    [StudentProfile.Components.CustomAuthorizeHelper]
    public partial class MusaferController : Controller
    {

        public Permissions CheckPermissionsfn(int screenId)
        {

            var CurrentUser = Session["UserId"] as DashBoard_Users;

            //int userId = int.Parse(Session["UserId"].ToString());


            var perm = CheckPermissions.IsAuthorized(CurrentUser.ID, screenId);

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
            }

            return permissions;
        }

    }
}

