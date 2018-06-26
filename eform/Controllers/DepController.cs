using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using System.Data.Entity.Infrastructure;

namespace eform.Controllers
{
    [Authorize]
    public class DepController : Controller
    {
        // GET: Dep
        public ActionResult Index(string id)
        {
            var context = new ApplicationDbContext();

            var model = context.deps;

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(dep model)
        {
            var db = new ApplicationDbContext();
            if (db.deps.Where(x=>x.depNm==model.depNm).Count()>0)
            {
                ModelState.AddModelError("depNm", "部門名稱重複");
            }
            try
            {
                model.depNo = Guid.NewGuid().ToString();
                if (ModelState.IsValid)
                {
                    db.deps.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException  dex )
            {
                ModelState.AddModelError("", dex.Message);
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
           }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        public ActionResult Details(string id)
        {
            var db = new ApplicationDbContext();
            var dep = db.deps.Where(x => x.depNo == id).SingleOrDefault<dep>();
            dep.depJobPos = db.jobPos.Where(x => x.depNo == id).ToList<jobPo>();
            ViewBag.id = id;
            var model = dep;
            return View(model);
        }
        public ActionResult CreatePo(string id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePo(jobPo model)
        {
            var db = new ApplicationDbContext();
            dep depObj = db.deps.Where(x => x.depNo == model.depNo).FirstOrDefault();
            if (db.jobPos.Where(x => x.poNm == model.poNm && x.depNo==model.depNo).Count() > 0)
            {
                ModelState.AddModelError("poNm", "部門職稱名稱重複");
            }
            try
            {
                model.poNo = Guid.NewGuid().ToString();
                if (ModelState.IsValid)
                {
                    db.jobPos.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Details","Dep",new {id = model.depNo});
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }
    }
}