using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using eform.Models;
using Newtonsoft.Json;
using System.Dynamic;

namespace eform.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext ctx;
        public HomeController()
        {
            ctx = new ApplicationDbContext();
        }

        string getDayOffData()
        {
            DateTime today = DateTime.Today;
            DateTime minDate = today.AddMonths(-2);
            List<FlowMain> fMainList = ctx.FlowMainList.Where(
                x => x.billDate != null && x.billDate.Value.Year == today.Year 
                //&& x.billDate.Value.Month == today.Month
                && x.flowStatus == 2
                && x.defId == "DayOff").ToList<FlowMain>();
            List<string> billNoList = (from f in fMainList select f.id).ToList<string>();
            List<dayOff> dayOffList = ctx.dayOffList.Where(x => billNoList.Contains(x.flowId)).ToList<dayOff>();

            List<dynamic> dataList = new List<dynamic>();
            int idx = 1;
            foreach (var f in fMainList)
            {
                string urla = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("Detils", "Form", new { id = f.id });
                string urlb = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("Details", "Form", new { id = f.id });

                vwEmployee user = ctx.getUserByWorkNo(f.senderNo);
                dayOff d = dayOffList.Where(x => x.flowId == f.id).FirstOrDefault();
                dynamic obj = new ExpandoObject();
                obj.billNo = f.billNo;
                obj.date = f.billDate;
                obj.user = user.UserCName;
                obj.start = d.dtBegin;
                obj.end = d.dtEnd;
                if (d.dtBegin.Value.ToString("yyyyMMdd") == d.dtEnd.Value.ToString("yyyyMMdd"))
                {
                    obj.title = "休" + "-" + user.UserCName + " " + d.dtBegin.Value.ToString("HH:mm") + "~" + d.dtEnd.Value.ToString("HH:mm");
                }
                else
                {
                    obj.title = "休" + "-" + user.UserCName + " " + d.dtBegin.Value.ToString("MM/dd HH:mm") + "~" + d.dtEnd.Value.ToString("MM/dd HH:mm");
                }
                obj.formId = d.id;
                obj.url = f.senderNo == User.Identity.Name ? urla : urlb;
                obj.id = idx;
                obj.color = "#ff33cc";
                obj.textColor = "#ffffff";
                obj.order = 1;
                dataList.Add(obj);
                idx++;
            }

            return JsonConvert.SerializeObject(dataList);
        }

        string getPublicOutList()
        {
            DateTime today = DateTime.Today;
            DateTime minDate = today.AddMonths(-2);

            List<FlowMain> fMainList = ctx.FlowMainList.Where(
                x => x.billDate != null && x.billDate.Value.Year == today.Year 
                && x.flowStatus == 2
                && x.defId == "PublicOut").ToList<FlowMain>();
            List<string> billNoList = (from f in fMainList select f.id).ToList<string>();
            List<publicOut> publicOutList = ctx.publicOutList.Where(x => billNoList.Contains(x.flowId)).ToList<publicOut>();

            List<dynamic> dataList = new List<dynamic>();
            int idx = 1;
            foreach (var f in fMainList)
            {
                string urla = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("Detils", "Form", new { id = f.id });
                string urlb = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("Details", "Form", new { id = f.id });

                vwEmployee user = ctx.getUserByWorkNo(f.senderNo);
                publicOut d = publicOutList.Where(x => x.flowId == f.id).FirstOrDefault();
                dynamic obj = new ExpandoObject();
                obj.billNo = f.billNo;
                obj.date = f.billDate;
                obj.user = user.UserCName;
                obj.start = d.dtBegin;
                obj.end = d.dtEnd;
                obj.title = "外-" + d.dtBegin.Value.ToString("HH:mm") + "~" + d.dtEnd.Value.ToString("HH:mm")
                            + "-" + user.UserCName + "-" + d.destination;
                obj.formId = d.id;
                obj.url = f.senderNo == User.Identity.Name ? urla : urlb;
                obj.id = idx;
                obj.color = "#ff9900";
                obj.textColor = "#ffffff";
                obj.order = 2;
                dataList.Add(obj);
                idx++;
            }

            return JsonConvert.SerializeObject(dataList);
        }

        string getGuestFormList()
        {
            DateTime today = DateTime.Today;
            DateTime minDate = today.AddMonths(-2);

            List<FlowMain> fMainList = ctx.FlowMainList.Where(
                x => x.billDate != null && x.billDate.Value.Year == today.Year 
                && x.flowStatus == 2
                && x.defId == "GuestForm").ToList<FlowMain>();
            List<string> billNoList = (from f in fMainList select f.id).ToList<string>();
            List<guestForm> guestFormList = ctx.guestFormList.Where(x => billNoList.Contains(x.flowId)).ToList<guestForm>();

            List<dynamic> dataList = new List<dynamic>();
            int idx = 1;
            foreach (var f in fMainList)
            {
                string urla = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("Detils", "Form", new { id = f.id });
                string urlb = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("Details", "Form", new { id = f.id });

                vwEmployee user = ctx.getUserByWorkNo(f.senderNo);
                guestForm d = guestFormList.Where(x => x.flowId == f.id).FirstOrDefault();
                dynamic obj = new ExpandoObject();
                obj.billNo = f.billNo;
                obj.date = f.billDate;
                obj.user = user.UserCName;
                obj.start = d.dtBegin;
                obj.end = d.dtEnd;

                if (d.dtBegin.Value.ToString("yyyyMMdd") == d.dtEnd.Value.ToString("yyyyMMdd"))
                {
                    obj.title = "訪" + "-" + user.UserCName + " - " + d.guestCmp + " " + d.dtBegin.Value.ToString("HH:mm") + "~" + d.dtEnd.Value.ToString("HH:mm");
                }
                else
                {
                    obj.title = "訪" + " - " + user.UserCName + " - " + d.guestCmp + " " + d.dtBegin.Value.ToString("MM/dd HH:mm") + "~" + d.dtEnd.Value.ToString("MM/dd HH:mm");
                }

                //obj.title = "訪-"+user.UserCName +"-"+ d.guestCmp;
                obj.formId = d.id;
                obj.url = f.senderNo == User.Identity.Name ? urla : urlb;
                obj.id = idx;
                obj.color = "#ffb366";
                obj.textColor = "#000000";
                obj.order = 2;
                dataList.Add(obj);
                idx++;
            }

            return JsonConvert.SerializeObject(dataList);
        }

        string getEventScheduleList(string eventType = "")
        {
            DateTime today = DateTime.Today;
            DateTime minDate = today.AddMonths(-2);

            List<FlowMain> fMainList = ctx.FlowMainList.Where(
                x => x.billDate != null && x.billDate.Value.Year == today.Year 
                && x.flowStatus != 99
                && x.defId == "EventSchedule").ToList<FlowMain>();
            List<string> billNoList = (from f in fMainList select f.id).ToList<string>();
            List<EventSchedule> eventScheduleList_ = ctx.eventScheduleList.Where(x => billNoList.Contains(x.flowId)).ToList<EventSchedule>();
            List<EventSchedule> eventScheduleList = new List<EventSchedule>();

            if (eventType != "")
            {
                foreach (var evt in eventScheduleList_)
                {
                    if (evt.eventType.ToString() == eventType)
                    {
                        eventScheduleList.Add(evt);
                    }
                }
            }
            else
            {
                eventScheduleList = eventScheduleList_;
            }

            List<dynamic> dataList = new List<dynamic>();
            int idx = 1;
            foreach (var f in fMainList)
            {
                string urla = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("Details", "Form", new { id = f.id });
                string urlb = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + Url.Action("Details", "Form", new { id = f.id });

                vwEmployee user = ctx.getUserByWorkNo(f.senderNo);
                EventSchedule d = eventScheduleList.Where(x => x.flowId == f.id).FirstOrDefault();

                if (d != null)
                {
                    dynamic obj = new ExpandoObject();

                    string strTime1 = "";
                    string strTime2 = "";
                    strTime1 = String.Format("{0:00}", d.beginHH) + ":" + String.Format("{0:00}", d.beginMM);
                    strTime2 = String.Format("{0:00}", d.endHH) + ":" + String.Format("{0:00}", d.endMM);


                    obj.billNo = f.billNo;
                    obj.date = d.beginDate;
                    obj.user = user.UserCName;
                    obj.start = Convert.ToDateTime(d.beginDate).ToString("yyyy-MM-dd") + " " + strTime1;
                    obj.end = Convert.ToDateTime(d.endDate).ToString("yyyy-MM-dd") + " " + strTime2;


                    string prefix = "業";

                    if (d.eventType == 1)
                    {

                    }
                    else
                    {
                        prefix = "會";
                    }

                    if (d.beginDate.Value.ToString("yyyyMMdd") == d.endDate.Value.ToString("yyyyMMdd"))
                    {
                        obj.title = d.subject + "-" + user.UserCName + " " + strTime1 + "~" + strTime2;
                    }
                    else
                    {
                        obj.title = d.subject + "-" + user.UserCName + " " + d.beginDate.Value.ToString("MM/dd") + " " + strTime1 + "~" + d.endDate.Value.ToString("MM/dd") + " " + strTime2;
                    }

                    if (string.IsNullOrEmpty(d.colorTag))
                    {
                        if (d.eventType == 1)
                        {
                            obj.color = "red";
                            obj.textColor = "white";
                        }
                        else if (d.eventType == 2)
                        {
                            obj.color = "#00cc00";
                            obj.textColor = "white";
                        }
                    }
                    else
                    {
                        if (d.colorTag == "1")
                        {
                            obj.color = "gray";
                            obj.textColor = "white";
                        }
                        if (d.colorTag == "2")
                        {
                            obj.color = "lightgray";
                            obj.textColor = "red";
                        }
                        if (d.colorTag == "3")
                        {
                            obj.color = "lightgray";
                            obj.textColor = "black";
                        }
                        if (d.colorTag == "4")
                        {
                            obj.color = "forestgreen";
                            obj.textColor = "white";
                        }
                        if (d.colorTag == "5")
                        {
                            obj.color = "yellow";
                            obj.textColor = "black";
                        }
                        if (d.colorTag == "6")
                        {
                            obj.color = "red";
                            obj.textColor = "white";
                        }

                    }
                    if (d.eventType == 3)
                    {
                        obj.color = "#BA55D3";
                        obj.textColor = "white";
                    }
                    obj.formId = d.id;
                    obj.url = f.senderNo == User.Identity.Name ? urla : urlb;
                    obj.id = idx;
                    obj.order = 3;
                    dataList.Add(obj);
                    idx++;
                }
            }

            return JsonConvert.SerializeObject(dataList);
        }


        public ActionResult Index(string selForm = "2")
        {
            var context = new ApplicationDbContext();
            List<news> newslist = (from item in context.newsList.ToList<news>() orderby item.createTime2 descending select item).ToList<news>();

            List<SelectListItem> myFormList = new List<SelectListItem>();

            myFormList.Add(new SelectListItem
            {
                Text = "業務行程",
                Value = "1"
            });
            myFormList.Add(new SelectListItem
            {
                Text = "公司活動",
                Value = "2",
                Selected = true
            });
            myFormList.Add(new SelectListItem
            {
                Text = "會議室預約",
                Value = "3"
            });
            myFormList.Add(new SelectListItem
            {
                Text = "訪客",
                Value = "4"
            });
            myFormList.Add(new SelectListItem
            {
                Text = "外出",
                Value = "5"
            });
            myFormList.Add(new SelectListItem
            {
                Text = "休假",
                Value = "6"
            });

            myFormList.Insert(0, new SelectListItem
            {
                Text = "全部",
                Value = "ALL"
            });
            ViewBag.myForms = myFormList;

            ViewBag.newslist = newslist;
            ViewBag.dayOffList = JsonConvert.SerializeObject(new List<dynamic>());
            ViewBag.publicOutList = JsonConvert.SerializeObject(new List<dynamic>());
            ViewBag.guestFormList = JsonConvert.SerializeObject(new List<dynamic>());
            ViewBag.eventScheduleList = JsonConvert.SerializeObject(new List<dynamic>());

            if (selForm == "ALL")
            {
                ViewBag.dayOffList = getDayOffData();
                ViewBag.publicOutList = getPublicOutList();
                ViewBag.guestFormList = getGuestFormList();
                ViewBag.eventScheduleList = getEventScheduleList();
            }
            if (selForm == "1" || selForm == "2" || selForm == "3")
            {
                ViewBag.eventScheduleList = getEventScheduleList(selForm);
            }
            if (selForm == "4")
            {
                ViewBag.guestFormList = getGuestFormList();
            }
            if (selForm == "5")
            {
                ViewBag.publicOutList = getPublicOutList();
            }
            if (selForm == "6")
            {
                ViewBag.dayOffList = getDayOffData();
            }

            return View();
        }

        public ActionResult NewsDetail(string id)
        {
            var context = new ApplicationDbContext();
            news Model = context.newsList.Where(x => x.id == id).FirstOrDefault();
            return View(Model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}