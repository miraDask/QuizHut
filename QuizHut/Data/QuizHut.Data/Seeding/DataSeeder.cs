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
            var teacherId = await CreateUser(
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

            if (!dbContext.Quizzes.Any())
            {
                await CreateQuizzes(teacherId, userManager, dbContext);
            }
        }

        private static async Task CreateQuizzes(string teacherId, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            var teacher = await userManager.FindByIdAsync(teacherId);
            for (int i = 1; i <= 5; i++)
            {
                var quiz = new Quiz()
                {
                    Name = $"Test quiz {i}",
                    Password = new Password() { Content = $"password{i}" },
                    Description = $"<p>This is test quiz {i}</p>",
                };

                await dbContext.Quizzes.AddAsync(quiz);

                for (int j = 1; j <= 10; j++)
                {
                    var question = new Question()
                    {
                        Text = $"<p>Question {j}</p>",
                        Number = j,
                    };

                    for (int l = 1; l <= 3; l++)
                    {
                        var answer = new Answer
                        {
                            Text = l % 2 == 0 ? "<p>True answer</p>" : "<p>False answer</p>",
                            IsRightAnswer = l % 2 == 0 ? true : false,
                        };

                        await dbContext.Answers.AddAsync(answer);
                        question.Answers.Add(answer);
                    }

                    await dbContext.Questions.AddAsync(question);
                    quiz.Questions.Add(question);
                }

                teacher.CreatedQuizzes.Add(quiz);
                await userManager.UpdateAsync(teacher);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task<string> CreateUser(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            string name,
            string email,
            string roleName = null)
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

            return user.Id;
        }
    }
}
