namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.MerchantCallback", "TransactionId", unique: true, name: "CallbackTransactionId_UIDX");
            CreateIndex("dbo.MerchantCallback", "State", name: "CallbackState_IDX");
            CreateIndex("dbo.MerchantCallback", "NextAttemptTime", name: "CallbackNextAttemptTime_IDX");
            CreateIndex("dbo.Transaction", "TransactionId", unique: true, name: "TransactionTransactionId_UIDX");
            CreateIndex("dbo.Transaction", "Type", name: "TransactionType_IDX");
            CreateIndex("dbo.Transaction", "State", name: "TransactionState_IDX");
            CreateIndex("dbo.Transaction", "Status", name: "TransactionStatus_IDX");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Transaction", "TransactionStatus_IDX");
            DropIndex("dbo.Transaction", "TransactionState_IDX");
            DropIndex("dbo.Transaction", "TransactionType_IDX");
            DropIndex("dbo.Transaction", "TransactionTransactionId_UIDX");
            DropIndex("dbo.MerchantCallback", "CallbackNextAttemptTime_IDX");
            DropIndex("dbo.MerchantCallback", "CallbackState_IDX");
            DropIndex("dbo.MerchantCallback", "CallbackTransactionId_UIDX");
        }
    }
}
