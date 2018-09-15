using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eform.Models
{
    public class vwEmployee
    {

        [HiddenInput(DisplayValue = true)]
        public string Id { get; set; }
        [Display(Name = "工號")]
        [Required]
        public string workNo { get; set; }
        [Required]
        [Display(Name = "姓名")]
        public string UserCName { get; set; }

        [Required]
        [Display(Name = "英文名")]
        public string UserEName { get; set; }

        [Display(Name = "密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "確認密碼")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string rePassword { get; set; }

        [Display(Name = "職稱")]
        public string Title { get; set; }

        [Display(Name = "到職日")]
        //[Range(typeof(DateTime), "1980/1/1", "2030/12/31")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime? beginWorkDate { get; set; }

        public string sBeginWorkDate
        {
            get
            {
                string beginDate = "";
                try
                {
                    beginDate = Convert.ToDateTime(beginWorkDate).ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    beginDate = "";
                }
                return beginDate;
            }
            set
            {
                try
                { 
                    beginWorkDate = Convert.ToDateTime(value);
                }
                catch
                {

                }
            }
        }

        public string Email { get; set; }

        public string Company
        {
            get
            {
                if (workNo.ToUpper().Substring(0,1)=="H")
                {
                    return "群翌能源股份有限公司";
                }
                if (workNo.ToUpper().Substring(0, 1) == "A")
                {
                    return "亞洲氫能股份有限公司";
                }
                if (workNo.ToUpper().Substring(0, 1) == "S")
                {
                    return "上海群羿能源有限公司";
                }
                else
                {
                    return "群翌能源股份有限公司";
                }
                
            }
        }

        public ICollection<vwPoNo> poList { get; set; }
    }


}