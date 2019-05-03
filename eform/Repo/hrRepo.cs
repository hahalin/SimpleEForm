using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using eform.Models;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;


namespace eform.Repo
{
    public class hrRepo
    {
        ApplicationDbContext ctx;
        vwEmployee emp;
        string constring = "";
        SqlConnection con;
        public hrRepo(string workNo)
        {
            ctx = new ApplicationDbContext();
            emp = ctx.getUserByWorkNo(workNo);
            constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            con = new SqlConnection(constring);
        }

        public DataTable QueryDayOffBaseData(int y)
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
                                and a.workNo=@workNo
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
                                and a.workNo=@workNo
                            ) t
                            pivot
                                (
                                sum(v1)
                                for [m] in (
                                    [m0],[m1],[m2],[m3],[m4],[m5],[m6],[m7],[m8],[m9],[m10],[m11],[m12]
                                )
                            ) p2 
                        ) tb order by hType
                    ",
                    new { y = y, workNo = this.emp.workNo }
                )
            );
            #endregion
            return tb;
        }

    }
}