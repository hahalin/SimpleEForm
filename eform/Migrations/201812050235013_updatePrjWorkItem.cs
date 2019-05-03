namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePrjWorkItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.prjWorkItems", "pm", c => c.String());
            AddColumn("dbo.prjWorkItems", "gm", c => c.String());
            AddColumn("dbo.prjWorkItems", "mgr1", c => c.String());
            AddColumn("dbo.prjWorkItems", "mgr2", c => c.String());
            AddColumn("dbo.prjWorkItems", "mgr3", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.prjWorkItems", "mgr3");
            DropColumn("dbo.prjWorkItems", "mgr2");
            DropColumn("dbo.prjWorkItems", "mgr1");
            DropColumn("dbo.prjWorkItems", "gm");
            DropColumn("dbo.prjWorkItems", "pm");
        }
    }
}
