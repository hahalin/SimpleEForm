namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSort2Deps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.deps", "sort", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.deps", "sort");
        }
    }
}
