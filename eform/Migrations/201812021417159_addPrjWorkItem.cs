namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPrjWorkItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.prjWorkItems",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        pid = c.String(),
                        dt = c.DateTime(),
                        prjCode = c.String(),
                        hours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        subject = c.String(),
                        smemo = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.prjWorkItems");
        }
    }
}
