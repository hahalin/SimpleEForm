namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPrjCode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.prjCodes",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        code = c.String(),
                        nm = c.String(),
                        owner = c.String(),
                        status = c.String(),
                        mmo1 = c.String(),
                        mmo2 = c.String(),
                        mmo3 = c.String(),
                        mmo4 = c.String(),
                        mmo5 = c.String(),
                        mmo6 = c.String(),
                        mmo7 = c.String(),
                        mmo8 = c.String(),
                        mmo9 = c.String(),
                        mmo10 = c.String(),
                        mmo11 = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            AlterColumn("dbo.EventSchedules", "beginDate", c => c.DateTime());
            AlterColumn("dbo.EventSchedules", "endDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EventSchedules", "endDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.EventSchedules", "beginDate", c => c.DateTime(nullable: false));
            DropTable("dbo.prjCodes");
        }
    }
}
