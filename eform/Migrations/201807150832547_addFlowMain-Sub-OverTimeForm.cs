namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFlowMainSubOverTimeForm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FlowMains",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        billNo = c.String(),
                        senderNo = c.String(),
                        billDate = c.DateTime(),
                        defId = c.String(),
                        flowName = c.String(),
                        flowStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.FlowSubs",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        pid = c.String(),
                        seq = c.Int(nullable: false),
                        workNo = c.String(),
                        signType = c.Int(nullable: false),
                        signResult = c.Int(nullable: false),
                        comment = c.String(),
                        signDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ReqOverTimes",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        flowId = c.String(),
                        dtBegin = c.DateTime(),
                        dtEnd = c.DateTime(),
                        hours = c.Int(nullable: false),
                        sMemo = c.String(),
                    })
                .PrimaryKey(t => t.id);
        }
        
        public override void Down()
        {
            DropTable("dbo.ReqOverTimes");
            DropTable("dbo.FlowSubs");
            DropTable("dbo.FlowMains");
        }
    }
}
