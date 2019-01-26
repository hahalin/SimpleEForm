namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPrjWork : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.prjWorks",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        workNo = c.String(),
                        y = c.Int(nullable: false),
                        w = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.prjWorks");
        }
    }
}
