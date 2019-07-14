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

        void setHMList()
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

        [HttpGet]
        public ActionResult CreateForumItem(string prjId, string code,bool mobile=false)
        {
            ViewBag.userlist = ctx.getUserList();
            ViewBag.code = code;
            vwForumItem model = new vwForumItem
            {
                workNo = User.Identity.Name,
                prjId = prjId,
                billDate = ctx.getLocalTiime(),
                mobile = mobile
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateForumItem(vwForumItem model)
        {
            prjCode prjCodeObj = ctx.prjCodeList.Where(x => x.id == model.prjId).FirstOrDefault();

            ViewBag.userlist = ctx.getUserList();
            ViewBag.code = prjCodeObj.code;

            if(string.IsNullOrEmpty(model.subject))
            {
                ModelState.AddModelError("subject", "請輸入標題");
            }

            if (!ModelState.IsValid)
            {
                model.workNo = User.Identity.Name;
                model.billDate = ctx.getLocalTiime();
                return View(model);
            }

            if (string.IsNullOrEmpty(model.id))
            {
                model.id = Guid.NewGuid().ToString();
            }
            model.billDate = ctx.getLocalTiime();
            model.sfile = ""; model.sfile2 = ""; model.sfile3 = ""; model.sfile4 = "";
            for (int i = 0; i < 4; i++)
            {
                if (Request.Files[i] != null)
                {
                    var file = Request.Files[i];
                    if (file.ContentLength > 0)
                    {
                        string originFileName = file.FileName;
                        string filename = file.FileName.Replace("%", "").Replace(" ", "_");
                        filename = filename.Split('.')[0] + ctx.getLocalTiime().ToString("yyMMddhhmmss") +"-"+ "." + filename.Split('.')[1];
                        string fileUrl = @"Forum/" + model.id + "/" + filename;

                        var path = @"C:\inetpub\upload\Forum\" + model.id + @"\";
                        DirectoryInfo di = new DirectoryInfo(path);
                        if (di.Exists == false)
                        {
                            di.Create();
                        }
                        path = path + @"\" + filename;
                        try
                        {
                            file.SaveAs(path);
                            switch (i)
                            {
                                case 0:
                                    model.sfile = @"Forum\" + model.id + @"\" + filename;
                                    break;
                                case 1:
                                    model.sfile2 = @"Forum\" + model.id + @"\" + filename;
                                    break;
                                case 2:
                                    model.sfile3 = @"Forum\" + model.id + @"\" + filename;
                                    break;
                                case 3:
                                    model.sfile4 = @"Forum\" + model.id + @"\" + filename;
                                    break;
                                default:
                                    break;
                            }


                        }
                        catch (Exception ex)
                        {
                            switch (i)
                            {
                                case 0:
                                    model.sfile = "";
                                    break;
                                case 1:
                                    model.sfile2 = "";
                                    break;
                                case 2:
                                    model.sfile3 = "";
                                    break;
                                case 3:
                                    model.sfile4 = "";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            ForumItem fItem = new ForumItem
            {
                id = model.id,
                prjId = model.prjId,
                seq = model.seq,
                billDate = model.billDate,
                workNo = model.workNo,
                subject = model.subject,
                smemo = model.smemo,
                othersA1 = model.othersA1,
                othersA2 = model.othersA2,
                othersA3 = model.othersA3,
                othersA4 = model.othersA4,
                othersA5 = model.othersA5,
                othersA6 = model.othersA6,
                othersB1 = model.othersB1,
                othersB2 = model.othersB2,
                othersB3 = model.othersB3,
                othersB4 = model.othersB4,
                othersB5 = model.othersB5,
                othersB6 = model.othersB6,
                sfile = model.sfile,
                sfile2 = model.sfile2,
                sfile3 = model.sfile3,
                sfile4 = model.sfile4
            };
            ctx.prjForumItems.Add(fItem);
            ctx.SaveChanges();

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("您好," + "<br/><br/>");
                vwEmployee sender = ctx.getUserByWorkNo(model.workNo);
                sb.Append(model.workNo + " " + sender.UserEName + " 發出討論文章："+"<br/>");
                prjCode prjObj = ctx.prjCodeList.Where(x => x.id == model.prjId).FirstOrDefault();
                sb.Append("討論區：" + prjObj.code + ":" + prjObj.nm + "<br/>");
                sb.Append("標題：" + model.subject + "<br/>");
                sb.Append("內容：" + model.smemo + "<br/>");
                sb.AppendFormat("<a href='{0}'>{1}</a><br/>",
                    Url.Action("ForumDetail", "Prj", new RouteValueDictionary(new { id = model.id }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority),
                    "文章網址"
                );
                sb.Append("<br/>此信件為系統發出，請勿直接回信<br/>");

                if (model.isPrivate)
                {
                    sb.Append("私訊人員：" + string.Join(",", model.mailNmAList.ToArray<string>()) + "<br/>");
                }
                else
                {
                    sb.Append("正本人員：" + string.Join(",", model.mailNmAList.ToArray<string>()) + "<br/>");
                    sb.Append("副本人員：" + string.Join(",", model.mailNmBList.ToArray<string>()));
                }

                foreach (string signer in model.mailList)
                {
                    if (!string.IsNullOrEmpty(signer))
                    {
                        List<EmailAddress> mailList = new List<EmailAddress>();
                        mailList.Add(new EmailAddress
                        {
                            Email = signer
                            //Name = emp.workNo + " " + emp.UserCName
                        });
                        await SendGridSrv.sendEmail(mailList, sender.UserEName + "發出討論文章:" + model.subject, sb.ToString());
                    }
                }

            }
            catch (Exception ex)
            {

            }

            if(model.mobile)
            {
                return RedirectToAction("portal", "Prj", new { code = prjCodeObj.code,mobile=true });
            }
            return RedirectToAction("portal", "Prj", new { code = prjCodeObj.code });
        }

        [HttpGet]
        public ActionResult ReplyForumItem(string id, string prjId, string code, string pid, Boolean isPrivate = false, bool mobile=false)
        {
            ViewBag.userlist = ctx.getUserList();
            ViewBag.code = code;
            ForumItem fItem = ctx.prjForumItems.Where(x => x.id == id).FirstOrDefault();
            string memo = fItem.smemo;

            string[] lines = new string[] {};

            if (!string.IsNullOrEmpty(memo))
            { 
                lines=memo.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.None
                );
            }
            List<String> LineList = new List<String>();
            foreach(string s in lines)
            {
                LineList.Add(":" + s);
            }
            if (LineList.Count>0)
            {
                LineList.Insert(0, string.Format("※ 引述《{0}》：", ctx.getUserByWorkNo(fItem.workNo).UserEName));
            }
            memo = string.Join(Environment.NewLine, LineList);

            if(String.IsNullOrEmpty(fItem.subject))
            {
                var fMItem= ctx.prjForumItems.Where(x => x.id == fItem.pid).FirstOrDefault();
                if(fMItem!=null)
                {
                    fItem.subject = fMItem.subject;
                }
            }

            vwForumItem model = new vwForumItem
            {
                id = "",
                workNo = User.Identity.Name,
                prjId = prjId,
                pid = pid,
                pTitle = string.IsNullOrEmpty(fItem.subject)? fItem.subTitle:fItem.subject,
                subject = string.IsNullOrEmpty(fItem.subject) ? fItem.subTitle : fItem.subject,
                billDate = ctx.getLocalTiime(),
                isPrivate = isPrivate,
                mobile=mobile,
                smemo= memo
            };
            ViewBag.mobile = mobile;
            ViewBag.pid = pid;
            return View(model);
        }

        [HttpGet]
        public ActionResult UpdateForumItem(string id, string prjId, string code, string page, Boolean isPrivate = false,bool mobile=false)
        {
            ViewBag.code = code;
            ForumItem fItem = ctx.prjForumItems.Where(x => x.id == id).FirstOrDefault();
            vwUpateForumItem model = new vwUpateForumItem
            {
                id = fItem.id,
                seq=fItem.seq,
                workNo = fItem.workNo,
                prjId = prjId,
                pid = fItem.pid,
                subject = fItem.subject,
                subTitle = fItem.subTitle,
                smemo = fItem.smemo,
                billDate = fItem.billDate,
                page = page,
                mobile=mobile
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ReplyForumItem(vwForumItem model)
        {
            prjCode prjCodeObj = ctx.prjCodeList.Where(x => x.id == model.prjId).FirstOrDefault();

            ViewBag.userlist = ctx.getUserList();
            ViewBag.code = prjCodeObj.code;

            if (string.IsNullOrEmpty(model.subTitle))
            {
                ModelState.AddModelError("subTitle", "請輸入回覆標題");
            }

            if (!ModelState.IsValid)
            {
                model.billDate = ctx.getLocalTiime();
                return View(model);
            }



            if (string.IsNullOrEmpty(model.id))
            {
                model.id = Guid.NewGuid().ToString();
            }
            model.billDate = ctx.getLocalTiime();
            model.sfile = ""; model.sfile2 = ""; model.sfile3 = ""; model.sfile4 = "";
            for (int i = 0; i < 4; i++)
            {
                if (Request.Files[i] != null)
                {
                    var file = Request.Files[i];
                    if (file.ContentLength > 0)
                    {
                        string originFileName = file.FileName;
                        string filename = file.FileName.Replace("%", "").Replace(" ", "_");
                        filename = filename.Split('.')[0] + "-" +ctx.getLocalTiime().ToString("yyMddhhmmss") + "." + filename.Split('.')[1];
                        string fileUrl = @"Forum/" + model.id + "/" + filename;

                        var path = @"C:\inetpub\upload\Forum\" + model.id + @"\";
                        DirectoryInfo di = new DirectoryInfo(path);
                        if (di.Exists == false)
                        {
                            di.Create();
                        }
                        path = path + @"\" + filename;
                        try
                        {
                            file.SaveAs(path);
                            switch (i)
                            {
                                case 0:
                                    model.sfile = @"Forum\" + model.id + @"\" + filename;
                                    break;
                                case 1:
                                    model.sfile2 = @"Forum\" + model.id + @"\" + filename;
                                    break;
                                case 2:
                                    model.sfile3 = @"Forum\" + model.id + @"\" + filename;
                                    break;
                                case 3:
                                    model.sfile4 = @"Forum\" + model.id + @"\" + filename;
                                    break;
                                default:
                                    break;
                            }


                        }
                        catch (Exception ex)
                        {
                            switch (i)
                            {
                                case 0:
                                    model.sfile = "";
                                    break;
                                case 1:
                                    model.sfile2 = "";
                                    break;
                                case 2:
                                    model.sfile3 = "";
                                    break;
                                case 3:
                                    model.sfile4 = "";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            var seqList = from item in ctx.prjForumItems where item.prjId == prjCodeObj.id && item.pid == model.pid select item.seq;
            int seqNo = 1;
            if (seqList.Count() > 0)
            {
                seqNo = seqList.Max() + 1;
            }
            ForumItem fItem = new ForumItem
            {
                id = Guid.NewGuid().ToString(),
                prjId = model.prjId,
                pid = model.pid,
                seq = seqNo,
                billDate = model.billDate,
                workNo = model.workNo,
                subTitle = model.subTitle,
                smemo = model.smemo,
                othersA1 = model.othersA1,
                othersA2 = model.othersA2,
                othersA3 = model.othersA3,
                othersA4 = model.othersA4,
                othersA5 = model.othersA5,
                othersA6 = model.othersA6,
                othersB1 = model.othersB1,
                othersB2 = model.othersB2,
                othersB3 = model.othersB3,
                othersB4 = model.othersB4,
                othersB5 = model.othersB5,
                othersB6 = model.othersB6,
                sfile = model.sfile,
                sfile2 = model.sfile2,
                sfile3 = model.sfile3,
                sfile4 = model.sfile4,
                isPrivate = model.isPrivate
            };
            ctx.prjForumItems.Add(fItem);
            ctx.SaveChanges();

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("您好," + "<br/><br/>");
                vwEmployee sender = ctx.getUserByWorkNo(model.workNo);
                sb.Append(model.workNo + " " + sender.UserEName + " 回覆:<br/>");
                prjCode prjObj = ctx.prjCodeList.Where(x => x.id == model.prjId).FirstOrDefault();
                sb.Append("討論區：" + prjObj.code + ":" + prjObj.nm + "<br/>");
                sb.Append("原文標題：" + model.pTitle + "<br/>");
                sb.Append("回文標題：" + model.subTitle + "<br/>");
                sb.Append("內容：" + model.smemo+"<br/>");
                sb.AppendFormat("<a href='{0}'>{1}</a><br/>",
                    Url.Action("ForumDetail", "Prj", new RouteValueDictionary(new { id = model.pid }), HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority),
                    "文章網址"
                );

                sb.Append("<br/>此信件為系統發出，請勿直接回信<br/>");
                if (model.isPrivate)
                {
                    sb.Append("私訊人員：" + string.Join(",", model.mailNmAList.ToArray<string>()) + "<br/>");
                }
                else
                {
                    sb.Append("正本人員：" + string.Join(",", model.mailNmAList.ToArray<string>()) + "<br/>");
                    sb.Append("副本人員：" + string.Join(",", model.mailNmBList.ToArray<string>()));
                }


                foreach (string signer in model.mailList)
                {

                    if (!string.IsNullOrEmpty(signer))
                    {
                        List<EmailAddress> mailList = new List<EmailAddress>();
                        mailList.Add(new EmailAddress
                        {
                            Email = signer
                        });
                        await SendGridSrv.sendEmail(mailList, sender.UserEName + "回覆討論文章:" + model.subTitle, sb.ToString());
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("ForumDetail", "Prj", new { id = model.pid,mobile=model.mobile });
        }

        [HttpPost]
        public ActionResult UpdateForumItem(vwUpateForumItem model)
        {
            prjCode prjCodeObj = ctx.prjCodeList.Where(x => x.id == model.prjId).FirstOrDefault();
            ViewBag.code = prjCodeObj.code;

            if (string.IsNullOrEmpty(model.subject) && model.page == "portal")
            {
                ModelState.AddModelError("subject", "請輸入標題");
            }
            else if (string.IsNullOrEmpty(model.subTitle) && model.page != "portal")
            {
                ModelState.AddModelError("subTitle", "請輸入標題");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ForumItem fItem = ctx.prjForumItems.Where(x => x.id == model.id).FirstOrDefault();
            if (model.page == "portal")
            {
                fItem.subject = model.subject;
            }
            else
            {
                fItem.subTitle = model.subTitle;
            }
            fItem.seq = model.seq;
            fItem.smemo = model.smemo;
            ctx.SaveChanges();


            if (model.page == "portal")
            {
                return RedirectToAction("portal", "Prj", new { code = prjCodeObj.code,mobile=model.mobile });
            }
            else
            {
                return RedirectToAction("ForumDetail", "Prj", new { id = model.pid, code = prjCodeObj.code,mobile=model.mobile });
            }
        }

        [HttpGet]
        public ActionResult CreatePrjPM(string pid, string code)
        {
            ViewBag.userlist = ctx.getUserList();
            ViewBag.code = code;
            prjPM model = new prjPM
            {
                pid = pid
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult UpdatePrjPM(string id, string code)
        {
            prjPM model = ctx.prjPMList.Where(x => x.id == id).FirstOrDefault();
            ViewBag.PMName = ctx.getUserByWorkNo(model.WorkNo).UserEName;
            ViewBag.code = code;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePrjPM(prjPM model)
        {
            ViewBag.userlist = ctx.getUserList();
            prjCode prjCodeObj = ctx.prjCodeList.Where(x => x.id == model.pid).FirstOrDefault();
            ViewBag.code = prjCodeObj.code;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (string.IsNullOrEmpty(model.id))
            {
                model.id = Guid.NewGuid().ToString();
            }
            ctx.prjPMList.Add(model);
            ctx.SaveChanges();
            return RedirectToAction("portal", "Prj", new { code = prjCodeObj.code });
        }

        [HttpPost]
        public ActionResult UpdatePrjPM(prjPM model)
        {
            ViewBag.PMName = ctx.getUserByWorkNo(model.WorkNo).UserEName;
            prjCode prjCodeObj = ctx.prjCodeList.Where(x => x.id == model.pid).FirstOrDefault();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            prjPM pmUser = ctx.prjPMList.Where(x => x.id == model.id).FirstOrDefault();
            pmUser.Title = model.Title;
            ctx.SaveChanges();
            return RedirectToAction("portal", "Prj", new { code = prjCodeObj.code });
        }

        [HttpPost]
        public ActionResult DelPrjPM(string hid, string code)
        {
            foreach (var pm in ctx.prjPMList)
            {
                if (pm.id == hid )
                {
                    ctx.prjPMList.Remove(pm);
                    break;
                }
            }
            ctx.SaveChanges();
            return RedirectToAction("portal", "Prj", new { code = code });
        }

        [HttpGet]
        public ActionResult ForumUsers()
        {
            string sql = "select a.* from prjCodes a left join AspNetUsers b on a.owner=b.workNo ";
            sql += " where a.status=@status and a.owner=@owner order by a.code asc";
            List<vwPrjCode> list = con.Query<vwPrjCode>(sql, new { status = "使用中", owner = User.Identity.Name }).ToList<vwPrjCode>();
            return View(list);
        }

        [HttpGet]
        public ActionResult DeleteForumItem(string id, string code, string page, string pid = "",bool mobile=false)
        {
            ctx.prjForumItems.Remove(ctx.prjForumItems.Where(x => x.id == id).FirstOrDefault());
            ctx.SaveChanges();
            if (page.ToLower() == "portal")
            {
                return RedirectToAction("portal", new { code = code,mobile=mobile });
            }
            else
            {
                return RedirectToAction("ForumDetail", new { id = pid,mobile=mobile });

            }
        }

        [HttpGet]
        public ActionResult myPrjs(bool mobile=false)
        {
            List<prjCode> prjCodeList = new List<Models.prjCode>();
            var orgList = ctx.prjCodeList.Where(x=>x.status.Equals("使用中")).OrderBy(x => x.code).ToList<prjCode>();
            List <string> workNoList = new List<string>();
            foreach (prjCode prj in orgList)
            {
                if (User.Identity.Name == prj.owner || User.Identity.Name == prj.creator || User.Identity.Name=="sadmin")
                {
                    prjCodeList.Add(prj);
                }
                else
                {
                    if (ctx.prjPMList.Where(x => x.pid == prj.id && x.WorkNo == User.Identity.Name).Count() > 0)
                    {
                        prjCodeList.Add(prj);
                    }
                }
            }

            List<vwPrjCode> prjList = new List<vwPrjCode>();
            foreach(prjCode prj in prjCodeList)
            {
                vwPrjCode p = new vwPrjCode
                {
                    id = prj.id,
                    code = prj.code,
                    nm = prj.nm,
                    owner = prj.owner,
                    createDate = prj.createDate,
                    contractDate=prj.contractDate
                };
                prjList.Add(p);
            }

            prjList = prjList.OrderByDescending(x => Convert.ToDateTime(x.contractDate)).ToList<vwPrjCode>();

            //var selUserList= ctx.Users.Where(x => workNoList.Contains(x.workNo)).ToList<ApplicationUser>();


            ViewBag.mobile = mobile;
            return View(prjList);
        }

        [HttpGet]
        public ActionResult portal(string code = "",bool mobile=false)
        {
            PrjPortal model = new PrjPortal(User.Identity.Name);
            if (code != "")
            {
                model.prjCode = code;
            }
            else
            {
                model.prjCode = ctx.prjCodeList.OrderBy(x => x.code).FirstOrDefault().code;

            }

            List<string> prjUsers = new List<string>();
            prjUsers.Add(model.prjCodeObj.creator);
            prjUsers.Add(model.prjCodeObj.owner);

            foreach (var pm in model.PMList)
            {
               prjUsers.Add(pm.WorkNo);
            }

            if (prjUsers.Where(x => x == User.Identity.Name).Count() == 0 && (User.Identity.Name !="sadmin"))
            {
                return View("AccessDenied");
            }


            if (model.prjCodeObj != null)
            {
                model.prjEventList = ctx.prjEventList.Where(x => x.prjId == model.prjCodeObj.id).OrderBy(x => x.billNo).ToList<PrjEvent>();
            }

            ViewBag.mobile = mobile;
            if (mobile)
            {
                return View("portalMobile",model);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ForumDetail(string id, string code,bool mobile=false)
        {
            if (string.IsNullOrEmpty(code))
            {
                ForumItem fitem = ctx.prjForumItems.Where(x => x.id == id).FirstOrDefault();
                code = ctx.prjCodeList.Where(x => x.id == fitem.prjId).FirstOrDefault().code;
            }

            List<string> prjUsers = new List<string>();

            var prjCodeObj = ctx.prjCodeList.OrderBy(x => x.code).FirstOrDefault();
            prjUsers.Add(prjCodeObj.creator);
            prjUsers.Add(prjCodeObj.owner);

            foreach (var pm in ctx.prjPMList.Where(n=>n.pid==prjCodeObj.id && n.Title=="PM"))
            {
                prjUsers.Add(pm.WorkNo);
            }

            vwForumDetail Model = new vwForumDetail(id, code);
            Model.currentUser = ctx.getCurrentUser(User.Identity.Name).workNo;
            ViewBag.code = code;
            ViewBag.id = id;
            ViewBag.mobile = mobile;
            ViewBag.Pms = prjUsers;
            return View(Model);
        }

        [HttpGet]
        public ActionResult createEvent(string prjId)
        {
            setHMList();
            vwPrjEvent model = new vwPrjEvent();
            model.billDate = ctx.getLocalTiime();
            model.creator = User.Identity.Name;
            model.prjId = prjId;
            return View(model);
        }

        [HttpPost]
        public ActionResult createEvent(vwPrjEvent model)
        {
            model.creator = User.Identity.Name;
            setHMList();
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
            var sender = ctx.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            if (string.IsNullOrEmpty(model.id))
            {
                FlowMain fmain = new FlowMain();

                fmain.billNo = ctx.genBillNo(model.formEnm);
                fmain.id = Guid.NewGuid().ToString();
                fmain.defId = model.formEnm;
                fmain.flowName = "專案月曆";
                fmain.flowStatus = 1;
                fmain.senderNo = model.creator;
                fmain.billDate = model.billDate;
                ctx.FlowMainList.Add(fmain);

                model.id = Guid.NewGuid().ToString();
                model.billNo = ctx.genBillNo(model.formEnm);

                PrjEvent saveModel = new PrjEvent
                {
                    id = model.id,
                    prjId = model.prjId,
                    billNo = model.billNo,
                    billDate = model.billDate,
                    creator = model.creator,
                    subject = model.subject,
                    sMemo = model.sMemo,
                    beginDate = model.beginDate,
                    beginHH = model.beginHH,
                    beginMM = model.beginMM,
                    endDate = model.endDate,
                    endHH = model.endHH,
                    endMM = model.endMM
                };

                ctx.prjEventList.Add(saveModel);
                ctx.SaveChanges();
            }

            return RedirectToAction("portal", "Prj", new { code = model.prjCode.code });
        }

        [HttpGet]
        public ActionResult deleteEvent(string id, string code)
        {
            ctx.prjEventList.Remove(ctx.prjEventList.Where(x => x.id == id).FirstOrDefault());
            ctx.SaveChanges();
            return RedirectToAction("portal", "prj", new { code = code });
        }


        [HttpGet]
        public ActionResult ForumUserList(string id)
        {
            prjCode prjCodeObj = ctx.prjCodeList.Where(x => x.id == id).FirstOrDefault();
            ViewBag.prjCaption = prjCodeObj.code + "-" + prjCodeObj.nm;
            ViewBag.prjid = prjCodeObj.id;

            string sql = "select a.*,b.cName from prjForumUsers a left join AspNetUsers b on a.WorkNo=b.workNo ";
            sql += " where a.pid=@pid";
            List<vwPrjForumUser> list = con.Query<vwPrjForumUser>(sql, new { pid = id }).ToList<vwPrjForumUser>();
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

            if (ctx.prjForumUserList.Where(x => x.pid == model.pid && x.WorkNo == model.WorkNo).Count() == 0)
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
            if (TempData["msg"] != null)
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
            var checker = ctx.getUserByWorkNo(User.Identity.Name);
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
                if (pObj != null)
                {
                    if ((!string.IsNullOrEmpty(pObj.mgr1))
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
                    catch (Exception exRemoveWorkItem)
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

            if (errList.Count() > 0)
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
                        if (obj["chkpm"].Value<Boolean>()) //&& string.IsNullOrEmpty(pItem.pm))
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
            return RedirectToAction("signPrjWork", new { id = model.id });

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
                        if (plist.Where(x => x.id == item.id).Count() == 0)
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
        public ActionResult prjCodeList(int status)
        {
            rep = new prjRep(User.Identity.Name);

            return Content(JsonConvert.SerializeObject(rep.prjCodeList(status)), "application/json");
        }
        public ActionResult prjCodeDetail(string id)
        {
            rep = new prjRep(User.Identity.Name);
            dynamic r = new ExpandoObject();
            vwPrjCode prjCodeObj = rep.loadPrjCode(id);

            var pmList = ctx.prjPMList.Where(x => x.pid == id && x.Title == "PM");
            JArray jList = new JArray();
            foreach (prjPM pm in pmList)
            {
                JObject jItem = new JObject();
                jItem["pm"] = pm.WorkNo;
                jList.Add(jItem);
            }

            prjCodeObj.hPMList = jList.ToString();
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
            if (fm["contractDate"].ToString() != "")
            {
                prjObj.contractDate = Convert.ToDateTime(fm["contractDate"].ToString());
            }

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

            if (errList.Count() == 0)
            {
                JArray pmlist = new JArray();
                try
                {
                    pmlist = JArray.Parse(fm["hPMList"].ToString());
                }
                catch (Exception exPMList)
                {

                }
                //ctx.prjPMList.RemoveRange(ctx.prjPMList.Where(x => x.pid == prjObj.id));
                //ctx.SaveChanges();
                List<prjPM> pmDBList=ctx.prjPMList.Where(x=>x.pid==prjObj.id).ToList<prjPM>();
                foreach (var pm in pmlist)
                {
                    if (pmDBList.Where(x =>x.WorkNo == pm["pm"].ToString()).Count() == 0)
                    {
                        prjPM pmObj = new prjPM
                        {
                            id = Guid.NewGuid().ToString(),
                            pid = prjObj.id,
                            WorkNo = pm["pm"].ToString(),
                            Title ="PM"
                        };
                        ctx.prjPMList.Add(pmObj);
                        ctx.SaveChanges();
                    }
                }
                foreach(prjPM pm in pmDBList)
                {
                    if( (pmlist.Where(x=>x["pm"].ToString()==pm.WorkNo).Count()==0) && pm.Title=="PM")
                    {
                        ctx.prjPMList.Remove(ctx.prjPMList.Where(x => x.pid == prjObj.id && x.WorkNo == pm.WorkNo).FirstOrDefault());
                        ctx.SaveChanges();
                    }
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

        string getEventPrjWorks(List<prjCode> prjCodeList, string selPrj)
        {
            string r = "";

            DateTime today = DateTime.Today;
            List<dynamic> dataList = new List<dynamic>();

            if (string.IsNullOrEmpty(selPrj))
            {
                return JsonConvert.SerializeObject(dataList);
            }

            int idx = 1;
            List<prjWorkItem> prjWorkItemList = ctx.prjWorkItemList.ToList<prjWorkItem>();
            //List<PrjEvent> prjEventList = ctx.prjEventList.ToList<PrjEvent>();
            if (selPrj.ToUpper() != "ALL")
            {
                prjCodeList = prjCodeList.Where(x => x.code == selPrj).ToList<prjCode>();
                prjWorkItemList = prjWorkItemList.Where(x => x.prjCode == selPrj).ToList<prjWorkItem>();
            }

            foreach (prjWorkItem item in prjWorkItemList)
            {
                if (prjCodeList.Where(x => x.code == item.prjCode).Count() > 0)
                {
                    dynamic obj = new ExpandoObject();
                    obj.id = idx;
                    string prjWorkNm = "";
                    try
                    {
                        prjWorkNm = prjCodeList.Where(x => x.code == item.prjCode).FirstOrDefault().nm;
                    }
                    catch (Exception exGetPrjCodeNm)
                    {
                        prjWorkNm = "";
                    }
                    obj.title = prjWorkNm + "-" + item.subject + "-" + item.hours.ToString("0.0") + "小時";
                    obj.start = item.dt;
                    obj.end = item.dt;
                    dataList.Add(obj);
                    idx++;
                }
            }

            foreach (PrjEvent item in ctx.prjEventList.ToList<PrjEvent>())
            {
                dynamic obj = new ExpandoObject();
                obj.id = idx;
                string prjWorkNm = "";
                try
                {
                    var prjObj = prjCodeList.Where(x => x.id == item.prjId).FirstOrDefault();
                    if (prjObj != null)
                    {
                        prjWorkNm = prjObj.nm;
                        obj.url = Url.Action("portal", "Prj", new { code = prjObj.code });
                        obj.title = "專案月曆:" + prjWorkNm + "-" + item.subject;
                        obj.start = item.beginDate;
                        obj.end = item.endDate;
                        obj.eventType = "event";
                        dataList.Add(obj);
                        idx++;
                    }
                }
                catch (Exception exGetPrjCodeNm)
                {
                    prjWorkNm = "";
                }
            }

            return JsonConvert.SerializeObject(dataList);
        }

        public ActionResult Home(string selPrj = "ALL")
        {

            List<prjCode> prjCodeList = new List<Models.prjCode>();
            var orgList = ctx.prjCodeList.OrderBy(x => x.code).ToList<prjCode>();
            foreach (prjCode prj in orgList)
            {
                if (User.Identity.Name == prj.owner || User.Identity.Name == prj.creator)
                {
                    prjCodeList.Add(prj);
                }
                else
                {
                    if (ctx.prjPMList.Where(x => x.pid == prj.id && x.WorkNo == User.Identity.Name).Count() > 0)
                    {
                        prjCodeList.Add(prj);
                    }
                }
            }
            List<SelectListItem> myPrjList = new List<SelectListItem>();

            foreach (var prj in prjCodeList)
            {
                myPrjList.Add(new SelectListItem
                {
                    Text = prj.code + "-" + prj.nm,
                    Value = prj.code

                });
            }

            if (myPrjList.Count > 0)
            {
                myPrjList.Insert(0, new SelectListItem
                {
                    Text = "全部",
                    Value = "ALL",
                    Selected = true
                });
            }

            ViewBag.myPrjs = myPrjList;


            ViewBag.eventPrjWorks = getEventPrjWorks(prjCodeList, selPrj);

            return View();
        }
    }
}
