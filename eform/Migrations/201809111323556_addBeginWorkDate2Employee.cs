namespace eform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBeginWorkDate2Employee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "beginWorkDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "beginWorkDate");
        }
    }
}
