using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using System.Data.Entity.Infrastructure;
using eform.Attributes;
using System.Dynamic;
using Newtonsoft.Json;

namespace eform.Controllers
{
    [AdminAuthorize(Roles ="Admin")]
    public class FlowDefController : Controller
    {
        // GET: FlowDef
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();

            var model = context.FlowDefMainList.OrderBy(x => x.seq);

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FlowDefMain Model)
        {
            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            var context = new ApplicationDbContext();

            if (context.FlowDefMainList.Where(x=>x.nm==Model.nm).Count()>0)
            {
                ModelState.AddModelError("nm", "表單名稱不可重複");
                return View(Model);
            }

            Model.id = Guid.NewGuid().ToString();
            context.FlowDefMainList.Add(Model);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        void getUserList(ApplicationDbContext context)
        {
            var userlist0 = context.Users.Where(x =>x.status==1 && x.UserName.ToLower().Contains("admin") == false).ToList();
            List<dynamic> userlist = new List<dynamic>();
            foreach (var user in userlist0)
            {
                dynamic obj = new ExpandoObject();
                obj.id = user.workNo;
                obj.text = user.cName;
                userlist.Add(obj);
            }
            dynamic userSelTitle = new ExpandoObject();
            userSelTitle.id = ""; userSelTitle.text = "請選擇人員";
            userlist.Insert(0, userSelTitle);

            ViewBag.userlist = JsonConvert.SerializeObject(userlist);
        }

        void getFlowMainData(ApplicationDbContext context,string id)
        {
            ViewBag.FlowDefId = id;
            FlowDefMain main = context.FlowDefMainList.Where(x => x.id == id).FirstOrDefault();
            ViewBag.FlowDefName = main.nm;
        }

        [HttpGet]
        public ActionResult SetupFlow(string id)
        {
            var context = new ApplicationDbContext();

            getFlowMainData(context, id);

            List<FlowDefSub> list = context.FlowDefSubList.Where(x => x.pid == id).ToList<FlowDefSub>();

            List<vwFlowDefSub> vwList = new List<vwFlowDefSub>();

            foreach(var item in list)
            {
                vwList.Add(new vwFlowDefSub
                {
                    id = item.id,
                    pid = item.pid,
                    seq = item.seq,
                    signType = item.signType,
                    workNo = item.workNo,
                    UserCName = context.Users.Where(x => x.workNo == item.workNo).FirstOrDefault().cName
                });
            }

            //ViewBag.FlowDefSub = new FlowDefSub { pid = id, id = "", seq = 1, signType = 0, workNo = "" };
            //getUserList(context);

            return View(vwList);
        }

        [HttpGet]
        public ActionResult AddDefSub(string pid)
        {
            var context = new ApplicationDbContext();

            getFlowMainData(context, pid);

            getUserList(context);

            FlowDefSub DefSub = new FlowDefSub { pid = pid,id="", seq = 1, signType = 0, workNo = "" };

            ViewBag.EditMode = "Create";

            return View(DefSub);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDefSub(FlowDefSub model)
        {
            dbHelper dbh = new dbHelper();
            if(string.IsNullOrEmpty(model.id))
            {
                ViewBag.EditMode = "Create";
                model.id = Guid.NewGuid().ToString();

                if (!string.IsNullOrEmpty(model.workNo))
                {
                    int iuser = dbh.sql2count(
                            "select count(workNo) cnt from FlowDefSub where workNo='"
                            +model.workNo+"' and pid='" 
                            + model.pid + "'"
                        );
                    if (iuser>0)
                    {
                        ModelState.AddModelError("workNo", "工號重複");
                    }
                }

                if (model.signType==2)
                {
                    int iSign1=dbh.sql2count("select count(workNo) cnt from FlowDefSub where signType=1 and pid='" + model.pid + "'");
                    ModelState.AddModelError("signType", "已有設定【1.會簽】，不允許【2.會簽-全部同意】");
                }
                
            }
            else
            {
                ViewBag.EditMode = "Edit";
            }

            if (model.signType==0)
            {
                ModelState.AddModelError("signType", "請選擇簽核類別");
            }
            if (!ModelState.IsValid)
            {
                getUserList(new ApplicationDbContext());
                return View(model);
            }
            else
            {
                ApplicationDbContext context = new ApplicationDbContext();
                try
                {
                    context.FlowDefSubList.Add(model);
                    context.SaveChanges();
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    getUserList(new ApplicationDbContext());
                    return View(model);
                }
                return RedirectToAction("SetupFLow", new { id = model.pid });
            }
        }

        [HttpPost]
        public ActionResult DeleteSub(string hid,string hpid)
        {
            dbHelper dbh = new dbHelper();
            dbh.execSql("delete from FlowDefSub where id='" + hid+"'");
            return RedirectToAction("SetupFlow", new { id = hpid });
        }
    }
}