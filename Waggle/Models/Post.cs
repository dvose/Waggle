using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Waggle.Models
{
    /*
     * Post is the inner-most layer of the forum. A post is contained in a topic, and cannot move to another topic after creation.
     * Posts can be made by any user who has access to the forum (and therefore post) that the topic is in.
     * The main purpose of a Post is its body, which is where a user puts their message. The body cannot be empty.
     */
    public class Post
    {
        public int PostId;
        //public User Creator;
        public int TopicId;
        public string Body { get; set; }
        public bool IsDeleted = false;
        //public User Deleter;
        public DateTime PostTime;

        //temp posts for testing until database is implemented
        public static List<Post> PostList;
        public static void SetUpPostList()
        {
            PostList = new List<Post>{
                new Post{PostId = 0, TopicId = 0, Body = "This is the body of the first post in the first topic in the CS forum."},
                new Post{PostId = 1, TopicId = 0, Body = "This is the body of the second post in the first topic in the CS forum."},
                new Post{PostId = 2, TopicId = 0, Body = "This is the body of the third post in the first topic in the CS forum."},

                new Post{PostId = 3, TopicId = 1, Body = "This is the body of the first post in the second topic in the CS forum."},
                new Post{PostId = 4, TopicId = 1, Body = "This is the body of the second post in the second topic in the CS forum."},
                new Post{PostId = 5, TopicId = 1, Body = "This is the body of the third post in the second topic in the CS forum."},
            };
        }
    }
}