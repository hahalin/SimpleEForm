namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class whatneedadd : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.deps", "depNm", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.deps", "depNm", c => c.String());
        }
    }
}
