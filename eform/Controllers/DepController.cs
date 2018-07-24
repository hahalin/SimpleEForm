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

            var model = context.deps.OrderBy(x => x.sort).ToList<dep>();

            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.Title = "新增部門";
            ViewBag.Mode = "Create";
            return View();
        }
        public ActionResult Edit(string id)
        {
            var db = new ApplicationDbContext();
            var dep = db.deps.Where(x => x.depNo == id).SingleOrDefault<dep>();
            ViewBag.Title = "編輯部門";
            ViewBag.Mode = "Edit";
            var model = dep;
            return View("Create", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(dep model)
        {
            var context = new ApplicationDbContext();

            if (string.IsNullOrEmpty(model.depNo))
            {
                if (context.deps.Where(x => x.depNm == model.depNm).Count() > 0)
                {
                    ModelState.AddModelError("depNm", "部門名稱重複");
                }
                try
                {
                    model.depNo = Guid.NewGuid().ToString();
                    if (ModelState.IsValid)
                    {
                        context.deps.Add(model);
                        context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                if (context.deps.Where(x => x.depNm == model.depNm && x.depNo.Equals(model.depNo)==false).Count() > 0)
                {
                    ModelState.AddModelError("depNm", "部門名稱重複");
                }
                try
                {
                    if (ModelState.IsValid)
                    {
                        dep saveModel = context.deps.Where(x => x.depNo == model.depNo).FirstOrDefault();
                        saveModel.sort = model.sort;
                        saveModel.depNm = model.depNm;
                        context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(string hid)
        {
            dbHelper dbh = new dbHelper();
            dbh.execSql("delete from jobpoes where depNo='" + hid + "'");
            dbh.execSql("delete from deps where depNo='" + hid + "'");
            dbh.execSql("delete from PoUsers where poNo in (select poNo from jobPoes)");
            return RedirectToAction("Index");
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
            ViewBag.Title = "新增職稱";
            ViewBag.Mode = "Create";
            jobPo model = new jobPo();
            model.depNo = id;
            return View(model);
        }

        public ActionResult EditPo(string id)
        {
            ViewBag.Title = "編輯職稱";
            ViewBag.Mode = "Edit";
            ViewBag.id = id;
            var context = new ApplicationDbContext();
            jobPo model = context.jobPos.Where(x => x.poNo == id).FirstOrDefault();
            return View("CreatePo",model);
        }
        [HttpGet]
        public ActionResult DeletePo(string id)
        {
            dbHelper dbq = new dbHelper();
            dbq.execSql("delete from jobPoes where poNo='" + id + "'");
            return RedirectToAction("Details", "Dep", new { id = Request["depNo"].ToString() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePo(jobPo model)
        {
            var context = new ApplicationDbContext();
            if (string.IsNullOrEmpty(model.poNo))
            {
                dep depObj = context.deps.Where(x => x.depNo == model.depNo).FirstOrDefault();
                if (context.jobPos.Where(x => x.poNm == model.poNm && x.depNo == model.depNo).Count() > 0)
                {
                    ModelState.AddModelError("poNm", "部門職稱名稱重複");
                }
                try
                {
                    model.poNo = Guid.NewGuid().ToString();
                    if (ModelState.IsValid)
                    {
                        context.jobPos.Add(model);
                        context.SaveChanges();
                        return RedirectToAction("Details", "Dep", new { id = model.depNo });
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                dep depObj = context.deps.Where(x => x.depNo == model.depNo).FirstOrDefault();
                if (context.jobPos.Where(x => x.poNm == model.poNm && x.depNo == model.depNo && x.poNo.Equals(model.poNo)==false).Count() > 0)
                {
                    ModelState.AddModelError("poNm", "部門職稱名稱重複");
                }
                try
                {
                    if (ModelState.IsValid)
                    {
                        jobPo saveModel = context.jobPos.Where(x => x.poNo == model.poNo).FirstOrDefault();
                        saveModel.poNm = model.poNm;
                        saveModel.isFormSigner = model.isFormSigner;
                        context.SaveChanges();
                        return RedirectToAction("Details", "Dep", new { id = model.depNo });
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

    }
}