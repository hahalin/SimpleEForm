using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace eform.Models
{
    public class vwFlowMain
    {
        public string id { get; set; }
        [DisplayName("申請人")]
        public string sender { get; set; }
        [DisplayName("申請日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? billDate { get; set; }
        [DisplayName("表單名稱")]
        public string flowName { get; set; }
        [DisplayName("簽核狀態")]
        public string flowStatus { get; set; } 
    }

    public class vwFlowSub
    {
        public string id { get; set; }
        public string pid { get; set; }
        [DisplayName("序號")]
        public int seq { get; set; }
        [DisplayName("簽核人")]
        public string signer { get; set; }
        [DisplayName("簽核類型")]
        public string signType { get; set; }
        [DisplayName("簽核結果")]
        public string signResult { get; set; }
        [DisplayName("簽核備註")]
        public string comment { get; set; }
        [DisplayName("簽核日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? signDate { get; set; }
    }
}