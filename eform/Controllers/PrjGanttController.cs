using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Web.Mvc;
using eform.Models;
using eform.Repo;
using eform.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using Dapper;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using System.Text;
using eform.Service;
using System.Web.Routing;


namespace eform.Controllers
{
    [Authorize]
    public partial class PrjMgrController : Controller
    {
        // GET: PrjGantt
        public ActionResult TimeGrid()
        {

            prjRep rep = new prjRep(User.Identity.Name);

            var model = new Models.PrjGantt.fmGanttAssignUser
            {
                hmList = rep.getTimeSelection(),
                userlist=ctx.getUserList()
            };

            return View(model);
        }
    }
}