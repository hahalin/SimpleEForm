namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScheduleItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.scheduleItems",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        prjId = c.String(),
                        seq = c.Int(nullable: false),
                        itemTxt = c.String(),
                        dtBegin = c.DateTime(),
                        dtEnd = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            AlterColumn("dbo.schTempItems", "txt", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.schTempItems", "txt", c => c.String());
            DropTable("dbo.scheduleItems");
        }
    }
}
