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
    public class hrRep
    {
        ApplicationDbContext ctx;
        vwEmployee emp;
        string constring = "";
        SqlConnection con;
        public hrRep(string workNo)
        {
            ctx = new ApplicationDbContext();
            emp = ctx.getUserByWorkNo(workNo);
            constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            con = new SqlConnection(constring);
        }

        public Boolean isGMDep()
        {
                dbHelper dbh = new dbHelper();
                string sql = @"select count(b.depNo) as cnt
                            from PoUsers a inner join jobPoes b on a.poNO=b.poNo 
                                           inner join deps c on b.depNo=c.depNo
                            where a.UserId='@workno' and a.applicationUser_id is not null and b.depNo='001'";
                sql = sql.Replace("@workno", this.emp.workNo);
                return dbh.sql2count(sql) > 0;
        }

        public Boolean isMgr()
        {
            Boolean r = false;
            List<String> poList = con.Query<String>("select poNo from PoUsers where UserId =@uid", new { uid = this.emp.workNo }).ToList<String>();
            r = ctx.jobPos.Where(x => poList.Contains(x.poNo) && x.isFormSigner).Count() > 0;
            return r;
        }

        public List<string> getDepList()
        {
            List<String> depList = (from item in this.emp.poList select item.depNo).ToList<String>();
            return depList;
        }
        public List<string> getDepListByWorkNo(string workNo)
        {
            vwEmployee empObj = ctx.getUserByWorkNo(workNo);
            List<String> depList = (from item in empObj.poList select item.depNo).ToList<String>();
            return depList;
        }

        public DataTable queryMyDayOffList(int y)
        {
            DataTable tb = new DataTable();

            tb.Load(
                con.ExecuteReader(
                    @"
                        select * from
                            (
                            select N'本期新增特休' hType,* from (
                                select a.workNo,b.cName,'m'+convert(varchar(2),a.m) m,a.v1
                                from dayOffSums a left join AspNetUsers b on a.workNo=b.workNo where a.y=@y and a.sType='a' and a.workNo=@workNo
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
                                from dayOffSums a left join AspNetUsers b on a.workNo=b.workNo where a.y=@y and a.sType='b' and a.workNo=@workNo
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
                    new { y = y,workNo=emp.workNo }
                )
            );
            return tb;
        }


    }
}