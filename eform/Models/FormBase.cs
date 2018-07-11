using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eform.Models
{
    public class FormBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string billNo { get; set; }
        public DateTime billDate { get; set; }
        public string Title { get; set; }
        public string senderNo { get; set; }
        
    }
}