using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eform.Models;

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
        public List<vwEmployee> vPMList
        {
            get
            {
                List<vwEmployee> list = new List<vwEmployee>();
                foreach( prjPM pm in PMList)
                {
                    vwEmployee emp = ctx.getUserByWorkNo(pm.WorkNo);
                    list.Add(emp);
                }

                return list;
            }
        }

        public List<vwForumItem> forumList
        {
            get
            {
                var list = ctx.prjForumItems.Where(x => x.prjId == this.prjCodeObj.id && String.IsNullOrEmpty(x.pid)).OrderBy(x => x.seq); //OrderByDescending(x => x.billDate);
                List<vwForumItem> lista = new List<vwForumItem>();
                foreach(var f in list)
                {
                    vwForumItem fItem = new vwForumItem
                    {
                        id = f.id,
                        prjId = f.prjId,
                        workNo = f.workNo,
                        billDate = f.billDate,
                        subject = f.subject,
                        smemo = f.smemo,
                        othersA1 = f.othersA1,
                        othersA2 = f.othersA2,
                        othersA3 = f.othersA3,
                        othersA4 = f.othersA4,
                        othersA5 = f.othersA5,
                        othersA6 = f.othersA6,
                        othersB1 = f.othersB1,
                        othersB2 = f.othersB2,
                        othersB3 = f.othersB3,
                        othersB4 = f.othersB4,
                        othersB5 = f.othersB5,
                        othersB6 = f.othersB6,
                        seq = f.seq,
                        sfile = f.sfile,
                        sfile2 = f.sfile2,
                        sfile3 = f.sfile3,
                        sfile4 = f.sfile4
                    };
                    lista.Add(fItem);
                }
                return lista;
            }
        }
    }
}
