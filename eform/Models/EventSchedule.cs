using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Web.Mvc;

namespace eform.Models
{
    public class EventSchedule
    {
        [Key]
        public string id { get; set; }
        public string flowId { get; set; }
        public int eventType { get; set; } = 1;
        public string subject { get; set; }
        public string location { get; set; }
        public string sMemo { get; set; }
        public int beginHH { get; set; } = 9;
        public int beginMM { get; set; } = 0;
        public int endHH { get; set; } = 9;
        public int endMM { get; set; } = 0;
        public DateTime? beginDate { get; set; }
        public DateTime? endDate { get; set; }
        public string colorTag { get;set; }

    }

    public class vwEventSchedule
    {
        public vwEventSchedule()
        {
            eventTypeList = new List<SelectListItem>();
            eventTypeList.Add(new SelectListItem
            {
                Value = "1",
                Text = "業務行程"
            });
            eventTypeList.Add(new SelectListItem
            {
                Value = "2",
                Text = "公司活動"
            });
            eventTypeList.Add(new SelectListItem
            {
                Value = "3",
                Text = "會議室預約"
            });

        }
        public string id { get; set; }
        public string flowId { get; set; }
        public string formCode
        {
            get
            {
                return "B001A1";
            }
        }
        public string formEnm
        {
            get
            {
                return "EventSchedule";
            }
        }
        public vwEmployee user { get; set; }
        [DisplayName("單號")]
        public string billNo { get; set; }
        [DisplayName("申請日期")]
        [Required(ErrorMessage = "請輸入申請日期")]
        public DateTime? billDate { get; set; }
        [DisplayName("活動種類")]
        public int eventType { get; set; } = 1;
        [DisplayName("活動種類")]
        public string vEventType
        {
            get
            {
               if (eventType==1)
                {
                    return "業務行程";
                }
                else if (eventType == 2)
                {
                    return "公司活動";
                }
                else 
                {
                    return "會議室預約";
                }
            }
        }
        public List<SelectListItem> eventTypeList {
            get;set;
        }
        [DisplayName("標題")]
        [Required(ErrorMessage = "請輸入標題")]
        public string subject { get; set; }
        [DisplayName("地點")]
        [Required(ErrorMessage = "請輸入地點")]
        public string location { get; set; }
        [DisplayName("備註")]
        public string sMemo { get; set; }
        [DisplayName("開始時間")]
        public int beginHH { get; set; } = 9;
        public int beginMM { get; set; } = 0;
        [DisplayName("結束時間")]
        public int endHH { get; set; } = 9;
        public int endMM { get; set; } = 0;
        [DisplayName("開始日期")]
        [Required(ErrorMessage = "請輸入開始日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? beginDate { get; set; }
        [DisplayName("結束日期")]
        [Required(ErrorMessage = "請輸入結束日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? endDate { get; set; }
        [DisplayName("顯示設定")]
        public string colorTag { get; set; }
        public List<SelectListItem> colorTagList
        {
            get
            {
                List<SelectListItem> vlist = new List<SelectListItem>();
                //vlist.Add(new SelectListItem
                //{
                //    Value = "",
                //    Text = ""
                //});
                for (int i = 1; i <= 6; i++)
                {
                    vlist.Add(new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = "A",
                        Selected = i.ToString()==this.colorTag
                    });
                };
                return vlist;
            }
        }

    }
}