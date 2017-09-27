using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jdean_blog.Models;
using System.Collections;
using Microsoft.AspNet.Identity;

namespace jdean_blog.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //// GET: Comments
        //public ActionResult Index()
        //{
        //    var comments = db.Comments.Include(c => c.Author).Include(c => c.BlogPost);
        //    return View(comments.ToList());
        //}

        //// GET: Comments/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Comment comment = db.Comments.Find(id);
        //    if (comment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(comment);
        //}

        //// GET: Comments/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
        //    ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title");
        //    return View();
        //}

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BlogPostId,Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                comment.CreationDate = DateTime.Now;
                comment.AuthorId = user.Id;

                db.Comments.Add(comment);
                db.SaveChanges();

                Post blogPost = db.Posts.Find(comment.BlogPostId);
                return RedirectToAction("Details", "Posts", new { slug = blogPost.Slug});
            }

            //ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            //ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title", comment.BlogPostId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Comment comment = db.Comments.Find(id);
            if ((User.IsInRole("Admin") || User.IsInRole("Moderator")) || comment.AuthorId == user.Id)
                //
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
            //ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            //ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title", comment.BlogPostId);
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BlogPostId,AuthorId,Body,CreationDate,UpdatedReason")] Comment comment)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if ((User.IsInRole("Admin") || User.IsInRole("Moderator")) || comment.AuthorId == user.Id)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(comment).State = EntityState.Modified;
                    comment.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                    Post blogPost = db.Posts.Find(comment.BlogPostId);
                    return RedirectToAction("Details", "Posts", new { slug = blogPost.Slug });
                }
                ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title", comment.BlogPostId);
                return View(comment);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Comment comment = db.Comments.Find(id);
            if ((User.IsInRole("Admin") || User.IsInRole("Moderator") || comment.AuthorId == user.Id))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Comment comment = db.Comments.Find(id);

            if ((User.IsInRole("Admin") || User.IsInRole("Moderator") || comment.AuthorId == user.Id))
            {

                db.Comments.Remove(comment);
                db.SaveChanges();
                Post blogPost = db.Posts.Find(comment.BlogPostId);
                return RedirectToAction("Details", "Posts", new { slug = blogPost.Slug });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
