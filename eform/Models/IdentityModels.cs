using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;

namespace eform.Models
{
    // 您可將更多屬性新增至 ApplicationUser 類別，藉此為使用者新增設定檔資料，如需深入了解，請瀏覽 https://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 注意 authenticationType 必須符合 CookieAuthenticationOptions.AuthenticationType 中定義的項目
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在這裡新增自訂使用者宣告
            return userIdentity;
        }
        public string workNo { get; set; }
        public string cName { get; set; }
        public string eName { get; set; }
        public int status { get; set; } = 1;
        public DateTime? beginWorkDate { get; set; }
        public ICollection<PoUser> poList { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }
        public string Description { get; set; }
        public bool isDefault { get; set; } = false;
    }

    //public class ApplicationRoleManager : RoleManager<IdentityRole>
    //{
    //    public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
    //        : base(roleStore)
    //    {
    //    }

    //    public static ApplicationRoleManager Create(
    //        IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
    //    {
    //        return new ApplicationRoleManager(
    //            new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
    //    }
    //}

}
