namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSignDate2FlowMain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FlowMains", "signDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FlowMains", "signDate");
        }
    }
}
