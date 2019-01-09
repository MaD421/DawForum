using DawForum.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DawForum.Controllers
{
    public class TopicController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult Index(int id,int type)
        {
            var topics = db.Topics.Where(p => p.CategoryId == id).Include("User").Include("Category");
            if (type == 1)
            {
                ViewBag.type = 1;
                ViewBag.Topics = topics.OrderBy(p => p.Id);
            }
            else
            {
                ViewBag.type = 2;
                ViewBag.Topics = topics.OrderBy(p => p.Title);
            }
            ViewBag.CategoryId = id;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View();
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New(int id)
        {
            Topic topic = new Topic();
            topic.CategoryId = id;
            topic.UserId = User.Identity.GetUserId();
            return View(topic);
        }
       
        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New(Topic topic, int id)
        {
            topic.CategoryId = id;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Topics.Add(topic);
                    db.SaveChanges();
                    TempData["message"] = "Articolul a fost adaugat!";
                    return RedirectToAction("Index", new { id = id, type = 1 });
                }
                else
                {
                    return View(topic);
                }
            } 
            catch (Exception e)
            {
                return View(topic);
            }
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id)
        {

            Topic topic = db.Topics.Find(id);
            ViewBag.Topic = topic;
            topic.CategoryId = id;

            if(topic.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator") || User.IsInRole("Administrator"))
            {
                return View(topic);
            } else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                return RedirectToAction("Index", new { id = id, type = 1 });
            }
        }

        
        [HttpPut]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id, Topic requestTopic)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Topic topic = db.Topics.Find(id);

                    if (topic.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator") || User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(topic))
                        {
                            topic.Title = requestTopic.Title;
                            topic.Content = requestTopic.Content;
                            topic.Date = requestTopic.Date;
                            topic.CategoryId = requestTopic.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "Articolul a fost modificat!";
                        }
                        return RedirectToAction("Index", new { id = id, type = 1 });
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                        return RedirectToAction("Index", new { id = id, type = 1 });
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

            Topic topic = db.Topics.Find(id);

            if (topic.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator") || User.IsInRole("Administrator"))
            {
                db.Topics.Remove(topic);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters!";
                return RedirectToAction("Index", new { id = topic.CategoryId, type = 1 });
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un articol care nu va apartine!";
                return RedirectToAction("Index", new { id = topic.CategoryId, type = 1 });
            }

        }


    }
}