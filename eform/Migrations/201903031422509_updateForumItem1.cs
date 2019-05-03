namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateForumItem1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForumItems", "isPrivate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForumItems", "isPrivate");
        }
    }
}
