namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPrjPM_Title : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.prjPMs", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.prjPMs", "Title");
        }
    }
}
