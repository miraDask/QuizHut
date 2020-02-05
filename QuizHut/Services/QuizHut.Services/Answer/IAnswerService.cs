﻿namespace QuizHut.Services.Answer
{
    using System.Threading.Tasks;

    using QuizHut.Web.ViewModels.Answer;

    public interface IAnswerService
    {
        Task<string> AddNewAnswerAsync(AnswerViewModel answerViewModel);
    }
}
