namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addColorTag2EventSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventSchedules", "colorTag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventSchedules", "colorTag");
        }
    }
}
