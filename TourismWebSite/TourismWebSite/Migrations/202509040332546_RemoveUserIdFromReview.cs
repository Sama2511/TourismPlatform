namespace TourismWebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserIdFromReview : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Reviews", name: "User_Id", newName: "UserId");
            RenameIndex(table: "dbo.Reviews", name: "IX_User_Id", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Reviews", name: "IX_UserId", newName: "IX_User_Id");
            RenameColumn(table: "dbo.Reviews", name: "UserId", newName: "User_Id");
        }
    }
}
