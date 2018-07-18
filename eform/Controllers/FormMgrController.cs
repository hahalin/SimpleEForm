using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Attributes;
using eform.Models;
using eform.Attributes;

namespace eform.Controllers
{
    public class FormMgrController : Controller
    {
        // GET: FlowMgr
        [AdminAuthorize(Roles = "Admin,Employee")]
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();

            var user = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            var unSignMainList = from item in context.FlowMainList
                                 where item.flowStatus == 1
                                 select item.id;

            var mySignSubList = (from item in context.FlowSubList
                                 where item.workNo == user.workNo
                                 && unSignMainList.Contains(item.pid)
                                 select item.pid).ToList<string>();

            var mySignMainList = context.FlowMainList.Where(x => mySignSubList.Contains(x.id)).ToList<FlowMain>();

            List<vwFlowMain> list = new List<vwFlowMain>();

            foreach (FlowMain item in mySignMainList)
            {
                var Status = context.flowStatusList().Where(x => x.id == item.flowStatus).FirstOrDefault();
                var sender = context.Users.Where(x => x.workNo.Equals(item.senderNo)).FirstOrDefault();
                vwFlowMain vwItem = new vwFlowMain
                {
                    id = item.id,
                    sender = sender.workNo + " " + sender.cName,
                    billDate = item.billDate,
                    flowName = item.flowName,
                    flowStatus = Status == null ? "簽核中" : Status.nm
                };
                list.Add(vwItem);
            }

            return View(list);
        }
        [AdminAuthorize(Roles = "Admin,Employee")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(Roles = "Admin,Employee")]
        public ActionResult SignIt(FormCollection collection)
        {
            var context = new ApplicationDbContext();
            string id = collection["hid"].ToString();
            string signValue = collection["signValue"].ToString();
            string signMemo = collection["signMemo"].ToString();
            string workNo = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().workNo;

            FlowSub fsub = context.FlowSubList.Where(x => x.pid == id && x.workNo == workNo).FirstOrDefault();
            if (fsub !=null)
            {
                fsub.signDate = context.getLocalTiime();
                fsub.signResult = Convert.ToInt16(signValue);
                fsub.comment = signMemo;

                if (fsub.signResult==1)
                {
                    dbHelper dbh = new dbHelper();
                    dbh.execSql("update FlowMains set flowStatus=2 where id='" + id + "'");
                }
            }

            return RedirectToAction("Query", "FlowMgr");
        }

        [AdminAuthorize(Roles = "Admin,Employee")]
        public ActionResult Query()
        {
            var context = new ApplicationDbContext();

            var user = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            var unSignMainList = from item in context.FlowMainList
                                 where item.flowStatus != 1
                                 select item.id;

            var mySignSubList = (from item in context.FlowSubList
                                 where item.workNo == user.workNo
                                 && unSignMainList.Contains(item.pid)
                                 select item.pid).ToList<string>();

            var mySignMainList = context.FlowMainList.Where(x => mySignSubList.Contains(x.id)).ToList<FlowMain>();

            List<vwFlowMain> list = new List<vwFlowMain>();

            foreach (FlowMain item in mySignMainList)
            {
                var Status = context.flowStatusList().Where(x => x.id == item.flowStatus).FirstOrDefault();
                var sender = context.Users.Where(x => x.workNo.Equals(item.senderNo)).FirstOrDefault();
                vwFlowMain vwItem = new vwFlowMain
                {
                    id = item.id,
                    sender = sender.workNo + " " + sender.cName,
                    billDate = item.billDate,
                    flowName = item.flowName,
                    flowStatus = Status == null ? "簽核中" : Status.nm
                };
                list.Add(vwItem);
            }

            return View(list);
        }
    }
}