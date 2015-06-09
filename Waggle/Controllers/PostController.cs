using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Waggle.Models;
using Waggle.ViewModels;
using WebMatrix.WebData;

namespace Waggle.Controllers
{
    public class PostController : Controller
    {
        private PostContext db = new PostContext();
        public static TopicPost tp { get; set; }
        public ActionResult Index(int TopicId)
        {
            //PostList is a View Model that should contain forum/topic info to pass on to the View.
            PostList list = new PostList();

            Topic topic = null; //The topic whose posts need to be displayed.
            using(TopicContext dbTopics = new TopicContext())
            {
                foreach(Topic t in dbTopics.Topics)
                {
                    if(t.TopicId == TopicId)
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
            p.PostTime = DateTime.Now.ToString();
            p.ReplyTo = -1; //This is a top-level post, not a reply
            Debug.WriteLine(p.PostTime);
            if(ModelState.IsValid)
            {
                db.Posts.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index", new { TopicId = p.TopicId});
            }
            return View();
        }

        [Authorize]
        public ActionResult Reply(int TopicId, int PostId)
        {
            Topic topic = null;
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
            tp = new TopicPost() { PostTopic = topic, NewPostUserId = WebSecurity.CurrentUserId, replyToPost = PostId };
            return View(tp);
        }

        public ActionResult ReplyCreated(string PostBody)
        {
            Post p = new Post();
            p.UserId = tp.NewPostUserId;
            p.TopicId = tp.PostTopic.TopicId;
            p.Body = PostBody;
            p.IsDeleted = false;
            // DateTime d = DateTime.Now;
            // string time = d.DayOfWeek + " " + d.Month + " " + d.day;
            p.PostTime = DateTime.Now.ToString();
            p.ReplyTo = tp.replyToPost; //This is a top-level post, not a reply
            if (ModelState.IsValid)
            {
                db.Posts.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index", new { TopicId = p.TopicId });
            }
            return View();
        }

        [Authorize]
        public ActionResult Edit(int PostId)
        {
            PostContext dbPosts = new PostContext();
            Post p = dbPosts.Posts.Find(PostId);
            tp = new TopicPost { EditPost = p, EditPostId = p.PostId };
            return View(tp);
        }

        public ActionResult Edited(string PostBody)
        {
            Post edited = db.Posts.Find(tp.EditPostId);
            if (edited != null)
            {
                Post afterEdit = new Post
                {
                    PostId = edited.PostId,
                    UserId = edited.UserId,
                    User = edited.User,
                    TopicId = edited.TopicId,
                    Topic = edited.Topic,
                    Body = PostBody,
                    IsDeleted = edited.IsDeleted,
                    PostTime = edited.PostTime,
                    ReplyTo = edited.ReplyTo
                };
                db.Entry(edited).CurrentValues.SetValues(afterEdit);
                db.SaveChanges();
                return RedirectToAction("Index", new { TopicId = edited.TopicId });
            }
            return RedirectToAction("Index", new { TopicId = 0 });
        }

        [Authorize]
        public ActionResult Delete(int PostId)
        {
            Post toDelete = db.Posts.Find(PostId);
            Post afterDelete = new Post
            {
                PostId = toDelete.PostId,
                UserId = toDelete.UserId,
                User = toDelete.User,
                TopicId = toDelete.TopicId,
                Topic = toDelete.Topic,
                Body = toDelete.Body,
                IsDeleted = true,
                PostTime = toDelete.PostTime,
                ReplyTo = toDelete.ReplyTo
            };
            db.Entry(toDelete).CurrentValues.SetValues(afterDelete);
            db.SaveChanges();
            return RedirectToAction("Index", new { TopicId = toDelete.TopicId });
        }
    }
}
