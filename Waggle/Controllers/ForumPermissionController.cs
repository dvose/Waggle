using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using Waggle.Filters;
using Waggle.Models;
using WebMatrix.WebData;

namespace Waggle.Controllers
{
    [InitializeSimpleMembershipAttribute]
    public class ForumPermissionController : Controller
    {
        [HttpPost]
        [Authorize]
        public ActionResult AddPermission(String userEmail, int forumId)
        {
            if (User.IsInRole("admin")) {
                int userId = -1;
                using (UserEntitiesContext db = new UserEntitiesContext()) {
                    foreach (User user in db.Users) {
                        if (user.Email == userEmail) {
                            userId = user.Id;
                        }
                    }
                }
                if (userId != -1)
                {
                    using (Forum_PermissionContext db = new Forum_PermissionContext())
                    {
                        foreach (Forum_Permission permission in db.Forum_Permissions)
                        {
                            if (permission.UserId == userId && permission.ForumId == forumId)
                            {
                                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                            }
                        }
                        Forum_Permission fp = new Forum_Permission();
                        fp.UserId = userId;
                        fp.ForumId = forumId;
                        db.Forum_Permissions.Add(fp);
                        db.SaveChanges();
                    }
                }
            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        [Authorize]
        public ActionResult RemovePermission(int userId, int forumId)
        {
            if (User.IsInRole("admin"))
            {
                using (Forum_PermissionContext db = new Forum_PermissionContext())
                {
                    foreach (Forum_Permission permission in db.Forum_Permissions)
                    {
                        if (permission.UserId == userId && permission.ForumId == forumId)
                        {
                            db.Forum_Permissions.Remove(permission);
                        }
                    }
                    db.SaveChanges();
                }
            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }


    }
}
