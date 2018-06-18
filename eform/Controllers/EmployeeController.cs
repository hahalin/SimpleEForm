using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web.Security;

namespace eform.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            List<ApplicationUser> users = store.Users.ToList<ApplicationUser>();
            List<vwEmployee> model = new List<vwEmployee>();

            

            foreach (var usr in users)
            {
                context.Entry(usr).Collection(x => x.poList).Load();

                List<vwPoNo> poNoList = new List<vwPoNo>();
                List<string> usrTitle = new List<string>();

                foreach (var po in usr.poList)
                {
                    dep depObj = context.deps.Where(x => x.depNo == po.depNo).FirstOrDefault<dep>();
                    if (depObj != null)
                    {
                        jobPo poObj = context.jobPos.Where(x => x.poNo == po.poNo).FirstOrDefault<jobPo>();
                        if (poObj !=null)
                        {
                            poNoList.Add(new vwPoNo
                            {
                                poNo = poObj.poNo,
                                poNm = poObj.poNm,
                                depNo = poObj.depNo,
                                depNm = depObj.depNm
                            });
                            usrTitle.Add(depObj.depNm + "-" + poObj.poNm);
                        }
                    }
                }

                //if (manager.GetRoles(usr.Id).Where(x=>x=="Admin").Count()==0)
                {
                    model.Add(new vwEmployee
                    {
                        Id = usr.Id,
                        workNo = usr.workNo,
                        UserCName=usr.cName,
                        Title = string.Join(",", usrTitle.ToArray())
                    });
                }
                ViewBag.testList = usr.poList;
            }

            return View(model);
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
