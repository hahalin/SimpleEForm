using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
namespace eform.Models
{
    public class vwReqOverTime
    {
        public string id { get; set; }
        public string flowId { get; set; }
        [DisplayName("單號")]
        public string billNo { get; set; }
        public string worker { get; set; }
        [DisplayName("申請日期")]
        public DateTime? billDate { get; set; }
        [DisplayName("部門")]
        public string depNo { get; set; }
        [DisplayName("部門")]
        public string depNm { get; set; }
        [DisplayName("職稱")]
        public string poNo { get; set; }
        [DisplayName("職稱")]
        public string poNm { get; set; }
        [DisplayName("開始時間")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("結束時間")]
        public DateTime? dtEnd { get; set; }
        public int beginHH { get; set; } = 0;
        public int beginMM { get; set; } = 0;
        public int endHH { get; set; } = 0;
        public int endMM { get; set; } = 0;

        [DisplayName("加班時數")]
        //[Range(1,12,ErrorMessage ="加班時數必須於1到12小時間")]
        //[Required(ErrorMessage ="加班時數必須輸入")]
        public decimal hours { get; set; } = 0;
        [DisplayName("申請事由")]
        public string sMemo { get; set; }
        [DisplayName("申請類型")]
        public string sType { get; set; } = "平日";
        [DisplayName("工作地點")]
        //[Required(ErrorMessage ="請選擇工作地點")]
        public string place { get; set; }
        public string otherPlace { get; set; } = "";
        [DisplayName("專案代碼")]
        public string prjId { get; set; }
        [DisplayName("水電氣使用說明")]
        public string sMemo2 { get; set; }
    }


    public class vwRealOverTime
    {
        public string id { get; set; }
        public vwEmployee user { get; set; }
        public string flowId { get; set; }
        [DisplayName("單號")]
        public string billNo { get; set; }
        public string worker { get; set; }
        [DisplayName("申請日期")]
        public DateTime? billDate { get; set; }
        [DisplayName("開始時間")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("結束時間")]
        public DateTime? dtEnd { get; set; }
        public int beginHH { get; set; } = 0;
        public int beginMM { get; set; } = 0;
        public int endHH { get; set; } = 0;
        public int endMM { get; set; } = 0;
        [DisplayName("小時數")]
        [Required]
        public decimal hours { get; set; } = 0;
        [DisplayName("加班事由")]
        [Required]
        public string sMemo { get; set; }
        [DisplayName("專案號碼")]
        public string prjId { get; set; }
        [DisplayName("備註")]
        public string sMemo2 { get; set; }
        public string signer { get; set; }
        public int flowStatus { get; set; }
        public string  sflowStatus {
            get {
                if(flowStatus==2)
                {
                    return "核准";
                }
                if (flowStatus == 3)
                {
                    return "退回";
                }
                else
                {
                    return "簽核中";
                }
                
            }
        }
    }
}