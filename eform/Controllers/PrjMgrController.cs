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
    public class PrjMgrController : Controller
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
                    seq = item.seq,
                    itemTxt = item.itemTxt,
                    dtBegin = item.dtBegin,
                    dtEnd = item.dtEnd
                });
            }


            return View(model);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}