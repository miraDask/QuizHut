namespace QuizHut.Services.Data.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Models;
    using QuizHut.Services.Users;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class UsersServiceTests : BaseServiceTests
    {
        private IUsersService Service => this.ServiceProvider.GetRequiredService<IUsersService>();

        [Fact]
        public async Task AddStudentAsyncShouldReturnTrueIfStudentIsSuccessfullyAdded()
        {
            var teacherId = await this.CreateUserAsync("teacher@teacher.com");
            var studentId = await this.CreateUserAsync("student@student.com");

            var result = await this.Service.AddStudentAsync("student@student.com", teacherId);

            var teacher = await this.DbContext.Users.FirstOrDefaultAsync(x => x.Id == teacherId);
            var student = await this.DbContext.Users.FirstOrDefaultAsync(x => x.Id == studentId);

            Assert.True(result);
            Assert.Equal(1, teacher.Students.Count);
            Assert.True(teacher.Students.Contains(student));
            Assert.Equal(teacherId, student.TeacherId);
        }

        private async Task<string> CreateUserAsync(string email)
        {
            var user = new ApplicationUser()
            {
                FirstName = "First Name",
                LastName = "Last Name",
                Email = email,
                UserName = email,
            };

            await this.DbContext.Users.AddAsync(user);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<ApplicationUser>(user).State = EntityState.Detached;
            return user.Id;
        }
    }
}
