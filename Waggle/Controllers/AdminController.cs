using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

                    
                    

                    //Forum.ForumList.Add(newforum);
                    

                    

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

        // GET: Forbidden

        [AllowAnonymous]
        public ActionResult Forbidden()
        {
            return View();
        }

    }
}
