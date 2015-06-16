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
    public class ForumController : Controller
    {
        private ForumContext db = new ForumContext();
        [InitializeSimpleMembershipAttribute]
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

        [Authorize]
        public ActionResult DeleteForum(int forumID)
        {
            if (User.IsInRole("admin"))
            {
                using (ForumContext Fdb = new ForumContext())
                {
                    foreach (Forum f in Fdb.Forums)
                    {
                        if (f.ForumId == forumID)
                        {
                            f.IsDeleted = true;

                            using (TopicContext Tdb = new TopicContext())
                            {
                                foreach (Topic t in Tdb.Topics)
                                {
                                    if (t.ForumId == forumID)
                                    {
                                        t.IsDeleted = true;

                                        int topicID = t.TopicId;

                                        using (PostContext Pdb = new PostContext())
                                        {
                                            foreach (Post p in Pdb.Posts)
                                            {
                                                if (p.TopicId == topicID)
                                                {
                                                    p.IsDeleted = true;
                                                }
                                            }
                                            Pdb.SaveChanges();
                                        }
                                    }
                                }
                                Tdb.SaveChanges();
                            }
                        }
                    }
                    Fdb.SaveChanges();
                }
                return RedirectToAction("Index", "Forum");
            }
            else
            {
                return RedirectToAction("Http404", "Error");
            }
        }


        [Authorize]
        public ActionResult UndeleteForum(int forumID)
        {
            if (User.IsInRole("admin"))
            {
                using (ForumContext Fdb = new ForumContext())
                {
                    foreach (Forum f in Fdb.Forums)
                    {
                        if (f.ForumId == forumID)
                        {
                            f.IsDeleted = false;

                            using (TopicContext Tdb = new TopicContext())
                            {
                                foreach (Topic t in Tdb.Topics)
                                {
                                    if (t.ForumId == forumID)
                                    {
                                        t.IsDeleted = false;

                                        int topicID = t.TopicId;

                                        using (PostContext Pdb = new PostContext())
                                        {
                                            foreach (Post p in Pdb.Posts)
                                            {
                                                if (p.TopicId == topicID)
                                                {
                                                    p.IsDeleted = false;
                                                }
                                            }
                                            Pdb.SaveChanges();
                                        }
                                    }
                                }
                                Tdb.SaveChanges();
                            }
                        }
                    }
                    Fdb.SaveChanges();

                    return RedirectToAction("Index", "Forum");
                }
            }
            else
            {
                return RedirectToAction("Http404", "Error");
            }
        }

        [Authorize]
        [InitializeSimpleMembershipAttribute]
        public ActionResult CreateForum()
        {
            if (User.IsInRole("admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Forbidden", "Admin");
            }
        }

        [HttpPost]
        [Authorize]
        [InitializeSimpleMembership]
        public ActionResult CreateForum(AdminPanel model)
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
                        return RedirectToAction("Index", "Forum");
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

        [Authorize]
        public ActionResult EditForum(int ForumId)
        {
            if (User.IsInRole("admin"))
            {
                Forum model = null;
                using (ForumContext db = new ForumContext()) {
                    foreach (Forum forum in db.Forums) {
                        if (forum.ForumId == ForumId) {
                            model = forum;
                        }
                    }
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Forbidden", "Admin");
            }
        }

        [Authorize]
        public ActionResult TogglePrivacy(int forumId) {
            if (User.IsInRole("admin"))
            {
                Forum f = null;
                using (ForumContext db = new ForumContext())
                {
                    foreach (Forum forum in db.Forums)
                    {
                        if (forum.ForumId == forumId)
                        {
                            forum.IsPrivate = !forum.IsPrivate;
                            f = forum;
                        }
                    }
                    db.SaveChanges();
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }             
            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        

        [HttpPost]
        [Authorize]
        public ActionResult SubmitEdit(Forum model)
        {
            if (User.IsInRole("admin"))
            {
                if (ModelState.IsValid)
                {
                    using (ForumContext Fdb = new ForumContext())
                    {
                        foreach (Forum f in Fdb.Forums)
                        {
                            if (f.ForumId == model.ForumId)
                            {
                                f.Name = model.Name;
                                f.Description = model.Description;
                            }
                        }
                        Fdb.SaveChanges();
                        return RedirectToAction("Index", "Forum", Fdb.Forums);
                    }

                }
                return RedirectToAction("Index", "Forum");
            }
            else
            {
                return RedirectToAction("Forbidden", "Admin");
            }
        }
    }
}
