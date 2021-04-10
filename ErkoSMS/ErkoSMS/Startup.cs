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

            if (!roleManager.RoleExists(UserTypes.Administrator.Name))
            {
                var role = new IdentityRole();
                role.Name = UserTypes.Administrator.Name;
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists(UserTypes.Accountant.Name))
            {
                var role = new IdentityRole();
                role.Name = UserTypes.Accountant.Name;
                roleManager.Create(role);

            }
            if (!roleManager.RoleExists(UserTypes.Purchaser.Name))
            {
                var role = new IdentityRole();
                role.Name = UserTypes.Purchaser.Name;
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists(UserTypes.SalesMan.Name))
            {
                var role = new IdentityRole();
                role.Name = UserTypes.SalesMan.Name;
                roleManager.Create(role);

            }

            if (!roleManager.RoleExists(UserTypes.WareHouseMan.Name))
            {
                var role = new IdentityRole();
                role.Name = UserTypes.WareHouseMan.Name;
                roleManager.Create(role);

            }

        }

    }
}
