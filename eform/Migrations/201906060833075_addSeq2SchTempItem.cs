namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSeq2SchTempItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.schTempItems", "seq", c => c.Int(nullable: false));
            AlterColumn("dbo.schTemps", "code", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.schTemps", "code", c => c.String());
            DropColumn("dbo.schTempItems", "seq");
        }
    }
}
