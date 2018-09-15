using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace eform.Models
{
    public class dayOff
    {
        [Key]
        public string id { get; set; }
        public string flowId { get; set; }
        [DisplayName("開始時間")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("結束時間")]
        public DateTime? dtEnd { get; set; }
        public decimal beginHH { get; set; } = 0;
        public decimal endHH { get; set; } = 0;
        [DisplayName("假別")]
        public string dType { get; set; }
        [DisplayName("小時數")]
        public decimal hours { get; set; }
        [DisplayName("職務代理人")]
        public string jobAgent { get; set; }
        [DisplayName("請假事由")]
        public string sMemo { get; set; }
        public string jext { get; set; }

    }

    public class dayOffType
    {
        [Key]
        public int k { get; set; }
        public string v { get; set; }
    }
}