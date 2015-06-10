using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Waggle.Models;

namespace Waggle.ViewModels
{
    public class TopicPost
    {
        public Forum TopicForum;
        public Topic PostTopic { get; set; }

        [DataType(DataType.MultilineText)]
        public string NewPostBody { get; set; }
        public int NewPostUserId { get; set; }
        public Post EditPost { get; set; }
        public int EditPostId { get; set; }
        public string NewTopicTitle { get; set; }
        public int replyToPost { get; set; }
        public int forumId { get; set; }
    }
}