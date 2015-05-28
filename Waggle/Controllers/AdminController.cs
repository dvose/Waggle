using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Waggle.Models;
using Waggle.ViewModels;
using WebMatrix.WebData;

namespace Waggle.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        [Authorize]
        public ActionResult Index()
        {

            return View();

        }

        //
        // POST: /Admin/

        [HttpPost]
        [Authorize]
        public ActionResult Index (AdminPanel model)
        {
            if (ModelState.IsValid)
            {
                //need access to forum database to add the info
                //ForumEntitiesContext doesn't exist at the moment
                //workaround using exisiting list view
                //any forums created using this only exist during the session
                //when the application is running

                Forum newforum = new Forum();

                newforum.Name = model.forum.Name;
                newforum.Description = model.forum.Description;

                Forum.SetUpForumList();

                Forum.ForumList.Add(newforum);

                return View("../Forum/Index", Forum.ForumList);

            }
            else
            return View("Forbidden");
        }

        // GET: UserList

        [Authorize]
        public ActionResult UserList()
        {
            using (UserEntitiesContext db = new UserEntitiesContext())
            {
                var users = db.Users.ToList();

                return View(users);
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
