using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Waggle.Filters;
using Waggle.Models;
using Waggle.ViewModels;
using WebMatrix.WebData;

namespace Waggle.Controllers
{
    public class PostController : Controller
    {
        private PostContext db = new PostContext();
        public static TopicPost tp { get; set; }

         [InitializeSimpleMembershipAttribute]
        public ActionResult Index(int TopicId)
        {
            //PostList is a View Model that should contain forum/topic info to pass on to the View.
            PostList list = new PostList();

            Topic topic = null; //The topic whose posts need to be displayed.
            using (TopicContext dbTopics = new TopicContext())
            {
                foreach (Topic t in dbTopics.Topics)
                {
                    if (t.TopicId == TopicId)
                    {
                        topic = t;
                        break;
                    }
                }
            }
            if (topic != null)
            {
                list.TopicId = TopicId;
                list.TopicName = topic.Name;
                //list.ForumName = Forum.ForumList[topic.ForumId].Name; //The forum that the topic is in
                using (ForumContext dbForums = new ForumContext())
                {
                    foreach (Forum f in dbForums.Forums)
                    {
                        if (f.ForumId == topic.ForumId)
                        {
                            list.ForumName = f.Name;
                        }
                    }
                }
                list.ForumId = topic.ForumId;
                List<Post> posts = new List<Post>();
                using (PostContext dbPosts = new PostContext())
                {
                    foreach (Post p in dbPosts.Posts)
                    {
                        if (p.TopicId == TopicId)
                        {
                            posts.Add(p);
                        }
                    }
                }
                list.Posts = posts;

                return View(list);
            }
            return View("ERROR"); //Show an error or something. The topicId given as a parameter to this method doesn't exist.
        }

        [Authorize]
        public ActionResult CreatePost(int topicId)
        {
            Topic topic = null;
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
            tp = new TopicPost() { PostTopic = topic, NewPostUserId = WebSecurity.CurrentUserId };
            return View(tp);
        }

        public ActionResult PostCreated(string PostBody)
        {
            Post p = new Post();
            p.UserId = tp.NewPostUserId;
            p.TopicId = tp.PostTopic.TopicId;
            p.Body = PostBody;
            p.IsDeleted = false;
            // DateTime d = DateTime.Now;
            // string time = d.DayOfWeek + " " + d.Month + " " + d.day;
            p.PostTime = DateTime.Now.ToString();
            Debug.WriteLine(p.PostTime);
            if (ModelState.IsValid)
            {
                db.Posts.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index", new { TopicId = p.TopicId });
            }
            return View();
        }
    }
}