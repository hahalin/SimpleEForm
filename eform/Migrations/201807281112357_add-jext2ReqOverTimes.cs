namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addjext2ReqOverTimes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReqOverTimes", "jext", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReqOverTimes", "jext");
        }
    }
}
