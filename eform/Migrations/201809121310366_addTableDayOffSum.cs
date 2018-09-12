namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableDayOffSum : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.dayOffSums",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        workNo = c.String(),
                        y = c.Int(nullable: false),
                        m = c.Int(nullable: false),
                        sType = c.String(),
                        v1 = c.Decimal(nullable: false, precision: 8, scale: 1),
                        v2 = c.Decimal(nullable: false, precision: 8, scale: 1),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.dayOffSums");
        }
    }
}
