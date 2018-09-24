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
            prjObj.beginDate = ctx.getLocalTiime();


            return View(prjObj);
        }

        [HttpPost]
        public ActionResult Create(FormCollection fm)
        {
            prj prjObj = new prj();
            prjObj.creator = Request.Form["hcreator"].ToString();
            prjObj.prjId = Request.Form["prjId"].ToString();
            prjObj.nm = Request.Form["nm"].ToString();
            prjObj.custId = Request.Form["custId"].ToString();
            prjObj.custNm = Request.Form["custNm"].ToString();
            prjObj.beginDate = Convert.ToDateTime(Request.Form["beginDate"].ToString());
            prjObj.endDate = Convert.ToDateTime(Request.Form["endDate"].ToString());
            prjObj.sMemo = Request.Form["sMemo"].ToString();
            prjObj.dtMemo = Request.Form["dtMemo"].ToString();
            prjObj.status = (prjStatus)Enum.Parse(typeof(prjStatus), Request.Form["status"].ToString(), false);
            prjObj.id= Request.Form["id"].ToString();
            List<string> errList = new List<string>();

            if (string.IsNullOrEmpty(prjObj.id))
            {
                prjObj.id = Guid.NewGuid().ToString();
                rep = new prjRep(prjObj.creator);
                string rExec=rep.createPrj(prjObj);
                if (!string.IsNullOrEmpty(rExec))
                {
                    errList.Add(rExec);
                }
            }
            else
            {
                //do update
            }

            dynamic r = new ExpandoObject();
            if (errList.Count()==0)
            {
                r.success = true;
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

        public ActionResult empList()
        {
            rep = new prjRep(User.Identity.Name);
            return Content(rep.empList(), "application/json");
        }
    }
}