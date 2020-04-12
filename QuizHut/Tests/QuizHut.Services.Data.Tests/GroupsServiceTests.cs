namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Groups;
    using Xunit;

    public class GroupsServiceTests : BaseServiceTests
    {
        private IGroupsService Service => this.ServiceProvider.GetRequiredService<IGroupsService>();

        [Fact]
        public async Task AssignStudentsToGroupAsyncShouldAssignStudentsCorrectly()
        {
            List<string> studentsIds = new List<string>();

            for (int i = 1; i <= 5; i++)
            {
                var studentId = await this.CreateStudentAsync();
                studentsIds.Add(studentId);
            }

            var group = await this.CreateGroupAsync();
            await this.Service.AssignStudentsToGroupAsync(group.Id, studentsIds);

            var groupAfterAssigningStudents = await this.DbContext.Groups.FirstOrDefaultAsync(x => x.Id == group.Id);
            var idsOfAssignedStudents = groupAfterAssigningStudents.StudentstGroups.Select(x => x.StudentId);

            foreach (var id in studentsIds)
            {
                Assert.Contains(id, idsOfAssignedStudents);
            }

            Assert.Equal(5, groupAfterAssigningStudents.StudentstGroups.Count);
        }

        [Fact]
        public async Task AssignEventsToGroupAsyncAsyncShouldAssignEventsCorrectly()
        {
            List<string> eventsIds = new List<string>();

            for (int i = 1; i <= 5; i++)
            {
                var eventId = await this.CreateEventAsync($"Event {i}", DateTime.UtcNow);
                eventsIds.Add(eventId);
            }

            var group = await this.CreateGroupAsync();
            await this.Service.AssignEventsToGroupAsync(group.Id, eventsIds);

            var groupAfterAssigningEvents = await this.DbContext.Groups.FirstOrDefaultAsync(x => x.Id == group.Id);
            var idsOfAssignedEvents = groupAfterAssigningEvents.EventsGroups.Select(x => x.EventId);

            foreach (var id in eventsIds)
            {
                Assert.Contains(id, idsOfAssignedEvents);
            }

            Assert.Equal(5, groupAfterAssigningEvents.EventsGroups.Count);
        }

        private async Task<Group> CreateGroupAsync()
        {
            var group = new Group() { Name = "group" };
            await this.DbContext.Groups.AddAsync(group);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            return group;
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

        private async Task<string> CreateEventAsync(string name, DateTime activation)
        {
            var creatorId = Guid.NewGuid().ToString();

            var @event = new Event
            {
                Name = name,
                Status = Status.Pending,
                ActivationDateAndTime = activation,
                DurationOfActivity = TimeSpan.FromMinutes(30),
                CreatorId = creatorId,
            };

            await this.DbContext.Events.AddAsync(@event);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Event>(@event).State = EntityState.Detached;

            return @event.Id;
        }
    }
}
