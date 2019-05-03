namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDayOffEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.dayOffs",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        flowId = c.String(),
                        dtBegin = c.DateTime(),
                        dtEnd = c.DateTime(),
                        dType = c.String(),
                        hours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        jobAgent = c.String(),
                        sMemo = c.String(),
                        jext = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.dayOffTypes",
                c => new
                    {
                        k = c.Int(nullable: false, identity: true),
                        v = c.String(),
                    })
                .PrimaryKey(t => t.k);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.dayOffTypes");
            DropTable("dbo.dayOffs");
        }
    }
}
