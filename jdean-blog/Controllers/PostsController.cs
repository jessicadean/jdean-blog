using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jdean_blog.Models; //Using Directives specify namespace, using statements (found in code) handles unmanaged resources
using jdean_blog.Helpers;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace jdean_blog.Controllers //must ref this if using elsewhere
{
    [RequireHttps]
    public class PostsController : Controller //public= Access Modifier || := inheritance || Controller= what you're inheriting from
    {
        private ApplicationDbContext db = new ApplicationDbContext(); //Instantiation of ApplicationDbContext. Why? Makes the db.operations possible. Permits CRUD operations

        // GET: Posts
        public ActionResult Index(int? page) //returns the collection to the specified view
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            if(Request.IsAuthenticated && User.IsInRole("Admin"))
            { 
            return View(db.Posts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
            }
            return View(db.Posts.Where(p => p.Published == true).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Index(string searchStr, int? page) //returns the collection to the specified view
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            ViewBag.Search = searchStr;
            SearchHelper search = new SearchHelper();
            var blogList = search.IndexSearch(searchStr);
           
            if(Request.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(blogList);
            }
            return View(blogList.Where(p => p.Published == true).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }
   

        // GET: Posts/Details/5
        public ActionResult Details(string Slug)
        {
            if(String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post blogPost = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }
        //public ActionResult Details(int? id) //passing the parameter. (data type integer nullable, possibly null paramter to be passed)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Post post = db.Posts.Find(id); // Post post = explicitly declared variable (compare to var = x, implicitly declaring variable), advantage of precompiler. looking for 1 post item using (id)
        //    if (post == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(post);
        //}

        // GET: Posts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken] //after user sends data with form, generates token. this matches and allows to proceed, else will throw antiforgerytooken error
        public ActionResult Create([Bind(Include = "Id,Title,Body,Created,Updated,MediaUrl,Published,Slug")] Post blogPost, HttpPostedFileBase image) //Bind Attribute tells it to add these properties when it sends to view
        {
            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");
            }
            if (ModelState.IsValid) //makes sure all the properties are bound
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }
                var filePath = "/assets/Images/";
                var absPath = Server.MapPath("~" + filePath);
                blogPost.MediaUrl = filePath + image.FileName;
                image.SaveAs(Path.Combine(absPath, image.FileName));
                blogPost.Slug = Slug;
                blogPost.Created = DateTime.Now;

                db.Posts.Add(blogPost); //now that it's a complete object, ready to add into specified table
                db.SaveChanges(); //becomes permanent data in the table. will insert row into table
                return RedirectToAction("Index");
            }
            return View();
        }
            

        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Created,Updated,MediaUrl,Published,Slug")]  Post blogPost, string MediaUrl, HttpPostedFileBase image) //Bind Attribute tells it to add these properties when it sends to view
        {
            blogPost.Updated = DateTime.Now;
            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");
            }

            if (ModelState.IsValid)
            {
                
                if (image != null)
                {
                var filePath = "/assets/Images/";
                var absPath = Server.MapPath("~" + filePath);
                blogPost.MediaUrl = filePath + image.FileName;
                image.SaveAs(Path.Combine(absPath, image.FileName));
                }
                else
                {
                    blogPost.MediaUrl = MediaUrl;
                }
                var title = db.Posts.AsNoTracking().FirstOrDefault(p => p.Id == blogPost.Id).Title;
                if (blogPost.Title != title)
                { 

                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                

                blogPost.Slug = Slug;
                }
                db.Entry(blogPost).State = EntityState.Modified; //upon submit button execute, will save changes
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing); //r click: go to definition: see the virtual methods that are made to be overidden
        }
    }
}
