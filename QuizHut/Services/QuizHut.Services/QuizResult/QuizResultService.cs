namespace QuizHut.Services.QuizResult
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Quizzes;

    public class QuizResultService : IQuizResultService
    {
        private readonly IDeletableEntityRepository<QuizResult> repository;

        public QuizResultService(IDeletableEntityRepository<QuizResult> repository)
        {
            this.repository = repository;
        }

        public async Task CreateQuizResultAsync(string participantId, int points, int maxPoints, string quizId)
        {
            var quizResult = new QuizResult()
            {
                QuizId = quizId,
                ParticipantId = participantId,
                Points = points,
                MaxPoints = maxPoints,
            };

            await this.repository.AddAsync(quizResult);
            await this.repository.SaveChangesAsync();
        }

        public async Task<QuizResultViewModel> GetResultModel(
            string quizId,
            string participantId,
            IList<QuestionViewModel> originalQuizQuestions,
            IList<AttemtedQuizQuestionViewModel> attemptedQuizQuestions)
        {
            var points = this.CalculateResult(originalQuizQuestions, attemptedQuizQuestions);
            await this.CreateQuizResultAsync(participantId, points, originalQuizQuestions.Count, quizId);
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
