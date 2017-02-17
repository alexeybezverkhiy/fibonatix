namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Transaction1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Transaction", "x1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transaction", "x1", c => c.String());
        }
    }
}
