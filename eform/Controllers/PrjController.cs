using System;
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

namespace eform.Controllers
{
    public class PrjController : Controller
    {
        prjRep rep;
        ApplicationDbContext ctx;
        public PrjController()
        {
            ctx = new ApplicationDbContext();
        }

        // GET: Prj
        [Authorize]
        public ActionResult Index()
        {
            prj prjObj = new prj();
            var emp=ctx.getCurrentUser(User.Identity.Name);
            prjObj.creator = emp.workNo;
            prjObj.creatorNm = emp.UserCName;
            prjObj.createDate = ctx.getLocalTiime();


            return View(prjObj);
        }

        public ActionResult prjDetail(string prjId)
        {
            rep = new prjRep(User.Identity.Name);

            dynamic r = new ExpandoObject();
            r.prjId = prjId;
            prj prjObj= rep.loadPrj(prjId);
            r.prj = prjObj;
            r.empList = rep.loadPrjEmpList(prjObj.id);
            return Content(JsonConvert.SerializeObject(r), "application/json");
        }

        public ActionResult prjList()
        {
            rep = new prjRep(User.Identity.Name);

            return Content(JsonConvert.SerializeObject(rep.prjList()),"application/json");
        }

        [HttpPost]
        public ActionResult Create(FormCollection fm)
        {
            prj prjObj = new prj();
            prjObj.creator = fm["hcreator"].ToString();
            prjObj.prjId = fm["prjId"].ToString();
            prjObj.nm = fm["nm"].ToString();
            prjObj.custId = fm["custId"].ToString();
            prjObj.custNm = fm["custNm"].ToString();
            prjObj.createDate = dateUtils.checkDate(fm["createDate"].ToString());
            prjObj.beginDate = dateUtils.checkDate(fm["beginDate"].ToString());
            prjObj.endDate = dateUtils.checkDate(fm["endDate"].ToString());
            prjObj.sMemo = fm["sMemo"].ToString();
            prjObj.dtMemo = fm["dtMemo"].ToString();
            prjObj.status = prjStatus.active;
            prjObj.id = fm["hid"].ToString();
            prjObj.amt = numberUtils.getInt(fm["amt"]);
            prjObj.grossProfit = numberUtils.getDecimal(fm["grossProfit"]);
            List<string> errList = new List<string>();

            if (string.IsNullOrEmpty(prjObj.id))
            {
                prjObj.id = Guid.NewGuid().ToString();
                rep = new prjRep(prjObj.creator);
                JArray userList = new JArray();
                try
                {
                    userList = JArray.Parse(fm["userList"].ToString());
                }
                catch(Exception ex)
                {
                    userList = new JArray();
                }
                string rExec=rep.createPrj(prjObj,userList);
                if (!string.IsNullOrEmpty(rExec))
                {
                    errList.Add(rExec);
                }
            }
            else
            {
                rep = new prjRep(prjObj.creator);
                JArray userList = new JArray();
                try
                {
                    userList = JArray.Parse(fm["userList"].ToString());
                }
                catch (Exception ex)
                {
                    userList = new JArray();
                }
                string rExec = rep.updatePrj(prjObj, userList);
                if (!string.IsNullOrEmpty(rExec))
                {
                    errList.Add(rExec);
                }
            }

            dynamic r = new ExpandoObject();
            if (errList.Count()==0)
            {
                r.success = true;
                r.id = prjObj.id;
            }
            else
            {
                r.success = false;
                r.message = string.Join(",", errList);
            }
            return Content(JsonConvert.SerializeObject(r), "application/json");
        }

        [HttpGet]
        public ActionResult CreateWeekRpt(int rType=1)
        {
            return View();
        }

        public ActionResult initEmpList()
        {
            rep = new prjRep(User.Identity.Name);

            List<dynamic> list = new List<dynamic>();

            int i = 1;
            foreach(var item in rep.defaultOwnerList())
            {
                dynamic eitem = new ExpandoObject();
                eitem.seq = i;
                eitem.title = item.Value;
                eitem.workNo = "";
                eitem.perm = "1.全部";
                list.Add(eitem);
                i++;
            }
            return Content(JsonConvert.SerializeObject(list), "application/json");
        }

        public ActionResult empList()
        {
            rep = new prjRep(User.Identity.Name);
            return Content(rep.empList(), "application/json");
        }
    }
}