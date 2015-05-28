using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Waggle.Models;

namespace Waggle.ViewModels
{
    public class AdminPanel
    {
        public User user { get; set; }
        public Forum forum { get; set; }
        public bool isCurrentUser { get; set; }
    }
}