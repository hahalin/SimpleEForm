namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTitle2ForumItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForumItems", "subTitle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForumItems", "subTitle");
        }
    }
}
