using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;
using Dapper;
using eform.Models;
using System.Data.SqlClient;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;


namespace eform.Controllers
{
    public class HrMgrController : Controller
    {
        ApplicationDbContext ctx;
        SqlConnection con;
        string constring = "";
        public HrMgrController()
        {
            ctx = new ApplicationDbContext();
            constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            con = new SqlConnection(constring);
        }

        void QueryData(int y)
        {
            #region 組合Pivot矩陣月份資料

            DataTable tb = new DataTable();

            tb.Load(
                con.ExecuteReader(
                    @"
                        select * from
                            (
                            select N'本期新增特休' hType,* from (
                                select a.workNo,b.cName,'m'+convert(varchar(2),a.m) m,a.v1
                                from dayOffSums a left join AspNetUsers b on a.workNo=b.workNo where a.y=@y and a.sType='a'
                            ) t
                            pivot
                                (
                                sum(v1)
                                for [m] in (
                                    [m0],[m1],[m2],[m3],[m4],[m5],[m6],[m7],[m8],[m9],[m10],[m11],[m12]
                                )
                            ) p1
                            union
                            select N'本期新增補休' hType,* from (
                                select a.workNo,b.cName,'m'+convert(varchar(2),a.m) m,a.v1
                                from dayOffSums a left join AspNetUsers b on a.workNo=b.workNo where a.y=@y and a.sType='b'
                            ) t
                            pivot
                                (
                                sum(v1)
                                for [m] in (
                                    [m0],[m1],[m2],[m3],[m4],[m5],[m6],[m7],[m8],[m9],[m10],[m11],[m12]
                                )
                            ) p2 
                        ) tb order by workNo,hType
                    ",
                    new { y = y }
                )
            );
            ViewBag.tb = tb;
            #endregion
        }

        // GET: HRMgr
        [HttpGet]
        [Authorize]
        public ActionResult List(int? y)
        {
            if (y == null)
            {
                y = DateTime.Today.Year;
            }
            ViewBag.y = y;
            ViewBag.Title = "人力資源主管特休補修登錄區";
            QueryData(Convert.ToInt32(y));
            return View();
        }
        [HttpPost]
        public ActionResult List(int year)
        {
            ViewBag.y = year;
            ViewBag.Title = "人力資源主管特休補修登錄區";

            var cn = new SqlConnection(constring);
            var sumList = cn.Query("select id from dayOffSums where y=@y", new { y = year });
            if (sumList.Count() == 0)
            {
                var userList = cn.Query("select workNo from AspNetUsers u where u.status=1 and u.workNo not in (select workNo from dayOffSums where y=@y) and u.workNo !='sadmin' and u.workNo !='admin'", new { y = year });
                foreach (var user in userList)
                {
                    for (int i = 0; i <= 12; i++)
                    {
                        cn.Execute(@"insert into dayOffSums(id,y,m,workNo,sType,v1,v2) values (@id,@y,@m,@workNo,@sType,@v1,@v2)",
                            new[]
                            {
                               new{
                                   id =Guid.NewGuid().ToString(),
                                   y=year,
                                   m=i,
                                   workNo =user.workNo,
                                   sType="a",
                                   v1=0,
                                   v2=0
                               }
                            }
                        );
                    }
                }
            }

            QueryData(year);

            return View();
        }

        [HttpPost]
        public ActionResult SaveList(int year, List<vwDayOffSum> data)
        {
            dbHelper dbh = new dbHelper();


            DataTable tb = new DataTable();
            tb = dbh.sql2tb("select workNo,m,v1 from dayOffSums where y = " + year.ToString() + " and sType='a'");

            List<string> sqlList = new List<string>();

            foreach (vwDayOffSum item in data)
            {
                List<decimal> vlist = new List<decimal>();
                vlist.Add(item.m0a);
                vlist.Add(item.m1a); vlist.Add(item.m2a); vlist.Add(item.m3a);
                vlist.Add(item.m4a); vlist.Add(item.m5a); vlist.Add(item.m6a);
                vlist.Add(item.m7a); vlist.Add(item.m8a); vlist.Add(item.m9a);
                vlist.Add(item.m10a); vlist.Add(item.m11a); vlist.Add(item.m12a);



                for (int i = 0; i < vlist.Count; i++)
                {
                    foreach (DataRow row in tb.Rows)
                    {
                        if (row["workNo"].ToString() == item.workNo
                            && row["m"].ToString() == i.ToString()
                            && ((Convert.ToDecimal(row["v1"]) - vlist[i]) != 0)
                        )
                        {
                            sqlList.Add(
                                string.Format(
                                    "update dayOffSums set v1 = {0} where y = {1} and m = {2} and sType = 'a' and workNo = '{3}';",
                                    vlist[i], year, i, item.workNo
                                )
                            );
                            break;
                        }

                        if (sqlList.Count == 10)
                        {
                            dbh.execSql(string.Join(" ", sqlList));
                            sqlList.Clear();
                        }
                    }
                }
            }
            if (sqlList.Count > 0)
            {
                dbh.execSql(string.Join(" ", sqlList));
            }


            dynamic r = new ExpandoObject();
            r.success = true;
            return Content(JsonConvert.SerializeObject(r), "application /json");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                TempData["msg"] = "請選取上傳檔案";
                return RedirectToAction("List");
            }

            if (file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string ext = Path.GetExtension(file.FileName).Replace(".", "").ToUpper();
                if (ext != "XLSX")
                {
                    TempData["msg"] = "請選擇XLSX格式Excel檔案";
                    return RedirectToAction("List");
                }

                string path = @"C:\inetpub\upload\" + ctx.getLocalTiime().ToString("yyyyMM");
                DirectoryInfo di = new DirectoryInfo(path);
                if (di.Exists == false)
                {
                    di.Create();
                }
                string uploadfile = path + @"\" + "HrUpload" + ctx.getLocalTiime().ToString("yyyyMMdd") + ".xlsx";
                if (System.IO.File.Exists(uploadfile))
                {
                    System.IO.File.Delete(uploadfile);
                }
                file.SaveAs(uploadfile);

                ExcelPackage pkg = new ExcelPackage(new FileInfo(uploadfile));
                ExcelWorkbook bk = pkg.Workbook;
                ExcelWorksheet sht = bk.Worksheets[1];
                int year = Convert.ToInt32(sht.Cells[1,5].Text.Trim().Substring(0,4));
                for (int i = 2; i <= 65535; i++)
                {
                    if (sht.Cells[i, 1].Text.Trim() == "" || sht.Cells[i, 2].Text.Trim() == "")
                    {
                        break;
                    }
                    string workNo = sht.Cells[i, 2].Text.Trim();
                    con.Execute("delete from dayOffSums where y=@y and workNo=@workNo", new { @y = year, @workNo = workNo });
                }
                for (int i = 2; i <= 65535; i++)
                {
                    if (sht.Cells[i, 1].Text.Trim() == "" || sht.Cells[i, 2].Text.Trim() == "")
                    {
                        break;
                    }
                    string workNo = sht.Cells[i, 2].Text.Trim();
                    string hType = sht.Cells[i, 1].Text.Trim().Contains("新增特休") ? "a" : "b";
                    List<decimal> mvList = new List<decimal>();
                    for (int itmp=0;itmp<=12;itmp++)
                    {
                        decimal v = 0;
                        try
                        {
                            v = Convert.ToDecimal(sht.Cells[i, 4 + itmp].Text);
                        }
                        catch(Exception ex)
                        {
                            v = 0;
                        }
                        //mvList.Add(v);
                        string sql = "";
                        sql = "insert into dayOffSums(id, y, m, workNo, sType, v1,v2) values(@id, @y, @m, @workNo, @sType, @v,0)";
                        con.Execute(sql,
                            new[]
                            {
                               new{
                                   id =Guid.NewGuid().ToString(),
                                   y=year,
                                   m=itmp,
                                   workNo =workNo,
                                   sType=hType,
                                   v=v
                               }
                            }
                        );
                    }
                }
                TempData["msg"] = "上傳完成";
                return RedirectToAction("List");
            }
            else
            {
                TempData["msg"] = "上傳檔案失敗，請重新執行";
                return RedirectToAction("List");
            }
        }
    }
}