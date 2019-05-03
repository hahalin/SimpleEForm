namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addWording2FlowDefMain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FlowDefMain", "wording", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FlowDefMain", "wording");
        }
    }
}
