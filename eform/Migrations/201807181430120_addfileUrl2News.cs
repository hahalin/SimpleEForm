namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfileUrl2News : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.news", "fileUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.news", "fileUrl");
        }
    }
}
