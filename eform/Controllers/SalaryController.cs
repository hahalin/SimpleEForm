using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eform.Attributes;
using System.IO;
using OfficeOpenXml;

namespace eform.Controllers
{
    public class SalaryController : Controller
    {
        // GET: Salary
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "個人薪資查詢";
            ViewBag.Action = "Index";
            return View();
        }

        List<string> getHiddenFields()
        {
            List<string> list = new List<string>();

            list.Add("職稱");
            list.Add("ID");
            list.Add("生日");
            list.Add("生日月份");
            list.Add("到職日");
            list.Add("加保日");
            list.Add("最近異動日");
            list.Add("退保日");
            list.Add("基本年薪");

            list.Add("勞保-");
            list.Add("勞保加保天數");
            list.Add("勞保月投保薪資");
            list.Add("勞保-普通事故(9.5 % *雇主70 %)");
            list.Add("勞保-就業(1%*雇主70%)");
            list.Add("勞保-職業災害(0.33 % *雇主100 %)");
            list.Add("勞保-工資墊償基金(0.025 % *雇主100 %)");
            list.Add("雇主負擔");
            list.Add("勞保-普通事故(9.5 % *個人20 %)");
            list.Add("勞保-就業(1%*個人20%)");
            list.Add("本人負擔");
            list.Add("健保加保天數");
            list.Add("眷口數");
            list.Add("健保月投保薪資");

            list.Add("健保保險費率");
            list.Add("健保保險費率4.69%*雇主60%");
            list.Add("健保保險費率4.69%*個人30%");

            list.Add("退休金");
            list.Add("退休金加保天數");
            list.Add("退休金月投保薪資");
            list.Add("退休金雇主6%");

            list.Add("個人自願提繳率");
            list.Add("個人自願提繳率本人負擔>=6%");
            list.Add("個人自願提繳率本人負擔");
            return list;
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string ym = "")
        {
            ViewBag.Title = "個人薪資查詢";
            if (string.IsNullOrEmpty(ym))
            {
                ModelState.AddModelError("", "請輸入年度與月份");
                return View();
            }
            var ctx = new ApplicationDbContext();
            var user = ctx.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                string workNo = user.workNo;
                int y = Convert.ToInt32(ym.Split('-')[0]);
                int m = Convert.ToInt32(ym.Split('-')[1]);
                DateTime dtym = new DateTime(y, m, 1);
                ViewBag.ym = dtym.ToString("yyyy-MM");
                salary sObj = ctx.salaryList.Where(x => x.year == y && x.month == m && x.workNo == user.workNo).FirstOrDefault();
                if (sObj != null)
                {
                    JObject jobj = null;
                    try
                    {
                        jobj = JObject.Parse(sObj.jData);
                        List<string> removeList = new List<string>();
                        foreach(JProperty prop in jobj.Properties())
                        {
                            if (getHiddenFields().Where(x=>prop.Name.Contains(x)).Count()>0)
                            {
                                removeList.Add(prop.Name);
                            }
                        }
                        foreach(string propName in removeList)
                        {
                            jobj.Remove(propName);
                        }
                    }
                    catch (Exception ex)
                    {
                        jobj = null;
                    }
                    if (jobj == null)
                    {
                        return View();
                    }
                    DataTable dt = new DataTable();

                    List<KeyValuePair<string, int>> fdList = new List<KeyValuePair<string, int>>();

                    for (int i = 0; i < jobj.Properties().ToList<JProperty>().Count(); i++)
                    {
                        string pnm = jobj.Properties().ToList<JProperty>()[i].Name;
                        string pnm2 = "";
                        if (pnm.Split('-').Count() > 1)
                        {
                            pnm = pnm.Split('-')[0];
                        }
                        KeyValuePair<string, int> fd;
                        if (fdList.Where(x => x.Key == pnm).Count() > 0)
                        {
                            int idx = 0;
                            for (int itmp = 0; itmp < fdList.Count(); itmp++)
                            {
                                if (fdList[itmp].Key == pnm)
                                {
                                    fd = fdList.Where(x => x.Key == pnm).First();
                                    fd = new KeyValuePair<string, int>(pnm, fd.Value + 1);
                                    fdList[itmp] = fd;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            fd = new KeyValuePair<string, int>(pnm, 1);
                            fdList.Add(fd);
                        }
                        dt.Columns.Add(jobj.Properties().ToList<JProperty>()[i].Name);
                    }

                    DataRow r = dt.NewRow();
                    for (int i = 0; i < jobj.Properties().ToList<JProperty>().Count(); i++)
                    {
                        string pnm = jobj.Properties().ToList<JProperty>()[i].Name;
                        r[pnm] = jobj[pnm];
                    }
                    dt.Rows.Add(r);
                    dt.AcceptChanges();
                    ViewBag.data = dt;
                    ViewBag.fdList = fdList;
                    return View();
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult All()
        {
            ViewBag.Title = "全部薪資查詢(主管權限)";
            ViewBag.Action = "All";
            return View("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult All(string ym = "")
        {
            ViewBag.Title = "全部薪資查詢(主管權限)";
            if (string.IsNullOrEmpty(ym))
            {
                ModelState.AddModelError("", "請輸入年度與月份");
                return View();
            }
            var ctx = new ApplicationDbContext();
            int y = Convert.ToInt32(ym.Split('-')[0]);
            int m = Convert.ToInt32(ym.Split('-')[1]);
            DateTime dtym = new DateTime(y, m, 1);
            ViewBag.ym = dtym.ToString("yyyy-MM");
            salary sObj = ctx.salaryList.Where(x => x.year == y && x.month == m).FirstOrDefault();
            if (sObj != null)
            {
                JObject jobj = null;
                try
                {
                    jobj = JObject.Parse(sObj.jData);
                }
                catch (Exception ex)
                {
                    jobj = null;
                }
                if (jobj == null)
                {
                    return View();
                }
                DataTable dt = new DataTable();

                List<KeyValuePair<string, int>> fdList = new List<KeyValuePair<string, int>>();

                for (int i = 0; i < jobj.Properties().ToList<JProperty>().Count(); i++)
                {
                    string pnm = jobj.Properties().ToList<JProperty>()[i].Name;
                    if (pnm.Split('-').Count() > 1)
                    {
                        pnm = pnm.Split('-')[0];
                    }
                    KeyValuePair<string, int> fd;
                    if (fdList.Where(x => x.Key == pnm).Count() > 0)
                    {
                        int idx = 0;
                        for (int itmp = 0; itmp < fdList.Count(); itmp++)
                        {
                            if (fdList[itmp].Key == pnm)
                            {
                                fd = fdList.Where(x => x.Key == pnm).First();
                                fd = new KeyValuePair<string, int>(pnm, fd.Value + 1);
                                fdList[itmp] = fd;
                                break;
                            }
                        }
                    }
                    else
                    {
                        fd = new KeyValuePair<string, int>(pnm, 1);
                        fdList.Add(fd);
                    }
                    dt.Columns.Add(jobj.Properties().ToList<JProperty>()[i].Name);
                }

                DataRow r = dt.NewRow();
                for (int i = 0; i < jobj.Properties().ToList<JProperty>().Count(); i++)
                {
                    string pnm = jobj.Properties().ToList<JProperty>()[i].Name;
                    r[pnm] = jobj[pnm];
                }
                dt.Rows.Add(r);
                dt.AcceptChanges();
                ViewBag.data = dt;
                ViewBag.fdList = fdList;
                return View();
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Upload()
        {
            ViewBag.Title = "薪資檔案上傳";
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                ModelState.AddModelError("", "上傳檔案失敗，請重新執行");
                return View();
            }

            if (file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string ext = Path.GetExtension(file.FileName).Replace(".", "").ToUpper();
                if (ext != "XLSX")
                {
                    ModelState.AddModelError("", "請選擇XLSX格式Excel檔案！");
                    return View();
                }

                var ctx = new ApplicationDbContext();

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "upload", ctx.getLocalTiime().ToString("yyyyMMdd") + ".xlsx");
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                file.SaveAs(path);

                ExcelPackage pkg = new ExcelPackage(new FileInfo(path));
                ExcelWorkbook bk = pkg.Workbook;
                ExcelWorksheet sht = bk.Worksheets[1];

                List<string> fieldList = new List<string>();

                for (int j = 1; j <= 72; j++)
                {
                    fieldList.Add(sht.Cells[1, j].Text.Replace("\n", "").Replace(" ", ""));
                }

                for (int i = 2; i <= 65535; i++)
                {
                    if (sht.Cells[i, 1].Text.Trim() == "")
                    {
                        break;
                    }

                    int year = 0; int month = 0;
                    string workNo = "";

                    string[] ym = sht.Cells[i, 3].Text.Split('/');
                    if (ym.Length == 2)
                    {
                        year = Convert.ToInt32(ym[0]);
                        month = Convert.ToInt32(ym[1]);
                    }
                    workNo = sht.Cells[i, 1].Text;

                    salary sObj = null;
                    sObj = ctx.salaryList.Where(x => x.year == year && x.month == month && x.workNo == workNo).FirstOrDefault();
                    if (sObj == null)
                    {
                        sObj = new salary
                        {
                            id = Guid.NewGuid().ToString(),
                            year = year,
                            month = month,
                            workNo = workNo,
                            jData = "{}"
                        };
                        ctx.salaryList.Add(sObj);
                    }

                    JObject jobj = new JObject();
                    //jobj.Properties().ToList<JProperty>()[1].Name
                    for (int j = 0; j < fieldList.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(fieldList[j]))
                        {
                            jobj[fieldList[j]] = sht.Cells[i, j + 1].Text;
                        }
                    }
                    sObj.jData = jobj.ToString();
                    ctx.SaveChanges();
                }

                TempData["msg"] = "上傳完成";
                return View();
            }
            else
            {
                ModelState.AddModelError("", "上傳檔案失敗，請重新執行");
                return View();
            }

        }
    }
}