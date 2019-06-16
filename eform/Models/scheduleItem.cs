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
        [Required()]
        [Display(Name = "序號")]
        public int seq { get; set; }
        [Required()]
        [Display(Name = "工作項目")]
        public string itemTxt { get; set; }
        [Required()]
        [Display(Name ="開始日期")]
        public DateTime? dtBegin { get; set; }
        [Required()]
        [Display(Name = "結束日期")]
        public DateTime? dtEnd { get; set; }

        [Display(Name = "開始日期")]
        public string dtBeginStr
        {
            get
            {
                return dtBegin==null? "" : dtBegin.Value.ToString("yyyy-MM-dd");
            }
        }
        [Display(Name = "開始日期")]
        public string dtEndStr
        {
            get
            {
                return dtEnd == null ? "" : dtEnd.Value.ToString("yyyy-MM-dd");
            }
        }
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