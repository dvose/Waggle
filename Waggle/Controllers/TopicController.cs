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
    public class TopicController : Controller
    {
        private PostContext Pdb = new PostContext();
        private TopicContext Tdb = new TopicContext();
        public static TopicPost tp { get; set; }

        [InitializeSimpleMembershipAttribute]
        public ActionResult Index(int ForumId)
        {
            //PostList is a View Model that should contain forum/topic info to pass on to the View.
            PostList topics = new PostList();
            List<Topic> topicsList = new List<Topic>();
            List<Waggle.Models.File> filesList = new List<Waggle.Models.File>();

            using (TopicContext dbTopics = new TopicContext())
            {
                foreach (Topic t in dbTopics.Topics)
                {
                    if (t.ForumId == ForumId)
                    {
                       topicsList.Add(t);
                    }
                }
            }
            using (FileEntitiesContext dbFiles = new FileEntitiesContext()) {
               foreach (Waggle.Models.File file in dbFiles.Files) {
                     if(file.Forum_Id == ForumId){
                        filesList.Add(file);
                     }
               }
            }
             
            topics.Topics = topicsList;
            topics.Files = filesList;
            topics.ForumId = ForumId; //The forum whose topics should be displayed
            //topics.ForumName = Forum.ForumList[ForumId].Name;

            using (ForumContext dbForums = new ForumContext())
            {
                foreach (Forum f in dbForums.Forums)
                {
                    if (f.ForumId == ForumId)
                    {
                        topics.ForumName = f.Name;
                        break;
                    }
                }
            }
            ViewData["currentUserId"] = WebSecurity.CurrentUserId;
            return View(topics);
        }

        [Authorize]
        public ActionResult CreateTopic(int ForumId)
        {
            Forum forum = null;
            using (ForumContext dbForums = new ForumContext())
            {
                foreach (Forum f in dbForums.Forums)
                {
                    if (f.ForumId == ForumId)
                    {
                        forum = f;
                        break;
                    }
                }
            }
            /*
            using (TopicContext dbTopics = new TopicContext())
            {
                foreach (Topic t in dbTopics.Topics)
                {
                    if (t.TopicId == topicId)
                    {
                        topic = t;
                        break;
                    }
                }
            }
             */
            tp = new TopicPost() { TopicForum = forum, NewPostUserId = 31 /*CURRENT USER HERE!*/ };
            return View(tp);
        }
        public ActionResult TopicCreated(string PostBody, string TopicName)
        {
            Topic t = new Topic();
            t.UserId = tp.NewPostUserId;
            t.ForumId = tp.TopicForum.ForumId;
            t.Name = TopicName;
            t.IsDeleted = false;
            if (ModelState.IsValid)
            {
                Tdb.Topics.Add(t);
                Tdb.SaveChanges();
            }

            Post p = new Post();
            p.UserId = tp.NewPostUserId;
            //p.UserId = 31; //TEMP
            p.TopicId = t.TopicId;
            p.Body = PostBody;
            p.IsDeleted = false;
            p.PostTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                Pdb.Posts.Add(p);
                Pdb.SaveChanges();
                return RedirectToAction("Index", new { ForumId = t.ForumId });
            }
            return View();
        }

    }
}
