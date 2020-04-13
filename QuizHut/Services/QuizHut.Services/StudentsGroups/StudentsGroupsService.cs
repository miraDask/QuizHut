namespace QuizHut.Services.StudentsGroups
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class StudentsGroupsService : IStudentsGroupsService
    {
        private readonly IRepository<StudentGroup> repository;

        public StudentsGroupsService(IRepository<StudentGroup> repository)
        {
            this.repository = repository;
        }

        public async Task CreateStudentGroupAsync(string groupId, string studentId)
        {
            var studentGroup = new StudentGroup() { GroupId = groupId, StudentId = studentId };
            var studentExists = await this.repository
                .AllAsNoTracking()
                .Where(x => x.GroupId == groupId && x.StudentId == studentId)
                .FirstOrDefaultAsync()
                != null;

            if (!studentExists)
            {
                await this.repository.AddAsync(studentGroup);
                await this.repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string groupId, string studentId)
        {
            var studentGroup = await this.repository
                .AllAsNoTracking()
                .Where(x => x.GroupId == groupId && x.StudentId == studentId)
                .FirstOrDefaultAsync();

            this.repository.Delete(studentGroup);
            await this.repository.SaveChangesAsync();
        }
    }
}
