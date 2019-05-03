namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventScheduleAddTwoDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventSchedules", "beginDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.EventSchedules", "endDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventSchedules", "endDate");
            DropColumn("dbo.EventSchedules", "beginDate");
        }
    }
}
