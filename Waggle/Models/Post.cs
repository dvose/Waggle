using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Waggle.Models
{
    /*
     * Post is the inner-most layer of the forum. A post is contained in a topic, and cannot move to another topic after creation.
     * Posts can be made by any user who has access to the forum (and therefore post) that the topic is in.
     * The main purpose of a Post is its body, which is where a user puts their message. The body cannot be empty.
     */
    [Table("Post")]
    public class Post
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int PostId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Topic")]
        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        public string Body { get; set; }
        public bool IsDeleted { get; set; }
        //public User Deleter;
        [Column("Date_Stamp")]
        public string PostTime { get; set; }

        public int AttachmentId { get; set; }

        [NotMapped]
        public Waggle.Models.File attachment { get; set; }

        public void findAttachment()
        {
            using (FileEntitiesContext db = new FileEntitiesContext())
            {
                foreach (Waggle.Models.File file in db.Files)
                {
                    if (this.AttachmentId == file.Id)
                    {
                        this.attachment = file;
                        return;
                    }
                }
            }
        }

        //temp posts for testing until database is implemented
        //public static List<Post> PostList;
        //public static void SetUpPostList()
        //{
        //    PostList = new List<Post>{
        //        new Post{PostId = 0, TopicId = 0, Body = "This is the body of the first post in the first topic in the CS forum."},
        //        new Post{PostId = 1, TopicId = 0, Body = "This is the body of the second post in the first topic in the CS forum."},
        //        new Post{PostId = 2, TopicId = 0, Body = "This is the body of the third post in the first topic in the CS forum."},

        //        new Post{PostId = 3, TopicId = 1, Body = "This is the body of the first post in the second topic in the CS forum."},
        //        new Post{PostId = 4, TopicId = 1, Body = "This is the body of the second post in the second topic in the CS forum."},
        //        new Post{PostId = 5, TopicId = 1, Body = "This is the body of the third post in the second topic in the CS forum."},
        //    };
        //}
    }



    public class PostContext : DbContext
    {
        public PostContext()
            : base("WaggleDb")
        {
        }

        public DbSet<Post> Posts { get; set; }
    }
}