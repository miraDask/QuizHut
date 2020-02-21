namespace QuizHut.Services.ParticipantGroup
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class ParticipantGroupService : IParticipantGroupService
    {
        private readonly IRepository<ParticipantGroup> repository;

        public ParticipantGroupService(IRepository<ParticipantGroup> repository)
        {
            this.repository = repository;
        }

        public async Task CreateAsync(string groupId, string participantId)
        {
            var participantGroup = new ParticipantGroup() { GroupId = groupId, ParticipantId = participantId };
            await this.repository.AddAsync(participantGroup);
            await this.repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(string groupId, string participantId)
        {
            var participantGroup = await this.repository
                .AllAsNoTracking()
                .Where(x => x.GroupId == groupId && x.ParticipantId == participantId)
                .FirstOrDefaultAsync();

            this.repository.Delete(participantGroup);
            await this.repository.SaveChangesAsync();
        }
    }
}
