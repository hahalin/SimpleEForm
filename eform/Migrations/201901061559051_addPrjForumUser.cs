namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPrjForumUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.prjForumUsers",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        pid = c.String(),
                        WorkNo = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.prjForumUsers");
        }
    }
}
