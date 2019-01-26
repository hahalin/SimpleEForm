namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePrjCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.prjCodes", "creator", c => c.String());
            AddColumn("dbo.prjCodes", "createDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.prjCodes", "createDate");
            DropColumn("dbo.prjCodes", "creator");
        }
    }
}
