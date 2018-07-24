namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewsType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.newsTypes",
                c => new
                    {
                        code = c.String(nullable: false, maxLength: 128),
                        seq = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.code);
            
            AddColumn("dbo.news", "newsType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.news", "newsType");
            DropTable("dbo.newsTypes");
        }
    }
}
