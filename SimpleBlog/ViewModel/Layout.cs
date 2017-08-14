using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog.ViewModel
{
    public class LayoutSidebar
    {
        public bool IsLogedIn { get; set; }
        public bool IsAdmin { get; set; }
        public string Username { get; set; }
        public IEnumerable<SidebarTag> Tags { get; set; }
    }

    public class SidebarTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int PostCount { get; set; }
    }
}