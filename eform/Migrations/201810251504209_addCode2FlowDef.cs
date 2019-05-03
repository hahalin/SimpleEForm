namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCode2FlowDef : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FlowDefMain", "code", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FlowDefMain", "code");
        }
    }
}
