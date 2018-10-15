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

namespace eform.Controllers
{
    [AdminAuthorize(Roles = "Admin,Employee")]
    public class FormController : Controller
    {
        ApplicationDbContext ctx;
        SqlConnection con;
        public FormController()
        {
            ctx = new ApplicationDbContext();
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        // GET: Form
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();
            List<vwFlowMain> list = new List<vwFlowMain>();

            var sender = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            List<FlowMain> flist = context.FlowMainList.Where(x => x.senderNo == sender.workNo && x.flowStatus != 99).OrderByDescending(x => x.billDate).ToList<FlowMain>();

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

        void getOverTime(FlowMain fmain)
        {
            string id = fmain.id;
            ReqOverTime reqOverTimeObj = ctx.reqOverTimeList.Where(x => x.flowId == id).FirstOrDefault();
            vwReqOverTime vwReqOverTimeObj = null;
            if (reqOverTimeObj != null)
            {
                vwReqOverTimeObj = new vwReqOverTime
                {
                    billDate = fmain.billDate,
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
                    vwReqOverTimeObj.poNm = jobjext["poNo"] == null ? "" : ctx.jobPos.Where(x => x.poNo.Equals(vwReqOverTimeObj.poNo)).FirstOrDefault().poNm;

                    jobPo poObj = ctx.jobPos.Where(x => x.poNo == vwReqOverTimeObj.poNo).First();
                    string depNo = poObj.depNo;
                    string depNm = ctx.getParentDeps(depNo, ctx) + " : " + poObj.poNm;
                    vwReqOverTimeObj.depNm = depNm;

                }
                catch (Exception ex)
                {
                }
            }
            ViewBag.SubModel = vwReqOverTimeObj;
        }
        void getRealOverTime(FlowMain fmain)
        {
            string id = fmain.id;
            ReqOverTime reqOverTimeObj = ctx.reqOverTimeList.Where(x => x.flowId == id).FirstOrDefault();
            vwRealOverTime vwReqOverTimeObj = null;
            if (reqOverTimeObj != null)
            {
                vwReqOverTimeObj = new vwRealOverTime
                {
                    user = ctx.getUserByWorkNo(fmain.senderNo),
                    billDate = fmain.billDate,
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
        }
        // GET: Form/Details/5
        public ActionResult Details(string id)
        {
            var context = new ApplicationDbContext();
            List<vwFlowSub> list = new List<vwFlowSub>();
            List<FlowSub> fsubList = ctx.FlowSubList.Where(x => x.pid == id).OrderBy(x => x.seq).ToList<FlowSub>();
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
                    signer = signUser == null ? "" : signUser.workNo + "-" + signUser.cName
                };
                list.Add(item);
            }

            FlowMain fmain = ctx.FlowMainList.Where(x => x.id == id).FirstOrDefault();

            vwEmployee employee = ctx.getUserByWorkNo(fmain.senderNo);
            ViewBag.Employee = employee;

            if (null != fmain)
            {
                ViewBag.flowMain = fmain;
                if (fmain.defId == "OverTime")
                {
                    getOverTime(fmain);
                    return View("Details", list);
                }
                if (fmain.defId == "RealOverTime")
                {
                    getRealOverTime(fmain);
                    return View("Details", list);
                }
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
            }

            return View(list);
        }
        void initCreateOverTimeFormViewBag(ApplicationDbContext context)
        {
            List<SelectListItem> placelist = new List<SelectListItem>();
            placelist.Add(new SelectListItem { Value = "", Text = "選擇工作地點" });
            placelist.Add(new SelectListItem { Value = "測試區", Text = "測試區" });
            placelist.Add(new SelectListItem { Value = "組裝區", Text = "組裝區" });
            placelist.Add(new SelectListItem { Value = "加工區", Text = "加工區" });
            placelist.Add(new SelectListItem { Value = "倉庫區", Text = "倉庫區" });
            placelist.Add(new SelectListItem { Value = "辦公區", Text = "辦公區" });
            placelist.Add(new SelectListItem { Value = "公道路辦公室", Text = "公道路辦公室" });
            placelist.Add(new SelectListItem { Value = "其他", Text = "其他" });
            ViewBag.placelist = placelist;

            var user = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            string workno = user.workNo;
            dbHelper dbh = new dbHelper();
            string sql = @"select a.poNo,b.depNo,b.poNm,c.depNm
                            from PoUsers a inner join jobPoes b on a.poNO=b.poNo 
                                           inner join deps c on b.depNo=c.depNo
                            where a.UserId='@workno' and a.applicationUser_id is not null";
            sql = sql.Replace("@workno", workno);
            DataTable tbpo = dbh.sql2tb(sql);
            List<vwPoNo> polist = new List<vwPoNo>();
            List<dep> deplist = new List<dep>();
            foreach (DataRow r in tbpo.Rows)
            {
                if (deplist.Where(x => x.depNo == r["depNo"].ToString()).Count() == 0)
                {
                    deplist.Add(new dep
                    {
                        depNo = r["depNo"].ToString(),
                        depNm = r["depNm"].ToString()
                    });
                }

                polist.Add(new vwPoNo
                {
                    depNo = r["depNo"].ToString(),
                    depNm = r["depNm"].ToString(),
                    poNo = r["poNo"].ToString(),
                    poNm = r["poNm"].ToString()
                });
            }

            foreach (var item in polist)
            {
                string depParentName = "";
                if (item.depNo == "001")
                {
                    depParentName = item.depNm;
                }
                else
                {
                    depParentName = context.getParentDeps(item.depNo, context);
                }

                item.depNm = depParentName + ":" + item.poNm;
            }

            setHMList();
            ViewBag.depList = deplist;
            ViewBag.poList = polist;
        }
        void setHMList()
        {
            List<SelectListItem> beginHH = new List<SelectListItem>();
            List<SelectListItem> beginMM = new List<SelectListItem>();
            List<SelectListItem> endHH = new List<SelectListItem>();
            List<SelectListItem> endMM = new List<SelectListItem>();

            for (int i = 0; i <= 24; i++)
            {
                beginHH.Add(new SelectListItem { Text = i.ToString("0#"), Value = i.ToString("0#") });
                endHH.Add(new SelectListItem { Text = i.ToString("0#"), Value = i.ToString("0#") });
            }

            beginMM.Add(new SelectListItem { Text = "00", Value = "00" });
            endMM.Add(new SelectListItem { Text = "00", Value = "00" });
            beginMM.Add(new SelectListItem { Text = "30", Value = "30" });
            endMM.Add(new SelectListItem { Text = "30", Value = "30" });

            //for (int i = 0; i <= 60; i++)
            //{
            //    beginMM.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            //    endMM.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            //}
            ViewBag.beginHH = beginHH;
            ViewBag.beginMM = beginMM;
            ViewBag.endHH = endHH;
            ViewBag.endMM = endMM;

        }
        List<string> getDepSigner(ApplicationDbContext ctx, dep depObj, string senderWorkNo, List<string> signerList)
        {
            if (signerList == null)
            {
                signerList = new List<string>();
            }
            dbHelper dbh = new dbHelper();
            if (depObj == null)
            {
                return signerList;
            }

            var mgrPo = ctx.jobPos.Where(x => x.isFormSigner == true && x.depNo == depObj.depNo).FirstOrDefault();
            if (mgrPo != null)
            {
                string mgrNo = dbh.sql2Str("select top 1 UserId from PoUsers where  ApplicationUser_Id is not null and  poNo='" + mgrPo.poNo + "' and UserId <> '" + senderWorkNo + "'");

                if (!string.IsNullOrEmpty(mgrNo))
                {
                    signerList.Add(mgrNo);
                }
            }
            if (depObj.depLevel == 1)
            {
                return signerList;
            }
            else
            {
                dep parentDepObj = ctx.deps.Where(x => x.depNo == depObj.parentDepNo).FirstOrDefault();
                if (parentDepObj != null)
                {
                    signerList = getDepSigner(ctx, parentDepObj, senderWorkNo, signerList);
                }
                return signerList;
            }
        }
        [AdminAuthorize(Roles = "Employee,Admin")]
        public ActionResult CreateOverTimeForm()
        {
            string UserName = User.Identity.Name;
            var context = new ApplicationDbContext();
            var user = context.Users.Where(x => x.workNo == UserName).FirstOrDefault();
            ViewBag.UserName = user.cName;
            initCreateOverTimeFormViewBag(context);
            vwReqOverTime Model = new vwReqOverTime();
            Model.sType = "平日";
            return View(Model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOverTimeForm(vwReqOverTime Model)
        {
            #region "checkinput"
            List<string> errList = new List<string>();

            DateTime dtBegin, dtEnd;
            dtBegin = Convert.ToDateTime(Model.dtBegin);
            dtEnd = Convert.ToDateTime(Model.dtEnd);
            try
            {
                dtBegin = new DateTime(dtBegin.Year, dtBegin.Month, dtBegin.Day, Model.beginHH, Model.beginMM, 0);
                dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, Model.endHH, Model.endMM, 0);

                int icompare = DateTime.Compare(dtBegin, dtEnd);
                if (icompare >= 0)
                {
                    errList.Add("結束時間必須大於開始時間");
                }

            }
            catch (Exception ex)
            {
                errList.Add("開始時間、結束時間必須是正確格式");
            }
            if (string.IsNullOrEmpty(Model.poNo))
            {
                errList.Add("請選擇部門與職稱");
            }
            if (string.IsNullOrEmpty(Model.sType))
            {
                errList.Add("請選擇加班類型");
            }
            if (Model.place == "其他" && string.IsNullOrEmpty(Model.otherPlace))
            {
                errList.Add("請選擇工作地點");
            }

            if (string.IsNullOrEmpty(Model.place))
            {
                errList.Add("請選擇工作地點");
            }

            if (string.IsNullOrEmpty(Model.sMemo))
            {
                errList.Add("請輸入加班事由");
            }

            if (errList.Count > 0)
            {
                ViewBag.UserName = Request.Form["worker"];
                ModelState.AddModelError("", string.Join(" , ", errList.ToArray<string>()));
            }

            #endregion

            var context = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {
                initCreateOverTimeFormViewBag(context);
                return View(Model);
            }

            Model.hours = Convert.ToInt32(((DateTime)Model.dtEnd).Subtract((DateTime)Model.dtBegin).TotalHours);

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
            fmain.flowName = fDefMain.nm;
            fmain.flowStatus = 1;
            fmain.senderNo = sender.workNo;
            fmain.billDate = context.getLocalTiime();//Model.billDate;
            context.FlowMainList.Add(fmain);

            ReqOverTime fmOverTime = new ReqOverTime
            {
                dtBegin = dtBegin,
                dtEnd = dtEnd,
                hours = Model.hours,
                sMemo = Model.sMemo,
                id = Guid.NewGuid().ToString(),
                flowId = fmain.id
            };

            context.reqOverTimeList.Add(fmOverTime);

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

            try
            {
                context.SaveChanges();

                JObject jobj = new JObject();
                jobj["worker"] = sender.cName;
                jobj["stype"] = Model.sType;
                jobj["place"] = Model.place;
                jobj["otherPlace"] = Model.otherPlace;
                jobj["prjId"] = Model.prjId;
                jobj["sMemo2"] = Model.sMemo2;
                jobj["depNo"] = Model.depNo;
                jobj["poNo"] = Model.poNo;

                dbh.execSql("update ReqOverTimes set jext =N'" + jobj.ToString() + "' where flowId='" + fmain.id + "'");

                List<EmailAddress> mailList = new List<EmailAddress>();
                foreach (FlowDefSub defItem in fDefSubList)
                {
                    vwEmployee emp = ctx.getUserByWorkNo(defItem.workNo);
                    if (!string.IsNullOrEmpty(emp.Email))
                    {
                        mailList.Add(new EmailAddress
                        {
                            Email = emp.Email,
                            Name = emp.workNo + " " + emp.UserCName
                        });
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("您好," + "<br/><br/>");
                sb.Append(sender.workNo + " " + sender.cName + " 送出非工作時間廠務申請單" + "<br/><br/>");
                sb.AppendFormat("<a href='{0}'>{1}</a><br/>",
                    Url.Action("Details", "FormMgr", new RouteValueDictionary(new { id = fmain.id }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority),
                    "單據網址"
                );
                sb.Append("<br/>此信件為系統發出，請勿直接回信<br/>");
                await SendGridSrv.sendEmail(mailList, "非工作時間廠務申請單送出通知", sb.ToString());

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                initCreateOverTimeFormViewBag(context);
                ViewBag.UserName = Request.Form["worker"].ToString();
                return View(Model);
            }
        }
        void initCreateDayOffFormViewBag(vwDayOffForm model)
        {
            model.user = ctx.getCurrentUser(User.Identity.Name);
            setHMList();
            List<SelectListItem> dTypeList = new List<SelectListItem>();

            foreach (var item in ctx.dayOffTypeList)
            {
                dTypeList.Add(new SelectListItem { Text = item.v, Value = item.v });
            }
            ViewBag.dTypeList = dTypeList;
            ViewBag.userlist = ctx.getUserList();
            List<FlowMain> dayOffFlowMainList = ctx.FlowMainList.Where(x => x.senderNo == model.user.workNo && x.defId == model.defId && x.flowStatus == 2).ToList<FlowMain>();
            ViewBag.dayOffFlowMainList = dayOffFlowMainList;
            var qdayOffFlowMainList = from item in dayOffFlowMainList select item.id;
            ViewBag.dayOffList = ctx.dayOffList.Where(x => qdayOffFlowMainList.Contains(x.flowId)).ToList<dayOff>();
        }
        [HttpGet]
        [AdminAuthorize(Roles = "Employee,Admin")]
        public ActionResult CreateDayOffForm()
        {
            vwDayOffForm model = new vwDayOffForm();
            initCreateDayOffFormViewBag(model);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(Roles = "Employee,Admin")]
        public async Task<ActionResult> CreateDayOffForm(vwDayOffForm Model)
        {
            Model.dayOffForm.dType = Model.dType;
            Model.user = ctx.getCurrentUser(User.Identity.Name);
            #region "checkinput"
            List<string> errList = new List<string>();

            DateTime dtBegin, dtEnd;
            dtBegin = Convert.ToDateTime(Model.dayOffForm.dtBegin);
            dtEnd = Convert.ToDateTime(Model.dayOffForm.dtEnd);
            try
            {
                dtBegin = new DateTime(dtBegin.Year, dtBegin.Month, dtBegin.Day, Model.beginHH, Model.beginMM, 0);
                dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, Model.endHH, Model.endMM, 0);

                int icompare = DateTime.Compare(dtBegin, dtEnd);
                if (icompare >= 0)
                {
                    errList.Add("結束時間必須大於開始時間");
                }

            }
            catch (Exception ex)
            {
                errList.Add("開始時間、結束時間必須是正確格式");
            }

            if (string.IsNullOrEmpty(Model.dayOffForm.sMemo))
            {
                errList.Add("請輸入請假事由");
            }

            if (errList.Count > 0)
            {
                ViewBag.UserName = Request.Form["worker"];
                ModelState.AddModelError("", string.Join(" , ", errList.ToArray<string>()));
            }

            #endregion

            var context = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {
                initCreateDayOffFormViewBag(Model);
                return View(Model);
            }

            string FlowDefKey = Model.defId;
            FlowMain fmain = new FlowMain();
            List<FlowSub> fsublist = new List<FlowSub>();
            FlowDefMain fDefMain = context.FlowDefMainList.Where(x => x.enm == FlowDefKey).FirstOrDefault();
            List<FlowDefSub> fDefSubList = new List<FlowDefSub>();
            if (fDefMain != null)
            {
                fDefSubList = context.FlowDefSubList.Where(x => x.pid == fDefMain.id).OrderBy(x => x.seq).ToList<FlowDefSub>();
            }

            fmain.id = Guid.NewGuid().ToString();
            fmain.defId = Model.defId;
            fmain.flowName = ctx.FlowDefMainList.Where(x => x.enm == Model.defId).FirstOrDefault().nm;
            fmain.flowStatus = 1;
            fmain.senderNo = Model.user.workNo;
            fmain.billDate = context.getLocalTiime();
            context.FlowMainList.Add(fmain);

            dayOff dayOffObj = new dayOff
            {
                dtBegin = dtBegin,
                dtEnd = dtEnd,
                hours = Model.dayOffForm.hours,
                dType = Model.dType,
                jobAgent = Model.dayOffForm.jobAgent,
                sMemo = Model.dayOffForm.sMemo,
                id = Model.dayOffForm.id,
                flowId = fmain.id
            };

            context.dayOffList.Add(dayOffObj);

            int flowSeq = 1;

            dbHelper dbh = new dbHelper();
            string senderPoNo = dbh.sql2Str("select poNo from PoUsers where UserId='" + Model.user.workNo + "' and ApplicationUser_Id is not null");
            string senderMgrNo = "";

            List<string> signerList = new List<string>();
            if (!string.IsNullOrEmpty(senderPoNo))
            {
                string senderDepNo = dbh.sql2Str("select depNo from jobPoes where poNo='" + senderPoNo + "'");
                if (!string.IsNullOrEmpty(senderDepNo))
                {
                    dep depObj = context.deps.Where(x => x.depNo == senderDepNo).FirstOrDefault();
                    signerList = getDepSigner(context, depObj, Model.user.workNo, signerList);
                }
            }

            foreach (string signer in signerList)
            {
                FlowSub fsub = new FlowSub();
                fsub.pid = fmain.id;
                fsub.id = Guid.NewGuid().ToString();
                fsub.seq = flowSeq;
                fsub.workNo = signer;
                fsub.signType = 4;
                fsub.signResult = 0;
                flowSeq++;
                context.FlowSubList.Add(fsub);
            }

            foreach (FlowDefSub defItem in fDefSubList)
            {
                if (defItem.workNo != senderMgrNo && signerList.Where(x => x == defItem.workNo).Count() == 0 && defItem.workNo != Model.user.workNo)
                {
                    FlowSub fsub = new FlowSub();
                    fsub.pid = fmain.id;
                    fsub.id = Guid.NewGuid().ToString();
                    fsub.seq = flowSeq;
                    fsub.workNo = defItem.workNo;
                    fsub.signType = 4; //defItem.signType;
                    fsub.signResult = 0;
                    flowSeq++;
                    context.FlowSubList.Add(fsub);
                    signerList.Add(defItem.workNo);
                }
            }
            try
            {
                context.SaveChanges();
                List<EmailAddress> mailList = new List<EmailAddress>();
                foreach (FlowDefSub defItem in fDefSubList)
                {
                    vwEmployee emp = ctx.getUserByWorkNo(defItem.workNo);
                    if (!string.IsNullOrEmpty(emp.Email))
                    {
                        mailList.Add(new EmailAddress
                        {
                            Email = emp.Email,
                            Name = emp.workNo + " " + emp.UserCName
                        });
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("您好," + "<br/><br/>");
                sb.Append(Model.user.workNo + " " + Model.user.UserCName + " 送出請假單" + "<br/><br/>");
                sb.AppendFormat("<a href='{0}'>{1}</a><br/>",
                    Url.Action("Details", "FormMgr", new RouteValueDictionary(new { id = fmain.id }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority),
                    "單據網址"
                );
                sb.Append("<br/>此信件為系統發出，請勿直接回信<br/>");
                await SendGridSrv.sendEmail(mailList, "請假單送出通知", sb.ToString());
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                initCreateDayOffFormViewBag(Model);
                return View(Model);
            }
        }

        [AdminAuthorize(Roles = "Employee,Admin")]
        public ActionResult RealOverTimeForm()
        {
            vwRealOverTime Model = new vwRealOverTime();
            Model.user = ctx.getCurrentUser(User.Identity.Name);
            setHMList();

            string sql = "select a.id,a.dtBegin,a.dtEnd,a.hours,a.sMemo,b.flowStatus ";
            sql += " from ReqOverTimes a inner join FlowMains b on a.flowId=b.id ";
            sql += " where b.senderNo=@sender and b.flowStatus <> 99 and b.defId=@defId order by b.billDate desc";
            List<vwRealOverTime> historyList = con.Query<vwRealOverTime>(sql, new { sender = Model.user.workNo, defId = "RealOverTime" }).ToList<vwRealOverTime>();
            ViewBag.historyList = historyList;
            return View(Model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RealOverTimeForm(vwRealOverTime Model)
        {
            #region "checkinput"
            List<string> errList = new List<string>();

            DateTime dtBegin, dtEnd;
            dtBegin = Convert.ToDateTime(Model.dtBegin);
            dtEnd = Convert.ToDateTime(Model.dtEnd);
            try
            {
                dtBegin = new DateTime(dtBegin.Year, dtBegin.Month, dtBegin.Day, Model.beginHH, Model.beginMM, 0);
                dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, Model.endHH, Model.endMM, 0);

                int icompare = DateTime.Compare(dtBegin, dtEnd);
                if (icompare >= 0)
                {
                    errList.Add("結束時間必須大於開始時間");
                }

            }
            catch (Exception ex)
            {
                errList.Add("開始時間、結束時間必須是正確格式");
            }

            if (Model.hours <= 0)
            {
                errList.Add("小時數必須大於0");
            }

            if (errList.Count > 0)
            {
                ViewBag.UserName = Request.Form["worker"];
                ModelState.AddModelError("", string.Join(" , ", errList.ToArray<string>()));
            }

            #endregion

            var context = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {
                Model.user = ctx.getCurrentUser(User.Identity.Name);
                setHMList();
                return View(Model);
            }

            Model.hours = Model.hours;  //Convert.ToInt32(((DateTime)Model.dtEnd).Subtract((DateTime)Model.dtBegin).TotalHours);

            string FlowDefKey = "RealOverTime";
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
            fmain.defId = FlowDefKey;
            fmain.flowName = fDefMain.nm;
            fmain.flowStatus = 1;
            fmain.senderNo = sender.workNo;
            fmain.billDate = context.getLocalTiime();
            context.FlowMainList.Add(fmain);

            ReqOverTime fmOverTime = new ReqOverTime
            {
                dtBegin = dtBegin,
                dtEnd = dtEnd,
                hours = Model.hours,
                sMemo = Model.sMemo,
                id = Guid.NewGuid().ToString(),
                flowId = fmain.id
            };

            context.reqOverTimeList.Add(fmOverTime);

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
                fsub.signType = 4;
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

            try
            {
                context.SaveChanges();

                JObject jobj = new JObject();
                jobj["sMemo2"] = Model.sMemo2;

                dbh.execSql("update ReqOverTimes set jext =N'" + jobj.ToString() + "' where flowId='" + fmain.id + "'");

                List<EmailAddress> mailList = new List<EmailAddress>();
                foreach (FlowDefSub defItem in fDefSubList)
                {
                    vwEmployee emp = ctx.getUserByWorkNo(defItem.workNo);
                    if (!string.IsNullOrEmpty(emp.Email))
                    {
                        mailList.Add(new EmailAddress
                        {
                            Email = emp.Email,
                            Name = emp.workNo + " " + emp.UserCName
                        });
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("您好," + "<br/><br/>");
                sb.Append(sender.workNo + " " + sender.cName + " 送出加班申請單" + "<br/><br/>");
                sb.AppendFormat("<a href='{0}'>{1}</a><br/>",
                    Url.Action("Details", "FormMgr", new RouteValueDictionary(new { id = fmain.id }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority),
                    "單據網址"
                );
                sb.Append("<br/>此信件為系統發出，請勿直接回信<br/>");
                await SendGridSrv.sendEmail(mailList, "加班申請單送出通知", sb.ToString());

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Model.user = ctx.getCurrentUser(User.Identity.Name);
                setHMList();
                return View(Model);
            }
        }

        [AdminAuthorize(Roles = "Employee,Admin")]
        public ActionResult PublicOutForm()
        {
            vwPublicOut Model = new vwPublicOut();
            Model.requestDate = ctx.getLocalTiime();
            Model.user = ctx.getCurrentUser(User.Identity.Name);
            setHMList();
            return View(Model);
        }
        [AdminAuthorize(Roles = "Employee,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PublicOutForm(vwPublicOut Model)
        {
            #region "checkinput"
            List<string> errList = new List<string>();

            DateTime dtBegin, dtEnd;
            dtBegin = Convert.ToDateTime(Model.dtBegin);
            dtEnd = Convert.ToDateTime(Model.dtEnd);
            try
            {
                dtBegin = new DateTime(dtBegin.Year, dtBegin.Month, dtBegin.Day, Model.beginHH, Model.beginMM, 0);
                dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, Model.endHH, Model.endMM, 0);

                int icompare = DateTime.Compare(dtBegin, dtEnd);
                if (icompare >= 0)
                {
                    errList.Add("回廠時間必須大於出發時間");
                }

            }
            catch (Exception ex)
            {
                errList.Add("開始時間、結束時間必須是正確格式");
            }

            if (errList.Count > 0)
            {
                ModelState.AddModelError("", string.Join(",", errList));
            }

           

            var context = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {
                Model.user = ctx.getCurrentUser(User.Identity.Name);
                Model.requestDate = ctx.getLocalTiime();
                setHMList();
                return View(Model);
            }

            #endregion

            #region "FlowWork"
            string FlowDefKey = "PublicOut";
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
            fmain.defId = FlowDefKey;
            fmain.flowName = fDefMain.nm;
            fmain.flowStatus = 1;
            fmain.senderNo = sender.workNo;
            fmain.billDate = context.getLocalTiime();
            context.FlowMainList.Add(fmain);

            publicOut fmObj = new publicOut
            {
                dtBegin = dtBegin,
                dtEnd = dtEnd,
                destination = Model.destination,
                requestDate = Model.requestDate,
                subject = Model.subject,
                transport = Model.transport,
                id = Guid.NewGuid().ToString(),
                flowId = fmain.id
            };

            context.publicOutList.Add(fmObj);

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

                List<EmailAddress> mailList = new List<EmailAddress>();
                foreach (FlowDefSub defItem in fDefSubList)
                {
                    vwEmployee emp = ctx.getUserByWorkNo(defItem.workNo);
                    if (!string.IsNullOrEmpty(emp.Email))
                    {
                        mailList.Add(new EmailAddress
                        {
                            Email = emp.Email,
                            Name = emp.workNo + " " + emp.UserCName
                        });
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("您好," + "<br/><br/>");
                sb.Append(sender.workNo + " " + sender.cName + " 送出外出申請單" + "<br/><br/>");
                sb.AppendFormat("<a href='{0}'>{1}</a><br/>",
                    Url.Action("Details", "FormMgr", new RouteValueDictionary(new { id = fmain.id }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority),
                    "單據網址"
                );
                sb.Append("<br/>此信件為系統發出，請勿直接回信<br/>");
                await SendGridSrv.sendEmail(mailList, "外出申請單送出通知", sb.ToString());

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Model.user = ctx.getCurrentUser(User.Identity.Name);
                Model.requestDate = ctx.getLocalTiime();
                setHMList();
                return View(Model);
            }

        }
    }
}
