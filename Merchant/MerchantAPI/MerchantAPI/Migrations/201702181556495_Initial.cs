namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transaction",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TransactionId = c.String(maxLength: 36),
                        SerialNumber = c.String(maxLength: 36),
                        ProcessingTransactionId = c.String(maxLength: 48),
                        MerchantTransactionId = c.String(maxLength: 48),
                        Type = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        ReferenceQuery = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.TransactionId, unique: true);
        }

        public override void Down()
        {
            DropTable("dbo.Transaction");
        }
    }
}
