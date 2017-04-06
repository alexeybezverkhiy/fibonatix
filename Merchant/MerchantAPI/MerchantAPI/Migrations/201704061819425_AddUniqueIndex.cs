namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Transaction", new[] { "MerchantTransactionId", "Type" }, unique: true, name: "TransactionMerchantTransactionId_UIDX");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Transaction", "TransactionMerchantTransactionId_UIDX");
        }
    }
}
