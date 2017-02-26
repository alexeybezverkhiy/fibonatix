namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MerchantCallback1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MerchantCallback", "StateReason", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MerchantCallback", "StateReason", c => c.String(maxLength: 64));
        }
    }
}
