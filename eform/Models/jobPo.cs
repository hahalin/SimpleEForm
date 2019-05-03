using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
namespace eform.Models
{
    public class jobPo
    {
        [Key]
        public string poNo { get; set; }
        public string depNo { get; set; }
        [Display(Name ="職稱")]
        public string poNm { get; set; }
        [Display(Name = "部門表單簽核權限")]
        public bool isFormSigner { get; set; } = false;
    }

    public class PoUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        ///public string depNo { get; set; }
        public string poNo { get; set; }
    }
}