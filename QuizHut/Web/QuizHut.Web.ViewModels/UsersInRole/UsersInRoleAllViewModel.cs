namespace QuizHut.Web.ViewModels.UsersInRole
{
    using System.Collections.Generic;

    public class UsersInRoleAllViewModel
    {
        public UsersInRoleAllViewModel()
        {
            this.Users = new HashSet<UserInRoleViewModel>();
        }

        public string RoleName { get; set; }

        public UserInputViewModel NewUser { get; set; }

        public IEnumerable<UserInRoleViewModel> Users { get; set; }
    }
}
