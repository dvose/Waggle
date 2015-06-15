using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Waggle.Models
{
    public class Forum_Permission
    {
            [Key]
            [Column("Id")]
            public int Forum_PermissionId { get; set; }
            [Column("User_Id")]
            public int UserId { get; set; }
            [Column("Forum_Id")]
            public int ForumId { get; set; }        
       
    }
    public class Forum_PermissionContext : DbContext
    {
        public Forum_PermissionContext()
            : base("WaggleDb")
        {
        }

        public DbSet<Forum_Permission> Forum_Permissions { get; set; }
    }
}