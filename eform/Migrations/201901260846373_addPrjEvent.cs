namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPrjEvent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrjEvents",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        prjId = c.String(),
                        billNo = c.String(),
                        billDate = c.DateTime(),
                        creator = c.String(),
                        subject = c.String(),
                        sMemo = c.String(),
                        beginHH = c.Int(nullable: false),
                        beginMM = c.Int(nullable: false),
                        endHH = c.Int(nullable: false),
                        endMM = c.Int(nullable: false),
                        beginDate = c.DateTime(),
                        endDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            AlterColumn("dbo.prjPMs", "WorkNo", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.prjPMs", "WorkNo", c => c.String());
            DropTable("dbo.PrjEvents");
        }
    }
}
