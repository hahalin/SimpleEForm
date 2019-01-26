using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eform.Attributes;

namespace eform.Models
{
    public class prjWork
    {
        [Key]
        public string id { get; set; }
        public string workNo { get; set; }
        public int y { get; set; }
        public int w { get; set; }
    }

    public class vwPrjWork
    {
        public vwPrjWork()
        {
            hList = "";
        }
        public string id { get; set; }
        [Display(Name ="申請人")]
        public string workNo { get; set; }
        public vwEmployee user {
            get
            {
                var ctx = new ApplicationDbContext();
                return ctx.getUserByWorkNo(workNo);
            }
        }
        public string workNoStr
        {
            get
            {
                if (string.IsNullOrEmpty(workNo))
                {
                    return "";
                }
                else
                {
                    return workNo + "-" + user.UserCName;
                }
            }
        }
        [Display(Name = "年度")]
        [Required(ErrorMessage = "請輸入年度")]
        public int y { get; set; }
        [Display(Name = "周別")]
        [Required(ErrorMessage = "請輸入周別")]
        [Range(1, 53, ErrorMessage = "合理的周別應該是1至53這個區間")]
        public int w { get; set; }
        public string DateRange
        {
            get;
            set;
        }
        public string hList { get; set; }
        public string hRemoveList { get; set; }
    }

    public class prjWorkItem
    {
        [Key]
        public string id { get; set; }
        public string pid { get; set; }
        public DateTime? dt { get; set; }
        public string prjCode { get; set; }
        public decimal hours { get; set; }
        public string subject { get; set; }
        public string smemo { get; set; }
        public string pm { get; set; }
        public string gm { get; set; }
        public string mgr1 { get; set; }
        public string mgr2 { get; set; }
        public string mgr3 { get; set; }

    }


}