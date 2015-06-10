using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Waggle.Filters;
using Waggle.Models;
using Waggle.ViewModels;
using WebMatrix.WebData;

namespace Waggle.Controllers
{
    public class AdminController : Controller
    {
        //private ForumContext Fdb = new ForumContext();
        //
        // GET: /Admin/

        [Authorize]
        [InitializeSimpleMembershipAttribute]
        public ActionResult Index()
        {
            if (User.IsInRole("admin"))
            {
                return View();
            }
            else {
                return RedirectToAction("Http404", "Error");
            }
        }

        //
        // POST: /Admin/

        [HttpPost]
        [Authorize]
        public ActionResult Index (AdminPanel model)
        {
            if (User.IsInRole("admin"))
            {
                if (ModelState.IsValid)
                {
                    using (ForumContext Fdb = new ForumContext())
                    {
                        Forum newforum = new Forum();

                        newforum.UserId = WebSecurity.CurrentUserId;
                        newforum.Name = model.forum.Name;
                        newforum.Description = model.forum.Description;
                        Fdb.Forums.Add(newforum);
                        Fdb.SaveChanges();
                        return View("Index", "Forum", Fdb.Forums);
                    }
                }
                else
                    return View("Forbidden");
            }
            else
            {
                return RedirectToAction("Http404", "Error");
            } 
        }

        // GET: UserList

        [Authorize]
        public ActionResult UserList()
        {
               if (User.IsInRole("admin"))
                {
                 using (UserEntitiesContext db = new UserEntitiesContext())
                 {
                     var users = db.Users.ToList();
        
                        return View(users);
                 }
                   }
            else
            {
                return RedirectToAction("Http404", "Error");
            }
        }

        [Authorize]
        public ActionResult SuspendUser(int userId)
        {
            if (User.IsInRole("admin"))
            {
                using (UserEntitiesContext db = new UserEntitiesContext())
                {
                    var users = db.Users.ToList();

                    foreach (User u in db.Users)
                    {
                        if (u.Id == userId)
                        {
                            u.IsSuspended = true;
                        }
                    }
                    db.SaveChanges();
                    return View("UserList", users);
                }
            }
            else
            {
                return RedirectToAction("Http404", "Error");
            }
        }

        [Authorize]
        public ActionResult UnsuspendUser(int userId)
        {
            if (User.IsInRole("admin"))
            {
                using (UserEntitiesContext db = new UserEntitiesContext())
                {
                    var users = db.Users.ToList();

                    foreach (User u in db.Users)
                    {
                        if (u.Id == userId)
                        {
                            u.IsSuspended = false;
                        }
                    }
                    db.SaveChanges();
                    return View("UserList", users);
                }
            }
            else
            {
                return RedirectToAction("Http404", "Error");
            }
        }

        [Authorize]
        public ActionResult Promote (int userId)
        {
            if (User.IsInRole("admin"))
            {
                using (UserEntitiesContext db = new UserEntitiesContext())
                {
                    var users = db.Users.ToList();

                    foreach (User u in db.Users)
                    {
                        if (u.Id == userId)
                        {
                            Roles.RemoveUserFromRole(u.Email, "normal");
                            Roles.AddUserToRole(u.Email, "admin");
                        }
                    }
                    return View("UserList", users);
                }
            }
            else
            {
                return RedirectToAction("Http404", "Error");
            }
        }

        [Authorize]
        public ActionResult Demote(int userId)
        {
            if (User.IsInRole("admin"))
            {
                using (UserEntitiesContext db = new UserEntitiesContext())
                {
                    var users = db.Users.ToList();

                    foreach (User u in db.Users)
                    {
                        if (u.Id == userId)
                        {
                            Roles.RemoveUserFromRole(u.Email, "admin");
                            Roles.AddUserToRole(u.Email, "normal");
                        }
                    }
                    return View("UserList", users);
                }
            }
            else
            {
                return RedirectToAction("Http404", "Error");
            }
        }

        // GET: Forbidden

        [AllowAnonymous]
        public ActionResult Forbidden()
        {
            return View();
        }

    }
}
