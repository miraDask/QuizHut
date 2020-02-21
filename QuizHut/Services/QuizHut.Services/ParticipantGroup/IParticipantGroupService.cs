namespace QuizHut.Services.ParticipantGroup
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IParticipantGroupService
    {
        Task CreateAsync(string groupId, string participantId);
    }
}
