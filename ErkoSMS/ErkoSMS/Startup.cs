using ErkoSMS.Enums;
using ErkoSMS.Models;
using Microsoft.AspNet.Identity;
using AspNet.Identity.SQLite;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ErkoSMS.Startup))]
namespace ErkoSMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }

        // In this method we will create default User roles and Admin user for login    
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext("DefaultConnection");
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser, IdentityRole>(context));

            if (!roleManager.RoleExists(UserType.Administrator.ToString()))
            {
                var role = new IdentityRole();
                role.Name = UserType.Administrator.ToString();
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists(UserType.Accountant.ToString()))
            {
                var role = new IdentityRole();
                role.Name = UserType.Accountant.ToString();
                roleManager.Create(role);

            }
            if (!roleManager.RoleExists(UserType.Purchaser.ToString()))
            {
                var role = new IdentityRole();
                role.Name = UserType.Purchaser.ToString();
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists(UserType.SalesMan.ToString()))
            {
                var role = new IdentityRole();
                role.Name = UserType.SalesMan.ToString();
                roleManager.Create(role);

            }

            if (!roleManager.RoleExists(UserType.WareHouseMan.ToString()))
            {
                var role = new IdentityRole();
                role.Name = UserType.WareHouseMan.ToString();
                roleManager.Create(role);

            }

        }

    }
}
