namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Transaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transaction", "x1", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transaction", "x1");
        }
    }
}
