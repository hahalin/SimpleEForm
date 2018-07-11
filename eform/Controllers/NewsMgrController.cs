using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using eform.Attributes;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace eform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NewsMgrController : Controller
    {
        // GET: NewsMgr
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();
            var qry = from item in context.newsList.ToList<news>() orderby item.createTime descending select item;
            var model = qry.ToList<news>();
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var context = new ApplicationDbContext();
            var model = new news();
            var store = new UserStore<ApplicationUser>(context);
            var mgr = new ApplicationUserManager(store);
            var user = mgr.FindByName(User.Identity.Name);
            if (user != null)
            {
                model.uid = user.Id;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(news model)
        {
            var context = new ApplicationDbContext();
            if (string.IsNullOrEmpty(model.id))
            {
                model.id = Guid.NewGuid().ToString();
                context.newsList.Add(model);
            }
            else
            {
                news currentModel = context.newsList.Where(x => x.id == model.id).FirstOrDefault();
                currentModel.title = model.title;
                currentModel.createTime = DateTime.UtcNow.AddSeconds(28800);
                currentModel.content = model.content;
                currentModel.ndate = model.ndate;
            }
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }


            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var context = new ApplicationDbContext();
            news model = context.newsList.Where(x => x.id == id).FirstOrDefault();
            return View("Create", model);
        }
    }
    }