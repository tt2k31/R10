using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace R10_login_Identity.Migrations
{
    public partial class genUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 0; i < 150; i++)
            {
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[] {
                        "Id",
                        "UserName",                        
                        "Email",                        
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount",
                        "HomeAddress",
                        
                    },
                    values: new object[] {
                        Guid.NewGuid().ToString(),
                        "user-"+i.ToString("D3"),
                        $"email{i.ToString("D3")}@example.com",
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        false,
                        0,
                        "...!@#..."

                        
                    }
                    );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
