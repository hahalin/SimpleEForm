namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCreateTime2ToNEws : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.news", "createTime2", c => c.DateTime());
            AlterColumn("dbo.news", "content", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.news", "content", c => c.String(nullable: false));
            DropColumn("dbo.news", "createTime2");
        }
    }
}
