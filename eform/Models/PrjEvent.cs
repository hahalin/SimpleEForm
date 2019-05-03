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
    public class PrjEvent
    {
        [Key]
        public string id { get; set; }
        public string prjId { get; set; }
        public string billNo { get; set; }
        public DateTime? billDate { get; set; }
        public string creator { get; set; }
        public string subject { get; set; }
        public string sMemo { get; set; }
        public int beginHH { get; set; } = 9;
        public int beginMM { get; set; } = 0;
        public int endHH { get; set; } = 9;
        public int endMM { get; set; } = 0;
        public DateTime? beginDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class vwPrjEvent
    {
        ApplicationDbContext ctx;
        public vwPrjEvent()
        {
             ctx= new ApplicationDbContext();
        }
        public string id { get; set; }
        public string prjId { get; set; }
        public string formCode
        {
            get
            {
                return "P050A1";
            }
        }
        public string formEnm
        {
            get
            {
                return "EventPrj";
            }
        }
        [DisplayName("單號")]
        public string billNo { get; set; }
        [DisplayName("申請日期")]
        [Required(ErrorMessage = "請輸入申請日期")]
        public DateTime? billDate { get; set; }
        public string creator { get; set; }
        [DisplayName("標題")]
        [Required(ErrorMessage = "請輸入標題")]
        public string subject { get; set; }
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
        public vwEmployee user
        {
            get
            {
                return ctx.getUserByWorkNo(this.creator);
            }
        }
        public prjCode prjCode
        {
            get
            {
                return ctx.prjCodeList.Where(x => x.id == this.prjId).FirstOrDefault();
            }
        }

        public string prjStr
        {
            get
            {
                return prjCode.code + " " + prjCode.nm;
            }
        }

        public string sBeginDate
        {
            get
            {
                return Convert.ToDateTime(beginDate).ToString("yyyy-MM-dd") + beginHH.ToString("0#") + ":" + beginMM.ToString("0#");
            }
        }
        public string sEndDate
        {
            get
            {
                return Convert.ToDateTime(endDate).ToString("yyyy-MM-dd") + endHH.ToString("0#") + ":" + endMM.ToString("0#");
            }
        }
    }
}
