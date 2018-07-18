using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace eform.Models
{
    [Table("news")]
    public class news
    {
        [Key]
        public string id { get; set; }
        [Display(Name = "公告日期")]
        public DateTime? ndate { get; set; }
        public string uid { get; set; }
        [Display(Name ="標題")]
        [Required]
        public string title { get; set; }
        [Display(Name = "內容")]
        [Required]
        [AllowHtml]
        public string content { get; set; }
        public bool isActive { get; set; } = true;
        public DateTime? createTime { get; set; }
        [Display(Name = "夾檔")]
        public string fileUrl { get; set; }
    }
}