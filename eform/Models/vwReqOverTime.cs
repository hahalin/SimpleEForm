﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
namespace eform.Models
{
    public class vwReqOverTime
    {
        public string id { get; set; }
        public string flowId { get; set; }
        [DisplayName("申請日期")]
        public DateTime? billDate { get; set; }
        [DisplayName("開始時間")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("結束時間")]
        public DateTime? dtEnd { get; set; }
        [DisplayName("加班時數")]
        [Range(1,12,ErrorMessage ="加班時數必須於1到12小時間")]
        [Required(ErrorMessage ="加班時數必須輸入")]
        public int hours { get; set; } = 0;
        [DisplayName("備註")]
        public string sMemo { get; set; }
    }
}