using DawForum.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DawForum.Controllers
{
    public class TopicController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult Index()
        {
            var topics = db.Topics.Include("Category").Include("User");
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ViewBag.Topics = topics;
            
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
        public ActionResult New()
        {
            Topic topic = new Topic();
            // preluam lista de categorii din metoda GetAllCategories()
            topic.Categories = GetAllCategories();
            // Preluam ID-ul utilizatorului curent
            topic.UserId = User.Identity.GetUserId();
            return View(topic);
        }

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

       
        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New(Topic topic)
        {
            topic.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Topics.Add(topic);
                    db.SaveChanges();
                    TempData["message"] = "Discutia a fost adaugata!";
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
            topic.Categories = GetAllCategories();

            if(topic.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator") || User.IsInRole("Administrator"))
            {
                return View(topic);
            } else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei discutii care nu va apartine!";
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

                    if (topic.UserId == User.Identity.GetUserId() || User.IsInRole("Moderator") || User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(topic))
                        {
                            topic.Title = requestTopic.Title;
                            topic.Content = requestTopic.Content;
                            topic.Date = requestTopic.Date;
                            topic.CategoryId = requestTopic.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "Discutia a fost modificata!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei discutii care nu va apartine!";
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
                TempData["message"] = "Discutia a fost stersa!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o discutie care nu va apartine!";
                return RedirectToAction("Index");
            }

        }


    }
}