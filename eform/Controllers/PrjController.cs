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

namespace eform.Controllers
{
    [Authorize]
    public class PrjController : Controller
    {
        prjRep rep;
        ApplicationDbContext ctx;
        SqlConnection con;
        public PrjController()
        {
            ctx = new ApplicationDbContext();
            string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            con = new SqlConnection(constring);
        }

        string getDateRangeStrFromWeek(int y, int w)
        {
            DateTime start = new DateTime(y, 1, 4);
            start = start.AddDays(-((int)start.DayOfWeek));
            DateTime end = start.AddDays(7 * (w - 1));

            return start.ToString("yyyy/MM/dd") + "  ~  " + end.ToString("yyyy/MM/dd");
        }

        [HttpGet]
        public ActionResult ForumUsers()
        {
            string sql = "select a.* from prjCodes a left join AspNetUsers b on a.owner=b.workNo ";
            sql += " where a.status=@status and a.owner=@owner order by a.code asc";
            List<vwPrjCode> list = con.Query<vwPrjCode>(sql, new { status = "使用中",owner=User.Identity.Name }).ToList<vwPrjCode>();
            return View(list);
        }

        [HttpGet]
        public ActionResult portal(string code)
        {

            PrjPortal model = new PrjPortal();
            if (code!="")
            {
                model.prjCode = code;
            }
            else
            {
                model.prjCode = ctx.prjCodeList.OrderBy(x => x.code).FirstOrDefault().code;
                
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult ForumUserList(string id)
        {
            prjCode prjCodeObj = ctx.prjCodeList.Where(x => x.id == id).FirstOrDefault();
            ViewBag.prjCaption = prjCodeObj.code + "-" + prjCodeObj.nm;
            ViewBag.prjid = prjCodeObj.id;

            string sql = "select a.*,b.cName from prjForumUsers a left join AspNetUsers b on a.WorkNo=b.workNo ";
            sql += " where a.pid=@pid";
            List<vwPrjForumUser> list = con.Query<vwPrjForumUser>(sql, new { pid= id }).ToList<vwPrjForumUser>();
            return View(list);
        }

        [HttpGet]
        public ActionResult AddForumUser(string id)
        {
            vwPrjForumUser model = new vwPrjForumUser();
            prjCode prjCodeObj = ctx.prjCodeList.Where(x => x.id == id).FirstOrDefault();
            ViewBag.prjCaption = prjCodeObj.code + "-" + prjCodeObj.nm;
            model.pid = id;
            ViewBag.userlist = ctx.getUserList();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddForumUser(vwPrjForumUser model)
        {
            prjCode prjCodeObj = ctx.prjCodeList.Where(x => x.id == model.pid).FirstOrDefault();
            ViewBag.prjCaption = prjCodeObj.code + "-" + prjCodeObj.nm;
            ViewBag.userlist = ctx.getUserList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (ctx.prjForumUserList.Where(x=>x.pid==model.pid && x.WorkNo==model.WorkNo).Count()==0)
            {
                prjForumUser addModel = new prjForumUser();
                addModel.pid = model.pid;
                addModel.id = Guid.NewGuid().ToString();
                addModel.WorkNo = model.WorkNo;
                ctx.prjForumUserList.Add(addModel);
                ctx.SaveChanges();
                return RedirectToAction("ForumUserList", "Prj", new { id = model.pid });
            }


            return View(model);
        }

        [HttpGet]
        public ActionResult createPrjWork()
        {
            rep = new prjRep(User.Identity.Name);
            ViewBag.userlist = ctx.getUserList();
            ViewBag.prjCodelist = rep.getPrjCodeList();
            vwPrjWork model = new vwPrjWork
            {
                y = DateTime.Today.Year,
                w = dateUtils.GetIso8601WeekOfYear(DateTime.Today),
                workNo = User.Identity.Name
            };
            if (string.IsNullOrEmpty(model.DateRange))
            {
                model.DateRange = getDateRangeStrFromWeek(model.y, model.w);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult SignPrjWork(string id)
        {
            if (TempData["msg"]!=null)
            {
                ViewBag.msg = "儲存完成";
            };
            rep = new prjRep(User.Identity.Name);
            ViewBag.userlist = ctx.getUserList();
            ViewBag.prjCodelist = rep.getPrjCodeList();

            prjWork pModel = ctx.prjWorkList.Where(x => x.id == id).FirstOrDefault();

            vwPrjWork model = new vwPrjWork
            {
                id = pModel.id,
                y = pModel.y,
                w = pModel.w,
                workNo = pModel.workNo
            };
            model.DateRange = getDateRangeStrFromWeek(model.y, model.w);

            List<prjWorkItem> workItemList = ctx.prjWorkItemList.Where(x => x.pid == pModel.id).ToList<prjWorkItem>();
            JArray jList = new JArray();
            var checker= ctx.getUserByWorkNo(User.Identity.Name);
            ViewBag.mgrCode = checker.workNo;
            ViewBag.mgrNm = checker.UserCName;
            foreach (prjWorkItem item in workItemList)
            {
                JObject jItem = new JObject();
                jItem["itemId"] = item.id;
                jItem["dt"] = Convert.ToDateTime(item.dt).ToString("yyyy-MM-dd");
                jItem["prjCode"] = item.prjCode;
                jItem["hours"] = item.hours;
                jItem["subject"] = item.subject;
                jItem["smemo"] = item.smemo;
                jItem["pm"] = item.pm;
                jItem["gm"] = item.gm;
                jItem["mgr1"] = item.mgr1;
                jItem["mgr2"] = item.mgr2;
                jItem["mgr3"] = item.mgr3;
                jList.Add(jItem);
            }
            model.hList = jList.ToString();
            return View(model);
        }

        [HttpGet]
        public ActionResult editPrjWork(string id)
        {
            rep = new prjRep(User.Identity.Name);
            ViewBag.userlist = ctx.getUserList();
            ViewBag.prjCodelist = rep.getPrjCodeList();

            prjWork pModel = ctx.prjWorkList.Where(x => x.id == id).FirstOrDefault();

            vwPrjWork model = new vwPrjWork
            {
                id = pModel.id,
                y = pModel.y,
                w = pModel.w,
                workNo = pModel.workNo
            };
            model.DateRange = getDateRangeStrFromWeek(model.y, model.w);

            List<prjWorkItem> workItemList = ctx.prjWorkItemList.Where(x => x.pid == pModel.id).ToList<prjWorkItem>();
            JArray jList = new JArray();
            foreach (prjWorkItem item in workItemList)
            {
                JObject jItem = new JObject();
                jItem["itemId"] = item.id;
                jItem["dt"] = Convert.ToDateTime(item.dt).ToString("yyyy-MM-dd");
                jItem["prjCode"] = item.prjCode;
                jItem["hours"] = item.hours;
                jItem["subject"] = item.subject;
                jItem["smemo"] = item.smemo;
                jItem["pm"] = item.pm;
                jItem["gm"] = item.gm;
                jItem["mgr1"] = item.mgr1;
                jItem["mgr2"] = item.mgr2;
                jItem["mgr3"] = item.mgr3;
                jList.Add(jItem);
            }
            model.hList = jList.ToString();
            return View("CreatePrjWork", model);
        }
        [HttpPost]
        public ActionResult createPrjWork(vwPrjWork model)
        {
            rep = new prjRep(User.Identity.Name);
            ViewBag.userlist = ctx.getUserList();
            ViewBag.prjCodelist = rep.getPrjCodeList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.y < 2017)
            {
                ModelState.AddModelError("y", "年度輸入有誤");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.id))
            {
                if (ctx.prjWorkList.Where(x => x.workNo == model.workNo && x.y == model.y && x.w == model.w).Count() > 0)
                {
                    ModelState.AddModelError("", "此週已有資料，請勿重複輸入");
                    return View(model);
                }
            }

            List<string> errList = new List<string>();

            JArray removeList = new JArray();
            try
            {
                removeList = JArray.Parse(model.hRemoveList);
            }
            catch (Exception ex)
            {

            }
            List<prjWorkItem> prjWorkItemList = ctx.prjWorkItemList.ToList<prjWorkItem>();
            foreach (string obj in removeList)
            {
                prjWorkItem pObj = prjWorkItemList.Where(x => x.id == obj).FirstOrDefault();
                if (pObj!=null)
                {
                    if (  (!string.IsNullOrEmpty(pObj.mgr1))
                       || (!string.IsNullOrEmpty(pObj.mgr2))
                       || (!string.IsNullOrEmpty(pObj.mgr3))
                       || (!string.IsNullOrEmpty(pObj.pm))
                       || (!string.IsNullOrEmpty(pObj.gm))
                    )
                    {
                        errList.Add("已經簽核工作項目不可刪除");
                        break;
                    }
                }
            }

            JArray list = new JArray();
            try
            {
                list = JArray.Parse(model.hList);
            }
            catch (Exception ex)
            {

            }
            foreach (JObject obj in list)
            {
                if (string.IsNullOrEmpty(obj["dt"].Value<String>()))
                {
                    errList.Add("日期不可為空");
                    break;
                }
                if (!dateUtils.parseDate(obj["dt"].Value<String>()))
                {
                    errList.Add("日期輸入值有誤");
                    break;
                }
                DateTime dt = Convert.ToDateTime(obj["dt"].Value<String>());
                if (dateUtils.GetIso8601WeekOfYear(dt) != model.w)
                {
                    errList.Add("輸入日期必須在該週別");
                    break;
                }

                if (string.IsNullOrEmpty(obj["prjCode"].Value<String>()))
                {
                    errList.Add("工時代碼不可為空");
                    break;
                }

                if (numberUtils.getDecimal(obj["hours"].Value<String>()) <= 0)
                {
                    errList.Add("小時數必須大於0");
                    break;
                }


                if (string.IsNullOrEmpty(obj["subject"].Value<String>()))
                {
                    errList.Add("工作內容摘要不可為空");
                    break;
                }

            }

            if (errList.Count > 0)
            {
                List<prjWorkItem> workItemList = ctx.prjWorkItemList.Where(x => x.pid == model.id).ToList<prjWorkItem>();
                JArray jList = new JArray();
                foreach (prjWorkItem item in workItemList)
                {
                    JObject jItem = new JObject();
                    jItem["itemId"] = item.id;
                    jItem["dt"] = Convert.ToDateTime(item.dt).ToString("yyyy-MM-dd");
                    jItem["prjCode"] = item.prjCode;
                    jItem["hours"] = item.hours;
                    jItem["subject"] = item.subject;
                    jItem["smemo"] = item.smemo;
                    jItem["pm"] = item.pm;
                    jItem["gm"] = item.gm;
                    jItem["mgr1"] = item.mgr1;
                    jItem["mgr2"] = item.mgr2;
                    jItem["mgr3"] = item.mgr3;
                    jList.Add(jItem);
                }
                ModelState.AddModelError("", string.Join(",", errList.ToArray<String>()));
                model.hList = jList.ToString();
                return View(model);
            }

            try
            {
                string pid = "";
                if (string.IsNullOrEmpty(model.id))
                {
                    prjWork p = new prjWork
                    {
                        id = Guid.NewGuid().ToString(),
                        workNo = model.workNo,
                        y = model.y,
                        w = model.w
                    };
                    pid = p.id;
                    ctx.prjWorkList.Add(p);
                    ctx.SaveChanges();
                }
                else
                {
                    pid = model.id;
                }

                foreach (string obj in removeList)
                {
                    try
                    { 
                        ctx.prjWorkItemList.Remove(ctx.prjWorkItemList.Where(x => x.id == obj).FirstOrDefault());
                        ctx.SaveChanges();
                    }
                    catch(Exception exRemoveWorkItem)
                    {
                    }
                }

                foreach (JObject obj in list)
                {
                    if (string.IsNullOrEmpty(obj["itemId"].Value<String>()))
                    {
                        prjWorkItem pItem = new prjWorkItem
                        {
                            pid = pid,
                            id = Guid.NewGuid().ToString(),
                            dt = Convert.ToDateTime(obj["dt"].Value<String>()),
                            prjCode = obj["prjCode"].Value<String>(),
                            hours = numberUtils.getDecimal(obj["hours"].Value<String>()),
                            subject = obj["prjCode"].Value<String>(),
                            smemo = obj["smemo"].Value<String>()
                        };
                        ctx.prjWorkItemList.Add(pItem);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        string itemId = obj["itemId"].Value<String>();
                        prjWorkItem pItem = ctx.prjWorkItemList.Where(x => x.id == itemId).FirstOrDefault();
                        if (pItem != null)
                        {
                            pItem.pid = pid;
                            pItem.dt = Convert.ToDateTime(obj["dt"].Value<String>());
                            pItem.prjCode = obj["prjCode"].Value<String>();
                            pItem.hours = numberUtils.getDecimal(obj["hours"].Value<String>());
                            pItem.subject = obj["subject"].Value<String>();
                            pItem.smemo = obj["smemo"].Value<String>();
                            ctx.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

            return RedirectToAction("MyPrjWorkList");

        }

        [HttpPost]
        public ActionResult signPrjWork(vwPrjWork model)
        {
            rep = new prjRep(User.Identity.Name);
            ViewBag.userlist = ctx.getUserList();
            ViewBag.prjCodelist = rep.getPrjCodeList();

            List<string> errList = new List<string>();
            if (User.Identity.Name.ToUpper().Contains("ADMIN"))
            {
                //errList.Add("系統管理員不涉及簽核流程，請用其他使用者進行");
            }

            if (errList.Count()>0)
            {
                ModelState.AddModelError("", string.Join("<br/>", errList));
                List<prjWorkItem> workItemList = ctx.prjWorkItemList.Where(x => x.pid == model.id).ToList<prjWorkItem>();
                JArray jList = new JArray();
                foreach (prjWorkItem item in workItemList)
                {
                    JObject jItem = new JObject();
                    jItem["itemId"] = item.id;
                    jItem["dt"] = Convert.ToDateTime(item.dt).ToString("yyyy-MM-dd");
                    jItem["prjCode"] = item.prjCode;
                    jItem["hours"] = item.hours;
                    jItem["subject"] = item.subject;
                    jItem["smemo"] = item.smemo;
                    jItem["pm"] = item.pm;
                    jItem["gm"] = item.gm;
                    jItem["mgr1"] = item.mgr1;
                    jItem["mgr2"] = item.mgr2;
                    jItem["mgr3"] = item.mgr3;
                    jList.Add(jItem);
                }
                model.hList = jList.ToString();
                return View(model);
            }

            JArray list = new JArray();
            try
            {
                list = JArray.Parse(model.hList);
            }
            catch (Exception ex)
            {

            }

            try
            {
                string pid = "";
              
                pid = model.id;

                foreach (JObject obj in list)
                {
                    string itemId = obj["itemId"].Value<String>();
                    prjWorkItem pItem = ctx.prjWorkItemList.Where(x => x.id == itemId).FirstOrDefault();
                    if (pItem != null)
                    {
                        pItem.pid = pid;
                        //pItem.dt = Convert.ToDateTime(obj["dt"].Value<String>());
                        //pItem.prjCode = obj["prjCode"].Value<String>();
                        //pItem.hours = numberUtils.getDecimal(obj["hours"].Value<String>());
                        //pItem.subject = obj["subject"].Value<String>();
                        //pItem.smemo = obj["smemo"].Value<String>();
                        if (obj["chkpm"].Value<Boolean>() ) //&& string.IsNullOrEmpty(pItem.pm))
                        {
                            pItem.pm = User.Identity.Name;
                        }
                        else
                        {
                            pItem.pm = "";
                        }
                        if (obj["chkgm"].Value<Boolean>()) //&& string.IsNullOrEmpty(pItem.gm))
                        {
                            pItem.gm = User.Identity.Name;
                        }
                        else
                        {
                            pItem.gm = "";
                        }
                        if (obj["chkmgr1"].Value<Boolean>()) //&& string.IsNullOrEmpty(pItem.mgr1))
                        {
                            pItem.mgr1 = User.Identity.Name;
                        }
                        else
                        {
                            pItem.mgr1 = "";
                        }
                        if (obj["chkmgr2"].Value<Boolean>()) //&& string.IsNullOrEmpty(pItem.mgr2))
                        {
                            pItem.mgr2 = User.Identity.Name;
                        }
                        else
                        {
                            pItem.mgr2 = "";
                        }
                        if (obj["chkmgr3"].Value<Boolean>()) //&& string.IsNullOrEmpty(pItem.mgr3))
                        {
                            pItem.mgr3 = User.Identity.Name;
                        }
                        else
                        {
                            pItem.mgr3 = "";
                        }
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

            TempData["msg"] = "儲存完成";
            return RedirectToAction("signPrjWork",new {id=model.id});

        }


        public ActionResult Index()
        {
            prj prjObj = new prj();
            var emp = ctx.getCurrentUser(User.Identity.Name);
            prjObj.creator = emp.workNo;
            prjObj.creatorNm = emp.UserCName;
            prjObj.createDate = ctx.getLocalTiime();
            return View(prjObj);
        }
        void initViewBag()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Selected = true,
                Value = "使用中",
                Text = "使用中"
            });
            items.Add(new SelectListItem
            {
                Value = "關閉",
                Text = "關閉"
            });
            ViewBag.statusList = items;
            ViewBag.userlist = ctx.getUserList();
        }
        [HttpGet]
        public ActionResult CodeList()
        {
            vwPrjCode prjCodeObj = new vwPrjCode();
            var emp = ctx.getCurrentUser(User.Identity.Name);
            prjCodeObj.creator = emp.workNo;
            prjCodeObj.createDate = ctx.getLocalTiime();
            prjCodeObj.hPMList = "";
            initViewBag();
            return View(prjCodeObj);
        }
        [HttpGet]
        public ActionResult MyPrjWorkList()
        {
            var emp = ctx.getCurrentUser(User.Identity.Name);
            var list = ctx.prjWorkList.Where(x => x.workNo == emp.workNo).ToList<prjWork>().OrderByDescending(x => x.y).OrderByDescending(x => x.w);
            var model = new List<vwPrjWork>();
            foreach (var prj in list)
            {
                var mPrj = new vwPrjWork
                {
                    id = prj.id,
                    workNo = prj.workNo,
                    y = prj.y,
                    w = prj.w
                };
                model.Add(mPrj);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult MgrPrjWorkList()
        {
            hrRep hRep = new hrRep(User.Identity.Name);
            List<prjWork> plist = new List<prjWork>();
            List<String> roles = new List<string>();

            if (hRep.isGMDep())
            {
                plist = ctx.prjWorkList.ToList<prjWork>();
                roles.Add("GM");
            }
            else
            {
                List<String> PMPrjList = (from p in ctx.prjCodeList.Where(x => x.owner == User.Identity.Name) select p.code).ToList<String>();
                if (PMPrjList.Count() > 0)
                {
                    roles.Add("PM");
                    List<String> PrjIDList = (from p in ctx.prjWorkItemList.Where(x => PMPrjList.Contains(x.prjCode)) select p.pid).ToList<String>();
                    plist = ctx.prjWorkList.Where(x => PrjIDList.Contains(x.id)).ToList<prjWork>();
                }
            }
            if (hRep.isMgr() && (!roles.Contains("GM")))
            {
                List<String> MgrDepList = hRep.getDepList();
                foreach (prjWork item in ctx.prjWorkList)
                {
                    List<String> depList = hRep.getDepListByWorkNo(item.workNo);
                    if (depList.Intersect(MgrDepList).Count() > 0)
                    {
                        if (plist.Where(x=>x.id==item.id).Count()==0)
                        { 
                            plist.Add(item);
                        }
                    }
                }
                if (plist.Count > 0)
                {
                    roles.Add("MGR");
                }
            }

            var model = new List<vwPrjWork>();
            plist = plist.OrderByDescending(x => x.y).OrderByDescending(x => x.w).ToList<prjWork>();
            foreach (var prj in plist)
            {
                var mPrj = new vwPrjWork
                {
                    id = prj.id,
                    workNo = prj.workNo,
                    y = prj.y,
                    w = prj.w
                };
                model.Add(mPrj);
            }
            return View(model);
        }

        public ActionResult prjDetail(string prjId)
        {
            rep = new prjRep(User.Identity.Name);
            dynamic r = new ExpandoObject();
            r.prjId = prjId;
            prj prjObj = rep.loadPrj(prjId);
            r.prj = prjObj;
            r.empList = rep.loadPrjEmpList(prjObj.id);
            return Content(JsonConvert.SerializeObject(r), "application/json");
        }
        public ActionResult prjList()
        {
            rep = new prjRep(User.Identity.Name);
            return Content(JsonConvert.SerializeObject(rep.prjList()), "application/json");
        }
        public ActionResult prjCodeList()
        {
            rep = new prjRep(User.Identity.Name);

            return Content(JsonConvert.SerializeObject(rep.prjCodeList()), "application/json");
        }
        public ActionResult prjCodeDetail(string id)
        {
            rep = new prjRep(User.Identity.Name);
            dynamic r = new ExpandoObject();
            vwPrjCode prjCodeObj = rep.loadPrjCode(id);

            var pmList = ctx.prjPMList.Where(x => x.pid == id);
            JArray jList = new JArray();
            foreach (prjPM pm in pmList)
            {
                JObject jItem = new JObject();
                jItem["pm"] = pm.WorkNo;
                jList.Add(jItem);
            }

            prjCodeObj.hPMList =jList.ToString();
            r.prjCode = prjCodeObj;
            return Content(JsonConvert.SerializeObject(r), "application/json");
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
                catch (Exception ex)
                {
                    userList = new JArray();
                }
                string rExec = rep.createPrj(prjObj, userList);
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
            if (errList.Count() == 0)
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
        [HttpPost]
        public ActionResult CreatePrjCode(FormCollection fm)
        {
            prjCode prjObj = new prjCode();
            prjObj.owner = fm["owner"].ToString();
            prjObj.code = fm["code"].ToString();
            prjObj.nm = fm["nm"].ToString();
            prjObj.status = fm["status"].ToString();
            prjObj.owner = fm["owner"].ToString();
            prjObj.mmo1 = fm["mmo1"].ToString();
            prjObj.mmo2 = fm["mmo2"].ToString();
            prjObj.mmo3 = fm["mmo3"].ToString();
            prjObj.mmo4 = fm["mmo4"].ToString();
            prjObj.mmo5 = fm["mmo5"].ToString();
            prjObj.mmo6 = fm["mmo6"].ToString();
            prjObj.mmo7 = fm["mmo7"].ToString();
            prjObj.mmo8 = fm["mmo8"].ToString();
            prjObj.mmo9 = fm["mmo9"].ToString();
            prjObj.mmo10 = fm["mmo10"].ToString();
            prjObj.id = fm["hid"].ToString();

            List<string> errList = new List<string>();

            if (string.IsNullOrEmpty(prjObj.id))
            {
                prjObj.id = Guid.NewGuid().ToString();
                rep = new prjRep(fm["hUserId"].ToString());

                string rExec = rep.createPrjCode(prjObj, User.Identity.Name);
                if (!string.IsNullOrEmpty(rExec))
                {
                    errList.Add(rExec);
                }
            }
            else
            {
                rep = new prjRep(fm["hUserId"].ToString());
                string rExec = rep.updatePrjCode(prjObj, User.Identity.Name);
                if (!string.IsNullOrEmpty(rExec))
                {
                    errList.Add(rExec);
                }
            }

            if (errList.Count()==0)
            {
                JArray pmlist = new JArray();
                try
                {
                    pmlist=JArray.Parse(fm["hPMList"].ToString());
                }
                catch(Exception exPMList)
                {

                }
                ctx.prjPMList.RemoveRange(ctx.prjPMList.Where(x => x.pid == prjObj.id));
                ctx.SaveChanges();
                foreach (var pm in pmlist)
                {
                    prjPM pmObj = new prjPM
                    {
                        id = Guid.NewGuid().ToString(),
                        pid = prjObj.id,
                        WorkNo = pm["pm"].ToString()
                    };
                    ctx.prjPMList.Add(pmObj);
                    ctx.SaveChanges();
                }
            }

            dynamic r = new ExpandoObject();
            if (errList.Count() == 0)
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
        public ActionResult CreateWeekRpt(int rType = 1)
        {
            return View();
        }
        public ActionResult initEmpList()
        {
            rep = new prjRep(User.Identity.Name);
            List<dynamic> list = new List<dynamic>();
            int i = 1;
            foreach (var item in rep.defaultOwnerList())
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

        string getEventPrjWorks()
        {
            string r = "";

            DateTime today = DateTime.Today;
            List<dynamic> dataList = new List<dynamic>();
            int idx = 1;
            List<prjCode> prjCodeList = ctx.prjCodeList.ToList<prjCode>();
            foreach (prjWorkItem item in ctx.prjWorkItemList.ToList<prjWorkItem>())
            {
                dynamic obj = new ExpandoObject();
                obj.id = idx;
                string prjWorkNm = "";
                try
                {
                    prjWorkNm=prjCodeList.Where(x => x.code == item.prjCode).FirstOrDefault().nm;
                }
                catch(Exception exGetPrjCodeNm)
                {
                    prjWorkNm = "";
                }
                obj.title = prjWorkNm + "-" +item.subject + "-" + item.hours.ToString("0.0") + "小時";
                obj.start = item.dt;
                obj.end = item.dt;
                dataList.Add(obj);
                idx++;
            }
            return JsonConvert.SerializeObject(dataList);
        }

        public ActionResult Home()
        {
            ViewBag.eventPrjWorks = getEventPrjWorks();

            return View();
        }
    }
}