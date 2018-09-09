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
using Newtonsoft.Json.Linq;
using eform.Attributes;

namespace eform.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        [AdminAuthorize(Roles = "Admin")]
        public ActionResult Index(string Id = "")
        {

            ApplicationDbContext context = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            List<ApplicationUser> users = store.Users.Where(x => x.status == 1 && x.UserName.ToLower().Contains("sadmin") == false).OrderBy(x=>x.workNo).ToList<ApplicationUser>();
            List<vwEmployee> model = new List<vwEmployee>();

            foreach (var usr in users)
            {
                context.Entry(usr).Collection(x => x.poList).Load();

                List<vwPoNo> poNoList = new List<vwPoNo>();
                List<string> usrTitle = new List<string>();

                foreach (var po in usr.poList)
                {
                    jobPo poObj = context.jobPos.Where(x => x.poNo == po.poNo).FirstOrDefault();
                    dep depObj = poObj == null ? null : context.deps.Where(x => x.depNo == poObj.depNo).FirstOrDefault<dep>();
                    if (depObj != null)
                    {
                        string depNm = depObj.depNm;
                        dep depObj2= context.deps.Where(x => x.depNo == depObj.parentDepNo).FirstOrDefault<dep>();
                        if (depObj2 != null && depObj2.depNo !="001")
                        {
                            depNm = depObj2.depNm + "-" + depNm;
                            dep depObj3 = context.deps.Where(x => x.depNo == depObj2.parentDepNo).FirstOrDefault<dep>();
                            if (depObj3 != null && depObj3.depNo != "001")
                            {
                                depNm = depObj3.depNm + "-" + depNm;
                            }
                        }
                        poNoList.Add(new vwPoNo
                        {
                            poNo = poObj.poNo,
                            poNm = poObj.poNm,
                            depNo = poObj.depNo,
                            depNm = depObj.depNm
                        });
                        usrTitle.Add(depNm + "：" + poObj.poNm);
                    }
                }

                //if (manager.GetRoles(usr.Id).Where(x=>x=="Admin").Count()==0)
                {
                    model.Add(new vwEmployee
                    {
                        Id = usr.Id,
                        workNo = usr.workNo,
                        UserCName = usr.cName,
                        UserEName = usr.eName,
                        Title = string.Join(",", usrTitle.ToArray())
                    });
                }
                ViewBag.testList = usr.poList;
            }

            return View(model);
        }

        // GET: Employee/Create
        [AdminAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var user = new ApplicationUser();
            vwEmployee model = new vwEmployee
            {
                Id = "",
                workNo = "",
                UserCName = "",
                UserEName = "",
                Title = "",
                Password = "",
                rePassword = ""
            };

            List<vwPoNo> poNoList = new List<vwPoNo>();
            ViewBag.currentPoList = poNoList;

            setViewBagRoles(context, null);
            setViewBagPo(context);

            ViewBag.Title = "員工資料新增";
            ViewBag.EditMode = "Create";
            return View("Edit", model);
        }

        // POST: Employee/Create
        [HttpPost]
        [AdminAuthorize(Roles = "Admin")]
        public ActionResult Create(FormCollection collection)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            var user = new ApplicationUser
            {
                UserName = collection["workNo"].ToString(),
                workNo = collection["workNo"].ToString(),
                cName = collection["UserCName"].ToString(),
                eName = collection["UserEName"].ToString(),
                status = 1
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

            catch (Exception ex)
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
        [AdminAuthorize(Roles = "Admin")]
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
                UserEName = user.eName,
                Title = string.Join(",", usrTitle.ToArray())
            };

            setViewBagRoles(context, user);

            setViewBagPo(context);

            ViewBag.Title = "員工資料編輯";
            ViewBag.EditMode = "Edit";
            return View(model);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [AdminAuthorize(Roles = "Admin")]
        public ActionResult Edit(string id, vwEmployee model)
        {

            var context = new ApplicationDbContext();

            ViewBag.EditMode = "Edit";

            ApplicationUser user = null;

            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);

            if (string.IsNullOrEmpty(id))
            {
                ViewBag.EditMode = "Create";
                ViewBag.currentPoList = new List<vwPoNo>();
                if (string.IsNullOrEmpty((string)Request.Form["roles"]))
                {
                    ModelState.AddModelError("", "需指定權限");
                    setViewBagPo(context);
                    setViewBagRoles(context, null);
                    return View(model);
                }
                #region Create User

                var newuser = new ApplicationUser
                {
                    UserName = model.workNo,
                    workNo = model.workNo,
                    cName = model.UserCName,
                    eName = model.UserEName
                };

                try
                {
                    var r = manager.Create(newuser, model.Password);
                    if (!r.Succeeded)
                    {
                        ModelState.AddModelError("", string.Join(",", r.Errors.ToArray<string>()));
                        setViewBagPo(context);
                        setViewBagRoles(context, null);
                        return View(model);
                    }
                    var newUser = manager.FindByName(newuser.UserName);
                    id = newuser.Id;
                    user = newUser;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    setViewBagRoles(context, null);
                    setViewBagPo(context);
                    return View(model);
                }

                #endregion
            }

            try
            {
                if (ModelState.IsValid)
                {
                    user = context.Users.Where(x => x.Id == id).FirstOrDefault();

                    if (string.IsNullOrEmpty((string)Request.Form["roles"]))
                    {
                        ModelState.AddModelError("", "需指定權限");
                        setViewBagPo(context);
                        setViewBagRoles(context, user);
                        return View(model);
                    }
                    context.Entry(user).Collection(x => x.poList).Load();

                    if (!string.IsNullOrEmpty((string)Request.Form["hPoList"]))
                    {
                        List<string> poIdLst = ((string)Request.Form["hPoList"]).Split(',').ToList<string>();
                        if (user.poList == null)
                        {
                            user.poList = new List<PoUser>();
                        }
                        else
                        {
                            user.poList.Clear();
                        }
                        foreach (string poId in poIdLst)
                        {
                            PoUser po = user.poList.Where(x => x.poNo == poId).FirstOrDefault();
                            if (po == null)
                            {
                                user.poList.Add(new PoUser
                                {
                                    poNo = poId,
                                    UserId = user.workNo
                                });
                            }
                        }
                    }
                    else
                    {
                        user.poList.Clear();
                    }

                    List<string> roleList = ((string)Request.Form["roles"]).Split(',').ToList<string>();
                    foreach (string role in roleList)
                    {
                        if (!manager.IsInRole(user.Id, role))
                        {
                            manager.AddToRole(user.Id, role);
                        }
                    }

                    user.cName = model.UserCName;
                    user.eName = model.UserEName;
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
            catch (Exception ex)
            {
                setViewBagPo(context);
                setViewBagRoles(context, user);
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Employee/Delete/5
        [AdminAuthorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            dbHelper dbh = new dbHelper();
            dbh.execSql("update dbo.AspNetUsers set status=0,workNo=workNo+'-remove' where Id='" + id + "'");

            return RedirectToAction("Index");
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [AdminAuthorize(Roles = "Admin")]
        public ActionResult Delete(string id, FormCollection collection)
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

        [AdminAuthorize(Roles = "Admin")]
        public ActionResult ModifyPassword(string id)
        {
            return View();
        }

        public ActionResult Search(string q)
        {
            List<dynamic> list = new List<dynamic>();

            var context = new ApplicationDbContext();

            var userlist = context.Users.Where(x => x.status == 1 && x.workNo.Contains(q) || x.cName.Contains(q)).ToList<ApplicationUser>();

            foreach (var user in userlist)
            {
                dynamic obj = new ExpandoObject();
                obj.id = user.workNo;
                obj.text = user.cName;
                list.Add(obj);
            }
            dynamic r = new ExpandoObject();
            r.results = list;
            return Content(JsonConvert.SerializeObject(r), "application/json");

            //return new JsonResult
            //{
            //    Data = r,
            //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //};
        }

        [AdminAuthorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult ChangePassword(string id)
        {
            var context = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            ApplicationUserManager mgr = new ApplicationUserManager(store);
            var user = mgr.FindById(id);
            ViewBag.UserText = user.workNo + " " + user.cName;
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                id = id
            };
            return View(model);
        }

        [AdminAuthorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                string id = model.id;
                var context = new ApplicationDbContext();
                var store = new UserStore<ApplicationUser>(context);
                ApplicationUserManager mgr = new ApplicationUserManager(store);
                var user = mgr.FindById(id);
                mgr.RemovePassword(id);
                mgr.AddPassword(id, model.NewPassword);
                TempData["changepwdsucess"] = "Y";
                return RedirectToAction("Edit", new { id = id });
            }
        }

        [AdminAuthorize(Roles = "Admin,Employee")]
        [HttpGet]
        public ActionResult ChangeMyPassword()
        {
            var context = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            ApplicationUserManager mgr = new ApplicationUserManager(store);
            string id = User.Identity.GetUserId();
            var user = mgr.FindById(id);
            ViewBag.UserText = user.workNo + " " + user.cName;
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                id = id
            };
            return View(model);
        }

        [AdminAuthorize(Roles = "Admin,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeMyPassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                string id = model.id;
                var context = new ApplicationDbContext();
                var store = new UserStore<ApplicationUser>(context);
                ApplicationUserManager mgr = new ApplicationUserManager(store);
                var user = mgr.FindById(id);
                mgr.RemovePassword(id);
                mgr.AddPassword(id, model.NewPassword);
                TempData["changepwdsucess"] = "Y";
                return RedirectToAction("Index", "Home");
            }
        }


        void setViewBagPo(ApplicationDbContext context)
        {
            List<dynamic> AllDep = new List<dynamic>();
            List<vwPoNo> AllPo = new List<vwPoNo>();
            foreach (var dep in context.deps)
            {
                dynamic depobj = new ExpandoObject();
                depobj.depLevel = dep.depLevel;
                depobj.parentDepNo = dep.parentDepNo;
                depobj.k = dep.depNo;
                depobj.v = dep.depNm;
                AllDep.Add(depobj);
            }

            string[] depAry = (from depitem in context.deps select depitem.depNo).ToArray<string>();
            var listJobPos = from item in context.jobPos where depAry.Contains(item.depNo) select item;

            foreach (var po in listJobPos)
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

        void setViewBagRoles(ApplicationDbContext context, ApplicationUser user)
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
