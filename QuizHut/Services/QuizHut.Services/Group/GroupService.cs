namespace QuizHut.Services.Group
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.ParticipantGroup;
    using QuizHut.Services.QuizGroup;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Participants;
    using QuizHut.Web.ViewModels.Quizzes;

    public class GroupService : IGroupService
    {
        private readonly IDeletableEntityRepository<Group> repository;
        private readonly IQuizGroupService quizGroupService;
        private readonly IParticipantGroupService participantGroupService;

        public GroupService(
            IDeletableEntityRepository<Group> repository,
            IQuizGroupService quizGroupService,
            IParticipantGroupService participantGroupService)
        {
            this.repository = repository;
            this.quizGroupService = quizGroupService;
            this.participantGroupService = participantGroupService;
        }

        public async Task AssignQuizzesToGroupAsync(string groupId, List<string> quizzesIds)
        {
            foreach (var quizId in quizzesIds)
            {
                await this.quizGroupService.CreateAsync(groupId, quizId);
            }
        }

        public async Task AssignParticipantsToGroupAsync(string groupId, IList<string> participantsIds)
        {
            foreach (var participantId in participantsIds)
            {
                await this.participantGroupService.CreateAsync(groupId, participantId);
            }
        }

        public async Task<string> CreateAsync(string name, string creatorId)
        {
            var group = new Group() { Name = name, CreatorId = creatorId };
            await this.repository.AddAsync(group);
            await this.repository.SaveChangesAsync();
            return group.Id;
        }

        public async Task<IList<T>> GetAllByCreatorIdAsync<T>(string id)
         => await this.repository
                .AllAsNoTracking()
                .Where(x => x.CreatorId == id)
                .OrderByDescending(x => x.CreatedOn)
                .To<T>()
                .ToListAsync();

        public async Task<GroupWithQuizzesViewModel> GetGroupModelAsync(string groupId, string creatorId, IList<QuizAssignViewModel> quizzes)
        {
            var group = await this.repository.AllAsNoTracking().Where(x => x.Id == groupId).FirstOrDefaultAsync();
            var model = new GroupWithQuizzesViewModel()
            {
                GroupId = groupId,
                Name = group.Name,
                Quizzes = quizzes.Where(x => x.CreatorId == creatorId).ToList(),
            };

            return model;
        }

        public async Task<GroupDetailsViewModel> GetGroupDetailsModelAsync(string groupId)
        {
            var group = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == groupId)
                .Select(x => new GroupDetailsViewModel() 
                {
                    Id = x.Id,
                    Name = x.Name,
                    Quizzes = x.QuizzesGroups.Select(x => new QuizAssignViewModel()
                    {
                        Name = x.Quiz.Name,
                        Id = x.QuizId,
                    }).ToList(),
                    Participants = x.ParticipanstGroups.Select(x => new ParticipantViewModel()
                    {
                        Id = x.ParticipantId,
                        FullName = $"{x.Participant.FirstName} {x.Participant.LastName}",
                        Email = x.Participant.Email,
                    }).ToList(),
                })
                .FirstOrDefaultAsync();

            return group;
        }

        public async Task DeleteAsync(string groupId)
        {
            var group = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == groupId)
                .FirstOrDefaultAsync();
            this.repository.Delete(group);
            await this.repository.SaveChangesAsync();
        }

        public async Task DeleteQuizFromGroupAsync(string groupId, string quizId)
        {
            await this.quizGroupService.DeleteAsync(groupId, quizId);
        }

        public async Task DeleteParticipantFromGroupAsync(string groupId, string participantId)
        {
            await this.participantGroupService.DeleteAsync(groupId, participantId);
        }
    }
}
