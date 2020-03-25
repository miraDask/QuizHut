namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data;
    using QuizHut.Data.Models;
    using QuizHut.Data.Repositories;
    using QuizHut.Services.StudentsGroups;
    using Xunit;

    public class StudentsGroupsServiceTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly EfRepository<StudentGroup> repository;
        private readonly StudentsGroupsService service;

        public StudentsGroupsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;
            this.dbContext = new ApplicationDbContext(options);
            this.repository = new EfRepository<StudentGroup>(this.dbContext);
            this.service = new StudentsGroupsService(this.repository);
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var studentId = await this.CreateStudentAsync();
            var groupId = await this.CreateGroupAsync();
            await this.CreateStudentGroupAsync(studentId, groupId);

            await this.service.DeleteAsync(groupId, studentId);

            var studentsGroupsCount = this.dbContext.StudentsGroups.ToArray().Count();
            var studentGroup = await this.dbContext.StudentsGroups.FindAsync(new string[] { studentId, groupId });
            Assert.Equal(0, studentsGroupsCount);
            Assert.Null(studentGroup);
        }

        [Fact]
        public async Task GetAllStudentsIdsByGroupIdAsyncShouldReturnCorrectCollection()
        {
            var groupId = await this.CreateGroupAsync();
            var firstStudentId = await this.CreateStudentAsync();
            var secondStudentId = await this.CreateStudentAsync();

            await this.CreateStudentGroupAsync(firstStudentId, groupId);
            await this.CreateStudentGroupAsync(secondStudentId, groupId);

            var studentIds = await this.service.GetAllStudentsIdsByGroupIdAsync(groupId);

            Assert.Contains(firstStudentId, studentIds);
            Assert.Contains(secondStudentId, studentIds);
        }

        [Fact]
        public async Task CreateStudentGroupAsyncShouldCreateNewStudentGroupIfDoesntExist()
        {
            var groupId = await this.CreateGroupAsync();
            var studentId = await this.CreateStudentAsync();
            await this.service.CreateStudentGroupAsync(groupId, studentId);

            var studentGroup = await this.dbContext.StudentsGroups.FindAsync(new string[] { studentId, groupId });

            Assert.NotNull(studentGroup);
            Assert.Equal(studentId, studentGroup.StudentId);
            Assert.Equal(groupId, studentGroup.GroupId);
        }

        [Fact]
        public async Task CreateStudentGroupAsyncShouldNotCreateNewStudentGroupAlreadyExist()
        {
            var groupId = await this.CreateGroupAsync();
            var studentId = await this.CreateStudentAsync();
            await this.service.CreateStudentGroupAsync(groupId, studentId);
            await this.service.CreateStudentGroupAsync(groupId, studentId);

            var studentsGroupsCount = this.dbContext.StudentsGroups.ToArray().Count();

            Assert.Equal(1, studentsGroupsCount);
        }

        private async Task CreateStudentGroupAsync(string studentId, string groupId)
        {
            var studentGroup = new StudentGroup()
            {
                StudentId = studentId,
                GroupId = groupId,
            };

            await this.dbContext.StudentsGroups.AddAsync(studentGroup);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<StudentGroup>(studentGroup).State = EntityState.Detached;
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

            await this.dbContext.Users.AddAsync(student);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<ApplicationUser>(student).State = EntityState.Detached;
            return student.Id;
        }

        private async Task<string> CreateGroupAsync()
        {
            var creatorId = Guid.NewGuid().ToString();
            var group = new Group() { Name = "Group", CreatorId = creatorId };
            await this.dbContext.Groups.AddAsync(group);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Group>(group).State = EntityState.Detached;
            return group.Id;
        }
    }
}
