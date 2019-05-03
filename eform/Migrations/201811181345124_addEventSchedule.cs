namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEventSchedule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventSchedules",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        flowId = c.String(),
                        eventType = c.Int(nullable: false),
                        subject = c.String(),
                        location = c.String(),
                        sMemo = c.String(),
                        beginHH = c.Int(nullable: false),
                        beginMM = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EventSchedules");
        }
    }
}
