namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addReqInHouse : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReqInHouses",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        flowId = c.String(),
                        reqMemo = c.String(),
                        reqLimit = c.String(),
                        contact = c.String(),
                        depNo = c.String(),
                        sMemo = c.String(),
                        jext = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ReqInHouses");
        }
    }
}
