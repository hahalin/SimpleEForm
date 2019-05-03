namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addisFormSigner2jobpo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.jobPoes", "isFormSigner", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.jobPoes", "isFormSigner");
        }
    }
}
