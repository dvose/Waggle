using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Waggle.Models;
using Waggle.ViewModels;

namespace Waggle.Controllers
{
    public class TopicController : Controller
    {

        public ActionResult Index(int ForumId)
        {
            //PostList is a View Model that should contain forum/topic info to pass on to the View.
            PostList topics = new PostList();
            List<Topic> topicsList = new List<Topic>(); 
            foreach (Topic t in Topic.TopicList)
            {
                if (t.ForumId == ForumId)
                {
                    topicsList.Add(t);
                }
            }
            topics.Topics = topicsList;
            topics.ForumId = ForumId; //The forum whose topics should be displayed
            topics.ForumName = Forum.ForumList[ForumId].Name;
            return View(topics);
        }

    }
}
