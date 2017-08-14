using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Infrastructure;
using SimpleBlog.Models;
using SimpleBlog.ViewModel;

namespace SimpleBlog.Controllers
{
    public class PostsController : Controller
    {
        private const int PageSize = 5;

        public ActionResult Index(int page = 1)
        {
            var totalPostsCount = Database.Session.Query<Post>().Count(p => p.DeletedAt == null);

            var currentPagePostIs = Database.Session.Query<Post>()
                .Where(p => p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(p => p.Id)
                .ToArray();

            var currentPagePosts = Database.Session.Query<Post>()
                .Where(p => currentPagePostIs.Contains(p.Id))
                .OrderByDescending(p => p.CreatedAt)
                .FetchMany(p => p.Tags)
                .Fetch(p => p.User)
                .ToList();


            return View(new PostsIndex
            {
                Posts = new PagedData<Post>(currentPagePosts, totalPostsCount, page, PageSize)
            });
        }

        public ActionResult Show(int id, string slug)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null || post.IsDeleted)
            {
                return HttpNotFound();
            }

            return View(new PostsShow
            {
                Post = post
            });
        }

        public ActionResult Tag(int id, string slug, int page = 1)
        {
            var tag = Database.Session.Load<Tag>(id);

            if (tag == null)
            {
                return HttpNotFound();
            }

            var totalPostsCount = tag.Posts.Count(p => p.DeletedAt == null);

            var currentPagePostIs = tag.Posts
                .Where(p => p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(p => p.Id)
                .ToArray();

            var currentPagePosts = Database.Session.Query<Post>()
                .Where(p => currentPagePostIs.Contains(p.Id))
                .OrderByDescending(p => p.CreatedAt)
                .FetchMany(p => p.Tags)
                .Fetch(p => p.User)
                .ToList();


            return View(new PostsTag
            {
                Tag = tag,
                Posts = new PagedData<Post>(currentPagePosts, totalPostsCount, page, PageSize)
            });

        }
    }
}