using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;


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
        public DbSet<OverTimeForm> overTimeFormList { get; set; }
        public DbSet<news> newsList { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // configures one-to-many relationship
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<jobPo>()
            //    .HasRequired<ApplicationUser>(u=>u.a)
            //    .HasForeignKey<int>(s => s.CurrentGradeId);
        }
    }
}