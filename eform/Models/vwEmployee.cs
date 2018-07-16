using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eform.Models
{
    public class vwEmployee
    {

        [HiddenInput(DisplayValue = true)]
        public string Id { get; set; }
        [Display(Name = "工號")]
        [Required]
        public string workNo { get; set; }
        [Required]
        [Display(Name = "姓名")]
        public string UserCName { get; set; }

        [Required]
        [Display(Name = "英文名")]
        public string UserEName { get; set; }

        [Display(Name = "密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "確認密碼")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string rePassword { get; set; }

        [Display(Name = "職稱")]
        public string Title { get; set; }

        public ICollection<vwPoNo> poList { get; set; }
    }

    
}