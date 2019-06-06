namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prjCode_createDate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.prjCodes", "createDate2", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.prjCodes", "createDate2");
        }
    }
}
