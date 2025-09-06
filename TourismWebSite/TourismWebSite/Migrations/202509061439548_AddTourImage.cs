namespace TourismWebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTourImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tours", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tours", "ImageUrl");
        }
    }
}
