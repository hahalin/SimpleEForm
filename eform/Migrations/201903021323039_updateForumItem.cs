namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateForumItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForumItems", "othersA4", c => c.String());
            AddColumn("dbo.ForumItems", "othersA5", c => c.String());
            AddColumn("dbo.ForumItems", "othersA6", c => c.String());
            AddColumn("dbo.ForumItems", "othersB4", c => c.String());
            AddColumn("dbo.ForumItems", "othersB5", c => c.String());
            AddColumn("dbo.ForumItems", "othersB6", c => c.String());
            AddColumn("dbo.ForumItems", "sfile2", c => c.String());
            AddColumn("dbo.ForumItems", "sfile3", c => c.String());
            AddColumn("dbo.ForumItems", "sfile4", c => c.String());
            AddColumn("dbo.ForumItems", "sfile5", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForumItems", "sfile5");
            DropColumn("dbo.ForumItems", "sfile4");
            DropColumn("dbo.ForumItems", "sfile3");
            DropColumn("dbo.ForumItems", "sfile2");
            DropColumn("dbo.ForumItems", "othersB6");
            DropColumn("dbo.ForumItems", "othersB5");
            DropColumn("dbo.ForumItems", "othersB4");
            DropColumn("dbo.ForumItems", "othersA6");
            DropColumn("dbo.ForumItems", "othersA5");
            DropColumn("dbo.ForumItems", "othersA4");
        }
    }
}
