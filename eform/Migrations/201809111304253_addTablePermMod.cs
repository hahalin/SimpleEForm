namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTablePermMod : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.permMods",
                c => new
                    {
                        mod = c.String(nullable: false, maxLength: 128),
                        modCname = c.String(),
                    })
                .PrimaryKey(t => t.mod);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.permMods");
        }
    }
}
