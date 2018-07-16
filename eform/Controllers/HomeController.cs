using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using eform.Models;

namespace eform.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();
            List<news> newslist = (from item in context.newsList.ToList<news>() orderby item.createTime descending select item).ToList<news>();
            ViewBag.newslist = newslist;
            return View();
        }

        public ActionResult NewsDetail(string id)
        {
            var context = new ApplicationDbContext();
            news Model = context.newsList.Where(x => x.id == id).FirstOrDefault();
            return View(Model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}