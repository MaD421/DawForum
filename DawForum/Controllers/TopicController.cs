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

        public ActionResult Index(int id)
        {
            var topics = db.Topics.Where(p => p.CategoryId == id).Include("User").Include("Category");
            var test = topics.ToList();
            ViewBag.Topics = test;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View();
        }

        public ActionResult Show(int id)
        {
            Topic topic = db.Topics.Find(id);
            ViewBag.Topic = topic;
            ViewBag.Category = topic.Category;
            ViewBag.afisareButoane = false;
            if(User.IsInRole("Moderator") || User.IsInRole("Administrator")){
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Administrator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View(topic);
           
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New(int id)
        {
            Topic topic = new Topic();
            topic.CategoryId = id;
            // Preluam ID-ul utilizatorului curent
            topic.UserId = User.Identity.GetUserId();
            return View(topic);
        }
        /*
        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();
            // Extragem toate categoriile din baza de date
            var categories = from cat in db.Categories select cat;
            // iteram prin categorii
            foreach(var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            // returnam lista de categorii
            return selectList;
        }
        */
       
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
                    return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
                    ViewBag.id = topic.Id;

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
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                        return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un articol care nu va apartine!";
                return RedirectToAction("Index");
            }

        }


    }
}