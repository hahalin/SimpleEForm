namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFlowDef : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FlowDefMain",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        nm = c.String(nullable: false),
                        seq = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.FlowDefSub",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        pid = c.String(),
                        seq = c.Int(nullable: false),
                        workNo = c.String(nullable: false),
                        signType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FlowDefSub");
            DropTable("dbo.FlowDefMain");
        }
    }
}
