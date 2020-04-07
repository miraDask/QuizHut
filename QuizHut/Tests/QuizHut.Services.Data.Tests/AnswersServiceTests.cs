namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Data.Repositories;
    using QuizHut.Services.Answers;
    using QuizHut.Services.Mapping;
    using QuizHut.Web.ViewModels.Answers;
    using Xunit;

    public class AnswersServiceTests
    {
        private readonly IDeletableEntityRepository<Answer> repository;
        private readonly AnswersService service;
        private readonly ApplicationDbContext dbContext;
        private readonly string firstAnswerId;

        public AnswersServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            this.dbContext = new ApplicationDbContext(options);
            this.repository = new EfDeletableEntityRepository<Answer>(this.dbContext);
            this.service = new AnswersService(this.repository);
            AutoMapperConfig.RegisterMappings(typeof(AnswerViewModel).GetTypeInfo().Assembly);
            this.firstAnswerId = this.CreateFirstAnswer();
        }

        [Fact]
        public async Task CreateAnswerAsyncShouldCreateNewAnswerInDb()
        {
            var questionId = Guid.NewGuid().ToString();

            await this.service.CreateAnswerAsync(answerText: "Second answer", isRightAnswer: false, questionId);
            var secondAnswer = await this.dbContext.Answers.Where(x => x.Id != this.firstAnswerId).FirstOrDefaultAsync();
            var answersCount = this.dbContext.Answers.ToArray().Count();
            Assert.Equal(2, answersCount);
            Assert.Equal("Second answer", secondAnswer.Text);
            Assert.Equal(questionId, secondAnswer.QuestionId);
            Assert.False(secondAnswer.IsRightAnswer);
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnCorrectModel()
        {
            var answer = await this.dbContext.Answers.FirstOrDefaultAsync(x => x.Id == this.firstAnswerId);

            var model = new AnswerViewModel()
            {
                Id = answer.Id,
                Text = answer.Text,
                QuestionId = answer.QuestionId,
                IsRightAnswer = answer.IsRightAnswer,
            };

            var resultModel = await this.service.GetByIdAsync<AnswerViewModel>(answer.Id);

            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Text, resultModel.Text);
            Assert.True(resultModel.IsRightAnswer);
            Assert.Equal(model.QuestionId, resultModel.QuestionId);
        }

        [Fact]
        public async Task UpdateAsyncShouldUpdateCorrectly()
        {
            await this.service.UpdateAsync(this.firstAnswerId, "First test answer", false);
            var updatedAnswer = await this.dbContext.Answers.FirstOrDefaultAsync(x => x.Id == this.firstAnswerId);

            Assert.Equal("First test answer", updatedAnswer.Text);
            Assert.False(updatedAnswer.IsRightAnswer);
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            await this.service.DeleteAsync(this.firstAnswerId);

            var answersCount = this.dbContext.Answers.Where(x => !x.IsDeleted).ToArray().Count();
            var answer = await this.dbContext.Answers.FindAsync(this.firstAnswerId);
            Assert.Equal(0, answersCount);
            Assert.True(answer.IsDeleted);
        }

        private string CreateFirstAnswer()
        {
            var questionId = Guid.NewGuid().ToString();
            var answer = new Answer()
            {
                Text = "First answer",
                QuestionId = questionId,
                IsRightAnswer = true,
            };
            this.dbContext.Answers.Add(answer);
            this.dbContext.SaveChanges();
            this.dbContext.Entry<Answer>(answer).State = EntityState.Detached;
            return answer.Id;
        }
    }
}
