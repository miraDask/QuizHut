namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Models;
    using QuizHut.Services.StudentsGroups;
    using Xunit;

    public class StudentsGroupsServiceTests : BaseServiceTests
    {
        private IStudentsGroupsService Service => this.ServiceProvider.GetRequiredService<IStudentsGroupsService>();

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var studentId = await this.CreateStudentAsync();
            var groupId = await this.CreateGroupAsync();
            await this.CreateStudentGroupAsync(studentId, groupId);

            await this.Service.DeleteAsync(groupId, studentId);

            var studentsGroupsCount = this.DbContext.StudentsGroups.ToArray().Count();
            var studentGroup = await this.DbContext.StudentsGroups.FindAsync(new string[] { studentId, groupId });
            Assert.Equal(0, studentsGroupsCount);
            Assert.Null(studentGroup);
        }

        [Fact]
        public async Task CreateStudentGroupAsyncShouldCreateNewStudentGroupIfDoesntExist()
        {
            var groupId = await this.CreateGroupAsync();
            var studentId = await this.CreateStudentAsync();
            await this.Service.CreateStudentGroupAsync(groupId, studentId);

            var studentGroup = await this.DbContext.StudentsGroups.FindAsync(new string[] { studentId, groupId });

            Assert.NotNull(studentGroup);
            Assert.Equal(studentId, studentGroup.StudentId);
            Assert.Equal(groupId, studentGroup.GroupId);
        }

        [Fact]
        public async Task CreateStudentGroupAsyncShouldNotCreateNewStudentGroupIfAlreadyExist()
        {
            var groupId = await this.CreateGroupAsync();
            var studentId = await this.CreateStudentAsync();
            await this.Service.CreateStudentGroupAsync(groupId, studentId);
            await this.Service.CreateStudentGroupAsync(groupId, studentId);

            var studentsGroupsCount = this.DbContext.StudentsGroups.ToArray().Count();

            Assert.Equal(1, studentsGroupsCount);
        }

        private async Task CreateStudentGroupAsync(string studentId, string groupId)
        {
            var studentGroup = new StudentGroup()
            {
                StudentId = studentId,
                GroupId = groupId,
            };

            await this.DbContext.StudentsGroups.AddAsync(studentGroup);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<StudentGroup>(studentGroup).State = EntityState.Detached;
        }

        private async Task<string> CreateStudentAsync()
        {
            var student = new ApplicationUser()
            {
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "email@email.com",
                UserName = "email@email.com",
            };

            await this.DbContext.Users.AddAsync(student);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<ApplicationUser>(student).State = EntityState.Detached;
            return student.Id;
        }

        private async Task<string> CreateGroupAsync()
        {
            var creatorId = Guid.NewGuid().ToString();
            var group = new Group() { Name = "Group", CreatorId = creatorId };
            await this.DbContext.Groups.AddAsync(group);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            return group.Id;
        }
    }
}
