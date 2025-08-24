namespace TourismWebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTours : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tours",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),                      
                    Description = c.String(),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DurationDays = c.Int(nullable: false),
                    Destination = c.String(),
                    StartDate = c.DateTime(nullable: false),
                    EndDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.Tours");
        }
    }
}
