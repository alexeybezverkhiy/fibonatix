namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class val : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Transaction", "TransactionTransactionId_UIDX");
            CreateIndex("dbo.Transaction", new[] { "TransactionId", "Type" }, unique: true, name: "TransactionTransactionId_UIDX");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Transaction", "TransactionTransactionId_UIDX");
            CreateIndex("dbo.Transaction", "TransactionId", unique: true, name: "TransactionTransactionId_UIDX");
        }
    }
}
