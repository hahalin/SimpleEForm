namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTBstockItemInitList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.stockItemInits",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        pdId = c.String(),
                        pdNm = c.String(),
                        spec = c.String(),
                        dateA = c.String(),
                        billNm = c.String(),
                        billInNo = c.String(),
                        InvNo = c.String(),
                        brandNo = c.String(),
                        supPdId = c.String(),
                        supId = c.String(),
                        supNm = c.String(),
                        smemoA = c.String(),
                        smemoB = c.String(),
                        qty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        priceA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        amtA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        amtB = c.Decimal(nullable: false, precision: 18, scale: 2),
                        curId = c.String(),
                        curRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        priceB = c.Decimal(nullable: false, precision: 18, scale: 2),
                        amtC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        prjId = c.String(),
                        prjNm = c.String(),
                        reqId = c.String(),
                        reqId2 = c.String(),
                        dateB = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.stockItemInits");
        }
    }
}
