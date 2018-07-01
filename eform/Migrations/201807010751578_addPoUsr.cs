namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPoUsr : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.jobPoes", new[] { "ApplicationUser_Id" });
            CreateTable(
                "dbo.PoUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        poNo = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ApplicationUser_Id);
            
            DropColumn("dbo.jobPoes", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.jobPoes", "ApplicationUser_Id", c => c.String(maxLength: 128));
            DropIndex("dbo.PoUsers", new[] { "ApplicationUser_Id" });
            DropTable("dbo.PoUsers");
            CreateIndex("dbo.jobPoes", "ApplicationUser_Id");
        }
    }
}
