namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPublicOutTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.publicOuts",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        flowId = c.String(),
                        requestDate = c.DateTime(),
                        dtBegin = c.DateTime(),
                        dtEnd = c.DateTime(),
                        subject = c.String(),
                        transport = c.String(),
                        destination = c.String(),
                        jext = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            AlterColumn("dbo.ReqOverTimes", "hours", c => c.Decimal(nullable: false, precision: 10, scale: 1));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ReqOverTimes", "hours", c => c.Int(nullable: false));
            DropTable("dbo.publicOuts");
        }
    }
}
