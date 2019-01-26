namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateEventSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventSchedules", "endHH", c => c.Int(nullable: false));
            AddColumn("dbo.EventSchedules", "endMM", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventSchedules", "endMM");
            DropColumn("dbo.EventSchedules", "endHH");
        }
    }
}
