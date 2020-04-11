namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Models;
    using QuizHut.Services.Users;
    using QuizHut.Web.ViewModels.Students;
    using QuizHut.Web.ViewModels.UsersInRole;
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

        [Fact]
        public async Task AddStudentAsyncShouldReturnFalseIfStudentIsNotFound()
        {
            var teacherId = await this.CreateUserAsync("teacher@teacher.com");

            var result = await this.Service.AddStudentAsync("student@student.com", teacherId);

            var teacher = await this.DbContext.Users.FirstOrDefaultAsync(x => x.Id == teacherId);

            Assert.False(result);
            Assert.Equal(0, teacher.Students.Count);
        }

        [Fact]
        public async Task DeleteFromTeacherListAsyncShouldRemoveStudentSuccessfully()
        {
            var teacherId = await this.CreateUserAsync("teacher@teacher.com");
            var studentId = await this.CreateUserAsync("student@student.com");

            await this.AddStudentAsync(studentId, teacherId);
            await this.Service.DeleteFromTeacherListAsync(studentId, teacherId);

            var teacher = await this.GetUserAsync(teacherId);
            var student = await this.GetUserAsync(studentId);

            Assert.Equal(0, teacher.Students.Count);
            Assert.False(teacher.Students.Contains(student));
            Assert.Null(student.TeacherId);
        }

        [Fact]
        public async Task GetAllByRoleAsyncShouldReturnCorrectModelCollection()
        {
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "teacher");
            await this.CreateUserAsync("student@student.com");

            var model = new UserInRoleViewModel()
            {
                Id = teacherId,
                FullName = "John Doe",
                Email = "teacher@teacher.com",
            };

            var resultModelCollection = await this.Service.GetAllByRoleAsync<UserInRoleViewModel>("teacher");

            Assert.Equal(1, resultModelCollection.Count);
            Assert.Equal(model.Id, resultModelCollection.First().Id);
            Assert.Equal(model.FullName, resultModelCollection.First().FullName);
            Assert.Equal(model.Email, resultModelCollection.First().Email);
        }

        [Fact]
        public async Task GetAllByGroupIdAsyncShouldReturnCorrectModelCollection()
        {
            await this.CreateUserAsync("teacher@teacher.com");
            var studentId = await this.CreateUserAsync("student@student.com");
            var groupId = await this.AssignStudentToGroupAsync(studentId);

            var model = new StudentViewModel()
            {
                Id = studentId,
                FullName = "John Doe",
                Email = "student@student.com",
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllByGroupIdAsync<StudentViewModel>(groupId);

            Assert.Equal(1, resultModelCollection.Count);
            Assert.Equal(model.Id, resultModelCollection.First().Id);
            Assert.Equal(model.FullName, resultModelCollection.First().FullName);
            Assert.Equal(model.Email, resultModelCollection.First().Email);
        }

        private async Task<string> AssignStudentToGroupAsync(string studentId)
        {
            var group = new Group() { Name = "group" };
            var studentGroup = new StudentGroup() { StudentId = studentId, GroupId = group.Id };

            await this.DbContext.Groups.AddAsync(group);
            await this.DbContext.StudentsGroups.AddAsync(studentGroup);
            await this.DbContext.SaveChangesAsync();

            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            this.DbContext.Entry<StudentGroup>(studentGroup).State = EntityState.Detached;
            return group.Id;
        }

        private async Task AddStudentAsync(string studentId, string teacherId)
        {
            var teacher = await this.GetUserAsync(teacherId);
            var student = await this.GetUserAsync(studentId);
            teacher.Students.Add(student);
            student.TeacherId = teacherId;
            this.DbContext.Users.Update(student);
            this.DbContext.Users.Update(teacher);

            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<ApplicationUser>(student).State = EntityState.Detached;
            this.DbContext.Entry<ApplicationUser>(teacher).State = EntityState.Detached;
        }

        private async Task<string> CreateUserAsync(string email, string roleName = null)
        {
            var user = new ApplicationUser()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                UserName = email,
            };

            if (roleName != null)
            {
                var role = new ApplicationRole()
                {
                    Name = roleName,
                };

                await this.DbContext.Roles.AddAsync(role);
                var userRole = new IdentityUserRole<string>
                {
                    RoleId = role.Id,
                    UserId = user.Id,
                };

                await this.DbContext.UserRoles.AddAsync(userRole);
                await this.DbContext.SaveChangesAsync();
                this.DbContext.Entry<ApplicationRole>(role).State = EntityState.Detached;
                this.DbContext.Entry<IdentityUserRole<string>>(userRole).State = EntityState.Detached;
            }

            await this.DbContext.Users.AddAsync(user);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<ApplicationUser>(user).State = EntityState.Detached;
            return user.Id;
        }

        private async Task<ApplicationUser> GetUserAsync(string id)
        {
            var user = await this.DbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            this.DbContext.Entry<ApplicationUser>(user).State = EntityState.Detached;
            return user;
        }
    }
}
