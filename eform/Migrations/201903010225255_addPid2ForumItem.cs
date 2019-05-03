namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPid2ForumItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForumItems", "pid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForumItems", "pid");
        }
    }
}
