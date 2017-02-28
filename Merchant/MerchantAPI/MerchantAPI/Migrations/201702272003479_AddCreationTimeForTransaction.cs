namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreationTimeForTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transaction", "CreationTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transaction", "CreationTime");
        }
    }
}
