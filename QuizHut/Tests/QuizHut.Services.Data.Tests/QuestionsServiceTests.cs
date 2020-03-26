namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data;
    using QuizHut.Data.Models;
    using QuizHut.Data.Repositories;
    using QuizHut.Services.Questions;
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
