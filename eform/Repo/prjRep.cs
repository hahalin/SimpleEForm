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
using System.Configuration;


namespace eform.Repo
{
    public class prjRep
    {
        ApplicationDbContext ctx;
        vwEmployee emp;
        string constring = "";
        SqlConnection con;
        public prjRep(string workNo)
        {
            ctx = new ApplicationDbContext();
            emp = ctx.getUserByWorkNo(workNo);
            constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            con = new SqlConnection(constring);
        }

        public Dictionary<int, string> defaultOwnerList()
        {
            Dictionary<int, string> list = new Dictionary<int, string>();

            list.Add(1, "總經理室");
            list.Add(2, "專案管理人");
            list.Add(3, "專案經理");
            list.Add(4, "專案生管");
            list.Add(5, "專案業務助理");
            list.Add(6, "專案採購");
            list.Add(7, "專案物管");
            list.Add(8, "專案財務");
            return list;
        }

        public string empList()
        {
            string sql = "select workNo,cName from AspNetUsers where status=@status order by workNo asc";
            List<dynamic> list = con.Query<dynamic>(sql, new { status = 1}).ToList<dynamic>();
            dynamic r = new ExpandoObject();
            r.data = list;
            return JsonConvert.SerializeObject(r);
        }

        public string prjList(prjStatus status = prjStatus.active)
        {
            string sql = "select * from prjs where status=@status order by beginDate asc";
            List<prj> list = con.Query<prj>(sql, new { status = (int)status }).ToList<prj>();
            dynamic r = new ExpandoObject();
            r.data = list;

            return JsonConvert.SerializeObject(r);
        }

        public string createPrj(prj prjObj)
        {
            string r = "";
            string sql = "";
            List<ExpandoObject> paramList = new List<ExpandoObject>();
            dynamic param = new ExpandoObject();
            paramList.Add(param);
            con.Execute(sql, paramList);
            return r;
        }

    }
}