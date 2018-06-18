using eform.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace eform.CustomMigration
{
    //http://johnatten.com/2014/08/10/asp-net-identity-2-0-implementing-group-based-permissions-management/
    public class DefaultSeed
    {
        public static bool importDefaultAdmin(eform.Models.ApplicationDbContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);

            MinimumLengthValidator valid =(MinimumLengthValidator) manager.PasswordValidator;
            valid.RequiredLength = 4;

            var user = new ApplicationUser
            {
                UserName = "Admin",
                workNo="Admin"
            };

            try
            {
                var r = manager.Create(user, "1234");
            }
            catch (Exception ex)

            {
                var r = ex;
            }

            //https://stackoverflow.com/questions/24389126/mvc5-usermanager-addtorole-error-adding-user-to-role-userid-not-found
            var newUser = manager.FindByName(user.UserName);
            manager.AddToRole(newUser.Id, "Admin");
            return true;
        }

        public static bool importDefaultRole(eform.Models.ApplicationDbContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            var store = new RoleStore<ApplicationRole>(context);

            var manager = new RoleManager<ApplicationRole>(store);

            var role = new ApplicationRole { Name = "Admin",isDefault=true,Description="系統管理員"};
            manager.Create(role);
            role = new ApplicationRole { Name = "Employee",isDefault=true,Description="員工" };
            manager.Create(role);
            return true;
        }

        public static bool doMigrate(DbMigrationsConfiguration config)
        {
            var migrator = new DbMigrator(config);
            bool _pendingMigrations = migrator.GetPendingMigrations().Any();

            // If there are pending migrations run migrator.Update() to create/update the database then run the Seed() method to populate
            //  the data if necessary
            if (_pendingMigrations)
            {
                migrator.Update();
                var context=new Models.ApplicationDbContext();
                importDefaultRole(context);
                importDefaultAdmin(context);
            }
            return true;
        }
    }
}