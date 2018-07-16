namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEnglishName2User : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.news",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        ndate = c.DateTime(),
                        uid = c.String(),
                        title = c.String(nullable: false),
                        content = c.String(nullable: false),
                        isActive = c.Boolean(nullable: false),
                        createTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.AspNetUsers", "eName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "eName");
            DropTable("dbo.news");
        }
    }
}
