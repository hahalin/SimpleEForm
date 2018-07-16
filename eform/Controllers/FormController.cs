﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Attributes;
using eform.Models;
namespace eform.Controllers
{
    [AdminAuthorize(Roles = "Admin,Employee")]
    public class FormController : Controller
    {
        // GET: Form
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();
            List<vwFlowMain> list = new List<vwFlowMain>();

            var sender = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            List<FlowMain> flist = context.FlowMainList.Where(x => x.senderNo == sender.workNo).OrderByDescending(x => x.billDate).ToList<FlowMain>();

            foreach (FlowMain fitem in flist)
            {
                var Status = context.flowStatusList().Where(x => x.id == fitem.flowStatus).FirstOrDefault();
                vwFlowMain item = new vwFlowMain
                {
                    id = fitem.id,
                    sender = sender.workNo + " " + sender.cName,
                    billDate = fitem.billDate,
                    flowName = fitem.flowName,
                    flowStatus = Status == null ? "簽核中" : Status.nm
                };
                list.Add(item);
            }

            return View(list);
        }

        // GET: Form/Details/5
        public ActionResult Details(string id)
        {
            var context = new ApplicationDbContext();
            List<vwFlowSub> list = new List<vwFlowSub>();
            List<FlowSub> fsubList = context.FlowSubList.Where(x => x.pid == id).OrderBy(x => x.seq).ToList<FlowSub>();
            foreach (FlowSub sitem in fsubList)
            {
                var signResultObj = context.signResultList().Where(x => x.id == sitem.signResult).FirstOrDefault();
                var signTypeObj = context.signTypeList().Where(x => x.id == sitem.signType).FirstOrDefault();
                var signUser = context.Users.Where(x => x.workNo == sitem.workNo).FirstOrDefault();
                vwFlowSub item = new vwFlowSub
                {
                    id = sitem.id,
                    seq = sitem.seq,
                    signDate = sitem.signDate,
                    signResult = signResultObj == null ? "" : signResultObj.nm,
                    signType = signTypeObj == null ? "會簽" : signTypeObj.nm,
                    signer = signUser == null ? "" : signUser.cName
                };
                list.Add(item);
            }

            FlowMain fmain = context.FlowMainList.Where(x => x.id == id).FirstOrDefault();
            ViewBag.flowMain = fmain;

            ReqOverTime reqOverTimeObj = context.reqOverTimeList.Where(x => x.flowId == id).FirstOrDefault();
            vwReqOverTime vwReqOverTimeObj = null;
            if (reqOverTimeObj != null)
            {
                vwReqOverTimeObj = new vwReqOverTime
                {
                    dtBegin = reqOverTimeObj.dtBegin,
                    dtEnd = reqOverTimeObj.dtEnd,
                    hours = reqOverTimeObj.hours,
                    sMemo = reqOverTimeObj.sMemo
                };
            }
            ViewBag.SubModel = vwReqOverTimeObj;
            return View(list);
        }

        // GET: Form/Create
        public ActionResult CreateOverTimeForm()
        {
            return View();
        }

        // POST: Form/CreateOverTimeForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOverTimeForm(vwReqOverTime Model)
        {
            if (Model.dtEnd.HasValue && Model.dtBegin.HasValue)
            {
                if (DateTime.Compare(Convert.ToDateTime(Model.dtBegin), Convert.ToDateTime(Model.dtEnd)) > 0)
                {
                    ModelState.AddModelError("", "開始時間不得晚於結束時間");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            var context = new ApplicationDbContext();
            string FlowDefKey = "OverTime";
            FlowMain fmain = new FlowMain();
            List<FlowSub> fsublist = new List<FlowSub>();
            FlowDefMain fDefMain = context.FlowDefMainList.Where(x => x.enm == FlowDefKey).FirstOrDefault();
            List<FlowDefSub> fDefSubList = new List<FlowDefSub>();
            if (fDefMain != null)
            {
                fDefSubList = context.FlowDefSubList.Where(x => x.pid == fDefMain.id).OrderBy(x => x.seq).ToList<FlowDefSub>();
            }

            var sender = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            fmain.id = Guid.NewGuid().ToString();
            fmain.defId = "OverTime";
            fmain.flowName = "加班單";
            fmain.flowStatus = 1;
            fmain.senderNo = sender.workNo;
            fmain.billDate = Model.billDate;
            context.FlowMainList.Add(fmain);

            ReqOverTime fmOverTime = new ReqOverTime
            {
                dtBegin = Model.dtBegin,
                dtEnd = Model.dtEnd,
                hours = Model.hours,
                sMemo = Model.sMemo,
                id = Guid.NewGuid().ToString(),
                flowId = fmain.id
            };

            context.reqOverTimeList.Add(fmOverTime);

            int flowSeq = 1;

            dbHelper dbh = new dbHelper();
            string senderPoNo = dbh.sql2Str("select poNo from PoUsers where UserId='" + sender.workNo + "'");
            string senderMgrNo = "";
            if (!string.IsNullOrEmpty(senderPoNo))
            {
                string senderDepNo = dbh.sql2Str("select depNo from jobPoes where poNo='" + senderPoNo + "'");
                if (!string.IsNullOrEmpty(senderDepNo))
                {
                    var mgrPo = context.jobPos.Where(x => x.isFormSigner == true && x.depNo == senderDepNo).FirstOrDefault();
                    senderMgrNo = dbh.sql2Str("select top 1 UserId from PoUsers where poNo='" + mgrPo.poNo + "' and UserId <> '" + sender.workNo + "'");
                }
            }

            if (!string.IsNullOrEmpty(senderMgrNo))
            {
                FlowSub fsub = new FlowSub();
                fsub.pid = fmain.id;
                fsub.id = Guid.NewGuid().ToString();
                fsub.seq = flowSeq;
                fsub.workNo = senderMgrNo;
                fsub.signType = 1;
                fsub.signResult = 0;
                flowSeq++;
                context.FlowSubList.Add(fsub);
            }

            foreach (FlowDefSub defItem in fDefSubList)
            {
                if (defItem.workNo != senderMgrNo)
                {
                    FlowSub fsub = new FlowSub();
                    fsub.pid = fmain.id;
                    fsub.id = Guid.NewGuid().ToString();
                    fsub.seq = flowSeq;
                    fsub.workNo = defItem.workNo;
                    fsub.signType = defItem.signType;
                    fsub.signResult = 0;
                    flowSeq++;
                    context.FlowSubList.Add(fsub);
                }
            }

            try
            {
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(Model);
            }
        }

        // GET: Form/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Form/Edit/5
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

        // GET: Form/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Form/Delete/5
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
