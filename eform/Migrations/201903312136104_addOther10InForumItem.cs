namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOther10InForumItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForumItems", "othersA7", c => c.String());
            AddColumn("dbo.ForumItems", "othersA8", c => c.String());
            AddColumn("dbo.ForumItems", "othersA9", c => c.String());
            AddColumn("dbo.ForumItems", "othersA10", c => c.String());
            AddColumn("dbo.ForumItems", "othersB7", c => c.String());
            AddColumn("dbo.ForumItems", "othersB8", c => c.String());
            AddColumn("dbo.ForumItems", "othersB9", c => c.String());
            AddColumn("dbo.ForumItems", "othersB10", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForumItems", "othersB10");
            DropColumn("dbo.ForumItems", "othersB9");
            DropColumn("dbo.ForumItems", "othersB8");
            DropColumn("dbo.ForumItems", "othersB7");
            DropColumn("dbo.ForumItems", "othersA10");
            DropColumn("dbo.ForumItems", "othersA9");
            DropColumn("dbo.ForumItems", "othersA8");
            DropColumn("dbo.ForumItems", "othersA7");
        }
    }
}
