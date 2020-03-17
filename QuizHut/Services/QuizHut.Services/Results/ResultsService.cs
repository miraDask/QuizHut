namespace QuizHut.Services.Results
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Quizzes;

    public class ResultsService : IResultsService
    {
        private readonly IDeletableEntityRepository<Result> repository;
        private readonly IDeletableEntityRepository<Quiz> quizRepository;
        private readonly IDeletableEntityRepository<Event> eventRepository;

        public ResultsService(
            IDeletableEntityRepository<Result> repository,
            IDeletableEntityRepository<Quiz> quizRepository,
            IDeletableEntityRepository<Event> eventRepository)
        {
            this.repository = repository;
            this.quizRepository = quizRepository;
            this.eventRepository = eventRepository;
        }

        public async Task<IEnumerable<T>> GetAllResultsByEventIdAsync<T>(string eventId, string groupName)
        => await this.repository
        .AllAsNoTracking()
        .Where(x => x.EventId == eventId)
        .Where(x => x.Student.StudentsInGroups.Any(x => x.Group.Name == groupName))
        .To<T>()
        .ToListAsync();

        public async Task CreateResultAsync(string studentId, int points, int maxPoints, string quizId)
        {
            var @event = await this.eventRepository
                .AllAsNoTracking()
                .Where(x => x.QuizId == quizId)
                .FirstOrDefaultAsync();
            var result = new Result()
            {
                Points = points,
                StudentId = studentId,
                MaxPoints = maxPoints,
                EventId = @event.Id,
            };

            @event.Results.Add(result);
            await this.repository.AddAsync(result);
            await this.eventRepository.SaveChangesAsync();
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
                await this.CreateResultAsync(studentId, points, originalQuizQuestions.Count, quizId);
            }

            return new QuizResultViewModel()
            {
                Points = points,
                MaxPoints = originalQuizQuestions.Count,
            };
        }

        public async Task<IEnumerable<T>> GetAllByStudentIdAsync<T>(string id)
        => await this.repository
            .AllAsNoTracking()
            .Where(x => x.StudentId == id)
            .OrderByDescending(x => x.CreatedOn)
            .To<T>()
            .ToListAsync();

        private int CalculateResult(
            IList<QuestionViewModel> originalQuizQuestions,
            IList<AttemtedQuizQuestionViewModel> attemptedQuizQuestions)
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
