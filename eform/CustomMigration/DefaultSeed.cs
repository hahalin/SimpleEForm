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

            MinimumLengthValidator valid = (MinimumLengthValidator)manager.PasswordValidator;
            valid.RequiredLength = 4;

            var user = new ApplicationUser
            {
                UserName = "Admin",
                workNo = "Admin"
            };

            try
            {
                var r = manager.Create(user, "a@123456");
            }
            catch (Exception ex)

            {
                var r = ex;
            }

            //https://stackoverflow.com/questions/24389126/mvc5-usermanager-addtorole-error-adding-user-to-role-userid-not-found
            var newUser = manager.FindByName(user.UserName);
            manager.AddToRole(newUser.Id, "Admin");

            user = new ApplicationUser
            {
                UserName = "sadmin",
                workNo = "sadmin"
            };
            manager.Create(user, "a@123456");
            newUser = manager.FindByName(user.UserName);
            manager.AddToRole(newUser.Id, "Admin");
            return true;
        }

        public static bool importDefaultRole(eform.Models.ApplicationDbContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            var store = new RoleStore<ApplicationRole>(context);

            var manager = new RoleManager<ApplicationRole>(store);

            var role = new ApplicationRole { Name = "Admin", isDefault = true, Description = "系統管理員" };
            manager.Create(role);
            role = new ApplicationRole { Name = "Employee", isDefault = true, Description = "員工" };
            manager.Create(role);
            return true;
        }

        static void createFlowDef(ApplicationDbContext ctx)
        {
            int idx = 6;
            if (ctx.FlowDefMainList.Where(x=>x.code== "P020A1").Count()==0)
            {
                FlowDefMain defObj = new FlowDefMain
                {
                    id = Guid.NewGuid().ToString(),
                    code = "P020A1",
                    nm= "廠務派工及總務需求申請單",
                    seq=6,
                    enm="ReqInHouse"
                };
                ctx.FlowDefMainList.Add(defObj);
                ctx.SaveChanges();
            }
            if (ctx.FlowDefMainList.Where(x => x.code == "B001A1").Count() == 0)
            {
                FlowDefMain defObj = new FlowDefMain
                {
                    id = Guid.NewGuid().ToString(),
                    code = "B001A1",
                    nm = "公司行程規劃表",
                    seq = 7,
                    enm = "EventSchedule"
                };
                ctx.FlowDefMainList.Add(defObj);
                ctx.SaveChanges();
            }
        }

        static void createPermMod(ApplicationDbContext ctx)
        {
            if (ctx.permModList.Where(x=>x.mod=="PrjCreateCode").Count()==0)
            {
                permMod obj = new permMod
                {
                    mod = "PrjCreateCode",
                    modCname = "專案代碼管理"
                };
                ctx.permModList.Add(obj);
                ctx.SaveChanges();
            }
            else
            {
                permMod obj = ctx.permModList.Where(x => x.mod == "PrjCreate").FirstOrDefault();
                obj.modCname = "專案代碼管理";
                ctx.SaveChanges();
            }
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
                var context = new Models.ApplicationDbContext();
                importDefaultRole(context);
                importDefaultAdmin(context);

                dbHelper dbh = new dbHelper();
                dbh.execSql("update deps set sort=1 where sort=null");

                if (context.newsTypeList.Count() == 0)
                {
                    seedingNewsTypeList(context);
                }

                if (context.dayOffTypeList.Count() == 0)
                {
                    context.dayOffTypeList.Add(new dayOffType { k = 1, v = "事假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 2, v = "家庭照顧假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 3, v = "普通傷病假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 4, v = "生理假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 5, v = "公傷病假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 6, v = "婚假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 7, v = "喪假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 8, v = "陪產假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 9, v = "產檢假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 10, v = "產假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 11, v = "安胎假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 12, v = "公假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 13, v = "特別休假" });
                    context.dayOffTypeList.Add(new dayOffType { k = 14, v = "補休" });
                    context.SaveChanges();
                }

                createFlowDef(context);
                createPermMod(context);
            }
            return true;
        }
        public static void seedingNewsTypeList(ApplicationDbContext context)
        {
            context.newsTypeList.Add(new newsType { seq = 1, code = "9000-營運處" });
            context.newsTypeList.Add(new newsType { seq = 2, code = "1000-測試設備製造處" });
            context.newsTypeList.Add(new newsType { seq = 3, code = "1200-測試設備開發處" });
            context.newsTypeList.Add(new newsType { seq = 4, code = "2000-燃料電池材料業務處" });
            context.newsTypeList.Add(new newsType { seq = 5, code = "3000-電子計量產品業務處" });
            context.newsTypeList.Add(new newsType { seq = 6, code = "4000-自動化設備處" });
            context.newsTypeList.Add(new newsType { seq = 7, code = "5000-特用碳材料業務處" });
            context.newsTypeList.Add(new newsType { seq = 8, code = "5100-上海群羿碳材料業務處" });
            context.newsTypeList.Add(new newsType { seq = 9, code = "6000-開發處-亞氫" });
            context.newsTypeList.Add(new newsType { seq = 10, code = "7000-Yanmar 業務暨工程處" });
            context.SaveChanges();
        }
    }
}