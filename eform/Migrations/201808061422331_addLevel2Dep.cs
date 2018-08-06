namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLevel2Dep : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.deps", "depLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.deps", "depLevel");
        }
    }
}
