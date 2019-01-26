using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace eform.Models
{
    public class ReqInHouse
    {
        [Key]
        public string id { get; set; }
        public string flowId { get; set; }
        [DisplayName("需求說明")]
        public string reqMemo { get; set; }
        [DisplayName("處理期限")]
        public string reqLimit { get; set; }
        [DisplayName("聯繫窗口")]
        public string contact { get; set; }
        [DisplayName("處理單位")]
        public string depNo { get; set; }
        public string sMemo { get; set; }
        public string jext { get; set; }
    }

    public class vwReqInHouse
    {
        public string id { get; set; }
        public string flowId { get; set; }

        public vwEmployee user { get; set; }
        [DisplayName("單號")]
        public string billNo { get; set; }
        [DisplayName("申請日期")]
        public DateTime? billDate { get; set; }
        [DisplayName("需求說明")]
        [Required]
        public string reqMemo { get; set; }
        [DisplayName("處理期限")]
        [Required]
        public string reqLimit { get; set; }
        [DisplayName("聯繫窗口")]
        [Required]
        public string contact { get; set; }
        [DisplayName("處理單位")]
        [Required]
        public string depNo { get; set; }
        public string sMemo { get; set; }
        public string jext { get; set; }
    }
}