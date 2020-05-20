namespace T1809E_Project_Sem3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Database : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Address", c => c.String());
            AddColumn("dbo.AspNetUsers", "Birthday", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "Gender", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "CreateAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CreateAt");
            DropColumn("dbo.AspNetUsers", "Status");
            DropColumn("dbo.AspNetUsers", "Gender");
            DropColumn("dbo.AspNetUsers", "Birthday");
            DropColumn("dbo.AspNetUsers", "Address");
        }
    }
}
