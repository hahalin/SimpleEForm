using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eform.Attributes;
namespace eform.Models
{
    [Table("deps")]
    public class dep
    {
        [Key]
        public string depNo { get; set; }
        [Display(Name ="部門名稱")]
        [Required(ErrorMessage ="請輸入名稱")]
        public string depNm { get; set; }
        public string parentDepNo { get; set;}
        public int depLevel { get; set; } = 1;
        [Display(Name = "序號")]
        public int sort { get; set; } = 1;
        public ICollection<jobPo> depJobPos { get; set; }

    }
}