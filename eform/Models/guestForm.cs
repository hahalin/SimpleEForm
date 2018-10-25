using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace eform.Models
{
    //FormCode:P011A1
    public class guestForm
    {
        [Key]
        public string id { get; set; }
        public string flowId { get; set; }
        public string formCode{
            get
            {
                return "P011A1";
            }
        }
        [DisplayName("申請日期")]
        public DateTime? requestDate { get; set; }
        [DisplayName("預計到廠")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("預計離廠")]
        public DateTime? dtEnd { get; set; }
        [DisplayName("拜訪部門")]
        public string toDep { get; set; }
        [DisplayName("拜訪對象")]
        public string to { get; set; }
        [DisplayName("來訪目的")]
        public string subject { get; set; }
        [DisplayName("來賓公司名稱")]
        public string guestCmp { get; set; }
        [DisplayName("來賓姓名")]
        public string guestName { get; set; }
        [DisplayName("來賓人數")]
        public int guestCnt { get; set; } = 1;
        [DisplayName("來賓手機號碼(選填)")]
        public string cellPhone { get; set; }
        [DisplayName("活動區域")]
        public bool area1 { get; set; } = false;
        public bool area2 { get; set; } = false;
        public string area21 { get; set; }
        public bool area3 { get; set; } = false;
        public bool area4 { get; set; } = false;
        public string area41 { get; set; }
        [DisplayName("備註")]
        public string sMemo { get; set; }
    }

    public class vwGuestForm
    {
        public string id { get; set; }
        public string flowId { get; set; }
        public vwEmployee user { get; set; }
        [DisplayName("申請日期")]
        public DateTime? requestDate { get; set; }
        [Required]
        [DisplayName("預計到廠")]
        public DateTime? dtBegin { get; set; }
        [Required]
        [DisplayName("預計離廠")]
        public DateTime? dtEnd { get; set; }
        [Required]
        [DisplayName("拜訪部門")]
        public string toDep { get; set; }
        [Required]
        [DisplayName("拜訪對象")]
        public string to { get; set; }
        [Required]
        [DisplayName("來訪目的")]
        public string subject { get; set; }
        [Required]
        [DisplayName("來賓公司名稱")]
        public string guestCmp { get; set; }
        [Required]
        [DisplayName("來賓姓名")]
        public string guestName { get; set; }
        [Required]
        [DisplayName("來賓人數")]
        public int guestCnt { get; set; } = 1;
        [DisplayName("來賓手機號碼(選填)")]
        public string cellPhone { get; set; }
        [DisplayName("活動區域")]
        public bool area1 { get; set; } = false;
        public bool area2 { get; set; } = false;
        public string area21 { get; set; }
        public bool area3 { get; set; } = false;
        public bool area4 { get; set; } = false;
        public string area41 { get; set; }
        [DisplayName("備註")]
        public string sMemo { get; set; }

        public int beginHH { get; set; } = 0;
        public int beginMM { get; set; } = 0;
        public int endHH { get; set; } = 0;
        public int endMM { get; set; } = 0;

    }
}