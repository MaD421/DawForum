using DawForum.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DawForum.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult Index(int id)
        {
            var comments = db.Comments.Include("User").Include("Topic").Where(p => p.Topic.Id == id);
            var topic = db.Topics.Find(id);

            if (TempData.ContainsKey("result"))
            {
                ViewBag.result = TempData["result"].ToString();
            }
            ViewBag.Topic = topic;
            ViewBag.Comments = comments;
            ViewBag.TopicId = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New(Comment comment, int id)
        {
            comment.TopicId = id;
            comment.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comment);
                    db.SaveChanges();
                    TempData["result"] = "Mesajul a fost adaugata!";
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    return RedirectToAction("Index", new { id = id });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", new { id = id });
            }
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id)
        {

            Comment comment = db.Comments.Find(id);
            ViewBag.Comment = comment;

            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator") || User.IsInRole("Administrator"))
            {
                return View(comment);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui comentariu care nu va apartine!";
                return RedirectToAction("Index", new { id = id });
            }
        }


        [HttpPut]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Comment comment = db.Comments.Find(id);

                    if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator") || User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(comment))
                        {
                            comment.Message = requestComment.Message;
                            comment.Date = requestComment.Date;
                            comment.TopicId = requestComment.TopicId;
                            db.SaveChanges();
                            TempData["message"] = "Comentariul a fost modificat!";
                        }
                        return RedirectToAction("Index", new { id = id });
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui comentariu care nu va apartine!";
                        return RedirectToAction("Index", new { id = id });
                    }


                }
                else
                {
                    return View();
                }

            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Delete(int id)
        {

            Comment comment = db.Comments.Find(id);

            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator") || User.IsInRole("Administrator"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost sters!";
                return RedirectToAction("Index", new { id = comment.TopicId });
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un comentariu care nu va apartine!";
                return RedirectToAction("Index", new { id = comment.TopicId });
            }

        }

    }
}