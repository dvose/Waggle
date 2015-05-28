using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Waggle.Models
{
    /*
     * Topic is the middle layer in the forum. A topic is contained in a Forum, and cannot move to another forum once created.
     * Topics can be created by any user who has access to the forum it is being created in. 
     * The main purpose of a topic is to contain a list of posts. This list cannot be empty; the creating user must create the first post when creating a topic.
     */
    public class Topic
    {
        public int TopicId;
        //public User creator;
        public int ForumId;
        public string Name { get; set; }
        public string Description { get; set; }


        //Test topics. Actual topics should come from the databse once there is a database
        public static List<Topic> TopicList;
        public static void SetUpTopicList()
        {
            TopicList = new List<Topic>
            {
                new Topic{TopicId = 0, ForumId = 0, Name = "Big Data: How Big is Too Big? [CS Topic 1]"},
                new Topic{TopicId = 1, ForumId = 0, Name = "I'm bad at math and programming, should I major in SWE? [CS Topic 2]"},
                new Topic{TopicId = 2, ForumId = 0, Name = "ASP .NET MVC Tutorial [CS Topic 3]"},

                new Topic{TopicId = 3, ForumId = 1, Name = "Math Topic 1"},
                new Topic{TopicId = 4, ForumId = 1, Name = "Math Topic 2"},
                new Topic{TopicId = 5, ForumId = 1, Name = "Math Topic 3"}
            };
        }
    }
}