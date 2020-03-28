namespace QuizHut.Web.Infrastructure.Components
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Users;
    using QuizHut.Web.ViewModels.UsersInRole;

    [ViewComponent(Name = "UsersInRole")]
    public class UsersInRoleComponent : ViewComponent
    {
        private readonly IUsersService service;

        public UsersInRoleComponent(IUsersService service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string roleName, string invalidEmail = null)
        {
            var users = await this.service.GetAllByRoleAsync<UserInRoleViewModel>(roleName);
            var model = new UsersInRoleAllViewModel()
            {
                RoleName = roleName,
                Users = users,
                NewUser = new UserInputViewModel(),
            };

            if (invalidEmail != null)
            {
                model.NewUser.IsNotAdded = true;
                model.NewUser.Email = invalidEmail;
            }

            return this.View(model);
        }
    }
}
