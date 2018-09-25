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
            List<dynamic> list = con.Query<dynamic>(sql, new { status = 1 }).ToList<dynamic>();
            dynamic r = new ExpandoObject();
            r.data = list;
            return JsonConvert.SerializeObject(r);
        }

        public List<prjItem> loadPrjEmpList(string pid)
        {
            string sql = "select * from prjUsers where pid=@pid order by seq";
            List<prjItem> list = con.Query<prjItem>(sql, new { pid = pid }).ToList<prjItem>();
            return list;
        }

        public string prjList(prjStatus status = prjStatus.active)
        {
            string sql = "select a.*,b.cName creatorNm from prjs a left join AspNetUsers b on a.creator=b.workNo where a.status=@status order by a.createDate asc";
            List<prj> list = con.Query<prj>(sql, new { status = (int)status }).ToList<prj>();
            dynamic r = new ExpandoObject();
            r.data = list;

            return JsonConvert.SerializeObject(r);
        }

        public prj loadPrj(string prjId)
        {
            string sql = "select * from prjs where prjId=@prjId";
            prj prjObj = con.Query<prj>(sql, new { prjId = prjId }).FirstOrDefault<prj>();
            return prjObj;
        }

        public string createPrj(prj prjObj, JArray userList)
        {
            string r = "";
            string sql = "";

            if (string.IsNullOrEmpty(prjObj.prjId))
            {
                return "專案編號不得空白";
            }

            sql = "select count(prjId) as cnt from prjs where prjId=@prjId";
            int icnt = (int)con.ExecuteScalar(sql, new { prjId = prjObj.prjId });
            if (icnt > 0)
            {
                return "專案編號重複";
            }

            sql = "insert into prjs ";
            sql += "(id,prjId,nm,custId,custNm,beginDate,endDate,sMemo,dtMemo,status,creator,createDate,amt,grossProfit) ";
            sql += " values ";
            sql += " (@id,@prjId,@nm,@custId,@custNm,@beginDate,@endDate,@sMemo,@dtMemo,@status,@creator,@createDate,@amt,@grossProfit)";

            List<DynamicParameters> paramList = new List<DynamicParameters>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@id", prjObj.id);
            param.Add("@prjId", prjObj.prjId);
            param.Add("@nm", prjObj.nm);
            param.Add("@custId", prjObj.custId);
            param.Add("@custNm", prjObj.custNm);
            param.Add("@beginDate", prjObj.beginDate);
            param.Add("@endDate", prjObj.endDate);
            param.Add("@createDate", prjObj.createDate);
            param.Add("@creator", prjObj.creator);
            param.Add("@sMemo", prjObj.sMemo);
            param.Add("@dtMemo", prjObj.dtMemo);
            param.Add("@status", prjObj.status);
            param.Add("@amt", prjObj.amt);
            param.Add("@grossProfit", prjObj.grossProfit);
            paramList.Add(param);

            try
            {
                con.Execute(sql, paramList);

                sql = "insert into prjUsers (id,pid,seq,title,workNo,perm) values ";
                sql += "( @id,@pid,@seq,@title,@workNo,@perm)";
                int iseq = 1;
                foreach (JObject user in userList)
                {
                    if (!user.ContainsKey("workNo"))
                    {
                        continue;
                    }
                    if (iseq > 8 && user["workNo"].ToString().Trim() == "")
                    {
                        continue;
                    }

                    string workNo, perm;
                    workNo = user["workNo"].ToString();
                    if (workNo.Split('-').Count() > 1)
                    {
                        workNo = workNo.Split('-')[1];
                    }
                    perm = user["perm"].ToString();
                    if (perm.Split('.').Count() > 1)
                    {
                        perm = perm.Split('.')[0];
                    }
                    con.Execute(sql,
                        new[] {
                            new {
                               id=Guid.NewGuid().ToString(),
                               pid=prjObj.id,
                               seq=user["seq"].ToString(),
                               title=user["title"].ToString(),
                               workNo=workNo,
                               perm=numberUtils.getInt(perm)
                            }
                        }
                    );
                    iseq++;
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string updatePrj(prj prjObj, JArray userList)
        {
            string r = "";
            string sql = "";

            if (prjObj.creator != emp.workNo)
            {
                return "專案建立人才能修改此專案";
            }
            if (string.IsNullOrEmpty(prjObj.prjId))
            {
                return "專案編號不得空白";
            }
            sql = "select count(prjId) as cnt from prjs where prjId=@prjId and id != @id";
            int icnt = (int)con.ExecuteScalar(sql, new { prjId = prjObj.prjId,id=prjObj.id });
            if (icnt > 0)
            {
                return "專案編號重複";
            }

            sql = "update prjs set prjId=@prjId,nm=@nm,custId=@custId,custNm=@custNm, ";
            sql += " beginDate=@beginDate,endDate=@endDate,amt=@amt,grossProfit=@grossProfit, ";
            sql += " sMemo=@sMemo,dtMemo=@dtMemo ";
            sql += " where id=@id";

            List<DynamicParameters> paramList = new List<DynamicParameters>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@id", prjObj.id);
            param.Add("@prjId", prjObj.prjId);
            param.Add("@nm", prjObj.nm);
            param.Add("@custId", prjObj.custId);
            param.Add("@custNm", prjObj.custNm);
            param.Add("@beginDate", prjObj.beginDate);
            param.Add("@endDate", prjObj.endDate);
            param.Add("@sMemo", prjObj.sMemo);
            param.Add("@dtMemo", prjObj.dtMemo);
            param.Add("@amt", prjObj.amt);
            param.Add("@grossProfit", prjObj.grossProfit);
            paramList.Add(param);

            try
            {
                con.Execute(sql, paramList);
                con.Execute("delete prjUsers where pid=@pid",new { pid = prjObj.id });
                sql = "insert into prjUsers (id,pid,seq,title,workNo,perm) values ";
                sql += "( @id,@pid,@seq,@title,@workNo,@perm)";
                int iseq = 1;
                foreach (JObject user in userList)
                {
                    if (!user.ContainsKey("workNo"))
                    {
                        continue;
                    }
                    if (iseq > 8 && user["workNo"].ToString().Trim() == "")
                    {
                        continue;
                    }

                    string workNo, perm;
                    workNo = user["workNo"].ToString();
                    if (workNo.Split('-').Count() > 1)
                    {
                        workNo = workNo.Split('-')[1];
                    }
                    perm = user["perm"].ToString();
                    if (perm.Split('.').Count() > 1)
                    {
                        perm = perm.Split('.')[0];
                    }
                    con.Execute(sql,
                        new[] {
                            new {
                               id=Guid.NewGuid().ToString(),
                               pid=prjObj.id,
                               seq=user["seq"].ToString(),
                               title=user["title"].ToString(),
                               workNo=workNo,
                               perm=numberUtils.getInt(perm)
                            }
                        }
                    );
                    iseq++;
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}