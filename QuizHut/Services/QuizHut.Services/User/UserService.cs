namespace QuizHut.Services.User
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> repository;

        public UserService(IDeletableEntityRepository<ApplicationUser> repository)
        {
            this.repository = repository;
        }

        public async Task<bool> AddAsync(string email, string managerId)
        {
            var user = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                var manager = await this.repository
                    .AllAsNoTracking()
                    .Where(x => x.Id == managerId)
                    .FirstOrDefaultAsync();

                user.ManagerId = managerId;
                manager.Participants.Add(user);
                this.repository.Update(user);
                this.repository.Update(manager);
                await this.repository.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task DeleteAsync(string id, string managerId)
        {
            var participantToRemove = await this.repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            participantToRemove.ManagerId = null;
            this.repository.Update(participantToRemove);
            await this.repository.SaveChangesAsync();
        }

        public async Task<IList<T>> GetAllByUserIdAsync<T>(string id)
          => await this.repository
                .AllAsNoTracking()
                .Where(x => x.ManagerId == id)
                .To<T>()
                .ToListAsync();
    }
}
