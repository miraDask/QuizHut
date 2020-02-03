namespace QuizHut.Services.Question
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Question;

    public interface IQuestionService
    {
        Task<int> AddNewQuestionAsync(QuestionViewModel questionModel);

        Task<int> GetQuizIdByQuestionIdAsync(int id);
    }
}
