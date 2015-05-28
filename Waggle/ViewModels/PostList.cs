using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Waggle.Models;

namespace Waggle.ViewModels
{
    /*
     * View Model for the Topic and Post pages.
     */ 
    public class PostList
    {
        public string ForumName { get; set; }
        public int ForumId { get; set; }

        public string TopicName { get; set; }
        public int TopicId { get; set;  }

        public List<Post> Posts { get; set; }
        public List<Topic> Topics { get; set; }
    }
}