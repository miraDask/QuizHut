namespace QuizHut.Services.ParticipantGroup
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;

    public class ParticipantGroupService : IParticipantGroupService
    {
        private readonly IDeletableEntityRepository<ParticipantGroup> repository;

        public ParticipantGroupService(IDeletableEntityRepository<ParticipantGroup> repository)
        {
            this.repository = repository;
        }

        public async Task CreateAsync(string groupId, string participantId)
        {
            var participantGroup = new ParticipantGroup() { GroupId = groupId, ParticipantId = participantId };
            await this.repository.AddAsync(participantGroup);
            await this.repository.SaveChangesAsync();
        }
    }
}
