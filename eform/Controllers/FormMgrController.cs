using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eform.Attributes;
using eform.Models;

namespace eform.Controllers
{
    public class FormMgrController : Controller
    {
        // GET: FlowMgr
        public ActionResult Index()
        {
            return View();
        }
    }
}