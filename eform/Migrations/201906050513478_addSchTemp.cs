namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSchTemp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.schTemps",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        code = c.String(),
                        txt = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.schTemps");
        }
    }
}
