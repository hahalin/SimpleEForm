namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addContractDate2PrjCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.prjCodes", "contractDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.prjCodes", "contractDate");
        }
    }
}
