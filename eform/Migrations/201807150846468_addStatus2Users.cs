namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStatus2Users : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "status");
        }
    }
}
