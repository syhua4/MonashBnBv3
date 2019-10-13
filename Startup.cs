using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using MonashBnBv3.Models;
using Owin;
using System.Web;

[assembly: OwinStartupAttribute(typeof(MonashBnBv3.Startup))]
namespace MonashBnBv3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }
        // In this method we will create default User roles and Admin user for login    
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();


            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup iam creating first Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool    
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                   



            }
            var store = new UserStore<ApplicationUser>(context);
            var manager = new ApplicationUserManager(store);
            var user = new ApplicationUser() { Email = "admin@gmail.com", UserName = "admin", LastName ="admin", FirstName = "admin"};
            IdentityResult checkUser = manager.Create(user, "admin_1234");
            if (checkUser.Succeeded)
            {
                manager.AddToRole(user.Id, "Admin");
            }
            if (!roleManager.RoleExists("Customer"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Customer";
                roleManager.Create(role);

            }
        }
    }
}
