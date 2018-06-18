using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using eform.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace eform.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        // GET: Role
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();
            var store = new RoleStore<ApplicationRole>(context);
            var manager = new RoleManager<ApplicationRole>(store);

            List<vwRole> model = new List<vwRole>();
            foreach(var role in store.Roles)
            {
                model.Add(new vwRole
                {
                    Name=role.Name,
                    description=role.Description
                });
            }
            return View(model);
        }
    }
}