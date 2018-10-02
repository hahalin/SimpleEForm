using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace eform.Models
{
    public class publicOut
    {
        [Key]
        public string id { get; set; }
        public string flowId { get; set; }
        [DisplayName("申請日期")]
        public DateTime? billDate { get; set; }
        [DisplayName("預計出發時間")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("預計回廠時間")]
        public DateTime? dtEnd { get; set; }
        public int beginHH { get; set; } = 0;
        public int beginMM { get; set; } = 0;
        public int endHH { get; set; } = 0;
        public int endMM { get; set; } = 0;
        [DisplayName("出差目的")]
        public string subject { get; set; } 
        [DisplayName("交通工具")]
        public string transport { get; set; }
        [DisplayName("出差地點(由出發地填寫至結束地)：")]
        public string destination { get; set; }
        public string jext { get; set; }
    }

    public class vwPublicOut
    {
        public string id { get; set; }
        public string flowId { get; set; }
        public vwEmployee user { get; set; }
        [DisplayName("申請日期")]
        public DateTime? billDate { get; set; }
        [DisplayName("預計出發時間")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("預計回廠時間")]
        public DateTime? dtEnd { get; set; }
        public int beginHH { get; set; } = 0;
        public int beginMM { get; set; } = 0;
        public int endHH { get; set; } = 0;
        public int endMM { get; set; } = 0;
        [DisplayName("出差目的")]
        public string subject { get; set; } 
        [DisplayName("交通工具")]
        public string transport { get; set; }
        [DisplayName("出差地點")]
        public string destination { get; set; }
        public string jext { get; set; }
    }
}