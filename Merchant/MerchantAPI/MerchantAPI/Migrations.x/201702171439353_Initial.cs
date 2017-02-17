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
                        TransactionId = c.Guid(nullable: false),
                        ProcessingTransactionId = c.Guid(),
                        Type = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        ReferenceQuery = c.String(),
                    })
                .PrimaryKey(t => t.TransactionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Transaction");
        }
    }
}
