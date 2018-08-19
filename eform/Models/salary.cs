using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eform.Attributes;

namespace eform.Models
{
    public class salary
    {
        [Key]
        public string id { get; set; }
        public int year { get; set; }
        public int month { get; set;}
        public string workNo { get; set; }
        public string jData { get; set; }
    }
}