namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addenm2flowdefmain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FlowDefMain", "enm", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FlowDefMain", "enm");
        }
    }
}
