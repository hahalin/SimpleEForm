using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Attributes;
using eform.Models;
using eform.Attributes;
using Newtonsoft.Json.Linq;

namespace eform.Controllers
{
    public class FormMgrController : Controller
    {
        ApplicationDbContext ctx;

        public FormMgrController()
        {
            ctx = new ApplicationDbContext();
        }
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
                                 where item.workNo == user.workNo && item.signResult==0
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
        [HttpPost]
        public ActionResult Delete(FormCollection formCollection)
        {
            try
            {
                string[] ids = formCollection["ID"].Split(new char[] { ',' });
                if (ids.Count() == 0)
                {
                    return RedirectToAction("ListAll");
                }
                List<string> idList = new List<string>();
                foreach (string s in ids)
                {
                    idList.Add("'" + s + "'");
                }
                string instr = string.Join(",", idList);
                dbHelper dbh = new dbHelper();
                dbh.execSql("update flowmains set flowStatus=99 where id in (" + instr + ")");
                return RedirectToAction("ListAll");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ListAll");
            }
        }

        [AdminAuthorize(Roles = "Admin,Employee")]
        public ActionResult Details(string id, string FlowPageType = "")
        {
            var context = new ApplicationDbContext();
            ViewBag.FlowPageType = FlowPageType;

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
                    signer = signUser == null ? "" : signUser.workNo + "-" + signUser.cName,
                    comment = sitem.comment
                };
                list.Add(item);
            }

            FlowMain fmain = context.FlowMainList.Where(x => x.id == id).FirstOrDefault();
            ViewBag.flowMain = fmain;
            vwEmployee employee = ctx.getUserByWorkNo(fmain.senderNo);
            ViewBag.Employee = employee;

            #region "加班預填單"
            if (fmain.defId == "OverTime")
            {
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
                    JObject jobjext = null;
                    try
                    {
                        jobjext = JObject.Parse(reqOverTimeObj.jext);
                        vwReqOverTimeObj.sType = jobjext["stype"] == null ? "" : jobjext["stype"].ToString();
                        vwReqOverTimeObj.place = jobjext["place"] == null ? "" : jobjext["place"].ToString();
                        vwReqOverTimeObj.otherPlace = jobjext["otherPlace"] == null ? "" : jobjext["otherPlace"].ToString();
                        vwReqOverTimeObj.prjId = jobjext["prjId"] == null ? "" : jobjext["prjId"].ToString();

                        vwReqOverTimeObj.sMemo2 = jobjext["sMemo2"] == null ? "" : jobjext["sMemo2"].ToString();
                        vwReqOverTimeObj.worker = jobjext["worker"] == null ? "" : jobjext["worker"].ToString();
                        vwReqOverTimeObj.depNo = jobjext["depNo"] == null ? "" : jobjext["depNo"].ToString();

                        vwReqOverTimeObj.poNo = jobjext["poNo"] == null ? "" : jobjext["poNo"].ToString();
                        vwReqOverTimeObj.poNm = jobjext["poNo"] == null ? "" : context.jobPos.Where(x => x.poNo.Equals(vwReqOverTimeObj.poNo)).FirstOrDefault().poNm;

                        jobPo poObj = context.jobPos.Where(x => x.poNo == vwReqOverTimeObj.poNo).First();
                        string depNo = poObj.depNo;
                        string depNm = context.getParentDeps(depNo, context) + " : " + poObj.poNm;
                        vwReqOverTimeObj.depNm = depNm;
                    }
                    catch (Exception ex)
                    {
                    }
                }
                ViewBag.SubModel = vwReqOverTimeObj;
                return View(list);
            }
            #endregion

            #region "請假單"
            if (fmain.defId == "DayOff")
            {
                vwDayOffForm subModel = new vwDayOffForm();
                dayOff dayOffObj = ctx.dayOffList.Where(x => x.flowId == fmain.id).FirstOrDefault();
                subModel.dayOffForm = new vwdayOff
                {
                    id = dayOffObj.id,
                    dtBegin = dayOffObj.dtBegin,
                    dtEnd = dayOffObj.dtEnd,
                    hours = dayOffObj.hours,
                    dType = dayOffObj.dType,
                    sMemo = dayOffObj.sMemo,
                    jobAgent = dayOffObj.jobAgent
                };
                ViewBag.SubModel = subModel;
                return View("Details", list);
            }
            #endregion

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
            FlowMain fmain = ctx.FlowMainList.Where(x => x.id == id).FirstOrDefault();
            if (fsub != null && fmain != null)
            {
                if (fmain.defId == "OverTime")
                {
                    fsub.signDate = context.getLocalTiime();
                    fsub.signResult = Convert.ToInt16(signValue);
                    fsub.comment = signMemo;
                    context.SaveChanges();

                    dbHelper dbh = new dbHelper();
                    if (fsub.signResult == 1)
                    {
                        dbh.execSql("update FlowMains set flowStatus=2 where id='" + id + "'");
                    }

                    if (fsub.signResult == 2)
                    {
                        int allDenyCnt = context.FlowSubList.Where(x => x.pid == id && x.signResult == 2).Count();
                        int allCnt = context.FlowSubList.Where(x => x.pid == id).Count();
                        //if (allCnt == allDenyCnt)
                        {
                            dbh.execSql("update FlowMains set flowStatus=3 where id='" + id + "'");
                        }
                    }
                }
                if (fmain.defId == "DayOff")
                {
                    List<FlowSub> qPriorSigner = ctx.FlowSubList.Where(x => x.pid == id && x.seq < fsub.seq && x.signResult == 0).ToList<FlowSub>();
                    if (qPriorSigner.Count()>0)
                    {
                        TempData["error"] = "上一位簽核人尚未簽核";
                        return RedirectToAction("Details", "FormMgr",new { id = fmain.id });
                    }
                    fsub.signDate = context.getLocalTiime();
                    fsub.signResult = Convert.ToInt16(signValue);
                    fsub.comment = signMemo;
                    context.SaveChanges();

                    dbHelper dbh = new dbHelper();
                    var fsublist = ctx.FlowSubList.Where(x => x.pid == id);
                    if (fsub.signResult == 1 && 
                        fsublist.Where(x=>x.signResult==1).Count()==fsublist.Count()
                    )
                    {
                        dbh.execSql("update FlowMains set flowStatus=2 where id='" + id + "'");
                    }

                    if (fsub.signResult == 2)
                    {
                        int allCnt = context.FlowSubList.Where(x => x.pid == id).Count();
                        {
                            dbh.execSql("update FlowMains set flowStatus=3 where id='" + id + "'");
                        }
                    }
                }
            }

            return RedirectToAction("Query", "FormMgr");
        }

        [AdminAuthorize(Roles = "Admin,Employee")]
        public ActionResult Query()
        {
            var context = new ApplicationDbContext();

            var user = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            //var unSignMainList = (from item in context.FlowMainList
            //                      where item.flowStatus != 1 && item.flowStatus != 99 && item.defId == "OverTime"
            //                      select item.id);

            var signOkSubList = from item in context.FlowSubList
                                      where item.signResult !=0 && item.signResult != 99 && item.workNo == user.workNo
                                      select item.pid;


            var mySignSubList = (from item in context.FlowSubList
                                 where item.workNo == user.workNo
                                 && signOkSubList.Contains(item.pid)
                                 select item.pid).ToList<string>();

            var mySignMainList = context.FlowMainList.Where(x => mySignSubList.Contains(x.id))
                .OrderByDescending(x=>x.billDate).ToList<FlowMain>();

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
            ViewBag.FlowPageType = "Query";
            return View(list);
        }

        [AdminAuthorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult ListAll(string status = "0")
        {
            var context = new ApplicationDbContext();
            List<SelectListItem> StatusItems = new List<SelectListItem>();

            StatusItems.Add(new SelectListItem
            {
                Text = "全部",
                Value = "0"
            });

            foreach (var item in context.flowStatusList())
            {
                StatusItems.Add(new SelectListItem
                {
                    Text = item.nm,
                    Value = item.id.ToString()
                });
            }
            ViewBag.SignResultItems = StatusItems;

            int iStatus = int.Parse(status);

            List<FlowMain> SignMainList = null;

            if (iStatus == 0)
            {
                SignMainList = (from item in context.FlowMainList
                                where item.flowStatus != 99
                                orderby item.billDate descending
                                select item).ToList<FlowMain>();
            }
            else
            {
                SignMainList = (from item in context.FlowMainList
                                where item.flowStatus != 99
                                orderby item.billDate descending
                                where item.flowStatus == iStatus
                                select item).ToList<FlowMain>();
            }

            List<vwFlowMain> list = new List<vwFlowMain>();

            foreach (FlowMain item in SignMainList)
            {
                var Status = context.flowStatusList().Where(x => x.id == item.flowStatus).FirstOrDefault();
                var sender = context.Users.Where(x => x.workNo.Equals(item.senderNo)).FirstOrDefault();
                if (sender != null)
                {
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
            }

            return View(list);
        }
    }
}