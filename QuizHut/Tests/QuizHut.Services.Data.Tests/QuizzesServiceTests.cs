namespace QuizHut.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.Data.Models;
    using QuizHut.Services.Quizzes;
    using Xunit;

    public class QuizzesServiceTests : BaseServiceTests
    {
        private IQuizzesService Service => this.ServiceProvider.GetRequiredService<IQuizzesService>();

        [Fact]
        public async Task CreateQuizAsyncShouldCreateCorrectly()
        {
            var creatorId = Guid.NewGuid().ToString();
            var quizId = await this.Service.CreateQuizAsync("Quiz", "description", 30, creatorId, "123456789");

            var quiz = await this.DbContext.Quizzes.FirstOrDefaultAsync(x => x.Id == quizId);

            Assert.NotNull(quiz);
            Assert.Equal("Quiz", quiz.Name);
            Assert.Equal("description", quiz.Description);
            Assert.Equal(30, quiz.Timer);
            Assert.Equal(creatorId, quiz.CreatorId);
            Assert.Equal("123456789", quiz.Password.Content);
        }

        [Fact]
        public async Task CreateQuizAsyncShouldCreateNewPassword()
        {
            var quizId = await this.Service.CreateQuizAsync("Quiz", "description", 30, Guid.NewGuid().ToString(), "123456789");
            var password = await this.DbContext.Passwords.FirstOrDefaultAsync(x => x.QuizId == quizId);

            Assert.NotNull(password);
            Assert.Equal("123456789", password.Content);
        }

        [Fact]
        public async Task GetQuizIdByPasswordAsyncShouldReturnCorrectQuizId()
        {
            var quizId = await this.CreateQuizAsync("Quiz", null, "testpassword");

            var resultQuizId = await this.Service.GetQuizIdByPasswordAsync("testpassword");

            Assert.NotNull(resultQuizId);
            Assert.Equal(quizId, resultQuizId);
        }

        private async Task<string> CreateQuizAsync(string name, string creatorId = null, string password = null)
        {
            var quiz = new Quiz
            {
                Name = name,
                Description = null,
                Timer = null,
                CreatorId = creatorId ?? Guid.NewGuid().ToString(),
            };

            await this.DbContext.Quizzes.AddAsync(quiz);
            await this.DbContext.SaveChangesAsync();

            var passwordModel = new Password()
            {
                Content = password ?? "123456",
                QuizId = quiz.Id,
            };

            await this.DbContext.Passwords.AddAsync(passwordModel);

            quiz.PasswordId = passwordModel.Id;
            this.DbContext.Quizzes.Update(quiz);
            await this.DbContext.SaveChangesAsync();

            this.DbContext.Entry<Quiz>(quiz).State = EntityState.Detached;
            this.DbContext.Entry<Password>(passwordModel).State = EntityState.Detached;

            return quiz.Id;
        }
    }
}
