namespace QuizHut.Services.EventsResults
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Results;
    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Quizzes;

    public class EventsResultsService : IEventsResultsService
    {
        private readonly IDeletableEntityRepository<EventResult> repository;
        private readonly IResultsService resultsService;
        private readonly IDeletableEntityRepository<Quiz> quizRepository;

        public EventsResultsService(
            IDeletableEntityRepository<EventResult> repository,
            IResultsService resultsService,
            IDeletableEntityRepository<Quiz> quizRepository)
        {
            this.repository = repository;
            this.resultsService = resultsService;
            this.quizRepository = quizRepository;
        }

        public async Task CreateEventResultAsync(string studentId, int points, int maxPoints, string quizId)
        {
            var resultId = await this.resultsService.CreateResultAsync(studentId, points, maxPoints);
            var eventId = await this.quizRepository
                .AllAsNoTracking()
                .Where(x => x.Id == quizId)
                .Select(x => x.EventId)
                .FirstOrDefaultAsync();

            var eventResult = new EventResult()
            {
                EventId = eventId,
                ResultId = resultId,
            };

            await this.repository.AddAsync(eventResult);
            await this.repository.SaveChangesAsync();
        }

        public async Task<QuizResultViewModel> GetResultModel(
            string quizId,
            string studentId,
            IList<QuestionViewModel> originalQuizQuestions,
            IList<AttemtedQuizQuestionViewModel> attemptedQuizQuestions)
        {
            var points = this.CalculateResult(originalQuizQuestions, attemptedQuizQuestions);

            var quizCreatorId = await this.quizRepository
                .AllAsNoTracking()
                .Where(x => x.Id == quizId)
                .Select(x => x.CreatorId)
                .FirstOrDefaultAsync();
            if (quizCreatorId != studentId)
            {
                await this.CreateEventResultAsync(studentId, points, originalQuizQuestions.Count, quizId);
            }

            return new QuizResultViewModel()
            {
                Points = points,
                MaxPoints = originalQuizQuestions.Count,
            };
        }

        private int CalculateResult(IList<QuestionViewModel> originalQuizQuestions, IList<AttemtedQuizQuestionViewModel> attemptedQuizQuestions)
        {
            var totalPoints = 0;
            foreach (var question in originalQuizQuestions)
            {
                var points = 0;
                var correspondingAttendedQuestion = attemptedQuizQuestions.FirstOrDefault(x => x.Id == question.Id);
                var corectAnswersInQuestion = question.Answers.Where(x => x.IsRightAnswer).Count();
                var originalAnswers = question.Answers;
                foreach (var answer in originalAnswers)
                {
                    var correspondingAnswerAttempt = correspondingAttendedQuestion.Answers.FirstOrDefault(x => x.Id == answer.Id);
                    if (answer.IsRightAnswer == false && correspondingAnswerAttempt.IsRightAnswerAssumption == false)
                    {
                        continue;
                    }
                    else if (answer.IsRightAnswer != correspondingAnswerAttempt.IsRightAnswerAssumption)
                    {
                        break;
                    }
                    else
                    {
                        points++;
                    }
                }

                if (points == corectAnswersInQuestion)
                {
                    totalPoints++;
                }
            }

            return totalPoints;
        }
    }
}
