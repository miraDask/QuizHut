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
    using QuizHut.Web.ViewModels.Groups;
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

            var groupId = await this.CreateGroupAsync();
            await this.Service.AssignStudentsToGroupAsync(groupId, studentsIds);

            var groupAfterAssigningStudents = await this.DbContext.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            var idsOfAssignedStudents = groupAfterAssigningStudents.StudentstGroups.Select(x => x.StudentId);

            foreach (var id in studentsIds)
            {
                Assert.Contains(id, idsOfAssignedStudents);
            }

            Assert.Equal(5, groupAfterAssigningStudents.StudentstGroups.Count);
        }

        [Fact]
        public async Task DeleteStudentFromGroupAsyncShouldRemoveCurrentStudentFromGroup()
        {
            var groupId = await this.CreateGroupAsync();
            List<string> studentsIds = new List<string>();

            for (int i = 1; i <= 5; i++)
            {
                var studentId = await this.CreateStudentAsync();
                studentsIds.Add(studentId);
                await this.AssignStudentToGroupAsync(studentId, groupId);
            }

            await this.Service.DeleteStudentFromGroupAsync(groupId, studentsIds[0]);

            var studentsInGroupsAfterDeleteStudent = await this.DbContext.Groups
                .Where(x => x.Id == groupId).Select(x => x.StudentstGroups.Select(x => x.StudentId)).FirstOrDefaultAsync();

            Assert.DoesNotContain(studentsIds[0], studentsInGroupsAfterDeleteStudent);
            Assert.Equal(4, studentsInGroupsAfterDeleteStudent.Count());
        }

        [Fact]
        public async Task AssignEventsToGroupAsyncShouldAssignEventsCorrectly()
        {
            List<string> eventsIds = new List<string>();

            for (int i = 1; i <= 5; i++)
            {
                var eventId = await this.CreateEventAsync($"Event {i}", DateTime.UtcNow);
                eventsIds.Add(eventId);
            }

            var groupId = await this.CreateGroupAsync();
            await this.Service.AssignEventsToGroupAsync(groupId, eventsIds);

            var groupAfterAssigningEvents = await this.DbContext.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            var idsOfAssignedEvents = groupAfterAssigningEvents.EventsGroups.Select(x => x.EventId);

            foreach (var id in eventsIds)
            {
                Assert.Contains(id, idsOfAssignedEvents);
            }

            Assert.Equal(5, groupAfterAssigningEvents.EventsGroups.Count);
        }

        [Fact]
        public async Task DeleteEventFromGroupAsyncShouldRemoveCurrentEventFromGroup()
        {
            var groupId = await this.CreateGroupAsync();
            List<string> eventsIds = new List<string>();

            for (int i = 1; i <= 5; i++)
            {
                var eventId = await this.CreateEventAsync($"Event {i}", DateTime.UtcNow);
                eventsIds.Add(eventId);
                await this.AssignEventToGroupAsync(eventId, groupId);
            }

            await this.Service.DeleteEventFromGroupAsync(groupId, eventsIds[0]);

            var eventsInGroupsAfterDeleteEvent = await this.DbContext.Groups
                .Where(x => x.Id == groupId).Select(x => x.EventsGroups.Select(x => x.EventId)).FirstOrDefaultAsync();

            Assert.DoesNotContain(eventsIds[0], eventsInGroupsAfterDeleteEvent);
            Assert.Equal(4, eventsInGroupsAfterDeleteEvent.Count());
        }

        [Fact]
        public async Task CreateGroupAsyncShouldCreateCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            await this.Service.CreateGroupAsync("Test Group", creatorId);

            var group = await this.DbContext.Groups.FirstOrDefaultAsync();
            Assert.NotNull(group);
            Assert.Equal("Test Group", group.Name);
            Assert.Equal(creatorId, group.CreatorId);
            Assert.Equal(1, this.DbContext.Groups.Count());
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var groupId = await this.CreateGroupAsync();

            await this.Service.DeleteAsync(groupId);

            var group = await this.DbContext.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            Assert.Null(group);
            Assert.Equal(0, this.DbContext.Groups.Count());
        }

        [Fact]
        public async Task UpdateNameAsyncShouldChangeTheNameOfTheGroupCorrectly()
        {
            var groupId = await this.CreateGroupAsync(null, "group");
            await this.Service.UpdateNameAsync(groupId, "Test Group");

            var updatedGroup = await this.DbContext.Groups.FirstOrDefaultAsync();
            Assert.Equal("Test Group", updatedGroup.Name);
        }

        [Fact]
        public async Task GetAllGroupsCountShouldReturnCorrectCount()
        {
            var creatorId = Guid.NewGuid().ToString();

            for (int i = 1; i <= 4; i++)
            {
                var currentCreator = i % 2 == 0 ? creatorId : null;
                await this.CreateGroupAsync(currentCreator);
            }

            var caseWithCreatorIdPassedCount = await this.Service.GetAllGroupsCountAsync(creatorId);
            var caseNoCreatorIdPassedCount = await this.Service.GetAllGroupsCountAsync();

            Assert.Equal(2, caseWithCreatorIdPassedCount);
            Assert.Equal(4, caseNoCreatorIdPassedCount);
        }

        [Fact]
        public async Task GetGroupModelAsyncShouldReturnCorrectModel()
        {
            var firstgroupId = await this.CreateGroupAsync(null, "First Group");
            await this.CreateGroupAsync();

            var model = new GroupWithEventsViewModel()
            {
                Id = firstgroupId,
                Name = "First Group",
                Error = false,
            };

            var resultModel = await this.Service.GetGroupModelAsync<GroupWithEventsViewModel>(firstgroupId);

            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Name, resultModel.Name);
            Assert.False(resultModel.Error);
            Assert.Empty(resultModel.Events);
        }

        [Fact]
        public async Task GetEventsFirstGroupAsyncShouldReturnCorrectModel()
        {
            var firstgroupId = await this.CreateGroupAsync(null, "First Group");
            await this.CreateGroupAsync();
            var eventId = await this.CreateEventAsync("Event", DateTime.UtcNow);
            await this.AssignEventToGroupAsync(eventId, firstgroupId);

            var model = new GroupWithEventResultsViewModel()
            {
                Id = firstgroupId,
                Name = "First Group",
            };

            var resultModel = await this.Service.GetEventsFirstGroupAsync<GroupWithEventResultsViewModel>(eventId);

            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Name, resultModel.Name);
            Assert.Empty(resultModel.Results);
        }

        [Fact]
        public async Task GetAllByEventIdAsyncShouldReturnCorrectModelCollection()
        {
            var eventId = Guid.NewGuid().ToString();
            var creatorId = Guid.NewGuid().ToString();

            var firstGroupId = await this.CreateGroupAsync(creatorId, "First Group");
            var secondGroupId = await this.CreateGroupAsync(creatorId, "Second Group");

            await this.AssignEventToGroupAsync(eventId, firstGroupId);
            await this.AssignEventToGroupAsync(eventId, secondGroupId);

            var firstModel = new GroupAssignViewModel()
            {
                Id = firstGroupId,
                Name = "First Group",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var secondModel = new GroupAssignViewModel()
            {
                Id = secondGroupId,
                Name = "Second Group",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllByEventIdAsync<GroupAssignViewModel>(eventId);

            Assert.Equal(firstModel.Id, resultModelCollection.First().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.First().Name);
            Assert.Equal(firstModel.CreatorId, resultModelCollection.First().CreatorId);
            Assert.False(resultModelCollection.First().IsAssigned);
            Assert.Equal(secondModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(secondModel.CreatorId, resultModelCollection.Last().CreatorId);
            Assert.False(resultModelCollection.Last().IsAssigned);
            Assert.Equal(2, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetAllPerPageAsyncShouldReturnCorrectModelCollection()
        {
            var firstGroupId = await this.CreateGroupAsync(null, "First Group");
            var secondGroupId = await this.CreateGroupAsync(null, "Second Group");

            var firstModel = new GroupListViewModel()
            {
                Id = firstGroupId,
                Name = "First Group",
                StudentsCount = 0,
                EventsCount = 0,
            };

            var secondModel = new GroupListViewModel()
            {
                Id = secondGroupId,
                Name = "Second Group",
                StudentsCount = 0,
                EventsCount = 0,
            };

            var resultModelCollection = await this.Service.GetAllPerPageAsync<GroupListViewModel>(1, 2);

            Assert.Equal(2, resultModelCollection.Count());
            Assert.IsAssignableFrom<IList<GroupListViewModel>>(resultModelCollection);

            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.StudentsCount, resultModelCollection.Last().StudentsCount);
            Assert.Equal(firstModel.EventsCount, resultModelCollection.Last().EventsCount);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.StudentsCount, resultModelCollection.First().StudentsCount);
            Assert.Equal(secondModel.EventsCount, resultModelCollection.First().EventsCount);
        }

        [Fact]
        public async Task GetAllPerPageAsyncShouldReturnCorrectModelCollectionWhenCreatorIdIsPassed()
        {
            var creatorId = Guid.NewGuid().ToString();
            await this.CreateGroupAsync(null, "First Group");
            var secondGroupId = await this.CreateGroupAsync(creatorId, "Second Group");

            var secondModel = new GroupListViewModel()
            {
                Id = secondGroupId,
                Name = "Second Group",
                StudentsCount = 0,
                EventsCount = 0,
            };

            var resultModelCollection = await this.Service.GetAllPerPageAsync<GroupListViewModel>(1, 2, creatorId);

            Assert.Single(resultModelCollection);
            Assert.IsAssignableFrom<IList<GroupListViewModel>>(resultModelCollection);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.StudentsCount, resultModelCollection.First().StudentsCount);
            Assert.Equal(secondModel.EventsCount, resultModelCollection.First().EventsCount);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(1, 1000)]
        public async Task GetAllPerPageAsyncShouldTakeCorrectCountPerPage(int page, int countPerPage)
        {
            for (int i = 0; i < countPerPage * 2; i++)
            {
                await this.CreateGroupAsync();
            }

            var resultModelCollection = await this.Service.GetAllPerPageAsync<GroupListViewModel>(page, countPerPage);

            Assert.Equal(countPerPage, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetPerPageByStudentIdFilteredByStatusAsyncShouldSkipCorrectly()
        {
            var firstGroupId = await this.CreateGroupAsync(null, "First Group");
            await this.CreateGroupAsync(null, "Second Group");

            var firstModel = new GroupListViewModel()
            {
                Id = firstGroupId,
                Name = "First Group",
                StudentsCount = 0,
                EventsCount = 0,
            };

            var resultModelCollection = await this.Service.GetAllPerPageAsync<GroupListViewModel>(2, 1);

            Assert.Single(resultModelCollection);
            Assert.IsAssignableFrom<IList<GroupListViewModel>>(resultModelCollection);

            Assert.Equal(firstModel.Id, resultModelCollection.First().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.First().Name);
            Assert.Equal(firstModel.StudentsCount, resultModelCollection.First().StudentsCount);
            Assert.Equal(firstModel.EventsCount, resultModelCollection.First().EventsCount);
        }

        [Fact]
        public async Task GetAllAsyncShouldReturnCorrectModelCollection()
        {
            var creatorId = Guid.NewGuid().ToString();

            var firstGroupId = await this.CreateGroupAsync(creatorId, "First Group");
            var secondGroupId = await this.CreateGroupAsync(creatorId, "Second Group");

            var firstModel = new GroupAssignViewModel()
            {
                Id = firstGroupId,
                Name = "First Group",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var secondModel = new GroupAssignViewModel()
            {
                Id = secondGroupId,
                Name = "Second Group",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllAsync<GroupAssignViewModel>();

            Assert.Equal(2, resultModelCollection.Count());

            Assert.Equal(firstModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(firstModel.Name, resultModelCollection.Last().Name);
            Assert.Equal(firstModel.CreatorId, resultModelCollection.Last().CreatorId);
            Assert.False(resultModelCollection.Last().IsAssigned);
            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.CreatorId, resultModelCollection.First().CreatorId);
            Assert.False(resultModelCollection.First().IsAssigned);
        }

        [Fact]
        public async Task GetAllAsyncShouldReturnOnlyGroupsCreatedByCurrentCreatorWhenCreatorIdIsPassed()
        {
            var creatorId = Guid.NewGuid().ToString();

            await this.CreateGroupAsync(null, "First Group");
            var secondGroupId = await this.CreateGroupAsync(creatorId, "Second Group");

            var secondModel = new GroupAssignViewModel()
            {
                Id = secondGroupId,
                Name = "Second Group",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllAsync<GroupAssignViewModel>(creatorId);

            Assert.Single(resultModelCollection);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.CreatorId, resultModelCollection.First().CreatorId);
            Assert.False(resultModelCollection.First().IsAssigned);
        }

        [Fact]
        public async Task GetAllAsyncShouldReturnOnlyUnAssignedGroupsToCurrentEventWhenEventIdIsPassed()
        {
            var creatorId = Guid.NewGuid().ToString();
            var eventId = Guid.NewGuid().ToString();

            var firstGroupId = await this.CreateGroupAsync(null, "First Group");
            await this.AssignEventToGroupAsync(eventId, firstGroupId);
            var secondGroupId = await this.CreateGroupAsync(creatorId, "Second Group");

            var secondModel = new GroupAssignViewModel()
            {
                Id = secondGroupId,
                Name = "Second Group",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllAsync<GroupAssignViewModel>(null, eventId);

            Assert.Single(resultModelCollection);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.CreatorId, resultModelCollection.First().CreatorId);
            Assert.False(resultModelCollection.First().IsAssigned);
        }

        [Fact]
        public async Task GetAllAsyncShouldReturnOnlyUnAssignedGroupsToCurrentEventAndCreatedByCurrentCreatorWhenEventIdAndCreatorIdArePassed()
        {
            var creatorId = Guid.NewGuid().ToString();
            var eventId = Guid.NewGuid().ToString();

            var firstGroupId = await this.CreateGroupAsync(null, "First Group");
            await this.AssignEventToGroupAsync(eventId, firstGroupId);
            var secondGroupId = await this.CreateGroupAsync(creatorId, "Second Group");

            var secondModel = new GroupAssignViewModel()
            {
                Id = secondGroupId,
                Name = "Second Group",
                CreatorId = creatorId,
                IsAssigned = false,
            };

            var resultModelCollection = await this.Service.GetAllAsync<GroupAssignViewModel>(creatorId, eventId);

            Assert.Single(resultModelCollection);

            Assert.Equal(secondModel.Id, resultModelCollection.First().Id);
            Assert.Equal(secondModel.Name, resultModelCollection.First().Name);
            Assert.Equal(secondModel.CreatorId, resultModelCollection.First().CreatorId);
            Assert.False(resultModelCollection.First().IsAssigned);
        }

        private async Task<string> CreateGroupAsync(string creatorId = null, string name = null)
        {
            var group = new Group()
            {
                Name = name ?? "group",
                CreatorId = creatorId ?? Guid.NewGuid().ToString(),
            };

            await this.DbContext.Groups.AddAsync(group);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Group>(group).State = EntityState.Detached;
            return group.Id;
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

        private async Task AssignStudentToGroupAsync(string studentId, string groupId)
        {
            var studentGroup = new StudentGroup() { StudentId = studentId, GroupId = groupId };
            await this.DbContext.StudentsGroups.AddAsync(studentGroup);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<StudentGroup>(studentGroup).State = EntityState.Detached;
        }

        private async Task AssignEventToGroupAsync(string eventId, string groupId)
        {
            var eventGroup = new EventGroup() { EventId = eventId, GroupId = groupId };
            await this.DbContext.EventsGroups.AddAsync(eventGroup);

            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<EventGroup>(eventGroup).State = EntityState.Detached;
        }
    }
}
