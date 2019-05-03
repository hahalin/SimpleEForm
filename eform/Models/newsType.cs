using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace eform.Models
{
    public class newsType
    {
        [Key]
        public string code { get; set; }
        public int seq { get; set; } = 0;
    }
}