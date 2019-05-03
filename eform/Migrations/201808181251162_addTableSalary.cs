namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableSalary : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.salaries",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        year = c.Int(nullable: false),
                        month = c.Int(nullable: false),
                        workNo = c.String(),
                        jData = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.salaries");
        }
    }
}
