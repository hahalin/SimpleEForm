using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eform.Models
{
    public class vwFlowRequest
    {
        public vwFlowRequest()
        {
            flowDefMain = new FlowDefMain();
            flowDefSubList = new List<FlowDefSub>();
            flowMain = new FlowMain();
            flowsSubList = new List<FlowSub>();
        }
        public FlowMain flowMain { get; set; }
        public List<FlowSub> flowsSubList { get; set; }
        public FlowDefMain flowDefMain { get; set; }
        public List<FlowDefSub> flowDefSubList { get; set; }
    }
}