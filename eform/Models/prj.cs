using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace eform.Models
{
    public class prj
    {
        public string id { get; set; }
        public string prjId { get; set; }
        public string nm { get; set; }
        public string custId { get; set; }
        public string custNm { get; set; }
        public DateTime? createDate { get; set; }
        public DateTime? beginDate { get; set; }
        public DateTime? endDate { get; set; }
        public string sMemo { get; set; }
        public string dtMemo { get; set; }
        public prjStatus status { get; set; }
        public string creator { get; set; }
        public string creatorNm { get; set; }
        public int amt { get; set; }
        public decimal grossProfit { get; set; } = 0;

        public string sBeginDate
        {
            get
            {
                return Convert.ToDateTime(beginDate).ToString("yyyy/MM/dd");
            }
        }
        public string sEndDate
        {
            get
            {
                return Convert.ToDateTime(endDate).ToString("yyyy/MM/dd");
            }
        }

        public string sCreateDate
        {
            get
            {
                return Convert.ToDateTime(createDate).ToString("yyyy/MM/dd");
            }
        }
    }

    public class prjItem
    {
        public string id { get; set; }
        public string pid { get; set; }
        public int seq { get; set; }
        public string title { get; set; }
        public string workNo { get; set; }
        public int perm { get; set; }
    }

    public enum prjStatus
    {
        active=1,
        closed=2
    }
}