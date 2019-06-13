using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Dapper;
using eform.Attributes;
using eform.Models;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using System.Text;
using eform.Service;
using System.Web.Routing;
using eform.Repo;

namespace eform.Controllers
{
    [AdminAuthorize(Roles = "Admin,Employee")]
    public partial class FormController : Controller
    {
        // GET: FormMgn
        [HttpGet]
        public ActionResult reqInHouse()
        {
            vwReqInHouse model = new vwReqInHouse();
            model.user = ctx.getCurrentUser(User.Identity.Name);
            ViewBag.userList = ctx.getUserList();
            ViewBag.depList = ctx.getDepListP020A1(2);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> reqInHouse(vwReqInHouse Model)
        {
            Model.user = ctx.getCurrentUser(User.Identity.Name);
            ViewBag.userList = ctx.getUserList();
            ViewBag.depList = ctx.getDepListP020A1(2);

            #region "checkinput"
            List<string> errList = new List<string>();

            var context = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            #endregion

            #region "FlowWork"
            string FlowDefKey = "ReqInHouse";
            FlowMain fmain = new FlowMain();
            List<FlowSub> fsublist = new List<FlowSub>();
            FlowDefMain fDefMain = context.FlowDefMainList.Where(x => x.enm == FlowDefKey).FirstOrDefault();
            List<FlowDefSub> fDefSubList = new List<FlowDefSub>();
            if (fDefMain != null)
            {
                fDefSubList = context.FlowDefSubList.Where(x => x.pid == fDefMain.id).OrderBy(x => x.seq).ToList<FlowDefSub>();
            }

            var sender = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            fmain.billNo = ctx.genBillNo(FlowDefKey);
            fmain.id = Guid.NewGuid().ToString();
            fmain.defId = FlowDefKey;
            fmain.flowName = fDefMain.nm;
            fmain.flowStatus = 1;
            fmain.senderNo = sender.workNo;
            fmain.billDate = context.getLocalTiime();
            context.FlowMainList.Add(fmain);

            ReqInHouse fmObj = new ReqInHouse
            {
                contact = Model.contact,
                depNo = Model.depNo,
                reqLimit = Model.reqLimit,
                reqMemo = Model.reqMemo,
                id = Guid.NewGuid().ToString(),
                flowId = fmain.id
            };

            context.reqInHouseList.Add(fmObj);

            int flowSeq = 1;

            dbHelper dbh = new dbHelper();
            string senderPoNo = dbh.sql2Str("select poNo from PoUsers where UserId='" + sender.workNo + "' and ApplicationUser_Id is not null");
            string senderMgrNo = "";

            List<string> signerList = new List<string>();

            if (!string.IsNullOrEmpty(senderPoNo))
            {
                string senderDepNo = dbh.sql2Str("select depNo from jobPoes where poNo='" + senderPoNo + "'");
                if (!string.IsNullOrEmpty(senderDepNo))
                {
                    dep depObj = context.deps.Where(x => x.depNo == senderDepNo).FirstOrDefault();
                    signerList = getDepSigner(context, depObj, sender.workNo, signerList);
                }
            }

            foreach (string signer in signerList)
            {
                FlowSub fsub = new FlowSub();
                fsub.pid = fmain.id;
                fsub.id = Guid.NewGuid().ToString();
                fsub.seq = flowSeq;
                fsub.workNo = signer;
                fsub.signType = 1;
                fsub.signResult = 0;
                flowSeq++;
                context.FlowSubList.Add(fsub);
            }

            foreach (FlowDefSub defItem in fDefSubList)
            {
                if (defItem.workNo != senderMgrNo && signerList.Where(x => x == defItem.workNo).Count() == 0 && defItem.workNo != sender.workNo)
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
                    signerList.Add(defItem.workNo);
                }
            }

            #endregion

            try
            {
                context.SaveChanges();
                StringBuilder sb = new StringBuilder();
                sb.Append("您好," + "<br/><br/>");
                sb.Append(sender.workNo + " " + sender.cName + " 送出廠務派工及總務需求申請單：" + fmain.billNo + "<br/><br/>");
                sb.AppendFormat("<a href='{0}'>{1}</a><br/>",
                    Url.Action("Details", "FormMgr", new RouteValueDictionary(new { id = fmain.id }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority),
                    "單據網址"
                );
                sb.Append("<br/>此信件為系統發出，請勿直接回信<br/>");
                foreach (string signer in signerList)
                {
                    vwEmployee emp = ctx.getUserByWorkNo(signer);
                    if (!string.IsNullOrEmpty(emp.Email))
                    {
                        List<EmailAddress> mailList = new List<EmailAddress>();

                        mailList.Add(new EmailAddress
                        {
                            Email = emp.Email,
                            Name = emp.workNo + " " + emp.UserCName
                        });
                        await SendGridSrv.sendEmail(mailList, "廠務派工及總務需求申請單", sb.ToString());
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(Model);
            }

        }

        void setHMList2()
        {
            List<SelectListItem> beginHH = new List<SelectListItem>();
            List<SelectListItem> beginMM = new List<SelectListItem>();
            List<SelectListItem> endHH = new List<SelectListItem>();
            List<SelectListItem> endMM = new List<SelectListItem>();

            for (int i = 0; i <= 24; i++)
            {
                beginHH.Add(new SelectListItem { Text = i.ToString("0#"), Value = i.ToString() });
                endHH.Add(new SelectListItem { Text = i.ToString("0#"), Value = i.ToString() });
            }

            beginMM.Add(new SelectListItem { Text = "00", Value = "00" });
            endMM.Add(new SelectListItem { Text = "00", Value = "00" });
            beginMM.Add(new SelectListItem { Text = "30", Value = "30" });
            endMM.Add(new SelectListItem { Text = "30", Value = "30" });

            ViewBag.beginHH = beginHH;
            ViewBag.beginMM = beginMM;
            ViewBag.endHH = endHH;
            ViewBag.endMM = endMM;

        }

        void prepareEventScheduleViewBag()
        {
            setHMList2();
            try
            {
                ViewBag.wording = ctx.FlowDefMainList.Where(x => x.enm == "EventSchedule").ToList<FlowDefMain>().First().wording;
            }
            catch (Exception ex)
            {
                ViewBag.wording = "";
            }
        }
        [HttpGet]
        public ActionResult eventSchedule(string id = "")
        {
            vwEventSchedule model = new vwEventSchedule();
            model.user = ctx.getCurrentUser(User.Identity.Name);
            prepareEventScheduleViewBag();

            if (id == "")
            {
                model.id = "";
                model.billDate = ctx.getLocalTiime();
                model.beginDate = ctx.getLocalTiime();
                model.endDate = ctx.getLocalTiime();
            }
            else
            {
                EventSchedule eventObj = ctx.eventScheduleList.Where(x => x.id == id).FirstOrDefault();
                FlowMain fmain = ctx.FlowMainList.Where(x => x.id == eventObj.flowId).FirstOrDefault();
                model.id = eventObj.id;
                model.billNo = fmain.billNo;
                model.flowId = fmain.id;
                model.billDate = fmain.billDate;
                model.beginHH = eventObj.beginHH;
                model.beginMM = eventObj.beginMM;
                model.endHH = eventObj.endHH;
                model.endMM = eventObj.endMM;
                model.beginDate = eventObj.beginDate;
                model.endDate = eventObj.endDate;
                model.subject = eventObj.subject;
                model.location = eventObj.location;
                model.eventType = eventObj.eventType;
                model.sMemo = eventObj.sMemo;
                model.colorTag = eventObj.colorTag;
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> eventSchedule(vwEventSchedule model)
        {
            model.user = ctx.getCurrentUser(User.Identity.Name);
            prepareEventScheduleViewBag();

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                DateTime _dt = Convert.ToDateTime(model.billDate);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("billDate", "請輸入正確日期格式");
                return View(model);
            }
            var sender= ctx.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            FlowMain fmain = new FlowMain();
            if (string.IsNullOrEmpty(model.id))
            {
                #region "FlowWork"
                string FlowDefKey = "EventSchedule";
                List<FlowSub> fsublist = new List<FlowSub>();
                FlowDefMain fDefMain = ctx.FlowDefMainList.Where(x => x.enm == FlowDefKey).FirstOrDefault();
                List<FlowDefSub> fDefSubList = new List<FlowDefSub>();
                if (fDefMain != null)
                {
                    fDefSubList = ctx.FlowDefSubList.Where(x => x.pid == fDefMain.id).OrderBy(x => x.seq).ToList<FlowDefSub>();
                }


                fmain.billNo = ctx.genBillNo(FlowDefKey);
                fmain.id = Guid.NewGuid().ToString();
                fmain.defId = FlowDefKey;
                fmain.flowName = fDefMain.nm;
                fmain.flowStatus = 1;
                fmain.senderNo = sender.workNo;
                fmain.billDate = model.billDate;
                ctx.FlowMainList.Add(fmain);

                EventSchedule fmObj = new EventSchedule
                {
                    id = Guid.NewGuid().ToString(),
                    flowId = fmain.id,
                    subject = model.subject,
                    location = model.location,
                    beginHH = model.beginHH,
                    beginMM = model.beginMM,
                    endHH = model.endHH,
                    endMM = model.endMM,
                    beginDate=model.beginDate,
                    endDate=model.endDate,
                    eventType = model.eventType,
                    sMemo = model.sMemo,
                    colorTag=model.colorTag
                };

                ctx.eventScheduleList.Add(fmObj);

                int flowSeq = 1;

                dbHelper dbh = new dbHelper();
                string senderPoNo = dbh.sql2Str("select poNo from PoUsers where UserId='" + sender.workNo + "' and ApplicationUser_Id is not null");
                string senderMgrNo = "";

                List<string> signerList = new List<string>();

                if (!string.IsNullOrEmpty(senderPoNo))
                {
                    string senderDepNo = dbh.sql2Str("select depNo from jobPoes where poNo='" + senderPoNo + "'");
                    if (!string.IsNullOrEmpty(senderDepNo))
                    {
                        dep depObj = ctx.deps.Where(x => x.depNo == senderDepNo).FirstOrDefault();
                        signerList = getDepSigner(ctx, depObj, sender.workNo, signerList);
                    }
                }

                foreach (string signer in signerList)
                {
                    FlowSub fsub = new FlowSub();
                    fsub.pid = fmain.id;
                    fsub.id = Guid.NewGuid().ToString();
                    fsub.seq = flowSeq;
                    fsub.workNo = signer;
                    fsub.signType = 1;
                    fsub.signResult = 0;
                    flowSeq++;
                    ctx.FlowSubList.Add(fsub);
                }

                foreach (FlowDefSub defItem in fDefSubList)
                {
                    if (defItem.workNo != senderMgrNo && signerList.Where(x => x == defItem.workNo).Count() == 0 && defItem.workNo != sender.workNo)
                    {
                        FlowSub fsub = new FlowSub();
                        fsub.pid = fmain.id;
                        fsub.id = Guid.NewGuid().ToString();
                        fsub.seq = flowSeq;
                        fsub.workNo = defItem.workNo;
                        fsub.signType = defItem.signType;
                        fsub.signResult = 0;
                        flowSeq++;
                        ctx.FlowSubList.Add(fsub);
                        signerList.Add(defItem.workNo);
                    }
                }
                #endregion

                try
                {
                    ctx.SaveChanges();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("您好," + "<br/><br/>");
                    if (model.eventType == 1)
                    {
                        sb.Append(sender.workNo + " " + sender.cName + " 送出業務拜訪行程" +"<br/>");
                    }
                    else if (model.eventType == 2)
                    {
                        sb.Append(sender.workNo + " " + sender.cName + " 送出公司行程/活動"+"<br/>");
                    }
                    else if (model.eventType == 3)
                    {
                        sb.Append(sender.workNo + " " + sender.cName + " 送出會議室預約"+"<br/>");
                    }
                    sb.AppendFormat("單號:{0}<br/>", fmain.billNo);
                    sb.AppendFormat("<a href='{0}'>{1}</a><br/>",
                        Url.Action("Details", "FormMgr", new RouteValueDictionary(new { id = fmain.id }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority),
                        "單據網址"
                    );

                    sb.AppendFormat("申請人:{0}<br/>", sender.cName);
                    sb.AppendFormat("申請日期:{0}<br/>", model.billDate.Value.ToString("yyyy-MM-dd"));

                    if (model.eventType == 1)
                    {
                        sb.AppendFormat("活動種類:{0}<br/>", "業務拜訪行程");
                    }
                    else if (model.eventType == 2)
                    {
                        sb.AppendFormat("活動種類:{0}<br/>", "公司行程/活動");
                    }
                    else if (model.eventType == 3)
                    {
                        sb.AppendFormat("活動種類:{0}<br/>", "會議室預約");
                    }

                    sb.AppendFormat("標題:{0}<br/>", model.subject);
                    sb.AppendFormat("地點:{0}<br/>", model.location );

                    sb.AppendFormat("開始日期:{0} {1}:{2}<br/>", 
                        model.beginDate.Value.ToString("yyyy-MM-dd"), 
                        model.beginHH.ToString().PadLeft(2,'0'),
                        model.beginMM.ToString().PadLeft(2,'0')
                    );
                    sb.AppendFormat("結束日期:{0} {1}:{2}<br/>", 
                        model.endDate.Value.ToString("yyyy-MM-dd") ,
                        model.endHH.ToString().PadLeft(2, '0'),
                        model.endMM.ToString().PadLeft(2, '0')
                    );

                    sb.Append("<br/>此信件為系統發出，請勿直接回信<br/>");
                    foreach (string signer in signerList)
                    {
                        vwEmployee emp = ctx.getUserByWorkNo(signer);
                        if (!string.IsNullOrEmpty(emp.Email))
                        {
                            List<EmailAddress> mailList = new List<EmailAddress>();

                            mailList.Add(new EmailAddress
                            {
                                Email = emp.Email,
                                Name = emp.workNo + " " + emp.UserCName
                            });
                            await SendGridSrv.sendEmail(mailList, "公司行程規劃通知", sb.ToString());
                        }
                    }

                    return RedirectToAction("ListEventSchedule");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
            }
            else
            {
                fmain = ctx.FlowMainList.Where(x => x.id == model.flowId).FirstOrDefault();
                EventSchedule eventObj = ctx.eventScheduleList.Where(x => x.id == model.id).FirstOrDefault();

                eventObj.subject = model.subject;
                eventObj.location = model.location;
                eventObj.beginHH = model.beginHH;
                eventObj.beginMM = model.beginMM;
                eventObj.endHH = model.endHH;
                eventObj.endMM = model.endMM;
                eventObj.beginDate = model.beginDate;
                eventObj.endDate = model.endDate;
                eventObj.eventType = model.eventType;
                eventObj.sMemo = model.sMemo;
                eventObj.colorTag = model.colorTag;

                ctx.SaveChanges();

                return RedirectToAction("Details", "Form", new {@id= model.flowId });
            }

        }

        public ActionResult ListEventSchedule()
        {
            var context = new ApplicationDbContext();
            List<vwEventSchedule> list = new List<vwEventSchedule>();

            var sender = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            List<FlowMain> flist = context.FlowMainList.Where(x => x.defId == "EventSchedule" && x.flowStatus != 99).OrderByDescending(x => x.billDate).ToList<FlowMain>();
            List<string> billNoList = (from item in flist select item.id).ToList<string>();
            List<EventSchedule> formObjList = ctx.eventScheduleList.Where(x => billNoList.Contains(x.flowId)).ToList<EventSchedule>();

            foreach (FlowMain fitem in flist)
            {
                EventSchedule formObj = formObjList.Where(x => x.flowId == fitem.id).FirstOrDefault();
                vwEventSchedule item = new vwEventSchedule
                {
                    id = formObj.id,
                    flowId = formObj.flowId,
                    billNo = fitem.billNo,
                    user = ctx.getUserByWorkNo(fitem.senderNo),
                    billDate = fitem.billDate,
                    eventType = formObj.eventType
                };
                list.Add(item);
            }
            return View(list);
        }
    }
}