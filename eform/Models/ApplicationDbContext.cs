using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace eform.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

            //Database.SetInitializer(new CustomMigration.eFormDBInitializer());

            Database.SetInitializer<ApplicationDbContext>(null);
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<dep> deps { get; set; }
        public DbSet<jobPo> jobPos { get; set; }
        public DbSet<FlowDefMain> FlowDefMainList { get; set; }
        public DbSet<FlowDefSub> FlowDefSubList { get; set; }
        public DbSet<FlowMain> FlowMainList { get; set; }
        public DbSet<FlowSub> FlowSubList { get; set; }
        public DbSet<news> newsList { get; set; }
        public DbSet<newsType> newsTypeList { get; set; }
        public DbSet<ReqOverTime> reqOverTimeList { get; set; }
        public DbSet<permission> permList { get; set; }
        public DbSet<permMod> permModList { get; set; }
        public DbSet<salary> salaryList { get; set; }

        public DbSet<dayOffType> dayOffTypeList { get; set; }
        public DbSet<dayOff> dayOffList { get; set; }
        public DbSet<dayOffSum> dayOffSumList { get; set; }


        public List<vwSignType> signTypeList()
        {
            List<vwSignType> list = new List<vwSignType>();
            list.Add(new vwSignType
            {
                id = 1,
                nm = "會簽"
            });
            list.Add(new vwSignType
            {
                id = 2,
                nm = "會簽-全部同意"
            });
            list.Add(new vwSignType
            {
                id = 3,
                nm = "直接許可"
            });
            return list;
        }
        public List<vwFlowStatus> flowStatusList()
        {
            List<vwFlowStatus> list = new List<vwFlowStatus>();

            list.Add(new vwFlowStatus
            {
                id=1,
                nm="簽核中"
            });

            list.Add(new vwFlowStatus
            {
                id = 2,
                nm = "核准"
            });

            list.Add(new vwFlowStatus
            {
                id = 3,
                nm = "退回"
            });

            return list;
        }

        public List<vwSignResult> signResultList()
        {
            List<vwSignResult> list = new List<vwSignResult>();
            list.Add(new vwSignResult
            {
                id = 1,
                nm = "同意"
            });
            list.Add(new vwSignResult
            {
                id = 2,
                nm = "不同意"
            });
            return list;
        }

        public DateTime getLocalTiime()
        {
            return DateTime.UtcNow.AddSeconds(28800);
        }

        public string getParentDeps(string depNo, ApplicationDbContext ctx)
        {
            string r = "";
            dep depObj = ctx.deps.Where(x => x.depNo == depNo).First();

            if (depObj != null)
            {
                r = depObj.depNm;
                if (depNo != "001")
                {
                    if (depObj.depLevel > 1)
                    {
                        r = getParentDeps(depObj.parentDepNo, ctx) + "-" + r;
                    }
                    else if (depObj.parentDepNo != "001")
                    {
                        r = depObj.depNm + "-" + r;
                    }
                }
            }

            return r;
        }

        public vwEmployee getCurrentUser(string  Name)
        {
            ApplicationUser user=this.Users.Where(x => x.UserName == Name).FirstOrDefault();
            vwEmployee employee = new vwEmployee{
                Id = user.Id,
                UserCName = user.cName,
                UserEName = user.eName,
                workNo = user.workNo,
                Email = user.Email,
                beginWorkDate=user.beginWorkDate
            };
            return employee;
        }
    }
}