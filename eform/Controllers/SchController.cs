using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using eform.Attributes;

namespace eform.Controllers
{
    [AdminAuthorize(Roles = "Admin")]
    public class SchController : Controller
    {
        ApplicationDbContext ctx;
        public SchController()
        {
            ctx = new ApplicationDbContext();
        }

        // GET: Sch
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult tempItemList(string id)
        {
            var list = ctx.schTempItemList.Where(x => x.pid == id).OrderBy(x => x.seq);
            ViewBag.schTemp = ctx.schTempList.Where(x => x.id == id).FirstOrDefault();
            return View(list);
        }

        [HttpGet]
        public ActionResult tempItemCreate(string pid)
        {
            schTempItem model = new schTempItem();
            model.pid = pid;
            return View(model);
        }

        [HttpPost]
        public ActionResult tempItemCreate(schTempItem model)
        {
            model.id = Guid.NewGuid().ToString();
            ctx.schTempItemList.Add(model);
            ctx.SaveChanges();
            return RedirectToAction("tempItemList", new {id = model.pid});
        }

        [HttpGet]
        public ActionResult tempItemRemove(string id,string pid)
        {
            ctx.schTempItemList.Remove(ctx.schTempItemList.Where(x => x.id == id).FirstOrDefault());
            ctx.SaveChanges();
            return RedirectToAction("tempItemList", new { id = pid });
        }

        public ActionResult tempList()
        {
            var list = ctx.schTempList.OrderBy(x => x.code);
            return View(list);
        }

        [HttpGet]
        public ActionResult tempCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult tempCreate(schTemp model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            if(ctx.schTempList.Where(x=>x.code==model.code).Count()>0)
            {
                ModelState.AddModelError("code", "分類編號重複");
                return View(model);
            }
            model.id = Guid.NewGuid().ToString();
            ctx.schTempList.Add(model);
            ctx.SaveChanges();

            return RedirectToAction("tempList");
        }

        [HttpGet]
        public ActionResult tempRemove(string id)
        {
            ctx.schTempItemList.RemoveRange(ctx.schTempItemList.Where(x => x.pid == id));
            ctx.schTempList.Remove(ctx.schTempList.Where(x => x.id == id).FirstOrDefault());
            ctx.SaveChanges();
            return RedirectToAction("tempList", new { id = id });
        }
    }
}