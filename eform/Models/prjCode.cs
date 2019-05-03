using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace eform.Models
{
    public class prjCode
    {
        [Key]
        public string id { get; set; }
        public string code { get; set; }
        public string nm { get; set; }
        public string owner { get; set; }
        public string status { get; set; }
        public string creator { get; set; }
        public DateTime? createDate { get; set; }
        public string mmo1 { get; set; }
        public string mmo2 { get; set; }
        public string mmo3 { get; set; }
        public string mmo4 { get; set; }
        public string mmo5 { get; set; }
        public string mmo6 { get; set; }
        public string mmo7 { get; set; }
        public string mmo8 { get; set; }
        public string mmo9 { get; set; }
        public string mmo10 { get; set; }
        public string mmo11 { get; set; }
    }
    public class prjPM { 
        [Key]
        public string id { get; set; }
        public string pid { get; set; }
        [Required]
        [Display(Name ="工號")]
        public string WorkNo { get; set; }
        [Display(Name = "職稱")]
        public string Title { get; set; }
    }
    public class prjForumUser
    {
        [Key]
        public string id { get; set; }
        public string pid { get; set; }
        public string WorkNo { get; set; }
    }

    public class vwPrjForumUser
    {
        public string id { get; set; }
        public string pid { get; set; }
        [Display(Name = "工號")]
        [Required]
        public string WorkNo { get; set; }
        public string cName { get; set; }
    }

    public class vwPrjCode
    {
        public vwPrjCode()
        {
        }

        public string id { get; set; }
        [Display(Name = "專案編號")]
        [Required]
        public string code { get; set; }
        [Display(Name = "專案名稱")]
        [Required]
        public string nm { get; set; }
        [Display(Name = "專案經理")]
        [Required]
        public string owner { get; set; }
        [Display(Name = "專案狀態")]
        public string status { get; set; }
        [Display(Name = "備註1")]
        public string mmo1 { get; set; }
        [Display(Name = "備註2")]
        public string mmo2 { get; set; }
        [Display(Name = "備註3")]
        public string mmo3 { get; set; }
        [Display(Name = "備註4")]
        public string mmo4 { get; set; }
        [Display(Name = "備註5")]
        public string mmo5 { get; set; }
        [Display(Name = "備註6")]
        public string mmo6 { get; set; }
        [Display(Name = "備註7")]
        public string mmo7 { get; set; }
        [Display(Name = "備註8")]
        public string mmo8 { get; set; }
        [Display(Name = "備註9")]
        public string mmo9 { get; set; }
        [Display(Name = "備註10")]
        public string mmo10 { get; set; }
        [Display(Name = "備註11")]
        public string mmo11 { get; set; }
        public string creator { get; set; }
        public DateTime? createDate { get; set; }
        public string hPMList { get; set; }
        public string ownerNm {
            get
            {
                if (!string.IsNullOrEmpty(owner))
                {
                    ApplicationDbContext ctx = new ApplicationDbContext();
                    return ctx.getUserByWorkNo(owner).UserCName;
                }
                return "";
            }
        }

        public string ownerStr
        {
            get
            {
                if (string.IsNullOrEmpty(owner))
                {
                    return "";
                }
                else
                {
                    return owner + "-" + ownerNm;
                }
            }
        }

        public string createDateStr
        {
            get
            {
                try
                {
                    string r= Convert.ToDateTime(createDate).ToString("yyyy-MM-dd");
                    if (r.Substring(0,2)=="00")
                    {
                        return "";
                    }
                    return r;
                }
                catch(Exception ex)
                {
                    return "";
                }
            }
        }
        public string creatorNm
        {
            get
            {
                if (!string.IsNullOrEmpty(creator))
                {
                    ApplicationDbContext ctx = new ApplicationDbContext();
                    return ctx.getUserByWorkNo(creator).UserCName;
                }
                return null;
            }
        }

        public string creatorStr
        {
            get
            {
                if (string.IsNullOrEmpty(creator))
                {
                    return "";
                }
                else
                {
                    return creator + "-" + creatorNm;
                }
            }
        }
    }

}
