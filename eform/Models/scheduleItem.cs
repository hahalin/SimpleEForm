using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eform.Models
{
    public class scheduleItem
    {
        [Key]
        public string id { get; set; }
        public string prjId { get; set; }
        public int seq { get; set; }
        public string itemTxt { get; set; }
        public DateTime? dtBegin { get; set; }
        public DateTime? dtEnd { get; set; }
    }

    public class vwSchItemList
    {
        public vwSchItemList()
        {
            itemList = new List<vwScheduleItem>();
        }

        public prjCode prj { get; set; }

        public List<vwScheduleItem> itemList { get; set; }
    }

    public class vwScheduleItem
    {
        public string id { get; set; }
        public string prjId { get; set; }
        public int seq { get; set; }
        public string itemTxt { get; set; }
        public DateTime? dtBegin { get; set; }
        public DateTime? dtEnd { get; set; }
    }

    public class schTemp
    {
        [Key]
        public string id { get; set; }
        [Required]
        [Display(Name ="分類編號")]
        public string code { get; set; }
        public string txt { get; set; }
    }

    public class schTempItem
    {
        [Key]
        public string id { get; set; }
        public string pid { get; set; }
        [Display(Name = "序號")]
        [Required]
        public int seq { get; set; } = 0;
        [Display(Name = "內容")]
        [Required]
        public string txt { get; set; }
    }




}