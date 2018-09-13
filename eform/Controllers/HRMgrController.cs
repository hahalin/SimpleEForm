using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Dapper;
using eform.Models;
using System.Data.SqlClient;
using System.Dynamic;
using Newtonsoft.Json;

namespace eform.Controllers
{
    public class HrMgrController : Controller
    {
        ApplicationDbContext ctx;
        string constring = "";
        public HrMgrController()
        {
            ctx = new ApplicationDbContext();
            constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        // GET: HRMgr
        [HttpGet]
        public ActionResult List(int? y)
        {
            if (y == null)
            {
                y = DateTime.Today.Year;
            }

            ViewBag.y = y;
            ViewBag.Title = "人力資源主管特休補修登錄區";
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
                           },
                            new
                            {
                                id = Guid.NewGuid().ToString(),
                                y=year,
                                m=i,
                                workNo = user.workNo,
                                sType = "b",
                                v1 = 0,
                                v2 = 0
                            }
                            }
                        );
                    }
                }
            }

            DataTable tb = new DataTable();

            tb.Load(
                cn.ExecuteReader(
                    @"
                        select * from (
                            select a.workNo,b.cName,'m'+convert(varchar(2),a.m)+a.stype m,a.v1
                            from dayOffSums a left join AspNetUsers b on a.workNo=b.workNo where a.y=@y
                        ) t
                        pivot
                           (
                            sum(v1)
                            for [m] in (
                                [m0a],
                                [m1a],[m1b],[m2a],[m2b],[m3a],[m3b],
                                [m4a],[m4b],[m5a],[m5b],[m6a],[m6b],
                                [m7a],[m7b],[m8a],[m8b],[m9a],[m9b],
                                [m10a],[m10b],[m11a],[m11b],[m12a],[m12b]
                            )
                        ) p order by workNo
                    ",
                    new { y = year }
                )
            );

            ViewBag.tb = tb;
            return View();
        }

        [HttpPost]
        public ActionResult SaveList(int year, List<vwDayOffSum> data)
        {
            dbHelper dbh = new dbHelper();
                

            DataTable tb = new DataTable();
            tb=dbh.sql2tb("select workNo,m,v1 from dayOffSums where y = " + year.ToString() + " and sType='a'");

            List<string> sqlList = new List<string>();

            foreach (vwDayOffSum item in data)
            {
                List<decimal> vlist = new List<decimal>();
                vlist.Add(item.m0a);
                vlist.Add(item.m1a); vlist.Add(item.m2a); vlist.Add(item.m3a);
                vlist.Add(item.m4a); vlist.Add(item.m5a); vlist.Add(item.m6a);
                vlist.Add(item.m7a); vlist.Add(item.m8a); vlist.Add(item.m9a);
                vlist.Add(item.m10a); vlist.Add(item.m11a); vlist.Add(item.m12a);

                

                for(int i=0;i<vlist.Count;i++)
                {
                    foreach(DataRow row in tb.Rows)
                    {
                        if(row["workNo"].ToString()==item.workNo 
                            && row["m"].ToString()==i.ToString() 
                            && ( (Convert.ToDecimal(row["v1"]) -vlist[i]) !=0)
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

                        if (sqlList.Count ==10)
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
            r.sql = sqlList;
            return Content(JsonConvert.SerializeObject(r), "application /json");
        }

    }
}