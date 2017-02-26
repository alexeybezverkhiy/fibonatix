namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MerchantCallback : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MerchantCallback",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TransactionId = c.String(maxLength: 36),
                        State = c.Int(nullable: false),
                        StateReason = c.String(maxLength: 128),
                        LastModified = c.DateTime(nullable: false),
                        AttemptNo = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        NextAttemptTime = c.DateTime(),
                        CallbackUri = c.String(maxLength: 128),
                        CallbackQuery = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MerchantCallback");
        }
    }
}
