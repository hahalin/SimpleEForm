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
        //[Unique(ErrorMessage ="部門名稱重複", TargetModelType = typeof(dep), TargetPropertyName = "depNm")]
        [Display(Name ="部門名稱")]
        public string depNm { get; set; }
        public string parentDepNo { get; set;}
        public ICollection<jobPo> depJobPos { get; set; }

    }
}