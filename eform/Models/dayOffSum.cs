using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace eform.Models
{
    public class dayOffSum
    {
        [Key]
        public string id { get; set; }
        public string workNo { get; set; }
        public int y { get; set; }
        public int m { get; set; }
        public string sType { get; set; }
        public decimal v1 { get; set; } = 0;
        public decimal v2 { get; set; } = 0;
    }
}