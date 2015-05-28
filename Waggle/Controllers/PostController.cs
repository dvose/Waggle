using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Waggle.Models;
using Waggle.ViewModels;

namespace Waggle.Controllers
{
    public class PostController : Controller
    {
        public ActionResult Index(int TopicId)
        {
            //PostList is a View Model that should contain forum/topic info to pass on to the View.
            PostList list = new PostList();
            Topic topic = Topic.TopicList[TopicId]; //The topic whose posts need to be displayed.
            list.TopicId = TopicId;
            list.TopicName = topic.Name;
            list.ForumName = Forum.ForumList[topic.ForumId].Name; //The forum that the topic is in
            list.ForumId = topic.ForumId;
            List<Post> posts = new List<Post>();
            foreach (Post p in Post.PostList)
            {
                if (p.TopicId == TopicId)
                {
                    posts.Add(p);
                }
            }
            list.Posts = posts;

            return View(list);
        }

        public ActionResult CreatePost()
        {

            return View();
        }
    }
}
