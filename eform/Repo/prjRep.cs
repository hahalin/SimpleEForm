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
using System.Web.Mvc;

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
        public string prjCodeList(int status = 1)
        {
            string sql = "select a.* from prjCodes a left join AspNetUsers b on a.owner=b.workNo ";
            //sql +=" where a.status=@status order by a.code asc";
            if (status == 1)
            {
                sql += " where a.status=N'使用中' ";
            }
            else if (status == 0)
            {
                sql += " where a.status=N'關閉' ";
            }
            sql += " order by a.code asc";
            List<vwPrjCode> list = con.Query<vwPrjCode>(sql, new { status = "使用中" }).ToList<vwPrjCode>();
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

        public vwPrjCode loadPrjCode(string id)
        {
            string sql = "select * from prjCodes where id=@id";
            vwPrjCode prjObj = con.Query<vwPrjCode>(sql, new { id = id }).FirstOrDefault<vwPrjCode>();
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

        public string createPrjCode(prjCode prjObj, string creator, JArray userList = null)
        {
            string r = "";
            string sql = "";

            if (string.IsNullOrEmpty(prjObj.code))
            {
                return "專案編號不得空白";
            }

            if (string.IsNullOrEmpty(prjObj.nm))
            {
                return "專案名稱不得空白";
            }

            if (string.IsNullOrEmpty(prjObj.owner))
            {
                return "專案經理不得空白";
            }


            sql = "select count(code) as cnt from prjCodes where code=@code";
            int icnt = (int)con.ExecuteScalar(sql, new { code = prjObj.code });
            if (icnt > 0)
            {
                return "專案編號重複";
            }

            sql = "insert into prjCodes ";
            sql += "(id,code,nm,owner,status,mmo1,mmo2,mmo3,mmo4,mmo5,mmo6,mmo7,mmo8,mmo9,mmo10,creator,createDate,contractDate) ";
            sql += " values ";
            sql += " (@id,@code,@nm,@owner,@status,@mmo1,@mmo2,@mmo3,@mmo4,@mmo5,@mmo6,@mmo7,@mmo8,@mmo9,@mmo10,@creator,@createDate,@contractDate)";

            List<DynamicParameters> paramList = new List<DynamicParameters>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@id", prjObj.id);
            param.Add("@code", prjObj.code);
            param.Add("@nm", prjObj.nm);
            param.Add("@owner", prjObj.owner);
            param.Add("@status", prjObj.status);
            param.Add("@mmo1", prjObj.mmo1);
            param.Add("@mmo2", prjObj.mmo2);
            param.Add("@mmo3", prjObj.mmo3);
            param.Add("@mmo4", prjObj.mmo4);
            param.Add("@mmo5", prjObj.mmo5);
            param.Add("@mmo6", prjObj.mmo6);
            param.Add("@mmo7", prjObj.mmo7);
            param.Add("@mmo8", prjObj.mmo8);
            param.Add("@mmo9", prjObj.mmo9);
            param.Add("@mmo10", prjObj.mmo10);
            param.Add("@creator", creator);
            param.Add("@createDate", ctx.getLocalTiime());
            param.Add("@contractDate", prjObj.contractDate);
            paramList.Add(param);

            try
            {
                con.Execute(sql, paramList);
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
            int icnt = (int)con.ExecuteScalar(sql, new { prjId = prjObj.prjId, id = prjObj.id });
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
                con.Execute("delete prjUsers where pid=@pid", new { pid = prjObj.id });
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

        public string updatePrjCode(prjCode prjCodeObj, string workNo)
        {
            string r = "";
            string sql = "";

            //if (prjCodeObj.owner != workNo && workNo.ToUpper().Contains("ADMIN")==false)
            //{
            //return "專案代碼管理人才能修改此專案";
            //}
            if (string.IsNullOrEmpty(prjCodeObj.code))
            {
                return "專案代碼不得空白";
            }
            sql = "select count(id) as cnt from prjCodes where code=@code and id != @id";
            int icnt = (int)con.ExecuteScalar(sql, new { code = prjCodeObj.code, id = prjCodeObj.id });
            if (icnt > 0)
            {
                return "專案代碼重複";
            }

            sql = "update prjCodes set code=@code,nm=@nm,owner=@owner,status=@status, ";
            sql += " mmo1=@mmo1,mmo2=@mmo2,mmo3=@mmo3,mmo4=@mmo4,mmo5=@mmo5,";
            sql += " mmo6=@mmo6,mmo7=@mmo7,mmo8=@mmo8,mmo9=@mmo9,mmo10=@mmo10,contractDate=@contractDate";
            sql += " where id=@id";

            List<DynamicParameters> paramList = new List<DynamicParameters>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@id", prjCodeObj.id);
            param.Add("@code", prjCodeObj.code);
            param.Add("@nm", prjCodeObj.nm);
            param.Add("@owner", prjCodeObj.owner);
            param.Add("@status", prjCodeObj.status);
            param.Add("@mmo1", prjCodeObj.mmo1);
            param.Add("@mmo2", prjCodeObj.mmo2);
            param.Add("@mmo3", prjCodeObj.mmo3);
            param.Add("@mmo4", prjCodeObj.mmo4);
            param.Add("@mmo5", prjCodeObj.mmo5);
            param.Add("@mmo6", prjCodeObj.mmo6);
            param.Add("@mmo7", prjCodeObj.mmo7);
            param.Add("@mmo8", prjCodeObj.mmo8);
            param.Add("@mmo9", prjCodeObj.mmo9);
            param.Add("@mmo10", prjCodeObj.mmo10);
            param.Add("@contractDate", prjCodeObj.contractDate);
            paramList.Add(param);

            try
            {
                con.Execute(sql, paramList);
                if (string.IsNullOrEmpty(prjCodeObj.creator))
                {
                    con.Execute("update prjCodes set creator=@creator,createDate=@createDate where id=@id",
                        new
                        {
                            @id = prjCodeObj.id,
                            @creator = workNo,
                            @createDate = ctx.getLocalTiime()
                        }
                    );
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string getPrjList()
        {
            var prjs = this.ctx.prjCodeList.ToList<prjCode>();
            List<dynamic> prjlist = new List<dynamic>();
            foreach (var prj in prjs)
            {
                dynamic obj = new ExpandoObject();
                obj.id = prj.code;
                obj.text = prj.nm;
                obj.owner = prj.owner;
                prjlist.Add(obj);
            }
            dynamic prjSelTitle = new ExpandoObject();
            prjSelTitle.id = ""; prjSelTitle.text = "請選擇專案代碼";
            prjlist.Insert(0, prjSelTitle);

            return JsonConvert.SerializeObject(prjlist);
        }

        public string getPrjCodeList()
        {
            //var prjs = this.ctx.prjCodeList.ToList<prjCode>();
            string sql = "select a.* from prjCodes a left join AspNetUsers b on a.owner=b.workNo ";
            sql += " where a.status=@status order by a.code asc";
            List<vwPrjCode> prjs = con.Query<vwPrjCode>(sql, new { status = "使用中" }).ToList<vwPrjCode>();


            List<dynamic> prjlist = new List<dynamic>();
            foreach (var prj in prjs)
            {
                dynamic obj = new ExpandoObject();
                obj.id = prj.code;
                obj.text = prj.code + "-" + prj.nm;
                obj.nm = prj.nm;
                obj.owner = prj.owner;
                obj.ownerStr = prj.ownerStr;
                prjlist.Add(obj);
            }
            dynamic prjSelTitle = new ExpandoObject();
            prjSelTitle.id = ""; prjSelTitle.text = "請選擇工時代碼";
            prjSelTitle.nm = "請選擇工時代碼";
            prjlist.Insert(0, prjSelTitle);

            return JsonConvert.SerializeObject(prjlist);
        }

        public ValueTuple<List<SelectListItem>, List<SelectListItem>> getTimeSelection()
        {
            List<SelectListItem> beginHH = new List<SelectListItem>();
            List<SelectListItem> beginMM = new List<SelectListItem>();
            List<SelectListItem> endHH = new List<SelectListItem>();
            List<SelectListItem> endMM = new List<SelectListItem>();

            for (int i = 0; i <= 24; i++)
            {
                beginHH.Add(new SelectListItem { Text = i.ToString("0#"), Value = i.ToString("0#") });
                endHH.Add(new SelectListItem { Text = i.ToString("0#"), Value = i.ToString("0#") });
            }

            beginMM.Add(new SelectListItem { Text = "00", Value = "00" });
            endMM.Add(new SelectListItem { Text = "00", Value = "00" });
            beginMM.Add(new SelectListItem { Text = "30", Value = "30" });
            endMM.Add(new SelectListItem { Text = "30", Value = "30" });

            return (beginHH,beginMM);
        }
    }
}