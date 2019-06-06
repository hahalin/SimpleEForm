namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSchTempItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.schTempItems",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        pid = c.String(),
                        txt = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.schTempItems");
        }
    }
}
