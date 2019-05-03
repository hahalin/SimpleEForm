namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addForumItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForumItems",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        prjId = c.String(),
                        workNo = c.String(),
                        billDate = c.DateTime(nullable: false),
                        seq = c.Int(nullable: false),
                        subject = c.String(),
                        smemo = c.String(),
                        othersA1 = c.String(),
                        othersA2 = c.String(),
                        othersA3 = c.String(),
                        othersB1 = c.String(),
                        othersB2 = c.String(),
                        othersB3 = c.String(),
                        sfile = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ForumItems");
        }
    }
}
