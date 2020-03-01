namespace QuizHut.Services.User
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using QuizHut.Data.Common.Repositories;
    using QuizHut.Data.Models;
    using QuizHut.Services.Mapping;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> repository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public UserService(
            IDeletableEntityRepository<ApplicationUser> repository,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            this.repository = repository;
            this.userManager = userManager;
            this.roleManager = roleManager;
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

        public async Task<bool> AssignRoleAsync(string email, string roleName)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            if (user != null)
            {
                await this.userManager.AddToRoleAsync(user, roleName);
                return true;
            }

            return false;
        }

        public async Task RemoveFromRoleAsync(string id, string roleName)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if (user != null)
            {
                await this.userManager.RemoveFromRoleAsync(user, roleName);
            }
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

        public async Task<IList<T>> GetAllByRoleAsync<T>(string roleName)
        {
            var role = await this.roleManager.FindByNameAsync(roleName);
            return await this.repository
                   .AllAsNoTracking()
                   .Where(x => x.Roles.Any(x => x.RoleId == role.Id))
                   .To<T>()
                   .ToListAsync();
        }
    }
}
