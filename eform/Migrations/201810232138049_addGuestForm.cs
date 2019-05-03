namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addGuestForm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.guestForms",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        flowId = c.String(),
                        requestDate = c.DateTime(),
                        dtBegin = c.DateTime(),
                        dtEnd = c.DateTime(),
                        toDep = c.String(),
                        to = c.String(),
                        subject = c.String(),
                        guestCmp = c.String(),
                        guestName = c.String(),
                        guestCnt = c.Int(nullable: false),
                        cellPhone = c.String(),
                        area1 = c.Boolean(nullable: false),
                        area2 = c.Boolean(nullable: false),
                        area21 = c.String(),
                        area3 = c.Boolean(nullable: false),
                        area4 = c.Boolean(nullable: false),
                        area41 = c.String(),
                        sMemo = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.guestForms");
        }
    }
}
