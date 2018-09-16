using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace eform.Models
{
    public class vwDayOffForm
    {
        public string defId = "DayOff";
        private vwEmployee _user;
        ApplicationDbContext ctx;
        public vwDayOffForm()
        {
            ctx = new ApplicationDbContext();
            createForm();

        }
        public vwEmployee user
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                dayOffList = ctx.FlowMainList.Where(x => x.senderNo == value.workNo && x.defId.Equals(defId)).ToList<FlowMain>();
            }
        }
        void createForm()
        {
            dayOffForm = new vwdayOff();
            dayOffForm.flowId = "DayOff";
            dayOffForm.dtBegin = DateTime.Today;
            dayOffForm.id = Guid.NewGuid().ToString();
        }

        [DisplayName("申請日期")]
        public DateTime? billDate { get; set; }
        public vwdayOff dayOffForm { get; set; }
        public int beginHH { get; set; } = 0;
        public int beginMM { get; set; } = 0;
        public int endHH { get; set; } = 0;
        public int endMM { get; set; } = 0;
        [DisplayName("假別")]
        [Required]
        public string dType { get; set; }
        public List<FlowMain> dayOffList { get; set; }
    }

    public class vwdayOff
    {
        public string id { get; set; }
        public string flowId { get; set; }
        [DisplayName("開始時間")]
        public DateTime? dtBegin { get; set; }
        [DisplayName("結束時間")]
        public DateTime? dtEnd { get; set; }
        [DisplayName("假別")]
        public string dType { get; set; }
        [DisplayName("小時數")]
        public decimal hours { get; set; }
        [DisplayName("職務代理人")]
        [Required]
        public string jobAgent { get; set; }
        public vwEmployee jobAgentUser
        {
            get
            {
                ApplicationDbContext ctx = new ApplicationDbContext();
                return ctx.getUserByWorkNo(jobAgent);
            }
        }
        [DisplayName("請假事由")]
        [Required]
        public string sMemo { get; set; }
        public string jext { get; set; }
    }
}