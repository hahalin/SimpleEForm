using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Attributes;
using eform.Models;
using System.Data;
using Newtonsoft.Json.Linq;

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
                    billDate = fmain.billDate,
                    dtBegin = reqOverTimeObj.dtBegin,
                    dtEnd = reqOverTimeObj.dtEnd,
                    hours = reqOverTimeObj.hours,
                    sMemo = reqOverTimeObj.sMemo
                };

                JObject jobjext= null;
                try
                {
                    jobjext=JObject.Parse(reqOverTimeObj.jext);
                    vwReqOverTimeObj.sType= jobjext["stype"] == null ? "" : jobjext["stype"].ToString();
                    vwReqOverTimeObj.place = jobjext["place"] == null ? "" : jobjext["place"].ToString();
                    vwReqOverTimeObj.otherPlace = jobjext["otherPlace"] == null ? "" : jobjext["otherPlace"].ToString();
                    vwReqOverTimeObj.prjId = jobjext["prjId"] == null ? "" : jobjext["prjId"].ToString();
                    //vwReqOverTimeObj.sMemo = jobjext["sMemo"] == null ? "" : jobjext["sMemo"].ToString();
                    vwReqOverTimeObj.sMemo2 = jobjext["sMemo2"] == null ? "" : jobjext["sMemo2"].ToString();
                    vwReqOverTimeObj.worker= jobjext["worker"] == null ? "" : jobjext["worker"].ToString();
                    vwReqOverTimeObj.depNo = jobjext["depNo"] == null ? "" : jobjext["depNo"].ToString();
                    dep depobj = context.deps.Where(x => x.depNo.Equals(vwReqOverTimeObj.depNo)).FirstOrDefault();
                    vwReqOverTimeObj.depNm = jobjext["depNo"] == null ? "" : depobj.depNm;
                    vwReqOverTimeObj.poNo = jobjext["poNo"] == null ? "" : jobjext["poNo"].ToString();
                    vwReqOverTimeObj.poNm = jobjext["poNo"] == null ? "" : context.jobPos.Where(x => x.poNo.Equals(vwReqOverTimeObj.poNo)).FirstOrDefault().poNm;
                }
                catch (Exception ex)
                {
                    //vwReqOverTimeObj.sMemo = ex.Message;
                }


            }
            ViewBag.SubModel = vwReqOverTimeObj;
            return View(list);
        }

        // GET: Form/Create
        [AdminAuthorize(Roles = "Employee,Admin")]
        public ActionResult CreateOverTimeForm()
        {
            string UserName = User.Identity.Name;
            var context = new ApplicationDbContext();
            var user = context.Users.Where(x => x.workNo == UserName).FirstOrDefault();
            ViewBag.UserName = user.cName;
            initViewBag(context);
            return View();
        }

        // POST: Form/CreateOverTimeForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOverTimeForm(vwReqOverTime Model)
        {

            #region "checkinput"
            List<string> errList = new List<string>();
            if (Model.dtEnd.HasValue && Model.dtBegin.HasValue)
            {
                int icompare = DateTime.Compare(Convert.ToDateTime(Model.dtBegin), Convert.ToDateTime(Model.dtEnd));
                if (icompare >= 0)
                {
                    errList.Add("結束時間必須大於開始時間");
                }
            }
            if (string.IsNullOrEmpty(Model.poNo))
            {
                errList.Add("請選擇部門與職稱");
            }
            if (string.IsNullOrEmpty(Model.sType))
            {
                errList.Add("請選擇加班類型");
            }
            if (
                (string.IsNullOrEmpty(Model.place)) ||
                (Model.place == "其他" && string.IsNullOrEmpty(Model.otherPlace))
               )
            {
                errList.Add("請選擇工作地點");
            }
            if (string.IsNullOrEmpty(Model.sMemo))
            {
                errList.Add("請輸入加班事由");
            }

            if (errList.Count>0)
            {
                ViewBag.UserName = Request.Form["worker"];
                ModelState.AddModelError("", string.Join(" , " , errList.ToArray<string>()));
            }

            #endregion

            var context = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {
                initViewBag(context);
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
            fmain.flowName = "加班單";
            fmain.flowStatus = 1;
            fmain.senderNo = sender.workNo;
            fmain.billDate = context.getLocalTiime();//Model.billDate;
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

                JObject jobj = new JObject();
                jobj["worker"] = sender.cName;
                jobj["stype"] = Model.sType;
                jobj["place"] = Model.place;
                jobj["otherPlace"] = Model.otherPlace;
                jobj["prjId"] = Model.prjId;
                jobj["sMemo2"] = Model.sMemo2;
                jobj["depNo"] = Model.depNo;
                jobj["poNo"] = Model.poNo;

                dbh.execSql("update ReqOverTimes set jext =N'"+jobj.ToString()+"'");


                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                initViewBag(context);
                ViewBag.UserName = Request.Form["worker"].ToString();
                return View(Model);
            }
        }


        void initViewBag(ApplicationDbContext context)
        {
            List<SelectListItem> placelist = new List<SelectListItem>();
            placelist.Add(new SelectListItem { Value = "", Text = "選擇工作地點" });
            placelist.Add(new SelectListItem { Value = "測試區", Text = "測試區" });
            placelist.Add(new SelectListItem { Value = "組裝區", Text = "組裝區" });
            placelist.Add(new SelectListItem { Value = "加工區", Text = "加工區" });
            placelist.Add(new SelectListItem { Value = "倉庫區", Text = "倉庫區" });
            placelist.Add(new SelectListItem { Value = "辦公區", Text = "辦公區" });
            placelist.Add(new SelectListItem { Value = "其他", Text = "其他" });
            ViewBag.placelist = placelist;

            var user = context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            string workno = user.workNo;
            dbHelper dbh = new dbHelper();
            string sql = @"select a.poNo,b.depNo,b.poNm,c.depNm
                            from PoUsers a inner join jobPoes b on a.poNO=b.poNo 
                                           inner join deps c on b.depNo=c.depNo
                            where a.UserId='@workno'";
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

            ViewBag.depList = deplist;
            ViewBag.poList = polist;
        }
    }
}
