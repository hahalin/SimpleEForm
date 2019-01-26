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
        public PrjPortal()
        {
            ctx = new ApplicationDbContext();
        }

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
        public List<prjCode> prjCodeList
        {
            get
            {
                return ctx.prjCodeList.OrderBy(x => x.code).ToList<prjCode>();
            }

        }
    }
}