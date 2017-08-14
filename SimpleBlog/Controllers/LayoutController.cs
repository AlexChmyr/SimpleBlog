using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using SimpleBlog.Models;
using SimpleBlog.ViewModel;

namespace SimpleBlog.Controllers
{
    public class LayoutController : Controller
    {
        [ChildActionOnly]
        public ActionResult Sidebar()
        {
            var isLoggedIn = Auth.User != null;
            var username = Auth.User != null ? Auth.User.Username : "";
            var isAdmin = User.IsInRole("Admin");

            var tags = Database.Session.Query<Tag>()
                .Select(t => new SidebarTag
                {
                    Id = t.Id,
                    Name = t.Name,
                    Slug = t.Slug,
                    PostCount = t.Posts.Count
                })
                .Where(t => t.PostCount > 0)
                .OrderByDescending(t => t.PostCount)
                .ToList();

            return View(new LayoutSidebar
            {
                IsLogedIn = isLoggedIn,
                IsAdmin = isAdmin,
                Username = username,
                Tags = tags
            });
        }
    }
}