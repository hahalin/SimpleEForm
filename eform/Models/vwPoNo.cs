using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace eform.Models
{
    public class vwPoNo
    {
        public string depNo { get; set; }
        public string depNm { get; set; }
        public string poNo { get; set; }
        public string poNm { get; set; }
        [Display(Name = "部門表單簽核權限")]
        public bool isFormSigner { get; set; }
    }
}