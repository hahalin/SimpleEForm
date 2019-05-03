namespace eform.Migrations
{
    using eform.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration_Backup : DbMigrationsConfiguration<eform.Models.ApplicationDbContext>
    {
        public Configuration_Backup()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "eform.Models.ApplicationDbContext";
        }

        protected override void Seed(eform.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            importDefaultRole(context);
            importDefaultAdmin(context);
        }

        void importDefaultAdmin(eform.Models.ApplicationDbContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            var passwordHasher = new PasswordHasher();
            var user = new ApplicationUser
            {
                UserName = "admin"
            };

            try
            {
                var r = manager.Create(user, "123456");
            }
            catch(Exception ex)

            {
                var r = ex;
            }

            //https://stackoverflow.com/questions/24389126/mvc5-usermanager-addtorole-error-adding-user-to-role-userid-not-found
            var newUser = manager.FindByName(user.UserName);
            manager.AddToRole(newUser.Id, "admin");
            //newUser = manager.FindByName("test2");
            //manager.AddToRole(newUser.Id, "admin");
        }

        void importDefaultRole(eform.Models.ApplicationDbContext context)
        {
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            var role = new IdentityRole { Name = "Admin" };
            manager.Create(role);
            role = new IdentityRole { Name = "GM" };
            manager.Create(role);
            role = new IdentityRole { Name = "FS" };
            manager.Create(role);
            role = new IdentityRole { Name = "HR" };
            manager.Create(role);

        }
    }
}
