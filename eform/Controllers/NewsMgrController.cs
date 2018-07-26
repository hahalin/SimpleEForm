using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using eform.Attributes;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NewsMgrController : Controller
    {
        void seedingNewsTypeList(ApplicationDbContext context)
        {
            if (context.newsTypeList.Count() == 0)
            {
                context.newsTypeList.Add(new newsType { seq = 1, code = "9000-營運處" });
                context.newsTypeList.Add(new newsType { seq = 2, code = "1000-測試設備製造處" });
                context.newsTypeList.Add(new newsType { seq = 3, code = "1200-測試設備開發處" });
                context.newsTypeList.Add(new newsType { seq = 4, code = "2000-燃料電池材料業務處" });
                context.newsTypeList.Add(new newsType { seq = 5, code = "3000-電子計量產品業務處" });
                context.newsTypeList.Add(new newsType { seq = 6, code = "4000-自動化設備處" });
                context.newsTypeList.Add(new newsType { seq = 7, code = "5000-特用碳材料業務處" });
                context.newsTypeList.Add(new newsType { seq = 8, code = "5100-上海群羿碳材料業務處" });
                context.newsTypeList.Add(new newsType { seq = 9, code = "6000-開發處-亞氫" });
                context.newsTypeList.Add(new newsType { seq = 10, code = "7000-Yanmar 業務暨工程處" });
                context.SaveChanges();
            }
        }

        // GET: NewsMgr
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();
            seedingNewsTypeList(context);
            var qry = from item in context.newsList.ToList<news>() orderby item.createTime2 descending select item;
            var model = qry.ToList<news>();
            return View(model);
        }

        void initNewsTypeList(string value,ApplicationDbContext context=null)
        {
            if(context==null)
            {
                context = new ApplicationDbContext();
            }
            List<SelectListItem> newsTypeItems = new List<SelectListItem>();

            newsTypeItems.Add(new SelectListItem
            {
                Value = "",
                Text = "請選擇類別",
                Selected=true
            });

            foreach (var typeItem in context.newsTypeList.OrderBy(x => x.seq).ToList<newsType>())
            {
                newsTypeItems.Add(new SelectListItem
                {
                    Value = typeItem.code,
                    Text = typeItem.code,
                });
            }

            foreach(var item in newsTypeItems)
            {
                if (item.Value==value)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            ViewBag.newsTypeItems = newsTypeItems;

        }

        [HttpGet]
        public ActionResult Create()
        {
            var context = new ApplicationDbContext();
            var model = new news();
            var store = new UserStore<ApplicationUser>(context);
            var mgr = new ApplicationUserManager(store);
            var user = mgr.FindByName(User.Identity.Name);
            if (user != null)
            {
                model.uid = user.Id;
            }

            initNewsTypeList("", context);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(news model)
        {
            if (string.IsNullOrEmpty(model.newsType))
            {
                initNewsTypeList("");
                ModelState.AddModelError("newsType", "請選擇公告類別");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                initNewsTypeList(model.newsType);
                return View(model);
            }

            var context = new ApplicationDbContext();

            string ModelID = "";

            if (string.IsNullOrEmpty(model.id))
            {
                model.id = Guid.NewGuid().ToString();
                ModelID = model.id;
                model.createTime2 = context.getLocalTiime();
                //if (fileUrl != "")
                //{
                //    model.fileUrl = fileUrl;
                //}
                context.newsList.Add(model);
            }
            else
            {
                news currentModel = context.newsList.Where(x => x.id == model.id).FirstOrDefault();
                ModelID = currentModel.id;
                currentModel.title = model.title;
                currentModel.createTime = DateTime.UtcNow.AddSeconds(28800);
                model.createTime2 = context.getLocalTiime();
                currentModel.content = model.content;
                currentModel.ndate = model.ndate;
                currentModel.newsType = model.newsType;
                //if (fileUrl != "")
                //{
                //    currentModel.fileUrl = fileUrl;
                //}
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            dbHelper dbh = new dbHelper();

            string sfilelist = dbh.sql2Str("select fileUrl from news where id='" + ModelID + "'");
            JArray jlist = new JArray();

            try
            {
                jlist = JArray.Parse(sfilelist);
            }
            catch(Exception ex)
            {
                jlist = new JArray();
            }

            
            for (int idx = 0; idx < Request.Files.Count; idx++)
            {
                var file = Request.Files[idx];
                if (file==null)
                {
                    continue;
                }
                if (file.ContentLength > 0)
                {
                    string originFileName = file.FileName;
                    string filename = file.FileName.Replace("%", "").Replace(" ", "_");
                    string fileUrl = ModelID + @"/" + filename;

                    var path = @"C:\inetpub\upload\" + ModelID + @"\";
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists == false)
                    {
                        di.Create();
                    }
                    path = path + @"\" + filename;
                    try
                    {
                        file.SaveAs(path);
                        JObject fileObj = new JObject();
                        fileObj["fileName"] = originFileName;
                        fileObj["fileUrl"] = ModelID + @"\"+filename ;
                        jlist.Add(fileObj);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }

            dbh.execSql("update news set fileUrl=N'" + jlist.ToString() + "' where id='" + ModelID +"'");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var context = new ApplicationDbContext();
            news model = context.newsList.Where(x => x.id == id).FirstOrDefault();
            initNewsTypeList(model.newsType, context);
            return View("Create", model);
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            dbHelper dbh = new dbHelper();

            string filepath = dbh.sql2Str("select fileUrl from news where id='" + id + "'");

            try
            {
                DirectoryInfo di = new DirectoryInfo(@"C:\inetpub\upload\" + id);
                while(di.GetFiles().Count()>0)
                {
                    di.GetFiles()[0].Delete();
                }
                di.Delete();
            }
            catch(Exception ex)
            {
                string exs = ex.Message;
            }

            dbh.execSql("delete from news where id='" + id + "'");

            return RedirectToAction("Index");
        }
    }
}