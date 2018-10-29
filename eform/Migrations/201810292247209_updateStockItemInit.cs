namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateStockItemInit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.stockItemInits", "sType", c => c.String());
            AddColumn("dbo.stockItemInits", "filename", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.stockItemInits", "filename");
            DropColumn("dbo.stockItemInits", "sType");
        }
    }
}
