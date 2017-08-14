using SimpleBlog.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using NHibernate.Linq;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using SimpleBlog.Areas.Admin.ViewModels;
using SimpleBlog.Infrastructure.Extentions;
using SimpleBlog.Models;

namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("posts")]
    public class PostsController : Controller
    {
        private const int PageSize = 5;

        // GET: Admin/Posts
        public ActionResult Index(int page = 1)
        {
            var totalPostsCount = Database.Session.Query<Post>().Count();

            var currentPagePostIs = Database.Session.Query<Post>()
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
       
        public ActionResult New()
        {
            return View("Form", new PostsForm
            {
                IsNew = true,
                Tags = Database.Session.Query<Tag>().Select(t => new TagCheckbox
                {
                    Id = t.Id,
                    Name = t.Name,
                    IsChecked = false
                }).ToList()
            });
        }

        public ActionResult Edit(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View("Form", new PostsForm
            {
                IsNew = false,
                PostId = id,
                Content = post.Content,
                Title = post.Title,
                Slug = post.Slug,
                Tags = Database.Session.Query<Tag>().Select(t => new TagCheckbox
                {
                    Id = t.Id,
                    Name = t.Name,
                    IsChecked = post.Tags.Contains(t)
                }).ToList()
            });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Form(PostsForm form)
        {
            form.IsNew = form.PostId == null;

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var selectedTags = ReconcileTags(form.Tags);

            Post post;
            if (form.IsNew)
            {
                post = new Post
                {
                    User = Auth.User,
                    CreatedAt = DateTime.UtcNow
                };
            }
            else
            {
                post = Database.Session.Load<Post>(form.PostId);

                if (post == null)
                {
                    return HttpNotFound();
                }

                post.UpdatedAt = DateTime.UtcNow;
            }

            post.Title = form.Title;
            post.Slug = form.Slug;
            post.Content = form.Content;
            post.Tags = selectedTags;

            Database.Session.SaveOrUpdate(post);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Trash(int id)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            post.DeletedAt = DateTime.UtcNow;

            Database.Session.Save(post);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            Database.Session.Delete(post);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Restore(int id)
        {
            var post = Database.Session.Load<Post>(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            post.DeletedAt = null;

            Database.Session.Save(post);

            return RedirectToAction("Index");
        }

        private IList<Tag> ReconcileTags(IEnumerable<TagCheckbox> tags)
        {
            var selectedTags = new List<Tag>();

            foreach (var tag in tags.Where(t => t.IsChecked))
            {
               

                if (tag.Id != null)
                {
                    selectedTags.Add(Database.Session.Load<Tag>(tag.Id));
                    continue;
                }

                var existingTag = Database.Session.Query<Tag>().FirstOrDefault(t => t.Name == tag.Name);

                if (existingTag != null)
                {
                    selectedTags.Add(existingTag);
                    continue;
                }

                var newTag = new Tag
                {
                    Name = tag.Name,
                    Slug = tag.Name.Slugify()
                };

                Database.Session.Save(newTag);
                selectedTags.Add(newTag);
            }

            return selectedTags;
        }
    }
}