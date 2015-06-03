using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Waggle.Models;


namespace Waggle.Controllers
{
    public class ForumController : Controller
    {
        private ForumContext db = new ForumContext();
        public ActionResult Index()
        {
            //These create lists of Forums, Topics, and Posts used for testing until the database is up.
           // Forum.SetUpForumList(); //A list of all forums.
            //Topic.SetUpTopicList(); //A list of all Topics.
           // Post.SetUpPostList(); //A list of all posts

            //This will need to be changed to something that includes user information
            //Because some forums are private and there will be different options based whether they are an admin

            return View(db.Forums.ToList());
        }

    }
}
