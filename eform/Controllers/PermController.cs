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

    public class PermController : Controller
    {
        [AdminAuthorize(Roles = "Admin")]
        public ActionResult Index(string mod = "")
        {
            //ReportSalary

            ApplicationDbContext ctx = new ApplicationDbContext();

            List<string> userList = ctx.permList.Where(x => x.mod == mod).Select(x => x.workNo).ToList<string>();

            List<ApplicationUser> permUserList = (from user in ctx.Users where userList.Contains(user.workNo) select user).ToList<ApplicationUser>();

            ViewBag.Title = "權限管理";
            ViewBag.Mod = mod;
            if (mod == "ReportSalary")
            {
                ViewBag.Title = "薪資查詢特別權限";
            }
            if (mod == "ReportUpload")
            {
                ViewBag.Title = "薪資檔上傳權限";
            }
            return View(permUserList);
        }

        [AdminAuthorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create(string mod)
        {
            permission model = new permission();
            model.mod = mod;
            if (mod=="ReportSalary")
            {
                ViewBag.Title = "薪資查詢特別權限-新增";
            }
            if (mod == "ReportUpload")
            {
                ViewBag.Title = "薪資檔上傳權限-新增";
            }
            return View(model);
        }

        [AdminAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(permission model)
        {
            if (string.IsNullOrEmpty(model.workNo))
            {
                ModelState.AddModelError("workNo", "請輸入工號");
            }

            var ctx = new ApplicationDbContext();
            if (ctx.Users.Where(x => x.workNo == model.workNo && x.status==1).Count() == 0)
            {
                ModelState.AddModelError("workNo", "無此工號");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            if (ctx.permList.Where(x => x.mod == model.mod && x.workNo == model.workNo).Count() == 0)
            {
                model.id = Guid.NewGuid().ToString();
                ctx.permList.Add(model);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index", new { mod = model.mod });
        }

        [AdminAuthorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Delete(string workNo,string mod)
        {
            var ctx = new ApplicationDbContext();
            var list = ctx.permList.Where(x => x.workNo == workNo && x.mod == mod).ToList<permission>();
            ctx.permList.RemoveRange(list);
            ctx.SaveChanges();
            return RedirectToAction("Index","Perm", new { mod = mod });
        }
    }
}