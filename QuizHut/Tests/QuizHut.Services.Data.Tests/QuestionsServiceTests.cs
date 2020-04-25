namespace QuizHut.Services.Data.Tests
{
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.Questions;
    using QuizHut.Web.ViewModels.Questions;
    using Xunit;

    public class QuestionsServiceTests : BaseServiceTests
    {
        private IQuestionsService Service => this.ServiceProvider.GetRequiredService<IQuestionsService>();

        [Fact]
        public async Task A_CreateQuestionAsyncShouldCreateNewQuestionInDb()
        {
            var quizId = await this.CreateQuizAsync();

            var newQuestionId = await this.Service.CreateQuestionAsync(quizId, "First question text");
            var questionsCount = this.DbContext.Questions.ToArray().Count();
            Assert.Equal(1, questionsCount);
            Assert.NotNull(await this.DbContext.Questions.FirstOrDefaultAsync(x => x.Id == newQuestionId));
        }

        [Fact]
        public async Task B_UpdateAllQuestionsInQuizNumerationShouldUpdateNumerationOfQuestionsCorrectly()
        {
            var quizId = await this.CreateQuizAsync();
            var firstQuestionId = await this.CreateAndAddQuestionToQuiz(quizId, 1, "text");
            var secondQuestionId = await this.CreateAndAddQuestionToQuiz(quizId, 2, "text");
            var thirdQuestionId = await this.CreateAndAddQuestionToQuiz(quizId, 3, "text");

            await this.DeleteQuestionAsync(secondQuestionId, quizId);

            await this.Service.UpdateAllQuestionsInQuizNumeration(quizId);

            var firstQuestion = await this.DbContext.Questions.FirstOrDefaultAsync(x => x.Id == firstQuestionId);
            var thirdQuestion = await this.DbContext.Questions.FirstOrDefaultAsync(x => x.Id == thirdQuestionId);

            Assert.Equal(1, firstQuestion.Number);
            Assert.Equal(2, thirdQuestion.Number);
        }

        [Fact]
        public async Task C_DeleteQuestionByIdAsyncShouldDeleteCorrectly()
        {
            var quizId = await this.CreateQuizAsync();
            var questionId = await this.CreateAndAddQuestionToQuiz(quizId, 1, "text");
            await this.Service.DeleteQuestionByIdAsync(questionId);

            var questionsCount = this.DbContext.Questions.Where(x => !x.IsDeleted).ToArray().Count();
            var question = await this.DbContext.Questions.FindAsync(questionId);
            Assert.Equal(0, questionsCount);
            Assert.True(question.IsDeleted);
        }

        [Fact]
        public async Task D_UpdateShouldUpdateQuestionCorrectly()
        {
            var quizId = await this.CreateQuizAsync();
            var questionId = await this.CreateAndAddQuestionToQuiz(quizId, 1, "text");
            await this.Service.Update(questionId, "Updated text");

            var question = await this.DbContext.Questions.FindAsync(questionId);
            Assert.Equal("Updated text", question.Text);
        }

        [Fact]
        public async Task E_GetByIdAsyncShouldReturnCorrectModel()
        {
            AutoMapperConfig.RegisterMappings(typeof(QuestionInputModel).GetTypeInfo().Assembly);
            var text = "First question text";
            var quizId = await this.CreateQuizAsync();
            var newQuestionId = await this.Service.CreateQuestionAsync(quizId, text);

            var model = new QuestionInputModel()
            {
                Id = newQuestionId,
                Text = text,
            };

            var resultModel = await this.Service.GetByIdAsync<QuestionInputModel>(newQuestionId);

            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Text, resultModel.Text);
        }

        [Fact]
        public async Task F_GetAllByQuizIdAsyncShouldReturnCorrectModelCollection()
        {
            AutoMapperConfig.RegisterMappings(typeof(QuestionViewModel).GetTypeInfo().Assembly);
            var quizId = await this.CreateQuizAsync();
            var firstQuestionId = await this.Service.CreateQuestionAsync(quizId, "First Question");
            var secondQuestionId = await this.Service.CreateQuestionAsync(quizId, "Second Question");

            var firstModel = new QuestionViewModel()
            {
                Id = firstQuestionId,
                Text = "First Question",
                Number = 1,
            };

            var secondModel = new QuestionViewModel()
            {
                Id = secondQuestionId,
                Text = "Second Question",
                Number = 2,
            };

            var resultModelCollection = await this.Service.GetAllByQuizIdAsync<QuestionViewModel>(quizId);
            Assert.Equal(firstModel.Id, resultModelCollection.First().Id);
            Assert.Equal(firstModel.Text, resultModelCollection.First().Text);
            Assert.Equal(firstModel.Answers.Count, resultModelCollection.First().Answers.Count);
            Assert.Equal(secondModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(secondModel.Text, resultModelCollection.Last().Text);
            Assert.Equal(secondModel.Answers.Count, resultModelCollection.Last().Answers.Count);
            Assert.Equal(2, resultModelCollection.Count());
        }

        [Fact]
        public async Task G_GetAllByQuizIdCountShouldReturnCorrectCount()
        {
            var quizId = await this.CreateQuizAsync();
            await this.Service.CreateQuestionAsync(quizId, "First Question");
            await this.Service.CreateQuestionAsync(quizId, "Second Question");
            var count = await this.Service.GetAllByQuizIdCountAsync(quizId);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task H_GetQuestionByQuizIdAndNumberAsyncShouldReturnCorrectModel()
        {
            AutoMapperConfig.RegisterMappings(typeof(QuestionViewModel).GetTypeInfo().Assembly);
            var text = "First question text";
            var quizId = await this.CreateQuizAsync();
            var firstQuestionId = await this.Service.CreateQuestionAsync(quizId, text);

            var model = new QuestionViewModel()
            {
                Id = firstQuestionId,
                Text = text,
                Number = 1,
            };

            var resultModel = await this.Service.GetQuestionByQuizIdAndNumberAsync<QuestionViewModel>(quizId, 1);

            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Text, resultModel.Text);
            Assert.Equal(model.Number, resultModel.Number);
        }

        private async Task<string> CreateAndAddQuestionToQuiz(string quizId, int questionNumber, string text)
        {
            var question = new Question
            {
                Number = questionNumber,
                Text = text,
                QuizId = quizId,
            };

            var quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            quiz.Questions.Add(question);
            this.DbContext.Update(quiz);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Question>(question).State = EntityState.Detached;

            return question.Id;
        }

        private async Task<string> CreateQuizAsync()
        {
            var quiz = new Quiz() { Name = "Test Quiz" };
            await this.DbContext.Quizzes.AddAsync(quiz);
            await this.DbContext.SaveChangesAsync();

            return quiz.Id;
        }

        private async Task DeleteQuestionAsync(string questionId, string quizId)
        {
            var quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            var question = await this.DbContext.Questions.FirstOrDefaultAsync(x => x.Id == questionId);

            quiz.Questions.Remove(question);
            this.DbContext.Quizzes.Update(quiz);
            this.DbContext.Questions.Remove(question);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
        }
    }
}
