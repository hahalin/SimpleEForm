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
        public string id { get; set; }
        [DisplayName("工號")]
        public string workNo { get; set; }
        [DisplayName("申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreateDate { get; set; }
        [DisplayName("開始時間")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("結束時間")]
        public DateTime? dtEnd { get; set; }
        [DisplayName("備註")]
        public string memo { get; set; }
        [DisplayName("簽核狀態")]
        public int signType { get; set; } = 0;
    }
}