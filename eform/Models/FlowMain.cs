using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace eform.Models
{
    public class FlowMain
    {
        [Key]
        public string id { get; set; }
        [DisplayName("單號")]
        public string billNo { get; set; }
        public string senderNo { get; set; }
        [DisplayName("申請日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? billDate { get; set; }
        public string defId { get; set; }
        public string flowName { get; set; }
        [DisplayName("簽核狀態")]
        public int flowStatus { get; set; } = 0;
    }

    public class FlowSub
    {
        [Key]
        public string id { get; set; }
        public string pid { get; set; }
        public int seq { get; set; }
        public string workNo { get; set; }
        public int signType { get; set; } = 0;
        public int signResult { get; set; } = 0;
        public string comment { get; set; }
        public DateTime?signDate { get; set; }
    }
}