namespace QuizHut.Services.Question
{
    using System.Threading.Tasks;

    public interface IQuestionService
    {
        Task<string> AddNewQuestionAsync(string quizId, string quizText);

        Task DeleteQuestionByIdAsync(string id);

        Task UpdateAllQuestionsInQuizNumeration(string quizId);

        Task Update(string id, string text);
    }
}
