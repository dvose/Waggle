using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Waggle.Models
{
    public class FileEntitiesContext : DbContext
    {
         public FileEntitiesContext()
            : base("WaggleDb")
        {
        }

        public DbSet<Waggle.Models.File> Files { get; set; }
    }

    [Table("File")]
    public class File {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int User_Id { get; set; }
        public User User { get; set; }

        public int Forum_Id { get; set; }

        [Column("File_Name")]
        public string fileName { get; set; }

        [Column("File_Path")]
        public String filePath { get; set; }

        [Column("File_Type")]
        public String fileType { get; set; }

        [Column("File_Display_Name")]
        public String fileDisplayName { get; set; }
    }
}
