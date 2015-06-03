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
     * Forum is the outer-most layer of the forum. It is a broad field such as CS or Math.
     * Forums can be created and deleted by admins and can be public or private (only certain users may see a private forum)
     * The main purpose of a forum is to include a list of user-created Topics, but this list can be empty (such as right after creation)
     */
    [Table("Forum")]
    public class Forum
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int ForumId { get; set; }
        [Column("User_Id")]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        //public User Deleter;

        //Set some forums up for testing. Forum list will come from Database once there is a database!
        /*
        public static List<Forum> ForumList;
        public static void SetUpForumList()
        {
            ForumList = new List<Forum>
            {
                new Forum{ForumId = 0, Name = "Computer Science", Description = "A forum for discussing Computer Science"},
                new Forum{ForumId = 1, Name = "Mathematics", Description = "A forum to discuss math"},
                new Forum{ForumId = 2, Name = "Offtopic", Description = "A forum to discuss things not related to school"},
                new Forum{ForumId = 3, Name = "Deleted Forum", Description = "A forum that has been deleted.", IsDeleted = true}
            };
        }
         */
    }
    public class ForumContext : DbContext
    {
        public ForumContext()
            : base("WaggleDb")
        {
        }

        public DbSet<Forum> Forums { get; set; }
    }

}