using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eform.Models.PrjGantt
{
    public class fmGanttAssignUser
    {
        public string userlist { get; set; }
        /// <summary>
        /// 小時/分(0,30)清單
        /// </summary>
        public ValueTuple<List<SelectListItem>, List<SelectListItem>> hmList { get; set; }
    }
}