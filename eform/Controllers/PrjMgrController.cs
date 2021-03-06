﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Mvc;
using eform.Models;
using eform.Repo;
using eform.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using Dapper;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using System.Text;
using eform.Service;
using System.Web.Routing;


namespace eform.Controllers
{
    [Authorize]
    public sealed partial class PrjMgrController : Controller
    {
        prjRep rep;
        ApplicationDbContext ctx;
        SqlConnection con;

        public PrjMgrController()
        {
            ctx = new ApplicationDbContext();
            string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            con = new SqlConnection(constring);
        }

        public ActionResult PmPrjList()
        {
            var prjIdList = ctx.prjPMList.Where(x => x.WorkNo == User.Identity.Name && x.Title == "PM").Select(x => x.pid);

            prjIdList= prjIdList.Concat(ctx.prjCodeList.Where(x => x.owner == User.Identity.Name).Select(x => x.id));

            List<vwPrjCode> prjList = new List<vwPrjCode>();
            var pmPrjs = ctx.prjCodeList.Where(x => prjIdList.Contains(x.id));
            foreach (prjCode prj in pmPrjs)
            {
                vwPrjCode p = new vwPrjCode
                {
                    id = prj.id,
                    code = prj.code,
                    nm = prj.nm,
                    owner = prj.owner,
                    createDate = prj.createDate,
                    contractDate = prj.contractDate
                };
                prjList.Add(p);
            }

            prjList = prjList.OrderByDescending(x => Convert.ToDateTime(x.contractDate)).ToList<vwPrjCode>();

            return View(prjList);
        }


        public ActionResult ItemList(string prjId)
        {

            vwSchItemList model = new vwSchItemList();

            model.prj = ctx.prjCodeList.Where(x => x.id == prjId).FirstOrDefault();

            List<scheduleItem> list = ctx.scheduleItems.Where(x => x.prjId == prjId).OrderBy(x => x.seq).ToList<scheduleItem>();

            foreach (var item in list)
            {
                model.itemList.Add(new vwScheduleItem
                {
                    prjId = item.prjId,
                    id = item.id,
                    seq = item.seq,
                    itemTxt = item.itemTxt,
                    dtBegin = item.dtBegin,
                    dtEnd = item.dtEnd
                });
            }


            return View(model);
        }

        [HttpGet]
        public ActionResult CreateSchItem(string prjId)
        {
            vwScheduleItem model = new vwScheduleItem
            {
                prjId = prjId
            };
            ViewBag.prjId = prjId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSchItem(vwScheduleItem model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ctx.scheduleItems.Add(new scheduleItem
            {
                id = Guid.NewGuid().ToString(),
                seq = model.seq,
                prjId=model.prjId,
                itemTxt=model.itemTxt,
                dtBegin=model.dtBegin,
                dtEnd=model.dtEnd
            });

            ctx.SaveChanges();

            return RedirectToAction("ItemList", new { prjId = model.prjId });

        }
        [HttpGet]
        public ActionResult ImportTemplate(string prjId,string tId)
        {
            var items = ctx.schTempItemList.Where(x => x.pid == tId);
            int maxSeq = 1;
            try
            { 
                var vMaxSeq = ctx.scheduleItems.Where(x => x.prjId == prjId).Max(x => x.seq);
                maxSeq = (int)vMaxSeq + 1;
            }
            catch(Exception exGetMaxSeq)
            {
                maxSeq = 1;
            }


            foreach (var i in items)
            {
                ctx.scheduleItems.Add(new scheduleItem
                {
                    id = Guid.NewGuid().ToString(),
                    seq = maxSeq++,
                    prjId=prjId,
                    itemTxt=i.txt,
                    dtBegin=null,
                    dtEnd=null
                });
            }

            ctx.SaveChanges();

            return RedirectToAction("ItemList", new { prjId = prjId });
        }

        [HttpGet]
        public ActionResult SelTemplate(string prjId)
        {
            var temps = ctx.schTempList.OrderBy(x => x.code).ToList<schTemp>();

            ViewBag.prjId = prjId;

            return View(temps);
        }

        [HttpGet]
        public ActionResult EditSchItem(string prjId,string itemId)
        {
            var item = ctx.scheduleItems.Where(x => x.id == itemId).FirstOrDefault();

            var model = new vwScheduleItem
            {
                id=item.id,
                prjId=item.prjId,
                seq=item.seq,
                itemTxt=item.itemTxt,
                dtBegin=item.dtBegin,
                dtEnd=item.dtEnd
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult EditSchItem(vwScheduleItem model)
        {
            var item = ctx.scheduleItems.Where(x => x.id == model.id).FirstOrDefault();

            item.seq = model.seq;
            item.itemTxt = model.itemTxt;
            item.dtBegin = model.dtBegin;
            item.dtEnd = model.dtEnd;

            ctx.SaveChanges();
            return RedirectToAction("ItemList", new { prjId = model.prjId });
        }

        [HttpGet]
        public ActionResult RemoveSchItem(string prjId,string itemId)
        {
            ctx.scheduleItems.Remove(ctx.scheduleItems.Where(x => x.id == itemId).FirstOrDefault());
            ctx.SaveChanges();
            return RedirectToAction("ItemList", new { prjId = prjId });
        }


        public ActionResult Index()
        {
            return View();
        }
    }
}