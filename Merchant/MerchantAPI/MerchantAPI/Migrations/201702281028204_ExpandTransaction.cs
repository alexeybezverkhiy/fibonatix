namespace MerchantAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpandTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transaction", "StateReason", c => c.String(maxLength: 128));
            AddColumn("dbo.Transaction", "RedirectUri", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transaction", "RedirectUri");
            DropColumn("dbo.Transaction", "StateReason");
        }
    }
}
