using StudentProfile.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace StudentProfile.Components
{
    public class CheckPermissions
    {
        private static SchoolAccGam3aEntities db = new SchoolAccGam3aEntities();

        //private static int _userId = int.Parse(Session["UserId"].ToString());
        public static List<string> IsAuthorized(int? userId, int screenId)
        {
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
                .Where(x => (x.GroupId == groupId & x.ScreenActions.ScreenId == screenId))
                .Select(x => x.ScreenActions.ActionName).ToList();
            return groupPermission;
        }
        public static List<int> IsAuthorizedTab(int? userId)
        {
            var groupPermission = new List<int>();
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
                .Where(x => x.ScreenActions.ActionName == "قراءة" && (x.GroupId == groupId && ((x.ScreenActions.ScreenId >= 1 && x.ScreenActions.ScreenId < 12) || x.ScreenActions.ScreenId == 18 || x.ScreenActions.ScreenId == 26)))
                .Select(x => x.ScreenActions.ScreenId).ToList();
            return groupPermission;
        }
    }
}