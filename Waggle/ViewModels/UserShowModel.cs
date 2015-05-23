using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using Waggle.Models;

namespace Waggle.ViewModels{
    public class UserShowModel
    {
        public User user { get; set; }
        public UserProfile userprofile { get; set; }
        public List<File> files;
        public bool isCurrentUser { get; set; }
    }
}