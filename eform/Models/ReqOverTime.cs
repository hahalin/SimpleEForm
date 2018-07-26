using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace eform.Models
{
    public class ReqOverTime
    {
        [Key]
        public string id { get; set; }
        public string flowId { get; set; }
        [DisplayName("開始時間")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("結束時間")]
        public DateTime? dtEnd { get; set; }
        [DisplayName("加班時數")]
        public int hours { get; set; } = 0;
        [DisplayName("加班事由")]
        public string sMemo { get; set; }
    }
}