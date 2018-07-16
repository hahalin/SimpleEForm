namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addenm2overworkform : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OverTimeForms", "defEnm", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OverTimeForms", "defEnm");
        }
    }
}
