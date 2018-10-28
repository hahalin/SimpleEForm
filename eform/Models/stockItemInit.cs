using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace eform.Models
{
    public class stockItemInit
    {
        [Key]
        public string id { get; set; }
        [DisplayName("產品代號")]
        public string pdId { get; set; }
        [DisplayName("品名")]
        public string pdNm { get; set; }
        [DisplayName("規格")]
        public string spec { get; set; }
        [DisplayName("日期(轉換)")]
        public string dateA { get; set; }
        [DisplayName("單別名稱")]
        public string billNm { get; set; }
        [DisplayName("進銷單號")]
        public string billInNo { get; set; }
        [DisplayName("發票號碼")]
        public string InvNo { get; set; }
        [DisplayName("廠牌代號")]
        public string brandNo { get; set; }
        [DisplayName("對方品名/品名備註")]
        public string supPdId { get; set; }
        [DisplayName("客戶供應商代號")]
        public string supId { get; set; }
        [DisplayName("客戶供應商簡稱")]
        public string supNm { get; set; }
        [DisplayName("進銷-備註(M)")]
        public string smemoA { get; set; }
        [DisplayName("備註")]
        public string smemoB { get; set; }
        [DisplayName("數量")]
        public decimal qty { get; set; }
        [DisplayName("登打單價")]
        public decimal priceA { get; set; } = 0;
        [DisplayName("明細金額")]
        public decimal amtA { get; set; } = 0;
        [DisplayName("明細含稅金額")]
        public decimal amtB { get; set; } = 0;
        [DisplayName("外幣代號")]
        public string curId { get; set; }
        [DisplayName("匯率")]
        public decimal curRate { get; set; } = 0;
        [DisplayName("外幣登打單價")]
        public decimal priceB { get; set; } = 0;
        [DisplayName("明細外幣金額")]
        public decimal amtC { get; set; } = 0;
        [DisplayName("明細外幣含稅金額")]
        public decimal amtD { get; set; } = 0;
        [DisplayName("專案/項目編號")]
        public string prjId { get; set; }
        [DisplayName("專案/項目名稱")]
        public string prjNm { get; set; }
        [DisplayName("採購單號")]
        public string reqId { get; set; }
        [DisplayName("採訂單號")]
        public string reqId2 { get; set; }
        [DisplayName("到貨日期")]
        public string dateB { get; set; }
    }
}