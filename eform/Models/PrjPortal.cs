using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eform.Models;
using System.Data;

namespace eform.Models
{
    public class PrjPortal
    {
        ApplicationDbContext ctx;
        public PrjPortal(string user)
        {
            currentUser = user;
            ctx = new ApplicationDbContext();
            prjEventList = new List<PrjEvent>();

            prjCodeList = new List<Models.prjCode>();
            var orgList = ctx.prjCodeList.OrderBy(x => x.code).ToList<prjCode>();
            foreach (prjCode prj in orgList)
            {
                if (currentUser == prj.owner || currentUser==prj.creator)
                {
                    prjCodeList.Add(prj);
                }
                else
                {
                    if (ctx.prjPMList.Where(x => x.pid == prj.id && x.WorkNo == currentUser).Count() > 0)
                    {
                        prjCodeList.Add(prj);
                    }
                }
            }

        }
        public string currentUser { get; set; }
        public string prjCode { get; set; }
        public prjCode prjCodeObj
        {
            get
            {
                if (prjCode == "")
                {
                    return ctx.prjCodeList.OrderBy(x => x.code).FirstOrDefault();
                }
                else
                {
                    return ctx.prjCodeList.Where(x => x.code == prjCode).FirstOrDefault();
                }
            }
        }
        public vwPrjCode vwPrjCodeObj
        {
            get
            {
                vwPrjCode prjObj = new vwPrjCode
                {
                    id= prjCodeObj.id,
                    nm=prjCodeObj.nm,
                    code= prjCodeObj.code,
                    creator=prjCodeObj.creator,
                    owner=prjCodeObj.owner,
                    createDate= prjCodeObj.createDate,
                    contractDate= prjCodeObj.contractDate,
                    mmo1= prjCodeObj.mmo1,
                    mmo2 = prjCodeObj.mmo2,
                    mmo3 = prjCodeObj.mmo3,
                    mmo4 = prjCodeObj.mmo4,
                    mmo5 = prjCodeObj.mmo5,
                    mmo6 = prjCodeObj.mmo6,
                    mmo7 = prjCodeObj.mmo7,
                    mmo8 = prjCodeObj.mmo8,
                    mmo9 = prjCodeObj.mmo9,
                    mmo10 = prjCodeObj.mmo10
                };
                return prjObj;
            }
        }
        public List<prjCode> prjCodeList
        {
            get;set;
        }
        public List<PrjEvent> prjEventList
        {
            get;set;
        }

        public List<vwPrjEvent> vwEventList
        {
            get
            {
                List<vwPrjEvent> list = new List<vwPrjEvent>();
                foreach(var evt in prjEventList)
                {
                    vwPrjEvent vevt = new vwPrjEvent
                    {
                        id = evt.id,
                        billNo = evt.billNo,
                        prjId = evt.prjId,
                        billDate = evt.billDate,
                        subject = evt.subject,
                        sMemo = evt.sMemo,
                        beginDate = evt.beginDate,
                        endDate = evt.endDate,
                        beginHH = evt.beginHH,
                        beginMM = evt.beginMM,
                        endHH = evt.endHH,
                        endMM = evt.endMM
                    };

                    list.Add(vevt);
                }
                return list;
            }
        }

        public List<prjPM> PMList
        {
            get
            {
                List<prjPM> list = new List<prjPM>();
                list = ctx.prjPMList.Where(x => x.pid == this.prjCodeObj.id).ToList<prjPM>();
                return list;
            }
        }
        public List<vPrjPM> vPMList
        {
            get
            {
                List<vPrjPM> list = new List<vPrjPM>();

                List<string> workNoList = new List<string>();
                foreach (prjPM pm in PMList)
                {
                    workNoList.Add(pm.WorkNo);
                }

                var users = from u in ctx.Users
                            where workNoList.Contains(u.workNo)
                            select new { u.Id, u.workNo, u.cName, u.eName };
                           
                foreach(var u in users)
                {
                    var pm = PMList.Where(x => x.WorkNo == u.workNo).FirstOrDefault();
                    list.Add(new vPrjPM
                    {
                        id=pm.id,
                        WorkNo = u.workNo,
                        UserEName = u.eName,
                        UserCName = u.cName,
                        Title= pm.Title
                    });
                }

                //foreach( prjPM pm in PMList)
                //{
                //    vwEmployee emp = ctx.getUserByWorkNo(pm.WorkNo);
                //    list.Add(emp);
                //}

                return list;
            }
        }

        public List<vwForumItem> forumList
        {
            get
            {
                dbHelper dbh = new dbHelper();

                string sql = @"
                    select a.*,
                        (select count(id) from ForumItems WITH (NOLOCK) where pid=a.id) replyCnt,isnull(b.lastReplyDate,'') lastReplyDate
                        from ForumItems a WITH (NOLOCK) left join 
                        (select max(billDate) lastReplyDate,pid from ForumItems WITH (NOLOCK) group by pid) b on b.pid=a.id
                        where (a.pid is null  or a.pid='') and a.prjId='{0}' order by a.seq
                ";
                sql = string.Format(sql, this.prjCodeObj.id);

                DataTable tb = dbh.sql2tb(sql);
                //var list = ctx.prjForumItems.Where(x => x.prjId == this.prjCodeObj.id && String.IsNullOrEmpty(x.pid)).OrderBy(x => x.seq); //OrderByDescending(x => x.billDate);
                List<vwForumItem> lista = new List<vwForumItem>();
                foreach(DataRow  r in tb.Rows)
                {
                    vwForumItem fItem = new vwForumItem
                    {
                        id = r["id"].ToString(),
                        prjId = r["prjId"].ToString(),
                        workNo = r["workNo"].ToString(),
                        billDate = Convert.ToDateTime(r["billDate"].ToString()),
                        subject = r["subject"].ToString(),
                        smemo = r["smemo"].ToString(),
                        othersA1 = r["othersA1"].ToString(),
                        othersA2 = r["othersA2"].ToString(),
                        othersA3 = r["othersA3"].ToString(),
                        othersA4 = r["othersA4"].ToString(),
                        othersA5 = r["othersA5"].ToString(),
                        othersA6 = r["othersA6"].ToString(),
                        othersB1 = r["othersB1"].ToString(),
                        othersB2 = r["othersB2"].ToString(),
                        othersB3 = r["othersB3"].ToString(),
                        othersB4 = r["othersB4"].ToString(),
                        othersB5 = r["othersB5"].ToString(),
                        othersB6 = r["othersB6"].ToString(),
                        seq = Convert.ToInt32(r["seq"].ToString()),
                        sfile = r["sfile"].ToString(),
                        sfile2 = r["sfile2"].ToString(),
                        sfile3 = r["sfile3"].ToString(),
                        sfile4 = r["sfile4"].ToString(),
                        replyCnt = Convert.ToInt32(r["replyCnt"].ToString()),
                        lastReplyDate = Convert.ToInt32(r["replyCnt"].ToString())==0 ? "": Convert.ToDateTime(r["lastReplyDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    lista.Add(fItem);
                }
                return lista;
            }
        }
    }
}
