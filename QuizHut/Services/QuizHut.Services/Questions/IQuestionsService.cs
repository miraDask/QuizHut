namespace QuizHut.Services.Questions
{
    using System.Threading.Tasks;

    public interface IQuestionsService
    {
        Task<string> CreateQuestionAsync(string quizId, string quizText);

        Task DeleteQuestionByIdAsync(string id);

        Task UpdateAllQuestionsInQuizNumeration(string quizId);

        Task Update(string id, string text);
    }
}
