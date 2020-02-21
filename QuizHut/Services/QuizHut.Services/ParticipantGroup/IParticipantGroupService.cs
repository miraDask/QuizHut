namespace QuizHut.Services.ParticipantGroup
{
    using System.Threading.Tasks;

    public interface IParticipantGroupService
    {
        Task CreateAsync(string groupId, string participantId);

        Task DeleteAsync(string groupId, string participantId);
    }
}
