using PetCare_system.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index()
        {
            return View(db.Posts.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
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

        // GET: Posts/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post post, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var userId = Session["LoggedInUserId"]?.ToString();
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = db.Users.FirstOrDefault(u => u.Id == userId);

                    if (user != null)
                    {
                        post.UserName = $"{user.FirstName} {user.LastName}";

                        if (ImageFile != null && ImageFile.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(ImageFile.FileName);
                            string filePath = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                            ImageFile.SaveAs(filePath);
                            post.AttachmentUrl = "/Uploads/" + fileName;
                        }
                        post.UserId = userId;
                        db.Posts.Add(post);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                ModelState.AddModelError("", "User not logged in or invalid session.");
            }

            return View(post);
        }







        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Post_Id,Category,Title,Body,AttachmentUrl,UserName,UserProfilePic,Timestamp")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
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
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(Comment comment)
        {
            var currentUserId = Session["LoggedInUserId"]?.ToString();

            if (string.IsNullOrEmpty(currentUserId))
                return RedirectToAction("Index");

            var post = db.Posts.Include("Comments")
                               .FirstOrDefault(p => p.Post_Id == comment.Post_Id);

            if (post == null)
                return RedirectToAction("Index"); // Or show an error

            var latestComment = post.Comments.OrderByDescending(c => c.CreatedAt).FirstOrDefault();
            var isSelfPost = post.UserId == currentUserId;
            var isSelfLatestComment = latestComment?.UserId == currentUserId;

            if (isSelfPost || isSelfLatestComment)
            {
                TempData["ErrorMessage"] = "You cannot reply to your own post or latest comment.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                comment.UserId = currentUserId;
                comment.UserName = Session["LoggedInUserName"]?.ToString() ?? "Anonymous";
                comment.CreatedAt = DateTime.Now;

                db.Comments.Add(comment);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
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
