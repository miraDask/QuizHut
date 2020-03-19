namespace QuizHut.Web.Infrastructure.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    using QuizHut.Web.ViewModels.Questions;
    using QuizHut.Web.ViewModels.Quizzes;

    public class ResultHelper : IResultHelper
    {
        public int CalculateResult(
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
