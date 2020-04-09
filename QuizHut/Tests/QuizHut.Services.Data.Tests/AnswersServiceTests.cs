namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Models;
    using QuizHut.Services.Answers;
    using QuizHut.Web.ViewModels.Answers;
    using Xunit;

    public class AnswersServiceTests : BaseServiceTests
    {
        private IAnswersService Service => this.ServiceProvider.GetRequiredService<IAnswersService>();

        [Fact]
        public async Task CreateAnswerAsyncShouldCreateNewAnswerInDb()
        {
            var questionId = Guid.NewGuid().ToString();
            await this.Service.CreateAnswerAsync(answerText: "answer", isRightAnswer: false, questionId);

            var answer = await this.DbContext.Answers.FirstOrDefaultAsync();
            var answersCount = this.DbContext.Answers.ToArray().Count();

            Assert.Equal(1, answersCount);
            Assert.Equal("answer", answer.Text);
            Assert.Equal(questionId, answer.QuestionId);
            Assert.False(answer.IsRightAnswer);
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnCorrectModel()
        {
            var answer = await this.CreateAnswerAsync();

            var model = new AnswerViewModel()
            {
                Id = answer.Id,
                Text = answer.Text,
                QuestionId = answer.QuestionId,
                IsRightAnswer = answer.IsRightAnswer,
            };

            var resultModel = await this.Service.GetByIdAsync<AnswerViewModel>(answer.Id);

            Assert.Equal(model.Id, resultModel.Id);
            Assert.Equal(model.Text, resultModel.Text);
            Assert.True(resultModel.IsRightAnswer);
            Assert.Equal(model.QuestionId, resultModel.QuestionId);
        }

        [Fact]
        public async Task UpdateAsyncShouldUpdateCorrectly()
        {
            var answer = await this.CreateAnswerAsync();

            await this.Service.UpdateAsync(answer.Id, "First test answer", false);
            var updatedAnswer = await this.DbContext.Answers.FirstOrDefaultAsync(x => x.Id == answer.Id);

            Assert.Equal("First test answer", updatedAnswer.Text);
            Assert.False(updatedAnswer.IsRightAnswer);
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var answer = await this.CreateAnswerAsync();

            await this.Service.DeleteAsync(answer.Id);

            var answersCount = this.DbContext.Answers.Where(x => !x.IsDeleted).ToArray().Count();
            var deletedAnswer = await this.DbContext.Answers.FirstOrDefaultAsync(x => x.Id == answer.Id);
            Assert.Equal(0, answersCount);
            Assert.Null(deletedAnswer);
        }

        private async Task<Answer> CreateAnswerAsync()
        {
            var questionId = Guid.NewGuid().ToString();
            var answer = new Answer()
            {
                Text = "First answer",
                QuestionId = questionId,
                IsRightAnswer = true,
            };
            await this.DbContext.Answers.AddAsync(answer);
            await this.DbContext.SaveChangesAsync();
            this.DbContext.Entry<Answer>(answer).State = EntityState.Detached;
            return answer;
        }
    }
}
