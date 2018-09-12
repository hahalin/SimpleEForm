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
                var userList = cn.Query("select workNo from AspNetUsers u where u.status=1 and u.workNo not in (select workNo from dayOffSums where y=@y)", new { y = year });
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
                            select a.workNo,b.cName,convert(varchar(2),a.m)+'月'+case a.stype when 'a' then '前期轉入'  when 'b' then '本期新增' end m,a.v1
                            from dayOffSums a left join AspNetUsers b on a.workNo=b.workNo where a.y=@y
                        ) t
                        pivot
                           (
                            sum(v1)
                            for [m] in (
                                [1月前期轉入],[1月本期新增],
                                [2月前期轉入],[2月本期新增],
                                [3月前期轉入],[3月本期新增],
                                [4月前期轉入],[4月本期新增],
                                [5月前期轉入],[5月本期新增],
                                [6月前期轉入],[6月本期新增],
                                [7月前期轉入],[7月本期新增],
                                [8月前期轉入],[8月本期新增],
                                [9月前期轉入],[9月本期新增],
                                [10月前期轉入],[10月本期新增],
                                [11月前期轉入],[11月本期新增],
                                [12月前期轉入],[12月本期新增]
                            )
                        ) p
                    ",
                    new { y = year }
                )
            );

            ViewBag.tb = tb;
            return View();
        }
    }
}