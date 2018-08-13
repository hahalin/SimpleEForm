using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using System.Data.Entity.Infrastructure;
using System.Web.Mvc.Html;

namespace eform.Controllers
{
    [Authorize]
    public class DepController : Controller
    {
        // GET: Dep
        public ActionResult Index(int depLevel=1,string pId="001")
        {
            var context = new ApplicationDbContext();
            var model = context.deps.Where(x=>x.depLevel==depLevel && x.parentDepNo==pId).OrderBy(x => x.sort).ToList<dep>();
            if (depLevel == 2)
            {
                List<dep> pDepList = context.deps.Where(x => x.depLevel == 1).ToList<dep>();
                List<SelectListItem> depSelList = new List<SelectListItem>();

                depSelList.Add(new SelectListItem
                {
                    Value = "0",
                    Text = "請選擇所屬【處】"
                });

                foreach (dep depObj in pDepList)
                {
                    depSelList.Add(new SelectListItem
                    {
                        Value=depObj.depNo,
                        Text=depObj.depNm
                    });
                };

                foreach (SelectListItem item in depSelList)
                {
                  item.Selected = item.Value == pId;
                }
                ViewBag.depLevel = depLevel;
                ViewBag.depSelList = depSelList;
            }

            if (depLevel == 3)
            {
                List<dep> pDepList = context.deps.Where(x => x.depLevel == 1).ToList<dep>();
                List<SelectListItem> depSelList = new List<SelectListItem>();

                depSelList.Add(new SelectListItem
                {
                    Value = "0",
                    Text = "請選擇所屬【部門】"
                    //,Enabled= true
                });

                foreach (dep depObj in pDepList)
                {
                    //depSelList.Add(new SelectListItemExtends
                    //{
                    //    Value = depObj.depNo,
                    //    Text = depObj.depNm,
                    //    Enabled=false
                    //});
                    var gp = new SelectListGroup { Name = depObj.depNm };

                    List<SelectListItem> depSubSelList = new List<SelectListItem>();
                    List<dep> pDepSubList = context.deps.Where(x => x.depLevel == 2 && x.parentDepNo==depObj.depNo).ToList<dep>();
                    foreach(dep depSubObj in pDepSubList)
                    {
                        depSelList.Add(new SelectListItem
                        {
                            Value = depSubObj.depNo,
                            Text = depSubObj.depNm,
                            Group = gp
                            //Enabled = true
                        });
                    }

                    foreach (SelectListItem item in depSelList)
                    {
                        item.Selected = item.Value == pId;
                    }
                    ViewBag.depLevel = depLevel;
                    ViewBag.depSelList = depSelList;

                };

                foreach (SelectListItem item in depSelList)
                {
                    item.Selected = item.Value == pId;
                }
                ViewBag.depLevel = depLevel;
                ViewBag.depSelList = depSelList;
            }

            ViewBag.depLevel = depLevel;
            ViewBag.pId = pId;
            return View(model);
        }

        public ActionResult Create(int depLevel=1,string pId="001")
        {
            switch (depLevel)
            {
                case 1:
                    ViewBag.Title = "新增-處級";
                    break;
                case 2:
                    ViewBag.Title = "新增-部門";
                    break;
                case 3:
                    ViewBag.Title = "新增-課別";
                    break;
            }

            ViewBag.Mode = "Create";
            ViewBag.depLevel = depLevel;
            dep model = new dep
            {
                parentDepNo=pId,
                depLevel = depLevel
            };
            return View(model);
        }
        public ActionResult Edit(string id)
        {
            var db = new ApplicationDbContext();
            var dep = db.deps.Where(x => x.depNo == id).SingleOrDefault<dep>();
            int depLevel = dep.depLevel;
            switch (depLevel)
            {
                case 1:
                    ViewBag.Title = "編輯-處級單位";
                    break;
                case 2:
                    ViewBag.Title = "編輯-部門單位";
                    break;
                case 3:
                    ViewBag.Title = "編輯-課別單位";
                    break;
            }

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
                if (context.deps.Where(x => x.depNm == model.depNm && x.depLevel==model.depLevel).Count() > 0)
                {
                    ModelState.AddModelError("depNm", "名稱重複");
                }
                try
                {
                    model.depNo = Guid.NewGuid().ToString();
                    if (ModelState.IsValid)
                    {
                        context.deps.Add(model);
                        context.SaveChanges();
                        return RedirectToAction("Index",new { depLevel = model.depLevel,pId=model.parentDepNo});
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                if (context.deps.Where(x => x.depNm == model.depNm && x.depLevel==model.depLevel && x.depNo.Equals(model.depNo)==false).Count() > 0)
                {
                    ModelState.AddModelError("depNm", "名稱重複");
                }
                try
                {
                    if (ModelState.IsValid)
                    {
                        dep saveModel = context.deps.Where(x => x.depNo == model.depNo).FirstOrDefault();
                        saveModel.sort = model.sort;
                        saveModel.depNm = model.depNm;
                        saveModel.depLevel = model.depLevel;
                        context.SaveChanges();
                        return RedirectToAction("Index",new { depLevel = model.depLevel,pId=model.parentDepNo });
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
            if(string.IsNullOrEmpty(model.poNm))
            {
                ModelState.AddModelError("poNm", "不可為空值");
                return View(model);
            }

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

        [HttpGet]
        public ActionResult Gm()
        {
            var ctx = new ApplicationDbContext();
            dep model = ctx.deps.Where(x => x.depNo == "001").Count() == 0 ? new dep() : ctx.deps.Where(x => x.depNo == "001").First();
            return View(model);
        }

        [HttpPost]
        public ActionResult Gm (dep Model)
        {
            var ctx = new ApplicationDbContext();
            dep tModel = null;

            tModel = ctx.deps.Where(x => x.depNo == "001").Count() == 0 ? null : ctx.deps.Where(x => x.depNo == "001").First();

            if (tModel==null)
            {
                tModel = new dep
                {
                    depNo = "001",
                    sort = 1
                };
                ctx.deps.Add(tModel);
            }

            tModel.depNm = Model.depNm;
            tModel.parentDepNo = "";
            tModel.depLevel = 0;

            ctx.SaveChanges();
            TempData["saveok"] = "Y";
            return View(tModel);
        }

    }
}