using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eform.Attributes;
using eform.Models;
using Newtonsoft.Json.Linq;
using System.Data;

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
                                 where item.workNo == user.workNo && item.signResult == 0
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
                    billNo=item.billNo,
                    billDate = item.billDate,
                    flowName = item.flowName,
                    flowStatus = Status == null ? "簽核中" : Status.nm
                };
                list.Add(vwItem);
            }
            var rlist = list.OrderByDescending(x => x.billDate).ToList<vwFlowMain>();
            return View(rlist);
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
        public ActionResult Details(string id, string FlowPageType = "", string ReturnAction = "")
        {
            if (ctx.FlowSubList.Where(x=>x.pid==id && x.workNo== User.Identity.Name).Count()==0)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ctx.FlowSubList.Where(x => x.pid == id && x.workNo == User.Identity.Name && x.signType == 701).Count() > 0)
            {
                FlowPageType = "HRCheck";
            }

            var context = new ApplicationDbContext();
            ViewBag.FlowPageType = FlowPageType;
            ViewBag.ReturnAction = ReturnAction;

            


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

            #region "非工作時間廠務申請單"
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

            #region "加班單"
            if (fmain.defId == "RealOverTime")
            {
                ReqOverTime reqOverTimeObj = context.reqOverTimeList.Where(x => x.flowId == id).FirstOrDefault();
                vwRealOverTime vwReqOverTimeObj = null;
                if (reqOverTimeObj != null)
                {
                    vwReqOverTimeObj = new vwRealOverTime
                    {
                        user = ctx.getUserByWorkNo(fmain.senderNo),
                        dtBegin = reqOverTimeObj.dtBegin,
                        dtEnd = reqOverTimeObj.dtEnd,
                        hours = reqOverTimeObj.hours,
                        sMemo = reqOverTimeObj.sMemo
                    };
                    JObject jobjext = null;
                    try
                    {
                        jobjext = JObject.Parse(reqOverTimeObj.jext);
                        vwReqOverTimeObj.sMemo2 = jobjext["sMemo2"] == null ? "" : jobjext["sMemo2"].ToString();
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

            #region "外出申請單"
            if (fmain.defId == "PublicOut")
            {
                publicOut publicOutObj = ctx.publicOutList.Where(x => x.flowId == fmain.id).FirstOrDefault();
                vwPublicOut subModel = new vwPublicOut
                {
                    id = publicOutObj.id,
                    user = ctx.getUserByWorkNo(fmain.senderNo),
                    requestDate = publicOutObj.requestDate,
                    dtBegin = publicOutObj.dtBegin,
                    dtEnd = publicOutObj.dtEnd,
                    subject = publicOutObj.subject,
                    destination = publicOutObj.destination,
                    transport = publicOutObj.transport
                };
                ViewBag.SubModel = subModel;
                return View("Details", list);
            }

            #endregion

            return View(list);
        }

        bool isGmDep(string workNo)
        {
            dbHelper dbh = new dbHelper();
            string sql = @"select count(b.depNo) as cnt
                            from PoUsers a inner join jobPoes b on a.poNO=b.poNo 
                                           inner join deps c on b.depNo=c.depNo
                            where a.UserId='@workno' and a.applicationUser_id is not null and b.depNo='001'";
            sql = sql.Replace("@workno", workNo);
            return dbh.sql2count(sql) > 0;
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

            FlowSub fsub = context.FlowSubList.Where(x => x.pid == id && x.workNo == workNo && x.signType != 701).FirstOrDefault();
            FlowMain fmain = ctx.FlowMainList.Where(x => x.id == id).FirstOrDefault();
            bool bIsGmDep = false;
            bIsGmDep = isGmDep(workNo);
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
                       dbh.execSql("update FlowMains set flowStatus=3 where id='" + id + "'");
                    }
                }

                if (fmain.defId == "RealOverTime")
                {
                    if (fsub.signType != 3 && (!bIsGmDep))
                    {
                        List<FlowSub> qPriorSigner = ctx.FlowSubList.Where(x => x.pid == id && x.signType !=701 && x.seq < fsub.seq && x.signResult == 0).ToList<FlowSub>();
                        if (qPriorSigner.Count() > 0)
                        {
                            TempData["error"] = "上一位簽核人尚未簽核";
                            return RedirectToAction("Details", "FormMgr", new { id = fmain.id });
                        }
                    }
                    fsub.signDate = context.getLocalTiime();
                    fsub.signResult = Convert.ToInt16(signValue);
                    fsub.comment = signMemo;
                    context.SaveChanges();

                    dbHelper dbh = new dbHelper();
                    var fsublist = ctx.FlowSubList.Where(x => x.pid == id && x.signType !=701);
                    if (fsub.signResult == 1 &&
                        (fsublist.Where(x => x.signResult == 1).Count() == fsublist.Count() || bIsGmDep)
                    )
                    {
                        dbh.execSql("update FlowMains set flowStatus=2 where id='" + id + "'");
                    }

                    if (fsub.signResult == 2)
                    {
                        dbh.execSql("update FlowMains set flowStatus=3 where id='" + id + "'");
                    }
                }

                if (fmain.defId == "DayOff")
                {
                    if (fsub.signType != 3 && (!bIsGmDep))
                    {
                        List<FlowSub> qPriorSigner = ctx.FlowSubList.Where(x => x.pid == id && x.signType!=701 && x.seq < fsub.seq && x.signResult == 0).ToList<FlowSub>();
                        if (qPriorSigner.Count() > 0)
                        {
                            TempData["error"] = "上一位簽核人尚未簽核";
                            return RedirectToAction("Details", "FormMgr", new { id = fmain.id });
                        }
                    }
                    fsub.signDate = context.getLocalTiime();
                    fsub.signResult = Convert.ToInt16(signValue);
                    fsub.comment = signMemo;
                    context.SaveChanges();

                    dbHelper dbh = new dbHelper();
                    var fsublist = ctx.FlowSubList.Where(x => x.pid == id && x.signType != 701);
                    if (fsub.signResult == 1 &&
                        (fsublist.Where(x => x.signResult == 1).Count() == fsublist.Count() || bIsGmDep)
                    )
                    {
                        dbh.execSql("update FlowMains set flowStatus=2 where id='" + id + "'");
                    }

                    if (fsub.signResult == 2)
                    {
                        dbh.execSql("update FlowMains set flowStatus=3 where id='" + id + "'");
                    }
                }

                if (fmain.defId == "PublicOut")
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
                       dbh.execSql("update FlowMains set flowStatus=3 where id='" + id + "'");
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


            var signOkSubList = from item in context.FlowSubList
                                where item.signResult != 0 && item.signResult != 99 && item.workNo == user.workNo
                                select item.pid;


            var mySignSubList = (from item in context.FlowSubList
                                 where item.workNo == user.workNo
                                 && signOkSubList.Contains(item.pid)
                                 select item.pid).ToList<string>();

            var mySignMainList = context.FlowMainList.Where(x => mySignSubList.Contains(x.id))
                .OrderByDescending(x => x.billDate).ToList<FlowMain>();

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
                    billNo=item.billNo,
                    flowName = item.flowName,
                    flowStatus = Status == null ? "簽核中" : Status.nm
                };
                list.Add(vwItem);
            }
            ViewBag.FlowPageType = "Query";
            return View(list.OrderByDescending(x => x.billDate).ToList<vwFlowMain>());
        }

        [AdminAuthorize(Roles = "Admin,Employee")]
        public ActionResult HRCheckFlow()
        {
            return RedirectToAction("onWorking", "Account");

            var user = ctx.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            var signOkSubList = from item in ctx.FlowSubList
                                where item.signResult != 0 && item.signResult != 99 && item.workNo == user.workNo
                                select item.pid;


            var mySignSubList = (from item in ctx.FlowSubList
                                 where item.workNo == user.workNo
                                 && signOkSubList.Contains(item.pid)
                                 select item.pid).ToList<string>();

            var mySignMainList = ctx.FlowMainList.Where(x => mySignSubList.Contains(x.id))
                .OrderByDescending(x => x.billDate).ToList<FlowMain>();

            List<vwFlowMain> list = new List<vwFlowMain>();

            foreach (FlowMain item in mySignMainList)
            {
                var Status = ctx.flowStatusList().Where(x => x.id == item.flowStatus).FirstOrDefault();
                var sender = ctx.Users.Where(x => x.workNo.Equals(item.senderNo)).FirstOrDefault();
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
            ViewBag.FlowPageType = "HRCheck";
            return View(list.OrderByDescending(x => x.billDate).ToList<vwFlowMain>());
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
                        billNo=item.billNo,
                        billDate = item.billDate,
                        flowName = item.flowName,
                        flowStatus = Status == null ? "簽核中" : Status.nm
                    };
                    list.Add(vwItem);
                }
            }
            return View(list.OrderByDescending(x => x.billDate).ToList<vwFlowMain>());
        }
    }
}