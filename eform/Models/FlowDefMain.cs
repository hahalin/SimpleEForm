﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eform.Attributes;
namespace eform.Models
{
    [Table("FlowDefMain")]
    public class FlowDefMain
    {
        [Key]
        public string id { get; set; }
        [Display(Name = "表單名稱")]
        [Required]
        public string nm { get; set; }
        public string enm { get; set; }
        [MaxLength(20)]
        public string code { get; set; }
        public int seq { get; set; } = 1;
        public string wording { get; set; }
    }

    [Table("FlowDefSub")]
    public class FlowDefSub
    {
        [Key]
        public string id { get; set; }
        public string pid { get; set; }
        [Display(Name = "序號")]
        [Required]
        public int seq { get; set; } = 1;
        [Display(Name = "工號")]
        [Required]
        public string workNo { get; set; }
        [Display(Name = "簽核類型")]
        [Required]
        public int signType { get; set; } = 1;
    }

    public class vwFlowDefSub
    {
        public string id { get; set; }
        public string pid { get; set; }
        [Display(Name = "序號")]
        [Required]
        public int seq { get; set; } = 1;
        [Display(Name = "工號")]
        [Required]
        public string workNo { get; set; }
        [Display(Name = "簽核類型")]
        [Required]
        public int signType { get; set; }
        public string UserCName { get; set; }
        public string singTypeName
        {
            get
            {
                switch (signType)
                {
                    case 1:
                        return "會簽";
                    case 2:
                        return "會簽-全部同意";
                    case 3:
                        return "直接許可";
                    case 4:
                        return "串簽";
                    case 701:
                        return "人事登錄";
                    case 801:
                        return "代理人";
                    default:
                        return "會簽";
                }
            }
        }
    }

    public class vwFlowDefWording
    {
        public string code { get; set; }
        public string wording { get; set; }
    }
}