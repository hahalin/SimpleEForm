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
        const string defId= "DayOff";
        public vwDayOffForm(vwEmployee _user)
        {
            ApplicationDbContext ctx = new ApplicationDbContext();
            user = _user;
            dayOffList = ctx.FlowMainList.Where(x => x.senderNo == user.workNo && x.defId.Equals(defId)).ToList<FlowMain>();
       }
        
        public void createForm()
        {
            dayOffForm = new dayOff();
            dayOffForm.flowId = "DayOff";
            dayOffForm.dtBegin = DateTime.Today;
            dayOffForm.id = Guid.NewGuid().ToString();
        }

        [DisplayName("申請日期")]
        public DateTime? billDate { get; set; }

        public vwEmployee user { get; set; }
        public dayOff dayOffForm { get; set; }
        public List<FlowMain> dayOffList { get; set; }
    }
}