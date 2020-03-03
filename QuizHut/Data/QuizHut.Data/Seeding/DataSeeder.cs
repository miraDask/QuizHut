namespace QuizHut.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Common;
    using QuizHut.Data.Models;

    public class DataSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            // creating admin user;
            await CreateUser(
                userManager,
                roleManager,
                GlobalConstants.DataSeeding.AdminName,
                GlobalConstants.DataSeeding.AdminEmail,
                GlobalConstants.AdministratorRoleName);

            // creating teacher user;
            await CreateUser(
                userManager,
                roleManager,
                GlobalConstants.DataSeeding.TeacherName,
                GlobalConstants.DataSeeding.TeacherEmail,
                GlobalConstants.TeacherRoleName);

            // creating student user;
            await CreateUser(
                userManager,
                roleManager,
                GlobalConstants.DataSeeding.StudentName,
                GlobalConstants.DataSeeding.StudentEmail);
        }

        private static async Task CreateUser(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, string name, string email, string roleName = null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                FirstName = name,
                LastName = name,
                Email = email,
            };

            var password = GlobalConstants.DataSeeding.Password;

            if (roleName != null)
            {
                var role = await roleManager.FindByNameAsync(roleName);

                if (!userManager.Users.Any(x => x.Roles.Any(x => x.RoleId == role.Id)))
                {
                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, roleName);
                    }
                }
            }
            else
            {
                if (!userManager.Users.Any(x => x.Roles.Count() == 0))
                {
                    var result = await userManager.CreateAsync(user, password);
                }
            }
        }
    }
}
