namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserdepNoField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "depNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "depNo");
        }
    }
}
