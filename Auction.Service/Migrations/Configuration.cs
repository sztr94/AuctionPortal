using Auction.Service.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Auction.Service.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AdministratorDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AdministratorDbContext context)
        {
            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context));

                IdentityRole role = new IdentityRole { Name = "administrator" };
                roleManager.Create(role);

                IdentityUser adminUser = new IdentityUser { UserName = "admin" };
                userManager.Create(adminUser, "alma1234");
                userManager.AddToRole(adminUser.Id, "administrator");
            }
        }
    }
}
