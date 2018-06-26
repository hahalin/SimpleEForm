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
                    dep depObj = context.deps.Where(x => x.depNo == po.depNo).FirstOrDefault<dep>();
                    if (depObj != null)
                    {
                        jobPo poObj = context.jobPos.Where(x => x.poNo == po.poNo).FirstOrDefault<jobPo>();
                        if (poObj != null)
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

            var roleStore = new RoleStore<ApplicationRole>(context);

            var roleManager = new RoleManager<ApplicationRole>(roleStore);

            List<vwRole> vwRoles = new List<vwRole>();

            foreach (var role in roleManager.Roles)
            {
                vwRoles.Add(new vwRole
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    selected = false
                });
            }
            
            ViewBag.Roles = vwRoles;

            List<dynamic> AllDep = new List<dynamic>();
            List<vwPoNo> AllPo = new List<vwPoNo>();
            List<vwPoNo> poNoList = new List<vwPoNo>();

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

            ViewBag.currentPoList = poNoList;
            ViewBag.allDep = AllDep;
            ViewBag.allPo = AllPo;
            return View(model);
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

            List<vwPoNo> poNoList = new List<vwPoNo>();

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
                    selected = false
                });
            }

            List<dynamic> AllDep = new List<dynamic>();
            List<vwPoNo> AllPo = new List<vwPoNo>();
            foreach (var dep in context.deps)
            {
                dynamic depobj = new ExpandoObject();
                depobj.k = dep.depNo;
                depobj.v = dep.depNm;
                AllDep.Add(depobj);
            }

            //foreach (var po in context.jobPos)
            //{
            //    AllPo.Add(new vwPoNo
            //    {
            //        depNo = po.depNo,
            //        depNm = AllDep.Where(x => x.k == po.depNo).FirstOrDefault().v,
            //        poNo = po.poNo,
            //        poNm = po.poNm,
            //    });
            //}

            ViewBag.Roles = vwRoles;
            ViewBag.allDep = AllDep;
            ViewBag.allPo = AllPo;
            ViewBag.currentPoList = poNoList;

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
                return View();
            }
        }
        
        void setViewBagRoles(ApplicationDbContext context)
        {
            
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
                dep depObj = context.deps.Where(x => x.depNo == po.depNo).FirstOrDefault<dep>();
                if (depObj != null)
                {
                    jobPo poObj = context.jobPos.Where(x => x.poNo == po.poNo).FirstOrDefault<jobPo>();
                    if (poObj != null)
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

            vwEmployee model = new vwEmployee
            {
                Id = user.Id,
                workNo = user.workNo,
                UserCName = user.cName,
                Title = string.Join(",", usrTitle.ToArray())
            };

            var roleStore = new RoleStore<ApplicationRole>(context);

            var roleManager = new RoleManager<ApplicationRole>(roleStore);

            List<vwRole> vwRoles = new List<vwRole>();

            foreach (var role in roleManager.Roles)
            {
                vwRoles.Add(new vwRole
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    selected = user.Roles.Where(x => x.RoleId == role.Id).Count() > 0
                });
            }

            ViewBag.Roles = vwRoles;

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

            ViewBag.currentPoList = poNoList;
            ViewBag.allDep = AllDep;
            ViewBag.allPo = AllPo;
            return View(model);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, vwEmployee model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var context = new ApplicationDbContext();
                    var user = context.Users.Where(x => x.Id == id).FirstOrDefault();
                    user.cName = model.UserCName;
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
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
