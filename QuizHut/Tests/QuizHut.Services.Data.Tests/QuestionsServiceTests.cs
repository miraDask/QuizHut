namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data;
    using QuizHut.Data.Models;
    using QuizHut.Data.Repositories;
    using QuizHut.Services.Mapping;
    using QuizHut.Services.Questions;
    using QuizHut.Web.ViewModels.Questions;
    using Xunit;

    public class QuestionsServiceTests
    {
        private readonly ApplicationDbContext dbContext;
        private readonly EfDeletableEntityRepository<Question> questionsRepository;
        private readonly EfDeletableEntityRepository<Quiz> quizzesRepository;
        private readonly QuestionsService service;

        public QuestionsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
              .UseInMemoryDatabase(Guid.NewGuid().ToString())
              .Options;
            this.dbContext = new ApplicationDbContext(options);
            this.questionsRepository = new EfDeletableEntityRepository<Question>(this.dbContext);
            this.quizzesRepository = new EfDeletableEntityRepository<Quiz>(this.dbContext);
            this.service = new QuestionsService(this.questionsRepository, this.quizzesRepository);
            AutoMapperConfig.RegisterMappings(typeof(QuestionInputModel).GetTypeInfo().Assembly);
            AutoMapperConfig.RegisterMappings(typeof(QuestionViewModel).GetTypeInfo().Assembly);
        }

        [Fact]
        public async Task CreateQuestionAsyncShouldCreateNewQuestionInDb()
        {
            var quizId = await this.CreateQuizAsync();

            var newQuestionId = await this.service.CreateQuestionAsync(quizId, "First question text");
            var questionsCount = this.dbContext.Questions.ToArray().Count();
            Assert.Equal(1, questionsCount);
            Assert.NotNull(await this.dbContext.Questions.FirstOrDefaultAsync(x => x.Id == newQuestionId));
        }

        [Fact]
        public async Task UpdateAllQuestionsInQuizNumerationShouldUpdateNumerationOfQuestionsCorrectly()
        {
            var quizId = await this.CreateQuizAsync();
            var firstQuestionId = await this.CreateAndAddQuestionToQuiz(quizId, 1, "text");
            var secondQuestionId = await this.CreateAndAddQuestionToQuiz(quizId, 2, "text");
            var thirdQuestionId = await this.CreateAndAddQuestionToQuiz(quizId, 3, "text");

            await this.DeleteQuestionAsync(secondQuestionId, quizId);

            await this.service.UpdateAllQuestionsInQuizNumeration(quizId);

            var firstQuestion = await this.dbContext.Questions.FirstOrDefaultAsync(x => x.Id == firstQuestionId);
            var thirdQuestion = await this.dbContext.Questions.FirstOrDefaultAsync(x => x.Id == thirdQuestionId);

            Assert.Equal(1, firstQuestion.Number);
            Assert.Equal(2, thirdQuestion.Number);
        }

        [Fact]
        public async Task DeleteQuestionByIdAsyncShouldDeleteCorrectly()
        {
            var quizId = await this.CreateQuizAsync();
            var questionId = await this.CreateAndAddQuestionToQuiz(quizId, 1, "text");
            await this.service.DeleteQuestionByIdAsync(questionId);

            var questionsCount = this.dbContext.Questions.Where(x => !x.IsDeleted).ToArray().Count();
            var question = await this.dbContext.Questions.FindAsync(questionId);
            Assert.Equal(0, questionsCount);
            Assert.True(question.IsDeleted);
        }

        [Fact]
        public async Task UpdateShouldUpdateQuestionCorrectly()
        {
            var quizId = await this.CreateQuizAsync();
            var questionId = await this.CreateAndAddQuestionToQuiz(quizId, 1, "text");
            await this.service.Update(questionId, "Updated text");

            var question = await this.dbContext.Questions.FindAsync(questionId);
            Assert.Equal("Updated text", question.Text);
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnCorrectModel()
        {
            var text = "First question text";
            var quizId = await this.CreateQuizAsync();
            var newQuestionId = await this.service.CreateQuestionAsync(quizId, text);

            var model = new QuestionInputModel()
            {
                Id = newQuestionId,
                Text = text,
            };

            var resultModel = await this.service.GetByIdAsync<QuestionInputModel>(newQuestionId);

            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Text, resultModel.Text);
        }

        [Fact]
        public async Task GetAllByQuizIdAsyncShouldReturnCorrectModelCollection()
        {
            var quizId = await this.CreateQuizAsync();
            var firstQuestionId = await this.service.CreateQuestionAsync(quizId, "First Question");
            var secondQuestionId = await this.service.CreateQuestionAsync(quizId, "Second Question");

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

            var resultModelCollection = await this.service.GetAllByQuizIdAsync<QuestionViewModel>(quizId);
            Assert.Equal(firstModel.Id, resultModelCollection.First().Id);
            Assert.Equal(firstModel.Text, resultModelCollection.First().Text);
            Assert.Equal(firstModel.Answers.Count, resultModelCollection.First().Answers.Count);
            Assert.Equal(secondModel.Id, resultModelCollection.Last().Id);
            Assert.Equal(secondModel.Text, resultModelCollection.Last().Text);
            Assert.Equal(secondModel.Answers.Count, resultModelCollection.Last().Answers.Count);
            Assert.Equal(2, resultModelCollection.Count());
        }

        [Fact]
        public async Task GetAllByQuizIdCountShouldReturnCorrectCount()
        {
            var quizId = await this.CreateQuizAsync();
            await this.service.CreateQuestionAsync(quizId, "First Question");
            await this.service.CreateQuestionAsync(quizId, "Second Question");
            var count = this.service.GetAllByQuizIdCount(quizId);

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetQuestionByQuizIdAndNumberAsyncShouldReturnCorrectModel()
        {
            var text = "First question text";
            var quizId = await this.CreateQuizAsync();
            var firstQuestionId = await this.service.CreateQuestionAsync(quizId, text);

            var model = new QuestionViewModel()
            {
                Id = firstQuestionId,
                Text = text,
                Number = 1,
            };

            var resultModel = await this.service.GetQuestionByQuizIdAndNumberAsync<QuestionViewModel>(quizId, 1);

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

            var quiz = await this.dbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            quiz.Questions.Add(question);
            this.dbContext.Update(quiz);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Question>(question).State = EntityState.Detached;

            return question.Id;
        }

        private async Task<string> CreateQuizAsync()
        {
            var quiz = new Quiz() { Name = "Test Quiz" };
            await this.dbContext.Quizzes.AddAsync(quiz);
            await this.dbContext.SaveChangesAsync();

            return quiz.Id;
        }

        private async Task DeleteQuestionAsync(string questionId, string quizId)
        {
            var quiz = await this.dbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);
            var question = await this.dbContext.Questions.FirstOrDefaultAsync(x => x.Id == questionId);

            quiz.Questions.Remove(question);
            this.dbContext.Quizzes.Update(quiz);
            this.dbContext.Questions.Remove(question);
            await this.dbContext.SaveChangesAsync();
            this.dbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
        }
    }
}
