using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace eform.Models
{
    public class permission
    {
        [Key]
        public string id { get; set; }
        [Display(Name ="工號")]
        public string workNo { get; set; }
        public string mod { get; set; }
        public string modItem { get; set; }
    }
}