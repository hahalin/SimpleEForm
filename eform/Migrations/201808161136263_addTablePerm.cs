namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTablePerm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.permissions",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        workNo = c.String(),
                        mod = c.String(),
                        modItem = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.permissions");
        }
    }
}
