namespace QuizHut.Services.Data.Tests
{
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
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teachers");
            var studentId = await this.CreateUserAsync("student@student.com", "Student", "Student");

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
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teachers");

            var result = await this.Service.AddStudentAsync("student@student.com", teacherId);

            var teacher = await this.DbContext.Users.FirstOrDefaultAsync(x => x.Id == teacherId);

            Assert.False(result);
            Assert.Equal(0, teacher.Students.Count);
        }

        [Fact]
        public async Task DeleteFromTeacherListAsyncShouldRemoveStudentSuccessfully()
        {
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teachers");
            var studentId = await this.CreateUserAsync("student@student.com", "Student", "Student");

            await this.AddStudentAsync(studentId, teacherId);
            await this.Service.DeleteFromTeacherListAsync(studentId, teacherId);

            var teacher = await this.GetUserAsync(teacherId);
            var student = await this.GetUserAsync(studentId);

            Assert.Equal(0, teacher.Students.Count);
            Assert.False(teacher.Students.Contains(student));
            Assert.Null(student.TeacherId);
        }

        [Fact]
        public async Task GetAllInRolesPerPageAsyncShouldReturnCorrectModelCollection()
        {
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teacher", "Teacher");
            await this.CreateUserAsync("student@student.com", "Student", "Student");

            var model = new UserInRoleViewModel()
            {
                Id = teacherId,
                FullName = "Teacher Teacher",
                Email = "teacher@teacher.com",
            };

            var resultModelCollection = await this.Service.GetAllInRolesPerPageAsync<UserInRoleViewModel>(1, 1);

            Assert.Equal(1, resultModelCollection.Count);
            Assert.Equal(model.Id, resultModelCollection.First().Id);
            Assert.Equal(model.FullName, resultModelCollection.First().FullName);
            Assert.Equal(model.Email, resultModelCollection.First().Email);
        }

        [Theory]
        [InlineData("Administrator", null)]
        [InlineData("Administrator", "N")]
        [InlineData("Administrator", "JO")]
        [InlineData("Administrator", "d")]
        [InlineData("FullName", "john")]
        [InlineData("FirstName", "JO")]
        [InlineData("LastName", "d")]
        [InlineData("Email", "N")]
        public async Task GetAllInRolesPerPageAsyncShouldFilterCorectlyWhenSearchCriteriaIsAdminAndSearchTextIsPassed(string searchCriteria, string searchText)
        {
            await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teacers", "Teacher");
            await this.CreateUserAsync("student@student.com", "Student", "Student");
            var adminId = await this.CreateUserAsync("admin@admin.com", "John", "Doe", "Administrator");

            var adminModel = new UserInRoleViewModel()
            {
                Id = adminId,
                FullName = "John Doe",
                Email = "admin@admin.com",
            };

            var resultModelCollection = await this.Service.GetAllInRolesPerPageAsync<UserInRoleViewModel>(1, 1, searchCriteria, searchText);

            Assert.Equal(1, resultModelCollection.Count);
            Assert.Equal(adminModel.Id, resultModelCollection.First().Id);
            Assert.Equal(adminModel.FullName, resultModelCollection.First().FullName);
            Assert.Equal(adminModel.Email, resultModelCollection.First().Email);
        }

        [Theory]
        [InlineData("Teacher", null)]
        [InlineData("Teacher", "N")]
        [InlineData("Teacher", "JO")]
        [InlineData("Teacher", "d")]
        [InlineData("FullName", "N")]
        [InlineData("FirstName", "JO")]
        [InlineData("LastName", "d")]
        [InlineData("Email", "T")]
        public async Task GetAllInRolesPerPageAsyncShouldFilterCorectlyWhenSearchCriteriaIsTeacherAndSearchTextIsPassed(string searchCriteria, string searchText)
        {
            await this.CreateUserAsync("admin@admin.com", "Admin", "Admin", "Administrator");
            await this.CreateUserAsync("student@student.com", "Student", "Student");
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "John", "Doe", "Teacher");

            var teacherModel = new UserInRoleViewModel()
            {
                Id = teacherId,
                FullName = "John Doe",
                Email = "teacher@teacher.com",
            };

            var resultModelCollection = await this.Service.GetAllInRolesPerPageAsync<UserInRoleViewModel>(1, 1, searchCriteria, searchText);

            Assert.Equal(1, resultModelCollection.Count);
            Assert.Equal(teacherModel.Id, resultModelCollection.First().Id);
            Assert.Equal(teacherModel.FullName, resultModelCollection.First().FullName);
            Assert.Equal(teacherModel.Email, resultModelCollection.First().Email);
        }

        [Fact]
        public async Task GetAllByGroupIdAsyncShouldReturnCorrectModelCollection()
        {
            await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teachers");
            var studentId = await this.CreateUserAsync("student@student.com", "Student", "Student");
            var groupId = await this.AssignStudentToGroupAsync(studentId);

            var model = new StudentViewModel()
            {
                Id = studentId,
                FullName = "Student Student",
                Email = "student@student.com",
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllByGroupIdAsync<StudentViewModel>(groupId);

            Assert.Equal(1, resultModelCollection.Count);
            Assert.Equal(model.Id, resultModelCollection.First().Id);
            Assert.Equal(model.FullName, resultModelCollection.First().FullName);
            Assert.Equal(model.Email, resultModelCollection.First().Email);
        }

        [Fact]
        public async Task GetAllStudentsCountShouldReturnCorrectCountOfAllStudent()
        {
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teachers", "Teacher");
            var firstStudentId = await this.CreateUserAsync("student1@student.com", "Student", "Student");
            await this.AddStudentAsync(firstStudentId, teacherId);

            await this.CreateUserAsync("student2@student.com", "Student2", "Student2");

            var studentsAllCount = await this.Service.GetAllStudentsCountAsync();
            var studentsWithSameTeacherCount = await this.Service.GetAllStudentsCountAsync(teacherId);

            Assert.Equal(2, studentsAllCount);
            Assert.Equal(1, studentsWithSameTeacherCount);
        }

        [Fact]
        public async Task GetAllInRolesCountShouldReturnCorrectCount()
        {
            await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teacers", "Teacher");
            await this.CreateUserAsync("admin@admin.com", "Admin", "Admin", "Administrator");
            await this.CreateUserAsync("student@student.com", "Student", "Student");

            var usersInRole = await this.Service.GetAllInRolesCountAsync();

            Assert.Equal(2, usersInRole);
        }

        [Fact]
        public async Task GetAllStudentsAsyncShouldReturnCorrectModelCollection()
        {
            await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teacers", "Teacher");
            var studentId = await this.CreateUserAsync("student@student.com", "John", "Doe");

            var model = new StudentViewModel()
            {
                Id = studentId,
                FullName = "John Doe",
                Email = "student@student.com",
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllStudentsAsync<StudentViewModel>();

            Assert.Equal(1, resultModelCollection.Count);
            Assert.Equal(model.Id, resultModelCollection.First().Id);
            Assert.Equal(model.FullName, resultModelCollection.First().FullName);
            Assert.Equal(model.Email, resultModelCollection.First().Email);
        }

        [Fact]
        public async Task GetAllStudentsAsyncShouldReturnAllStudentsWithSameTeacherWhenTeacherIdIsPassed()
        {
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teachers", "Teacher");
            var firstStudentId = await this.CreateUserAsync("student1@student.com", "Student", "Student");
            await this.AddStudentAsync(firstStudentId, teacherId);
            var secondStudentId = await this.CreateUserAsync("student2@student.com", "Student2", "Student2");
            await this.AddStudentAsync(secondStudentId, teacherId);
            await this.CreateUserAsync("student3@student.com", "Student3", "Student3");

            var resultModelCollection = await this.Service.GetAllStudentsAsync<StudentViewModel>(teacherId);

            Assert.Equal(2, resultModelCollection.Count);
        }

        [Fact]
        public async Task GetAllStudentsAsyncShouldReturnAllStudentsThatAreNotInTheGroupWhichGroupIdIsPassed()
        {
            var group = await this.CreateGroupAsync();

            var firstStudentId = await this.CreateUserAsync("student1@student.com", "Student", "Student");
            await this.AssignStudentToGroupAsync(firstStudentId, group.Id);
            var secondStudentId = await this.CreateUserAsync("student2@student.com", "Student2", "Student2");
            await this.AssignStudentToGroupAsync(secondStudentId, group.Id);

            await this.CreateUserAsync("student3@student.com", "Student3", "Student3");

            var resultModelCollection = await this.Service.GetAllStudentsAsync<StudentViewModel>(null, group.Id);

            Assert.Equal(1, resultModelCollection.Count);
        }

        [Fact]
        public async Task GetAllStudentsAsyncShouldReturnAllStudentsWithSameTeacherWhichIdIsPassedAndThatAreNotInTheGroupWhichIdIsPassed()
        {
            var group = await this.CreateGroupAsync();
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teachers", "Teacher");

            var firstStudentId = await this.CreateUserAsync("student1@student.com", "Student", "Student");
            await this.AddStudentAsync(firstStudentId, teacherId);

            var secondStudentId = await this.CreateUserAsync("student2@student.com", "Student2", "Student2");
            await this.AddStudentAsync(secondStudentId, teacherId);
            await this.AssignStudentToGroupAsync(secondStudentId, group.Id);

            var thirdStudentId = await this.CreateUserAsync("student3@student.com", "Student3", "Student3");
            await this.AssignStudentToGroupAsync(thirdStudentId, group.Id);

            var model = new StudentViewModel()
            {
                Id = firstStudentId,
                FullName = "Student Student",
                Email = "student1@student.com",
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllStudentsAsync<StudentViewModel>(teacherId, group.Id);

            Assert.Single(resultModelCollection);
            Assert.Equal(model.Id, resultModelCollection.First().Id);
            Assert.Equal(model.FullName, resultModelCollection.First().FullName);
            Assert.Equal(model.Email, resultModelCollection.First().Email);
        }

        [Fact]
        public async Task GetAllStudentsPerPageAsyncShouldReturnCorrectModelCollectionIfTeacherIdIsNull()
        {
            var firstStudentId = await this.CreateUserAsync("student1@student.com", "Student", "Student");
            var secondStudentId = await this.CreateUserAsync("student2@student.com", "Student2", "Student2");

            var firstModel = new StudentViewModel()
            {
                Id = firstStudentId,
                FullName = "Student Student",
                Email = "student1@student.com",
                IsAssigned = false,
            };

            var secondModel = new StudentViewModel()
            {
                Id = secondStudentId,
                FullName = "Student2 Student2",
                Email = "student2@student.com",
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllStudentsPerPageAsync<StudentViewModel>(1, 2);

            Assert.Equal(2, resultModelCollection.Count());
            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.FullName, resultModelCollection.First().FullName);
            Assert.Equal(secondModel.Email, resultModelCollection.First().Email);
            Assert.False(resultModelCollection.First().IsAssigned);

            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.FullName, resultModelCollection.Last().FullName);
            Assert.Equal(firstModel.Email, resultModelCollection.Last().Email);
            Assert.False(resultModelCollection.Last().IsAssigned);
        }

        [Fact]
        public async Task GetAllStudentsPerPageAsyncShouldReturnCorrectModelCollectionIfTeacherIdIsPassed()
        {
            var teacherId = await this.CreateUserAsync("teacher@teacher.com", "Teacher", "Teachers");

            await this.CreateUserAsync("student1@student.com", "Student", "Student");
            var secondStudentId = await this.CreateUserAsync("student2@student.com", "Student2", "Student2");
            await this.AddStudentAsync(secondStudentId, teacherId);

            var secondModel = new StudentViewModel()
            {
                Id = secondStudentId,
                FullName = "Student2 Student2",
                Email = "student2@student.com",
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllStudentsPerPageAsync<StudentViewModel>(1, 2, teacherId);

            Assert.Single(resultModelCollection);
            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.FullName, resultModelCollection.First().FullName);
            Assert.Equal(secondModel.Email, resultModelCollection.First().Email);
            Assert.False(resultModelCollection.First().IsAssigned);
        }

        [Fact]
        public async Task GetAllStudentsPerPageAsyncShouldSkipCorrectly()
        {
            var firstStudentId = await this.CreateUserAsync("student1@student.com", "Student", "Student");
            await this.CreateUserAsync("student2@student.com", "Student2", "Student2");

            var firstModel = new StudentViewModel()
            {
                Id = firstStudentId,
                FullName = "Student Student",
                Email = "student1@student.com",
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllStudentsPerPageAsync<StudentViewModel>(2, 1);

            Assert.Single(resultModelCollection);

            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.FullName, resultModelCollection.Last().FullName);
            Assert.Equal(firstModel.Email, resultModelCollection.Last().Email);
            Assert.False(resultModelCollection.Last().IsAssigned);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 10)]
        public async Task GetAllStudentsPerPageAsyncShouldTakeCorrectCountPerPage(int page, int countPerPage)
        {
            for (int i = 1; i <= countPerPage * 2; i++)
            {
                await this.CreateUserAsync($"student{i}@student.com", "Student", "Student");
            }

            var resultModelCollection = await this.Service.GetAllStudentsPerPageAsync<StudentViewModel>(page, countPerPage);

            Assert.Equal(countPerPage, resultModelCollection.Count());
        }

        private async Task<string> AssignStudentToGroupAsync(string studentId, string groupId = null)
        {
            Group group;
            if (groupId != null)
            {
                group = await this.DbContext.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            }
            else
            {
                group = await this.CreateGroupAsync();
            }

            var studentGroup = new StudentGroup() { StudentId = studentId, GroupId = group.Id };
            await this.DbContext.StudentsGroups.AddAsync(studentGroup);
            await this.DbContext.SaveChangesAsync();

            // this.DbContext.Entry<Group>(group).State = EntityState.Detached;
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

        private async Task<string> CreateUserAsync(string email, string firstName, string lastName, string roleName = null)
        {
            var user = new ApplicationUser()
            {
                FirstName = firstName,
                LastName = lastName,
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

        private async Task<Group> CreateGroupAsync()
        {
            var group = new Group() { Name = "group" };
            await this.DbContext.Groups.AddAsync(group);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            return group;
        }
    }
}
