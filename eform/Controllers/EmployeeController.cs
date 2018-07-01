using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Dynamic;
using Newtonsoft.Json;

namespace eform.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index(string Id = "")
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
                    jobPo poObj = context.jobPos.Where(x => x.poNo == po.poNo).FirstOrDefault();
                    dep depObj = poObj==null?null:context.deps.Where(x => x.depNo == poObj.depNo).FirstOrDefault<dep>();
                    if (depObj != null)
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

                //if (manager.GetRoles(usr.Id).Where(x=>x=="Admin").Count()==0)
                {
                    model.Add(new vwEmployee
                    {
                        Id = usr.Id,
                        workNo = usr.workNo,
                        UserCName = usr.cName,
                        Title = string.Join(",", usrTitle.ToArray())
                    });
                }
                ViewBag.testList = usr.poList;
            }

            return View(model);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {

            ApplicationDbContext context = new ApplicationDbContext();
            var user = new ApplicationUser();


            vwEmployee model = new vwEmployee
            {
                Id = "",
                workNo = "",
                UserCName = "",
                Title="",
                Password="",
                rePassword=""
            };

            List<vwPoNo> poNoList = new List<vwPoNo>();
            ViewBag.currentPoList = poNoList;

            setViewBagRoles(context, null);
            setViewBagPo(context);

            ViewBag.Title = "員工資料新增";
            return View("Edit",model);
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            var user = new ApplicationUser
            {
                UserName = collection["workNo"].ToString(),
                workNo = collection["workNo"].ToString(),
                cName = collection["UserCName"].ToString()
            };

            try
            {
                var r = manager.Create(user, collection["Password"].ToString());
                string[] roles = collection["roles"].ToString().Split(',');
                var newUser = manager.FindByName(user.UserName);
                foreach (string role in roles)
                {
                    manager.AddToRole(newUser.Id, role);
                }
                return RedirectToAction("Index");
            }

            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                List<vwPoNo> poNoList = new List<vwPoNo>();
                ViewBag.currentPoList = poNoList;
                setViewBagRoles(context, null);
                setViewBagPo(context);
                return View();
            }

        }

        // GET: Employee/Edit/5
        public ActionResult Edit(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            var user = store.Users.Where(x => x.Id == id).FirstOrDefault();

            List<vwPoNo> poNoList = new List<vwPoNo>();
            List<string> usrTitle = new List<string>();

            context.Entry(user).Collection(x => x.poList).Load();

            foreach (var po in user.poList)
            {
                jobPo poObj = context.jobPos.Where(x => x.poNo == po.poNo).FirstOrDefault();
                dep depObj = poObj == null ? null : context.deps.Where(x => x.depNo == poObj.depNo).FirstOrDefault<dep>();
                if (depObj != null)
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

            ViewBag.currentPoList = poNoList;

            vwEmployee model = new vwEmployee
            {
                Id = user.Id,
                workNo = user.workNo,
                UserCName = user.cName,
                Title = string.Join(",", usrTitle.ToArray())
            };

            setViewBagRoles(context, user);

            setViewBagPo(context);

            

            ViewBag.Title = "員工資料編輯";
            return View(model);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, vwEmployee model)
        {
            var context = new ApplicationDbContext();
            ApplicationUser user = null;
            try
            {
                if (string.IsNullOrEmpty((string)Request.Form["roles"]))
                {
                    ModelState.AddModelError("", "需指定權限");
                }

                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        //create employee
                    }
                     user= context.Users.Where(x => x.Id == id).FirstOrDefault();
                    context.Entry(user).Collection(x => x.poList).Load();

                    if (!string.IsNullOrEmpty((string)Request.Form["hPoList"]))
                    {
                        List<string> poIdLst = ((string)Request.Form["hPoList"]).Split(',').ToList<string>();
                        if (user.poList == null)
                        {
                            user.poList = new List<PoUser>();
                        }
                        foreach (string poId in poIdLst)
                        {
                            PoUser po = user.poList.Where(x => x.poNo == poId).FirstOrDefault();
                            if (po == null)
                            {
                                user.poList.Add(new PoUser
                                {
                                    poNo=poId,
                                    UserId=user.workNo
                                });
                            }
                        }
                    }

                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new UserManager<ApplicationUser>(store);

                    List<string> roleList = ((string)Request.Form["roles"]).Split(',').ToList<string>();
                    foreach(string role in roleList)
                    {
                        if (!manager.IsInRole(user.Id,role))
                        {
                            manager.AddToRole(user.Id, role);
                        }
                    }

                    user.cName = model.UserCName;
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    setViewBagPo(context);
                    setViewBagRoles(context, user);
                    return View();
                }
            }
            catch(Exception ex)
            {
                setViewBagPo(context);
                setViewBagRoles(context, user);
                ModelState.AddModelError("",ex.Message);
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

        void setViewBagPo(ApplicationDbContext context)
        {
            List<dynamic> AllDep = new List<dynamic>();
            List<vwPoNo> AllPo = new List<vwPoNo>();
            foreach (var dep in context.deps)
            {
                dynamic depobj = new ExpandoObject();
                depobj.k = dep.depNo;
                depobj.v = dep.depNm;
                AllDep.Add(depobj);
            }

            foreach (var po in context.jobPos)
            {
                AllPo.Add(new vwPoNo
                {
                    depNo = po.depNo,
                    depNm = AllDep.Where(x => x.k == po.depNo).FirstOrDefault().v,
                    poNo = po.poNo,
                    poNm = po.poNm,
                });
            }
            ViewBag.allDep = AllDep;
            ViewBag.allPo = AllPo;
        }

        void setViewBagRoles(ApplicationDbContext context,ApplicationUser user)
        {
            List<vwRole> vwRoles = new List<vwRole>();

            var roleStore = new RoleStore<ApplicationRole>(context);

            var roleManager = new RoleManager<ApplicationRole>(roleStore);

            foreach (var role in roleManager.Roles)
            {

                vwRoles.Add(new vwRole
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    selected = (user != null && user.Roles.Where(x => x.RoleId == role.Id).Count() > 0)
                });
            }
            ViewBag.Roles = vwRoles;
        }
    }
}
